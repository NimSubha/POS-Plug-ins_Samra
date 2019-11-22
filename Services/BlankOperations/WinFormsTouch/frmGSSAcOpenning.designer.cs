//namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
namespace BlankOperations
//namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    partial class frmGSSAcOpenning
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if(disposing && (components != null))
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
            this.lblGss = new System.Windows.Forms.Label();
            this.lblIdType = new System.Windows.Forms.Label();
            this.cmbIdType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chkMinor = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtPhoneNumber = new System.Windows.Forms.TextBox();
            this.txtCustomerAddress = new System.Windows.Forms.TextBox();
            this.txtCustomerName = new System.Windows.Forms.TextBox();
            this.txtCustomerAccount = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtSalesPerson = new System.Windows.Forms.TextBox();
            this.btnSalesPerson = new DevExpress.XtraEditors.SimpleButton();
            this.lblEMIAmount = new System.Windows.Forms.Label();
            this.txtEMIAmount = new System.Windows.Forms.TextBox();
            this.btnNewCustomer = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnSearchCustomer = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbSchemeCode = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtIdNo = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label14 = new System.Windows.Forms.Label();
            this.txtRelationship = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.txtGuardianName = new System.Windows.Forms.TextBox();
            this.txtNEmail = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txtNPhone = new System.Windows.Forms.TextBox();
            this.txtNMobile = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtNName = new System.Windows.Forms.TextBox();
            this.btnClose = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnSubmit = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblGss
            // 
            this.lblGss.AutoSize = true;
            this.lblGss.Font = new System.Drawing.Font("Segoe UI", 18F);
            this.lblGss.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblGss.Location = new System.Drawing.Point(351, 9);
            this.lblGss.Name = "lblGss";
            this.lblGss.Size = new System.Drawing.Size(203, 32);
            this.lblGss.TabIndex = 4;
            this.lblGss.Text = "GSS A/C Opening";
            // 
            // lblIdType
            // 
            this.lblIdType.AutoSize = true;
            this.lblIdType.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.lblIdType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblIdType.Location = new System.Drawing.Point(33, 113);
            this.lblIdType.Name = "lblIdType";
            this.lblIdType.Size = new System.Drawing.Size(44, 13);
            this.lblIdType.TabIndex = 181;
            this.lblIdType.Text = "Id Type";
            // 
            // cmbIdType
            // 
            this.cmbIdType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbIdType.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.cmbIdType.FormattingEnabled = true;
            this.cmbIdType.Location = new System.Drawing.Point(140, 112);
            this.cmbIdType.Name = "cmbIdType";
            this.cmbIdType.Size = new System.Drawing.Size(211, 21);
            this.cmbIdType.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label2.Location = new System.Drawing.Point(389, 172);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 179;
            this.label2.Text = "Sales Person:";
            // 
            // chkMinor
            // 
            this.chkMinor.AutoSize = true;
            this.chkMinor.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.chkMinor.Location = new System.Drawing.Point(391, 28);
            this.chkMinor.Name = "chkMinor";
            this.chkMinor.Size = new System.Drawing.Size(52, 17);
            this.chkMinor.TabIndex = 9;
            this.chkMinor.Text = "Minor";
            this.chkMinor.UseVisualStyleBackColor = true;
            this.chkMinor.CheckedChanged += new System.EventHandler(this.chkMinor_CheckedChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.label9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label9.Location = new System.Drawing.Point(33, 88);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(77, 13);
            this.label9.TabIndex = 176;
            this.label9.Text = "Phone Number";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label1.Location = new System.Drawing.Point(33, 61);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 13);
            this.label1.TabIndex = 175;
            this.label1.Text = "Customer Address";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.label8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label8.Location = new System.Drawing.Point(396, 32);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(83, 13);
            this.label8.TabIndex = 174;
            this.label8.Text = "Customer Name";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label7.Location = new System.Drawing.Point(33, 32);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(95, 13);
            this.label7.TabIndex = 173;
            this.label7.Text = "Customer Account";
            // 
            // txtPhoneNumber
            // 
            this.txtPhoneNumber.BackColor = System.Drawing.SystemColors.Control;
            this.txtPhoneNumber.Enabled = false;
            this.txtPhoneNumber.Location = new System.Drawing.Point(140, 86);
            this.txtPhoneNumber.MaxLength = 20;
            this.txtPhoneNumber.Name = "txtPhoneNumber";
            this.txtPhoneNumber.ReadOnly = true;
            this.txtPhoneNumber.Size = new System.Drawing.Size(211, 21);
            this.txtPhoneNumber.TabIndex = 103;
            // 
            // txtCustomerAddress
            // 
            this.txtCustomerAddress.BackColor = System.Drawing.SystemColors.Control;
            this.txtCustomerAddress.Enabled = false;
            this.txtCustomerAddress.Location = new System.Drawing.Point(140, 59);
            this.txtCustomerAddress.MaxLength = 20;
            this.txtCustomerAddress.Name = "txtCustomerAddress";
            this.txtCustomerAddress.ReadOnly = true;
            this.txtCustomerAddress.Size = new System.Drawing.Size(558, 21);
            this.txtCustomerAddress.TabIndex = 102;
            // 
            // txtCustomerName
            // 
            this.txtCustomerName.BackColor = System.Drawing.SystemColors.Control;
            this.txtCustomerName.Enabled = false;
            this.txtCustomerName.Location = new System.Drawing.Point(487, 31);
            this.txtCustomerName.MaxLength = 60;
            this.txtCustomerName.Name = "txtCustomerName";
            this.txtCustomerName.ReadOnly = true;
            this.txtCustomerName.Size = new System.Drawing.Size(211, 21);
            this.txtCustomerName.TabIndex = 101;
            // 
            // txtCustomerAccount
            // 
            this.txtCustomerAccount.BackColor = System.Drawing.SystemColors.Control;
            this.txtCustomerAccount.Enabled = false;
            this.txtCustomerAccount.ForeColor = System.Drawing.Color.Navy;
            this.txtCustomerAccount.Location = new System.Drawing.Point(140, 30);
            this.txtCustomerAccount.MaxLength = 20;
            this.txtCustomerAccount.Name = "txtCustomerAccount";
            this.txtCustomerAccount.ReadOnly = true;
            this.txtCustomerAccount.Size = new System.Drawing.Size(211, 21);
            this.txtCustomerAccount.TabIndex = 100;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtSalesPerson);
            this.groupBox1.Controls.Add(this.btnSalesPerson);
            this.groupBox1.Controls.Add(this.lblEMIAmount);
            this.groupBox1.Controls.Add(this.txtEMIAmount);
            this.groupBox1.Controls.Add(this.btnNewCustomer);
            this.groupBox1.Controls.Add(this.btnSearchCustomer);
            this.groupBox1.Controls.Add(this.txtCustomerAccount);
            this.groupBox1.Controls.Add(this.txtCustomerName);
            this.groupBox1.Controls.Add(this.txtCustomerAddress);
            this.groupBox1.Controls.Add(this.txtPhoneNumber);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.cmbIdType);
            this.groupBox1.Controls.Add(this.cmbSchemeCode);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.lblIdType);
            this.groupBox1.Controls.Add(this.txtIdNo);
            this.groupBox1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.groupBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.groupBox1.Location = new System.Drawing.Point(14, 52);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(860, 216);
            this.groupBox1.TabIndex = 200;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Customer Info";
            // 
            // txtSalesPerson
            // 
            this.txtSalesPerson.Enabled = false;
            this.txtSalesPerson.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.txtSalesPerson.Location = new System.Drawing.Point(480, 168);
            this.txtSalesPerson.MaxLength = 60;
            this.txtSalesPerson.Name = "txtSalesPerson";
            this.txtSalesPerson.Size = new System.Drawing.Size(211, 21);
            this.txtSalesPerson.TabIndex = 304;
            // 
            // btnSalesPerson
            // 
            this.btnSalesPerson.Image = global::Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.search;
            this.btnSalesPerson.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnSalesPerson.Location = new System.Drawing.Point(697, 167);
            this.btnSalesPerson.Name = "btnSalesPerson";
            this.btnSalesPerson.Size = new System.Drawing.Size(57, 22);
            this.btnSalesPerson.TabIndex = 303;
            this.btnSalesPerson.Text = "Search";
            this.btnSalesPerson.Click += new System.EventHandler(this.btnSalesPerson_Click);
            // 
            // lblEMIAmount
            // 
            this.lblEMIAmount.AutoSize = true;
            this.lblEMIAmount.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.lblEMIAmount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblEMIAmount.Location = new System.Drawing.Point(33, 170);
            this.lblEMIAmount.Name = "lblEMIAmount";
            this.lblEMIAmount.Size = new System.Drawing.Size(65, 13);
            this.lblEMIAmount.TabIndex = 302;
            this.lblEMIAmount.Text = "EMI Amount";
            // 
            // txtEMIAmount
            // 
            this.txtEMIAmount.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.txtEMIAmount.Location = new System.Drawing.Point(140, 167);
            this.txtEMIAmount.MaxLength = 30;
            this.txtEMIAmount.Name = "txtEMIAmount";
            this.txtEMIAmount.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.txtEMIAmount.Size = new System.Drawing.Size(211, 21);
            this.txtEMIAmount.TabIndex = 301;
            this.txtEMIAmount.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtEMIAmount_KeyPress);
            this.txtEMIAmount.Leave += new System.EventHandler(this.txtEMIAmount_Leave);
            // 
            // btnNewCustomer
            // 
            this.btnNewCustomer.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnNewCustomer.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnNewCustomer.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnNewCustomer.Appearance.Options.UseFont = true;
            this.btnNewCustomer.Appearance.Options.UseForeColor = true;
            this.btnNewCustomer.Location = new System.Drawing.Point(729, 14);
            this.btnNewCustomer.Name = "btnNewCustomer";
            this.btnNewCustomer.Size = new System.Drawing.Size(118, 50);
            this.btnNewCustomer.TabIndex = 300;
            this.btnNewCustomer.Text = "New Customer";
            this.btnNewCustomer.Click += new System.EventHandler(this.btnNewCustomer_Click);
            // 
            // btnSearchCustomer
            // 
            this.btnSearchCustomer.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSearchCustomer.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnSearchCustomer.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnSearchCustomer.Appearance.Options.UseBackColor = true;
            this.btnSearchCustomer.Appearance.Options.UseFont = true;
            this.btnSearchCustomer.Appearance.Options.UseForeColor = true;
            this.btnSearchCustomer.Location = new System.Drawing.Point(729, 70);
            this.btnSearchCustomer.Name = "btnSearchCustomer";
            this.btnSearchCustomer.Size = new System.Drawing.Size(122, 50);
            this.btnSearchCustomer.TabIndex = 0;
            this.btnSearchCustomer.Text = "Search Customer";
            this.btnSearchCustomer.Click += new System.EventHandler(this.btnSearchCustomer_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label5.Location = new System.Drawing.Point(389, 143);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(33, 13);
            this.label5.TabIndex = 190;
            this.label5.Text = "Id No";
            // 
            // cmbSchemeCode
            // 
            this.cmbSchemeCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSchemeCode.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.cmbSchemeCode.FormattingEnabled = true;
            this.cmbSchemeCode.Location = new System.Drawing.Point(140, 140);
            this.cmbSchemeCode.Name = "cmbSchemeCode";
            this.cmbSchemeCode.Size = new System.Drawing.Size(211, 21);
            this.cmbSchemeCode.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label3.Location = new System.Drawing.Point(33, 143);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 13);
            this.label3.TabIndex = 183;
            this.label3.Text = "Scheme Code";
            // 
            // txtIdNo
            // 
            this.txtIdNo.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.txtIdNo.Location = new System.Drawing.Point(480, 142);
            this.txtIdNo.MaxLength = 60;
            this.txtIdNo.Name = "txtIdNo";
            this.txtIdNo.Size = new System.Drawing.Size(211, 21);
            this.txtIdNo.TabIndex = 2;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.txtRelationship);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.txtGuardianName);
            this.groupBox2.Controls.Add(this.txtNEmail);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.txtNPhone);
            this.groupBox2.Controls.Add(this.txtNMobile);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.txtNName);
            this.groupBox2.Controls.Add(this.chkMinor);
            this.groupBox2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.groupBox2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.groupBox2.Location = new System.Drawing.Point(15, 286);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(859, 143);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Nominee details";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.label14.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label14.Location = new System.Drawing.Point(391, 85);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(65, 13);
            this.label14.TabIndex = 201;
            this.label14.Text = "Relationship";
            // 
            // txtRelationship
            // 
            this.txtRelationship.BackColor = System.Drawing.SystemColors.Window;
            this.txtRelationship.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.txtRelationship.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtRelationship.Location = new System.Drawing.Point(479, 76);
            this.txtRelationship.MaxLength = 20;
            this.txtRelationship.Name = "txtRelationship";
            this.txtRelationship.Size = new System.Drawing.Size(211, 21);
            this.txtRelationship.TabIndex = 11;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.label11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label11.Location = new System.Drawing.Point(25, 110);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(75, 13);
            this.label11.TabIndex = 198;
            this.label11.Text = "Nominee Email";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.label12.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label12.Location = new System.Drawing.Point(391, 55);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(80, 13);
            this.label12.TabIndex = 197;
            this.label12.Text = "Guardian Name";
            // 
            // txtGuardianName
            // 
            this.txtGuardianName.BackColor = System.Drawing.SystemColors.Window;
            this.txtGuardianName.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.txtGuardianName.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtGuardianName.Location = new System.Drawing.Point(479, 47);
            this.txtGuardianName.MaxLength = 60;
            this.txtGuardianName.Name = "txtGuardianName";
            this.txtGuardianName.Size = new System.Drawing.Size(211, 21);
            this.txtGuardianName.TabIndex = 10;
            // 
            // txtNEmail
            // 
            this.txtNEmail.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.txtNEmail.Location = new System.Drawing.Point(132, 106);
            this.txtNEmail.MaxLength = 80;
            this.txtNEmail.Name = "txtNEmail";
            this.txtNEmail.Size = new System.Drawing.Size(211, 21);
            this.txtNEmail.TabIndex = 8;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label6.Location = new System.Drawing.Point(25, 55);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(81, 13);
            this.label6.TabIndex = 194;
            this.label6.Text = "Nominee Mobile";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.label10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label10.Location = new System.Drawing.Point(25, 84);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(81, 13);
            this.label10.TabIndex = 193;
            this.label10.Text = "Nominee Phone";
            // 
            // txtNPhone
            // 
            this.txtNPhone.BackColor = System.Drawing.SystemColors.Window;
            this.txtNPhone.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.txtNPhone.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtNPhone.Location = new System.Drawing.Point(132, 79);
            this.txtNPhone.MaxLength = 20;
            this.txtNPhone.Name = "txtNPhone";
            this.txtNPhone.Size = new System.Drawing.Size(211, 21);
            this.txtNPhone.TabIndex = 7;
            // 
            // txtNMobile
            // 
            this.txtNMobile.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.txtNMobile.Location = new System.Drawing.Point(132, 52);
            this.txtNMobile.MaxLength = 20;
            this.txtNMobile.Name = "txtNMobile";
            this.txtNMobile.Size = new System.Drawing.Size(211, 21);
            this.txtNMobile.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label4.Location = new System.Drawing.Point(25, 28);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(78, 13);
            this.label4.TabIndex = 189;
            this.label4.Text = "Nominee Name";
            // 
            // txtNName
            // 
            this.txtNName.BackColor = System.Drawing.SystemColors.Window;
            this.txtNName.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.txtNName.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtNName.Location = new System.Drawing.Point(132, 24);
            this.txtNName.MaxLength = 59;
            this.txtNName.Name = "txtNName";
            this.txtNName.Size = new System.Drawing.Size(211, 21);
            this.txtNName.TabIndex = 5;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnClose.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnClose.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnClose.Appearance.Options.UseFont = true;
            this.btnClose.Appearance.Options.UseForeColor = true;
            this.btnClose.Location = new System.Drawing.Point(756, 452);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(118, 50);
            this.btnClose.TabIndex = 13;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSubmit
            // 
            this.btnSubmit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSubmit.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnSubmit.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnSubmit.Appearance.Options.UseFont = true;
            this.btnSubmit.Appearance.Options.UseForeColor = true;
            this.btnSubmit.Location = new System.Drawing.Point(628, 452);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(118, 50);
            this.btnSubmit.TabIndex = 12;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // frmGSSAcOpenning
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(885, 512);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblGss);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmGSSAcOpenning";
            this.Text = "GSS A/C Opening";
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblGss;
        private System.Windows.Forms.Label lblIdType;
        private System.Windows.Forms.ComboBox cmbIdType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkMinor;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        public System.Windows.Forms.TextBox txtPhoneNumber;
        public System.Windows.Forms.TextBox txtCustomerAddress;
        public System.Windows.Forms.TextBox txtCustomerName;
        public System.Windows.Forms.TextBox txtCustomerAccount;
        private System.Windows.Forms.GroupBox groupBox1;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnNewCustomer;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnSearchCustomer;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox cmbSchemeCode;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtNName;
        private System.Windows.Forms.TextBox txtIdNo;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txtRelationship;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtGuardianName;
        private System.Windows.Forms.TextBox txtNEmail;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtNPhone;
        private System.Windows.Forms.TextBox txtNMobile;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnClose;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnSubmit;
        private System.Windows.Forms.Label lblEMIAmount;
        private System.Windows.Forms.TextBox txtEMIAmount;
        private DevExpress.XtraEditors.SimpleButton btnSalesPerson;
        private System.Windows.Forms.TextBox txtSalesPerson;
    }
}