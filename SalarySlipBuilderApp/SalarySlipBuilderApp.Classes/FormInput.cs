using Humanizer;
using NReco.PdfGenerator;
using SalarySlipBuilderApp.Models;
using SalarySlipBuilderApp.SalarySlipBuilderApp.Common;
using SalarySlipBuilderApp.SalarySlipBuilderApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Net.Security;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using TemplateApp.TemplateApp.Classes;
using TemplateApp.TemplateApp.Interfaces;

namespace SalarySlipBuilderApp.Classes
{
    public class FormInput:SalarySlip
    {
        InitialData _objInitialData;
        public FormInput(InitialData objInitialData)
            : base(objInitialData)
        {
            this._objInitialData = objInitialData;
        }        
    }
}
