using System;
using System.Collections.Generic;
using Enbloc.DbEntities;
using FluentValidation;
using Common;

namespace Enbloc.Entities
{
    public class LoadedEnblocSnapshot
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

    public class LoadedEnblocValidator : AbstractValidator<LoadedEnblocSnapshot>
    {
        public LoadedEnblocValidator()
        {
            RuleFor(enbloc => enbloc.Vessel).NotEmpty().WithMessage("Vessel field can not be empty");
            RuleFor(enbloc => enbloc.Voyage).NotEmpty().WithMessage("ISO field can not be empty");
            RuleFor(enbloc => enbloc.ContainerNo).Length(11).WithMessage("Container Number field should be 11 digit");
            RuleFor(enbloc => enbloc.ContainerNo).Must(IsChecksumMatched).WithMessage("Container Number does not match ISO 6346 Standards");
        }

        protected bool IsChecksumMatched(string containerNo)
        {
            var checkSum = CommonFunctions.calculateChecksum(containerNo);
            return checkSum.IsChecksumMatched;
        }
    }

    public class LoadedEnblocValidatorCollectionValidator : AbstractValidator<IEnumerable<LoadedEnblocSnapshot>>
    {
        public LoadedEnblocValidatorCollectionValidator()
        {
            RuleFor(x => x).SetCollectionValidator(new LoadedEnblocValidator());
        }
    }
}
