using System.Collections.Generic;

namespace MiniSocialMedia
{
    public interface IPostable
    {
        void AddPost(string content);
        IReadOnlyList<Post> GetPosts();
    }
}
