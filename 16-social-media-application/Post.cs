using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace MiniSocialMedia
{
    public class Post
    {
        public User Author { get; }
        public string Content { get; }
        public DateTime CreatedAt { get; }

        public Post(User author, string content)
        {
            if (author is null) throw new ArgumentNullException(nameof(author));
            Author = author;
            Content = content ?? throw new ArgumentNullException(nameof(content));
            CreatedAt = DateTime.UtcNow;
        }

        internal Post(User author, string content, DateTime createdAt)
        {
            if (author is null) throw new ArgumentNullException(nameof(author));
            Author = author;
            Content = content ?? throw new ArgumentNullException(nameof(content));
            CreatedAt = createdAt;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Author.ToString());
            sb.Append(" • ");
            sb.Append(CreatedAt.ToLocalTime().ToString("MMM dd HH:mm"));
            sb.AppendLine();
            sb.AppendLine(Content);

            var regex = new Regex("#[A-Za-z]+", RegexOptions.Compiled);
            var matches = regex.Matches(Content);
            if (matches.Count > 0)
            {
                sb.Append("Tags: ");
                sb.AppendJoin(", ", matches.Cast<Match>().Select(m => m.Value));
            }

            return sb.ToString().TrimEnd();
        }
    }
}