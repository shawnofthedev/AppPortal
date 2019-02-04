using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppPortal.Models
{
    public partial class LeaveBuyBackRequest
    {
        public int Id { get; set; }

        [Display(Name = "Employee Number")]
        public int EmpNum { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Employee Name")]
        public string EmpName { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}")]
        [Display(Name = "Pay Period End Date")]
        public DateTime PayPeriodEnd { get; set; }

        [Range(1, 40, ErrorMessage = "Cannot request more than 40 hours")]
        [Display(Name = "Number of Hours Requested")]
        public int HoursRequested { get; set; }

        public DateTime RequestDate { get; set; }

        [Required]
        [StringLength(50)]
        public string RequestBy { get; set; }

        [Required]
        [StringLength(3)]
        public string ProgramNum { get; set; }

        [Column(TypeName = "numeric")]
        [Range(120, double.MaxValue, ErrorMessage = "Employee does not have the minimum 120 hours available")]
        public decimal AvailableHours { get; set; }

        public int FiscalYear { get; set; }

    }
}
