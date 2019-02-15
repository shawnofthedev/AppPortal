using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppPortal.Models.Finance
{
    public class OrgChart
    {
        public int Id { get; set; }

        public string FundFunctProg { get; set; }

        public string ProgramNum { get; set; }

        public string ProgramName { get; set; }

        public Division Division { get; set; }

        public int DivisionId { get; set; }

        public Analyst Analyst { get; set; }

        public int AnalystId { get; set; }
    }

    public class Division
    {
        public int Id { get; set; }

        public string DivisionName { get; set; }

        public DivLead DivLead { get; set; }

        public int DivLeadId { get; set; }
    }

    public class DivLead
    {
        public int Id { get; set; }

        public string DivLeadName { get; set; }

        public string DivLeadEmail { get; set; }
    }

    public class Analyst
    {
        public int Id { get; set; }

        public string AnalystName { get; set; }

        public string AnalystEmail { get; set; }
    }
}
