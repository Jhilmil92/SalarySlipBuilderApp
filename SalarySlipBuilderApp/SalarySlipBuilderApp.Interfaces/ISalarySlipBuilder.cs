﻿using SalarySlipBuilderApp.SalarySlipApp.Product;
using SalarySlipBuilderApp.SalarySlipBuilderApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalarySlipBuilderApp.SalarySlipBuilderApp.Interfaces
{
    public interface ISalarySlipBuilder
    {
        void ComputeRules(ICollection<Rules> userAdditionComponents, ICollection<Rules> userDeductionComponents);
        void CreateTemplate(ICollection<Rules> employeePayDetails);
        void CreateFileForTemplate(string templateContent);
        void SendEmail();
        bool DeleteFileAfterSendingEmail();
        SalarySlipProduct GetSalarySlipProduct();
    }
}
