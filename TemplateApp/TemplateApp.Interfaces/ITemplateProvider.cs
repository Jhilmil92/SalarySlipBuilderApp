using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateApp.TemplateApp.Interfaces
{
    public interface ITemplateProvider:IDisposable
    {
        StreamReader SupplyTemplateStream();
    }
}
