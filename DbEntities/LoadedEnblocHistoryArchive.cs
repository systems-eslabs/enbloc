﻿using System;
using System.Collections.Generic;

namespace Enbloc.DbEntities
{
    public partial class LoadedEnblocHistoryArchive
    {
        public long Id { get; set; }
        public string Vessel { get; set; }
        public string Voyage { get; set; }
        public string EnblocNumber { get; set; }
        public string AgentName { get; set; }
        public string ViaNo { get; set; }
        public string Date { get; set; }
        public long TransactionId { get; set; }
        public string Status { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime? StartDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? HistoryBy { get; set; }
        public DateTime? HistoryDate { get; set; }
        public int? ArchivedBy { get; set; }
        public DateTime ArchivedDate { get; set; }
    }
}
