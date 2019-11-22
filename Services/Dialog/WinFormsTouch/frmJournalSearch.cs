/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Windows.Forms;
using LSRetailPosis.POSProcesses;
using Microsoft.Dynamics.Retail.Pos.Contracts.UI;

namespace Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch
{
    public partial class frmJournalSearch : frmTouchBase
    {

        private string selectedTransactionId;
        private DateTime selectedDate;

        /// <summary>
        /// Returns selectedtransactionid as string.
        /// </summary>
        public string SelectedTransactionId
        {
            get { return selectedTransactionId; }
        }
        /// <summary>
        /// Returns selected date as datetime.
        /// </summary>
        public DateTime SelectedDate
        {
            get { return selectedDate; }
        }
        /// <summary>
        /// Gives all details from database for search.
        /// </summary>
        public frmJournalSearch()
        {

            //
            // Get all text through the Translation function in the ApplicationLocalizer
            //
            // TextID's for frmJournalSearch are reserved at 2450 - 2499 
            // The last id in use is: 2456
            //

            InitializeComponent();

            try
            {
                // Translate all components...

                groupBoxTransactionId.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(2450);           // Search by receipt id
                groupBoxDate.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(2451);                    // Search by date

                btnClose.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(2452);                        // Close
                btnDate.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(2453);                         // Select date

                lblTransactionIdHeading.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(2454) + ":";   // Receipt Id:
                btnClear.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(1280);                        // Clear

                lblHeader.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(2457);                       // Journal search

            }
            catch (Exception)
            {
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnTransactionId_Click(object sender, EventArgs e)
        {
            if (txtTransactionId.Text.Trim().Length != 0)
            {
                selectedTransactionId = txtTransactionId.Text.Trim();

                this.DialogResult = DialogResult.OK;
                Close();
            }

            SetFormFocus();
        }

        private void btnDate_Click(object sender, EventArgs e)
        {
            try
            {
                // Display the date picker dialog....
                using (frmDatePicker dateDialog = new frmDatePicker())
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dateDialog);

                    if (dateDialog.DialogResult != DialogResult.OK)
                    {
                        SetFormFocus();
                        return;
                    }

                    this.selectedDate = dateDialog.SelectedDate;
                    this.DialogResult = DialogResult.OK;
                    Close();
                }
            }
            catch
            {
            }
        }
        

        private void frmJournalSearch_Load(object sender, EventArgs e)
        {
            SetFormFocus();
        }

        private void SetFormFocus()
        {
            txtTransactionId.Focus();
        }

        private void txtTransactionId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnTransactionId_Click(this, new EventArgs());
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtTransactionId.Text = string.Empty;
            SetFormFocus();
        }

    }
}
