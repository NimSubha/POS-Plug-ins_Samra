/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using LSRetailPosis;
using LSRetailPosis.Settings;
using LSRetailPosis.Settings.HardwareProfiles;
using LSRetailPosis.Transaction;
using LSRetailPosis.Transaction.Line.TenderItem;
using Microsoft.Dynamics.Retail.Diagnostics;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
using Microsoft.Dynamics.Retail.Pos.BlankOperations;
using System.Data.SqlClient;
using System.Data;
using LSRetailPosis.Transaction.Line.SaleItem;
using Microsoft.Dynamics.Retail.Pos.BlankOperations.Report;


namespace Microsoft.Dynamics.Retail.Pos.Printing
{
    [Export(typeof(IPrinting))]
    public class Printing : IPrinting
    {
        /// <summary>
        /// IApplication instance.
        /// </summary>
        private IApplication application;

        /// <summary>
        /// Gets or sets the IApplication instance.
        /// </summary>
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
                InternalApplication = value;
            }
        }

        /// <summary>
        /// Gets or sets the static IApplication instance.
        /// </summary>
        internal static IApplication InternalApplication { get; private set; }

        public int iPrintFromShowJournal=0;
        /// <summary>
        /// Returns true if print preview ids shown.
        /// </summary>
        /// <param name="formType"></param>
        /// <param name="posTransaction"></param>
        /// <returns></returns>
        public bool ShowPrintPreview(FormType formType, IPosTransaction posTransaction)
        {
            if (FiscalPrinter.FiscalPrinter.Instance.FiscalPrinterEnabled())
            {
                return FiscalPrinter.FiscalPrinter.Instance.ShowPrintPreview(formType, posTransaction);
            }

            FormModulation formMod = new FormModulation(Application.Settings.Database.Connection);
            RetailTransaction retailTransaction = (RetailTransaction)posTransaction;

            FormInfo formInfo = formMod.GetInfoForForm(formType, false, LSRetailPosis.Settings.HardwareProfiles.Printer.ReceiptProfileId);
            formMod.GetTransformedTransaction(formInfo, retailTransaction);
            string textForPreview = formInfo.Header;
            textForPreview += formInfo.Details;
            textForPreview += formInfo.Footer;
            textForPreview = textForPreview.Replace("|1C", string.Empty);
            textForPreview = textForPreview.Replace("|2C", string.Empty);

            ICollection<Point> signaturePoints = null;
            if (retailTransaction.TenderLines != null
                && retailTransaction.TenderLines.Count > 0
                && retailTransaction.TenderLines.First.Value != null)
            {
                signaturePoints = retailTransaction.TenderLines.First.Value.SignatureData;
            }

            using (frmReportList preview = new frmReportList(textForPreview, signaturePoints))
            {
                this.Application.ApplicationFramework.POSShowForm(preview);
                if (preview.DialogResult == DialogResult.OK)
                {
                    if (LSRetailPosis.Settings.HardwareProfiles.Printer.DeviceType == LSRetailPosis.Settings.HardwareProfiles.DeviceTypes.None)
                    {
                        this.Application.Services.Dialog.ShowMessage(ApplicationLocalizer.Language.Translate(10060), MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return false;
                    }
                    iPrintFromShowJournal = 1;
                    return true;
                }
            }
            iPrintFromShowJournal = 1;
            return false;
        }

        /// <summary>
        /// Print the standard slip, returns false if printing should be aborted altogether.
        /// </summary>
        /// <param name="formType"></param>
        /// <param name="posTransaction"></param>
        /// <param name="copyReceipt"></param>
        /// <returns></returns>
        public bool PrintReceipt(FormType formType, IPosTransaction posTransaction, bool copyReceipt)
        {
            bool result = false;
            if (formType == FormType.Receipt)
            {
                //SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);

                SqlConnection connection = new SqlConnection();
                if (application != null)
                    connection = application.Settings.Database.Connection;
                else
                    connection = ApplicationSettings.Database.LocalConnection;

                //Start :02/07/2014
                if (posTransaction is IRetailTransaction)
                {
                    RetailTransaction retailTransaction = (RetailTransaction)posTransaction;
                    SaleLineItem item = retailTransaction.SaleItems.Last.Value;

                    if (item.Description == "Add to gift card")
                    {
                        //====
                        if (application != null)
                            connection = application.Settings.Database.Connection;
                        else
                            connection = ApplicationSettings.Database.LocalConnection;
                        string sTransactionId = retailTransaction.TransactionId;
                        string sTerminalId = retailTransaction.TerminalId;
                        string sCardNo = string.Empty;
                        decimal sAmt = 0;

                        DataTable dt = GetGiftCardAmountInfo(connection, sTransactionId, sTerminalId);

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            for (int i = 0; i <= dt.Rows.Count -1; i++)
                            {
                                if (string.IsNullOrEmpty(sCardNo))
                                {
                                    sAmt = Convert.ToDecimal(dt.Rows[i]["AMOUNT"]);

                                    sCardNo = Convert.ToString(dt.Rows[i]["COMMENT"]);
                                    sCardNo = new String('x', Convert.ToInt16(sCardNo.Length) - 4) + sCardNo.Substring(Convert.ToInt16(sCardNo.Length) - 4);

                                }
                                else
                                {
                                    sCardNo = sCardNo + "  / " + new String('x', Convert.ToInt16(Convert.ToString(dt.Rows[i]["COMMENT"]).Length) - 4) + Convert.ToString(dt.Rows[i]["COMMENT"]).Substring(Convert.ToInt16(Convert.ToString(dt.Rows[i]["COMMENT"]).Length) - 4);
                                    sAmt = sAmt + Convert.ToDecimal(dt.Rows[i]["AMOUNT"]);
                                }
                            }
                        }
                        frmR_ProductAdvanceReceipt objProdAdv = new frmR_ProductAdvanceReceipt(posTransaction, connection, sTransactionId, Convert.ToString(sAmt), sTerminalId, item.Description, sCardNo);
                        objProdAdv.ShowDialog();
                    }
                    else
                    {
                        if(retailTransaction.RefundReceiptId == "1")
                        {
                            string sTransactionId = retailTransaction.TransactionId;
                            string sTerminalId = retailTransaction.TerminalId;
                            string sCardNo = string.Empty;
                            decimal sAmt = 0;

                            DataTable dt = GetGiftCardAmountInfo(connection, sTransactionId, sTerminalId);

                            if(dt != null && dt.Rows.Count > 0)
                            {
                                for(int i = 0; i <= dt.Rows.Count - 1; i++)
                                {
                                    if(string.IsNullOrEmpty(sCardNo))
                                    {
                                        sAmt = Convert.ToDecimal(dt.Rows[i]["AMOUNT"]);

                                        sCardNo = Convert.ToString(dt.Rows[i]["COMMENT"]);
                                        sCardNo = new String('x', Convert.ToInt16(sCardNo.Length) - 4) + sCardNo.Substring(Convert.ToInt16(sCardNo.Length) - 4);

                                    }
                                    else
                                    {
                                        sCardNo = sCardNo + "  / " + new String('x', Convert.ToInt16(Convert.ToString(dt.Rows[i]["COMMENT"]).Length) - 4) + Convert.ToString(dt.Rows[i]["COMMENT"]).Substring(Convert.ToInt16(Convert.ToString(dt.Rows[i]["COMMENT"]).Length) - 4);
                                        sAmt = sAmt + Convert.ToDecimal(dt.Rows[i]["AMOUNT"]);
                                    }
                                }
                            }

                            frmR_ProductAdvanceReceipt objProdAdv = new frmR_ProductAdvanceReceipt(posTransaction, connection, sTransactionId, Convert.ToString(sAmt), sTerminalId, item.Description, sCardNo, 1);
                            objProdAdv.ShowDialog();
                        }
                        else
                        {
                            if (retailTransaction.SaleIsReturnSale)
                            {
                                frmSaleInv reportfrm = new frmSaleInv(posTransaction, connection, copyReceipt, 0, 0, iPrintFromShowJournal);
                                reportfrm.ShowDialog();
                                frmSaleInvAccountsCopy reportfrmAcc = new frmSaleInvAccountsCopy(posTransaction, connection, copyReceipt, 0, 0, iPrintFromShowJournal);
                                reportfrmAcc.ShowDialog();
                                //frmSaleInvControlCopy reportfrmCon = new frmSaleInvControlCopy(posTransaction, connection, copyReceipt, 0, 0, iPrintFromShowJournal);
                                //reportfrmCon.ShowDialog();// commented on 080819/ req by Soudip paul
                            }
                            else
                            {
                                //Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch.frmLanguageForInvoice
                                //    objLI = new BlankOperations.WinFormsTouch.frmLanguageForInvoice();

                                //objLI.ShowDialog();
                                //int iLanguage = 0;
                                //if (objLI.isEnglish == true)
                                //    iLanguage = 1;
                                //else if (objLI.isArabic == true)
                                //    iLanguage = 2;
                                //else if (objLI.isBoth == true)
                                //    iLanguage = 3;
                               
                               
                                if(iPrintFromShowJournal==1)
                                {
                                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Is this a gift invoice?", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                                    {
                                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                        if (Convert.ToString(dialog.DialogResult).ToUpper().Trim() == "YES")
                                        {
                                            frmSaleInv reportfrm = new frmSaleInv(posTransaction, connection, copyReceipt, 1, 0, iPrintFromShowJournal);//Default English for Sales Return
                                            reportfrm.ShowDialog();

                                            frmSaleInv reportfrm1 = new frmSaleInv(posTransaction, connection, copyReceipt, 0, 1, iPrintFromShowJournal);
                                            reportfrm1.ShowDialog();
                                        }
                                        else
                                        {
                                            ChooseInvoiceLanguage(posTransaction, copyReceipt, connection, iPrintFromShowJournal);
                                        }
                                    }
                                    iPrintFromShowJournal = 0;
                                }
                                else
                                {
                                    ChooseInvoiceLanguage(posTransaction, copyReceipt, connection, iPrintFromShowJournal);
                                }
                                
                            }
                        }
                    }


                    //retailTransaction
                }

                //End : 02/07/2014




            }
            else if (formType == FormType.CustomerAccountDeposit)
            {
                RetailTransaction retailTransaction = posTransaction as RetailTransaction;
                string sGSSNo = string.Empty;
                if (retailTransaction != null)
                {
                    SqlConnection connection = new SqlConnection();
                    if (application != null)
                        connection = application.Settings.Database.Connection;
                    else
                        connection = ApplicationSettings.Database.LocalConnection;
                    string sTransactionId = retailTransaction.TransactionId;
                    // string sTerminalId = ApplicationSettings.Terminal.TerminalId;

                    string sTerminalId = retailTransaction.TerminalId;

                    DataTable dtAdv = GetAdvanceInfo(connection, sTransactionId, sTerminalId);

                    if (dtAdv != null && dtAdv.Rows.Count > 0)
                    {
                        sGSSNo = Convert.ToString(dtAdv.Rows[0]["GSSNUMBER"]);

                        if (sGSSNo != string.Empty)
                        {
                            frmR_GSSInstalmentReceipt objRGSS = new frmR_GSSInstalmentReceipt(posTransaction, connection, sTransactionId, Convert.ToString(dtAdv.Rows[0]["AMOUNT"]), sGSSNo, sTerminalId);
                            objRGSS.ShowDialog();
                        }
                        else
                        {
                            Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch.frmLanguageForInvoice
                                objLI = new BlankOperations.WinFormsTouch.frmLanguageForInvoice();

                            objLI.ShowDialog();
                            int iLanguage = 0;
                            if (objLI.isEnglish == true)
                                iLanguage = 1;
                            else if (objLI.isArabic == true)
                                iLanguage = 2;
                            else if (objLI.isBoth == true)
                                iLanguage = 3;

                            frmR_ProductAdvanceReceipt objProdAdv = new frmR_ProductAdvanceReceipt(posTransaction, connection, sTransactionId, Convert.ToString(dtAdv.Rows[0]["AMOUNT"]), sTerminalId,"","",0, iLanguage);
                            objProdAdv.ShowDialog();
                        }

                    }
                }

            }
            else
            {
                if (FiscalPrinter.FiscalPrinter.Instance.FiscalPrinterEnabled())
                {
                    return FiscalPrinter.FiscalPrinter.Instance.PrintReceipt(formType, posTransaction, copyReceipt);
                }

                FormModulation formMod = new FormModulation(Application.Settings.Database.Connection);
                IList<PrinterAssociation> printerMapping = PrintingActions.GetActivePrinters(formMod, formType, copyReceipt);

                // bool result = false;
                foreach (PrinterAssociation printerMap in printerMapping)
                {
                    bool printResult = PrintingActions.PrintFormTransaction(printerMap, formMod, formType, posTransaction, copyReceipt);

                    result = result || printResult;
                }
            }

            return result;
        }

        private static void ChooseInvoiceLanguage(IPosTransaction posTransaction, bool copyReceipt, SqlConnection connection,int iFromShowJour)
        {
            Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch.frmLanguageForInvoice
                objLI = new BlankOperations.WinFormsTouch.frmLanguageForInvoice();

            objLI.ShowDialog();
            int iLanguage = 0;
            if(objLI.isEnglish == true)
                iLanguage = 1;
            else if(objLI.isArabic == true)
                iLanguage = 2;
            else if(objLI.isBoth == true)
                iLanguage = 3;

            frmSaleInv reportfrm = new frmSaleInv(posTransaction, connection, copyReceipt, 0, iLanguage, iFromShowJour);
            reportfrm.ShowDialog();

            frmSaleInvAccountsCopy reportfrmAcc = new frmSaleInvAccountsCopy(posTransaction, connection, copyReceipt, 0, iLanguage, iFromShowJour);
            reportfrmAcc.ShowDialog();

            if (iFromShowJour == 1)
            {
                frmSaleInvControlCopy reportfrmCon = new frmSaleInvControlCopy(posTransaction, connection, copyReceipt, 0, iLanguage, iFromShowJour);
                reportfrmCon.ShowDialog();
            }
        }


        /// <summary>
        /// Print card slips.
        /// </summary>
        /// <param name="formType"></param>
        /// <param name="posTransaction"></param>
        /// <param name="tenderLineItem"></param>
        /// <param name="copyReceipt"></param>
        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "only single cast of each type per condition")]
        public void PrintCardReceipt(FormType formType, IPosTransaction posTransaction, ITenderLineItem tenderLineItem, bool copyReceipt)
        {
            //PrintingActions.Print(formType, copyReceipt, true,
            //    delegate(FormModulation formMod, FormInfo formInfo)
            //    {
            //        return formMod.GetTransformedCardTender(formInfo, ((ICardTenderLineItem)tenderLineItem).EFTInfo, (RetailTransaction)posTransaction);
            //    });
        }

        /// <summary>
        /// Print card slips.
        /// </summary>
        /// <param name="formType"></param>
        /// <param name="posTransaction"></param>
        /// <param name="eftInfo"></param>
        /// <param name="copyReceipt"></param>
        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "only single cast of each type per condition")]
        public void PrintCardReceipt(FormType formType, IPosTransaction posTransaction, IEFTInfo eftInfo, bool copyReceipt)
        {
            //PrintingActions.Print(formType, copyReceipt, true,
            //    delegate(FormModulation formMod, FormInfo formInfo)
            //    {
            //        return formMod.GetTransformedCardTender(formInfo, eftInfo, (RetailTransaction)posTransaction);
            //    });
        }

        /// <summary>
        /// Print customer account slips.
        /// </summary>
        /// <param name="formType"></param>
        /// <param name="posTransaction"></param>
        /// <param name="tenderLineItem"></param>
        /// <param name="copyReceipt"></param>
        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "only single cast of each type per condition")]
        public void PrintCustomerReceipt(FormType formType, IPosTransaction posTransaction, ITenderLineItem tenderLineItem, bool copyReceipt)
        {
            //PrintingActions.Print(formType, copyReceipt, true,
            //    delegate(FormModulation formMod, FormInfo formInfo)
            //    {
            //        return formMod.GetTransformedTender(formInfo, (TenderLineItem)tenderLineItem, (RetailTransaction)posTransaction);
            //    });
        }

        /// <summary>
        /// Print declare starting amount receipt
        /// </summary>
        /// <param name="posTransaction">FloatEntryTransaction</param>
        public void PrintStartngAmountReceipt(IPosTransaction posTransaction)
        {
            bool copyReceipt = false;

            PrintingActions.Print(FormType.FloatEntry, copyReceipt, false, delegate(FormModulation formMod, FormInfo formInfo)
            {
                StringBuilder reportLayout = new StringBuilder();
                StartingAmountTransaction startingAmountTransaction = (StartingAmountTransaction)posTransaction;

                PrintingActions.PrepareReceiptHeader(reportLayout, posTransaction, 10077, false);
                reportLayout.AppendLine(PrintingActions.SingleLine);

                reportLayout.AppendLine();
                reportLayout.AppendLine(PrintingActions.FormatTenderLine(ApplicationLocalizer.Language.Translate(10078),
                    Printing.InternalApplication.Services.Rounding.Round(startingAmountTransaction.Amount, true)));
                reportLayout.AppendLine(startingAmountTransaction.Description.ToString());
                reportLayout.AppendLine();
                reportLayout.AppendLine(PrintingActions.DoubleLine);

                return reportLayout.ToString();
            });
        }

        /// <summary>
        /// Print Float Entry Receipt
        /// </summary>
        /// <param name="posTransaction">FloatEntryTransaction</param>
        public void PrintFloatEntryReceipt(IPosTransaction posTransaction)
        {
            if (FiscalPrinter.FiscalPrinter.Instance.FiscalPrinterEnabled())
            {
                FiscalPrinter.FiscalPrinter.Instance.PrintFloatEntry(posTransaction);
                return;
            }

            bool copyReceipt = false;

            PrintingActions.Print(FormType.FloatEntry, copyReceipt, false, delegate(FormModulation formMod, FormInfo formInfo)
            {
                StringBuilder reportLayout = new StringBuilder();
                FloatEntryTransaction asFloatEntryTransaction = (FloatEntryTransaction)posTransaction;

                PrintingActions.PrepareReceiptHeader(reportLayout, posTransaction, 10061, copyReceipt);
                reportLayout.AppendLine(PrintingActions.SingleLine);

                reportLayout.AppendLine();
                reportLayout.AppendLine(PrintingActions.FormatTenderLine(ApplicationLocalizer.Language.Translate(10062),
                    Printing.InternalApplication.Services.Rounding.Round(asFloatEntryTransaction.Amount, true)));
                reportLayout.AppendLine(asFloatEntryTransaction.Description.ToString());
                reportLayout.AppendLine();
                reportLayout.AppendLine(PrintingActions.DoubleLine);

                return reportLayout.ToString();
            });
        }

        /// <summary>
        /// Print Tender Removal Receipt
        /// </summary>
        /// <param name="posTransaction">RemoveTenderTransaction</param>
        public void PrintRemoveTenderReceipt(IPosTransaction posTransaction)
        {
            if (FiscalPrinter.FiscalPrinter.Instance.FiscalPrinterEnabled())
            {
                FiscalPrinter.FiscalPrinter.Instance.PrintRemoveTender(posTransaction);
                return;
            }

            bool copyReceipt = false;

            PrintingActions.Print(FormType.RemoveTender, copyReceipt, false, delegate(FormModulation formMod, FormInfo formInfo)
            {
                StringBuilder reportLayout = new StringBuilder();
                PrintingActions.PrepareReceiptHeader(reportLayout, posTransaction, 10063, copyReceipt);
                reportLayout.AppendLine(PrintingActions.SingleLine);

                reportLayout.AppendLine();
                RemoveTenderTransaction asRemoveTenderTransaction = (RemoveTenderTransaction)posTransaction;
                reportLayout.AppendLine(PrintingActions.FormatTenderLine(ApplicationLocalizer.Language.Translate(10064),
                    Printing.InternalApplication.Services.Rounding.Round(asRemoveTenderTransaction.Amount, true)));
                reportLayout.AppendLine(asRemoveTenderTransaction.Description.ToString());
                reportLayout.AppendLine();
                reportLayout.AppendLine(PrintingActions.DoubleLine);

                formMod = new FormModulation(Application.Settings.Database.Connection);
                formInfo = formMod.GetInfoForForm(FormType.FloatEntry, copyReceipt, LSRetailPosis.Settings.HardwareProfiles.Printer.ReceiptProfileId);

                return reportLayout.ToString();
            });
        }

        /// <summary>
        /// Print credit card memo.
        /// </summary>
        /// <param name="formType"></param>
        /// <param name="posTransaction"></param>
        /// <param name="tenderLineItem"></param>
        /// <param name="copyReceipt"></param>
        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "only single cast of each type per condition")]
        public void PrintCreditMemo(FormType formType, IPosTransaction posTransaction, ITenderLineItem tenderLineItem, bool copyReceipt)
        {
            //PrintingActions.Print(formType, copyReceipt, true,
            //    delegate(FormModulation formMod, FormInfo formInfo)
            //    {
            //        return formMod.GetTransformedTender(formInfo, (TenderLineItem)tenderLineItem, (RetailTransaction)posTransaction);
            //    });
        }

        /// <summary>
        /// Print balance of credit card memo.
        /// </summary>
        /// <param name="formType"></param>
        /// <param name="balance"></param>
        /// <param name="copyReceipt"></param>
        public void PrintCreditMemoBalance(FormType formType, Decimal balance, bool copyReceipt)
        {
            PrintingActions.Print(formType, copyReceipt, true,
                delegate(FormModulation formMod, FormInfo formInfo)
                {
                    IRetailTransaction tr = Printing.InternalApplication.BusinessLogic.Utility.CreateRetailTransaction(
                        ApplicationSettings.Terminal.StoreId,
                        ApplicationSettings.Terminal.StoreCurrency,
                        ApplicationSettings.Terminal.TaxIncludedInPrice,
                        Printing.InternalApplication.Services.Rounding);
                    tr.AmountToAccount = balance;
                    formMod.GetTransformedTransaction(formInfo, (RetailTransaction)tr);

                    return formInfo.Header;
                });
        }

        /// <summary>
        /// Print invoice receipt.
        /// </summary>
        /// <param name="posTransaction">The pos transaction.</param>
        /// <param name="copyInvoice">if set to <c>true</c> [copy invoice].</param>
        /// <param name="printPreview">Not supported.</param>
        /// <returns></returns>
        public bool PrintInvoice(IPosTransaction posTransaction, bool copyInvoice, bool printPreview)
        {
            if (FiscalPrinter.FiscalPrinter.Instance.FiscalPrinterEnabled())
            {
                FiscalPrinter.FiscalPrinter.Instance.PrintInvoice(posTransaction, copyInvoice, printPreview);
                return true;
            }

            FormModulation formMod = new FormModulation(Application.Settings.Database.Connection);

            bool noPrinterDefined = true; // Initilize as error

            IList<PrinterAssociation> printerMapping = PrintingActions.GetActivePrinters(formMod, FormType.Invoice, copyInvoice);

            bool result = true;
            foreach (PrinterAssociation printerMap in printerMapping)
            {
                noPrinterDefined = noPrinterDefined && (printerMap.Type == DeviceTypes.None);

                if ((printerMap.Type == DeviceTypes.OPOS) || (printerMap.Type == DeviceTypes.Windows))
                {
                    bool printResult = PrintingActions.PrintFormTransaction(printerMap, formMod, FormType.Invoice, posTransaction, copyInvoice);

                    result = result && printResult;
                }
            }

            if (noPrinterDefined)
            {
                // 10060 - No printer type has been defined.
                this.Application.Services.Dialog.ShowMessage(ApplicationLocalizer.Language.Translate(10060), MessageBoxButtons.OK, MessageBoxIcon.Stop);
                result = false;
            }

            return result;

        }

        /// <summary>
        /// Print Tender Decaraton Receipt
        /// </summary>
        /// <param name="posTransaction">TenderDeclarationTransaction</param>
        public void PrintTenderDeclaration(IPosTransaction posTransaction)
        {
            bool copyReceipt = false;

            PrintingActions.Print(FormType.TenderDeclaration, copyReceipt, false, delegate(FormModulation formMod, FormInfo formInfo)
            {
                StringBuilder reportLayout = new StringBuilder();
                PrintingActions.PrepareReceiptHeader(reportLayout, posTransaction, 10065, copyReceipt);
                reportLayout.AppendLine(PrintingActions.SingleLine);

                PrintingActions.PrepareReceiptTenders(reportLayout, posTransaction);
                reportLayout.AppendLine(PrintingActions.DoubleLine);

                return reportLayout.ToString();
            });
        }

        /// <summary>
        /// Print Bank drop Receipt
        /// </summary>
        /// <param name="posTransaction">BankDropTransaction</param>
        public void PrintBankDrop(IPosTransaction posTransaction)
        {
            if (FiscalPrinter.FiscalPrinter.Instance.FiscalPrinterEnabled())
            {
                FiscalPrinter.FiscalPrinter.Instance.PrintBankDrop(posTransaction);
                return;
            }

            bool copyReceipt = false;

            PrintingActions.Print(FormType.BankDrop, copyReceipt, false, delegate(FormModulation formMod, FormInfo formInfo)
            {
                StringBuilder reportLayout = new StringBuilder();
                PrintingActions.PrepareReceiptHeader(reportLayout, posTransaction, 10066, copyReceipt);
                reportLayout.AppendLine(PrintingActions.FormatHeaderLine(10069, ((BankDropTransaction)posTransaction).BankBagNo.ToString(), true));
                reportLayout.AppendLine(PrintingActions.SingleLine);

                PrintingActions.PrepareReceiptTenders(reportLayout, posTransaction);
                reportLayout.AppendLine(PrintingActions.DoubleLine);

                return reportLayout.ToString();
            });
        }

        /// <summary>
        /// Print safe drop Receipt
        /// </summary>
        /// <param name="posTransaction">SafeDropTransaction</param>
        public void PrintSafeDrop(IPosTransaction posTransaction)
        {
            if (FiscalPrinter.FiscalPrinter.Instance.FiscalPrinterEnabled())
            {
                FiscalPrinter.FiscalPrinter.Instance.PrintSafeDrop(posTransaction);
                return;
            }

            bool copyReceipt = false;

            PrintingActions.Print(FormType.SafeDrop, copyReceipt, false, delegate(FormModulation formMod, FormInfo formInfo)
            {
                StringBuilder reportLayout = new StringBuilder();
                PrintingActions.PrepareReceiptHeader(reportLayout, posTransaction, 10067, copyReceipt);
                reportLayout.AppendLine(PrintingActions.SingleLine);

                PrintingActions.PrepareReceiptTenders(reportLayout, posTransaction);
                reportLayout.AppendLine(PrintingActions.DoubleLine);

                return reportLayout.ToString();
            });
        }

        /// <summary>
        /// Pring Gift Certificate
        /// </summary>
        /// <param name="formType">Currently unused</param>
        /// <param name="posTransaction"></param>
        /// <param name="giftCardItem"></param>
        /// <param name="copyReceipt">True if it is duplicate</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "Grandfather")]
        public void PrintGiftCertificate(FormType formType, IPosTransaction posTransaction, IGiftCardLineItem giftCardLineItem, bool copyReceipt)
        {
            PrintingActions.Print(formType, copyReceipt, false, delegate(FormModulation formMod, FormInfo formInfo)
            {
                StringBuilder reportLayout = new StringBuilder();

                PrintingActions.PrepareReceiptHeader(reportLayout, posTransaction, 10068, copyReceipt);
                reportLayout.AppendLine(PrintingActions.SingleLine);

                reportLayout.AppendLine();
                reportLayout.AppendLine(PrintingActions.FormatTenderLine(ApplicationLocalizer.Language.Translate(10070), giftCardLineItem.SerialNumber));
                reportLayout.AppendLine(PrintingActions.FormatTenderLine(ApplicationLocalizer.Language.Translate(10071),
                    Printing.InternalApplication.Services.Rounding.RoundForDisplay(giftCardLineItem.Balance,
                    true, true)));
                reportLayout.AppendLine();
                reportLayout.AppendLine(PrintingActions.DoubleLine);

                return reportLayout.ToString();
            });
        }

        /// <summary>
        /// Print pack slip.
        /// </summary>
        /// <param name="posTransaction">Transaction instance.</param>
        public void PrintPackSlip(IPosTransaction posTransaction)
        {
            bool copyReceipt = false;
            FormType formType = FormType.PackingSlip;

            PrintingActions.Print(formType, copyReceipt, true, delegate(FormModulation formMod, FormInfo formInfo)
            {
                formMod.GetTransformedTransaction(formInfo, (RetailTransaction)posTransaction);

                return formInfo.Header + formInfo.Details + formInfo.Footer;
            });
        }

        /// <summary>
        /// Print directly using the printText provided
        /// </summary>
        /// <param name="allowFallback">if set to <c>true</c> [allow fallback].</param>
        /// <param name="printText">The print text.</param>
        public void PrintDefault(bool allowFallback, string printText)
        {
            if (Printing.InternalApplication.Services.Peripherals.Printer.IsActive)
            {   // Print to the default printer (#1)
                Printing.InternalApplication.Services.Peripherals.Printer.PrintReceipt(printText);
            }
            else if (allowFallback && Printing.InternalApplication.Services.Peripherals.Printer2.IsActive)
            {   // Use the fallback printer
                Printing.InternalApplication.Services.Peripherals.Printer2.PrintReceipt(printText);
            }
            else
            {
                NetTracer.Information("Printing.PrintDefault() - printing is skipped as no printer is available.");
            }
        }

        public DataTable GetAdvanceInfo(SqlConnection conn, string sTransactionId, string sTerminalId)
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            DataTable dt = new DataTable();
            string commandText = "SELECT ISNULL(GSSNUMBER,0) AS GSSNUMBER, CAST(ISNULL(AMOUNT,0) AS DECIMAL(28,2)) AS AMOUNT FROM  RETAILDEPOSITTABLE WHERE TRANSACTIONID ='" + sTransactionId + "' AND TERMINALID ='" + sTerminalId + "'";

            SqlCommand command = new SqlCommand(commandText, conn);
            command.CommandTimeout = 0;

            SqlDataAdapter da = new SqlDataAdapter(command);
            da.Fill(dt);
            if (conn.State == ConnectionState.Open)
                conn.Close();

            return dt;

        }
        public DataTable GetGiftCardAmountInfo(SqlConnection conn, string sTransactionId, string sTerminalId)
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            DataTable dt = new DataTable();
            string commandText = "SELECT COMMENT, CAST(ISNULL(PRICE,0) AS DECIMAL(28,2)) AS AMOUNT FROM  RETAILTRANSACTIONSALESTRANS WHERE TRANSACTIONID ='" + sTransactionId + "' AND TERMINALID ='" + sTerminalId + "'";

            SqlCommand command = new SqlCommand(commandText, conn);
            command.CommandTimeout = 0;

            SqlDataAdapter da = new SqlDataAdapter(command);
            da.Fill(dt);
            if (conn.State == ConnectionState.Open)
                conn.Close();

            return dt;

        }
    }
}
