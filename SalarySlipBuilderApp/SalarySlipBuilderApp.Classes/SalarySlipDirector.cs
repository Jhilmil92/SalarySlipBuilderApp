using SalarySlipBuilderApp.SalarySlipApp.Product;
using SalarySlipBuilderApp.SalarySlipBuilderApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalarySlipBuilderApp.SalarySlipBuilderApp.Classes
{
    public class SalarySlipDirector
    {
        private readonly IBuildSalarySlip _objectBuilder;
        private readonly InitialData _initialData;

        public SalarySlipDirector(IBuildSalarySlip objectBuilder, InitialData initialData)
        {
            _objectBuilder = objectBuilder;
            _initialData = initialData;
        }

        public void CreateSalarySlip()
        {
            _objectBuilder.ComputeRules(_initialData.UserAdditionComponents, _initialData.UserDeductionComponents);
            _objectBuilder.CreateTemplate(_initialData.EmployeePayDetails);
            _objectBuilder.CreateFileForTemplate(_initialData.TemplateContent);
            _objectBuilder.SendEmail();
            _objectBuilder.DeleteFileAfterSendingEmail();
        }

        public SalarySlipProduct GetSalarySlip()
        {
            return _objectBuilder.GetSalarySlipProduct();
        }
    }
}
