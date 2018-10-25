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

        private new BaseReturn<Dictionary<string, string>> GetEmailAttachments(Email email)
        {
            return base.GetEmailAttachments(email);
        }

        private BaseReturn<Dictionary<string, string>> ProcessEmailAttachments(Email email, List<EmptyEnblocSnapshot> lstEnblocSnapshot)
        {
            return base.ProcessEmailAttachments<EmptyEnblocSnapshot>(email, lstEnblocSnapshot);
        }

        private BaseReturn<Dictionary<string, string>> ValidateEnbloc(Email email, List<EmptyEnblocSnapshot> lstEnblocSnapshot)
        {
            return base.ValidateEnbloc(email, lstEnblocSnapshot);
        }

        public override void ProcessEnbloc<T>(FileInfo file, string programCode, int transactionId, IEnumerable<T> baselstEnblocSnapshot)
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


        public override BaseReturn<Dictionary<string, string>> SaveEnblocToDB<T>(Email email, IEnumerable<T> baselstEnblocSnapshot)
        {            
            BaseReturn<Dictionary<string, string>> baseObject = new BaseReturn<Dictionary<string, string>>();
            Dictionary<string, string> obj = new Dictionary<string, string>();
            obj.Add("transactionNo", Convert.ToString(email.TransactionId));

            try
            {
                var lstEnblocSnapshot = (List<EmptyEnblocSnapshot>)baselstEnblocSnapshot;
                var enblocFromSnapshot = lstEnblocSnapshot.First();
                string vesselno = enblocFromSnapshot.Vessel.Replace(" ", "") + enblocFromSnapshot.ViaNo.ToString();

                EmptyEnbloc objEnbloc = new EmptyEnbloc()
                {
                    Vessel = enblocFromSnapshot.Vessel,
                    Voyage = "",
                    VesselNo = vesselno,
                    ViaNo = enblocFromSnapshot.ViaNo,
                    TransactionId = enblocFromSnapshot.TransactionId,
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
                        //ViaNo = enblocContainer.ViaNo,
                        VesselNo = vesselno,
                        ContainerNo = enblocContainer.ContainerNo,
                        ContainerSize = Convert.ToInt16(enblocContainer.ContainerSize),
                        ContainerType = enblocContainer.ContainerType,
                        IsoCode = enblocContainer.IsoCode,
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

        public override bool IsVesselVoyageExists(string vessel, string voyage)
        {
            string vesselno = vessel.Split(' ').ToList().Aggregate((x, y) => x.Trim() + y.Trim()) + voyage;
            return new EmpezarRepository<EmptyEnbloc>().IsExists(x => x.VesselNo == vesselno);
        }


        public override ValidationResult ValidateEnblocData<T>(IEnumerable<T> lstEnblocSnapshot)
        {
            EmptyEnblocValidatorCollectionValidator validator = new EmptyEnblocValidatorCollectionValidator();
            ValidationResult results = validator.Validate((List<EmptyEnblocSnapshot>)lstEnblocSnapshot);
            return results;
        }


    }



}