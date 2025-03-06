﻿using Microsoft.AspNetCore.Identity;

namespace SocialNetwork1.Entities
{
    public class CustomIdentityUser : IdentityUser
    {
        public string? Image { get; set; }
        public bool IsOnline { get; set; }
        public DateTime DisconnectTime { get; set; } = DateTime.Now;
        public string? ConnectTime { get; set; } = "";

    }
}
