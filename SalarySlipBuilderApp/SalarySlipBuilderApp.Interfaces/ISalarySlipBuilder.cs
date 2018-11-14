using SalarySlipBuilderApp.SalarySlipApp.Product;
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
        void PopulateMonths();
        void PopulateYears();
        void CollectEmployeeInformation();
        List<Rules> SegregateComponents();
        ICollection<Rules> FetchUserComponents();
        ICollection<Rules> ComputeRules();
        ICollection<Rules> PopulateGrid();
        string CollectTemplateData();
        string SendTemplate();
        void DeleteSalarySlips();
        SalarySlipProduct GetSalarySlipProduct();

    }
}
