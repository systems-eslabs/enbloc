using System;
using System.Collections.Generic;

namespace enbloc.DbEntity.Classes
{
    public partial class MasterStatus
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Scope { get; set; }
        public sbyte IsActive { get; set; }
        public DateTime CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedBy { get; set; }
        public DateTime? ArchivedDate { get; set; }
    }
}
