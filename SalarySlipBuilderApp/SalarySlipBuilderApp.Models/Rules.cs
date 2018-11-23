using SalarySlipBuilderApp.SalarySlipBuilder.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalarySlipBuilderApp.SalarySlipBuilderApp.Models
{
    /// <summary>
    /// A class having properties that define a component's nature - The type of computation component (Addition/Subtraction),
    /// the name of the component and the it's value.
    /// </summary>
    public class Rules
    {
        public ComputationVariety ComputationName { get; set; }
        public string RuleName { get; set; }
        public decimal RuleValue { get; set; }
    }
}
