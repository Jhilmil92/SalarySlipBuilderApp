﻿using Humanizer;
using NReco.PdfGenerator;
using SalarySlipBuilderApp.SalarySlipApp.Common;
using SalarySlipBuilderApp.SalarySlipApp.Constants;
using SalarySlipBuilderApp.SalarySlipApp.Product;
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

namespace SalarySlipBuilderApp.SalarySlipBuilderApp.Classes
{
    public class FormInput:IBuildSalarySlip
    {
        private readonly InitialData _objEmployeeData;
        SalarySlipProduct _objSalarySlipProduct = new SalarySlipProduct();
        public FormInput(InitialData objEmployeeData)
        {
            _objEmployeeData = objEmployeeData;
        }
        public void ComputeRules(ICollection<Rules> userAdditionComponents, ICollection<Rules> userDeductionComponents)
        {
            decimal grossSalary = 0.0m;
            decimal componentAmount = 0.0m;
            StringBuilder componentValueAsString = new StringBuilder();
            ICollection<Rules> computedRules = new List<Rules>();
            decimal salary = Convert.ToDecimal(_objEmployeeData.Salary);
            var additionSectionCollection = ConfigurationManager.GetSection(Constants.additionSection) as NameValueCollection;
            if ((additionSectionCollection != null) && (additionSectionCollection.Count > 0))
            {

                foreach (var component in additionSectionCollection.AllKeys)
                {
                    if (component != Constants.balance)
                    {
                        //Check percent or actual amount
                        if (additionSectionCollection[component].EndsWith("%"))
                        {
                            componentValueAsString.Append(additionSectionCollection[component]);
                            componentAmount = Convert.ToDecimal(componentValueAsString.Remove(componentValueAsString.Length - 1, 1).ToString());
                            componentAmount = Decimal.Round(((componentAmount) / 100) * salary, 2);
                        }
                        computedRules.Add(new Rules
                        {
                            ComputationName = ComputationVariety.ADDITION,
                            RuleName = component,
                            RuleValue = componentAmount
                        });
                        grossSalary += componentAmount;
                        componentValueAsString.Clear();
                    }

                }

                if (computedRules != null && computedRules.Count > 0)
                {
                    decimal remainingSalary = salary - grossSalary;
                    computedRules.Add(new Rules
                    {
                        ComputationName = ComputationVariety.ADDITION,
                        RuleName = Constants.balance,
                        RuleValue = remainingSalary
                    });
                    grossSalary += remainingSalary;
                }

                //Add additional components to the gross salary.
                if ((userAdditionComponents != null) && (userAdditionComponents.Count > 0))
                {
                    decimal userAdditionComponentTotal = 0.0m;
                    foreach (var component in userAdditionComponents)
                    {
                        computedRules.Add(new Rules
                        {
                            ComputationName = component.ComputationName,
                            RuleName = component.RuleName,
                            RuleValue = component.RuleValue
                        }
                            );
                        userAdditionComponentTotal += component.RuleValue;
                    }
                    grossSalary += userAdditionComponentTotal;
                }


            }

            var subtractionSectionCollection = ConfigurationManager.GetSection(Constants.subtractionSection) as NameValueCollection;
            if ((subtractionSectionCollection != null) && (subtractionSectionCollection.Count > 0))
            {
                foreach (var component in subtractionSectionCollection.AllKeys)
                {
                    if (subtractionSectionCollection[component].EndsWith("%"))
                    {
                        componentValueAsString.Append(subtractionSectionCollection[component]);
                        componentAmount = Convert.ToDecimal(componentValueAsString.Remove(componentValueAsString.Length - 1, 1).ToString());
                        componentAmount = Decimal.Round(((componentAmount) / 100) * grossSalary, 2);
                    }
                    computedRules.Add(new Rules
                    {
                        ComputationName = ComputationVariety.SUBTRACTION,
                        RuleName = component,
                        RuleValue = componentAmount
                    });
                    componentValueAsString.Clear();
                }

                if ((userDeductionComponents != null) && (userDeductionComponents.Count > 0))
                {
                    decimal userDeductionComponentTotal = 0.0m;
                    foreach (var component in userDeductionComponents)
                    {
                        computedRules.Add(new Rules
                        {
                            ComputationName = component.ComputationName,
                            RuleName = component.RuleName,
                            RuleValue = component.RuleValue
                        });
                        userDeductionComponentTotal += component.RuleValue;
                    }
                }
            }
            _objSalarySlipProduct.ComputeRules = computedRules;
        }
        public void CreateTemplate(ICollection<Rules> employeePayDetails)
        {
            int beginCounter = -1;
            int endCounter = -1;
            int largerListCount = 0;
            string templateBody = string.Empty;
            StringBuilder genericBuilder = new StringBuilder();

            using (ITemplateProvider templateApplication = new TemplateProvider())
            {
                templateBody = templateApplication.SupplyTemplateStream().ReadToEnd();
            }

            templateBody = templateBody.Replace("$dateOfJoining", _objEmployeeData.DateOfJoining);
            templateBody = templateBody.Replace("$panNumber", _objEmployeeData.PanNumber);
            templateBody = templateBody.Replace("$name", _objEmployeeData.EmployeeName);
            templateBody = templateBody.Replace("$designation", _objEmployeeData.Designation);
            templateBody = templateBody.Replace("$accountNumber", _objEmployeeData.AccountNumber);
            templateBody = templateBody.Replace("$salary", _objEmployeeData.Salary);
            templateBody = templateBody.Replace("$month", _objEmployeeData.Month.ToUpper());
            templateBody = templateBody.Replace("$year", _objEmployeeData.Year);

            //New set of code -- Start.

            var additionPayDetails = employeePayDetails.Where(a => (a.ComputationName == ComputationVariety.ADDITION) && (a.RuleName != Constants.netPay && a.RuleName != Constants.additionTotal)).ToArray();
            var deductionPayDetails = employeePayDetails.Where(a => (a.ComputationName == ComputationVariety.SUBTRACTION) && (a.RuleName != Constants.subtractionTotal)).ToArray();
            var additionTotal = employeePayDetails.Where(a => (a.ComputationName == ComputationVariety.ADDITION) && (a.RuleName == Constants.additionTotal)).ToArray();
            var deductionTotal = employeePayDetails.Where(a => (a.ComputationName == ComputationVariety.SUBTRACTION) && (a.RuleName == Constants.subtractionTotal)).ToArray();


            if ((additionPayDetails != null && additionPayDetails.Count() > 0) && (deductionPayDetails != null && deductionPayDetails.Count() > 0))
            {
                largerListCount = (additionPayDetails.Count() > deductionPayDetails.Count()) ? additionPayDetails.Count() : deductionPayDetails.Count();
            }

            for (int i = 0; i < largerListCount; i++)
            {
                if (beginCounter == -1)
                {
                    genericBuilder.Append("<tr class=\"alignment-style\">");
                    beginCounter++;
                }
                if ((beginCounter == 0))
                {
                    if (i < additionPayDetails.Count() && i < deductionPayDetails.Count())
                    {
                        if (additionPayDetails[i] != null && deductionPayDetails[i] != null)
                        {
                            genericBuilder.Append(string.Format("<td colspan = \"1\">{0}</td><td colspan = \"1\">{1}</td><td colspan = \"1\">{2}</td><td colspan = \"1\">{3}</td></tr>", additionPayDetails[i].RuleName, additionPayDetails[i].RuleValue, deductionPayDetails[i].RuleName, deductionPayDetails[i].RuleValue));

                            beginCounter = -1;
                            endCounter++;
                        }
                        else if (additionPayDetails[i] != null && deductionPayDetails[i] == null)
                        {
                            genericBuilder.Append(string.Format("<td colspan = \"1\">{0}</td><td colspan = \"1\">{1}</td><td colspan = \"1\">{2}</td><td colspan = \"1\">{2}</td></tr>", additionPayDetails[i].RuleName, additionPayDetails[i].RuleValue, string.Empty));
                            beginCounter = -1;
                            endCounter++;
                        }
                        else if (additionPayDetails[i] == null && deductionPayDetails[i] != null)
                        {
                            genericBuilder.Append(string.Format("<td colspan = \"1\">{0}</td><td colspan = \"1\">{0}</td><td colspan = \"2\">{1}</td><td colspan = \"1\">{2}</td></tr>", string.Empty, deductionPayDetails[i].RuleName, deductionPayDetails[i].RuleValue));
                            beginCounter = -1;
                            endCounter++;
                        }
                    }
                    else if (i < additionPayDetails.Count() && i == deductionPayDetails.Count())
                    {
                        if (additionPayDetails[i] != null)
                        {
                            genericBuilder.Append(string.Format("<td colspan = \"1\">{0}</td><td colspan = \"1\">{1}</td><td colspan = \"1\">{2}</td><td colspan = \"1\">{2}</td></tr>", additionPayDetails[i].RuleName, additionPayDetails[i].RuleValue, string.Empty));
                            beginCounter = -1;
                            endCounter++;
                        }
                    }
                    else if (i == additionPayDetails.Count() && i < deductionPayDetails.Count())
                    {
                        if (deductionPayDetails[i] != null)
                        {
                            genericBuilder.Append(string.Format("<td colspan = \"1\">{0}</td><td colspan = \"1\">{0}</td><td colspan = \"1\">{1}</td><td colspan = \"1\">{2}</td></tr>", string.Empty, deductionPayDetails[i].RuleName, deductionPayDetails[i].RuleValue));
                            beginCounter = -1;
                            endCounter++;
                        }
                    }

                }
            }
            if (endCounter == largerListCount - 1)
            {
                if ((additionTotal != null && additionTotal.Count() > 0) && (deductionTotal != null && deductionTotal.Count() > 0))
                {
                    genericBuilder.Append(string.Format("<tr class=\"alignment-style\"><td colspan = \"1\">{0}</td><td colspan = \"1\">{1}</td><td colspan = \"1\">{2}</td><td colspan = \"1\">{3}</td></tr>", Constants.grossSalary, additionTotal[0].RuleValue, Constants.totalDeduction, deductionTotal[0].RuleValue));
                }
                else if ((additionTotal != null && additionTotal.Count() > 0) && (deductionTotal == null || deductionTotal.Count() == 0))
                {
                    genericBuilder.Append(string.Format("<tr class=\"alignment-style\"><td colspan = \"1\">{0}</td><td colspan = \"1\">{1}</td><td colspan = \"1\">{2}</td><td colspan = \"1\">{2}</td></tr>", Constants.grossSalary, additionPayDetails[0].RuleValue, string.Empty));
                }
                else if ((additionTotal == null || additionTotal.Count() == 0) && (deductionTotal != null && deductionTotal.Count() > 0))
                {
                    genericBuilder.Append(string.Format("<tr class=\"alignment-style\"><td colspan = \"1\">{0}</td><td colspan = \"1\">{0}</td><td colspan = \"1\">{1}</td><td colspan = \"1\">{2}</td></tr>", string.Empty, Constants.totalDeduction, deductionPayDetails[0].RuleValue));
                }
            }


            if (genericBuilder != null && genericBuilder.Length > 0)
            {
                templateBody = templateBody.Replace("$additionAndDeductionComponents", genericBuilder.ToString());
            }
            else
            {
                templateBody = templateBody.Replace("$additionAndDeductionComponents", string.Empty);
            }
            genericBuilder.Clear();
            //New set of code -- Stop.


            var details = employeePayDetails.Where(a => a.RuleName == Constants.netPay).Select(a => a).ToList();
            var ruleValue = details[0].RuleValue.ToString("#,#.##", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
            var ruleValueinDecimal = Convert.ToDecimal(ruleValue);
            genericBuilder.Append(string.Format("<tr class=\"alignment-style\"><td colspan=\"3\">{0}:</td><td colspan=\"1\">{1}</td></tr>", details[0].RuleName, ruleValue));
            templateBody = templateBody.Replace("$netPay", genericBuilder.ToString());
            genericBuilder.Clear();
            var value = (NumberToWordsExtension.ToWords((long)ruleValueinDecimal)).Titleize();
            genericBuilder.Append(string.Format("<tr><td colspan=\"1\" class=\"left-alignment-style\">Net Pay in Words:</td><td colspan=\"3\" class=\"alignment-style-center\">{0}</td></tr>", value));
            templateBody = templateBody.Replace("$payInWords", genericBuilder.ToString());
            genericBuilder.Clear();
            templateBody = templateBody.Replace("$contentOfHeader", string.Format("<img src=\"{0}\" alt=\"{1}\">", ConfigurationManager.AppSettings[Constants.headerImage], "No Image Found"));
            templateBody = templateBody.Replace("$contentOfFooter", HelperMethods.FetchFooterContent() != null ? HelperMethods.FetchFooterContent() : string.Empty);
            _objSalarySlipProduct.CreateTemplate = templateBody;
        }
        public void CreateFileForTemplate(string templateContent)
        {
            string pdfFilePath = @"e:\SalarySlips\";
            string pdfFileName = string.Format("{0}{1:dd-MMM-yyyy HH-mm-ss-fff}{2}", "SalarySlip", DateTime.Now, ".pdf");
            string finalPdfPath = Path.Combine(pdfFilePath, pdfFileName);
            _objSalarySlipProduct.CreateFileForTemplate = HelperMethods.HtmlToPdfConverter(pdfFilePath, pdfFileName, finalPdfPath, templateContent);
            _objSalarySlipProduct.FullPdfPath = finalPdfPath; 
        }
        public void SendEmail()
        {
            bool isMailSent = false;
            string senderID = "jhilmil.basu92@gmail.com";
            string senderPassword = "Jhilmil@12111992";
            RemoteCertificateValidationCallback orgCallback = ServicePointManager.ServerCertificateValidationCallback;
            string body = "Test";
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(HelperMethods.OnValidateCertificate);
                ServicePointManager.Expect100Continue = true;
                MailMessage mail = new MailMessage();

                Attachment attachment = new Attachment(_objSalarySlipProduct.FullPdfPath, MediaTypeNames.Application.Octet);
                ContentDisposition disposition = attachment.ContentDisposition;
                disposition.CreationDate = File.GetCreationTime(_objSalarySlipProduct.FullPdfPath);
                disposition.ModificationDate = File.GetLastWriteTime(_objSalarySlipProduct.FullPdfPath);
                disposition.ReadDate = File.GetLastAccessTime(_objSalarySlipProduct.FullPdfPath);
                disposition.FileName = Path.GetFileName(_objSalarySlipProduct.FullPdfPath);
                disposition.Size = new FileInfo(_objSalarySlipProduct.FullPdfPath).Length;
                disposition.DispositionType = DispositionTypeNames.Attachment;
                mail.Attachments.Add(attachment);

                mail.To.Add(_objEmployeeData.EmailId);
                mail.From = new MailAddress(senderID);
                mail.Subject = "My Test Email!";
                mail.Body = "Salary Slip";
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Credentials = new System.Net.NetworkCredential(senderID, senderPassword);
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.Send(mail);
                isMailSent = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            _objSalarySlipProduct.SendEmail = isMailSent;
        }
        public void DeleteFileAfterSendingEmail()
        {
            _objSalarySlipProduct.IsFileDeleted = HelperMethods.DeleteSalarySlips(_objEmployeeData.TempPdfFilePath);
        }
        public SalarySlipProduct GetSalarySlipProduct()
        {
            return _objSalarySlipProduct;
        }
    }
}
