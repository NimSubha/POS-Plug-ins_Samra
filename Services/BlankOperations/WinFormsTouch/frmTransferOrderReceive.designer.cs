namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    partial class frmTransferOrderReceive
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
            this.lblTitle = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnTransferSearch = new DevExpress.XtraEditors.SimpleButton();
            this.txtTransferId = new System.Windows.Forms.TextBox();
            this.lblTransferId = new System.Windows.Forms.Label();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblTitle.Location = new System.Drawing.Point(235, 21);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(258, 32);
            this.lblTitle.TabIndex = 15;
            this.lblTitle.Text = "Transfer Order Receive";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btnTransferSearch);
            this.panel1.Controls.Add(this.txtTransferId);
            this.panel1.Controls.Add(this.lblTransferId);
            this.panel1.Location = new System.Drawing.Point(9, 77);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(720, 83);
            this.panel1.TabIndex = 16;
            // 
            // btnTransferSearch
            // 
            this.btnTransferSearch.Image = global::Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.search;
            this.btnTransferSearch.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnTransferSearch.Location = new System.Drawing.Point(281, 18);
            this.btnTransferSearch.Name = "btnTransferSearch";
            this.btnTransferSearch.Size = new System.Drawing.Size(57, 32);
            this.btnTransferSearch.TabIndex = 6;
            this.btnTransferSearch.Text = "Search";
            this.btnTransferSearch.Click += new System.EventHandler(this.btnTransferSearch_Click);
            // 
            // txtTransferId
            // 
            this.txtTransferId.BackColor = System.Drawing.SystemColors.Control;
            this.txtTransferId.Location = new System.Drawing.Point(105, 23);
            this.txtTransferId.MaxLength = 20;
            this.txtTransferId.Name = "txtTransferId";
            this.txtTransferId.ReadOnly = true;
            this.txtTransferId.Size = new System.Drawing.Size(170, 21);
            this.txtTransferId.TabIndex = 5;
            // 
            // lblTransferId
            // 
            this.lblTransferId.AutoSize = true;
            this.lblTransferId.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblTransferId.Location = new System.Drawing.Point(6, 25);
            this.lblTransferId.Name = "lblTransferId";
            this.lblTransferId.Size = new System.Drawing.Size(96, 13);
            this.lblTransferId.TabIndex = 4;
            this.lblTransferId.Text = "Transfer Order Id:";
            // 
            // btnSubmit
            // 
            this.btnSubmit.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnSubmit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnSubmit.Location = new System.Drawing.Point(535, 187);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(94, 43);
            this.btnSubmit.TabIndex = 17;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // btnClose
            // 
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnClose.Location = new System.Drawing.Point(635, 187);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(94, 43);
            this.btnClose.TabIndex = 18;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // frmTransferOrderReceive
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(739, 243);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblTitle);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmTransferOrderReceive";
            this.Text = "Transfer Order Receive";
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel panel1;
        private DevExpress.XtraEditors.SimpleButton btnTransferSearch;
        public System.Windows.Forms.TextBox txtTransferId;
        private System.Windows.Forms.Label lblTransferId;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Button btnClose;
    }
}