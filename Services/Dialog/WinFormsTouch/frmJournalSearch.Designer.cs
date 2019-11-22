/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

namespace Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch
{
    partial class frmJournalSearch
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
            this.basePanel = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBoxTransactionId = new System.Windows.Forms.GroupBox();
            this.btnTransactionId = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnClear = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.txtTransactionId = new DevExpress.XtraEditors.TextEdit();
            this.lblTransactionIdHeading = new DevExpress.XtraEditors.LabelControl();
            this.groupBoxDate = new System.Windows.Forms.GroupBox();
            this.btnDate = new DevExpress.XtraEditors.SimpleButton();
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            this.lblHeader = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            this.basePanel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBoxTransactionId.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtTransactionId.Properties)).BeginInit();
            this.groupBoxDate.SuspendLayout();
            this.SuspendLayout();
            // 
            // basePanel
            // 
            this.basePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.basePanel.Controls.Add(this.tableLayoutPanel1);
            this.basePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.basePanel.Location = new System.Drawing.Point(0, 0);
            this.basePanel.Name = "basePanel";
            this.basePanel.Size = new System.Drawing.Size(1018, 743);
            this.basePanel.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.groupBoxTransactionId, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupBoxDate, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnClose, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.lblHeader, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(30, 40, 30, 11);
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1016, 741);
            this.tableLayoutPanel1.TabIndex = 40;
            // 
            // groupBoxTransactionId
            // 
            this.groupBoxTransactionId.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.groupBoxTransactionId.Controls.Add(this.btnTransactionId);
            this.groupBoxTransactionId.Controls.Add(this.btnClear);
            this.groupBoxTransactionId.Controls.Add(this.txtTransactionId);
            this.groupBoxTransactionId.Controls.Add(this.lblTransactionIdHeading);
            this.groupBoxTransactionId.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.groupBoxTransactionId.Location = new System.Drawing.Point(243, 188);
            this.groupBoxTransactionId.Name = "groupBoxTransactionId";
            this.groupBoxTransactionId.Size = new System.Drawing.Size(530, 209);
            this.groupBoxTransactionId.TabIndex = 0;
            this.groupBoxTransactionId.TabStop = false;
            this.groupBoxTransactionId.Text = "Transaction Id";
            // 
            // btnTransactionId
            // 
            this.btnTransactionId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTransactionId.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTransactionId.Appearance.Options.UseFont = true;
            this.btnTransactionId.Image = global::Microsoft.Dynamics.Retail.Pos.Dialog.Properties.Resources.search;
            this.btnTransactionId.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnTransactionId.Location = new System.Drawing.Point(355, 97);
            this.btnTransactionId.Name = "btnTransactionId";
            this.btnTransactionId.Padding = new System.Windows.Forms.Padding(0);
            this.btnTransactionId.Size = new System.Drawing.Size(57, 32);
            this.btnTransactionId.TabIndex = 35;
            this.btnTransactionId.Tag = "";
            this.btnTransactionId.Click += new System.EventHandler(this.btnTransactionId_Click);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.Appearance.Options.UseFont = true;
            this.btnClear.Image = global::Microsoft.Dynamics.Retail.Pos.Dialog.Properties.Resources.remove;
            this.btnClear.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnClear.Location = new System.Drawing.Point(418, 97);
            this.btnClear.Name = "btnClear";
            this.btnClear.Padding = new System.Windows.Forms.Padding(0);
            this.btnClear.Size = new System.Drawing.Size(57, 32);
            this.btnClear.TabIndex = 36;
            this.btnClear.Tag = "";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // txtTransactionId
            // 
            this.txtTransactionId.Location = new System.Drawing.Point(43, 97);
            this.txtTransactionId.Name = "txtTransactionId";
            this.txtTransactionId.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.txtTransactionId.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.txtTransactionId.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtTransactionId.Properties.Appearance.Options.UseBackColor = true;
            this.txtTransactionId.Properties.Appearance.Options.UseFont = true;
            this.txtTransactionId.Properties.Appearance.Options.UseForeColor = true;
            this.txtTransactionId.Properties.Appearance.Options.UseTextOptions = true;
            this.txtTransactionId.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.txtTransactionId.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.txtTransactionId.Size = new System.Drawing.Size(306, 32);
            this.txtTransactionId.TabIndex = 1;
            this.txtTransactionId.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTransactionId_KeyDown);
            // 
            // lblTransactionIdHeading
            // 
            this.lblTransactionIdHeading.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblTransactionIdHeading.Location = new System.Drawing.Point(43, 75);
            this.lblTransactionIdHeading.Name = "lblTransactionIdHeading";
            this.lblTransactionIdHeading.Size = new System.Drawing.Size(85, 17);
            this.lblTransactionIdHeading.TabIndex = 33;
            this.lblTransactionIdHeading.Text = "Transaction Id:";
            // 
            // groupBoxDate
            // 
            this.groupBoxDate.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.groupBoxDate.Controls.Add(this.btnDate);
            this.groupBoxDate.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.groupBoxDate.Location = new System.Drawing.Point(243, 403);
            this.groupBoxDate.Name = "groupBoxDate";
            this.groupBoxDate.Size = new System.Drawing.Size(530, 125);
            this.groupBoxDate.TabIndex = 37;
            this.groupBoxDate.TabStop = false;
            this.groupBoxDate.Text = "Date";
            // 
            // btnDate
            // 
            this.btnDate.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnDate.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDate.Appearance.Options.UseFont = true;
            this.btnDate.Location = new System.Drawing.Point(201, 42);
            this.btnDate.Name = "btnDate";
            this.btnDate.Size = new System.Drawing.Size(127, 57);
            this.btnDate.TabIndex = 38;
            this.btnDate.Tag = "";
            this.btnDate.Text = "Select date";
            this.btnDate.Click += new System.EventHandler(this.btnDate_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnClose.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Appearance.Options.UseFont = true;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(444, 669);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(127, 57);
            this.btnClose.TabIndex = 39;
            this.btnClose.Tag = "BtnLong";
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblHeader
            // 
            this.lblHeader.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblHeader.AutoSize = true;
            this.lblHeader.Font = new System.Drawing.Font("Segoe UI Light", 36F);
            this.lblHeader.Location = new System.Drawing.Point(350, 40);
            this.lblHeader.Margin = new System.Windows.Forms.Padding(0, 0, 0, 30);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(316, 65);
            this.lblHeader.TabIndex = 40;
            this.lblHeader.Text = "Journal search";
            // 
            // frmJournalSearch
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(1018, 743);
            this.Controls.Add(this.basePanel);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmJournalSearch";
            this.Load += new System.EventHandler(this.frmJournalSearch_Load);
            this.Controls.SetChildIndex(this.basePanel, 0);
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            this.basePanel.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBoxTransactionId.ResumeLayout(false);
            this.groupBoxTransactionId.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtTransactionId.Properties)).EndInit();
            this.groupBoxDate.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel basePanel;
        private DevExpress.XtraEditors.SimpleButton btnClose;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.GroupBox groupBoxTransactionId;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnTransactionId;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnClear;
        private DevExpress.XtraEditors.TextEdit txtTransactionId;
        private DevExpress.XtraEditors.LabelControl lblTransactionIdHeading;
        private System.Windows.Forms.GroupBox groupBoxDate;
        private DevExpress.XtraEditors.SimpleButton btnDate;
    }
}
