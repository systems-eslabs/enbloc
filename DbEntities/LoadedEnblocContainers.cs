using System;
using System.Collections.Generic;

namespace Enbloc.DbEntities
{
    public partial class LoadedEnblocContainers
    {
        public long Id { get; set; }
        public string TransactionId { get; set; }
        public string Status { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? HoldDate { get; set; }
        public DateTime? GateOutDate { get; set; }
        public DateTime? GateInDate { get; set; }
        public string Vessel { get; set; }
        public string Voyage { get; set; }
        public string EnblocNumber { get; set; }
        public string Srl { get; set; }
        public string ContainerNo { get; set; }
        public int ContainerSize { get; set; }
        public string ContainerType { get; set; }
        public string Wt { get; set; }
        public string Cargo { get; set; }
        public string IsoCode { get; set; }
        public string SealNo1 { get; set; }
        public string SealNo2 { get; set; }
        public string SealNo3 { get; set; }
        public string ImdgClass { get; set; }
        public string ReferTemrature { get; set; }
        public string OogDeatils { get; set; }
        public string ContainerGrossDetails { get; set; }
        public string CargoDescription { get; set; }
        public string BlNumber { get; set; }
        public string Name { get; set; }
        public string ItemNo { get; set; }
        public string DisposalMode { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
