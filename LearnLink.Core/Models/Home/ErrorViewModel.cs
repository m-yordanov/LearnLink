﻿namespace LearnLink.Core.Models.Home
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public int StatusCode { get; set; }
        
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
