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

        /// <summary>
        /// Responsible for carrying out the conversion of an html file to a pdf file.
        /// Firstly, it creates a directory called SalarySlips in the temp folder path of the local machine if directory doesn't exist
        /// ,to temporarily store the pdf format of the html file. This directory's path including itself is given the necessary permissions.
        /// The template control which is in html format at this point is then converted into pdf format using Nreco PdfGenerator package.
        /// A stream is opened to the file where the pdf byte equivalent is to be written to and stored in the temporary path of the machine.
        /// </summary>
        /// <param name="pdfFilePath">The temporary file path where the generated salary slip is to be stored.</param>
        /// <param name="finalPdfPath">The complete path of where the pdf file is to be dtored. It includes the name of the pdf file itself,
        /// which if doesn't exist, is created.</param>
        /// <param name="templateContent">The html content which is to be converted to its pdf equivalent.</param>
        /// <returns>A boolean value indicating whether the pdf file was successfully created.</returns>
        public static bool HtmlToPdfConverter(string pdfFilePath,string finalPdfPath, string templateContent)
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

        /// <summary>
        /// Responsible for fetching the footer content for the html content and the pdf file, line by line from
        /// a file whose path is mentioned in the application configuration settings file.
        /// </summary>
        /// <returns>The contents which have been read from the file, line by line.</returns>
        public static string FetchFooterContent()
        {
            string line = null;
            StringBuilder footerContent = new StringBuilder();
            try
            {
                var fileToRead = ConfigurationManager.AppSettings[Constants.footerContent];
                if (File.Exists(fileToRead))
                {
                    using (StreamReader reader = new StreamReader(fileToRead))
                    {
                        while((line = reader.ReadLine()) != null)
                        {
                            footerContent.AppendLine(line).Replace("\r\n","<br/>");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return footerContent.ToString();
        }

        /// <summary>
        /// Sets the permission to the path where the salary slip is to be temporarily stored in the machine's
        /// temp folder.
        /// </summary>
        /// <param name="pdfFilePath">The path where the permissions are to be defined</param>
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

        /// <summary>
        /// Checks whether the file is still being accessed by another process/thread or not.
        /// </summary>
        /// <param name="file">The file which is to be checked.</param>
        /// <returns>Returns a true value if the file is locked, false otherwise.</returns>
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
    }
}
