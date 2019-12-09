using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

//Created from database views because we cannot connect to ERP data any other way
//Maybe one day they will support Restful API 

namespace AppPortal.Models
{
    public class MunisVw_fa_master
    {
        [Key]
        [Display(Name ="Asset Number")]
        public string a_asset_number { get; set; }

        public string fa_serial_number { get; set; }

        public string a_asset_desc { get; set; }

        public string a_department_code { get; set; }

        public string fm_manuf_code { get; set; }

        public string fa_model_number { get; set; }

        public Int16 fa_model_year { get; set; }

        public string fa_license_number { get; set; } 
    }
}
