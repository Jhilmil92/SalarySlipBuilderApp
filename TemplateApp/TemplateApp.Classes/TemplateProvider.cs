using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateApp.TemplateApp.Interfaces;

namespace TemplateApp.TemplateApp.Classes
{
    public class TemplateProvider:ITemplateProvider
    {
        private StreamReader _stream;
        public System.IO.StreamReader SupplyTemplateStream()
        {
            var currentAssembly = typeof(ITemplateProvider).Assembly;
            _stream = new StreamReader(currentAssembly.GetManifestResourceStream("TemplateApp.TemplateApp.Templates.SalarySlipTemplate.html"));
            return _stream;
        }

        public void Dispose()
        {
            _stream.Dispose();
        }
    }
}
