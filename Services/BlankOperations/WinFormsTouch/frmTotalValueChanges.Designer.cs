namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    partial class frmTotalValueChanges
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
            this.btnCommit = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnCancel = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.label5 = new System.Windows.Forms.Label();
            this.txtPer = new System.Windows.Forms.TextBox();
            this.lblCode = new System.Windows.Forms.Label();
            this.simpleButtonEx1 = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnSubmit = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.txtTotValue = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCommit
            // 
            this.btnCommit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCommit.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCommit.Appearance.Options.UseFont = true;
            this.btnCommit.Location = new System.Drawing.Point(381, 208);
            this.btnCommit.Name = "btnCommit";
            this.btnCommit.Size = new System.Drawing.Size(108, 42);
            this.btnCommit.TabIndex = 203;
            this.btnCommit.Text = "Commit";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.Location = new System.Drawing.Point(267, 208);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(108, 42);
            this.btnCancel.TabIndex = 204;
            this.btnCancel.Text = "Cancel";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI Light", 9F, System.Drawing.FontStyle.Bold);
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(-166, -127);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(76, 15);
            this.label5.TabIndex = 205;
            this.label5.Text = "Amount (%)";
            // 
            // txtPer
            // 
            this.txtPer.Location = new System.Drawing.Point(-66, -125);
            this.txtPer.MaxLength = 20;
            this.txtPer.Name = "txtPer";
            this.txtPer.Size = new System.Drawing.Size(146, 21);
            this.txtPer.TabIndex = 202;
            this.txtPer.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblCode
            // 
            this.lblCode.AutoSize = true;
            this.lblCode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblCode.Location = new System.Drawing.Point(10, 38);
            this.lblCode.Name = "lblCode";
            this.lblCode.Size = new System.Drawing.Size(79, 13);
            this.lblCode.TabIndex = 207;
            this.lblCode.Text = "Changed Value";
            // 
            // simpleButtonEx1
            // 
            this.simpleButtonEx1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.simpleButtonEx1.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.simpleButtonEx1.Appearance.Options.UseFont = true;
            this.simpleButtonEx1.Location = new System.Drawing.Point(195, 63);
            this.simpleButtonEx1.Name = "simpleButtonEx1";
            this.simpleButtonEx1.Size = new System.Drawing.Size(70, 36);
            this.simpleButtonEx1.TabIndex = 1;
            this.simpleButtonEx1.Text = "Cancel";
            this.simpleButtonEx1.Click += new System.EventHandler(this.simpleButtonEx1_Click);
            // 
            // btnSubmit
            // 
            this.btnSubmit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSubmit.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSubmit.Appearance.Options.UseFont = true;
            this.btnSubmit.Location = new System.Drawing.Point(115, 63);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(74, 36);
            this.btnSubmit.TabIndex = 2;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // txtValue
            // 
            this.txtValue.Location = new System.Drawing.Point(94, 35);
            this.txtValue.MaxLength = 9;
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(169, 21);
            this.txtValue.TabIndex = 0;
            this.txtValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtValue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtValue_KeyPress);
            // 
            // txtTotValue
            // 
            this.txtTotValue.BackColor = System.Drawing.SystemColors.Control;
            this.txtTotValue.Enabled = false;
            this.txtTotValue.Location = new System.Drawing.Point(94, 10);
            this.txtTotValue.MaxLength = 9;
            this.txtTotValue.Name = "txtTotValue";
            this.txtTotValue.Size = new System.Drawing.Size(169, 21);
            this.txtTotValue.TabIndex = 212;
            this.txtTotValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label1.Location = new System.Drawing.Point(10, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 211;
            this.label1.Text = "Total Value";
            // 
            // frmTotalValueChanges
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(275, 111);
            this.ControlBox = false;
            this.Controls.Add(this.txtTotValue);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtValue);
            this.Controls.Add(this.simpleButtonEx1);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.lblCode);
            this.Controls.Add(this.btnCommit);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtPer);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmTotalValueChanges";
            this.Text = "Total Value Changes";
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCommit;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCancel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtPer;
        private System.Windows.Forms.Label lblCode;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx simpleButtonEx1;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnSubmit;
        private System.Windows.Forms.TextBox txtValue;
        private System.Windows.Forms.TextBox txtTotValue;
        private System.Windows.Forms.Label label1;
    }
}