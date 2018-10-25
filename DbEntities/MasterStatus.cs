using System;
using System.Collections.Generic;

namespace Enbloc.DbEntities
{
    public partial class MasterStatus
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Scope { get; set; }
        public sbyte IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
