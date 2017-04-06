using CsvHelper.Configuration;
using DMSLite.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMSLite.CSVMaps
{
    public sealed class DonationClassMap : CsvClassMap<Donation>
    {
        public DonationClassMap()
        {
            Map(m => m.Id);
            Map(m => m.DonationDonor_Id);
            Map(m => m.ObjectDescription);
            Map(m => m.Value);
            Map(m => m.DonationBatch_Id);
            Map(m => m.DonationAccount_Id);
            Map(m => m.DonationReceipt_Id);
            Map(m => m.TenantOrganizationId);
            Map(m => m.Gift);
        }
    }
}