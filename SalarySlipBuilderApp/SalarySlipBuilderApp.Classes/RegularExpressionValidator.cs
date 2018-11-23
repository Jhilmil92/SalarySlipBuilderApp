using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SalarySlipBuilderApp.SalarySlipBuilderApp.Classes
{
    public class RegularExpressionValidator
    {
        /// <summary>
        /// Defines a regular expression to validate an employee name.
        /// The name is limited only to letters and spaces, with the exception that the name must not start with a space.
        /// </summary>
        /// <param name="input">The name which is to be validated against the regular expression.</param>
        /// <returns>A boolean value of true if the validation is successful, false otherwise.</returns>
        public static bool IsValidName(string input)
        {
            bool isValidInput = false;
            Regex regularExpression = new Regex("^[a-zA-Z]+[a-zA-Z\\s]+[a-zA-Z]$");
            Match match = regularExpression.Match(input);
            if (match.Success)
            {
                isValidInput = true;
            }
            return isValidInput;
        }

        /// <summary>
        /// Defines a regular expression to validate an employee's 10 digit aplhanumeric PAN number.
        /// </summary>
        /// <param name="input">The PAN number which is to be validated against the regular expression.</param>
        /// <returns>A boolean value of true if the validation is successful, false otherwise.</returns>
        public static bool IsValidPan(string input)
        {
            bool isValidInput = false;
            Regex regularExpression = new Regex("[a-zA-Z]{5}[\\d]{4}[a-zA-Z]$");
            Match match = regularExpression.Match(input);
            if (match.Success)
            {
                isValidInput = true;
            }
            return isValidInput;
        }

        /// <summary>
        /// Defines a regular expression to validate an employee's account number.
        /// The account number is only limited to numbers.
        /// </summary>
        /// <param name="input">The Account number which is to be validated against the regular expression.</param>
        /// <returns>A boolean value of true if the validation is successful, false otherwise.</returns>
        public static bool IsValidAccountNumber(string input)
        {
            bool isValidInput = false;
            Regex regularExpression = new Regex("^[\\d]+$");
            Match match = regularExpression.Match(input);
            if (match.Success)
            {
                isValidInput = true;
            }
            return isValidInput;
        }

        /// <summary>
        /// Defines a regular expression to validate whether an employee's designation is in the format format.
        /// The designation is only limited to letters from the english alphabet.
        /// </summary>
        /// <param name="input">The designation which is to be validated against the regular expression.</param>
        /// <returns>A boolean value of true if the validation is successful, false otherwise.</returns>
        public static bool IsValidDesignation(string input)
        {
            bool isValidInput = false;
            Regex regularExpression = new Regex("^[a-zA-Z]+[a-zA-Z\\s]+[a-zA-Z\\d]$");
            Match match = regularExpression.Match(input);
            if (match.Success)
            {
                isValidInput = true;
            }
            return isValidInput;
        }

        /// <summary>
        /// Defines a regular expression to validate whether an employee's salary is in a proper format.
        /// The salary should have only numbers and shoudn't exceed 99 lakhs.
        /// </summary>
        /// <param name="input">The salary which is to be validated against the regular expression.</param>
        /// <returns>A boolean value of true if the validation is successful, false otherwise.</returns>
        public static bool IsValidSalary(string input)
        {
            bool isValidInput = false;
            Regex regularExpression = new Regex("^[1-9][\\d]{2,6}$");
            Match match = regularExpression.Match(input);
            if (match.Success)
            {
                isValidInput = true;
            }
            return isValidInput;
        }
    }
}
