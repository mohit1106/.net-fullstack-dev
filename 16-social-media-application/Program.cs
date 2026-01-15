using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.IO;

namespace MiniSocialMedia
{
    // ---------- Task 1: Custom Exception ----------
    public class SocialException : Exception
    {
        public SocialException(string message) : base(message) { }
        public SocialException(string message, Exception inner) : base(message, inner) { }
    }

    // ---------- Task 2: Posting Interface ----------
    public interface IPostable
    {
        void AddPost(string content);
        IReadOnlyList<Post> GetPosts();
    }

    // ---------- Task: Post class ----------
    public class Post
    {
        public User Author { get; }
        public string Content { get; }
        public DateTime CreatedAt { get; }

        // public constructor - default CreatedAt is UtcNow
        public Post(User author, string content)
        {
            if (author == null) throw new ArgumentNullException(nameof(author));
            Author = author;
            Content = content ?? throw new ArgumentNullException(nameof(content));
            CreatedAt = DateTime.UtcNow;
        }

        // internal constructor used when loading saved posts (preserves CreatedAt)
        internal Post(User author, string content, DateTime createdAt)
        {
            if (author == null) throw new ArgumentNullException(nameof(author));
            Author = author;
            Content = content ?? throw new ArgumentNullException(nameof(content));
            CreatedAt = createdAt;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            // Author representation and date
            sb.AppendLine($"{Author} • {CreatedAt.ToString("MMM dd HH:mm")}");
            // Content exactly as stored
            sb.AppendLine(Content);

            // Find hashtags
            var hashtagPattern = new Regex(@"#[A-Za-z]+");
            var matches = hashtagPattern.Matches(Content);
            if (matches.Count > 0)
            {
                sb.Append("Tags: ");
                sb.AppendJoin(", ", matches.Cast<Match>().Select(m => m.Value));
            }

            // Trim any trailing whitespace/newlines
            return sb.ToString().TrimEnd();
        }
    }

    // ---------- Task 3: User (partial) - core ----------
    public partial class User : IPostable, IComparable<User>
    {
        public string Username { get; init; }
        public string Email { get; init; }

        private readonly List<Post> _posts = new();
        private readonly HashSet<string> _following = new(StringComparer.OrdinalIgnoreCase);

        // Event for new posts
        public event Action<Post>? OnNewPost;

        public User(string username, string email)
        {
            // Step 2: Validate username
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be null or whitespace", nameof(username));

            // Step 3: Validate email basic pattern
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or whitespace", nameof(email));

            username = username.Trim();
            email = email.Trim().ToLowerInvariant();

            var emailPattern = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!emailPattern.IsMatch(email))
                throw new SocialException("Invalid email format");

            Username = username;
            Email = email;
        }

        // FOLLOW method
        public void Follow(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Target username required", nameof(username));

            if (string.Equals(username.Trim(), this.Username, StringComparison.OrdinalIgnoreCase))
                throw new SocialException("Cannot follow yourself");

            _following.Add(username.Trim()); // hashset will handle duplicates & case-insensitivity
        }

        // IsFollowing (lambda style)
        public bool IsFollowing(string username) => _following.Contains(username);

        // AddPost
        public void AddPost(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Post content cannot be empty");

            var trimmed = content.Trim();

            if (trimmed.Length > 280)
                throw new SocialException("Post too long (max 280 characters)");

            var post = new Post(this, trimmed);
            _posts.Add(post);

            // Trigger event safely
            OnNewPost?.Invoke(post);
        }

        // GetPosts: read-only view
        public IReadOnlyList<Post> GetPosts() => _posts.AsReadOnly();

        // CompareTo
        public int CompareTo(User? other)
        {
            if (other is null) return 1;
            return string.Compare(this.Username, other.Username, StringComparison.OrdinalIgnoreCase);
        }

        public override string ToString() => $"@{Username}";

        // Internal helper to add pre-built Post (used while loading from file)
        internal void AddPostInternal(Post p)
        {
            if (p == null) throw new ArgumentNullException(nameof(p));
            _posts.Add(p);
        }

        // Internal access for reflection-based helper (not exposed publicly)
    }

    // ---------- User (partial) - display extension ----------
    public partial class User
    {
        public string GetDisplayName()
        {
            return $"User: {Username} <{Email}>";
        }
    }

    // ---------- Task: Generic Repository ----------
    public class Repository<T> where T : class
    {
        private readonly List<T> _items = new();

        public void Add(T item) => _items.Add(item);

        public IReadOnlyList<T> GetAll() => _items.AsReadOnly();

        public T? Find(Predicate<T> match) => _items.Find(match);
    }

    // ---------- SocialUtils - DateTime extension ----------
    public static class SocialUtils
    {
        public static string FormatTimeAgo(this DateTime dt)
        {
            var diff = DateTime.UtcNow - dt;
            if (diff.TotalSeconds < 60) return "just now";
            if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes} min ago";
            if (diff.TotalHours < 24) return $"{(int)diff.TotalHours} h ago";
            return dt.ToString("MMM dd");
        }
    }

    // ---------- UserExtensions - helper (uses reflection to read private _following) ----------
    public static class UserExtensions
    {
        // Returns names of users this user is following
        public static IReadOnlyList<string> GetFollowingNames(this User user)
        {
            if (user == null) return Array.Empty<string>();

            // Access private field _following via reflection (assignment requires care)
            var field = typeof(User).GetField("_following", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field == null) return Array.Empty<string>();

            var set = field.GetValue(user) as HashSet<string>;
            if (set == null) return Array.Empty<string>();

            return set.ToList().AsReadOnly();
        }
    }

    // ---------- Program - application flow ----------
    public static class Program
    {
        private static readonly Repository<User> _users = new();
        private static User? _currentUser = null;
        private static readonly string _dataFile = "social-data.json";
        private static Action<Post>? _notificationHandler;

        public static void Main()
        {
            Console.Title = "MiniSocial - Console Edition";
            Console.WriteLine("=== MiniSocial ===");

            LoadData();

            while (true)
            {
                try
                {
                    if (_currentUser == null)
                        ShowLoginMenu();
                    else
                        ShowMainMenu();
                }
                catch (SocialException ex)
                {
                    ConsoleColorWrite(ConsoleColor.Red, $"Error: {ex.Message}");
                    if (ex.InnerException != null)
                        Console.WriteLine(" → " + ex.InnerException.Message);
                }
                catch (Exception ex)
                {
                    ConsoleColorWrite(ConsoleColor.Red, "An unexpected error occurred. See log for details.");
                    LogError(ex);
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey(intercept: true);
                Console.Clear();
            }
        }

        // ---------- Login / Register / Exit ----------
        private static void ShowLoginMenu()
        {
            Console.WriteLine("1) Register");
            Console.WriteLine("2) Login");
            Console.WriteLine("3) Exit");
            Console.Write("Choice: ");
            var choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    Register();
                    break;
                case "2":
                    Login();
                    break;
                case "3":
                    SaveData();
                    Environment.Exit(0);
                    break;
                default:
                    ConsoleColorWrite(ConsoleColor.Yellow, "Invalid choice.");
                    break;
            }
        }

        private static void Register()
        {
            Console.Write("Enter username: ");
            var username = Console.ReadLine();
            Console.Write("Enter email: ");
            var email = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email))
                throw new SocialException("Both username and email are required.");

            // Check exists
            var existing = _users.Find(u => string.Equals(u.Username, username.Trim(), StringComparison.OrdinalIgnoreCase));
            if (existing != null)
                throw new SocialException("Username already exists.");

            var user = new User(username, email);
            _users.Add(user);

            ConsoleColorWrite(ConsoleColor.Green, $"Welcome, {user.Username}!");
        }

        private static void Login()
        {
            Console.Write("Enter username: ");
            var username = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(username))
            {
                ConsoleColorWrite(ConsoleColor.Yellow, "Login cancelled.");
                return;
            }

            var user = _users.Find(u => string.Equals(u.Username, username.Trim(), StringComparison.OrdinalIgnoreCase));
            if (user == null)
            {
                throw new SocialException("User not found");
            }

            _currentUser = user;
            ConsoleColorWrite(ConsoleColor.Green, $"Logged in as {_currentUser.Username}");

            // Subscribe to all users' OnNewPost events to be notified when someone you follow posts.
            // Store handler so we can unsubscribe on logout.
            _notificationHandler = (post) =>
            {
                // Show notification only if current user follows the post's author (and it's not the current user's own post)
                if (_currentUser != null && _currentUser.IsFollowing(post.Author.Username))
                {
                    ConsoleColorWrite(ConsoleColor.Cyan, $"Notification: {post.Author} posted {post.CreatedAt.FormatTimeAgo()}");
                    Console.WriteLine(post.Content);
                }
            };

            foreach (var u in _users.GetAll())
            {
                // avoid duplicate attaching: remove then add
                u.OnNewPost -= _notificationHandler;
                u.OnNewPost += _notificationHandler;
            }
        }

        // ---------- Main Menu ----------
        private static void ShowMainMenu()
        {
            Console.WriteLine($"Logged in: {_currentUser!.GetDisplayName()}");
            Console.WriteLine("1) Post message");
            Console.WriteLine("2) View my posts");
            Console.WriteLine("3) View timeline");
            Console.WriteLine("4) Follow user");
            Console.WriteLine("5) List users");
            Console.WriteLine("6) Logout");
            Console.WriteLine("7) Exit and save");
            Console.Write("Choice: ");
            var choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    PostMessage();
                    break;
                case "2":
                    ShowPosts(_currentUser.GetPosts());
                    break;
                case "3":
                    ShowTimeline();
                    break;
                case "4":
                    FollowUser();
                    break;
                case "5":
                    ListUsers();
                    break;
                case "6":
                    Logout();
                    break;
                case "7":
                    SaveData();
                    Environment.Exit(0);
                    break;
                default:
                    ConsoleColorWrite(ConsoleColor.Yellow, "Invalid choice.");
                    break;
            }
        }

        private static void Logout()
        {
            if (_currentUser == null) return;

            // Unsubscribe notification handler from all users
            if (_notificationHandler != null)
            {
                foreach (var u in _users.GetAll())
                {
                    u.OnNewPost -= _notificationHandler;
                }

                _notificationHandler = null;
            }

            ConsoleColorWrite(ConsoleColor.Green, $"User {_currentUser.Username} logged out.");
            _currentUser = null;
        }

        // ---------- Posting ----------
        private static void PostMessage()
        {
            if (_currentUser == null) throw new SocialException("No user logged in");

            Console.Write("Write your post (leave blank to cancel): ");
            var content = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(content))
            {
                ConsoleColorWrite(ConsoleColor.Yellow, "Post cancelled.");
                return;
            }

            _currentUser.AddPost(content);
            ConsoleColorWrite(ConsoleColor.Green, "Post created.");
        }

        // ---------- Timeline ----------
        private static void ShowTimeline()
        {
            if (_currentUser == null) throw new SocialException("No user logged in");

            var timeline = new List<Post>();

            // Add current user's posts
            timeline.AddRange(_currentUser.GetPosts());

            // Add posts from followed users
            var followingNames = _currentUser.GetFollowingNames();
            if (followingNames.Any())
            {
                foreach (var name in followingNames)
                {
                    var other = _users.Find(u => string.Equals(u.Username, name, StringComparison.OrdinalIgnoreCase));
                    if (other != null)
                        timeline.AddRange(other.GetPosts());
                }
            }

            // Sort most recent first
            var ordered = timeline.OrderByDescending(p => p.CreatedAt).ToList();
            ShowPosts(ordered);
        }

        private static void ShowPosts(IEnumerable<Post> posts)
        {
            var list = posts.ToList();
            if (!list.Any())
            {
                ConsoleColorWrite(ConsoleColor.Yellow, "No posts to show.");
                return;
            }

            const int maxDisplay = 50;
            var display = list.Take(maxDisplay);
            foreach (var p in display)
            {
                Console.WriteLine(p.ToString());
                Console.WriteLine($"[{p.CreatedAt.FormatTimeAgo()}]");
                Console.WriteLine(new string('-', 40));
            }

            if (list.Count > maxDisplay)
                Console.WriteLine($"Showing {maxDisplay} of {list.Count} posts.");
        }

        // ---------- Follow ----------
        private static void FollowUser()
        {
            if (_currentUser == null) throw new SocialException("No user logged in");

            Console.Write("Enter username to follow (leave blank to cancel): ");
            var target = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(target))
            {
                ConsoleColorWrite(ConsoleColor.Yellow, "Follow cancelled.");
                return;
            }

            var targetUser = _users.Find(u => string.Equals(u.Username, target.Trim(), StringComparison.OrdinalIgnoreCase));
            if (targetUser == null)
                throw new SocialException("User to follow not found");

            _currentUser.Follow(target.Trim());
            ConsoleColorWrite(ConsoleColor.Green, $"Now following {target.Trim()}");
        }

        // ---------- List users ----------
        private static void ListUsers()
        {
            var all = _users.GetAll().OrderBy(u => u.Username, StringComparer.OrdinalIgnoreCase).ToList();
            if (!all.Any())
            {
                ConsoleColorWrite(ConsoleColor.Yellow, "No users registered yet.");
                return;
            }

            foreach (var u in all)
            {
                Console.WriteLine($"{u.Username} - {u.Email}");
            }
        }

        // ---------- Save / Load (JSON) ----------
        // We'll save DTOs to avoid serializing events / circular refs
        private class PostDto
        {
            public string Content { get; set; } = "";
            public DateTime CreatedAt { get; set; }
        }

        private class UserDto
        {
            public string Username { get; set; } = "";
            public string Email { get; set; } = "";
            public List<string> Following { get; set; } = new();
            public List<PostDto> Posts { get; set; } = new();
        }

        private static void SaveData()
        {
            try
            {
                var dtoList = new List<UserDto>();
                foreach (var u in _users.GetAll())
                {
                    var dto = new UserDto
                    {
                        Username = u.Username,
                        Email = u.Email,
                        Following = u.GetFollowingNames().ToList(),
                        Posts = u.GetPosts().Select(p => new PostDto { Content = p.Content, CreatedAt = p.CreatedAt }).ToList()
                    };
                    dtoList.Add(dto);
                }

                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(dtoList, options);
                File.WriteAllText(_dataFile, json);
                ConsoleColorWrite(ConsoleColor.Green, "Data saved.");
            }
            catch (Exception ex)
            {
                LogError(ex);
                ConsoleColorWrite(ConsoleColor.Red, "Failed to save data. See log.");
            }
        }

        private static void LoadData()
        {
            try
            {
                if (!File.Exists(_dataFile))
                {
                    Console.WriteLine("No data file found — starting fresh.");
                    return;
                }

                var json = File.ReadAllText(_dataFile);
                if (string.IsNullOrWhiteSpace(json))
                {
                    Console.WriteLine("Data file empty — starting fresh.");
                    return;
                }

                var dtoList = JsonSerializer.Deserialize<List<UserDto>>(json);
                if (dtoList == null)
                {
                    Console.WriteLine("No users in data file.");
                    return;
                }

                // First create user objects (without posts)
                foreach (var dto in dtoList)
                {
                    try
                    {
                        var user = new User(dto.Username, dto.Email);
                        _users.Add(user);
                    }
                    catch (SocialException se)
                    {
                        // Skip invalid user entries but log
                        LogError(se);
                    }
                }

                // Then add posts (so authors exist)
                foreach (var dto in dtoList)
                {
                    var user = _users.Find(u => string.Equals(u.Username, dto.Username, StringComparison.OrdinalIgnoreCase));
                    if (user == null) continue;

                    foreach (var pd in dto.Posts)
                    {
                        var post = new Post(user, pd.Content, pd.CreatedAt);
                        user.AddPostInternal(post);
                    }

                    // restore following set by simply Follow (which checks for self-follow)
                    foreach (var f in dto.Following)
                    {
                        try
                        {
                            // Add to set ignoring exceptions if self-follow in file
                            if (!string.Equals(f, user.Username, StringComparison.OrdinalIgnoreCase))
                                user.Follow(f);
                        }
                        catch (Exception ex)
                        {
                            LogError(ex);
                        }
                    }
                }

                ConsoleColorWrite(ConsoleColor.Green, $"Loaded { _users.GetAll().Count } users from {_dataFile}");
            }
            catch (Exception ex)
            {
                LogError(ex);
                ConsoleColorWrite(ConsoleColor.Red, "Failed to load data. Starting fresh.");
            }
        }

        // ---------- Logging ----------
        private static void LogError(Exception ex)
        {
            try
            {
                var logFile = "social-error.log";
                var sb = new StringBuilder();
                sb.AppendLine($"[{DateTime.UtcNow:O}] {ex.GetType().Name}: {ex.Message}");
                sb.AppendLine(ex.StackTrace ?? "");
                sb.AppendLine(new string('=', 80));
                File.AppendAllText(logFile, sb.ToString());
            }
            catch
            {
                // Logging must not crash the app.
            }
        }

        // ---------- Console color helper ----------
        private static void ConsoleColorWrite(ConsoleColor color, string message)
        {
            var original = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = original;
        }
    }
}
