using AppPortal.Models;
using System.Collections.Generic;

namespace AppPortal.ViewModels
{
    public class StaggeredCostViewModel
    { 
        public CapFundingRequest CapFundingRequest { get; set; }

        public StaggeredCost StaggeredCost { get; set; }

        public IEnumerable<StaggeredCost> StaggeredCosts { get; set; } 
    }
}
