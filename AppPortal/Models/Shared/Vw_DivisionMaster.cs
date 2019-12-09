using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

//Created from database views because we cannot connect to ERP data any other way
//Maybe one day they will support Restful API 

namespace AppPortal.Models
{
    public class Vw_DivisionMaster
    {

        public int ID { get; set; }

        [Display(Name = "Program Number")]
        public string ProgramNo { get; set; }

        public string SectionName { get; set; }

        public string DivisionName { get; set; }

        public string ProgramName { get; set; }

        public string DivLead { get; set; }

        public string DivLeadEmail { get; set; }

        //public float? FiscalYr { get; set; }
    }
}
