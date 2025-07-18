﻿using System.ComponentModel.DataAnnotations;

namespace HIV.DTOs
{
    public class DTOUpdate
    {

        [Required]
        public string full_name { get; set; }

        [Required]
        [Phone]
        public string phone { get; set; }

        [Required]
        public string gender { get; set; }

        public DateOnly? birthdate { get; set; }
        public string role { get; set; }

        public string address { get; set; }

        public string? user_avatar { get; set; }
    }
}
