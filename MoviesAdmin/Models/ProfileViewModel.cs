namespace MoviesAdmin.ViewModels
{
    public class ProfileViewModel
    {
        public string? Email { get; set; }

        public string? Nickname { get; set; }

        public string? ProfilePicture { get; set; }

        public string? Role { get; set; }

        public bool EmailVerified { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}