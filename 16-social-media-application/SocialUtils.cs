using System;

namespace MiniSocialMedia
{
    public static class SocialUtils
    {
        public static string FormatTimeAgo(this DateTime dt)
        {
            var span = DateTime.UtcNow - dt;
            if (span.TotalSeconds < 60) return "just now";
            if (span.TotalMinutes < 60) return ((int)span.TotalMinutes) + " min ago";
            if (span.TotalHours < 24) return ((int)span.TotalHours) + " h ago";
            return dt.ToLocalTime().ToString("MMM dd");
        }
    }
}