﻿namespace TenmoClient.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
    }

    /// <summary>
    /// Model to return upon successful login
    /// </summary>
    public class ReturnUser
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        //public string Role { get; set; }
        public string Token { get; set; }
    }
}
