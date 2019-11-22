namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    partial class frmTransferOrderPrint
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnDetail = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtTransOrdrId = new System.Windows.Forms.TextBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnSummary = new System.Windows.Forms.Button();
            this.btnGTId = new DevExpress.XtraEditors.SimpleButton();
            this.lblTransOrdrId = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblTitle.Location = new System.Drawing.Point(111, 22);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(225, 32);
            this.lblTitle.TabIndex = 20;
            this.lblTitle.Text = "Transfer Order Print";
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.btnDetail);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.txtTransOrdrId);
            this.panel2.Controls.Add(this.btnClose);
            this.panel2.Controls.Add(this.btnSummary);
            this.panel2.Controls.Add(this.btnGTId);
            this.panel2.Controls.Add(this.lblTransOrdrId);
            this.panel2.Location = new System.Drawing.Point(9, 71);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(410, 180);
            this.panel2.TabIndex = 19;
            // 
            // btnDetail
            // 
            this.btnDetail.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnDetail.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnDetail.Location = new System.Drawing.Point(207, 63);
            this.btnDetail.Name = "btnDetail";
            this.btnDetail.Size = new System.Drawing.Size(94, 43);
            this.btnDetail.TabIndex = 3;
            this.btnDetail.Text = "Detail";
            this.btnDetail.UseVisualStyleBackColor = true;
            this.btnDetail.Click += new System.EventHandler(this.btnDetail_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label1.Location = new System.Drawing.Point(6, 79);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 92;
            this.label1.Text = "Print Option :";
            // 
            // txtTransOrdrId
            // 
            this.txtTransOrdrId.Location = new System.Drawing.Point(107, 26);
            this.txtTransOrdrId.Name = "txtTransOrdrId";
            this.txtTransOrdrId.Size = new System.Drawing.Size(214, 21);
            this.txtTransOrdrId.TabIndex = 1;
            // 
            // btnClose
            // 
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnClose.Location = new System.Drawing.Point(156, 124);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(94, 43);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSummary
            // 
            this.btnSummary.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnSummary.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnSummary.Location = new System.Drawing.Point(107, 63);
            this.btnSummary.Name = "btnSummary";
            this.btnSummary.Size = new System.Drawing.Size(94, 43);
            this.btnSummary.TabIndex = 2;
            this.btnSummary.Text = "Summary";
            this.btnSummary.UseVisualStyleBackColor = true;
            this.btnSummary.Click += new System.EventHandler(this.btnSummary_Click);
            // 
            // btnGTId
            // 
            this.btnGTId.Image = global::Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.search;
            this.btnGTId.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnGTId.Location = new System.Drawing.Point(652, 67);
            this.btnGTId.Name = "btnGTId";
            this.btnGTId.Size = new System.Drawing.Size(57, 32);
            this.btnGTId.TabIndex = 9;
            this.btnGTId.Text = "Search";
            // 
            // lblTransOrdrId
            // 
            this.lblTransOrdrId.AutoSize = true;
            this.lblTransOrdrId.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblTransOrdrId.Location = new System.Drawing.Point(6, 26);
            this.lblTransOrdrId.Name = "lblTransOrdrId";
            this.lblTransOrdrId.Size = new System.Drawing.Size(99, 13);
            this.lblTransOrdrId.TabIndex = 1;
            this.lblTransOrdrId.Text = "Transfer Order Id :";
            // 
            // frmTransferOrderPrint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(435, 263);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.panel2);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmTransferOrderPrint";
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox txtTransOrdrId;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnSummary;
        private DevExpress.XtraEditors.SimpleButton btnGTId;
        private System.Windows.Forms.Label lblTransOrdrId;
        private System.Windows.Forms.Button btnDetail;
        private System.Windows.Forms.Label label1;
    }
}