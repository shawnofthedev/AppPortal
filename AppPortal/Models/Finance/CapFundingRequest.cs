using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AppPortal.Models
{
    public class CapFundingRequest
    {
        public int Id { get; set; }

        [Display(Name = "Program Number")]
        public string ProgramNum { get; set; }

        [Display(Name = "Division Name")]
        public string DivisionName { get; set; }

        [Display(Name = "Program Name")]
        public string ProgramName { get; set; }

        //These titles of these could change over time so 
        //divlead was chosen.  To change the display name
        //change the string in quotes 
        [Display(Name = "Secretary")]
        public string DivLead { get; set; }

        public string Initiator { get; set; }

        public DateTime? TimeStamp { get; set; }

        public DateTime? LastUpdate { get; set; }

        [Display(Name = "Secretary Email")]
        public string DivLeadEmail { get; set; }

        [Display(Name = "Project / Purchase Name")]
        public string ProjectName { get; set; }

        [Display(Name = "Project Overview")]
        public string ProjectOverview { get; set; }

        [Display(Name = "Total Requested Amount")]
        //[DataType(DataType.Currency)]
        public decimal AmtRequest { get; set; }

        [Display(Name = "Total from other Sources")]
        //[DataType(DataType.Currency)]
        public decimal AmtOtherSource { get; set; }

        [Display(Name = "Total Cost")]
        //[DataType(DataType.Currency)]
        public decimal TotalCost { get; set; }

        [Display(Name = "Explanation of other Sources")]
        public string OtherSourceExplain { get; set; }

        [Display(Name = "One Time Purchase/Project")]
        public bool OneTimePurchase { get; set; }

        [Display(Name = "Recurring Operational Need")]
        public bool RecurringNeed { get; set; }

        [Display(Name = "Replace Existing Asset")]
        public bool ReplaceAsset { get; set; }

        [Display(Name = "Year")]
        public Int16 AssetYear { get; set; }

        public string Make { get; set; }

        [Display(Name = "EBCI Tag")]
        public string AssetNum { get; set; }

        [Display(Name = "Serial/Vin")]
        public string Serial { get; set; }

        [Display(Name = "Description")]
        public string AssetDesc { get; set; }

        [Display(Name="Status")]
        public string RequestStatus { get; set; }

        public IEnumerable<FundingRequestAttachments> FundingRequestAttachments { get; set; }

        public IEnumerable<StaggeredCost> StaggeredCosts { get; set; }

        public IEnumerable<AttachedQuote> AttachedQuotes { get; set; } 
    }

    public class FundingRequestAttachments
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
 
        public string FileName { get; set; }

        public string FileLocation { get; set; }

        public CapFundingRequest CapFundingRequest { get; set; }

        public int CapFundingRequestId { get; set; }
    }

    public class StaggeredCost
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "Fiscal Year")]
        public Int16 FiscalYear { get; set; }

        public decimal Amount { get; set; }

        [Display(Name = "Amt. Justification")]
        public string AmtJustification { get; set; }

        [Display(Name = "Description of Activity")]
        public string DescOfActivity { get; set; }

        public CapFundingRequest CapFundingRequest { get; set; }

        public int CapfundingRequestId { get; set; }

    } 

    public class AttachedQuote 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
 
        [Display(Name = "Vendor Name")]
        public string VendorName { get; set; }

        [Required(ErrorMessage = "You must provide a phone number")]
        [Display(Name = "Telephone #")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string ContactNum { get; set; }


        [Display(Name = "Contact Name")]
        public string ContactName { get; set; }

        [Display(Name = "Quote")]
        public decimal QuoteAmt { get; set; }

        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime? QuoteDate { get; set; }

        public string AttachedFileName { get; set; }

        public string AttachedFileLocation { get; set; }

        public CapFundingRequest CapFundingRequest { get; set; }

        public int CapFundingRequestId { get; set; } 
    }
}
