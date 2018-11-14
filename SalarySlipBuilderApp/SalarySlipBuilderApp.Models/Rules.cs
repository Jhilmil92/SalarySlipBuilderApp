using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalarySlipBuilderApp.SalarySlipBuilderApp.Models
{
    public class Rules
    {
        public SalarySlip.Common.ComputationVariety ComputationName { get; set; }
        public string RuleName { get; set; }
        public decimal RuleValue { get; set; }
    }
}
