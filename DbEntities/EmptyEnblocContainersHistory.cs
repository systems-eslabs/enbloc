using System;
using System.Collections.Generic;

namespace Enbloc.DbEntities
{
    public partial class EmptyEnblocContainersHistory
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
        public string ViaNo { get; set; }
        public string ContainerNo { get; set; }
        public int ContainerSize { get; set; }
        public string ContainerType { get; set; }
        public string IsoCode { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? HistoryBy { get; set; }
        public DateTime HistoryDate { get; set; }
    }
}
