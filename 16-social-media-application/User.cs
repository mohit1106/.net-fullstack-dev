using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MiniSocialMedia
{
    public partial class User : IPostable, IComparable<User>
    {
        public string Username { get; init; }
        public string Email { get; init; }

        private readonly List<Post> _posts = new();
        private readonly HashSet<string> _following = new(StringComparer.OrdinalIgnoreCase);

        public event Action<Post>? OnNewPost;

        internal IEnumerable<string> Following => _following;

        public User(string username, string email)
        {
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("username", nameof(username));
            if (string.IsNullOrWhiteSpace(email)) throw new SocialException("Invalid email format");

            var cleanedUsername = username.Trim();
            var cleanedEmail = email.Trim().ToLowerInvariant();

            var emailPattern = new Regex(@"^.+@.+\..+$");
            if (!emailPattern.IsMatch(cleanedEmail)) throw new SocialException("Invalid email format");

            Username = cleanedUsername;
            Email = cleanedEmail;
        }

        public void Follow(string username)
        {
            if (string.Equals(username, Username, StringComparison.OrdinalIgnoreCase))
                throw new SocialException("Cannot follow yourself");

            _following.Add(username);
        }

        public bool IsFollowing(string username) => _following.Contains(username);

        public void AddPost(string content)
        {
            if (string.IsNullOrWhiteSpace(content)) throw new ArgumentException("Post content cannot be empty");
            if (content.Length > 280) throw new SocialException("Post too long (max 280 characters)");

            var cleaned = content.Trim();
            var post = new Post(this, cleaned);
            _posts.Add(post);
            OnNewPost?.Invoke(post);
        }

        public IReadOnlyList<Post> GetPosts() => _posts.AsReadOnly();

        public int CompareTo(User? other)
        {
            if (other is null) return 1;
            return string.Compare(Username, other.Username, StringComparison.OrdinalIgnoreCase);
        }

        public override string ToString() => "@" + Username;
    }
}