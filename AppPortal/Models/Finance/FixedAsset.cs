using System;
using System.Collections.Generic;

namespace AppPortal.Models
{
    public partial class FixedAsset
    {
        public int Id { get; set; }
        public string AssetNum { get; set; }
        public string AssetDesc { get; set; }
        public string AssetDept { get; set; }
        public string TransactType { get; set; }
        public string DisposalReason { get; set; }
        public string TransferTo { get; set; }
        public string MailedTo { get; set; }
        public string MailedFrom { get; set; }
        public string DateOfRequest { get; set; }
        public string Notes { get; set; }
    }
}
