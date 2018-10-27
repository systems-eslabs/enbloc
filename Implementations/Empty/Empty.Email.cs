using System;
using System.Collections.Generic;
using EmailService;
using OfficeOpenXml;
using Common;
using System.IO;
using System.Linq;
using Enbloc.DbEntities;
using FluentValidation.Results;
using Enbloc.Entities;


namespace Enbloc
{
    public partial class Empty : EnblocBase
    {
        Mail mailService = null;

        public Empty(Mail mailService) : base(mailService)
        {
            this.mailService = mailService;
        }

        public BaseReturn<Dictionary<string, string>> processEmail(Email email)
        {
            return base.processEmail<EmptyEnblocSnapshot>(email);
        }

        protected override void ProcessEnbloc<T>(FileInfo file, string programCode, int transactionId, IEnumerable<T> baselstEnblocSnapshot)
        {
            var lstEnblocSnapshot = (List<EmptyEnblocSnapshot>)baselstEnblocSnapshot;

            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                int rowCount = worksheet.Dimension.Rows;
                int ColCount = worksheet.Dimension.Columns;

                string transaction_no = programCode + transactionId;

                for (int row = 8; row <= rowCount; row++)
                {
                    if (Convert.ToString(worksheet.Cells[row, 1].Value).Trim() != "")
                    {
                        EmptyEnblocSnapshot enblocSnapshot = new EmptyEnblocSnapshot();
                        enblocSnapshot.TransactionId = transaction_no;
                        enblocSnapshot.Vessel = Convert.ToString(worksheet.Cells[row, GetColumnIndexByName(worksheet, "Vsl Name(D)")].Value).Trim();
                        enblocSnapshot.ViaNo = Convert.ToString(worksheet.Cells[row, GetColumnIndexByName(worksheet, "VIA(D)")].Value).Trim();
                        enblocSnapshot.EnblocNumber = enblocSnapshot.ViaNo.ToUpper();
                        enblocSnapshot.ContainerNo = Convert.ToString(worksheet.Cells[row, GetColumnIndexByName(worksheet, "Container Number")].Value).Trim();
                        enblocSnapshot.ContainerSize = Convert.ToString(worksheet.Cells[row, GetColumnIndexByName(worksheet, "CtrSize")].Value).Trim();
                        enblocSnapshot.ContainerType = Convert.ToString(worksheet.Cells[row, GetColumnIndexByName(worksheet, "CtrType")].Value).Trim();
                        enblocSnapshot.IsoCode = Convert.ToString(worksheet.Cells[row, GetColumnIndexByName(worksheet, "ISO")].Value).Trim();
                        enblocSnapshot.CreatedBy = 0;

                        lstEnblocSnapshot.Add(enblocSnapshot);
                    }
                }
            }
            //return lstEnblocSnapshot;
        }


        protected override BaseReturn<Dictionary<string, string>> SaveEnblocToDB<T>(Email email, IEnumerable<T> baselstEnblocSnapshot)
        {
            BaseReturn<Dictionary<string, string>> baseObject = new BaseReturn<Dictionary<string, string>>();
            Dictionary<string, string> obj = new Dictionary<string, string>();
            obj.Add("transactionNo", Convert.ToString(email.TransactionId));

            try
            {
                var lstEnblocSnapshot = (List<EmptyEnblocSnapshot>)baselstEnblocSnapshot;
                var enblocFromSnapshot = lstEnblocSnapshot.First();
                string enblocno = enblocFromSnapshot.Vessel.Replace(" ", "") + enblocFromSnapshot.ViaNo.ToString();

                EmptyEnbloc objEnbloc = new EmptyEnbloc()
                {
                    Vessel = enblocFromSnapshot.Vessel,
                    Voyage = "",
                    EnblocNumber = enblocno,
                    ViaNo = enblocFromSnapshot.ViaNo,
                    TransactionId = enblocFromSnapshot.TransactionId,
                    Status = Status.PENDING,
                    CreatedBy = 0
                };
                //Save to DB
                new EmpezarRepository<EmptyEnbloc>().Add(objEnbloc);

                List<EmptyEnblocContainers> lstEmptyEnblocContainers = new List<EmptyEnblocContainers>();
                lstEnblocSnapshot.ForEach(enblocContainer =>
                {
                    lstEmptyEnblocContainers.Add(new EmptyEnblocContainers()
                    {
                        TransactionId = enblocContainer.TransactionId,
                        Vessel = enblocContainer.Vessel,
                        Voyage = "",
                        ViaNo = enblocContainer.ViaNo,
                        EnblocNumber = enblocno,
                        ContainerNo = enblocContainer.ContainerNo,
                        ContainerSize = Convert.ToInt16(enblocContainer.ContainerSize),
                        ContainerType = enblocContainer.ContainerType,
                        IsoCode = enblocContainer.IsoCode,
                        Status = Status.PENDING,
                        CreatedBy = 0
                    });
                });
                //Save to DB
                new EmpezarRepository<EmptyEnblocContainers>().AddRange(lstEmptyEnblocContainers);

                baseObject.Success = true;
                baseObject.Code = (int)EnumTemplateCode.EmailProcessed;
                baseObject.Data = obj;
            }
            catch (Exception ex)
            {
                baseObject.Success = false;
                baseObject.Code = (int)EnumTemplateCode.ErrorOccured;
                baseObject.Data = obj;
            }
            return baseObject;
        }

        // protected override bool IsEnblocExists<T>(IEnumerable<T> lstEnblocSnapshot)
        // {
        //     var enblocData = ((List<EmptyEnblocSnapshot>)lstEnblocSnapshot).First();
        //     return new EmpezarRepository<EmptyEnbloc>().IsExists(x => x.EnblocNumber == enblocData.EnblocNumber && x.Status != Status.COMPLETED);
        // }

        protected override BaseReturn<Dictionary<string, string>> ValidateOtherData<T>(Email email, IEnumerable<T> lstEnblocSnapshot)
        {
            BaseReturn<Dictionary<string, string>> baseObject = new BaseReturn<Dictionary<string, string>>();
            Dictionary<string, string> obj = new Dictionary<string, string>();
            obj.Add("transactionNo", Convert.ToString(email.TransactionId));

            var lstenbloc = ((List<EmptyEnblocSnapshot>)lstEnblocSnapshot);
            var IsEnblocExists = new EmpezarRepository<EmptyEnbloc>().IsExists(x => x.EnblocNumber == lstenbloc.First().EnblocNumber && x.Status != Status.COMPLETED);
            if (!IsEnblocExists)
            {
                obj.Add("errors" + Guid.NewGuid().ToString(), "Enbloc already exists in the system");
                baseObject.Success = false;
                baseObject.Code = (int)EnumTemplateCode.ErrorOccuredExcel;
                baseObject.Data = obj;
                return baseObject;
            }
            
            if (!(lstenbloc.Select(x => x.ViaNo).Distinct().Count() == lstenbloc.Count))
            {
                obj.Add("errors" + Guid.NewGuid().ToString(), "Duplicate Via No. not allowed.");
                baseObject.Success = false;
                baseObject.Code = (int)EnumTemplateCode.ErrorOccuredExcel;
                baseObject.Data = obj;
                return baseObject;
            }
            baseObject.Success = true;
            return baseObject;
        }


        protected override ValidationResult ValidateEnblocData<T>(IEnumerable<T> lstEnblocSnapshot)
        {
            EmptyEnblocValidatorCollectionValidator validator = new EmptyEnblocValidatorCollectionValidator();
            ValidationResult results = validator.Validate((List<EmptyEnblocSnapshot>)lstEnblocSnapshot);
            return results;
        }


    }



}