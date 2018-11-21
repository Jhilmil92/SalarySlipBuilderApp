using Humanizer;
using SalarySlipBuilderApp.SalarySlipBuilder.Common;
using SalarySlipBuilderApp.SalarySlipBuilderApp.Common;
using SalarySlipBuilderApp.SalarySlipBuilderApp.Classes;
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
using System.Text;
using System.Threading.Tasks;
using TemplateApp.TemplateApp.Classes;
using TemplateApp.TemplateApp.Interfaces;

namespace SalarySlipBuilderApp.Models
{
    public abstract class SalarySlip
    {
        private InitialData _objInitialData;
        public SalarySlip() { }
        public SalarySlip(InitialData objInitialData)
        {
            this._objInitialData = objInitialData;
        }
                
        public void SalarySlipProcess()
        {
            ComputeRules(_objInitialData.UserAdditionComponents, _objInitialData.UserDeductionComponents);
            CreateTemplate();
            CreateFileForTemplate(_objInitialData.TemplateContent);
            SendEmail();
            DeleteFileAfterSendingEmail();
        }

        void ComputeRules(ICollection<Rules> userAdditionComponents, ICollection<Rules> userDeductionComponents)
        {
            decimal grossSalary = 0.0m;
            decimal componentAmount = 0.0m;
            decimal subtractionTotal = 0.0m;
            decimal remainingSalary = 0.0m;
            StringBuilder componentValueAsString = new StringBuilder();
            ICollection<Rules> computedRules = new List<Rules>();
            decimal salary = Convert.ToDecimal(_objInitialData.Salary);
            var additionSectionCollection = ConfigurationManager.GetSection(Constants.additionSection) as NameValueCollection;
            decimal additionComponentPercentageTotal = 0.0m;
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
                            additionComponentPercentageTotal += componentAmount;
                            componentAmount = Decimal.Round(((componentAmount) / 100) * salary, 2);
                        }
                        else if (!(char.IsLetter(additionSectionCollection[component].ToString(), 0)))
                        {
                            componentAmount = Convert.ToDecimal(additionSectionCollection[component]);
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

                //if (computedRules != null && computedRules.Count > 0)
                //{
                //    decimal remainingSalary = salary - grossSalary;
                //    computedRules.Add(new Rules
                //    {
                //        ComputationName = ComputationVariety.ADDITION,
                //        RuleName = Constants.balance,
                //        RuleValue = remainingSalary
                //    });
                //    grossSalary += remainingSalary;
                //}

                //Test Code Start.
                if (computedRules != null && computedRules.Count > 0)
                {
                    if (additionComponentPercentageTotal < 100.00m)//add round
                    {
                        if (additionSectionCollection.AllKeys.Contains(Constants.balance))
                        {
                            remainingSalary = salary - grossSalary;
                            computedRules.Add(new Rules
                            {
                                ComputationName = ComputationVariety.ADDITION,
                                RuleName = Constants.balance,
                                RuleValue = remainingSalary
                            });
                            grossSalary += remainingSalary;
                        }
                    }
                }
                //Test Code End.

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
                    else if (!(char.IsLetter(subtractionSectionCollection[component].ToString(), 0)))
                    {
                        componentAmount = Convert.ToDecimal(subtractionSectionCollection[component]);
                    }
                    computedRules.Add(new Rules
                    {
                        ComputationName = ComputationVariety.SUBTRACTION,
                        RuleName = component,
                        RuleValue = componentAmount
                    });
                    subtractionTotal += componentAmount; //newly added.
                    componentValueAsString.Clear();
                }

                if ((userDeductionComponents != null) && (userDeductionComponents.Count > 0))
                {
                    foreach (var component in userDeductionComponents)
                    {
                        computedRules.Add(new Rules
                        {
                            ComputationName = component.ComputationName,
                            RuleName = component.RuleName,
                            RuleValue = component.RuleValue
                        });
                        subtractionTotal += component.RuleValue; //newly added.
                    }
                }

                if(computedRules!= null)
                {
                    computedRules.Add(new Rules
                    {
                        ComputationName = ComputationVariety.ADDITION,
                        RuleName = Constants.grossSalary,
                        RuleValue = grossSalary
                    }
                    );
                    computedRules.Add(new Rules
                    {
                        ComputationName = ComputationVariety.SUBTRACTION,
                        RuleName = Constants.totalDeduction,
                        RuleValue = subtractionTotal
                    });
                    computedRules.Add(new Rules
                    {
                        ComputationName = ComputationVariety.ADDITION,
                        RuleName = Constants.netPay,
                        RuleValue = (grossSalary - subtractionTotal)
                    });
                }
            }
            _objInitialData.ComputedRules = computedRules;
        }

        void CreateTemplate()
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

            templateBody = templateBody.Replace("$associateCode", _objInitialData.AssociateCode);
            templateBody = templateBody.Replace("$dateOfJoining", _objInitialData.DateOfJoining);
            templateBody = templateBody.Replace("$panNumber", _objInitialData.PanNumber);
            templateBody = templateBody.Replace("$name", _objInitialData.EmployeeName);
            templateBody = templateBody.Replace("$designation", _objInitialData.Designation);
            templateBody = templateBody.Replace("$accountNumber", _objInitialData.AccountNumber);
            templateBody = templateBody.Replace("$salary", _objInitialData.Salary);
            templateBody = templateBody.Replace("$month", _objInitialData.Month.ToUpper());
            templateBody = templateBody.Replace("$year", _objInitialData.Year);

            var additionPayDetails = _objInitialData.ComputedRules.Where(a => (a.ComputationName == ComputationVariety.ADDITION) && (a.RuleName != Constants.netPay && a.RuleName != Constants.grossSalary)).ToArray();
            var deductionPayDetails = _objInitialData.ComputedRules.Where(a => (a.ComputationName == ComputationVariety.SUBTRACTION) && (a.RuleName != Constants.totalDeduction)).ToArray();
            var additionTotal = _objInitialData.ComputedRules.Where(a => (a.ComputationName == ComputationVariety.ADDITION) && (a.RuleName == Constants.grossSalary)).ToArray();
            var deductionTotal = _objInitialData.ComputedRules.Where(a => (a.ComputationName == ComputationVariety.SUBTRACTION) && (a.RuleName == Constants.totalDeduction)).ToArray();

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
                    else if (i < additionPayDetails.Count() && i >= deductionPayDetails.Count())
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

            var details = _objInitialData.ComputedRules.Where(a => a.RuleName == Constants.netPay).Select(a => a).ToList();
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
            _objInitialData.TemplateContent = templateBody;
        }

        void CreateFileForTemplate(string templateContent)
        {
            //string pdfFilePath = @"e:\SalarySlips\";
            string pdfFilePath = string.Format("{0}{1}", Path.GetTempPath(), "SalarySlips");
            string pdfFileName = string.Format("{0}{1:dd-MMM-yyyy HH-mm-ss-fff}{2}", "SalarySlip", DateTime.Now, ".pdf");
            string finalPdfPath = Path.Combine(pdfFilePath, pdfFileName);
            _objInitialData.CreateFileForTemplate = HelperMethods.HtmlToPdfConverter(pdfFilePath, pdfFileName, finalPdfPath, templateContent);
            _objInitialData.FullPdfPath = finalPdfPath;
            _objInitialData.PdfFilePath = pdfFilePath;
        }

        void SendEmail()
        {
            bool isMailSent = false;
            string senderID = ConfigurationManager.AppSettings[Constants.senderEmailId];
            string senderPassword = ConfigurationManager.AppSettings[Constants.senderEmailPassword];
            RemoteCertificateValidationCallback orgCallback = ServicePointManager.ServerCertificateValidationCallback;
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(HelperMethods.OnValidateCertificate);
                ServicePointManager.Expect100Continue = true;
                MailMessage mail = new MailMessage();

                Attachment attachment = new Attachment(_objInitialData.FullPdfPath, MediaTypeNames.Application.Octet);
                ContentDisposition disposition = attachment.ContentDisposition;
                disposition.CreationDate = File.GetCreationTime(_objInitialData.FullPdfPath);
                disposition.ModificationDate = File.GetLastWriteTime(_objInitialData.FullPdfPath);
                disposition.ReadDate = File.GetLastAccessTime(_objInitialData.FullPdfPath);
                disposition.FileName = Path.GetFileName(_objInitialData.FullPdfPath);
                disposition.Size = new FileInfo(_objInitialData.FullPdfPath).Length;
                disposition.DispositionType = DispositionTypeNames.Attachment;
                mail.Attachments.Add(attachment);

                mail.To.Add(_objInitialData.EmailId);
                mail.From = new MailAddress(senderID);
                mail.Subject = ConfigurationManager.AppSettings[Constants.emailSubject];
                mail.Body = "Salary Slip";
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = ConfigurationManager.AppSettings[Constants.smtpHost];
                smtp.Credentials = new System.Net.NetworkCredential(senderID, senderPassword);
                smtp.Port = Convert.ToInt32(ConfigurationManager.AppSettings[Constants.smtpPort]);
                smtp.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings[Constants.enableSsl]);
                smtp.Send(mail);
                isMailSent = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        void DeleteFileAfterSendingEmail()
        {
            try
            {
                if (Directory.Exists(_objInitialData.PdfFilePath))
                {
                    var directory = new DirectoryInfo(_objInitialData.PdfFilePath);
                    foreach (var file in directory.GetFiles())
                    {
                        if (!(HelperMethods.IsFileLocked(file)))
                        {
                            file.Delete();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
