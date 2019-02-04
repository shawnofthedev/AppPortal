using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppPortal.Models
{ 
    public class MunisVw_EmployeeAnnual
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int EmployeeNumber { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? AnnualAvailable { get; set; }
    }
}
