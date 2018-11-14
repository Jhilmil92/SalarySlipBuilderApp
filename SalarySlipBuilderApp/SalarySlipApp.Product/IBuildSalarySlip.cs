using SalarySlipBuilderApp.SalarySlipBuilderApp.Classes;
using SalarySlipBuilderApp.SalarySlipBuilderApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalarySlipBuilderApp.SalarySlipApp.Product
{
    public interface IBuildSalarySlip
    {        
        void ComputeRules();
        void CreateTemplate(ICollection<Rules> userAdditionComponents, ICollection<Rules> userDeductionComponents);
        public string CreateTemplate(ICollection<Rules> employeePayDetails);
        bool CreateFileForTemplate(string templateData);
        bool SendEmail();
        bool DeleteFileAfterSendingEmail();
    }
}
