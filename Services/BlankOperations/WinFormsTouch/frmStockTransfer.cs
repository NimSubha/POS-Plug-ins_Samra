using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Dynamics.Retail.Pos.Dialog;
using LSRetailPosis.Transaction.Line.SaleItem;
using Microsoft.Dynamics.Retail.Pos.Item;
using Microsoft.Dynamics.Retail.Pos.Dimension;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch;
using Microsoft.Dynamics.Retail.Pos.RoundingService;
using System.Data.SqlClient;
using LSRetailPosis.Settings;
using Microsoft.Dynamics.Retail.Notification.Contracts;
using Microsoft.Dynamics.Retail.Pos.Interaction;
using LSRetailPosis.DataAccess.DataUtil;
using BlankOperations;
using Microsoft.Dynamics.Retail.Pos.ApplicationService;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmStockTransfer : frmTouchBase
    {
        /// <summary>
        /// variable declear
        /// </summary>
        #region Variable
        SqlConnection conn = new SqlConnection();
        public IPosTransaction pos { get; set; }

        [Import]
        private IApplication application;
        DataTable skuItem = new DataTable();
        DataTable dtSku = new DataTable();

        #region enum  TransactionType
        /// <summary>
        /// </summary>
        enum TransactionType
        {
            Inter = 0,
            StockIn = 1,
            StockOut = 2
        }
        #endregion

        #endregion

        #region Event
        /// <summary>
        /// DEV BY RIPAN HOSSAIN ON 06032013-- CLOSE WINDOW
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
        private void btnCommit_Click(object sender, EventArgs e)
        {
            if (ValidateControls())
            {
                string sqlUpd = string.Empty;
                conn = application.Settings.Database.Connection;
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                int TransType;
                TransType = cmbTransferType.SelectedIndex;

                SaveStckTransfer();

                #region Update
                if (skuItem != null && skuItem.Rows.Count > 0 && grdView.RowCount > 0)
                {
                    foreach (DataRow drTotal in skuItem.Rows)
                    {
                        sqlUpd = " UPDATE SKUTableTrans "; 
                        switch (TransType)
                        {
                            case 0: //FOR INTER TRANSFER
                                sqlUpd = sqlUpd + " SET isAvailable = 1";
                                sqlUpd = sqlUpd + " ,TOCOUNTER = '" + Convert.ToString(txtToCounter.Text) + "'";
                                sqlUpd = sqlUpd + " ,FROMCOUNTER = '" + Convert.ToString(txtFromCounter.Text) + "' WHERE SKUNUMBER = '" + Convert.ToString(drTotal["SKUNUMBER"]) + "'";
                                break;
                            case 1://FOR STOCKIN TRANSFER

                                string sToCounter = txtToCounter.Text.Trim();
                                string sStoreId = ApplicationSettings.Terminal.StoreId;

                                sqlUpd = " IF EXISTS (SELECT SKUNUMBER FROM SKUTableTrans WHERE SKUNUMBER = '" + Convert.ToString(drTotal["SKUNUMBER"]) + "')" +
                                         " BEGIN UPDATE SKUTableTrans SET isAvailable = 1, TOCOUNTER = '" + sToCounter + "' WHERE SKUNUMBER = '" + Convert.ToString(drTotal["SKUNUMBER"]) + "' END ELSE BEGIN " +
                                         " INSERT INTO SKUTableTrans (SkuDate,SkuNumber,DATAAREAID,CREATEDON" +
                                         " ,isLocked,isAvailable,PDSCWQTY,QTY,INGREDIENT,FROMCOUNTER,TOCOUNTER,ECORESCONFIGURATIONNAME,INVENTCOLORID,INVENTSIZEID,RETAILSTOREID)" +
                                         " SELECT SkuDate,SkuNumber,DATAAREAID,CREATEDON,isLocked," +
                                         " 1,PDSCWQTY,QTY,INGREDIENT,FROMCOUNTER,'" + sToCounter + "',ECORESCONFIGURATIONNAME,INVENTCOLORID,INVENTSIZEID,'" + sStoreId + "' FROM SKUTable_Posted" +
                                         " WHERE SKUNUMBER = '" + Convert.ToString(drTotal["SKUNUMBER"]) + "' END"
                                        ;
                                break;
                            case 2://FOR STOCKOUT TRANSFER
                                sqlUpd = sqlUpd + " SET isAvailable = 0";
                                sqlUpd = sqlUpd + " ,FROMCOUNTER = '" + Convert.ToString(txtFromCounter.Text) + "' WHERE SKUNUMBER = '" + Convert.ToString(drTotal["SKUNUMBER"]) + "'";
                                break;
                        }

                        DBUtil dbUtil = new DBUtil(conn);
                        dbUtil.Execute(sqlUpd);
                    }

                    ClearControls();
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                }
                #endregion

                else
                {
                    ShowMessage("Atleast one item should be select.");
                    txtToCounter.Focus();
                }

            }
        }

        /// <summary>
        ///  DEV BY RIPAN HOSSAIN ON 06032013-- from counter searching
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFromCounterSearch_Click(object sender, EventArgs e)
        {
            DataTable dtCounter = new DataTable();
            DataRow drCounter = null;
            conn = application.Settings.Database.Connection;
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            string commandText = string.Empty;
            if (string.IsNullOrEmpty(txtToCounter.Text))
                commandText = "SELECT A.COUNTERCODE AS CODE,B.COUNTERDESC AS DESCRIPTION FROM RETAILSTORECOUNTERTABLE A" +
                              " INNER JOIN COUNTERMASTER B ON A.COUNTERCODE = B.COUNTERCODE WHERE A.RETAILSTOREID = '" + ApplicationSettings.Terminal.StoreId.Trim() + "' ";
            else
                commandText = "SELECT A.COUNTERCODE AS CODE,B.COUNTERDESC AS DESCRIPTION FROM RETAILSTORECOUNTERTABLE A" +
                              " INNER JOIN COUNTERMASTER B ON A.COUNTERCODE = B.COUNTERCODE" +
                              " WHERE A.RETAILSTOREID = '" + ApplicationSettings.Terminal.StoreId.Trim() + "' AND A.COUNTERCODE <> '" + Convert.ToString(txtToCounter.Text) + "'";

            SqlCommand command = new SqlCommand(commandText, conn);
            command.CommandTimeout = 0;
            SqlDataAdapter adapter = new SqlDataAdapter(commandText, conn);

            adapter.Fill(dtCounter);

            if (conn.State == ConnectionState.Open)
                conn.Close();

            Dialog.WinFormsTouch.frmGenericSearch oSearch = new Dialog.WinFormsTouch.frmGenericSearch(dtCounter, drCounter = null, "Counter Search");
            oSearch.ShowDialog();
            drCounter = oSearch.SelectedDataRow;
            if (drCounter != null)
            {
                txtFromCounter.Text = string.Empty;
                txtFromCounter.Text = Convert.ToString(drCounter["CODE"]);
            }
        }

        /// <summary>
        ///  DEV BY RIPAN HOSSAIN ON 06032013 -- to counter searching
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnToCounterSearch_Click(object sender, EventArgs e)
        {
            DataTable dtCounter = new DataTable();
            DataRow drCounter = null;
            SqlConnection conn = new SqlConnection();
            conn = application.Settings.Database.Connection;
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            string commandText = string.Empty;
            if (string.IsNullOrEmpty(txtFromCounter.Text))
                commandText = "SELECT A.COUNTERCODE AS CODE,B.COUNTERDESC AS DESCRIPTION FROM RETAILSTORECOUNTERTABLE A" +
                              " INNER JOIN COUNTERMASTER B ON A.COUNTERCODE = B.COUNTERCODE WHERE A.RETAILSTOREID = '" + ApplicationSettings.Terminal.StoreId.Trim() + "' ";
            else
                commandText = "SELECT A.COUNTERCODE AS CODE,B.COUNTERDESC AS DESCRIPTION FROM RETAILSTORECOUNTERTABLE A" +
                              " INNER JOIN COUNTERMASTER B ON A.COUNTERCODE = B.COUNTERCODE" +
                              " WHERE A.RETAILSTOREID = '" + ApplicationSettings.Terminal.StoreId.Trim() + "' AND A.COUNTERCODE <> '" + Convert.ToString(txtFromCounter.Text) + "'";


            SqlCommand command = new SqlCommand(commandText, conn);
            command.CommandTimeout = 0;
            SqlDataAdapter adapter = new SqlDataAdapter(commandText, conn);
            adapter.Fill(dtCounter);

            if (conn.State == ConnectionState.Open)
                conn.Close();

            Dialog.WinFormsTouch.frmGenericSearch oSearch = new Dialog.WinFormsTouch.frmGenericSearch(dtCounter, drCounter = null, "Counter Search");
            oSearch.ShowDialog();
            drCounter = oSearch.SelectedDataRow;
            if (drCounter != null)
            {
                txtToCounter.Text = string.Empty;
                txtToCounter.Text = Convert.ToString(drCounter["CODE"]);
            }
        }

        /// <summary>
        ///  DEV BY RIPAN HOSSAIN ON 06032013 --  pos skuproduct(item) search here 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPOSItemSearch_Click(object sender, EventArgs e)
        {
            DataTable dtProduct = new DataTable();
            DataRow drProduct = null;
            SqlConnection conn = new SqlConnection();
            conn = application.Settings.Database.Connection;
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            string commandText = string.Empty;
            if (cmbTransferType.Text.ToUpper().Trim() == "STOCKIN")
                commandText = "SELECT CAST(SKUNUMBER AS NVARCHAR (30)) AS SKUNUMBER,CAST(CAST(QTY AS NUMERIC(28,3)) AS NVARCHAR (50)) AS QTY" +
                              " FROM SKUTable_Posted WHERE SKUNUMBER NOT IN (SELECT SKUNUMBER FROM SKUTableTrans) UNION " +
                              " SELECT CAST(SKUNUMBER AS NVARCHAR (30)) AS SKUNUMBER,CAST(CAST(QTY AS NUMERIC(28,3)) AS NVARCHAR (50)) AS QTY" +
                              " FROM SKUTableTrans WHERE ISAVAILABLE=0"
                              ;
            else if (cmbTransferType.Text.ToUpper().Trim() == "INTER")
            {
                if (string.IsNullOrEmpty(txtFromCounter.Text))
                {
                    MessageBox.Show("Please select From Counter");
                    return;
                }
                else
                {
                   commandText = "SELECT CAST(SKUNUMBER AS NVARCHAR (30)) AS SKUNUMBER, CAST(CAST(QTY AS NUMERIC(28,3)) AS NVARCHAR (50)) AS QTY FROM SKUTableTrans WHERE ISAVAILABLE=1 AND TOCOUNTER = '"+ txtFromCounter.Text +"'";
                }
            }

            else
                commandText = "SELECT CAST(SKUNUMBER AS NVARCHAR (30)) AS SKUNUMBER, CAST(CAST(QTY AS NUMERIC(28,3)) AS NVARCHAR (50)) AS QTY FROM SKUTableTrans WHERE ISAVAILABLE=1";



            SqlCommand command = new SqlCommand(commandText, conn);
            command.CommandTimeout = 0;
            SqlDataAdapter adapter = new SqlDataAdapter(commandText, conn);
            adapter.Fill(dtProduct);

            if (conn.State == ConnectionState.Open)
                conn.Close();

            Dialog.WinFormsTouch.frmGenericSearch oSearch = new Dialog.WinFormsTouch.frmGenericSearch(dtProduct, drProduct = null, "Product Search");
            oSearch.ShowDialog();
            drProduct = oSearch.SelectedDataRow;

            if (drProduct != null)
            {
                txtItemId.Text = string.Empty;
                txtItemId.Text = Convert.ToString(drProduct["SKUNUMBER"]);

                getSkuDetails(Convert.ToString(txtItemId.Text));

            }
        }

        /// <summary>
        ///  DEV BY RIPAN HOSSAIN ON 06032013 -- void control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearControls();
        }

        /// <summary>
        ///  DEV BY RIPAN HOSSAIN ON 06032013 -- skuproduct control blank
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClearProduct_Click(object sender, EventArgs e)
        {
            txtItemId.Text = "";
            txtItemId.Focus();
        }

        /// <summary>
        /// DEV BY RIPAN HOSSAIN ON 06032013 --from counter tesxt clear
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFromCounterClear_Click(object sender, EventArgs e)
        {
            txtFromCounter.Text = string.Empty;
            txtFromCounter.Focus();
        }

        /// <summary>
        /// DEV BY RIPAN HOSSAIN ON 06032013 --to counter tesxt clear
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnToCounterClear_Click(object sender, EventArgs e)
        {
            txtToCounter.Text = string.Empty;
            txtToCounter.Focus();
        }

        /// <summary>
        /// DEV BY RIPAN HOSSAIN ON 06032013 --txtItemId enter event 
        /// same as Product search 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtItemId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                conn = application.Settings.Database.Connection;

                if (isValiedSku(conn, Convert.ToString(txtItemId.Text)))
                {
                    getSkuDetails(Convert.ToString(txtItemId.Text));

                    txtItemId.Text = string.Empty; // added on 17.08.2013
                }
                else
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please enter valid SKU.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {
                        txtItemId.Text = string.Empty; // added on 17.08.2013
                        txtItemId.Focus();
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    }
                }
            }
        }

        /// <summary>
        /// DEV BY RIPAN HOSSAIN ON 07032013 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            //int DeleteSelectedIndex = 0;
            if (grdView.RowCount > 0)
            {
                DeleteSelectedRows(grdView);
            }
        }

        /// <summary>
        /// DEV BY RIPAN HOSSAIN ON 12-03-2013
        /// Purpose :: From counter / To counter enable true/false according to selection of transaction type
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbTransferType_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtItemId.Text = string.Empty;
            txtFromCounter.Text = string.Empty;
            txtToCounter.Text = string.Empty;
            gridStockTrans.DataSource = null;
            skuItem = new DataTable();
            dtSku = new DataTable();

            if (cmbTransferType.SelectedIndex > -1 && cmbTransferType.SelectedItem != null)
            {
                switch (cmbTransferType.SelectedIndex)
                {
                    case 1:
                        txtToCounter.Enabled = true;
                        btnToCounterSearch.Enabled = true;
                        btnToCounterClear.Enabled = true;
                        txtFromCounter.Text = string.Empty;
                        txtFromCounter.Enabled = false;
                        btnFromCounterSearch.Enabled = false;
                        btnFromCounterClear.Enabled = false;
                        break;
                    case 2:
                        txtFromCounter.Enabled = true;
                        btnFromCounterSearch.Enabled = true;
                        btnFromCounterClear.Enabled = true;
                        txtToCounter.Text = string.Empty;
                        txtToCounter.Enabled = false;
                        btnToCounterSearch.Enabled = false;
                        btnToCounterClear.Enabled = false;
                        break;
                    default:
                        txtFromCounter.Enabled = true;
                        btnFromCounterSearch.Enabled = true;
                        btnFromCounterClear.Enabled = true;
                        txtToCounter.Enabled = true;
                        btnToCounterSearch.Enabled = true;
                        btnToCounterClear.Enabled = true;
                        break;
                }
            }
        }
        #endregion

        #region Function

        #region ValidateControls()
        /// <summary>
        /// DEV BY RIPAN HOSSAIN ON 06032013 --CONTROLL VALIDATE
        /// message showing using common function 'ShowMessage(string)'
        /// </summary>
        /// <returns></returns>
        bool ValidateControls()
        {
            if (cmbTransferType.Text.ToUpper().Trim() == "STOCKIN")
            {
                if (string.IsNullOrEmpty(txtToCounter.Text))
                {
                    ShowMessage("To counter can not be blank and empty");
                    txtToCounter.Focus();
                    return false;
                }
            }

            if (cmbTransferType.Text.ToUpper().Trim() == "STOCKOUT")
            {
                if (string.IsNullOrEmpty(txtFromCounter.Text))
                {
                    ShowMessage("From counter can not be blank and empty");
                    txtFromCounter.Focus();
                    return false;
                }
            }


            if (cmbTransferType.Text.ToUpper().Trim() == "INTER")
            {
                if (string.IsNullOrEmpty(txtFromCounter.Text))
                {
                    ShowMessage("From counter and To counter can not be blank and empty");
                    txtFromCounter.Focus();
                    return false;
                }
                if (string.IsNullOrEmpty(txtToCounter.Text))
                {
                    ShowMessage("From counter and To counter can not be blank and empty");
                    txtToCounter.Focus();
                    return false;
                }
            }

            if (txtFromCounter.Text.ToUpper().Trim() == txtToCounter.Text.ToUpper().Trim())
            {
                ShowMessage("From counter and To counter can not be same");
                txtFromCounter.Focus();
                return false;
            }
            if (dtSku == null)
            {
                ShowMessage("Altleast one item should be transfer.");
                txtItemId.Focus();
                return false;
            }
            else
            {
                return true;
            }
        }


        #endregion

        #region ClearControls
        /// <summary>

        /// </summary>
        private void ClearControls()
        {
            txtItemId.Text = string.Empty;
            txtFromCounter.Text = string.Empty;
            cmbTransferType.SelectedIndex = 0;
            txtToCounter.Text = string.Empty;
            gridStockTrans.DataSource = null;
            skuItem = new DataTable();
            dtSku = new DataTable();

            lblTotNoOfSKU.Text = "0";
            lblTotQty.Text  = "0.000";
            lblTotSetOf.Text  = "0";
        }

        #endregion

        #region Transaction Type Combo
        /// <summary>
        /// DEV BY RIPAN HOSSAIN ON 06032013-- TRANS TYPE COMBO LOAD By ENUM
        /// </summary>
        void BindRateTypeMakingTypeCombo()
        {
            cmbTransferType.DataSource = Enum.GetValues(typeof(TransactionType));
        }
        #endregion

        /// <summary>
        /// DEV BY RIPAN HOSSAIN ON 06032013--  THIS FORM INITIATING HERE
        /// </summary>
        /// <param name="posTransaction"></param>
        /// <param name="Application"></param>
        public frmStockTransfer(IPosTransaction posTransaction, IApplication Application)
        {
            InitializeComponent();

            pos = posTransaction;
            application = Application;
            BindRateTypeMakingTypeCombo();
            btnPOSItemSearch.Focus();
            //dtpStockTransfe.Text  = DateTime.Now.Date;
        }

        /// <summary>
        /// DEV BY RIPAN HOSSAIN ON 06032013--  GET DATA BY SELECTED SKUNUMBER INTO GRID
        /// </summary>
        /// <param name="_productSku"></param>
        /// <returns></returns>
        private void getSkuDetails(string _productSku)
        {
            //Start : added on 26/05/2014
            int iNoSku = 0;
            int iNoOfSetOf = 0;
            decimal dTotQty = 0;
            //End : added on 26/05/2014
            

            conn = application.Settings.Database.Connection;
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            string commandText = string.Empty;
            commandText = " SELECT SKUNUMBER,isnull(I.SetOf,0) as SetOf,CONVERT(DECIMAL(10,3),QTY) as QUANTITY " +
                           " FROM SKUTable_Posted " +
                           " LEFT JOIN INVENTTABLE I ON SKUTable_Posted.SKUNUMBER = I.ITEMID " +
                           " WHERE SKUNUMBER='" + _productSku + "'";

            SqlCommand command = new SqlCommand(commandText, conn);
            command.CommandTimeout = 0;
            SqlDataAdapter adapter = new SqlDataAdapter(commandText, conn);
            adapter.Fill(dtSku);

            if (conn.State == ConnectionState.Open)
                conn.Close();

            if (dtSku != null)
            {
                string s = _productSku;
                DataColumn[] columns = new DataColumn[1];
                columns[0] = dtSku.Columns["SKUNUMBER"];
                dtSku.PrimaryKey = columns; 

                skuItem = dtSku;
                gridStockTrans.DataSource = dtSku.DefaultView;

                //Start : added on 26/05/2014
                foreach (DataRow dr in dtSku.Rows) 
                {
                    iNoSku = iNoSku + 1;
                    iNoOfSetOf = iNoOfSetOf  + Convert.ToInt32(dr[1]);
                    dTotQty = dTotQty + Convert.ToDecimal(dr[2]);
                }

                lblTotNoOfSKU.Text = Convert.ToString(iNoSku);
                lblTotSetOf.Text = Convert.ToString(iNoOfSetOf);
                lblTotQty.Text = Convert.ToString(dTotQty);
                //End : added on 26/05/2014
            }
        }

        /// <summary>
        /// DEV BY RIPAN HOSSAIN ON 07032013-- at entry time validating skunumber
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="_productSku"></param>
        /// <returns></returns>
        public bool isValiedSku(SqlConnection conn, string _productSku)
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();

           // string commandText = " SELECT SKUNUMBER FROM SKUTable_Posted WHERE SKUNUMBER='" + _productSku + "'"; // blocked on 17.08.2013
            //17.08.2013
            string commandText = string.Empty;
            if (cmbTransferType.Text.ToUpper().Trim() == "STOCKIN")
            {
                commandText = "SELECT CAST(SKUNUMBER AS NVARCHAR (30)) AS SKUNUMBER" +
                              " FROM SKUTable_Posted WHERE SKUNUMBER NOT IN (SELECT SKUNUMBER FROM SKUTableTrans) AND SKUNUMBER='" + _productSku + "'" +
                              " UNION SELECT CAST(SKUNUMBER AS NVARCHAR (30)) AS SKUNUMBER" +
                              " FROM SKUTableTrans WHERE ISAVAILABLE=0 AND SKUNUMBER='" + _productSku + "'";
            }
            else if (cmbTransferType.Text.ToUpper().Trim() == "INTER")
            {
                if (string.IsNullOrEmpty(txtFromCounter.Text))
                {
                    MessageBox.Show("Please select From Counter");
                    return false;
                }
                else
                {
                    commandText = "SELECT CAST(SKUNUMBER AS NVARCHAR (30)) AS SKUNUMBER, CAST(CAST(QTY AS NUMERIC(28,3)) AS NVARCHAR (50)) AS QTY FROM SKUTableTrans WHERE ISAVAILABLE=1 AND TOCOUNTER = '" + txtFromCounter.Text + "' AND SKUNUMBER ='" + _productSku + "'";
                }
            }
            else
            {
                commandText = "SELECT CAST(SKUNUMBER AS NVARCHAR (30)) AS SKUNUMBER FROM SKUTableTrans WHERE ISAVAILABLE=1 AND SKUNUMBER ='" + _productSku + "' ";
            }

            // ------

            SqlCommand command = new SqlCommand(commandText, conn);
            command.CommandTimeout = 0;

            string sku = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            if (sku != string.Empty)
                return true;
            else
                return false;

        }

        #region common validation messege show
        /// <summary>
        /// DEV BY RIPAN HOSSAIN ON 12-03-2013
        /// Purpose :: to show validation message commonly
        /// </summary>
        /// <param name="_msg"></param>
        /// <returns></returns>
        public string ShowMessage(string _msg)
        {
            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage(_msg, MessageBoxButtons.OK, MessageBoxIcon.Information))
            {
                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                return _msg;
            }
        }
        #endregion

        #region SaveFuction
        /// <summary>
        /// DEV BY RIPAN HOSSAIN ON 14-03-2013
        /// </summary>
        private void SaveStckTransfer()
        {
            int iStockTransfer_Header = 0;
            int iStockTransfer_Details = 0;
            string TransferId = GetStockTransId();
            SqlTransaction transaction = null;

            //MODIFIED DATE :: 18/03/2013 ; MODIFIED BY : RIPAN HOSSAIN
            #region STOCK TRANSFER HEADER
            string commandText = " INSERT INTO [SKUTransfer_Header]([STOCKTRANSFERID],[STOCKTRANSFERDATE]," +
                                 " [STOCKTRANSFERTYPE],[FROMCOUNTER],[TOCOUNTER],[RETAILSTOREID],[RETAILTERMINALID],[RETAILSTAFFID]," +
                                 " [DATAAREAID],[CREATEDON])" +
                                 " VALUES(@STOCKTRANSFERID,@STOCKTRANSFERDATE,@STOCKTRANSFERTYPE,@FROMCOUNTER,@TOCOUNTER," +
                                 " @RETAILSTOREID,@RETAILTERMINALID,@RETAILSTAFFID," +
                                 " @DATAAREAID,@CREATEDON)";
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
                command.Parameters.Add("@STOCKTRANSFERID", SqlDbType.NVarChar).Value = TransferId.Trim();
                command.Parameters.Add("@STOCKTRANSFERDATE", SqlDbType.DateTime).Value = dtpStockTransfe.Value;
                command.Parameters.Add("@STOCKTRANSFERTYPE", SqlDbType.NVarChar, 10).Value = cmbTransferType.Text;
                command.Parameters.Add("@FROMCOUNTER", SqlDbType.NVarChar, 20).Value = Convert.ToString(txtFromCounter.Text);
                command.Parameters.Add("@TOCOUNTER", SqlDbType.NVarChar, 20).Value = Convert.ToString(txtToCounter.Text);
                command.Parameters.Add("@RETAILSTOREID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.StoreId;
                command.Parameters.Add("@RETAILTERMINALID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.TerminalId;
                command.Parameters.Add("@RETAILSTAFFID", SqlDbType.NVarChar, 10).Value = pos.OperatorId;
                if (application != null)
                    command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = application.Settings.Database.DataAreaID;
                else
                    command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = ApplicationSettings.Database.DATAAREAID;

                command.Parameters.Add("@CREATEDON", SqlDbType.DateTime).Value = DateTime.Today.ToShortDateString();
            #endregion

                command.CommandTimeout = 0;
                iStockTransfer_Header = command.ExecuteNonQuery();

                if (iStockTransfer_Header == 1)
                {
                    if (skuItem != null && skuItem.Rows.Count > 0)
                    {
                        #region ORDER DETAILS
                        //MODIFIED DATE :: 18/03/2013 ; MODIFIED BY : RIPAN HOSSAIN
                        string commandCustOrder_Detail = " INSERT INTO [SKUTRANSFER_DETAILS]([STOCKTRANSFERID],[LINENUMBER],[SKUNUMBER],[QTY]," +
                                                         " [RETAILSTOREID],[RETAILTERMINALID],[RETAILSTAFFID],[DATAAREAID])" +
                                                         " VALUES(@STOCKTRANSFERID  ,@LINENUMBER , @SKUNUMBER," +
                                                         " @QTY,@RETAILSTOREID,@RETAILTERMINALID,@RETAILSTAFFID,@DATAAREAID) ";

                        for (int ItemCount = 0; ItemCount < skuItem.Rows.Count; ItemCount++)
                        {

                            SqlCommand cmdStcokTransfer_Detail = new SqlCommand(commandCustOrder_Detail, connection, transaction);
                            cmdStcokTransfer_Detail.Parameters.Add("@STOCKTRANSFERID", SqlDbType.NVarChar, 20).Value = TransferId.Trim();
                            cmdStcokTransfer_Detail.Parameters.Add("@LINENUMBER", SqlDbType.Int, 10).Value = ItemCount + 1;

                            if (string.IsNullOrEmpty(Convert.ToString(skuItem.Rows[ItemCount]["SKUNUMBER"])))
                                cmdStcokTransfer_Detail.Parameters.Add("@SKUNUMBER", SqlDbType.NVarChar, 20).Value = DBNull.Value;
                            else
                                cmdStcokTransfer_Detail.Parameters.Add("@SKUNUMBER", SqlDbType.NVarChar, 20).Value = Convert.ToString(skuItem.Rows[ItemCount]["SKUNUMBER"]);

                            if (string.IsNullOrEmpty(Convert.ToString(skuItem.Rows[ItemCount]["QUANTITY"])))
                                cmdStcokTransfer_Detail.Parameters.Add("@QTY", SqlDbType.Decimal).Value = DBNull.Value;
                            else
                                cmdStcokTransfer_Detail.Parameters.Add("@QTY", SqlDbType.Decimal).Value = Convert.ToDecimal(skuItem.Rows[ItemCount]["QUANTITY"]);

                            cmdStcokTransfer_Detail.Parameters.Add("@RETAILSTOREID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.StoreId;
                            cmdStcokTransfer_Detail.Parameters.Add("@RETAILTERMINALID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.TerminalId;
                            cmdStcokTransfer_Detail.Parameters.Add("@RETAILSTAFFID", SqlDbType.NVarChar, 10).Value = pos.OperatorId;
                            cmdStcokTransfer_Detail.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = ApplicationSettings.Database.DATAAREAID;

                            cmdStcokTransfer_Detail.CommandTimeout = 0;
                            iStockTransfer_Details = cmdStcokTransfer_Detail.ExecuteNonQuery();
                            cmdStcokTransfer_Detail.Dispose();
                        #endregion
                        }
                    }
                }
                transaction.Commit();
                command.Dispose();
                transaction.Dispose();
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

        #region GetStockTransId()
        /// <summary>
        ///  DEV BY RIPAN HOSSAIN ON 14-03-2013
        /// </summary>
        /// <returns></returns>
        public string GetStockTransId()
        {
            string TransId = string.Empty;
            TransId = GetNextStockTransferTransactionId();
            return TransId;
        }
        #endregion

        #region - CHANGED BY NIMBUS TO GET THE ORDER ID

        enum StockTransactionId
        {
            StockTransaction = 11
        }

        public string GetNextStockTransferTransactionId()
        {
            try
            {
                StockTransactionId transType = StockTransactionId.StockTransaction;
                string storeId = LSRetailPosis.Settings.ApplicationSettings.Terminal.StoreId;
                string terminalId = LSRetailPosis.Settings.ApplicationSettings.Terminal.TerminalId;
                string staffId = pos.OperatorId;
                string mask;

                string funcProfileId = LSRetailPosis.Settings.FunctionalityProfiles.Functions.ProfileId;
                transactionNumber((int)transType, funcProfileId, out mask);
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

        #region GetTransferTransactionId()  - CHANGED BY NIMBUS
        /// <summary>
        ///  DEV BY RIPAN HOSSAIN ON 14-03-2013
        ///  Purpose :: to get mask of transaction id
        /// </summary>
        /// <param name="transType"></param>
        /// <param name="funcProfile"></param>
        /// <param name="mask"></param>
        private void transactionNumber(int transType, string funcProfile, out string mask)
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
        /// <summary>
        ///  DEV BY RIPAN HOSSAIN ON 14-03-2013
        ///  Purpose :: get max value(numeric) from SKUTRANSFER_HEADER table to generate new transaction id
        /// </summary>
        /// <returns></returns>
        private string GetSeedVal()
        {
            string sFuncProfileId = LSRetailPosis.Settings.FunctionalityProfiles.Functions.ProfileId;
            int iTransType = (int)StockTransactionId.StockTransaction;

            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            string Val = string.Empty;
            try
            {
                string queryString = "DECLARE @VAL AS INT  SELECT @VAL = CHARINDEX('#',mask) FROM RETAILRECEIPTMASKS WHERE FUNCPROFILEID ='" + sFuncProfileId + "'  AND RECEIPTTRANSTYPE = " + iTransType + " " +
                                     " SELECT  MAX(CAST(ISNULL(SUBSTRING(STOCKTRANSFERID,@VAL,LEN(STOCKTRANSFERID)),0) AS INTEGER)) + 1 from SKUTRANSFER_HEADER";

                using (SqlCommand command = new SqlCommand(queryString, conn))
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    Val = Convert.ToString(command.ExecuteScalar());
                    if(string.IsNullOrEmpty(Val))
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

        private void DeleteSelectedRows(DevExpress.XtraGrid.Views.Grid.GridView view)
        {
            //Start : added on 26/05/2014
            int iNoSku = Convert.ToInt32(lblTotNoOfSKU.Text);
            int iNoOfSetOf = Convert.ToInt32(lblTotSetOf.Text);
            decimal dTotQty = Convert.ToDecimal(lblTotQty.Text);
            //End : added on 26/05/2014
            
            if (view == null || view.SelectedRowsCount == 0) return;

            DataRow[] rows = new DataRow[view.SelectedRowsCount];
            for (int i = 0; i < view.SelectedRowsCount; i++)
                rows[i] = view.GetDataRow(view.GetSelectedRows()[i]);
            view.BeginSort();
            try
            {
                //Start : added on 26/05/2014
                foreach (DataRow dr in rows)
                {
                    iNoSku = iNoSku - 1;
                    iNoOfSetOf = iNoOfSetOf - Convert.ToInt32(dr[1]);
                    dTotQty = dTotQty - Convert.ToDecimal(dr[2]);
                }

                lblTotNoOfSKU.Text = Convert.ToString(iNoSku);
                lblTotSetOf.Text = Convert.ToString(iNoOfSetOf);
                lblTotQty.Text = Convert.ToString(dTotQty);
                
                foreach (DataRow rn in rows)
                {
                    foreach (DataRow row in dtSku.Rows)
                    {
                        if (Convert.ToString(row["SkuNumber"]) == Convert.ToString(rn[0]))
                        {
                            row.Delete();
                            break;
                        }
                    }
                    
                }
                dtSku.AcceptChanges();
                //End : added on 26/05/2014
            }
            finally
            {
                view.EndSort();
            }
        }
        #endregion
    }
}
