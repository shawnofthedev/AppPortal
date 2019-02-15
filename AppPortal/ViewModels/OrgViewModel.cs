using AppPortal.Models.Finance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppPortal.ViewModels
{
    public class OrgViewModel
    {
        public OrgChart OrgChart { get; set; }

        public IEnumerable<Division> Divisions { get; set; }

        public IEnumerable<Analyst> Analysts { get; set; }
    }
}
