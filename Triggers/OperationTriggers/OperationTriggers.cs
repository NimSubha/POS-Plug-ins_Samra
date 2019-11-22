using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Triggers;
using System;
using System.Data.SqlClient;
using LSRetailPosis.Settings;
using System.Data;
using Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch;
using LSRetailPosis.Transaction;
using LSRetailPosis.Transaction.Line.SaleItem;

namespace Microsoft.Dynamics.Retail.Pos.OperationTriggers
{
    [Export(typeof(IOperationTrigger))]
    public class OperationTriggers:IOperationTrigger
    {
        #region Constructor - Destructor


        public OperationTriggers()
        {

            // Get all text through the Translation function in the ApplicationLocalizer
            // TextID's for InfocodeTriggers are reserved at 59000 - 59999
        }

        #endregion

        #region IOperationTriggersV1 Members

        /// <summary>
        /// Before the operation is processed this trigger is called.
        /// </summary>
        /// <param name="preTriggerResult"></param>
        /// <param name="posTransaction"></param>
        /// <param name="posisOperation"></param>
        public void PreProcessOperation(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction, PosisOperations posisOperation)
        {

            LSRetailPosis.ApplicationLog.Log("ICustomerTriggersV1.PreProcessOperation", "Before the operation is processed this trigger is called.", LSRetailPosis.LogTraceLevel.Trace);
        }

        /// <summary>
        /// After the operation has been processed this trigger is called.
        /// </summary>
        /// <param name="posTransaction"></param>
        /// <param name="posisOperation"></param>
        /// 

        private IApplication application;

        [Import]
        public IApplication Application
        {
            get
            {
                return this.application;
            }
            set
            {
                this.application = value;

            }
        }

        public void PostProcessOperation(IPosTransaction posTransaction, PosisOperations posisOperation)
        {
            if(posisOperation == PosisOperations.CustomerAccountDeposit)
            {
                LSRetailPosis.Transaction.CustomerPaymentTransaction custTrans = posTransaction as LSRetailPosis.Transaction.CustomerPaymentTransaction;
                if(custTrans != null && custTrans.Amount != 0)
                {
                    custTrans.PartnerData.EFTCardNo = string.Empty;

                    if(!string.IsNullOrEmpty(Convert.ToString(custTrans.Customer.CustomerId)))
                    {
                        bool IsRepair = false;
                        string sCustOrder = OrderNum(out IsRepair);

                        //string sCustOrder = OrderNum();
                        if(!string.IsNullOrEmpty(sCustOrder))
                        {

                            decimal dFixedRatePercentage = 0m;
                            decimal dCustOrderTotalAmt = 0m;

                            if(!IsRepair)
                                dFixedRatePercentage = GetCustOrderFixedRateInfo(sCustOrder, ref dCustOrderTotalAmt);


                            SqlConnection connection = new SqlConnection();

                            if(application != null)
                                connection = application.Settings.Database.Connection;
                            else
                                connection = ApplicationSettings.Database.LocalConnection;

                            Enums.EnumClass oEnum = new Enums.EnumClass();
                            string sMaxAmount = string.Empty;
                            string sTerminalID = ApplicationSettings.Terminal.TerminalId;
                            string sMinAmt = Convert.ToString(oEnum.ValidateMinDeposit(connection, out sMaxAmount, sTerminalID, dCustOrderTotalAmt));

                            (((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction))).CustomerDepositItem.Comment = "INFORMATION : MINIMUM DEPOSIT AMOUNT IS " + decimal.Round(Convert.ToDecimal(sMinAmt), 2, MidpointRounding.AwayFromZero) + " " +
                                                                                                                                     "AND MAXIMUM DEPOSIT AMOUNT IS " + decimal.Round(Convert.ToDecimal(sMaxAmount), 2, MidpointRounding.AwayFromZero);



                            ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedRateCustOrdNo = sCustOrder;
                            ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedRateCustOrdPercentage = dFixedRatePercentage;
                            ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedRateCustOrdTotAmt = dCustOrderTotalAmt;

                            ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.GSSNum = string.Empty;
                            ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.NumMonths = string.Empty;
                            ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OperationType = Convert.ToString("NORMALCUSTOMERDEPOSIT");
                            ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OperationDone = Convert.ToBoolean(false);
                            ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OrderNo = sCustOrder;
                            ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.SKUData = string.Empty;
                            ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.ItemIds = string.Empty;
                            ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.IsRepair = IsRepair;
                        }
                        else
                        {
                            #region Comment
                            //frmOptionSelectionGSSorCustomerOrder optionSelection = new frmOptionSelectionGSSorCustomerOrder();
                            //optionSelection.ShowDialog();
                            //SqlConnection connection = new SqlConnection();

                            //if (application != null)
                            //    connection = application.Settings.Database.Connection;
                            //else
                            //    connection = ApplicationSettings.Database.LocalConnection;


                            //if (connection.State == ConnectionState.Closed)
                            //{
                            //    connection.Open();
                            //}

                            //if (optionSelection.isGSS)
                            //{
                            //    string commandText = string.Empty;

                            //    commandText = " SELECT   GSSACCOUNTOPENINGPOSTED.GSSACCOUNTNO AS [GSSACCOUNTNO.], " +
                            //                  " DIRPARTYTABLE.NAMEALIAS AS [CUSTOMERNAME],  " +
                            //                  "  CAST(GSSACCOUNTOPENINGPOSTED.INSTALLMENTAMOUNT AS NUMERIC(28,2)) AS [INSTALLMENTAMOUNT], " +
                            //                  " ( CASE  WHEN  GSSACCOUNTOPENINGPOSTED.SCHEMETYPE=0 THEN 'FIXED' ELSE 'FLEXIBLE' END) AS [SCHEMETYPE], " +
                            //                  " GSSACCOUNTOPENINGPOSTED.SCHEMECODE AS [SCHEMECODE], " +
                            //                  " ( CASE WHEN GSSACCOUNTOPENINGPOSTED.SCHEMEDEPOSITTYPE=0 THEN 'GOLD' ELSE 'AMOUNT' END) AS [DEPOSITTYPE],   " +
                            //                  " GSSACCOUNTOPENINGPOSTED.GSSConfirm AS [GSSCONFIRM] " +
                            //                  " FROM         DIRPARTYTABLE INNER JOIN " +
                            //                  " CUSTTABLE ON DIRPARTYTABLE.RECID = CUSTTABLE.PARTY INNER JOIN " +
                            //                  " GSSACCOUNTOPENINGPOSTED ON CUSTTABLE.ACCOUNTNUM = GSSACCOUNTOPENINGPOSTED.CUSTACCOUNT " +
                            //                  " WHERE GSSACCOUNTOPENINGPOSTED.CUSTACCOUNT = '" + custTrans.Customer.CustomerId + "'";

                            //    if (connection.State == ConnectionState.Closed)
                            //        connection.Open();


                            //    SqlCommand cmd = new SqlCommand(commandText, connection);
                            //    SqlDataReader rdr = cmd.ExecuteReader();
                            //    DataTable dtGSS = new DataTable();
                            //    dtGSS.Load(rdr);



                            //    BlankOperations.WinFormsTouch.frmGSSInput oGSS = new frmGSSInput(dtGSS, (((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction))).CustomerDepositItem.Amount);
                            //    oGSS.ShowDialog();

                            //    rdr.Dispose();

                            //    if (oGSS.bStatus)
                            //    {
                            //        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.GSSNum = Convert.ToString(oGSS.GSSnumber);
                            //        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.NumMonths = Convert.ToString(oGSS.months);
                            //        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OperationType = Convert.ToString("GSS");

                            //        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OperationDone = Convert.ToBoolean(true);
                            //        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.GoldFixing = oGSS.GoldFixing;
                            //        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).Amount = Convert.ToDecimal(oGSS.Amount);
                            //        (((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction))).CustomerDepositItem.Amount = Convert.ToDecimal(oGSS.Amount);
                            //        (((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction))).TransSalePmtDiff = Convert.ToDecimal(oGSS.Amount);

                            //        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedRateCustOrdNo = string.Empty;

                            //    }
                            //    else
                            //    {
                            //        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.GSSNum = string.Empty;
                            //        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.NumMonths = string.Empty;
                            //        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OperationType = Convert.ToString("NORMALCUSTOMERDEPOSIT");

                            //        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OperationDone = Convert.ToBoolean(false);

                            //        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedRateCustOrdNo = string.Empty;
                            //        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedRateCustOrdPercentage = 0;
                            //        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedRateCustOrdTotAmt = 0;


                            //    }
                            //}
                            #endregion

                            //else
                            //{
                            ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.GSSNum = string.Empty;
                            ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.NumMonths = string.Empty;
                            ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OperationType = Convert.ToString("NORMALCUSTOMERDEPOSIT");

                            ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OperationDone = Convert.ToBoolean(false);

                            ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedRateCustOrdNo = string.Empty;
                            ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedRateCustOrdPercentage = 0;
                            ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedRateCustOrdTotAmt = 0;
                            ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.ItemIds = string.Empty;
                            ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.IsRepair = false;

                            //}
                        }
                    }
                }
                else
                {
                    UpdateRetailTempTable();

                }
            }

            LSRetailPosis.ApplicationLog.Log("IOperationTriggersV1.PostProcessOperation", "After the operation has been processed this trigger is called.", LSRetailPosis.LogTraceLevel.Trace);
        }



        #endregion

        #region RETURN ORDER NUM
        private string OrderNum(out bool isRepair)
        {
            isRepair = false;
            SqlConnection connection = new SqlConnection();

            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;


            if(connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            // string commandText = " SELECT CUSTORDER FROM RETAILTEMPTABLE WHERE ID=1 ";
            string commandText = " SELECT CUSTORDER,CUSTID FROM RETAILTEMPTABLE WHERE ID=1 AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' "; // RETAILTEMPTABLE
            SqlCommand command = new SqlCommand(commandText, connection);
            SqlDataReader reader = command.ExecuteReader();
            string strCustOrder = string.Empty;
            if(reader.HasRows)
            {
                while(reader.Read())
                {
                    strCustOrder = Convert.ToString(reader.GetValue(reader.GetOrdinal("CUSTORDER")));
                    //string sIsRepair = strCustOrder = Convert.ToString(reader.GetValue(reader.GetOrdinal("CUSTID")));
                    string sIsRepair = Convert.ToString(reader.GetValue(reader.GetOrdinal("CUSTID")));
                    if(sIsRepair == "REPAIR")
                    {
                        isRepair = true;
                    }
                }
            }
            connection.Close();
            return strCustOrder;

        }
        #endregion

        #region UPDATE RETAILTEMPTABLE
        private void UpdateRetailTempTable()
        {
            SqlConnection connection = new SqlConnection();

            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;


            if(connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }


            // string commandText = " UPDATE RETAILTEMPTABLE SET TRANSID=NULL,CUSTORDER=NULL,GOLDFIXING=NULL,MINIMUMDEPOSITFORCUSTORDER=NULL WHERE ID=1 ";
            //Start : 05/06/2014
            string commandText = " UPDATE SKUTableTrans SET isLocked='False',isAvailable = 'TRUE' WHERE SkuNumber" +
                            " IN (select b.ITEMID from CUSTORDER_HEADER" +
                            " a left join CUSTORDER_DETAILS b on a.ORDERNUM =b.ORDERNUM " +
                            " where a.IsConfirmed=0 and b.IsBookedSKU=1 and" +
                            " a.ORDERNUM=(select CUSTORDER  from  RETAILTEMPTABLE" +
                            " WHERE ID=1 AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "'))" +
                //End : 05/06/2014
                            " UPDATE RETAILTEMPTABLE SET TRANSID=NULL,CUSTORDER=NULL,GOLDFIXING=NULL," +
                            " MINIMUMDEPOSITFORCUSTORDER=NULL WHERE ID=1 AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' "; // RETAILTEMPTABLE
            SqlCommand command = new SqlCommand(commandText, connection);
            command.ExecuteNonQuery();
            connection.Close();



        }
        #endregion

        private decimal GetCustOrderFixedRateInfo(string sOrderNo, ref decimal dCOTotAmt)  // Fixed Metal Rate New
        {
            decimal dFixRatePct = 0M;
            SqlConnection connection = new SqlConnection();

            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;


            if(connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string commandText = " DECLARE @FIXEDRATEVAL AS NUMERIC(32,3)" +
                                 " SELECT @FIXEDRATEVAL = ISNULL(FIXEDRATEADVANCEPCT,0) FROM RETAILPARAMETERS WHERE DATAAREAID = '" + application.Settings.Database.DataAreaID + "'" +
                                 " SELECT @FIXEDRATEVAL AS FIXEDRATEPCT,ISNULL(TOTALAMOUNT,0) AS TOTALAMOUNT FROM CUSTORDER_HEADER WHERE ORDERNUM = '" + sOrderNo + "' "
                                 ;

            SqlCommand command = new SqlCommand(commandText, connection);
            // string strCustOrder = Convert.ToString(command.ExecuteScalar());
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();

            if(reader.HasRows)
            {
                while(reader.Read())
                {
                    dFixRatePct = Convert.ToDecimal(reader.GetValue(0));
                    dCOTotAmt = Convert.ToDecimal(reader.GetValue(1));
                }
            }

            if(connection.State == ConnectionState.Open)
                connection.Close();

            return dFixRatePct;
        }
    }


}
