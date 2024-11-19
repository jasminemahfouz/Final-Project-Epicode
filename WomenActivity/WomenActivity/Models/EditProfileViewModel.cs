using System;
namespace WomenActivity.Models
{
    public class EditUserProfileViewModel
    {
        public UserProfile UserProfile { get; set; }

        // List of predefined profile photo options
        public List<string> AvailableProfilePhotos { get; set; } = new List<string>
        {
            "/images/profiles/photo1.jpg",
            "/images/profiles/photo2.jpg",
            "/images/profiles/photo3.jpg",
            "/images/profiles/photo4.jpg",
            "/images/profiles/photo5.jpg",
            "/images/profiles/photo6.jpg"
        };
    }
}

