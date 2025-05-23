﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentValidator.Core.DocumentProcessor
{
    public class LinkValidationResult
    {
        public string? Url { get; set; }
        public bool IsValid { get; set; }
        public int StatusCode { get; set; }
        public int PageNumber { get; set; }
        public string? LinkText { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
