﻿namespace ReadySetSurgical.Models
{
    public class ErrorLog
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? FileName { get; set; }
    }
}
