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

        public Division Division { get; set; }

        public Manager Manager { get; set; }

        public Analyst Analyst { get; set; }

        public IEnumerable<Division> Divisions { get; set; }

        public IEnumerable<DivLead> Leads { get; set; }

        public IEnumerable<Analyst> Analysts { get; set; }

        public IEnumerable<Manager> Managers { get; set; }

        public IEnumerable<OrgChart> Orgs { get; set; }
    }
}
