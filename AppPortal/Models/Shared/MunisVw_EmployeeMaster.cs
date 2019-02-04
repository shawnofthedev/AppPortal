 using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppPortal.Models
{ 
    public class MunisVw_EmployeeMaster
    {
        [Key]
        [Column(Order = 0)]
        [Display(Name = "Employee Number")]
        public int EmployeeNumber { get; set; }

        [Column(Order = 1)]
        [StringLength(1)]
        public string ActiveStatusCode { get; set; }

        [StringLength(11)]
        public string SSN { get; set; }

        [StringLength(30)]
        public string LastName { get; set; }

        [StringLength(30)]
        public string FirstName { get; set; }

        [StringLength(1)]
        public string MI { get; set; }

        [Column(Order = 2)]
        [StringLength(30)]
        public string ADDR1 { get; set; }

        [Column(Order = 3)]
        [StringLength(30)]
        public string ADDR2 { get; set; }

        [Column(Order = 4)]
        [StringLength(20)]
        public string CITY { get; set; }

        [Column(Order = 5)]
        [StringLength(2)]
        public string STATE { get; set; }

        [Column(Order = 6)]
        [StringLength(10)]
        public string ZIP { get; set; }

        [Column(Order = 7)]
        [StringLength(50)]
        public string EMAIL { get; set; }

        [Column(Order = 8)]
        [StringLength(30)]
        public string HomePhone { get; set; }

        [StringLength(30)]
        public string DtBirth { get; set; }

        [StringLength(1)]
        public string MaritalStatusCode { get; set; }

        [StringLength(1)]
        public string GenderCode { get; set; }

        [StringLength(2)]
        public string EEOEthnicityCode { get; set; }

        [StringLength(1)]
        public string EEOFullTime { get; set; }

        [StringLength(4)]
        public string VETERAN { get; set; }

        [StringLength(30)]
        public string DtHire { get; set; }

        [StringLength(30)]
        public string DtTerm { get; set; }

        [StringLength(2)]
        public string PayFrequencyCode { get; set; }

        [Column(Order = 9)]
        [StringLength(4)]
        public string WorkLocCd { get; set; }

        [StringLength(30)]
        public string LongDescription { get; set; }

        [StringLength(10)]
        public string ShortDescription { get; set; }

        [StringLength(5)]
        public string DepartmentCode { get; set; }
    }
}
