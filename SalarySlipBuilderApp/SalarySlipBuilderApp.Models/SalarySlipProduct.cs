﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalarySlipBuilderApp.SalarySlipBuilderApp.Models
{
    class SalarySlipProduct
    {
        public ICollection<Models.Rules> ComputeRules { get; set; }
        public string CreateTemplate { get; set; }
        public bool CreateFileForTemplate { get; set; }
        public bool SendEmail { get; set; }
    }
}
