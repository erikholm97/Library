﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Library
{
    public class Employees
    {
        public int Id { get; set; }
        [MaxLength(500)]
        [Required]
        public string FirstName { get; set; }
        [MaxLength(500)]
        [Required]
        public string LastName { get; set; }

        public decimal Salary { get; set; }

        public bool IsCEO { get; set; }

        public bool IsManager { get; set; }

        public int? ManagerId { get; set; }
    }
}
