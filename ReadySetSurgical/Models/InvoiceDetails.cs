﻿using Microsoft.VisualBasic;

namespace AWS_S3_Demo.Model
{
    public class InvoiceDetails
    {
        public int Id { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? VendorName { get; set; }
        public string? ReceiverName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? FileName { get; set; }
    }
}