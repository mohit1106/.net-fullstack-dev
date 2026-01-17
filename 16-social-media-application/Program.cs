using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;

using MiniSocialMedia;

class Program
{
    private static readonly Repository<User> _users = new();
    private static User? _currentUser;
    private static readonly string _dataFile = "social-data.json";
    private static readonly string _logFile = "error.log";

    static void Main()
    {
        Console.Title = "MiniSocial - Console Edition";
        Console.WriteLine("=== MiniSocial ===");
        LoadData();

        while (true)
        {
            try
            {
                if (_currentUser is null) ShowLoginMenu(); else ShowMainMenu();
            }
            catch (SocialException ex)
            {
                var prev = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {ex.Message}");
                if (ex.InnerException != null) Console.WriteLine(ex.InnerException.Message);
                Console.ForegroundColor = prev;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected Error!!");
                Console.WriteLine(ex.ToString());
                LogError(ex);
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
            Console.Clear();
        }
    }

    private static void ShowLoginMenu()
    {
        Console.WriteLine("1. Register\n2. Login\n0. Exit");
        var ch = Console.ReadLine()?.Trim();
        switch (ch)
        {
            case "1": Register(); break;
            case "2": Login(); break;
            case "0": SaveData(); Environment.Exit(0); break;
            default: Console.WriteLine("Invalid choice"); break;
        }
    }

    private static void Register()
    {
        Console.Write("Username: ");
        var username = Console.ReadLine();
        Console.Write("Email: ");
        var email = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email))
        {
            Console.WriteLine("Both fields are required.");
            return;
        }

        if (_users.Find(u => string.Equals(u.Username, username.Trim(), StringComparison.OrdinalIgnoreCase)) is not null)
        {
            Console.WriteLine("Username already exists.");
            return;
        }

        var user = new User(username, email);
        _users.Add(user);
        Console.WriteLine($"Welcome {user.Username}!");
    }

    static void Login()
    {
        Console.Write("Username: ");
        var name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name)) { Console.WriteLine("Invalid"); return; }
        var user = _users.Find(u => string.Equals(u.Username, name.Trim(), StringComparison.OrdinalIgnoreCase));
        if (user is null) { Console.WriteLine("User not found."); return; }
        _currentUser = user;
        _currentUser.OnNewPost += NotifyPost;
        Console.WriteLine($"Logged in as {_currentUser.Username}!");
    }

    private static void NotifyPost(Post p)
    {
        var prev = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Cyan;
        var preview = p.Content.Length > 40 ? p.Content.Substring(0, 40) + "..." : p.Content;
        Console.WriteLine($"[New post] {p.Author} : {preview}");
        Console.ForegroundColor = prev;
    }

    private static void ShowMainMenu()
    {
        Console.WriteLine($"Logged in: {_currentUser}");
        Console.WriteLine("1.Post message\n2.View my posts\n3.View timeline\n4.Follow user\n5.List users\n6.Logout\n0.Exit and save");
        var ch = Console.ReadLine()?.Trim();
        switch (ch)
        {
            case "1": PostMessage(); break;
            case "2": ShowPosts(_currentUser!.GetPosts()); break;
            case "3": ShowTimeline(); break;
            case "4": FollowUser(); break;
            case "5": ListUsers(); break;
            case "6": Logout(); break;
            case "0": SaveData(); Environment.Exit(0); break;
            default: Console.WriteLine("Invalid choice"); break;
        }
    }

    private static void PostMessage()
    {
        if (_currentUser is null) return;
        Console.WriteLine("Write your post (max 280 chars). Empty to cancel:");
        var content = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(content)) { Console.WriteLine("Post cancelled."); return; }
        try { _currentUser.AddPost(content.Trim()); Console.WriteLine("Posted."); }
        catch (Exception ex) { throw; }
    }

    static void ShowTimeline()
    {
        if (_currentUser is null) return;
        var timeline = new List<Post>();
        timeline.AddRange(_currentUser.GetPosts());
        foreach (var name in _currentUser.GetFollowingNames())
        {
            var other = _users.Find(u => string.Equals(u.Username, name, StringComparison.OrdinalIgnoreCase));
            if (other is null) continue;
            timeline.AddRange(other.GetPosts());
        }
        var sorted = timeline.OrderByDescending(p => p.CreatedAt).ToList();
        Console.WriteLine("=== Your Timeline ===");
        ShowPosts(sorted);
    }

    private static void ShowPosts(IEnumerable<Post> posts)
    {
        var list = posts.Take(20).ToList();
        if (!list.Any()) { Console.WriteLine("No posts yet."); return; }
        int counter = 0;
        foreach (var p in list)
        {
            Console.WriteLine(p.ToString());
            Console.WriteLine(p.CreatedAt.FormatTimeAgo());
            Console.WriteLine(new string('-', 30));
            counter++;
        }
    }

    private static void FollowUser()
    {
        if (_currentUser is null) return;
        Console.Write("Enter username to follow: ");
        var name = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(name)) { Console.WriteLine("Cancelled."); return; }
        if (string.Equals(name, _currentUser.Username, StringComparison.OrdinalIgnoreCase)) { Console.WriteLine("You cannot follow yourself."); return; }
        var exists = _users.Find(u => string.Equals(u.Username, name, StringComparison.OrdinalIgnoreCase));
        if (exists is null) { Console.WriteLine("User not found."); return; }
        try { _currentUser.Follow(name); Console.WriteLine($"Now following @{name}"); }
        catch (SocialException ex) { throw; }
    }

    private static void ListUsers()
    {
        Console.WriteLine("Registered users:");
        var all = _users.GetAll().OrderBy(u => u).ToList();
        foreach (var u in all)
        {
            Console.WriteLine($"{u.ToString(),-20} {u.Email}");
        }
    }

    private static void Logout()
    {
        if (_currentUser is null) return;
        _currentUser.OnNewPost -= NotifyPost;
        _currentUser = null;
        Console.WriteLine("Logged out.");
    }

    private static void SaveData()
    {
        try
        {
            var payload = _users.GetAll().Select(u => new
            {
                u.Username,
                u.Email,
                Following = u.GetFollowingNames().ToArray(),
                Posts = u.GetPosts().Select(p => new { p.Content, p.CreatedAt }).ToArray()
            }).ToArray();

            var opts = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(_dataFile, JsonSerializer.Serialize(payload, opts));
            Console.WriteLine("Data saved.");
        }
        catch (Exception ex)
        {
            LogError(ex);
            Console.WriteLine("Failed to save data.");
        }
    }

    private static void LoadData()
    {
        try
        {
            if (!File.Exists(_dataFile)) return;
            var raw = File.ReadAllText(_dataFile);
            if (string.IsNullOrWhiteSpace(raw)) return;
            var doc = JsonDocument.Parse(raw);
            foreach (var el in doc.RootElement.EnumerateArray())
            {
                var username = el.GetProperty("Username").GetString() ?? string.Empty;
                var email = el.GetProperty("Email").GetString() ?? string.Empty;
                var user = new User(username, email);
                _users.Add(user);
                if (el.TryGetProperty("Following", out var f))
                {
                    foreach (var fn in f.EnumerateArray()) user.Follow(fn.GetString() ?? string.Empty);
                }
                if (el.TryGetProperty("Posts", out var ps))
                {
                    foreach (var pj in ps.EnumerateArray())
                    {
                        var content = pj.GetProperty("Content").GetString() ?? string.Empty;
                        var created = pj.GetProperty("CreatedAt").GetDateTime();
                        var post = new Post(user, content, created);
                        // add directly to internal list via reflection? internal access: use AddPost by setting created time via internal constructor and an internal method
                        var list = typeof(User).GetField("_posts", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        if (list?.GetValue(user) is List<Post> lst) lst.Add(post);
                    }
                }
            }
            Console.WriteLine("Data loaded.");
        }
        catch (Exception ex)
        {
            LogError(ex);
            Console.WriteLine("Failed to load data.");
        }
    }

    private static void LogError(Exception ex)
    {
        try
        {
            var entry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {ex.Message}{Environment.NewLine}{ex.StackTrace}{Environment.NewLine}-----{Environment.NewLine}";
            File.AppendAllText(_logFile, entry);
        }
        catch { }
    }
}
