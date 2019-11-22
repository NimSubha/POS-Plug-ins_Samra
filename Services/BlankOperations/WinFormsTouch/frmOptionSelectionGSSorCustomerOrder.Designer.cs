namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    partial class frmOptionSelectionGSSorCustomerOrder
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
            this.lblOption = new System.Windows.Forms.Label();
            this.btnNormalCustomerDeposit = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnGSS = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            this.SuspendLayout();
            // 
            // lblOption
            // 
            this.lblOption.AutoSize = true;
            this.lblOption.Font = new System.Drawing.Font("Segoe UI Light", 36F);
            this.lblOption.ForeColor = System.Drawing.Color.Black;
            this.lblOption.Location = new System.Drawing.Point(10, 5);
            this.lblOption.Name = "lblOption";
            this.lblOption.Size = new System.Drawing.Size(524, 65);
            this.lblOption.TabIndex = 0;
            this.lblOption.Text = "Please select one Option";
            // 
            // btnNormalCustomerDeposit
            // 
            this.btnNormalCustomerDeposit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNormalCustomerDeposit.Appearance.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNormalCustomerDeposit.Appearance.Options.UseFont = true;
            this.btnNormalCustomerDeposit.Location = new System.Drawing.Point(12, 77);
            this.btnNormalCustomerDeposit.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.btnNormalCustomerDeposit.Name = "btnNormalCustomerDeposit";
            this.btnNormalCustomerDeposit.Padding = new System.Windows.Forms.Padding(0);
            this.btnNormalCustomerDeposit.ShowToolTips = false;
            this.btnNormalCustomerDeposit.Size = new System.Drawing.Size(521, 60);
            this.btnNormalCustomerDeposit.TabIndex = 1;
            this.btnNormalCustomerDeposit.Tag = "btnNormalCustomerDeposit";
            this.btnNormalCustomerDeposit.Text = "Normal Customer Deposit";
            this.btnNormalCustomerDeposit.Click += new System.EventHandler(this.btnNormalCustomerDeposit_Click);
            // 
            // btnGSS
            // 
            this.btnGSS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGSS.Appearance.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGSS.Appearance.Options.UseFont = true;
            this.btnGSS.Location = new System.Drawing.Point(12, 143);
            this.btnGSS.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.btnGSS.Name = "btnGSS";
            this.btnGSS.Padding = new System.Windows.Forms.Padding(0);
            this.btnGSS.ShowToolTips = false;
            this.btnGSS.Size = new System.Drawing.Size(521, 60);
            this.btnGSS.TabIndex = 1;
            this.btnGSS.Tag = "btnGSS";
            this.btnGSS.Text = "Gold Saving Scheme (GSS)";
            this.btnGSS.Click += new System.EventHandler(this.button2_Click);
            // 
            // frmOptionSelectionGSSorCustomerOrder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(545, 213);
            this.ControlBox = false;
            this.Controls.Add(this.btnGSS);
            this.Controls.Add(this.btnNormalCustomerDeposit);
            this.Controls.Add(this.lblOption);
            this.HelpButton = false;
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmOptionSelectionGSSorCustomerOrder";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Please select the Option for Customer Deposit";
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion


        private System.Windows.Forms.Label lblOption;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnNormalCustomerDeposit;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnGSS;
    }
}