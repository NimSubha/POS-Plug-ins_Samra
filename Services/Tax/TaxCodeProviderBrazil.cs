/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Diagnostics;
using LSRetailPosis.Transaction.Line.SaleItem;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;

namespace Microsoft.Dynamics.Retail.Pos.Tax
{
    /// <summary>
    /// Tax code provider for Brazil
    /// </summary>
    public sealed class TaxCodeProviderBrazil : TaxCodeProvider
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="application"></param>
        public TaxCodeProviderBrazil(IApplication application)
            : base(application)
        {
        }

        /// <summary>
        /// Returns the tax base amount for the given sales line item
        /// </summary>
        /// <param name="taxableItem"> </param>
        /// <param name="codes"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "codes", Justification = "Grandfather"),
         System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Grandfather")]
        public override decimal GetBasePriceForTaxIncluded(ITaxableItem taxableItem, ReadOnlyCollection<TaxCode> codes)
        {
            if (taxableItem == null)
            {
                throw new ArgumentNullException("taxableItem");
            }

            // Even though we use TaxIncludedInPrice, we have to adopt NetAmountPerUnit to mimic the Fiscal Printer behavior
            return taxableItem.NetAmountPerUnit;
        }
    }
}
