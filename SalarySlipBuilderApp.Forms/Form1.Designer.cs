namespace SalarySlipBuilderApp.SalarySlipBuilderApp.Forms
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.requiredName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.pan = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.accountNumber = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.designation = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.salary = new System.Windows.Forms.TextBox();
            this.generateButton = new System.Windows.Forms.Button();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.addOuterPanel = new System.Windows.Forms.Panel();
            this.deductOuterPanel = new System.Windows.Forms.Panel();
            this.addInnerPanel = new System.Windows.Forms.Panel();
            this.addComponent = new System.Windows.Forms.Button();
            this.addComponentNumber = new System.Windows.Forms.TextBox();
            this.deductInnerPanel = new System.Windows.Forms.Panel();
            this.deductComponent = new System.Windows.Forms.Button();
            this.deductComponentNumber = new System.Windows.Forms.TextBox();
            this.emailLabel = new System.Windows.Forms.Label();
            this.email = new System.Windows.Forms.TextBox();
            this.dateOfJoining = new System.Windows.Forms.DateTimePicker();
            this.month = new System.Windows.Forms.ComboBox();
            this.monthLabel = new System.Windows.Forms.Label();
            this.yearLabel = new System.Windows.Forms.Label();
            this.year = new System.Windows.Forms.ComboBox();
            this.associateCodeLabel = new System.Windows.Forms.Label();
            this.associateCode = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.addInnerPanel.SuspendLayout();
            this.deductInnerPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(304, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name";
            // 
            // requiredName
            // 
            this.requiredName.Location = new System.Drawing.Point(373, 21);
            this.requiredName.Name = "requiredName";
            this.requiredName.Size = new System.Drawing.Size(163, 20);
            this.requiredName.TabIndex = 1;
            this.requiredName.Validating += new System.ComponentModel.CancelEventHandler(this.requiredName_Validating);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Date of Joining";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(304, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "PAN";
            // 
            // pan
            // 
            this.pan.Location = new System.Drawing.Point(373, 52);
            this.pan.Name = "pan";
            this.pan.Size = new System.Drawing.Size(163, 20);
            this.pan.TabIndex = 5;
            this.pan.Validating += new System.ComponentModel.CancelEventHandler(this.pan_Validating);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 79);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Account Number";
            // 
            // accountNumber
            // 
            this.accountNumber.Location = new System.Drawing.Point(94, 79);
            this.accountNumber.Name = "accountNumber";
            this.accountNumber.Size = new System.Drawing.Size(196, 20);
            this.accountNumber.TabIndex = 7;
            this.accountNumber.Validating += new System.ComponentModel.CancelEventHandler(this.accountNumber_Validating);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(304, 82);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(63, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Designation";
            // 
            // designation
            // 
            this.designation.Location = new System.Drawing.Point(373, 79);
            this.designation.Name = "designation";
            this.designation.Size = new System.Drawing.Size(163, 20);
            this.designation.TabIndex = 9;
            this.designation.Validating += new System.ComponentModel.CancelEventHandler(this.designation_Validating);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 106);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(36, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Salary";
            // 
            // salary
            // 
            this.salary.Location = new System.Drawing.Point(94, 103);
            this.salary.Name = "salary";
            this.salary.Size = new System.Drawing.Size(196, 20);
            this.salary.TabIndex = 11;
            this.salary.Validating += new System.ComponentModel.CancelEventHandler(this.salary_Validating);
            // 
            // generateButton
            // 
            this.generateButton.Location = new System.Drawing.Point(219, 289);
            this.generateButton.Name = "generateButton";
            this.generateButton.Size = new System.Drawing.Size(129, 23);
            this.generateButton.TabIndex = 12;
            this.generateButton.Text = "Generate";
            this.generateButton.UseVisualStyleBackColor = true;
            this.generateButton.Click += new System.EventHandler(this.generateButton_Click);
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Location = new System.Drawing.Point(13, 318);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.Size = new System.Drawing.Size(542, 191);
            this.dataGridView.TabIndex = 13;
            // 
            // addOuterPanel
            // 
            this.addOuterPanel.Location = new System.Drawing.Point(22, 197);
            this.addOuterPanel.Name = "addOuterPanel";
            this.addOuterPanel.Size = new System.Drawing.Size(200, 86);
            this.addOuterPanel.TabIndex = 14;
            // 
            // deductOuterPanel
            // 
            this.deductOuterPanel.Location = new System.Drawing.Point(340, 197);
            this.deductOuterPanel.Name = "deductOuterPanel";
            this.deductOuterPanel.Size = new System.Drawing.Size(200, 86);
            this.deductOuterPanel.TabIndex = 15;
            // 
            // addInnerPanel
            // 
            this.addInnerPanel.Controls.Add(this.addComponent);
            this.addInnerPanel.Controls.Add(this.addComponentNumber);
            this.addInnerPanel.Location = new System.Drawing.Point(22, 169);
            this.addInnerPanel.Name = "addInnerPanel";
            this.addInnerPanel.Size = new System.Drawing.Size(200, 31);
            this.addInnerPanel.TabIndex = 0;
            // 
            // addComponent
            // 
            this.addComponent.Location = new System.Drawing.Point(141, 3);
            this.addComponent.Name = "addComponent";
            this.addComponent.Size = new System.Drawing.Size(56, 23);
            this.addComponent.TabIndex = 1;
            this.addComponent.Text = "AC";
            this.addComponent.UseVisualStyleBackColor = true;
            this.addComponent.Click += new System.EventHandler(this.addComponent_Click);
            // 
            // addComponentNumber
            // 
            this.addComponentNumber.Location = new System.Drawing.Point(3, 5);
            this.addComponentNumber.Name = "addComponentNumber";
            this.addComponentNumber.Size = new System.Drawing.Size(111, 20);
            this.addComponentNumber.TabIndex = 0;
            this.addComponentNumber.Validating += new System.ComponentModel.CancelEventHandler(this.addComponentNumber_Validating);
            // 
            // deductInnerPanel
            // 
            this.deductInnerPanel.Controls.Add(this.deductComponent);
            this.deductInnerPanel.Controls.Add(this.deductComponentNumber);
            this.deductInnerPanel.Location = new System.Drawing.Point(340, 169);
            this.deductInnerPanel.Name = "deductInnerPanel";
            this.deductInnerPanel.Size = new System.Drawing.Size(200, 31);
            this.deductInnerPanel.TabIndex = 1;
            // 
            // deductComponent
            // 
            this.deductComponent.Location = new System.Drawing.Point(141, 5);
            this.deductComponent.Name = "deductComponent";
            this.deductComponent.Size = new System.Drawing.Size(56, 23);
            this.deductComponent.TabIndex = 2;
            this.deductComponent.Text = "DC";
            this.deductComponent.UseVisualStyleBackColor = true;
            this.deductComponent.Click += new System.EventHandler(this.deductComponent_Click);
            // 
            // deductComponentNumber
            // 
            this.deductComponentNumber.Location = new System.Drawing.Point(3, 5);
            this.deductComponentNumber.Name = "deductComponentNumber";
            this.deductComponentNumber.Size = new System.Drawing.Size(111, 20);
            this.deductComponentNumber.TabIndex = 2;
            this.deductComponentNumber.Validating += new System.ComponentModel.CancelEventHandler(this.deductComponentNumber_Validating);
            // 
            // emailLabel
            // 
            this.emailLabel.AutoSize = true;
            this.emailLabel.Location = new System.Drawing.Point(7, 131);
            this.emailLabel.Name = "emailLabel";
            this.emailLabel.Size = new System.Drawing.Size(32, 13);
            this.emailLabel.TabIndex = 16;
            this.emailLabel.Text = "Email";
            // 
            // email
            // 
            this.email.Location = new System.Drawing.Point(94, 131);
            this.email.Name = "email";
            this.email.Size = new System.Drawing.Size(196, 20);
            this.email.TabIndex = 17;
            // 
            // dateOfJoining
            // 
            this.dateOfJoining.Location = new System.Drawing.Point(94, 52);
            this.dateOfJoining.Name = "dateOfJoining";
            this.dateOfJoining.Size = new System.Drawing.Size(196, 20);
            this.dateOfJoining.TabIndex = 3;
            // 
            // month
            // 
            this.month.FormattingEnabled = true;
            this.month.Location = new System.Drawing.Point(373, 103);
            this.month.Name = "month";
            this.month.Size = new System.Drawing.Size(89, 21);
            this.month.TabIndex = 18;
            // 
            // monthLabel
            // 
            this.monthLabel.AutoSize = true;
            this.monthLabel.Location = new System.Drawing.Point(304, 108);
            this.monthLabel.Name = "monthLabel";
            this.monthLabel.Size = new System.Drawing.Size(37, 13);
            this.monthLabel.TabIndex = 19;
            this.monthLabel.Text = "Month";
            // 
            // yearLabel
            // 
            this.yearLabel.AutoSize = true;
            this.yearLabel.Location = new System.Drawing.Point(468, 108);
            this.yearLabel.Name = "yearLabel";
            this.yearLabel.Size = new System.Drawing.Size(29, 13);
            this.yearLabel.TabIndex = 20;
            this.yearLabel.Text = "Year";
            // 
            // year
            // 
            this.year.FormattingEnabled = true;
            this.year.Location = new System.Drawing.Point(501, 103);
            this.year.Name = "year";
            this.year.Size = new System.Drawing.Size(54, 21);
            this.year.TabIndex = 21;
            // 
            // associateCodeLabel
            // 
            this.associateCodeLabel.AutoSize = true;
            this.associateCodeLabel.Location = new System.Drawing.Point(7, 24);
            this.associateCodeLabel.Name = "associateCodeLabel";
            this.associateCodeLabel.Size = new System.Drawing.Size(81, 13);
            this.associateCodeLabel.TabIndex = 22;
            this.associateCodeLabel.Text = "Associate Code";
            // 
            // associateCode
            // 
            this.associateCode.Location = new System.Drawing.Point(94, 21);
            this.associateCode.Name = "associateCode";
            this.associateCode.Size = new System.Drawing.Size(196, 20);
            this.associateCode.TabIndex = 23;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ClientSize = new System.Drawing.Size(567, 509);
            this.Controls.Add(this.associateCode);
            this.Controls.Add(this.associateCodeLabel);
            this.Controls.Add(this.year);
            this.Controls.Add(this.yearLabel);
            this.Controls.Add(this.monthLabel);
            this.Controls.Add(this.month);
            this.Controls.Add(this.email);
            this.Controls.Add(this.emailLabel);
            this.Controls.Add(this.deductInnerPanel);
            this.Controls.Add(this.addInnerPanel);
            this.Controls.Add(this.deductOuterPanel);
            this.Controls.Add(this.addOuterPanel);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.generateButton);
            this.Controls.Add(this.salary);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.designation);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.accountNumber);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.pan);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dateOfJoining);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.requiredName);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Salary Slip Application";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.addInnerPanel.ResumeLayout(false);
            this.addInnerPanel.PerformLayout();
            this.deductInnerPanel.ResumeLayout(false);
            this.deductInnerPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox requiredName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox pan;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox accountNumber;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox designation;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox salary;
        private System.Windows.Forms.Button generateButton;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.Panel addOuterPanel;
        private System.Windows.Forms.Panel deductOuterPanel;
        private System.Windows.Forms.Panel addInnerPanel;
        private System.Windows.Forms.Panel deductInnerPanel;
        private System.Windows.Forms.Button addComponent;
        private System.Windows.Forms.TextBox addComponentNumber;
        private System.Windows.Forms.Button deductComponent;
        private System.Windows.Forms.TextBox deductComponentNumber;
        private System.Windows.Forms.Label emailLabel;
        private System.Windows.Forms.TextBox email;
        private System.Windows.Forms.DateTimePicker dateOfJoining;
        private System.Windows.Forms.ComboBox month;
        private System.Windows.Forms.Label monthLabel;
        private System.Windows.Forms.Label yearLabel;
        private System.Windows.Forms.ComboBox year;
        private System.Windows.Forms.Label associateCodeLabel;
        private System.Windows.Forms.TextBox associateCode;
    }
}