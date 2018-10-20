using System;
using System.Collections.Generic;
using FluentValidation;

namespace enbloc.Entities
{
    public class EmptyEnblocSnapshot
    {
        public long Id { get; set; }
        public string TransactionId { get; set; }
        public string Vessel { get; set; }
        public string Voyage { get; set; }
        public string AgentName { get; set; }
        public string ViaNo { get; set; }
        public string Date { get; set; }
        public string Srl { get; set; }
        public string ContainerNo { get; set; }
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
    }

       public class EmptyEnblocSnapshotValidator : AbstractValidator<EmptyEnblocSnapshot>
    {
        public EmptyEnblocSnapshotValidator()
        {
            RuleFor(enbloc => enbloc.Vessel).NotEmpty();
        }
    }
}
