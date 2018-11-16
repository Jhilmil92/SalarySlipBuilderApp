using NReco.PdfGenerator;
using SalarySlipBuilderApp.SalarySlipBuilderApp.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace SalarySlipBuilderApp.SalarySlipBuilderApp.Classes
{
    public class HelperMethods
    {
        private HelperMethods()
        {

        }

        public static bool HtmlToPdfConverter(string pdfFilePath, string pdfFileName, string finalPdfPath, string templateContent)
        {
            bool isPdfFileCreated = false;
            if (!(Directory.Exists(pdfFilePath)))
            {
                Directory.CreateDirectory(pdfFilePath);
            }

            SetPathPermission(pdfFilePath);
            var htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
            htmlToPdf.CustomWkHtmlArgs = "--disable-smart-shrinking";
            htmlToPdf.Size = PageSize.A4;
            htmlToPdf.Orientation = NReco.PdfGenerator.PageOrientation.Portrait;
            var pdfBytes = htmlToPdf.GeneratePdf(templateContent);
            if (pdfBytes != null)
            {
                using (FileStream fileStream = new FileStream(finalPdfPath, FileMode.OpenOrCreate))
                {
                    fileStream.Write(pdfBytes, 0, pdfBytes.Length);
                    fileStream.Close();
                    isPdfFileCreated = true;
                }
            }
            return isPdfFileCreated;
        }
        public static string FetchFooterContent()
        {
            StringBuilder footerContent = new StringBuilder();
            try
            {
                var fileToRead = ConfigurationManager.AppSettings[Constants.footerContent];
                if (File.Exists(fileToRead))
                {
                    using (StreamReader reader = new StreamReader(fileToRead))
                    {
                        footerContent.Append(reader.ReadToEnd());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return footerContent.ToString();
        }

        private static void SetPathPermission(string pdfFilePath)
        {
            var directoryInfo = new DirectoryInfo(pdfFilePath);
            var accessControl = directoryInfo.GetAccessControl();
            var userIdentity = WindowsIdentity.GetCurrent();
            var permissions = new FileSystemAccessRule(userIdentity.Name,
                                                  FileSystemRights.FullControl,
                                                  InheritanceFlags.ObjectInherit |
                                                  InheritanceFlags.ContainerInherit,
                                                  PropagationFlags.None,
                                                  AccessControlType.Allow);
            accessControl.AddAccessRule(permissions);
            directoryInfo.SetAccessControl(accessControl);
        }

        public static Boolean IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                //Don't change FileAccess to ReadWrite, 
                //because if a file is in readOnly, it fails.
                stream = file.Open
                (
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.None
                );
            }
            catch (IOException)
            {
                //the file is unavailable because it is
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

        public static bool OnValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        public static bool DeleteSalarySlips(string pdfFilePath)
        {
            bool isFileDeleted = false;
            try
            {
                if (Directory.Exists(pdfFilePath))
                {
                    var directory = new DirectoryInfo(pdfFilePath);
                    foreach (var file in directory.GetFiles())
                    {
                        if (!(IsFileLocked(file)))
                        {
                            file.Delete();
                            isFileDeleted = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return isFileDeleted;
        }
    }
}
