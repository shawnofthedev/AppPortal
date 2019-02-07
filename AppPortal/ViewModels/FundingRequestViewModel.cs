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

        public IEnumerable<FundingRequestAttachments> FundingRequestAttachments { get; set; }

        public IEnumerable<StaggeredCost> StaggeredCosts { get; set; }
    } 

}
