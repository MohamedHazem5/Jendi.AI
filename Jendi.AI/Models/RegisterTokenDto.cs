﻿namespace Jendi.AI.Models
{
    public class RegisterTokenDto
    {
        public string profileToken { get; set; }
        public string expiresIn { get; set; }
        public string tokenType { get; set; }
        public string refreshToken { get; set; }
    }
}
