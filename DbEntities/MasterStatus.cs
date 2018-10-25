using System;
using System.Collections.Generic;

namespace Enbloc.DbEntities
{
    public partial class MasterStatus
    {
        public MasterStatus()
        {
            EmptyEnbloc = new HashSet<EmptyEnbloc>();
            EmptyEnblocArchive = new HashSet<EmptyEnblocArchive>();
            EmptyEnblocContainers = new HashSet<EmptyEnblocContainers>();
            EmptyEnblocContainersHistory = new HashSet<EmptyEnblocContainersHistory>();
            EmptyEnblocContainersHistoryArchive = new HashSet<EmptyEnblocContainersHistoryArchive>();
            EmptyEnblocHistory = new HashSet<EmptyEnblocHistory>();
            EmptyEnblocHistoryArchive = new HashSet<EmptyEnblocHistoryArchive>();
            LoadedEnbloc = new HashSet<LoadedEnbloc>();
            LoadedEnblocArchive = new HashSet<LoadedEnblocArchive>();
            LoadedEnblocContainers = new HashSet<LoadedEnblocContainers>();
            LoadedEnblocContainersHistory = new HashSet<LoadedEnblocContainersHistory>();
            LoadedEnblocContainersHistoryArchive = new HashSet<LoadedEnblocContainersHistoryArchive>();
            LoadedEnblocHistory = new HashSet<LoadedEnblocHistory>();
            LoadedEnblocHistoryArchive = new HashSet<LoadedEnblocHistoryArchive>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Scope { get; set; }
        public sbyte IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public ICollection<EmptyEnbloc> EmptyEnbloc { get; set; }
        public ICollection<EmptyEnblocArchive> EmptyEnblocArchive { get; set; }
        public ICollection<EmptyEnblocContainers> EmptyEnblocContainers { get; set; }
        public ICollection<EmptyEnblocContainersHistory> EmptyEnblocContainersHistory { get; set; }
        public ICollection<EmptyEnblocContainersHistoryArchive> EmptyEnblocContainersHistoryArchive { get; set; }
        public ICollection<EmptyEnblocHistory> EmptyEnblocHistory { get; set; }
        public ICollection<EmptyEnblocHistoryArchive> EmptyEnblocHistoryArchive { get; set; }
        public ICollection<LoadedEnbloc> LoadedEnbloc { get; set; }
        public ICollection<LoadedEnblocArchive> LoadedEnblocArchive { get; set; }
        public ICollection<LoadedEnblocContainers> LoadedEnblocContainers { get; set; }
        public ICollection<LoadedEnblocContainersHistory> LoadedEnblocContainersHistory { get; set; }
        public ICollection<LoadedEnblocContainersHistoryArchive> LoadedEnblocContainersHistoryArchive { get; set; }
        public ICollection<LoadedEnblocHistory> LoadedEnblocHistory { get; set; }
        public ICollection<LoadedEnblocHistoryArchive> LoadedEnblocHistoryArchive { get; set; }
    }
}
