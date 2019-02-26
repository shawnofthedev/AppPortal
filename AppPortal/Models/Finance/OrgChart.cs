using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AppPortal.Models.Finance
{
    public class OrgChart
    {
        public int Id { get; set; }

        [Display(Name = "Fund.Funct.Prog")]
        public string FundFunctProg { get; set; }

        [Display(Name = "Program Number")]
        public string ProgramNum { get; set; }

        [Display(Name = "Program Name")]
        public string ProgramName { get; set; }

        public Division Division { get; set; }

        public int DivisionId { get; set; }

        public Analyst Analyst { get; set; }

        public int AnalystId { get; set; }
    }
    public class Manager
    {
        public int Id { get; set; }

        [Display(Name = "Manager Name")]
        public string ManagerName { get; set; }

        [Display(Name = "Manager Email")]
        public string ManagerEmail { get; set; }

        [Display(Name = "Program")]
        public OrgChart OrgChart { get; set; }

        public int OrgChartId { get; set; }
    }

    public class Division
    {
        public int Id { get; set; }

        [Display(Name = "Division Name")]
        public string DivisionName { get; set; }

        [Display(Name = "Secretary")]
        public DivLead DivLead { get; set; }

        public int DivLeadId { get; set; }
    }

    public class DivLead
    {
        public int Id { get; set; }


        [Display(Name = "Secretary Name")]
        public string DivLeadName { get; set; }

        [Display(Name = "Secretary Email")]
        public string DivLeadEmail { get; set; }
    }

    public class Analyst
    {
        public int Id { get; set; }

        [Display(Name = "Financial Analyst")]
        public string AnalystName { get; set; }

        [Display(Name = "Analyst Email")]
        public string AnalystEmail { get; set; }
    }
}
