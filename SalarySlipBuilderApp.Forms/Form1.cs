using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SalarySlipBuilderApp.SalarySlipBuilderApp.Models;
using SalarySlipBuilderApp.SalarySlipBuilder.ExtensionClasses;
using SalarySlipBuilderApp.SalarySlipBuilderApp.Classes;
using SalarySlipBuilderApp.Models;
using SalarySlipBuilderApp.Classes;
using System.Configuration;
using SalarySlipBuilderApp.SalarySlipBuilderApp.Common;
using SalarySlipBuilderApp.SalarySlipBuilder.Common;
using System.Collections.Specialized;

namespace SalarySlipBuilderApp.SalarySlipBuilderApp.Forms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets up the necessary state when the form is loaded.
        /// </summary>
        /// <param name="sender">The control that fired the event.</param>
        /// <param name="e">The event specific arguments.</param>
        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView.Hide();
            addOuterPanel.Hide();
            deductOuterPanel.Hide();
            associateCode.Select();
            addOuterPanel.Controls.Clear();
            deductOuterPanel.Controls.Clear();
            PopulateMonths();
            PopulateYears();

        }

        /// <summary>
        /// Populates a dropdown list with the months in a year and keeps the default display value of the 
        /// month to the current month.
        /// </summary>
        private void PopulateMonths()
        {
            month.DataSource = DateTime.Now.GetMonths();
            month.SelectedItem = CultureInfo.InvariantCulture.DateTimeFormat.MonthNames[DateTime.Now.AddMonths(-1).Month];
        }

        /// <summary>
        /// Populates a dropdown list with years ranging from the year 1951 through present.
        /// The default value on display is the current year.
        /// </summary>
        private void PopulateYears()
        {
            year.DataSource = Enumerable.Range(1950, DateTime.Now.Year - 1950 + 1).ToList();
            year.SelectedItem = DateTime.Now.Year;
        }

        /// <summary>
        /// The primary button control that generates the salary breakdown in a display grid.
        /// 1) It checks whether the salary input is not empty, populates a model called initialData with the values associated with the employee.
        /// 2) Collects the addition and the deduction components that the user has added from the UI(form) in userAdditionComponents and userDeductionComponents 
        ///    lists respectively and adds them to the initialData model.
        /// 3) The SalarySlipProcess() method of FormInput is then invoked to start the salary slip computation process.
        /// 4) The computedRules property of initialData model is used to populate the grid shown to the user.
        /// 5) The method PopulateGrid() is responsible for filling the display grid with the calculated components (salary Breakdown) that is displayed to the user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void generateButton_Click(object sender, EventArgs e)
        {
            InitialData initialData = null;
            ICollection<Rules> userAdditionComponents = null;
            ICollection<Rules> userDeductionComponents = null;
            SalarySlip objSalarySlip = null;
            //Setup
            if (salary.Text.ToString() != string.Empty)
            {
                initialData = new InitialData();
                decimal salaryAmount = Convert.ToDecimal(salary.Text);
                initialData.AssociateCode = associateCode.Text.ToString();
                initialData.EmployeeName = requiredName.Text.ToString();
                initialData.DateOfJoining = dateOfJoining.Text.ToString();
                initialData.PanNumber = pan.Text.ToString();
                initialData.AccountNumber = accountNumber.Text.ToString();
                initialData.Designation = designation.Text.ToString();
                initialData.Salary = salary.Text.ToString();
                initialData.EmailId = email.Text.ToString();
                initialData.Month = month.SelectedItem.ToString();
                initialData.Year = year.SelectedItem.ToString();

                if (addComponentNumber.Text != string.Empty)
                {
                    List<Rules> userRules = new List<Rules>();
                    userRules = SegregateComponents(addComponentNumber, ComputationVariety.ADDITION, userRules);
                    userAdditionComponents = FetchUserComponents(addOuterPanel.Controls, ComputationVariety.ADDITION, userRules);
                    initialData.UserAdditionComponents = userAdditionComponents;
                }
                if (deductComponentNumber.Text != string.Empty)
                {
                    List<Rules> userRules = new List<Rules>();
                    userRules = SegregateComponents(deductComponentNumber, ComputationVariety.SUBTRACTION, userRules);
                    userDeductionComponents = FetchUserComponents(deductOuterPanel.Controls, ComputationVariety.SUBTRACTION, userRules);
                    initialData.UserDeductionComponents = userDeductionComponents;
                }

                objSalarySlip = new FormInput(initialData);
                objSalarySlip.SalarySlipProcess();

                ICollection<Rules> computedRules = initialData.ComputedRules;
                ICollection<Rules> finalResults = PopulateGrid(computedRules, userAdditionComponents, userDeductionComponents);
            }

        }

        /// <summary>
        /// This method is responsible for receiving a component collection and invoking the SegregateComponents() method to 
        /// segregate the input values of the control collection.
        /// </summary>
        /// <param name="componentCollection">The collection of controls whose input is to be segregated.</param>
        /// <param name="typeOfOperation">Indicates whether the component is an addition component or a subtraction component</param>
        /// <param name="userRules">A list, initially having only one component input, which is nothing but the main input text box value that further 
        /// generates input text boxes on clicking the "AC" button and "DC" button for addition component and deduction component respectively.</param>
        /// <returns>A list of components, addition or subtraction at any point of time.</returns>
        private List<Rules> FetchUserComponents(Control.ControlCollection componentCollection, ComputationVariety typeOfOperation, List<Rules> userRules)
        {
            StringBuilder ruleHolder = new StringBuilder();
            if (componentCollection != null && componentCollection.Count > 0)
            {
                foreach (var component in componentCollection)
                {
                    userRules = SegregateComponents((Control)component, typeOfOperation, userRules);
                }
            }
            return userRules;
        }

        /// <summary>
        /// This method is responsible for receiving a control and breaking down the textbox control input which is in the Name,Value
        /// format and mapping it to the Rules model. The type of component, whether addition or subtraction is also specified in the
        /// Rules model.
        /// </summary>
        /// <param name="component">The component textbox control whose input is to be segregated.</param>
        /// <param name="typeOfOperation">Whether the control input is an addition component or a subtraction component.</param>
        /// <param name="userRules">The list which contains the segregated set of components' inputs.</param>
        /// <returns></returns>
        public List<Rules> SegregateComponents(Control component, ComputationVariety typeOfOperation, List<Rules> userRules)
        {
            StringBuilder componentBuilder = new StringBuilder();
            string[] componentParts = componentBuilder.Append(component).ToString().Split('=');
            componentParts = componentParts[0].Split(',');
            if ((componentParts != null) && (componentParts.Count() > 0))
            {
                userRules.Add(new Rules()
                {
                    ComputationName = typeOfOperation,
                    RuleName = componentParts[1].Split(':')[1],
                    RuleValue = Decimal.Round(Convert.ToDecimal(componentParts[2]), 2)
                }
                    );
            }
            componentBuilder.Clear();
            return userRules;
        }


        /// <summary>
        /// 1) This method is responsible for populating the grid with the calculated components, both statically derived from the web.config file
        /// as well as components entered by the user(inclusive of addition and deduction components). In general, it shows the salary breakup for
        /// a specific employee.
        /// 2)
        /// </summary>
        /// <param name="computedRules">The computed rules from both web.config file(statically defined) and user inputs (addition and/or subtraction components) based
        /// on the employee's salary.</param>
        /// <param name="userAdditionComponents">The list of rules extracted from the addition component category that is entered by the user.</param>
        /// <param name="userDeductionComponents">The list of rules extracted from the deduction component category that is entered by the user.</param>
        /// <returns></returns>
        private ICollection<Rules> PopulateGrid(ICollection<Rules> computedRules, ICollection<Rules> userAdditionComponents, ICollection<Rules> userDeductionComponents)
        {
            int computedRuleCounter = 0;
            int subtractiontotalRowsCount = 0;
            decimal additionSum = 0.0m;
            decimal subtractionSum = 0.0m;
            DataGridView dataGridView = this.dataGridView;
            var additionSectionCollection = ConfigurationManager.GetSection(Constants.additionSection) as NameValueCollection;
            var subtractionSectionCollection = ConfigurationManager.GetSection(Constants.subtractionSection) as NameValueCollection;

            if ((dataGridView != null))
            {
                DataTable dataTable = new DataTable();
                dataTable.Columns.AddRange(new DataColumn[4] {
                new DataColumn(Constants.addition,typeof(string)),
                new DataColumn(Constants.additionTotal, typeof(decimal)),
                new DataColumn(Constants.subtraction, typeof(string)),
                new DataColumn(Constants.subtractionTotal, typeof(decimal))});

                int additiontotalRowsCount = 0;


                if ((additionSectionCollection != null) && (additionSectionCollection.Count > 0))
                {
                    /*If the list of computed rules has the component corresponding to the addition rules from the app.config file
                    represented by additionSectionCollection, only then the count is incremented. This is done because all the
                    addition rules in additionSectionCollection may not be present in computedRules collection if calculations
                    require that a rule may be omitted (For e.g. if the percent values of the rules sum up to be 100%, then the 
                    rule "balance" won't apply, even though it will still be a part of additionSectionCollection .*/
                    additiontotalRowsCount = computedRules.Where(a => (additionSectionCollection.Keys.Cast<string>().Contains(a.RuleName))).Count();
                }

                //Add the user defined addition components' count.
                if ((userAdditionComponents != null) && (userAdditionComponents.Count > 0))
                {
                    additiontotalRowsCount += userAdditionComponents.Count;
                }

                //Adding addition components to the data table which is to be added to the grid.
                for (int i = 0; i < additiontotalRowsCount; i++)
                {
                    object[] additionArray = new object[2];
                    additionArray[0] = computedRules.Where(a => a.ComputationName == ComputationVariety.ADDITION && a.RuleName != Constants.grossSalary && a.RuleName != Constants.netPay).ElementAt(i).RuleName;
                    additionArray[1] = computedRules.Where(a => a.ComputationName == ComputationVariety.ADDITION && a.RuleName != Constants.grossSalary && a.RuleName != Constants.netPay).ElementAt(i).RuleValue;
                    additionSum += Convert.ToDecimal(additionArray[1]);
                    DataRow dataRow = dataTable.NewRow();
                    dataRow.ItemArray = additionArray;
                    dataTable.Rows.Add(dataRow);
                }

              
                if ((subtractionSectionCollection != null) && (subtractionSectionCollection.Count > 0))
                {
                    /*If the list of computed rules has the component corresponding to the subtraction rules from the app.config file
                  represented by subtractionSectionCollection, only then the count is incremented. This is done because all the
                  addition rules in subtractionSectionCollection may not be present in computedRules collection if calculations
                  require that a rule may be omitted */
                    subtractiontotalRowsCount = computedRules.Where(a => (subtractionSectionCollection.Keys.Cast<string>().Contains(a.RuleName))).Count();
                }

                //Add the user defined subtraction components' count.                
                if ((userDeductionComponents != null) && (userDeductionComponents.Count > 0))
                {
                    subtractiontotalRowsCount += userDeductionComponents.Count;
                }

                if (dataTable.Rows.Count != 0)
                {
                    for (int i = 0; i < subtractiontotalRowsCount; i++)
                    {
                        //Start adding the subtraction components aligned to the columns subtraction and subtractiontotal.
                        if (dataTable.Rows.Count < subtractiontotalRowsCount)
                        {
                            break;
                        }
                        dataTable.Rows[i][Constants.subtraction] = computedRules.Where(a => a.ComputationName == ComputationVariety.SUBTRACTION).ElementAt(i).RuleName;
                        dataTable.Rows[i][Constants.subtractionTotal] = computedRules.Where(a => a.ComputationName == ComputationVariety.SUBTRACTION).ElementAt(i).RuleValue;
                        computedRuleCounter++;
                        subtractionSum += Convert.ToDecimal(dataTable.Rows[i][Constants.subtractionTotal]);
                     }

                    //If there are more subtraction components than the addition component, then start a new row given by
                    //the location computedRuleCounter.
                     if (computedRuleCounter != subtractiontotalRowsCount)
                     {
                        for (int i = computedRuleCounter; i < subtractiontotalRowsCount; i++)
                        {
                            DataRow dataRow = dataTable.NewRow();
                            dataRow[Constants.subtraction] = computedRules.Where(a => a.ComputationName == ComputationVariety.SUBTRACTION).ElementAt(i).RuleName;
                            dataRow[Constants.subtractionTotal] = computedRules.Where(a => a.ComputationName == ComputationVariety.SUBTRACTION).ElementAt(i).RuleValue;
                            subtractionSum += Convert.ToDecimal(dataRow[Constants.subtractionTotal]);
                            dataTable.Rows.Add(dataRow);
                        }
                    }
              }
             else
             {
                //If there are no existing rows in the table. This may happen when there are no addition components.
                for (int i = 0; i < subtractionSectionCollection.Count; i++)
                {
                    DataRow dataRow = dataTable.NewRow();
                    dataRow[Constants.subtraction] = computedRules.Where(a => a.ComputationName == ComputationVariety.SUBTRACTION).ElementAt(i).RuleName;
                    dataRow[Constants.subtractionTotal] = computedRules.Where(a => a.ComputationName == ComputationVariety.SUBTRACTION).ElementAt(i).RuleValue;
                    subtractionSum += Convert.ToDecimal(dataRow[Constants.subtractionTotal]);
                    dataTable.Rows.Add(dataRow);
                }
             }

            if (dataTable.Rows.Count > 0)
            {
                //Makes sure that there are values in the addition and additionTotal columns of the data table.
                DataRow totalDataRow = null;
                var additionDataRows = dataTable.AsEnumerable()
                            .Where(w => (w.Field<string>(Constants.addition) != null && w.Field<string>(Constants.addition) != string.Empty)
                             && (w.Field<decimal>(Constants.additionTotal) != null))
                            .Select(a => a.Field<string>(Constants.addition));

                //Makes sure that there are values in the subtraction and subtractionTotal columns of the data table.
                var subtractionDataRows = dataTable.AsEnumerable()
                            .Where(w => (w.Field<string>(Constants.subtraction) != null && w.Field<string>(Constants.subtraction) != string.Empty)
                             && (w.Field<decimal>(Constants.subtractionTotal) != null))
                            .Select(a => a.Field<string>(Constants.subtraction));

                //If there are values in addition and additionTotal columns, then add the gross salary component and its value to the data table.
                if (additionDataRows != null && additionDataRows.Count() > 0)
                {
                    totalDataRow = dataTable.NewRow();
                    totalDataRow[Constants.addition] = Constants.grossSalary;
                    totalDataRow[Constants.additionTotal] = additionSum;
                    computedRules.Add(
                               new Rules
                               {
                                   ComputationName = ComputationVariety.ADDITION,
                                   RuleName = Constants.additionTotal,
                                   RuleValue = additionSum
                               }
                             );
                    dataTable.Rows.Add(totalDataRow);
                 }

                //If there are values in subtraction and subtractionTotal columns, then add the total deduction component and its value to the data table.
                 if (subtractionDataRows != null && subtractionDataRows.Count() > 0)
                 {

                    if (totalDataRow == null)
                    {
                        totalDataRow = dataTable.NewRow();
                        dataTable.Rows.Add(totalDataRow);
                    }
                    totalDataRow[Constants.subtraction] = Constants.totalDeduction;
                    totalDataRow[Constants.subtractionTotal] = subtractionSum;
                    computedRules.Add(
                                new Rules
                                {
                                    ComputationName = ComputationVariety.SUBTRACTION,
                                    RuleName = Constants.subtractionTotal,
                                    RuleValue = subtractionSum
                                });
                 }

                /*If the sum of all the addition components and subtraction components are greater than zero, then add the net pay component
                and it's value to the data table.*/
                 if (additionSum >= 0 && subtractionSum >= 0)
                 {
                    DataRow netPayDataRow = dataTable.NewRow();
                    netPayDataRow[Constants.addition] = Constants.netPay;
                    netPayDataRow[Constants.additionTotal] = Decimal.Round(additionSum - subtractionSum, 2);
                    dataTable.Rows.Add(netPayDataRow);
                    computedRules.Add(
                                new Rules
                                {
                                    ComputationName = ComputationVariety.ADDITION,
                                    RuleName = Constants.netPay,
                                    RuleValue = Decimal.Round(additionSum - subtractionSum, 2)
                                });
                  }
            }
            dataGridView.DataSource = dataTable;
            this.Width = 583;
            this.Height = 550;
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView.Show();
        }
        return computedRules;
      }

        /// <summary>
        /// 1)The event that is fired when a user tries to add a new addition component from the user interface by clicking on the "AC" button.
        /// 2)A new textbox is created and its y component is incremented by 20 points each time to position them vertically. This is achieved
        /// by first checking whether the number of controls is greater than one because the first textbox doesn't need a change in the position 
        /// the y coordinates. 
        /// 3)From the second component onwards, the location of the last added control is fetched and 20 points are added to it
        /// to align them vertically.
        /// 4)Only 10 components are allowed.
        /// </summary>
        /// <param name="sender">The control that fires the event.</param>
        /// <param name="e">Event arguments for the associated event.</param>
        private void addComponent_Click(object sender, EventArgs e)
        {
            if (addOuterPanel.Controls.Count < 9)
            {
                int xCoordinate = 10;
                int yCoordinate = 10;
                TextBox leftAddTextBox = new TextBox();
                if (addOuterPanel.HasChildren)
                {
                    IEnumerable<Control> controlCollection = addOuterPanel.Controls.Cast<Control>();
                    var yCoordinateCollection = controlCollection.OrderByDescending(a => a.Location.Y).First();
                    yCoordinate = yCoordinateCollection.Location.Y + 20;
                }
                leftAddTextBox.Location = new Point(xCoordinate, yCoordinate);
                leftAddTextBox.Size = new System.Drawing.Size(160, 100);
                addOuterPanel.Controls.Add(leftAddTextBox);
                addOuterPanel.AutoScroll = true;
                addOuterPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
                leftAddTextBox.Text = string.Format("Component Name,Value");
                addOuterPanel.Show();
            }
            else
            {
                MessageBox.Show("No more than 10 components are allowed", "Salary Slip Application", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        /// <summary>
        /// 1)The event that is fired when a user tries to add a new deduction component from the user interface by clicking on the "AC" button.
        /// 2)A new textbox is created and its y component is incremented by 20 points each time to position them vertically. This is achieved
        /// by first checking whether the number of controls is greater than one because the first textbox doesn't need a change in the position 
        /// the y coordinates. 
        /// 3)From the second component onwards, the location of the last added control is fetched and 20 points are added to it
        /// to align them vertically.
        /// 4)Only 10 components are allowed.
        /// </summary>
        /// <param name="sender">The control that fires the event.</param>
        /// <param name="e">Event arguments for the associated event.</param>
        private void deductComponent_Click(object sender, EventArgs e)
        {
            if (deductOuterPanel.Controls.Count < 9)
            {
                int xCoordinate = 10;
                int yCoordinate = 10;
                TextBox leftDeductTextBox = new TextBox();
                if (deductOuterPanel.HasChildren)
                {
                    IEnumerable<Control> controlCollection = deductOuterPanel.Controls.Cast<Control>();
                    var yCoordinateCollection = controlCollection.OrderByDescending(a => a.Location.Y).First();
                    yCoordinate = yCoordinateCollection.Location.Y + 20;
                }
                leftDeductTextBox.Location = new Point(xCoordinate, yCoordinate);
                leftDeductTextBox.Size = new System.Drawing.Size(160, 100);
                deductOuterPanel.Controls.Add(leftDeductTextBox);
                deductOuterPanel.AutoScroll = true;
                deductOuterPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
                leftDeductTextBox.Text = string.Format("Component Name,Value");
                deductOuterPanel.Show();
            }
            else
            {
                MessageBox.Show("No more than 10 components are allowed", "Salary Slip Application", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void requiredName_Validating(object sender, CancelEventArgs e)
        {
            string textInput = requiredName.Text.TrimStart().TrimEnd();
            if (string.IsNullOrEmpty(textInput))
            {
                MessageBox.Show("No name has been entered", "Salary Slip Application", MessageBoxButtons.OK, MessageBoxIcon.Error);
                requiredName.Focus();
            }
            else if (!(RegularExpressionValidator.IsValidName(textInput)))
            {
                MessageBox.Show("The entered name is not in proper format", "Salary Slip Application", MessageBoxButtons.OK, MessageBoxIcon.Error);
                requiredName.Focus();
            }
        }

        /// <summary>
        /// This method invokes a method having a regular expression that validates the format of the entered PAN Number.
        /// </summary>
        /// <param name="sender">The control that fired the event</param>
        /// <param name="e">The arguments associated with the fired event.</param>
        private void pan_Validating(object sender, CancelEventArgs e)
        {
            string textInput = pan.Text.TrimStart().TrimEnd();
            if (string.IsNullOrEmpty(textInput))
            {
                MessageBox.Show("No PAN has been entered", "Salary Slip Application", MessageBoxButtons.OK, MessageBoxIcon.Error);
                pan.Focus();
            }
            else if (!(RegularExpressionValidator.IsValidPan(textInput)))
            {
                MessageBox.Show("The entered PAN is not in proper format", "Salary Slip Application", MessageBoxButtons.OK, MessageBoxIcon.Error);
                pan.Focus();
            }
        }

        /// <summary>
        /// This method invokes a method having a regular expression that validates the format of the entered Bank Account Number.
        /// </summary>
        /// <param name="sender">The control that fired the event</param>
        /// <param name="e">The arguments associated with the fired event.</param>
        private void accountNumber_Validating(object sender, CancelEventArgs e)
        {
            string textInput = accountNumber.Text.TrimStart().TrimEnd();
            if (string.IsNullOrEmpty(textInput))
            {
                MessageBox.Show("No account number has been entered", "Salary Slip Application", MessageBoxButtons.OK, MessageBoxIcon.Error);
                accountNumber.Focus();
            }
            else if (!(RegularExpressionValidator.IsValidAccountNumber(textInput)))
            {
                MessageBox.Show("The entered account number is not in proper format", "Salary Slip Application", MessageBoxButtons.OK, MessageBoxIcon.Error);
                accountNumber.Focus();
            }
        }

        /// <summary>
        /// This method invokes a method having a regular expression that validates the format of the entered employee designation.
        /// </summary>
        /// <param name="sender">The control that fired the event</param>
        /// <param name="e">The arguments associated with the fired event.</param>
        private void designation_Validating(object sender, CancelEventArgs e)
        {
            string textInput = designation.Text.TrimStart().TrimEnd();
            if (string.IsNullOrEmpty(textInput))
            {
                MessageBox.Show("No designation has been entered", "Salary Slip Application", MessageBoxButtons.OK, MessageBoxIcon.Error);
                designation.Focus();
            }
            else if (!(RegularExpressionValidator.IsValidDesignation(textInput)))
            {
                MessageBox.Show("The entered designation is not in proper format", "Salary Slip Application", MessageBoxButtons.OK, MessageBoxIcon.Error);
                designation.Focus();
            }
        }

        /// <summary>
        /// This method invokes a method having a regular expression that validates the format of the entered employee salary.
        /// </summary>
        /// <param name="sender">The control that fired the event</param>
        /// <param name="e">The arguments associated with the fired event.</param>
        private void salary_Validating(object sender, CancelEventArgs e)
        {
            string textInput = salary.Text.TrimStart().TrimEnd();
            if (string.IsNullOrEmpty(textInput))
            {
                MessageBox.Show("No salary has been entered", "Salary Slip Application", MessageBoxButtons.OK, MessageBoxIcon.Error);
                salary.Focus();
            }
            else if (!(RegularExpressionValidator.IsValidSalary(textInput)))
            {
                MessageBox.Show("The entered salary is not in proper format", "Salary Slip Application", MessageBoxButtons.OK, MessageBoxIcon.Error);
                salary.Focus();
            }
        }
    }
}
