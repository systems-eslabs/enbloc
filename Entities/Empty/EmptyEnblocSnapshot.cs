using System;
using System.Collections.Generic;
using Enbloc.DbEntities;
using FluentValidation;
using Common;

namespace Enbloc.Entities
{
    public class EmptyEnblocSnapshot
    {
        public long Id { get; set; }
        public string TransactionId { get; set; }
        public string Vessel { get; set; }
        public string ViaNo { get; set; }
        public string ContainerNo { get; set; }
        public string ContainerSize { get; set; }
        public string ContainerType { get; set; }
        public string IsoCode { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class EmptyEnblocValidator : AbstractValidator<EmptyEnblocSnapshot>
    {
        public EmptyEnblocValidator()
        {
            RuleFor(enbloc => enbloc.Vessel).NotEmpty().WithMessage("Vessel field can not be empty");
            RuleFor(enbloc => enbloc.ViaNo).NotEmpty().WithMessage("Via No. field can not be empty");
            RuleFor(enbloc => enbloc.ContainerNo).Length(11).WithMessage("Container Number field should be 11 digit");
            RuleFor(enbloc => enbloc.ContainerNo).Must(IsChecksumMatched).WithMessage("Container Number does not match ISO 6346 Standards");

        }
        
        protected bool IsChecksumMatched(string containerNo)
        {
            var checkSum = CommonFunctions.calculateChecksum(containerNo);
            return checkSum.IsChecksumMatched;
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
