using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalarySlipBuilderApp.SalarySlipBuilder.ExtensionClasses
{
   public static class DateTimeExtensions
    {
       /// <summary>
       /// An extension method that fetches the twelve months in a year independent of the culture.
       /// </summary>
       /// <param name="dateTime">The datetime instance that actually invokes the extension method.</param>
        /// <returns>The tewlve months in a year</returns>
        public static List<String> GetMonths(this DateTime dateTime)
        {
            return CultureInfo.InvariantCulture.DateTimeFormat.MonthNames.Take(12).ToList();
        }
    }
}
