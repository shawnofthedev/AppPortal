using AppPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppPortal.ViewModels
{
    public class FundingRequestViewModel
    {
        public Vw_DivisionMaster Vw_DivisionMaster { get; set; }

        public MunisVw_fa_master MunisVw_Fa_Master { get; set; }

        public CapFundingRequest CapFundingRequest { get; set; }

        public AttachedQuote AttachedQuote { get; set; }

        public IEnumerable<FundingRequestAttachments> FundingRequestAttachments { get; set; }

        public IEnumerable<StaggeredCost> StaggeredCosts { get; set; }

        public IEnumerable<AttachedQuote> AttachedQuotes { get; set; }

        public IEnumerable<QuoteAttachments> QuoteAttachments { get; set; }
    }
    public class StaggeredCostViewModel
    {
        public CapFundingRequest CapFundingRequest { get; set; }

        public StaggeredCost StaggeredCost { get; set; }

        public IEnumerable<StaggeredCost> StaggeredCosts { get; set; }
    }
}
