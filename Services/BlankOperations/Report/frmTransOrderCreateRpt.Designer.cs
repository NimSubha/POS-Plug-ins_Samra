namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.Report
{
    partial class frmTransOrderCreateRpt
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
            this.components = new System.ComponentModel.Container();
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource2 = new Microsoft.Reporting.WinForms.ReportDataSource();
            this.dtTransOrderCreateBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dsTransOrderCreate = new Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.dsTransOrderCreate();
            this.dtTransOrderCreateDtlBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
            this.dtTransOrderCreateTableAdapter = new Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.dsTransOrderCreateTableAdapters.dtTransOrderCreateTableAdapter();
            this.dtTransOrderCreateDtlTableAdapter = new Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.dsTransOrderCreateTableAdapters.dtTransOrderCreateDtlTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.dtTransOrderCreateBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsTransOrderCreate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTransOrderCreateDtlBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // dtTransOrderCreateBindingSource
            // 
            this.dtTransOrderCreateBindingSource.DataMember = "dtTransOrderCreate";
            this.dtTransOrderCreateBindingSource.DataSource = this.dsTransOrderCreate;
            // 
            // dsTransOrderCreate
            // 
            this.dsTransOrderCreate.DataSetName = "dsTransOrderCreate";
            this.dsTransOrderCreate.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // dtTransOrderCreateDtlBindingSource
            // 
            this.dtTransOrderCreateDtlBindingSource.DataMember = "dtTransOrderCreateDtl";
            this.dtTransOrderCreateDtlBindingSource.DataSource = this.dsTransOrderCreate;
            // 
            // reportViewer1
            // 
            this.reportViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            reportDataSource1.Name = "dsTransOrder";
            reportDataSource1.Value = this.dtTransOrderCreateBindingSource;
            reportDataSource2.Name = "dsTransOrderDtl";
            reportDataSource2.Value = this.dtTransOrderCreateDtlBindingSource;
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource2);
            this.reportViewer1.LocalReport.ReportEmbeddedResource = "Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.rptTransOrderCreate.rdlc";
            this.reportViewer1.Location = new System.Drawing.Point(0, 0);
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.Size = new System.Drawing.Size(720, 506);
            this.reportViewer1.TabIndex = 0;
            // 
            // dtTransOrderCreateTableAdapter
            // 
            this.dtTransOrderCreateTableAdapter.ClearBeforeFill = true;
            // 
            // dtTransOrderCreateDtlTableAdapter
            // 
            this.dtTransOrderCreateDtlTableAdapter.ClearBeforeFill = true;
            // 
            // frmTransOrderCreateRpt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(720, 506);
            this.Controls.Add(this.reportViewer1);
            this.Name = "frmTransOrderCreateRpt";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.frmTransOrderCreateRpt_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dtTransOrderCreateBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsTransOrderCreate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTransOrderCreateDtlBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Reporting.WinForms.ReportViewer reportViewer1;
        private System.Windows.Forms.BindingSource dtTransOrderCreateBindingSource;
        private dsTransOrderCreate dsTransOrderCreate;
        private System.Windows.Forms.BindingSource dtTransOrderCreateDtlBindingSource;
        private dsTransOrderCreateTableAdapters.dtTransOrderCreateTableAdapter dtTransOrderCreateTableAdapter;
        private dsTransOrderCreateTableAdapters.dtTransOrderCreateDtlTableAdapter dtTransOrderCreateDtlTableAdapter;
    }
}