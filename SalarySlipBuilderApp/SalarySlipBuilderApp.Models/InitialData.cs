using SalarySlipBuilderApp.SalarySlipBuilderApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalarySlipBuilderApp.SalarySlipBuilderApp.Classes
{
    public class InitialData
    {
        public string EmployeeName { get; set; }
        public string DateOfJoining { get; set; }
        public string PanNumber { get; set; }
        public string AccountNumber { get; set; }
        public string Designation { get; set; }
        public string Salary { get; set; }
        public string EmailId { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string TempPdfFilePath { get; set; }
        public string TemplateContent { get; set; }
        public ICollection<Rules> UserAdditionComponents { get; set; }
        public ICollection<Rules> UserDeductionComponents { get; set; }
        public ICollection<Rules> EmployeePayDetails { get; set; }
    }
}
