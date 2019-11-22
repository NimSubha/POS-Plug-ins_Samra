namespace EFT
{
    partial class frmCard
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
            this.label2 = new System.Windows.Forms.Label();
            this.cardNumber = new System.Windows.Forms.TextBox();
            this.expDate = new System.Windows.Forms.TextBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.AmountPaid = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.AmountDue = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblApprovalCode = new System.Windows.Forms.Label();
            this.txtApprovalCode = new System.Windows.Forms.TextBox();
            this.txtExpMonth = new System.Windows.Forms.TextBox();
            this.lblExpMonth = new System.Windows.Forms.Label();
            this.txtExpYear = new System.Windows.Forms.TextBox();
            this.lblExpYear = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 70);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Card number";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 334);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Exp date";
            this.label2.Visible = false;
            // 
            // cardNumber
            // 
            this.cardNumber.Location = new System.Drawing.Point(104, 70);
            this.cardNumber.Name = "cardNumber";
            this.cardNumber.Size = new System.Drawing.Size(148, 20);
            this.cardNumber.TabIndex = 1;
            this.cardNumber.Validating += new System.ComponentModel.CancelEventHandler(this.cardNumber_Validating);
            // 
            // expDate
            // 
            this.expDate.Location = new System.Drawing.Point(97, 334);
            this.expDate.Name = "expDate";
            this.expDate.Size = new System.Drawing.Size(148, 20);
            this.expDate.TabIndex = 14;
            this.expDate.Visible = false;
            this.expDate.Validating += new System.ComponentModel.CancelEventHandler(this.expDate_Validating);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(97, 307);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 7;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // AmountPaid
            // 
            this.AmountPaid.Location = new System.Drawing.Point(104, 162);
            this.AmountPaid.Margin = new System.Windows.Forms.Padding(2);
            this.AmountPaid.Name = "AmountPaid";
            this.AmountPaid.Size = new System.Drawing.Size(148, 20);
            this.AmountPaid.TabIndex = 4;
            this.AmountPaid.Validating += new System.ComponentModel.CancelEventHandler(this.AmountPaid_Validating);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 162);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Amount";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(22, 192);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(73, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Security Code";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(22, 221);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Address";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(22, 250);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(84, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "Zip/Postal Code";
            // 
            // AmountDue
            // 
            this.AmountDue.Enabled = false;
            this.AmountDue.Location = new System.Drawing.Point(104, 39);
            this.AmountDue.Name = "AmountDue";
            this.AmountDue.Size = new System.Drawing.Size(148, 20);
            this.AmountDue.TabIndex = 0;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.Red;
            this.label7.Location = new System.Drawing.Point(22, 39);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(66, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Amount Due";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(172, 307);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblApprovalCode
            // 
            this.lblApprovalCode.AutoSize = true;
            this.lblApprovalCode.Location = new System.Drawing.Point(22, 279);
            this.lblApprovalCode.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblApprovalCode.Name = "lblApprovalCode";
            this.lblApprovalCode.Size = new System.Drawing.Size(77, 13);
            this.lblApprovalCode.TabIndex = 11;
            this.lblApprovalCode.Text = "Approval Code";
            // 
            // txtApprovalCode
            // 
            this.txtApprovalCode.Location = new System.Drawing.Point(103, 276);
            this.txtApprovalCode.MaxLength = 20;
            this.txtApprovalCode.Name = "txtApprovalCode";
            this.txtApprovalCode.Size = new System.Drawing.Size(148, 20);
            this.txtApprovalCode.TabIndex = 5;
            // 
            // txtExpMonth
            // 
            this.txtExpMonth.Location = new System.Drawing.Point(103, 99);
            this.txtExpMonth.MaxLength = 2;
            this.txtExpMonth.Name = "txtExpMonth";
            this.txtExpMonth.Size = new System.Drawing.Size(148, 20);
            this.txtExpMonth.TabIndex = 2;
            this.txtExpMonth.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtExpMonth_KeyPress);
            this.txtExpMonth.Leave += new System.EventHandler(this.txtExpMonth_Leave);
            // 
            // lblExpMonth
            // 
            this.lblExpMonth.AutoSize = true;
            this.lblExpMonth.Location = new System.Drawing.Point(22, 99);
            this.lblExpMonth.Name = "lblExpMonth";
            this.lblExpMonth.Size = new System.Drawing.Size(58, 13);
            this.lblExpMonth.TabIndex = 13;
            this.lblExpMonth.Text = "Exp Month";
            // 
            // txtExpYear
            // 
            this.txtExpYear.Location = new System.Drawing.Point(103, 129);
            this.txtExpYear.MaxLength = 2;
            this.txtExpYear.Name = "txtExpYear";
            this.txtExpYear.Size = new System.Drawing.Size(148, 20);
            this.txtExpYear.TabIndex = 3;
            this.txtExpYear.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtExpYear_KeyPress);
            this.txtExpYear.Leave += new System.EventHandler(this.txtExpYear_Leave);
            // 
            // lblExpYear
            // 
            this.lblExpYear.AutoSize = true;
            this.lblExpYear.Location = new System.Drawing.Point(22, 129);
            this.lblExpYear.Name = "lblExpYear";
            this.lblExpYear.Size = new System.Drawing.Size(50, 13);
            this.lblExpYear.TabIndex = 15;
            this.lblExpYear.Text = "Exp Year";
            // 
            // frmCard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(291, 357);
            this.Controls.Add(this.txtExpYear);
            this.Controls.Add(this.lblExpYear);
            this.Controls.Add(this.txtExpMonth);
            this.Controls.Add(this.lblExpMonth);
            this.Controls.Add(this.txtApprovalCode);
            this.Controls.Add(this.lblApprovalCode);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.AmountDue);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.AmountPaid);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.expDate);
            this.Controls.Add(this.cardNumber);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "frmCard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Enter card info";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmCard_FormClosed);
            this.Load += new System.EventHandler(this.frmCard_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox cardNumber;
        private System.Windows.Forms.TextBox expDate;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.TextBox AmountPaid;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox AmountDue;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblApprovalCode;
        private System.Windows.Forms.TextBox txtApprovalCode;
        private System.Windows.Forms.TextBox txtExpMonth;
        private System.Windows.Forms.Label lblExpMonth;
        private System.Windows.Forms.TextBox txtExpYear;
        private System.Windows.Forms.Label lblExpYear;
    }
}