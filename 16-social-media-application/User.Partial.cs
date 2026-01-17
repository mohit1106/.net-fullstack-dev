namespace MiniSocialMedia
{
    public partial class User
    {
        public string GetDisplayName() => $"User: {Username} <{Email}>";
    }
}
