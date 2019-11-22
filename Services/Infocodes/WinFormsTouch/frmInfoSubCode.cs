/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
using System;
using System.Data;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using LSRetailPosis.DataAccess;
using LSRetailPosis.POSProcesses.WinControls;
using LSRetailPosis.Settings;

namespace Microsoft.Dynamics.Retail.Pos.Services.InfoCodes
{
    /// <summary>
    /// Summary description for frmInfoSubCode.
    /// </summary>
    internal class FormInfoSubCode : FormInfoCodeSubCodeBase
    {
        private SimpleButtonEx[] button;
        private InfoCodeData infoCodeData;
        private PanelControl panelControl1;
        private Label lblPrompt;
        private TableLayoutPanel tableLayoutPanel1;
        private SimpleButtonEx cancelButton;
        private TableLayoutPanel tableLayoutPanel2;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public FormInfoSubCode()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
        }

        protected override void OnHelpRequested(HelpEventArgs hevent)
        {
            if (hevent == null)
                throw new ArgumentNullException("hevent");

            LSRetailPosis.POSControls.POSFormsManager.ShowHelp(System.Windows.Forms.Form.ActiveForm);
            hevent.Handled = true;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
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
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.lblPrompt = new System.Windows.Forms.Label();
            this.cancelButton = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.AutoSize = true;
            this.panelControl1.Controls.Add(this.tableLayoutPanel1);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(717, 501);
            this.panelControl1.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblPrompt, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.cancelButton, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(2, 2);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(30, 40, 30, 11);
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(713, 497);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanel2, 2);
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Location = new System.Drawing.Point(356, 274);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(0, 0);
            this.tableLayoutPanel2.TabIndex = 2;
            // 
            // lblPrompt
            // 
            this.lblPrompt.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblPrompt.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.lblPrompt, 2);
            this.lblPrompt.Font = new System.Drawing.Font("Segoe UI Light", 36F);
            this.lblPrompt.Location = new System.Drawing.Point(250, 40);
            this.lblPrompt.Margin = new System.Windows.Forms.Padding(0, 0, 0, 30);
            this.lblPrompt.Name = "lblPrompt";
            this.lblPrompt.Size = new System.Drawing.Size(213, 65);
            this.lblPrompt.TabIndex = 0;
            this.lblPrompt.Text = "InfoCode";
            this.lblPrompt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.cancelButton.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelButton.Appearance.Options.UseFont = true;
            this.tableLayoutPanel1.SetColumnSpan(this.cancelButton, 2);
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.No;
            this.cancelButton.Location = new System.Drawing.Point(293, 425);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(4, 11, 4, 4);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(127, 57);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.Visible = false;
            // 
            // FormInfoSubCode
            // 
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(717, 501);
            this.Controls.Add(this.panelControl1);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "FormInfoSubCode";
            this.Controls.SetChildIndex(this.panelControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private void newButton(int buttonId, string descr)
        {
            button[buttonId] = new SimpleButtonEx();
            button[buttonId].Anchor = AnchorStyles.Left | AnchorStyles.Right;
            button[buttonId].Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            button[buttonId].Name = descr;
            button[buttonId].Size = new System.Drawing.Size(120, 60);
            button[buttonId].TabIndex = buttonId;
            button[buttonId].DialogResult = DialogResult.OK;
            button[buttonId].Text = descr;

            //Click event
            button[buttonId].Click += new EventHandler(ClickHandler);

            // We use a two column TableLayoutPanel for layout. Calculate which row and column to insert button into.
            //
            //      Header
            //  B0          B1
            //  B2          B3
            //       ...
            //      Cancel
            //
            // Row is offset by 1 to account for the header label row zero.
            int row = 1 + (buttonId / this.tableLayoutPanel1.ColumnCount);
            int column = buttonId % this.tableLayoutPanel1.ColumnCount;

            // Insert new row when the column is zero
            if (column == 0)
            {
                this.tableLayoutPanel2.RowStyles.Insert(this.tableLayoutPanel2.RowCount - 1, new System.Windows.Forms.RowStyle());
                this.tableLayoutPanel2.RowCount++;
            }

            // Add button to cell in layout panel
            this.tableLayoutPanel2.Controls.Add(button[buttonId], column, row);
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!this.DesignMode)
            {
                infoCodeData = new InfoCodeData(
                    ApplicationSettings.Database.LocalConnection,
                    ApplicationSettings.Database.DATAAREAID,
                    InfoCodes.InternalApplication);

                lblPrompt.Text = infoCodePrompt;
                DataTable infoSubCodes = infoCodeData.GetSubInfoCode(infoCodeId);
                SubCodes = new System.Collections.Generic.List<LSRetailPosis.POSProcesses.SubcodeInfo>(infoSubCodes.Rows.Count);
                button = new SimpleButtonEx[infoSubCodes.Rows.Count];
                int buttonId = 0;

                this.tableLayoutPanel2.Controls.Clear();

                foreach (DataRow dr in infoSubCodes.Rows)
                {
                    LSRetailPosis.POSProcesses.SubcodeInfo subcode = new LSRetailPosis.POSProcesses.SubcodeInfo();
                    subcode.SubCodeId = (string)dr["SUBCODEID"];
                    subcode.Description = (string)dr["DESCRIPTION"];
                    subcode.TriggerFunction = (LSRetailPosis.POSProcesses.TriggerFunctionEnum)(int)dr["TRIGGERFUNCTION"];
                    subcode.TriggerCode = (string)dr["TRIGGERCODE"];
                    subcode.PriceType = (LSRetailPosis.POSProcesses.PriceTypeEnum)dr["PRICETYPE"];
                    subcode.AmountPercent = (decimal)dr["AMOUNTPERCENT"];

                    SubCodes.Add(subcode);

                    newButton(buttonId++, subcode.Description);
                }

                // Show and position cancel button
                if (!inputRequired)
                {
                    this.cancelButton.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(3605);
                    this.tableLayoutPanel1.SetCellPosition(this.cancelButton, new TableLayoutPanelCellPosition(0, this.tableLayoutPanel1.RowCount - 1));
                    this.cancelButton.Visible = true;
                }
            }

            base.OnLoad(e);
        }

        private void ClickHandler(object sender, EventArgs e)
        {
            SimpleButton tmpButton = (SimpleButton)sender;
            selectedDescription = tmpButton.Text;

            LSRetailPosis.POSProcesses.SubcodeInfo code = SubCodes[tmpButton.TabIndex];
            selectedSubCodeId = code.SubCodeId;
            triggerCode = code.TriggerCode;
            triggerFunction = (int)code.TriggerFunction;
            priceType = (int)code.PriceType;
            amountPercent = code.AmountPercent;

            Close();
        }
    }
}