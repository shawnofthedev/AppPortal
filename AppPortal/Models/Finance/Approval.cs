using System;

namespace AppPortal.Models.Finance
{
    public class Approval
    {
        public int Id { get; set; }

        public int CapFundingRequestId { get; set; }

        public bool L10Approved { get; set; }

        public bool L20Approved { get; set; }

        public bool L30Approved { get; set; }

        public bool L40Approved { get; set; }

        public bool L10Denied { get; set; }

        public bool L20Denied { get; set; }

        public bool L30Denied { get; set; }

        public bool L40Denied { get; set; }

        public string L10DenyReason { get; set; }

        public string L20DenyReason { get; set; }

        public string L30DenyReason { get; set; }

        public string L40DenyReason { get; set; }

        public string L10ByWho { get; set; }

        public string L20ByWho { get; set; }

        public string L30ByWho { get; set; }

        public string L40ByWho { get; set; }

        public DateTime L10TimeStamp { get; set; }

        public DateTime L20TimeStamp { get; set; }

        public DateTime L30TimeStamp { get; set; }

        public DateTime L40TimeStamp { get; set; }

        public bool AssetReplaceApprove { get; set; }

        public bool AssetReplaceDeny { get; set; }

        public string AssetReplaceDenyReason { get; set; }

        public string AssetReplaceByWho { get; set; }

        public DateTime AssetReplaceTimeStamp { get; set; }

        public bool FinalApproval { get; set; }

        public bool FinalDenial { get; set; }

        public string FinalDenyReason { get; set; }

        public string FinalByWho { get; set; }

        public DateTime FinalTimeStamp { get; set; }
    }
}
