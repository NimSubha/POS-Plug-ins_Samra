using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using System.Data.SqlClient;
using LSRetailPosis.Settings;
using Microsoft.Dynamics.Retail.Pos.ApplicationService;
using Microsoft.Dynamics.Retail.Pos.Customer;
using Microsoft.Dynamics.Retail.Pos.Dialog;
using Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch;
using Microsoft.Reporting.WinForms;
using Microsoft.Dynamics.Retail.Pos.BlankOperations;
using System.IO;

namespace BlankOperations
{
    public class frmCustomerOrder : frmTouchBase
    {
        private System.ComponentModel.IContainer components = null;

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
            this.pnlMain = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnClose = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnClear = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnStoneAdv = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnSearchOrder = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.btnCustomerSearch = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnSave = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnOrderDetails = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnAddCustomer = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.txtTotalAmount = new System.Windows.Forms.TextBox();
            this.txtPhoneNumber = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtCustomerAddress = new System.Windows.Forms.TextBox();
            this.lblCustomerAddress = new System.Windows.Forms.Label();
            this.dtPickerDeliveryDate = new System.Windows.Forms.DateTimePicker();
            this.lblDeliveryDate = new System.Windows.Forms.Label();
            this.txtOrderNo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtCustomerName = new System.Windows.Forms.TextBox();
            this.txtCustomerAccount = new System.Windows.Forms.TextBox();
            this.lblCustomerAccount = new System.Windows.Forms.Label();
            this.dTPickerOrderDate = new System.Windows.Forms.DateTimePicker();
            this.lblOrderDate = new System.Windows.Forms.Label();
            this.lblOrderNo = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            this.pnlMain.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlMain.Controls.Add(this.tableLayoutPanel1);
            this.pnlMain.Controls.Add(this.tableLayoutPanel);
            this.pnlMain.Controls.Add(this.panel2);
            this.pnlMain.Controls.Add(this.lblTitle);
            this.pnlMain.Location = new System.Drawing.Point(6, 6);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(710, 356);
            this.pnlMain.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.btnClose, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnClear, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnStoneAdv, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnSearchOrder, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(357, 194);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(345, 155);
            this.tableLayoutPanel1.TabIndex = 35;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnClose.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Appearance.Options.UseFont = true;
            this.btnClose.Location = new System.Drawing.Point(176, 81);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(164, 70);
            this.btnClose.TabIndex = 33;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnClear.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.Appearance.Options.UseFont = true;
            this.btnClear.Location = new System.Drawing.Point(4, 4);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(164, 70);
            this.btnClear.TabIndex = 34;
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnStoneAdv
            // 
            this.btnStoneAdv.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnStoneAdv.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStoneAdv.Appearance.Options.UseFont = true;
            this.btnStoneAdv.Location = new System.Drawing.Point(4, 81);
            this.btnStoneAdv.Name = "btnStoneAdv";
            this.btnStoneAdv.Size = new System.Drawing.Size(164, 70);
            this.btnStoneAdv.TabIndex = 32;
            this.btnStoneAdv.Text = "Void";
            this.btnStoneAdv.Click += new System.EventHandler(this.btnStoneAdvance_Click);
            // 
            // btnSearchOrder
            // 
            this.btnSearchOrder.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSearchOrder.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSearchOrder.Appearance.Options.UseFont = true;
            this.btnSearchOrder.Location = new System.Drawing.Point(176, 4);
            this.btnSearchOrder.Name = "btnSearchOrder";
            this.btnSearchOrder.Size = new System.Drawing.Size(164, 70);
            this.btnSearchOrder.TabIndex = 35;
            this.btnSearchOrder.Text = "Search Order";
            this.btnSearchOrder.Click += new System.EventHandler(this.btnSearchOrder_Click);
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.AutoSize = true;
            this.tableLayoutPanel.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.Controls.Add(this.btnCustomerSearch, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.btnSave, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.btnOrderDetails, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.btnAddCustomer, 1, 0);
            this.tableLayoutPanel.Location = new System.Drawing.Point(5, 194);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(345, 155);
            this.tableLayoutPanel.TabIndex = 4;
            // 
            // btnCustomerSearch
            // 
            this.btnCustomerSearch.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCustomerSearch.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCustomerSearch.Appearance.Options.UseFont = true;
            this.btnCustomerSearch.Location = new System.Drawing.Point(4, 4);
            this.btnCustomerSearch.Name = "btnCustomerSearch";
            this.btnCustomerSearch.Size = new System.Drawing.Size(164, 70);
            this.btnCustomerSearch.TabIndex = 28;
            this.btnCustomerSearch.Text = "Customer Search";
            this.btnCustomerSearch.Click += new System.EventHandler(this.btnCustomerSearch_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSave.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Appearance.Options.UseFont = true;
            this.btnSave.Location = new System.Drawing.Point(176, 81);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(164, 70);
            this.btnSave.TabIndex = 34;
            this.btnSave.Text = "Save and Close";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnOrderDetails
            // 
            this.btnOrderDetails.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnOrderDetails.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOrderDetails.Appearance.Options.UseFont = true;
            this.btnOrderDetails.Location = new System.Drawing.Point(4, 81);
            this.btnOrderDetails.Name = "btnOrderDetails";
            this.btnOrderDetails.Size = new System.Drawing.Size(164, 70);
            this.btnOrderDetails.TabIndex = 30;
            this.btnOrderDetails.Text = "Order Details";
            this.btnOrderDetails.Click += new System.EventHandler(this.btnOrderDetails_Click);
            // 
            // btnAddCustomer
            // 
            this.btnAddCustomer.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnAddCustomer.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddCustomer.Appearance.Options.UseFont = true;
            this.btnAddCustomer.Location = new System.Drawing.Point(175, 4);
            this.btnAddCustomer.Name = "btnAddCustomer";
            this.btnAddCustomer.Size = new System.Drawing.Size(165, 70);
            this.btnAddCustomer.TabIndex = 29;
            this.btnAddCustomer.Text = "Add Customer";
            this.btnAddCustomer.Click += new System.EventHandler(this.btnAddCustomer_Click);
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.txtTotalAmount);
            this.panel2.Controls.Add(this.txtPhoneNumber);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.txtCustomerAddress);
            this.panel2.Controls.Add(this.lblCustomerAddress);
            this.panel2.Controls.Add(this.dtPickerDeliveryDate);
            this.panel2.Controls.Add(this.lblDeliveryDate);
            this.panel2.Controls.Add(this.txtOrderNo);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.txtCustomerName);
            this.panel2.Controls.Add(this.txtCustomerAccount);
            this.panel2.Controls.Add(this.lblCustomerAccount);
            this.panel2.Controls.Add(this.dTPickerOrderDate);
            this.panel2.Controls.Add(this.lblOrderDate);
            this.panel2.Controls.Add(this.lblOrderNo);
            this.panel2.Location = new System.Drawing.Point(5, 42);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(697, 146);
            this.panel2.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label2.Location = new System.Drawing.Point(370, 120);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 13);
            this.label2.TabIndex = 18;
            this.label2.Text = "Total Amount:";
            // 
            // txtTotalAmount
            // 
            this.txtTotalAmount.BackColor = System.Drawing.SystemColors.Control;
            this.txtTotalAmount.Enabled = false;
            this.txtTotalAmount.Location = new System.Drawing.Point(477, 117);
            this.txtTotalAmount.MaxLength = 60;
            this.txtTotalAmount.Name = "txtTotalAmount";
            this.txtTotalAmount.ReadOnly = true;
            this.txtTotalAmount.Size = new System.Drawing.Size(211, 21);
            this.txtTotalAmount.TabIndex = 19;
            // 
            // txtPhoneNumber
            // 
            this.txtPhoneNumber.BackColor = System.Drawing.SystemColors.Control;
            this.txtPhoneNumber.Enabled = false;
            this.txtPhoneNumber.Location = new System.Drawing.Point(113, 115);
            this.txtPhoneNumber.MaxLength = 20;
            this.txtPhoneNumber.Name = "txtPhoneNumber";
            this.txtPhoneNumber.ReadOnly = true;
            this.txtPhoneNumber.Size = new System.Drawing.Size(211, 21);
            this.txtPhoneNumber.TabIndex = 17;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label3.Location = new System.Drawing.Point(6, 120);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "Phone Number:";
            // 
            // txtCustomerAddress
            // 
            this.txtCustomerAddress.BackColor = System.Drawing.SystemColors.Control;
            this.txtCustomerAddress.Enabled = false;
            this.txtCustomerAddress.Location = new System.Drawing.Point(113, 88);
            this.txtCustomerAddress.MaxLength = 20;
            this.txtCustomerAddress.Name = "txtCustomerAddress";
            this.txtCustomerAddress.ReadOnly = true;
            this.txtCustomerAddress.Size = new System.Drawing.Size(575, 21);
            this.txtCustomerAddress.TabIndex = 15;
            // 
            // lblCustomerAddress
            // 
            this.lblCustomerAddress.AutoSize = true;
            this.lblCustomerAddress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblCustomerAddress.Location = new System.Drawing.Point(6, 91);
            this.lblCustomerAddress.Name = "lblCustomerAddress";
            this.lblCustomerAddress.Size = new System.Drawing.Size(99, 13);
            this.lblCustomerAddress.TabIndex = 14;
            this.lblCustomerAddress.Text = "Customer Address:";
            // 
            // dtPickerDeliveryDate
            // 
            this.dtPickerDeliveryDate.Location = new System.Drawing.Point(477, 35);
            this.dtPickerDeliveryDate.Name = "dtPickerDeliveryDate";
            this.dtPickerDeliveryDate.Size = new System.Drawing.Size(211, 21);
            this.dtPickerDeliveryDate.TabIndex = 13;
            this.dtPickerDeliveryDate.ValueChanged += new System.EventHandler(this.dtPickerDeliveryDate_ValueChanged);
            // 
            // lblDeliveryDate
            // 
            this.lblDeliveryDate.AutoSize = true;
            this.lblDeliveryDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblDeliveryDate.Location = new System.Drawing.Point(370, 37);
            this.lblDeliveryDate.Name = "lblDeliveryDate";
            this.lblDeliveryDate.Size = new System.Drawing.Size(76, 13);
            this.lblDeliveryDate.TabIndex = 12;
            this.lblDeliveryDate.Text = "Delivery Date:";
            // 
            // txtOrderNo
            // 
            this.txtOrderNo.BackColor = System.Drawing.SystemColors.Control;
            this.txtOrderNo.Enabled = false;
            this.txtOrderNo.Location = new System.Drawing.Point(113, 6);
            this.txtOrderNo.MaxLength = 20;
            this.txtOrderNo.Name = "txtOrderNo";
            this.txtOrderNo.ReadOnly = true;
            this.txtOrderNo.Size = new System.Drawing.Size(211, 21);
            this.txtOrderNo.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label1.Location = new System.Drawing.Point(370, 63);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Customer Name:";
            // 
            // txtCustomerName
            // 
            this.txtCustomerName.BackColor = System.Drawing.SystemColors.Control;
            this.txtCustomerName.Enabled = false;
            this.txtCustomerName.Location = new System.Drawing.Point(477, 60);
            this.txtCustomerName.MaxLength = 60;
            this.txtCustomerName.Name = "txtCustomerName";
            this.txtCustomerName.ReadOnly = true;
            this.txtCustomerName.Size = new System.Drawing.Size(211, 21);
            this.txtCustomerName.TabIndex = 11;
            // 
            // txtCustomerAccount
            // 
            this.txtCustomerAccount.BackColor = System.Drawing.SystemColors.Control;
            this.txtCustomerAccount.Enabled = false;
            this.txtCustomerAccount.Location = new System.Drawing.Point(113, 61);
            this.txtCustomerAccount.MaxLength = 20;
            this.txtCustomerAccount.Name = "txtCustomerAccount";
            this.txtCustomerAccount.ReadOnly = true;
            this.txtCustomerAccount.Size = new System.Drawing.Size(211, 21);
            this.txtCustomerAccount.TabIndex = 9;
            // 
            // lblCustomerAccount
            // 
            this.lblCustomerAccount.AutoSize = true;
            this.lblCustomerAccount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblCustomerAccount.Location = new System.Drawing.Point(6, 63);
            this.lblCustomerAccount.Name = "lblCustomerAccount";
            this.lblCustomerAccount.Size = new System.Drawing.Size(99, 13);
            this.lblCustomerAccount.TabIndex = 8;
            this.lblCustomerAccount.Text = "Customer Account:";
            // 
            // dTPickerOrderDate
            // 
            this.dTPickerOrderDate.Location = new System.Drawing.Point(113, 35);
            this.dTPickerOrderDate.Name = "dTPickerOrderDate";
            this.dTPickerOrderDate.Size = new System.Drawing.Size(211, 21);
            this.dTPickerOrderDate.TabIndex = 5;
            // 
            // lblOrderDate
            // 
            this.lblOrderDate.AutoSize = true;
            this.lblOrderDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblOrderDate.Location = new System.Drawing.Point(6, 36);
            this.lblOrderDate.Name = "lblOrderDate";
            this.lblOrderDate.Size = new System.Drawing.Size(65, 13);
            this.lblOrderDate.TabIndex = 4;
            this.lblOrderDate.Text = "Order Date:";
            // 
            // lblOrderNo
            // 
            this.lblOrderNo.AutoSize = true;
            this.lblOrderNo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblOrderNo.Location = new System.Drawing.Point(6, 9);
            this.lblOrderNo.Name = "lblOrderNo";
            this.lblOrderNo.Size = new System.Drawing.Size(59, 13);
            this.lblOrderNo.TabIndex = 2;
            this.lblOrderNo.Text = "Order No.:";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblTitle.Location = new System.Drawing.Point(240, 4);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(220, 32);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "CUSTOMER ORDER";
            // 
            // frmCustomerOrder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(720, 368);
            this.ControlBox = false;
            this.Controls.Add(this.pnlMain);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmCustomerOrder";
            this.ShowIcon = false;
            this.Text = "Customer Order";
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DateTimePicker dtPickerDeliveryDate;
        private System.Windows.Forms.Label lblDeliveryDate;
        public TextBox txtOrderNo;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox txtCustomerName;
        public System.Windows.Forms.TextBox txtCustomerAccount;
        private System.Windows.Forms.Label lblCustomerAccount;
        private System.Windows.Forms.DateTimePicker dTPickerOrderDate;
        private System.Windows.Forms.Label lblOrderDate;
        private System.Windows.Forms.Label lblOrderNo;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnSearchOrder;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnStoneAdv;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnAddCustomer;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCustomerSearch;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnOrderDetails;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnSave;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnClose;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnClear;
        public System.Windows.Forms.TextBox txtCustomerAddress;
        private System.Windows.Forms.Label lblCustomerAddress;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox txtTotalAmount;
        public System.Windows.Forms.TextBox txtPhoneNumber;
        private System.Windows.Forms.Label label3;


        #region Variable Declaration
        public string CustAddress { get; set; }
        public string CustPhoneNo { get; set; }
        public IPosTransaction pos { get; set; }
        public DataTable dtOrderDetails = new DataTable("dtOrderDetails");
        public DataTable dtSampleDetails = new DataTable("dtSampleDetails");
        public DataTable dtSubOrderDetails = new DataTable("dtSubOrderDetails");
        public DataTable dtOrderStnAdv = new DataTable("dtOrderStnAdv");


        public DataTable dtSketchDetails = new DataTable("dtSketchDetails");
        public DataTable dtSampleSketch = new DataTable("dtSampleSketch");

        public string sOrderDetailsAmount = string.Empty;
        public string sSubOrderDetailsAmount = string.Empty;
        public string sCustOrderSearchNumber = string.Empty;
        DataSet dsOrderSearched = new DataSet();

        public bool bDataSaved = false;
        public string sCustAcc = string.Empty;
        public string sCustOrder = string.Empty;
        public string sTotalAmt = string.Empty;

        public decimal dFixedMetalRateVal = 0m; // Fixed Metal Rate New
        public string sFixedMetalRateConfigID = string.Empty; // Fixed Metal Rate New

        public DataTable dtRecvStoneDetails = new DataTable("dtRecvStoneDetails"); //Add by Palas Jana
        public DataTable dtPaySchedule = new DataTable("dtPaySchedule");
        int isBookedItem = 0;
        public int iIsCustOrderWithAdv = 0;

        [Import]
        private IApplication application;

        #endregion

        #region Initialization
        public frmCustomerOrder(IPosTransaction posTransaction, IApplication Application)
        {
            InitializeComponent();
            this.ClientSize = new System.Drawing.Size(720, 368);
            pos = posTransaction;
            application = Application;
            txtOrderNo.Text = GetOrderNum();
            dFixedMetalRateVal = GetFixedMetalRate(ref sFixedMetalRateConfigID); // Fixed Metal Rate New
            btnCustomerSearch.Focus();

        }
        #endregion

        #region GetOrderNum()
        public string GetOrderNum()
        {

            string OrderNum = string.Empty;
            OrderNum = GetNextCustomerOrderID();
            return OrderNum;
        }
        #endregion

        #region - CHANGED BY NIMBUS TO GET THE ORDER ID

        enum ReceiptTransactionType
        {
            CustomerGoldOrder = 8
        }

        public string GetNextCustomerOrderID()
        {
            try
            {
                ReceiptTransactionType transType = ReceiptTransactionType.CustomerGoldOrder;
                string storeId = LSRetailPosis.Settings.ApplicationSettings.Terminal.StoreId;
                string terminalId = LSRetailPosis.Settings.ApplicationSettings.Terminal.TerminalId;
                string staffId = pos.OperatorId;
                string mask;

                string funcProfileId = LSRetailPosis.Settings.FunctionalityProfiles.Functions.ProfileId;
                orderNumber((int)transType, funcProfileId, out mask);
                if (string.IsNullOrEmpty(mask))
                    return string.Empty;
                else
                {
                    string seedValue = GetSeedVal().ToString();
                    return ReceiptMaskFiller.FillMask(mask, seedValue, storeId, terminalId, staffId);
                }

            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
        }
        #endregion

        #region GetOrderNum()  - CHANGED BY NIMBUS
        private void orderNumber(int transType, string funcProfile, out string mask)
        {
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            string Val = string.Empty;
            try
            {
                string queryString = " SELECT MASK FROM RETAILRECEIPTMASKS WHERE FUNCPROFILEID='" + funcProfile.Trim() + "' " +
                                     " AND RECEIPTTRANSTYPE=" + transType;
                using (SqlCommand command = new SqlCommand(queryString, conn))
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    Val = Convert.ToString(command.ExecuteScalar());
                    mask = Val;

                }
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }



        }

        #endregion

        #region GetSeedVal() - CHANGED BY NIMBUS
        private string GetSeedVal()
        {
            string sFuncProfileId = LSRetailPosis.Settings.FunctionalityProfiles.Functions.ProfileId;
            int iTransType = (int)ReceiptTransactionType.CustomerGoldOrder;

            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            string Val = string.Empty;
            try
            {
                // string queryString = " SELECT  MAX(CAST(ISNULL(SUBSTRING(CUSTORDER_HEADER.ORDERNUM,3,LEN(CUSTORDER_HEADER.ORDERNUM)),0) AS INTEGER)) + 1 from CUSTORDER_HEADER ";

                string queryString = "DECLARE @VAL AS INT  SELECT @VAL = CHARINDEX('#',mask) FROM RETAILRECEIPTMASKS WHERE FUNCPROFILEID ='" + sFuncProfileId + "'  AND RECEIPTTRANSTYPE = " + iTransType + " " +
                                     " SELECT  MAX(CAST(ISNULL(SUBSTRING(ORDERNUM,@VAL,LEN(ORDERNUM)),0) AS INTEGER)) + 1 from CUSTORDER_HEADER";
                using (SqlCommand command = new SqlCommand(queryString, conn))
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    Val = Convert.ToString(command.ExecuteScalar());
                    if (string.IsNullOrEmpty(Val))
                    {
                        Val = "1";
                    }

                    return Val;

                }
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }



        }

        #endregion

        private string Amtinwds(double amt)
        {
            MultiCurrency objMulC = null;
            if (Convert.ToString(ApplicationSettings.Terminal.StoreCurrency) != "INR")
                objMulC = new MultiCurrency(Criteria.Foreign);
            else
                objMulC = new MultiCurrency(Criteria.Indian);
            Color cBlack = Color.FromName("Black");

            return GetCurrencyNameWithCode() + " " + objMulC.ConvertToWord(Convert.ToString(amt));
        }
        private string GetCurrencyNameWithCode()
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("SELECT TXT FROM CURRENCY WHERE CURRENCYCODE='" + ApplicationSettings.Terminal.StoreCurrency + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            if (!string.IsNullOrEmpty(sResult))
            {
                return sResult.Trim();
            }
            else
            {
                return "-";
            }
        }

        #region Customer Button Click
        private void btnCustomerSearch_Click(object sender, EventArgs e)
        {
            Microsoft.Dynamics.Retail.Pos.Customer.WinFormsTouch.frmCustomerSearch obfrm = new Microsoft.Dynamics.Retail.Pos.Customer.WinFormsTouch.frmCustomerSearch(this);
            obfrm.ShowDialog();
        }
        #endregion

        #region Delivery Date Changed
        private void dtPickerDeliveryDate_ValueChanged(object sender, EventArgs e)
        {
            if (Convert.ToDateTime(dtPickerDeliveryDate.Text) < Convert.ToDateTime(dTPickerOrderDate.Text))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("DELIVERY DATE CANNOT BE LESS THAN ORDER DATE.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    dtPickerDeliveryDate.Text = dTPickerOrderDate.Text;

                }
            }
        }
        #endregion

        #region Click Order Details
        private void btnOrderDetails_Click(object sender, EventArgs e)
        {
            BlankOperations.WinFormsTouch.frmOrderDetails objOrderdetails = null;
            if (dtOrderDetails != null && dtOrderDetails.Rows.Count > 0)
            {
                objOrderdetails = new BlankOperations.WinFormsTouch.frmOrderDetails(dtSampleDetails, dtSubOrderDetails, dtOrderDetails, dtPaySchedule, pos, application, this);
            }

            else if (dsOrderSearched != null && dsOrderSearched.Tables.Count > 0 && dsOrderSearched.Tables[1].Rows.Count > 0)
            {
                objOrderdetails = new BlankOperations.WinFormsTouch.frmOrderDetails(dsOrderSearched, pos, application, this);
            }
            else
            {
                dtOrderDetails = new DataTable();
                objOrderdetails = new BlankOperations.WinFormsTouch.frmOrderDetails(pos, application, this);
            }


            objOrderdetails.ShowDialog();
            txtTotalAmount.Text = sOrderDetailsAmount;
        }
        #endregion

        #region ADD NEW CUSTOMER
        private void btnAddCustomer_Click(object sender, EventArgs e)
        {
            Customer obj = new Customer();
            string strCustId = string.Empty;
            string strCustName = string.Empty;
            string strCustCurrency = string.Empty;

            obj.AddNew(out strCustId, out  strCustName, out strCustCurrency);
            {
                txtCustomerAccount.Text = strCustId;
                txtCustomerName.Text = strCustName;

            }
        }
        #endregion

        #region Save Click
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtOrderNo.Text))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Order Number is not defined.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                }
                return;
            }
            if (isValid())
            {
                //if (dtOrderDetails != null && dtOrderDetails.Rows.Count > 0)
                //{
                //    for (int ItemCount = 0; ItemCount < dtOrderDetails.Rows.Count; ItemCount++)
                //    {
                //        isBookedItem = Convert.ToInt32(dtOrderDetails.Rows[ItemCount]["IsBookedSKU"]);

                //        if (isBookedItem == 1)
                //        {
                frmOrderAdvOrNot objAdv = new frmOrderAdvOrNot();
                objAdv.ShowDialog();
                iIsCustOrderWithAdv = objAdv.iWithAdv;
                //        }

                //    }
                //}
                SaveOrder();
            }
        }
        #endregion

        #region enum  RateType
        enum RateType
        {
            Weight = 0,
            Pcs = 1,
            Tot = 2,
        }

        #region enum  MakingType
        enum MakingType
        {
            Weight = 2,
            Pieces = 0,
            Tot = 3,
            Percentage = 4,
        }
        #endregion

        #region Wastage Type

        enum WastageType // Added for wastage 
        {
            //  None    = 0,
            Weight = 0,
            Percentage = 1,
        }

        #endregion
        #endregion

        #region SaveFuction
        private void SaveOrder()
        {
            int iCustOrder_Header = 0;
            int iCustOrder_Details = 0;
            int iCustOrder_SubDetails = 0;
            //int iCustOrder_SampleDetails = 0;
            //int iCustOrder_StoneAdv = 0;

            SqlTransaction transaction = null;

            #region CUSTOMER ORDER HEADER
            string commandText = " INSERT INTO [CUSTORDER_HEADER]([ORDERNUM],[STOREID],[TERMINALID],[ORDERDATE],[DELIVERYDATE]," +
                                 " [CUSTACCOUNT],[CUSTNAME],[CUSTADDRESS],[CUSTPHONE] " +
                                 " ,[DATAAREAID],[STAFFID],[TOTALAMOUNT],[FIXEDMETALRATE],[FIXEDMETALRATECONFIGID],[SAMPLEFLAG],[STONEFLAG],IsConfirmed,[WITHADVANCE])" +
                                 " VALUES(@ORDERNUM,@STOREID,@TERMINALID,@ORDERDATE,@DELIVERYDATE,@CUSTACCOUNT,@CUSTNAME," +
                                 " @CUSTADDRESS,@CUSTPHONE,@DATAAREAID,@STAFFID,@TOTALAMOUNT,@FIXEDMETALRATE,@FIXEDMETALRATECONFIGID,@SAMPLEFLAG,@STONEFLAG,@IsConfirmed,@WITHADVANCE)";
            SqlConnection connection = new SqlConnection();
            try
            {
                if (application != null)
                    connection = application.Settings.Database.Connection;
                else
                    connection = ApplicationSettings.Database.LocalConnection;


                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                transaction = connection.BeginTransaction();

                SqlCommand command = new SqlCommand(commandText, connection, transaction);
                command.Parameters.Clear();
                command.Parameters.Add("@ORDERNUM", SqlDbType.NVarChar).Value = txtOrderNo.Text.Trim();
                command.Parameters.Add("@STOREID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.StoreId;
                command.Parameters.Add("@TERMINALID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.TerminalId;
                command.Parameters.Add("@ORDERDATE", SqlDbType.DateTime).Value = dTPickerOrderDate.Value;
                command.Parameters.Add("@DELIVERYDATE", SqlDbType.DateTime).Value = dtPickerDeliveryDate.Value;
                command.Parameters.Add("@CUSTACCOUNT", SqlDbType.NVarChar, 20).Value = txtCustomerAccount.Text.Trim();
                command.Parameters.Add("@CUSTNAME", SqlDbType.NVarChar, 60).Value = txtCustomerName.Text.Trim();
                command.Parameters.Add("@CUSTADDRESS", SqlDbType.NVarChar, 250).Value = CustAddress == null ? string.Empty : CustAddress;
                if (string.IsNullOrEmpty(CustPhoneNo))
                    command.Parameters.Add("@CUSTPHONE", SqlDbType.Decimal).Value = DBNull.Value;
                else
                    command.Parameters.Add(new SqlParameter("@CUSTPHONE", CustPhoneNo));

                if (application != null)

                    command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = application.Settings.Database.DataAreaID;
                else
                    command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = ApplicationSettings.Database.DATAAREAID;
                command.Parameters.Add("@STAFFID", SqlDbType.NVarChar, 10).Value = pos.OperatorId;
                command.Parameters.Add("@TOTALAMOUNT", SqlDbType.Decimal, 250).Value = sOrderDetailsAmount;
                command.Parameters.Add("@FIXEDMETALRATE", SqlDbType.Decimal, 250).Value = dFixedMetalRateVal; // Fixed Metal Rate New
                command.Parameters.Add("@FIXEDMETALRATECONFIGID", SqlDbType.NVarChar, 10).Value = sFixedMetalRateConfigID;// Fixed Metal Rate New
                command.Parameters.Add("@SAMPLEFLAG", SqlDbType.Int).Value = (dtSampleDetails.Rows.Count > 0) ? 1 : 0;
                command.Parameters.Add("@STONEFLAG", SqlDbType.Int).Value = (dtOrderStnAdv.Rows.Count > 0) ? 1 : 0;
                command.Parameters.Add("@IsConfirmed", SqlDbType.Int).Value = 1;
                command.Parameters.Add("@WITHADVANCE", SqlDbType.Int).Value = iIsCustOrderWithAdv;


            #endregion

                command.CommandTimeout = 0;
                iCustOrder_Header = command.ExecuteNonQuery();

                if (iCustOrder_Header == 1)
                {
                    #region Stone advance save //commented RH
                    //if(dtOrderStnAdv != null && dtOrderStnAdv.Rows.Count > 0)
                    //{

                    //    string commandCustOrder_StoneAdv = " INSERT INTO [CustOrderStoneAdvance]([ORDERNUM],[LINENUM],[STOREID],[TERMINALID],[ITEMID],[CONFIGID] " +
                    //                 " ,[CODE],[SIZEID],[STYLE],[PCS],[QTY],[DATAAREAID],[INVENTDIMID],[UNITID],REMARKS)" +
                    //                 " VALUES(@ORDERNUM,@LINENUM,@STOREID,@TERMINALID,@ITEMID ,@CONFIGID,@CODE ,@SIZEID,@STYLE,@PCS ,@QTY " +
                    //                 " ,@DATAAREAID , @INVENTDIMID, @UNITID, @REMARKS)";

                    //    for(int StoneAdvCount = 0; StoneAdvCount < dtOrderStnAdv.Rows.Count; StoneAdvCount++)
                    //    {

                    //        SqlCommand commandStoneAdv = new SqlCommand(commandCustOrder_StoneAdv, connection, transaction);
                    //        commandStoneAdv.Parameters.Add("@ORDERNUM", SqlDbType.NVarChar, 20).Value = txtOrderNo.Text;
                    //        commandStoneAdv.Parameters.Add("@LINENUM", SqlDbType.NVarChar, 20).Value = StoneAdvCount + 1;
                    //        commandStoneAdv.Parameters.Add("@STOREID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.StoreId;
                    //        commandStoneAdv.Parameters.Add("@TERMINALID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.TerminalId;
                    //        commandStoneAdv.Parameters.Add("@ITEMID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtOrderStnAdv.Rows[StoneAdvCount]["ITEMID"]);
                    //        commandStoneAdv.Parameters.Add("@CONFIGID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtOrderStnAdv.Rows[StoneAdvCount]["CONFIGURATION"]);
                    //        commandStoneAdv.Parameters.Add("@CODE", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtOrderStnAdv.Rows[StoneAdvCount]["COLOR"]);
                    //        commandStoneAdv.Parameters.Add("@SIZEID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtOrderStnAdv.Rows[StoneAdvCount]["SIZE"]);
                    //        commandStoneAdv.Parameters.Add("@STYLE", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtOrderStnAdv.Rows[StoneAdvCount]["STYLE"]);
                    //        if(string.IsNullOrEmpty(Convert.ToString(dtOrderStnAdv.Rows[StoneAdvCount]["PCS"])))
                    //            commandStoneAdv.Parameters.Add("@PCS", SqlDbType.Decimal).Value = DBNull.Value;
                    //        else
                    //            commandStoneAdv.Parameters.Add("@PCS", SqlDbType.Decimal).Value = Convert.ToDecimal(dtOrderStnAdv.Rows[StoneAdvCount]["PCS"]);

                    //        if(string.IsNullOrEmpty(Convert.ToString(dtOrderStnAdv.Rows[StoneAdvCount]["QUANTITY"])))
                    //            commandStoneAdv.Parameters.Add("@QTY", SqlDbType.Decimal).Value = DBNull.Value;
                    //        else
                    //            commandStoneAdv.Parameters.Add("@QTY", SqlDbType.Decimal).Value = Convert.ToDecimal(dtOrderStnAdv.Rows[StoneAdvCount]["QUANTITY"]);

                    //        if(application != null)
                    //            commandStoneAdv.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = application.Settings.Database.DataAreaID;
                    //        else
                    //            commandStoneAdv.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = ApplicationSettings.Database.DATAAREAID;


                    //        if(string.IsNullOrEmpty(Convert.ToString(dtOrderStnAdv.Rows[StoneAdvCount]["INVENTDIMID"])))
                    //            commandStoneAdv.Parameters.Add("@INVENTDIMID", SqlDbType.NVarChar, 20).Value = "";
                    //        else
                    //            commandStoneAdv.Parameters.Add("@INVENTDIMID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtOrderStnAdv.Rows[StoneAdvCount]["INVENTDIMID"]);


                    //        if(string.IsNullOrEmpty(Convert.ToString(dtOrderStnAdv.Rows[StoneAdvCount]["UNITID"])))
                    //            commandStoneAdv.Parameters.Add("@UNITID", SqlDbType.NVarChar, 20).Value = "";
                    //        else
                    //            commandStoneAdv.Parameters.Add("@UNITID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtOrderStnAdv.Rows[StoneAdvCount]["UNITID"]);

                    //        commandStoneAdv.Parameters.Add("@REMARKS", SqlDbType.NVarChar, 250).Value = Convert.ToString(dtOrderStnAdv.Rows[StoneAdvCount]["REMARKS"]);

                    //        commandStoneAdv.CommandTimeout = 0;
                    //        iCustOrder_StoneAdv = commandStoneAdv.ExecuteNonQuery();
                    //        commandStoneAdv.Dispose();
                    //    }
                    //}
                    #endregion

                    if (dtOrderDetails != null && dtOrderDetails.Rows.Count > 0)
                    {
                        #region ORDER DETAILS
                        string commandCustOrder_Detail = " INSERT INTO [CUSTORDER_DETAILS]([ORDERNUM],[LINENUM],[STOREID],[TERMINALID],[ITEMID],[CONFIGID] " +
                                                 " ,[CODE],[SIZEID],[STYLE],[PCS],[QTY],[CRATE],[RATETYPE],[AMOUNT],[MAKINGRATE],[MAKINGRATETYPE] " +
                                                 " ,[MAKINGAMOUNT],[EXTENDEDDETAILSAMOUNT],[DATAAREAID],[STAFFID],[INVENTDIMID],[UNITID]" +
                                                 " , WastageRate,WastageType,WastageQty,WastagePercentage,WastageAmount,LineTotalAmt,IsBookedSKU,REMARKSDTL)" +
                                                 " VALUES(@ORDERNUM,@LINENUM,@STOREID,@TERMINALID,@ITEMID ,@CONFIGID,@CODE ,@SIZEID,@STYLE,@PCS ,@QTY " +
                                                 " ,@RATE ,@RATETYPE,@AMOUNT  ,@MAKINGRATE ,@MAKINGRATETYPE,@MAKINGAMOUNT,@EXTENDEDDETAILSAMOUNT " +
                                                 " ,@DATAAREAID  ,@STAFFID , @INVENTDIMID, @UNITID " +
                                                 " ,@WastageRate,@WastageType,@WastageQty,@WastagePercentage,@WastageAmount,@LineTotalAmt,@IsBookedSKU,@REMARKSDTL)";

                        for (int ItemCount = 0; ItemCount < dtOrderDetails.Rows.Count; ItemCount++)
                        {

                            SqlCommand cmdCustOrder_Detail = new SqlCommand(commandCustOrder_Detail, connection, transaction);
                            cmdCustOrder_Detail.Parameters.Add("@ORDERNUM", SqlDbType.NVarChar, 20).Value = txtOrderNo.Text;
                            cmdCustOrder_Detail.Parameters.Add("@LINENUM", SqlDbType.NVarChar, 20).Value = ItemCount + 1;
                            cmdCustOrder_Detail.Parameters.Add("@STOREID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.StoreId;
                            cmdCustOrder_Detail.Parameters.Add("@TERMINALID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.TerminalId;
                            cmdCustOrder_Detail.Parameters.Add("@ITEMID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtOrderDetails.Rows[ItemCount]["ITEMID"]);
                            cmdCustOrder_Detail.Parameters.Add("@CONFIGID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtOrderDetails.Rows[ItemCount]["CONFIGURATION"]);
                            cmdCustOrder_Detail.Parameters.Add("@CODE", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtOrderDetails.Rows[ItemCount]["COLOR"]);
                            cmdCustOrder_Detail.Parameters.Add("@SIZEID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtOrderDetails.Rows[ItemCount]["SIZE"]);
                            cmdCustOrder_Detail.Parameters.Add("@STYLE", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtOrderDetails.Rows[ItemCount]["STYLE"]);
                            if (string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["PCS"])))
                                cmdCustOrder_Detail.Parameters.Add("@PCS", SqlDbType.Decimal).Value = 0;
                            else
                                cmdCustOrder_Detail.Parameters.Add("@PCS", SqlDbType.Decimal).Value = Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["PCS"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["QUANTITY"])))
                                cmdCustOrder_Detail.Parameters.Add("@QTY", SqlDbType.Decimal).Value = 0;
                            else
                                cmdCustOrder_Detail.Parameters.Add("@QTY", SqlDbType.Decimal).Value = Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["QUANTITY"]);


                            if (string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["RATE"])))
                                cmdCustOrder_Detail.Parameters.Add("@RATE", SqlDbType.Decimal).Value = 0;
                            else
                                cmdCustOrder_Detail.Parameters.Add("@RATE", SqlDbType.Decimal).Value = Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["RATE"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["RATETYPE"])))
                                cmdCustOrder_Detail.Parameters.Add("@RATETYPE", SqlDbType.NVarChar).Value = DBNull.Value;
                            else
                            {
                                //    cmdCustOrder_Detail.Parameters.Add("@RATETYPE", SqlDbType.NVarChar).Value = Convert.ToString(dtOrderDetails.Rows[ItemCount]["RATETYPE"]);
                                string rType = string.Empty;
                                if (Convert.ToString(dtOrderDetails.Rows[ItemCount]["RATETYPE"]) == Convert.ToString(RateType.Weight))
                                    rType = Convert.ToString((int)RateType.Weight);
                                else if (Convert.ToString(dtOrderDetails.Rows[ItemCount]["RATETYPE"]) == Convert.ToString(RateType.Pcs))
                                    rType = Convert.ToString((int)RateType.Pcs);
                                else
                                    rType = Convert.ToString((int)RateType.Tot);
                                cmdCustOrder_Detail.Parameters.Add("@RATETYPE", SqlDbType.NVarChar).Value = Convert.ToString(rType);
                            }
                            if (string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["AMOUNT"])))
                                //  cmdCustOrder_Detail.Parameters.Add("@AMOUNT", SqlDbType.Decimal).Value = DBNull.Value;
                                cmdCustOrder_Detail.Parameters.Add("@AMOUNT", SqlDbType.Decimal).Value = 0;
                            else
                                cmdCustOrder_Detail.Parameters.Add("@AMOUNT", SqlDbType.Decimal).Value = Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["AMOUNT"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["MAKINGRATE"])))
                                cmdCustOrder_Detail.Parameters.Add("@MAKINGRATE", SqlDbType.Decimal).Value = 0;
                            else
                                cmdCustOrder_Detail.Parameters.Add("@MAKINGRATE", SqlDbType.Decimal).Value = Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["MAKINGRATE"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["MAKINGTYPE"])))
                                cmdCustOrder_Detail.Parameters.Add("@MAKINGRATETYPE", SqlDbType.NVarChar).Value = DBNull.Value;
                            else
                            {
                                string mType = string.Empty;
                                if (Convert.ToString(dtOrderDetails.Rows[ItemCount]["MAKINGTYPE"]) == Convert.ToString(MakingType.Weight))
                                    mType = Convert.ToString((int)MakingType.Weight);
                                else if (Convert.ToString(dtOrderDetails.Rows[ItemCount]["MAKINGTYPE"]) == Convert.ToString(MakingType.Pieces))
                                    mType = Convert.ToString((int)MakingType.Pieces);
                                else if (Convert.ToString(dtOrderDetails.Rows[ItemCount]["MAKINGTYPE"]) == Convert.ToString(MakingType.Tot))
                                    mType = Convert.ToString((int)MakingType.Tot);
                                else
                                    mType = Convert.ToString((int)MakingType.Percentage);

                                cmdCustOrder_Detail.Parameters.Add("@MAKINGRATETYPE", SqlDbType.NVarChar).Value = Convert.ToString(mType);
                            }

                            if (string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["MAKINGAMOUNT"])))
                                // cmdCustOrder_Detail.Parameters.Add("@MAKINGAMOUNT", SqlDbType.Decimal).Value = DBNull.Value;
                                cmdCustOrder_Detail.Parameters.Add("@MAKINGAMOUNT", SqlDbType.Decimal).Value = 0;
                            else
                                cmdCustOrder_Detail.Parameters.Add("@MAKINGAMOUNT", SqlDbType.Decimal).Value = Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["MAKINGAMOUNT"]);


                            if (string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["EXTENDEDDETAILS"])))
                                cmdCustOrder_Detail.Parameters.Add("@EXTENDEDDETAILSAMOUNT", SqlDbType.Decimal).Value = 0;
                            else
                                cmdCustOrder_Detail.Parameters.Add("@EXTENDEDDETAILSAMOUNT", SqlDbType.Decimal).Value = Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["EXTENDEDDETAILS"]);



                            if (application != null)
                                cmdCustOrder_Detail.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = application.Settings.Database.DataAreaID;
                            else
                                cmdCustOrder_Detail.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = ApplicationSettings.Database.DATAAREAID;
                            cmdCustOrder_Detail.Parameters.Add("@STAFFID", SqlDbType.NVarChar, 10).Value = pos.OperatorId;

                            if (string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["INVENTDIMID"])))
                                cmdCustOrder_Detail.Parameters.Add("@INVENTDIMID", SqlDbType.NVarChar, 20).Value = "";
                            else
                                cmdCustOrder_Detail.Parameters.Add("@INVENTDIMID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtOrderDetails.Rows[ItemCount]["INVENTDIMID"]);


                            if (string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["UNITID"])))
                                cmdCustOrder_Detail.Parameters.Add("@UNITID", SqlDbType.NVarChar, 20).Value = "";
                            else
                                cmdCustOrder_Detail.Parameters.Add("@UNITID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtOrderDetails.Rows[ItemCount]["UNITID"]);

                            // Added for wastage 
                            if (string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["WastageRate"])))
                                cmdCustOrder_Detail.Parameters.Add("@WastageRate", SqlDbType.Decimal).Value = 0;
                            else
                                cmdCustOrder_Detail.Parameters.Add("@WastageRate", SqlDbType.Decimal).Value = Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["WastageRate"]);


                            if (string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["WastageType"])))
                                cmdCustOrder_Detail.Parameters.Add("@WastageType", SqlDbType.NVarChar).Value = "0";
                            else
                            {
                                string rType = string.Empty;
                                if (Convert.ToString(dtOrderDetails.Rows[ItemCount]["WastageType"]) == Convert.ToString(WastageType.Weight))
                                    rType = Convert.ToString((int)WastageType.Weight);
                                else
                                    rType = Convert.ToString((int)WastageType.Percentage);
                                cmdCustOrder_Detail.Parameters.Add("@WastageType", SqlDbType.NVarChar).Value = Convert.ToString(rType);
                            }

                            if (string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["WastageQty"])))
                                cmdCustOrder_Detail.Parameters.Add("@WastageQty", SqlDbType.Decimal).Value = 0;
                            else
                                cmdCustOrder_Detail.Parameters.Add("@WastageQty", SqlDbType.Decimal).Value = Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["WastageQty"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["WastagePercentage"])))
                                cmdCustOrder_Detail.Parameters.Add("@WastagePercentage", SqlDbType.Decimal).Value = 0;
                            else
                                cmdCustOrder_Detail.Parameters.Add("@WastagePercentage", SqlDbType.Decimal).Value = Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["WastagePercentage"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["WastageAmount"])))
                                cmdCustOrder_Detail.Parameters.Add("@WastageAmount", SqlDbType.Decimal).Value = 0;
                            else
                                cmdCustOrder_Detail.Parameters.Add("@WastageAmount", SqlDbType.Decimal).Value = Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["WastageAmount"]);

                            cmdCustOrder_Detail.Parameters.Add("@LineTotalAmt", SqlDbType.Decimal).Value = (!string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["AMOUNT"])) ? Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["AMOUNT"]) : 0)
                                                                                                            + (!string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["MAKINGAMOUNT"])) ? Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["MAKINGAMOUNT"]) : 0)
                                                                                                            + (!string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["WastageAmount"])) ? Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["WastageAmount"]) : 0);

                            cmdCustOrder_Detail.Parameters.Add("@IsBookedSKU", SqlDbType.Int).Value = Convert.ToInt32(dtOrderDetails.Rows[ItemCount]["IsBookedSKU"]);
                            cmdCustOrder_Detail.Parameters.Add("@REMARKSDTL", SqlDbType.NVarChar, 30).Value = Convert.ToString(dtOrderDetails.Rows[ItemCount]["REMARKSDTL"]);

                            cmdCustOrder_Detail.CommandTimeout = 0;
                            iCustOrder_Details = cmdCustOrder_Detail.ExecuteNonQuery();
                            cmdCustOrder_Detail.Dispose();
                        #endregion

                            if (iCustOrder_Details == 1)
                            {
                                if (dtSubOrderDetails != null && dtSubOrderDetails.Rows.Count > 0)
                                {
                                    #region SUB ORDER DETAILS

                                    string commandCust_SubOrderDetails = " INSERT INTO [CUSTORDER_SUBDETAILS]([ORDERNUM],[ORDERDETAILNUM],[LINENUM],[STOREID],[TERMINALID],[ITEMID],[CONFIGID],[CODE] "
                                                                + " ,[SIZEID],[STYLE],[PCS],[QTY],[CRATE],[RATETYPE],[DATAAREAID],[AMOUNT],[STAFFID],[INVENTDIMID],[UNITID]) VALUES (@ORDERNUM,@ORDERDETAILNUM,@LINENUM, "
                                                                + " @STOREID,@TERMINALID,@ITEMID,@CONFIGID,@CODE,@SIZEID,@STYLE,@PCS,@QTY,@RATE,@RATETYPE,@DATAAREAID,@AMOUNT,@STAFFID,@INVENTDIMID,@UNITID) ";

                                    int i = 1;
                                    int index = 1;
                                    for (int PaymentCount = 0; PaymentCount < dtSubOrderDetails.Rows.Count; PaymentCount++)
                                    {
                                        if (Convert.ToString(dtOrderDetails.Rows[ItemCount]["UNIQUEID"]).Trim() == Convert.ToString(dtSubOrderDetails.Rows[PaymentCount]["UNIQUEID"]).Trim())
                                        {
                                            index = i;
                                            //int iLine = iSubOrder_LineNum + 1;
                                            //iSubOrder_LineNum = iLine;
                                            //iLine = iSubOrder_LineNum + 1;
                                            SqlCommand cmdOGP_PAYMENT = new SqlCommand(commandCust_SubOrderDetails, connection, transaction);
                                            cmdOGP_PAYMENT.Parameters.Add("@ORDERNUM", SqlDbType.NVarChar, 20).Value = txtOrderNo.Text;
                                            cmdOGP_PAYMENT.Parameters.Add("@ORDERDETAILNUM", SqlDbType.NVarChar, 20).Value = ItemCount + 1;
                                            cmdOGP_PAYMENT.Parameters.Add("@LINENUM", SqlDbType.NVarChar, 20).Value = index;
                                            cmdOGP_PAYMENT.Parameters.Add("@STOREID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.StoreId;
                                            cmdOGP_PAYMENT.Parameters.Add("@ITEMID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtSubOrderDetails.Rows[PaymentCount]["ITEMID"]);
                                            cmdOGP_PAYMENT.Parameters.Add("@TERMINALID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.TerminalId;
                                            cmdOGP_PAYMENT.Parameters.Add("@CONFIGID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtSubOrderDetails.Rows[PaymentCount]["CONFIGURATION"]);
                                            cmdOGP_PAYMENT.Parameters.Add("@CODE", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtSubOrderDetails.Rows[PaymentCount]["COLOR"]);
                                            cmdOGP_PAYMENT.Parameters.Add("@SIZEID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtSubOrderDetails.Rows[PaymentCount]["SIZE"]);
                                            cmdOGP_PAYMENT.Parameters.Add("@STYLE", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtSubOrderDetails.Rows[PaymentCount]["STYLE"]);

                                            if (string.IsNullOrEmpty(Convert.ToString(dtSubOrderDetails.Rows[PaymentCount]["PCS"])))
                                                cmdOGP_PAYMENT.Parameters.Add("@PCS", SqlDbType.Decimal).Value = DBNull.Value;
                                            else
                                                cmdOGP_PAYMENT.Parameters.Add("@PCS", SqlDbType.Decimal).Value = Convert.ToDecimal(dtSubOrderDetails.Rows[PaymentCount]["PCS"]);

                                            if (string.IsNullOrEmpty(Convert.ToString(dtSubOrderDetails.Rows[PaymentCount]["QUANTITY"])))
                                                cmdOGP_PAYMENT.Parameters.Add("@QTY", SqlDbType.Decimal).Value = DBNull.Value;
                                            else
                                                cmdOGP_PAYMENT.Parameters.Add("@QTY", SqlDbType.Decimal).Value = Convert.ToDecimal(dtSubOrderDetails.Rows[PaymentCount]["QUANTITY"]);


                                            if (string.IsNullOrEmpty(Convert.ToString(dtSubOrderDetails.Rows[PaymentCount]["RATE"])))
                                                cmdOGP_PAYMENT.Parameters.Add("@RATE", SqlDbType.Decimal).Value = DBNull.Value;
                                            else
                                                cmdOGP_PAYMENT.Parameters.Add("@RATE", SqlDbType.Decimal).Value = Convert.ToDecimal(dtSubOrderDetails.Rows[PaymentCount]["RATE"]);

                                            if (string.IsNullOrEmpty(Convert.ToString(dtSubOrderDetails.Rows[PaymentCount]["RATETYPE"])))
                                                cmdOGP_PAYMENT.Parameters.Add("@RATETYPE", SqlDbType.NVarChar).Value = DBNull.Value;
                                            else
                                            {
                                                string rType = string.Empty;
                                                if (Convert.ToString(dtSubOrderDetails.Rows[PaymentCount]["RATETYPE"]) == Convert.ToString(RateType.Weight))
                                                    rType = Convert.ToString((int)RateType.Weight);
                                                else if (Convert.ToString(dtSubOrderDetails.Rows[PaymentCount]["RATETYPE"]) == Convert.ToString(RateType.Pcs))
                                                    rType = Convert.ToString((int)RateType.Pcs);
                                                else
                                                    rType = Convert.ToString((int)RateType.Tot);
                                                cmdOGP_PAYMENT.Parameters.Add("@RATETYPE", SqlDbType.NVarChar).Value = Convert.ToString(rType);
                                            }

                                            if (string.IsNullOrEmpty(Convert.ToString(dtSubOrderDetails.Rows[PaymentCount]["AMOUNT"])))
                                                cmdOGP_PAYMENT.Parameters.Add("@AMOUNT", SqlDbType.Decimal).Value = DBNull.Value;
                                            else
                                                cmdOGP_PAYMENT.Parameters.Add("@AMOUNT", SqlDbType.Decimal).Value = Convert.ToDecimal(dtSubOrderDetails.Rows[PaymentCount]["AMOUNT"]);



                                            if (application != null)
                                                cmdOGP_PAYMENT.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = application.Settings.Database.DataAreaID;
                                            else
                                                cmdOGP_PAYMENT.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = ApplicationSettings.Database.DATAAREAID;
                                            cmdOGP_PAYMENT.Parameters.Add("@STAFFID", SqlDbType.NVarChar, 10).Value = pos.OperatorId;

                                            if (string.IsNullOrEmpty(Convert.ToString(dtSubOrderDetails.Rows[PaymentCount]["INVENTDIMID"])))
                                                cmdOGP_PAYMENT.Parameters.Add("@INVENTDIMID", SqlDbType.NVarChar, 20).Value = string.Empty;
                                            else
                                                cmdOGP_PAYMENT.Parameters.Add("@INVENTDIMID", SqlDbType.NVarChar, 20).Value = dtSubOrderDetails.Rows[PaymentCount]["INVENTDIMID"];

                                            if (string.IsNullOrEmpty(Convert.ToString(dtSubOrderDetails.Rows[PaymentCount]["UNITID"])))
                                                cmdOGP_PAYMENT.Parameters.Add("@UNITID", SqlDbType.NVarChar, 20).Value = string.Empty;
                                            else
                                                cmdOGP_PAYMENT.Parameters.Add("@UNITID", SqlDbType.NVarChar, 20).Value = dtSubOrderDetails.Rows[PaymentCount]["UNITID"];



                                            cmdOGP_PAYMENT.CommandTimeout = 0;
                                            iCustOrder_SubDetails = cmdOGP_PAYMENT.ExecuteNonQuery();
                                            cmdOGP_PAYMENT.Dispose();
                                            i++;
                                        }
                                    }
                                    #endregion
                                }
                            }
                        }

                        #region Sample Details Entry, //Nimbus by MIAM @ 09Jun14 : Added
                        if (dtSampleDetails != null && dtSampleDetails.Rows.Count > 0)
                        {
                            StringBuilder commandText1 = new StringBuilder();
                            foreach (DataRow dr in dtSampleDetails.Rows)
                            {
                                commandText1.Append(@"INSERT INTO [CUSTORDSAMPLE]
                                                               ([ORDERNUM]
                                                                ,[LINENUM],[STOREID]
                                                               ,[TERMINALID],[ITEMID]
                                                               ,[CONFIGID],[CODE]
                                                               ,[SIZEID],[STYLE]
                                                                ,[INVENTDIMID],[PCS]
                                                                ,[QTY],[NETTWT]
		                                                       ,[DIAWT],[DIAAMT]
                                                                ,[STNWT],[STNAMT]
                                                                ,[TOTALAMT],[DATAAREAID]
                                                               ,[STAFFID],[UNITID]
                                                                ,[REMARKSDTL],[ISRETURNED])
                                                             VALUES");
                                commandText1.AppendLine();
                                commandText1.AppendFormat("('{0}'", txtOrderNo.Text);
                                commandText1.AppendFormat(",'{0}','{1}'", dr["LINENUM"].ToString(), ApplicationSettings.Terminal.StoreId);
                                commandText1.AppendFormat(",'{0}','{1}'", ApplicationSettings.Terminal.TerminalId, dr["ITEMID"].ToString());
                                commandText1.AppendFormat(",'{0}','{1}'", dr["CONFIGURATION"].ToString(), dr["COLOR"].ToString());
                                commandText1.AppendFormat(",'{0}','{1}'", dr["SIZE"].ToString(), dr["STYLE"].ToString());
                                commandText1.AppendFormat(",'{0}','{1}'", dr["INVENTDIMID"].ToString(), dr["PCS"].ToString());
                                commandText1.AppendFormat(",'{0}','{1}'", dr["QUANTITY"].ToString(), dr["NETTWT"].ToString());
                                commandText1.AppendFormat(",'{0}','{1}'", dr["DIAWT"].ToString(), dr["DIAAMT"].ToString());
                                commandText1.AppendFormat(",'{0}','{1}'", dr["STNWT"].ToString(), dr["STNAMT"].ToString());
                                commandText1.AppendFormat(",'{0}','{1}'", dr["TOTALAMT"].ToString(), ApplicationSettings.Database.DATAAREAID);
                                commandText1.AppendFormat(",'{0}','{1}'", pos.OperatorId, dr["UNITID"].ToString());
                                commandText1.AppendFormat(",'{0}','{1}');", dr["RemarksDtl"].ToString(), Convert.ToInt16(Convert.ToBoolean(dr["ISRETURNED"].ToString())));
                                commandText1.AppendLine();
                            }

                            using (SqlCommand cmdCustOrder_SampleDetail = new SqlCommand(commandText1.ToString(), connection, transaction))
                            {
                                cmdCustOrder_SampleDetail.CommandTimeout = 0;
                                cmdCustOrder_SampleDetail.ExecuteNonQuery();
                                cmdCustOrder_SampleDetail.Dispose();
                            }
                        }
                        #endregion

                        #region Receive Stone or loose Diamond Details Entry,
                        if (dtRecvStoneDetails != null && dtRecvStoneDetails.Rows.Count > 0)
                        {
                            int iIsreturn = 0;
                            int iL = 1;
                            foreach (DataRow dr in dtRecvStoneDetails.Rows)
                            {
                                string QRY = "INSERT INTO [CUSTORDSTONE] " +
                                           "([ORDERNUM],[LINENUM],[REFLINENUM],[STOREID],[TERMINALID],[ITEMID],[CODE],[SIZEID],[STYLE] " +
                                           " ,[PCS],[QTY],[NETWT],[DATAAREAID] " +
                                           ",[STAFFID],[REMARKSDTL],[ISRETURNED],[STONEBATCHID],[UNITID]) " +
                                           " VALUES" +
                                           "(@OrderNum,@LineNUM,@REFLINENUM,@STOREID,@TERMINALID,@ITEMID,@Code,@sizeid,@STYLE,@PCS," +
                                           " @QTY,@NETWT,@DATAAREAID," +
                                           " @STAFFID,@REMARKSDTL,@ISRETURNED,@STONEBATCHID,@UNITID)";

                                using (SqlCommand cmdCustOrder_StoneDetail = new SqlCommand(QRY, connection, transaction))
                                {
                                    cmdCustOrder_StoneDetail.CommandTimeout = 0;
                                    cmdCustOrder_StoneDetail.Parameters.Add("@DATAAREAID", SqlDbType.VarChar, 50).Value = ApplicationSettings.Database.DATAAREAID;
                                    cmdCustOrder_StoneDetail.Parameters.Add("@ITEMID", SqlDbType.VarChar, 50).Value = dr["itemID"].ToString();
                                    cmdCustOrder_StoneDetail.Parameters.Add("@STAFFID", SqlDbType.VarChar, 50).Value = pos.OperatorId;
                                    cmdCustOrder_StoneDetail.Parameters.Add("@QTY", SqlDbType.Decimal).Value = Convert.ToDecimal(dr["QTY"].ToString());
                                    cmdCustOrder_StoneDetail.Parameters.Add("@NETWT", SqlDbType.Decimal).Value = Convert.ToDecimal(dr["NETWT"].ToString());
                                    cmdCustOrder_StoneDetail.Parameters.Add("@STOREID", SqlDbType.VarChar, 50).Value = ApplicationSettings.Database.StoreID;
                                    cmdCustOrder_StoneDetail.Parameters.Add("@TERMINALID", SqlDbType.VarChar, 50).Value = ApplicationSettings.Database.TerminalID;
                                    cmdCustOrder_StoneDetail.Parameters.Add("@RECVDATE", SqlDbType.DateTime).Value = dTPickerOrderDate.Value;
                                    cmdCustOrder_StoneDetail.Parameters.Add("@OrderNum", SqlDbType.NVarChar, 20).Value = txtOrderNo.Text.Trim();
                                    cmdCustOrder_StoneDetail.Parameters.Add("@PCS", SqlDbType.Int).Value = Convert.ToInt32(dr["PCS"].ToString());
                                    cmdCustOrder_StoneDetail.Parameters.Add("@LineNUM", SqlDbType.Int).Value = Convert.ToInt32(dr["LineNum"].ToString());
                                    cmdCustOrder_StoneDetail.Parameters.Add("@REFLINENUM", SqlDbType.Int).Value = Convert.ToInt32(dr["REFLINENUM"].ToString());

                                    cmdCustOrder_StoneDetail.Parameters.Add("@REMARKSDTL", SqlDbType.NVarChar, 250).Value = dr["RemarksDtl"].ToString();
                                    if (Convert.ToBoolean(dr["ISRETURNED"]))
                                        iIsreturn = 1;
                                    else
                                        iIsreturn = 0;

                                    cmdCustOrder_StoneDetail.Parameters.Add("@ISRETURNED", SqlDbType.Int).Value = iIsreturn;
                                    cmdCustOrder_StoneDetail.Parameters.Add("@Code", SqlDbType.NVarChar, 50).Value = dr["COLOR"].ToString();
                                    cmdCustOrder_StoneDetail.Parameters.Add("@sizeid", SqlDbType.NVarChar, 50).Value = dr["Size"].ToString();
                                    cmdCustOrder_StoneDetail.Parameters.Add("@STYLE", SqlDbType.NVarChar, 50).Value = dr["STYLE"].ToString();
                                    cmdCustOrder_StoneDetail.Parameters.Add("@STONEBATCHID", SqlDbType.NVarChar, 50).Value = dr["STONEBATCHID"].ToString(); //txtOrderNo.Text.Trim() + "/" + Convert.ToInt32(dr["LineNum"].ToString()) + "/" + iL;
                                    cmdCustOrder_StoneDetail.Parameters.Add("@UNITID", SqlDbType.NVarChar, 50).Value = dr["UNITID"].ToString();

                                    cmdCustOrder_StoneDetail.ExecuteNonQuery();
                                    cmdCustOrder_StoneDetail.Dispose();
                                    iL++;
                                }
                            }
                        }

                        #endregion

                        #region Order payment schedule
                        if (dtPaySchedule != null && dtPaySchedule.Rows.Count > 0)
                        {

                            int iIsreturn = 0;
                            int iL = 1;
                            foreach (DataRow dr in dtPaySchedule.Rows)
                            {
                                string QRY = "INSERT INTO [CUSTORDERPAYSCHEDULE] " +
                                           "([ORDERNUM],[STOREID],[TERMINALID],[ORDERDATE],[PAYDATE],[PEROFAMT],[AMOUNT]) " +
                                           " VALUES" +
                                           "(@ORDERNUM,@STOREID,@TERMINALID,@ORDERDATE,@PAYDATE,@PEROFAMT," +
                                           " @AMOUNT)";

                                using (SqlCommand cmdCustOrder_PaySch = new SqlCommand(QRY, connection, transaction))
                                {
                                    cmdCustOrder_PaySch.CommandTimeout = 0;
                                    cmdCustOrder_PaySch.Parameters.Add("@ORDERNUM", SqlDbType.VarChar, 50).Value = txtOrderNo.Text;
                                    cmdCustOrder_PaySch.Parameters.Add("@STOREID", SqlDbType.VarChar, 50).Value = ApplicationSettings.Terminal.StoreId;
                                    cmdCustOrder_PaySch.Parameters.Add("@TERMINALID", SqlDbType.VarChar, 50).Value = ApplicationSettings.Terminal.TerminalId;
                                    cmdCustOrder_PaySch.Parameters.Add("@ORDERDATE", SqlDbType.DateTime).Value = dTPickerOrderDate.Value;
                                    cmdCustOrder_PaySch.Parameters.Add("@PAYDATE", SqlDbType.DateTime).Value = Convert.ToDateTime(dr["PAYDATE"].ToString());
                                    cmdCustOrder_PaySch.Parameters.Add("@PEROFAMT", SqlDbType.Decimal).Value = Convert.ToDecimal(dr["PerAmt"].ToString());
                                    cmdCustOrder_PaySch.Parameters.Add("@AMOUNT", SqlDbType.Decimal).Value = Convert.ToDecimal(dr["Amount"].ToString());
                                    cmdCustOrder_PaySch.ExecuteNonQuery();
                                    cmdCustOrder_PaySch.Dispose();
                                    iL++;
                                }
                            }
                        }
                        #endregion
                    }
                }
                transaction.Commit();
                command.Dispose();
                transaction.Dispose();

                //Updating order with sketch
                if (dtSketchDetails != null && dtSketchDetails.Rows.Count > 0)
                {
                    NIM_OrderUpdateWithSketch();
                }
                if (dtSampleSketch != null && dtSampleSketch.Rows.Count > 0)
                {
                    NIM_SaveOrderSampleSketch();
                }


                if (iCustOrder_SubDetails == 1 || iCustOrder_Details == 1)
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Order has been created successfully.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        try
                        {
                            PrintVoucher();
                        }
                        catch { }

                        sCustOrder = txtOrderNo.Text;
                        txtOrderNo.Text = GetOrderNum();
                        dtOrderDetails = new DataTable();
                        dtSubOrderDetails = new DataTable();
                        sOrderDetailsAmount = string.Empty;
                        sSubOrderDetailsAmount = string.Empty;
                        sCustOrderSearchNumber = string.Empty;
                        dsOrderSearched = new DataSet();
                        sCustAcc = txtCustomerAccount.Text;
                        sTotalAmt = txtTotalAmount.Text;
                        CLearControls();
                        bDataSaved = true;
                        this.Close();

                    }
                }
                else
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("DataBase error occured.Please try again later.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    }
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                transaction.Dispose();

                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage(ex.Message.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);

                }

            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
        #endregion

        #region Search Order
        private void btnSearchOrder_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(sCustOrderSearchNumber))
            {
                DataTable dtGridItems = new DataTable();

                string commandText = " SELECT ORDERNUM,CONVERT(VARCHAR(15),ORDERDATE,103) AS ORDERDATE ,CONVERT(VARCHAR(15),DELIVERYDATE,103) AS DELIVERYDATE , " +
                                     " CUSTNAME ,CONVERT(VARCHAR(15),TOTALAMOUNT) AS TOTALAMOUNT  FROM CUSTORDER_HEADER ORDER BY ORDERNUM ";
                SqlConnection connection = new SqlConnection();

                if (application != null)
                    connection = application.Settings.Database.Connection;
                else
                    connection = ApplicationSettings.Database.LocalConnection;


                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }


                SqlCommand command = new SqlCommand(commandText, connection);


                command.CommandTimeout = 0;
                SqlDataReader reader = command.ExecuteReader();
                dtGridItems = new DataTable();
                dtGridItems.Load(reader);
                if (dtGridItems != null && dtGridItems.Rows.Count > 0)
                {
                    DataRow selRow = null;
                    Dialog objCustOrderSearch = new Dialog();

                    objCustOrderSearch.GenericSearch(dtGridItems, ref selRow, "Customer Order");
                    if (selRow != null)
                        sCustOrderSearchNumber = Convert.ToString(selRow["ORDERNUM"]);
                    else
                        return;
                    //  grItems.DataSource = dtGridItems.DefaultView;
                }
                else
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No Order Exists.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    }
                    return;
                }

                //BlankOperations.WinFormsTouch.frmSearchOrder objSearchOrder = new BlankOperations.WinFormsTouch.frmSearchOrder(pos, application, this);
                //objSearchOrder.ShowDialog();
            }

            if (!string.IsNullOrEmpty(sCustOrderSearchNumber))
            {
                dsOrderSearched = new DataSet();

                string commandText = " SELECT ORDERNUM, ORDERDATE, DELIVERYDATE, CUSTACCOUNT, CUSTNAME,CUSTADDRESS,CUSTPHONE,TOTALAMOUNT FROM CUSTORDER_HEADER WHERE ORDERNUM = '" + sCustOrderSearchNumber.Trim() + "'; " +
                                     " SELECT ORDERNUM, LINENUM, ITEMID, CONFIGID, CODE, SIZEID, STYLE, PCS, QTY AS QUANTITY, CRATE AS RATE, " +
                                     " CASE WHEN RATETYPE=0 THEN '" + RateType.Weight + "' WHEN RATETYPE=1 THEN '" + RateType.Pcs + "' WHEN RATETYPE=2 THEN '" + RateType.Tot + "' END AS RATETYPE, " +
                                     " AMOUNT, MAKINGRATE, " +
                                     " CASE WHEN MAKINGRATETYPE=0 THEN '" + MakingType.Weight + "' WHEN MAKINGRATETYPE=1 THEN '" + MakingType.Pieces + "' WHEN MAKINGRATETYPE=2 THEN '" + MakingType.Tot + "' WHEN MAKINGRATETYPE=3 THEN '" + MakingType.Percentage + "' END AS MAKINGTYPE, " +
                    //   " MAKINGAMOUNT, EXTENDEDDETAILSAMOUNT AS EXTENDEDDETAILS, (ISNULL(AMOUNT,0) + ISNULL(MAKINGAMOUNT,0) + ISNULL(EXTENDEDDETAILSAMOUNT,0)) AS ROWTOTALAMOUNT," +
                                     " MAKINGAMOUNT, EXTENDEDDETAILSAMOUNT AS EXTENDEDDETAILS, (ISNULL(AMOUNT,0) + ISNULL(MAKINGAMOUNT,0) + ISNULL(WASTAGEAMOUNT,0)) AS ROWTOTALAMOUNT," +
                                     " CASE WHEN WastageType=0 THEN '" + WastageType.Weight + "' WHEN WastageType=1 THEN '" + WastageType.Percentage + "' END AS WastageType" +
                                     " ,ISNULL(WastageRate,0) AS WastageRate, ISNULL(WastageQty,0) AS WastageQty, ISNULL(WastagePercentage,0) AS WastagePercentage,ISNULL(WastageAmount,0) AS WastageAmount,CAST(ISNULL(IsBookedSKU,0) AS BIT) AS IsBookedSKU" +
                                     " ,SKETCH FROM CUSTORDER_DETAILS WHERE ORDERNUM ='" + sCustOrderSearchNumber.Trim() + "'; " +
                                     " SELECT ORDERNUM, ORDERDETAILNUM, LINENUM, ITEMID, CONFIGID, CODE, SIZEID, STYLE, PCS, QTY AS QUANTITY, CRATE AS RATE, " +
                                     " CASE WHEN RATETYPE=0 THEN '" + RateType.Weight + "' WHEN RATETYPE=1 THEN '" + RateType.Pcs + "' WHEN RATETYPE=2 THEN '" + RateType.Tot + "' END AS RATETYPE, " +
                                     " AMOUNT FROM CUSTORDER_SUBDETAILS WHERE ORDERNUM='" + sCustOrderSearchNumber.Trim() + "'; " +

                                     " SELECT [ORDERNUM],[LINENUM],[STOREID],[TERMINALID],[ITEMID] " +
                                     " ,[CONFIGID] [CONFIGURATION],[CODE] [COLOR],[SIZEID] [SIZE],[STYLE],[INVENTDIMID],[PCS],[QTY] [QUANTITY],[NETTWT],[DIAWT],[DIAAMT],[STNWT] " +
                                     " ,[STNAMT],[TOTALAMT],[DATAAREAID],[STAFFID],[REPLICATIONCOUNTER],[UNITID],[REMARKSDTL],[ISRETURNED]  FROM [CUSTORDSAMPLE] WHERE ORDERNUM='" + sCustOrderSearchNumber.Trim() + "' " +

                                     //" SELECT ORDERNUM, LINENUM, ITEMID, CONFIGID as CONFIGURATION, CODE as COLOR, SIZEID, STYLE, PCS, QTY AS QUANTITY, " +
                    //" REMARKS FROM CUSTORDERSTONEADVANCE WHERE ORDERNUM='" + sCustOrderSearchNumber.Trim() + "'";

                                        " SELECT [ORDERNUM],[LINENUM],REFLINENUM,[STOREID],[TERMINALID],[ITEMID] " +
                                       " ,[CODE] [COLOR],[SIZEID] [SIZE],[STYLE],[PCS],[QTY] " +
                                       " ,[NETWT],[DATAAREAID],[STAFFID],[REMARKSDTL],[ISRETURNED],[STONEBATCHID],UNITID" +
                                       " FROM [CUSTORDSTONE] WHERE ORDERNUM='" + sCustOrderSearchNumber.Trim() + "'" +

                                       " SELECT " +
                                       " [PAYDATE] as PayDate ,[PEROFAMT] as PerAmt ,[AMOUNT] as Amount  " +
                                       " FROM [CUSTORDERPAYSCHEDULE] WHERE ORDERNUM='" + sCustOrderSearchNumber.Trim() + "'";


                SqlConnection connection = new SqlConnection();

                if (application != null)
                    connection = application.Settings.Database.Connection;
                else
                    connection = ApplicationSettings.Database.LocalConnection;


                if (connection.State == ConnectionState.Closed)
                    connection.Open();


                SqlCommand command = new SqlCommand(commandText, connection);


                command.CommandTimeout = 0;

                SqlDataAdapter adapter = new SqlDataAdapter(commandText, connection);
                dsOrderSearched = new DataSet();
                adapter.Fill(dsOrderSearched);

                if (dsOrderSearched != null && dsOrderSearched.Tables.Count > 0)
                {
                    panel2.Enabled = false;
                    //btnStoneAdv.Enabled = false;
                    btnCustomerSearch.Enabled = false;
                    btnAddCustomer.Enabled = false;
                    btnSave.Enabled = false;
                    txtOrderNo.Text = Convert.ToString(dsOrderSearched.Tables[0].Rows[0]["ORDERNUM"]);
                    txtCustomerAccount.Text = Convert.ToString(dsOrderSearched.Tables[0].Rows[0]["CUSTACCOUNT"]);
                    txtCustomerAddress.Text = Convert.ToString(dsOrderSearched.Tables[0].Rows[0]["CUSTADDRESS"]);
                    txtPhoneNumber.Text = Convert.ToString(dsOrderSearched.Tables[0].Rows[0]["CUSTPHONE"]);
                    txtCustomerName.Text = Convert.ToString(dsOrderSearched.Tables[0].Rows[0]["CUSTNAME"]);
                    txtTotalAmount.Text = Convert.ToString(dsOrderSearched.Tables[0].Rows[0]["TOTALAMOUNT"]);
                    dTPickerOrderDate.Text = Convert.ToString(dsOrderSearched.Tables[0].Rows[0]["ORDERDATE"]);
                    dtPickerDeliveryDate.Text = Convert.ToString(dsOrderSearched.Tables[0].Rows[0]["DELIVERYDATE"]);
                    dtOrderDetails = new DataTable();
                }
            }


        }
        #endregion

        #region CLOSE CLICK
        private void btnClose_Click(object sender, EventArgs e)
        {
            if (dtOrderDetails != null && dtOrderDetails.Rows.Count > 0)
            {
                for (int ItemCount = 0; ItemCount < dtOrderDetails.Rows.Count; ItemCount++)
                {
                    SKUInfo(Convert.ToString(dtOrderDetails.Rows[ItemCount]["ITEMID"]), false);
                }
            }
            this.Close();
        }
        #endregion

        #region isValid()
        private bool isValid()
        {
            if (string.IsNullOrEmpty(txtCustomerAccount.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Customer Account can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            if (string.IsNullOrEmpty(txtCustomerName.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Customer Name can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            if (Convert.ToDateTime(dtPickerDeliveryDate.Text) < Convert.ToDateTime(dTPickerOrderDate.Text))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Delivery Date cannot be less than Order Date", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    dtPickerDeliveryDate.Text = dTPickerOrderDate.Text;
                    return false;
                }

            }


            else
            {
                return true;
            }
        }
        #endregion

        #region Clear Controls
        private void CLearControls()
        {
            txtCustomerName.Text = string.Empty;
            txtCustomerAccount.Text = string.Empty;
            txtCustomerAddress.Text = string.Empty;
            txtPhoneNumber.Text = string.Empty;
            txtTotalAmount.Text = string.Empty;
            dTPickerOrderDate.Text = DateTime.Now.ToString();
            dtPickerDeliveryDate.Text = DateTime.Now.ToString();
        }
        #endregion

        #region Clear CLick
        private void btnClear_Click(object sender, EventArgs e)
        {
            if (dtOrderDetails != null && dtOrderDetails.Rows.Count > 0)
            {
                for (int ItemCount = 0; ItemCount < dtOrderDetails.Rows.Count; ItemCount++)
                {
                    SKUInfo(Convert.ToString(dtOrderDetails.Rows[ItemCount]["ITEMID"]), false);
                }
            }
            dtOrderDetails = new DataTable();
            dtSubOrderDetails = new DataTable();
            sOrderDetailsAmount = string.Empty;
            sSubOrderDetailsAmount = string.Empty;
            sCustOrderSearchNumber = string.Empty;
            dsOrderSearched = new DataSet();
            txtOrderNo.Text = GetOrderNum();
            panel2.Enabled = true;
            btnStoneAdv.Enabled = true;
            btnCustomerSearch.Enabled = true;
            btnAddCustomer.Enabled = true;
            btnSave.Enabled = true;
            CLearControls();
        }
        #endregion

        private decimal GetFixedMetalRate(ref string sConfigId) // Fixed Metal Rate New
        {
            decimal dMetalRate = 0m;
            string storeId = string.Empty;
            SqlConnection conn = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);

            storeId = ApplicationSettings.Database.StoreID;
            StringBuilder commandText = new StringBuilder();
            commandText.Append(" DECLARE @INVENTLOCATION VARCHAR(20) ");
            commandText.Append(" DECLARE @CONFIGID VARCHAR(20) ");
            commandText.Append(" DECLARE @RATE numeric(28, 3) ");
            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM RETAILCHANNELTABLE INNER JOIN  ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE RETAILSTORETABLE.STORENUMBER='" + storeId + "' ");

            commandText.Append(" SELECT @CONFIGID = DEFAULTCONFIGIDGOLD FROM [INVENTPARAMETERS] WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "' ");
            commandText.Append(" SELECT TOP 1 CAST(ISNULL(RATES,0) AS NUMERIC(28,2))AS RATES,@CONFIGID FROM METALRATES WHERE INVENTLOCATIONID=@INVENTLOCATION ");
            commandText.Append(" AND METALTYPE = 1 AND ACTIVE=1 "); // METALTYPE -- > GOLD
            commandText.Append(" AND CONFIGIDSTANDARD=@CONFIGID  AND RATETYPE = 3 AND RETAIL = 0 "); // RATETYPE -- > SALE
            commandText.Append(" ORDER BY DATEADD(second, [TIME], [TRANSDATE]) DESC");

            //  enum RateTypeNew
            //   {
            //       Purchase = 0,
            //       OGP = 1,
            //       OGOP = 2,
            //       Sale = 3,
            //       GSS = 4,
            //       Exchange = 6,
            //   }

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    dMetalRate = Convert.ToDecimal(reader.GetValue(0));
                    sConfigId = Convert.ToString(reader.GetValue(1));
                }
            }

            if (conn.State == ConnectionState.Open)
                conn.Close();
            return dMetalRate;
        }

        private bool SKUInfo(string sItemId, bool bMode)
        {
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            if (bMode)
            {
                commandText.Append(" DECLARE @IsSKU AS INT; SET @IsSKU = 0; IF EXISTS   (SELECT TOP(1) [SkuNumber] FROM [SKUTableTrans] WHERE  [SkuNumber] = '" + sItemId + "'");
                commandText.Append(" AND isAvailable='True' AND isLocked='False' AND DATAAREAID='" + application.Settings.Database.DataAreaID + "') ");
                commandText.Append(" BEGIN SET @IsSKU = 1  UPDATE SKUTableTrans SET isAvailable='False',isLocked='False' WHERE SkuNumber = '" + sItemId + "' AND DATAAREAID='" + application.Settings.Database.DataAreaID + "' END SELECT @IsSKU");
            }
            else
            {
                commandText.Append("DECLARE @IsSKU AS INT; SET @IsSKU = 1; UPDATE SKUTableTrans SET isAvailable='True',isLocked='False' WHERE SkuNumber = '" + sItemId + "' AND DATAAREAID='" + application.Settings.Database.DataAreaID + "'  SELECT @IsSKU");
            }


            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;

            bool bVal = Convert.ToBoolean(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();

            return bVal;

        }

        private void btnStoneAdvance_Click(object sender, EventArgs e)
        {
            btnClear_Click(sender, e);

            #region stop stn adv against order, start against line
            ////frmStoneAdvance objOrderStnAdv = null;

            ////if(dtOrderDetails != null && dtOrderDetails.Rows.Count > 0)
            ////{
            ////    if(dtOrderStnAdv != null && dtOrderStnAdv.Rows.Count > 0)
            ////    {
            ////        objOrderStnAdv = new frmStoneAdvance(dtOrderStnAdv, pos, application, this);
            ////    }
            ////    else
            ////    {
            ////        dtOrderStnAdv = new DataTable();
            ////        objOrderStnAdv = new frmStoneAdvance(pos, application, this);
            ////    }
            ////    objOrderStnAdv.ShowDialog();
            ////}
            ////else if(dsOrderSearched != null && dsOrderSearched.Tables.Count > 0 && dsOrderSearched.Tables[4].Rows.Count > 0)
            ////{
            ////    objOrderStnAdv = new frmStoneAdvance(dsOrderSearched, pos, application, this);
            ////    objOrderStnAdv.ShowDialog();
            ////}
            ////else
            ////{
            ////    MessageBox.Show("No order item has been selected");
            ////}
            #endregion
        }

        #region Print Voucher
        private void PrintVoucher()
        {
            //PageSettings ps = new PageSettings { Landscape = false, PaperSize = new PaperSize { RawKind = (int)PaperKind.A4 }, Margins = new Margins { Top = 0, Right = 0, Bottom = 0, Left = 0 } };
            string sCompanyName = GetCompanyName();//aded on 14/04/2014 R.Hossain

            //datasources
            List<ReportDataSource> rds = new List<ReportDataSource>();
            rds.Add(new ReportDataSource("HEADERINFO", (DataTable)GetHeaderInfo()));
            rds.Add(new ReportDataSource("DETAILINFO", (DataTable)GetDetailInfo()));
            string sAmtinwds = Amtinwds(Math.Abs(Convert.ToDouble(sTotalAmt))); // added on 28/04/2014 RHossain               
            //parameters
            List<ReportParameter> rps = new List<ReportParameter>();
            rps.Add(new ReportParameter("StoreName", string.IsNullOrEmpty(ApplicationSettings.Terminal.StoreName) ? " " : ApplicationSettings.Terminal.StoreName, true));
            rps.Add(new ReportParameter("StoreAddress", string.IsNullOrEmpty(ApplicationSettings.Terminal.StoreAddress) ? " " : ApplicationSettings.Terminal.StoreAddress, true));
            rps.Add(new ReportParameter("StorePhone", string.IsNullOrEmpty(ApplicationSettings.Terminal.StorePhone) ? " " : ApplicationSettings.Terminal.StorePhone, true));
            rps.Add(new ReportParameter("Title", "Customer Order Voucher", true));
            rps.Add(new ReportParameter("CompName", sCompanyName, true));
            rps.Add(new ReportParameter("Amtinwds", sAmtinwds, true));

            string reportName = @"rptCustOrdVoucher";
            string reportPath = @"Microsoft.Dynamics.Retail.Pos.BlankOperations.Report." + reportName + ".rdlc";
            RdlcViewer rptView = new RdlcViewer("Customer Order Voucher", reportPath, rds, rps, null);
            rptView.ShowDialog();
        }
        private string GetCompanyName()
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;


            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string sCName = string.Empty;

            string sQry = "SELECT ISNULL(A.NAME,'') FROM DIRPARTYTABLE A INNER JOIN COMPANYINFO B" +
                " ON A.RECID = B.RECID WHERE B.DATAAREA = '" + ApplicationSettings.Database.DATAAREAID + "'";

            //using (SqlCommand cmd = new SqlCommand(sQry, conn))
            //{
            SqlCommand cmd = new SqlCommand(sQry, connection);
            cmd.CommandTimeout = 0;
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            sCName = Convert.ToString(cmd.ExecuteScalar());

            if (connection.State == ConnectionState.Open)
                connection.Close();
            //}

            return sCName;

        }
        private DataTable GetHeaderInfo()
        {

            sTotalAmt = Convert.ToString(txtTotalAmount.Text);

            DataTable dtHeader = new DataTable();
            dtHeader.Columns.Add("ORDERNO", typeof(string));
            dtHeader.Columns.Add("ORDERDATE", typeof(DateTime));
            dtHeader.Columns.Add("DELIVERYDATE", typeof(DateTime));
            dtHeader.Columns.Add("CUSTID", typeof(string));
            dtHeader.Columns.Add("CUSTNAME", typeof(string));
            dtHeader.Columns.Add("CUSTADD", typeof(string));
            dtHeader.Columns.Add("CUSTPHONE", typeof(string));
            dtHeader.Columns.Add("TOTALAMOUNT", typeof(decimal));
            dtHeader.Columns.Add("DELVERYDATEVISIBLE", typeof(bool));

            DataRow dr = dtHeader.NewRow();
            dr["ORDERNO"] = txtOrderNo.Text;
            dr["ORDERDATE"] = dTPickerOrderDate.Value;
            dr["DELIVERYDATE"] = dtPickerDeliveryDate.Value;
            dr["CUSTID"] = txtCustomerAccount.Text;
            dr["CUSTNAME"] = txtCustomerName.Text;
            dr["CUSTADD"] = txtCustomerAddress.Text;
            dr["CUSTPHONE"] = txtPhoneNumber.Text;
            dr["TOTALAMOUNT"] = Convert.ToDecimal(txtTotalAmount.Text);
            dr["DELVERYDATEVISIBLE"] = false;
            dtHeader.Rows.Add(dr);

            return dtHeader;
        }

        private DataTable GetDetailInfo()
        {
            DataTable dtDetails = new DataTable();
            dtDetails.Columns.Add("ITEMID", typeof(string));
            dtDetails.Columns.Add("PCS", typeof(decimal));
            dtDetails.Columns.Add("QTY", typeof(decimal));
            dtDetails.Columns.Add("RATE", typeof(decimal));
            dtDetails.Columns.Add("AMOUNT", typeof(decimal));
            dtDetails.Columns.Add("MAKINGRATE", typeof(decimal));
            dtDetails.Columns.Add("MAKINGAMOUNT", typeof(decimal));
            dtDetails.Columns.Add("TOTALAMOUNT", typeof(decimal));
            dtDetails.Columns.Add("REMARKS", typeof(string));

            foreach (DataRow item in dtOrderDetails.Rows)
            {
                DataRow dr = dtDetails.NewRow();
                dr["ITEMID"] = item["ITEMID"];
                dr["PCS"] = Convert.ToDecimal(item["PCS"]);
                dr["QTY"] = Convert.ToDecimal(item["QUANTITY"]);
                dr["RATE"] = Convert.ToDecimal(item["RATE"]);
                dr["AMOUNT"] = Convert.ToDecimal(item["AMOUNT"]);
                dr["MAKINGRATE"] = Convert.ToDecimal(item["MAKINGRATE"]);
                dr["MAKINGAMOUNT"] = Convert.ToDecimal(item["MAKINGAMOUNT"]);
                dr["TOTALAMOUNT"] = Convert.ToDecimal(item["ROWTOTALAMOUNT"]);
                dr["REMARKS"] = item["RemarksDtl"];
                dtDetails.Rows.Add(dr);
            }

            return dtDetails;
        }
        #endregion

        #region NIM_OrderUpdateWithSketch
        /// <summary>
        /// Created by : Ripan Hossain
        /// Created on : 24/04/2013
        /// Modified by :
        /// Modified on : 
        /// Purpose :Update Custom order with Sketch image 
        /// </summary>
        private void NIM_OrderUpdateWithSketch()
        {
            for (int sketchLine = 0; sketchLine < dtSketchDetails.Rows.Count; sketchLine++)
            {
                SqlConnection connection = new SqlConnection();
                if (application != null)
                    connection = application.Settings.Database.Connection;
                else
                    connection = ApplicationSettings.Database.LocalConnection;

                string commandText = " UPDATE [CUSTORDER_DETAILS] SET [SKETCH] = @SKETCH" +
                                   " WHERE ORDERNUM = @ORDERNUM AND LINENUM = @LINENUM AND STOREID = @STOREID AND TERMINALID = @TERMINALID";
                SqlCommand command = new SqlCommand(commandText, connection);

                if (string.IsNullOrEmpty(Convert.ToString(dtSketchDetails.Rows[sketchLine]["SKETCH"])))
                    command.Parameters.Add("@SKETCH", SqlDbType.Image).Value = DBNull.Value;
                else
                {
                    byte[] imageData = NIM_ReadFile(Convert.ToString(dtSketchDetails.Rows[sketchLine]["SKETCH"]));

                    command.Parameters.Add("@SKETCH", SqlDbType.Image).Value = imageData;// Base64ToImage(hexData); //hexData;

                    #region save into folder
                    string sArchivePath = string.Empty;
                    sArchivePath = GetArchivePathFromImage();
                    string path = sArchivePath + "" + txtOrderNo.Text + "_" + Convert.ToInt16(dtSketchDetails.Rows[sketchLine]["LINENUM"]) + "" + ".jpeg"; //

                    Bitmap b = new Bitmap(Convert.ToString(dtSketchDetails.Rows[sketchLine]["SKETCH"]));
                    b.Save(Convert.ToString(path));

                    #endregion
                }

                command.Parameters.Add("@ORDERNUM", SqlDbType.NVarChar, 20).Value = txtOrderNo.Text;
                command.Parameters.Add("@LINENUM", SqlDbType.NVarChar, 20).Value = Convert.ToInt16(dtSketchDetails.Rows[sketchLine]["LINENUM"]);
                command.Parameters.Add("@STOREID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.StoreId;
                command.Parameters.Add("@TERMINALID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.TerminalId;

                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                command.ExecuteNonQuery();
                connection.Close();
            }

        }
        #endregion

        /// <summary>
        /// Created by : Ripan Hossain
        /// Created on : 24/04/2013
        /// Modified by :
        /// Modified on : 
        /// Purpose :Open file in to a filestream and read data in a byte array.
        /// Read Image Bytes into a byte array
        /// </summary>
        /// <param name="sPath"></param>
        /// <returns></returns>
        byte[] NIM_ReadFile(string sPath)
        {
            //Initialize byte array with a null value initially.
            byte[] data = null;

            //Use FileInfo object to get file size.
            FileInfo fInfo = new FileInfo(sPath);
            long numBytes = fInfo.Length;

            //Open FileStream to read file
            FileStream fStream = new FileStream(sPath, FileMode.Open, FileAccess.Read);

            //Use BinaryReader to read file stream into byte array.
            BinaryReader br = new BinaryReader(fStream);

            //When you use BinaryReader, you need to supply number of bytes 
            //to read from file.
            //In this case we want to read entire file. 
            //So supplying total number of bytes.
            data = br.ReadBytes((int)numBytes);

            return data;
        }

        private string GetArchivePathFromImage()
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("select TOP(1) ARCHIVEPATH  from RETAILSTORETABLE where STORENUMBER='" + ApplicationSettings.Database.StoreID + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();

            if (!string.IsNullOrEmpty(sResult))
                return sResult.Trim();
            else
                return "-";

        }

        private void NIM_SaveOrderSampleSketch()
        {
            string sArchivePaths = string.Empty;
            for (int sketchLine = 0; sketchLine < dtSampleSketch.Rows.Count; sketchLine++)
            {
                if (!string.IsNullOrEmpty(Convert.ToString(dtSampleSketch.Rows[sketchLine]["SKETCH"])))
                {
                    sArchivePaths = GetArchivePathFromImage();
                    string path = sArchivePaths + "" + txtOrderNo.Text + "_" + Convert.ToInt64(dtSampleSketch.Rows[sketchLine]["LINENUM"]) + "_S" + ".jpeg"; //

                    Bitmap b = new Bitmap(Convert.ToString(dtSampleSketch.Rows[sketchLine]["SKETCH"]));
                    b.Save(Convert.ToString(path));
                }
            }

        }
    }
}
