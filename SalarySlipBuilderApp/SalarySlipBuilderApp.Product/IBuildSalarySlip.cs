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
        void ComputeRules(ICollection<Rules> userAdditionComponents, ICollection<Rules> userDeductionComponents);
        void CreateTemplate(ICollection<Rules> employeePayDetails);
        void CreateFileForTemplate(string templateContent);
        void SendEmail();
        void DeleteFileAfterSendingEmail();
        SalarySlipProduct GetSalarySlipProduct();
    }
}
