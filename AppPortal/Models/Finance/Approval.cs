//YOLO
//probably breaking convention by putting all these classes in the same file.
using System;
using System.ComponentModel.DataAnnotations;

namespace AppPortal.Models.Finance
{
    public class ManagerApproval
    {
        public int Id { get; set; }
        public bool Approved { get; set; }
        public bool Denied { get; set; }

        [Display(Name ="Reason for denial")]
        public string DenyReason { get; set; }

        [Display(Name ="Manager's Name")]
        public string ByWho { get; set; }
        public DateTime TimeStamp { get; set; }
        public CapFundingRequest CapFundingRequest { get; set; }
        public int CapFundingRequestId { get; set; }
    }

    public class SecretaryApproval
    {
        public int Id { get; set; }
        public bool Approved { get; set; }
        public bool Denied { get; set; }

        [Display(Name ="Reason for denial")]
        public string DenyReason { get; set; }

        [Display(Name ="Secretary's Name")]
        public string ByWho { get; set; }
        public DateTime TimeStamp { get; set; }
        public int CapFundingRequestId { get; set; }
        public CapFundingRequest CapFundingRequest { get; set; }
    }

    public class AnalystApproval
    {
        public int Id { get; set; }
        public bool Approved { get; set; }
        public bool Denied { get; set; }

        [Display(Name ="Reason for denial")]
        public string DenyReason { get; set; }

        [Display(Name ="Analyst's Name")]
        public string ByWho { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool ForFleet { get; set; }
        public int CapFundingRequestId { get; set; }
        public CapFundingRequest CapFundingRequest { get; set; }
    }

    public class FleetApproval
    {
        public int Id { get; set; }
        public bool Approved { get; set; }
        public bool Denied { get; set; }

        [Display(Name ="Reason for denial")]
        public string DenyReason { get; set; }

        [Display(Name ="Analyst's Name")]
        public string ByWho { get; set; }
        public DateTime TimeStamp { get; set; }
        public int CapFundingRequestId { get; set; }
        public CapFundingRequest CapFundingRequest { get; set; }
    }

    public class FinalApproval
    {
        public int Id { get; set; }
        public bool Approved { get; set; }
        public bool Denied { get; set; }

        [Display(Name ="Reason for denial")]
        public string DenyReason { get; set; }

        public string ByWho { get; set; }
        public DateTime TimeStamp { get; set; }
        public int CapFundingRequestId { get; set; }
        public CapFundingRequest CapFundingRequest { get; set; }
    }
}
