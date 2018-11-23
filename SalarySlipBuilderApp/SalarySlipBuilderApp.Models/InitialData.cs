using SalarySlipBuilderApp.SalarySlipBuilderApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalarySlipBuilderApp.Models
{
    /// <summary>
    /// A class having properties that define the employee data and necessary values required to carry out 
    /// the salary slip computation process.
    /// </summary>
    public class InitialData
    {
        public string AssociateCode { get; set; }
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
        public string FullPdfPath { get; set; }
        public string PdfFilePath { get; set; }
        public bool CreateFileForTemplate { get; set; }
        public ICollection<Rules> UserAdditionComponents { get; set; }
        public ICollection<Rules> UserDeductionComponents { get; set; }
        public ICollection<Rules> EmployeePayDetails { get; set; }
        public ICollection<Rules> ComputedRules { get; set; }
    }
}
