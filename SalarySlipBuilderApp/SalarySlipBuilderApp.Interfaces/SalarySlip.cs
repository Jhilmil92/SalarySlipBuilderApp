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
using System.Globalization;

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
        
        /// <summary>
        /// Responsible for restricting the method flow and calling subsequent operations in a defined order to be executed.
        /// </summary>
        public void SalarySlipProcess()
        {
            ComputeRules(_objInitialData.UserAdditionComponents, _objInitialData.UserDeductionComponents);
            CreateTemplate();
            CreateFileForTemplate(_objInitialData.TemplateContent);
            SendEmail();
            DeleteFileAfterSendingEmail();
        }

        /// <summary>
        /// 1) Collects the pre-defined addition and deduction components from the app.config file and holds them as a collection.
        /// 2) From the collection, if a value of a component has a percent sign, then that is removed and formatted in a decimal form to
        /// use it against the salary and the resuting value is added to the gross salary. If the value is not a percent, then the value
        /// is simply added to the gross salary. There is a simultaneous check to see if the percent value adds up to be 100% in the variable
        /// additionComponentPercentageTotal.
        /// 3)In the next step, the user defined addition components values are extracted and added to the gross salary. 
        /// 4)computedRules is the list which will contain the components at any point of time.
        /// 5) In case of addition components,if the additionComponentPercentageTotal value is greater than equal to 100, then the "Balance"
        /// component will not be added to computedRules.
        /// 6) The same process applies for subtraction components except step 5.
        /// 7) The gross salary, total deduction and net pay components are added towards the end.
        /// </summary>
        /// <param name="userAdditionComponents">The user defined addition components.</param>
        /// <param name="userDeductionComponents">The user defined deduction/subtraction components.</param>
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
                    subtractionTotal += componentAmount;
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
                        subtractionTotal += component.RuleValue;
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

        /// <summary>
        /// 1) Obtains a stream to the "SalarySlipTemplate.html" file from the "TemplateApp" application.
        /// 2) Replaces the pre-defined placeholders in the SalarySlipTemplate.html file with the employee details.
        /// 3) additionPayDetails list contains all the addition components except gross salary and netpay. Similarly, deductionPayDetails
        /// contains all the deduction components except the deduction total.
        /// 4) The additionTotal and the deductionTotal contain the values of the sum of all the addition components(gross salary) and the 
        /// sum of all deduction components (deduction total) respectively.
        /// 5) The additionPayDetails and the deductionPayDetails are used first to contruct the html row and column structurehaving all 
        /// the components except gross salary, total deduction and net pay. The three mentioned components are placed towards the end of
        /// the structure.
        /// 6)The net pay in words, the header and the footer values are also added to the structure towards the end.
        /// </summary>
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

        /// <summary>
        /// 1) Extracts the temporary folder path in the machine and appends a sub folder name called "SalarySlips" where the
        /// temporary salary slip files are to be stored (pdfFilePath).
        /// 2) Constructs the pdf file name to be given to the file which will be converted from its html equivalent. This name 
        /// is then combined with the pdfFilePath to reresent the complete path where the pdf file is to be created.
        /// 3) Invokes the HtmlToPdfConverter() file to make the pdf file from its html equivalent.
        /// </summary>
        /// <param name="templateContent"></param>
        void CreateFileForTemplate(string templateContent)
        {
            string pdfFilePath = string.Format("{0}{1}", Path.GetTempPath(), "SalarySlips");
            string pdfFileName = string.Format("{0}{1:dd-MMM-yyyy HH-mm-ss-fff}{2}", "SalarySlip", DateTime.Now, ".pdf");
            string finalPdfPath = Path.Combine(pdfFilePath, pdfFileName);
            _objInitialData.CreateFileForTemplate = HelperMethods.HtmlToPdfConverter(pdfFilePath,finalPdfPath, templateContent);
            _objInitialData.FullPdfPath = finalPdfPath;
            _objInitialData.PdfFilePath = pdfFilePath;
        }

        /// <summary>
        /// 1) Extracts sender credentials from the application configuration file.
        /// 2) The attachment properties are configured using "FullPdfPath" which is the nothing but the complete salary slip file path 
        /// and added as an attachment to the mail instance.
        /// 3) The mail properties are configured.
        /// 4) The smtp properties are configured.
        /// </summary>
        void SendEmail()
        {
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
                mail.Subject = ConfigurationManager.AppSettings[Constants.emailSubject].Replace("$Month", _objInitialData.Month.Pascalize()).Replace("$Year", _objInitialData.Year.Pascalize());
                mail.Body = "Salary Slip";
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = ConfigurationManager.AppSettings[Constants.smtpHost];
                smtp.Credentials = new System.Net.NetworkCredential(senderID, senderPassword);
                smtp.Port = Convert.ToInt32(ConfigurationManager.AppSettings[Constants.smtpPort]);
                smtp.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings[Constants.enableSsl]);
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Check if each salary slip file, presently created or existing, is locked or not and delete if unlocked,
        /// since a locked file cannot be deleted.
        /// </summary>
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
