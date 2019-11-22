namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.Report
{
    partial class frmOGReportrpt
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
            this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
            this.dsOGReport = new Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.dsOGReport();
            this.OGReportBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.OGReportTableAdapter = new Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.dsOGReportTableAdapters.OGReportTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.dsOGReport)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.OGReportBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // reportViewer1
            // 
            this.reportViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            reportDataSource1.Name = "OGReport";
            reportDataSource1.Value = this.OGReportBindingSource;
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer1.LocalReport.ReportEmbeddedResource = "Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.rptOGReport.rdlc";
            this.reportViewer1.Location = new System.Drawing.Point(0, 0);
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.Size = new System.Drawing.Size(494, 333);
            this.reportViewer1.TabIndex = 0;
            // 
            // dsOGReport
            // 
            this.dsOGReport.DataSetName = "dsOGReport";
            this.dsOGReport.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // OGReportBindingSource
            // 
            this.OGReportBindingSource.DataMember = "OGReport";
            this.OGReportBindingSource.DataSource = this.dsOGReport;
            // 
            // OGReportTableAdapter
            // 
            this.OGReportTableAdapter.ClearBeforeFill = true;
            // 
            // frmOGReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(494, 333);
            this.Controls.Add(this.reportViewer1);
            this.Name = "frmOGReport";
            this.Text = "frmOGReport";
            this.Load += new System.EventHandler(this.frmOGReport_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dsOGReport)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.OGReportBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Reporting.WinForms.ReportViewer reportViewer1;
        private System.Windows.Forms.BindingSource OGReportBindingSource;
        private dsOGReport dsOGReport;
        private dsOGReportTableAdapters.OGReportTableAdapter OGReportTableAdapter;
    }
}