using System;
using System.Collections.Generic;
using enbloc.DbEntities;
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
        public string DepotName { get; set; }
        public string ViaNo { get; set; }
        public string PermissionDate { get; set; }
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

    public class EmptyEnblocValidator : AbstractValidator<EmptyEnblocSnapshot>
    {
        public EmptyEnblocValidator()
        {
            RuleFor(enbloc => enbloc.Vessel).NotEmpty().WithMessage("Vessel can't be empty");
            RuleFor(enbloc => enbloc.ContainerNo).Length(11).WithMessage("Container Number should have length 11");
        }
    }

    public class EmptyEnblocValidatorCollectionValidator : AbstractValidator<IEnumerable<EmptyEnblocSnapshot>>
    {
        public EmptyEnblocValidatorCollectionValidator()
        {
            RuleFor(x => x).SetCollectionValidator(new EmptyEnblocValidator());
        }
    }
}
