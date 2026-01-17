using System.Collections.Generic;
using System.Linq;

namespace MiniSocialMedia
{
    public static class UserExtensions
    {
        public static IEnumerable<string> GetFollowingNames(this User user) => user.Following.ToList();
    }
}
