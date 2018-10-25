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
    public partial class Loaded : EnblocBase
    {
        Mail mailService = null;

        public Loaded(Mail mailService) : base(mailService)
        {
            this.mailService = mailService;
        }

        public BaseReturn<Dictionary<string, string>> processEmail(Email email)
        {
            return base.processEmail<LoadedEnblocSnapshot>(email);
        }

        private new BaseReturn<Dictionary<string, string>> GetEmailAttachments(Email email)
        {
            return base.GetEmailAttachments(email);
        }

        private BaseReturn<Dictionary<string, string>> ProcessEmailAttachments(Email email, List<LoadedEnblocSnapshot> lstEnblocSnapshot)
        {
            return base.ProcessEmailAttachments<LoadedEnblocSnapshot>(email, lstEnblocSnapshot);
        }

        private BaseReturn<Dictionary<string, string>> ValidateEnbloc(Email email, List<LoadedEnblocSnapshot> lstEnblocSnapshot)
        {
            return base.ValidateEnbloc(email, lstEnblocSnapshot);
        }

        public override void ProcessEnbloc<T>(FileInfo file, string programCode, int transactionId, IEnumerable<T> baselstEnblocSnapshot)
        {
            var lstEnblocSnapshot = (List<LoadedEnblocSnapshot>)baselstEnblocSnapshot;
            using (ExcelPackage package = new ExcelPackage(file))
            {

                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                int rowCount = worksheet.Dimension.Rows;
                int ColCount = worksheet.Dimension.Columns;

                string document_date = Convert.ToString(worksheet.Cells["C1"].Value);
                string vessel = Convert.ToString(worksheet.Cells["B4"].Value);
                string voyage = Convert.ToString(worksheet.Cells["D4"].Value);
                string agent_name = Convert.ToString(worksheet.Cells["B5"].Value);
                string via_no = Convert.ToString(worksheet.Cells["D5"].Value);
                string depot_name = Convert.ToString(worksheet.Cells["A3"].Value);
                string transaction_no = programCode + transactionId;

                for (int row = 8; row <= rowCount; row++)
                {
                    if (Convert.ToString(worksheet.Cells[row, 1].Value).Trim() != "")
                    {
                        LoadedEnblocSnapshot enblocSnapshot = new LoadedEnblocSnapshot();
                        enblocSnapshot.TransactionId = transaction_no;
                        enblocSnapshot.Vessel = vessel;
                        enblocSnapshot.Voyage = voyage;
                        enblocSnapshot.AgentName = agent_name;
                        enblocSnapshot.DepotName = depot_name;
                        enblocSnapshot.ViaNo = via_no;
                        enblocSnapshot.PermissionDate = document_date;
                        enblocSnapshot.Srl = Convert.ToString(worksheet.Cells[row, 1].Value).Trim();
                        enblocSnapshot.ContainerNo = Convert.ToString(worksheet.Cells[row, 2].Value).Trim();
                        enblocSnapshot.ContainerType = Convert.ToString(worksheet.Cells[row, 3].Value).Trim();
                        enblocSnapshot.Wt = Convert.ToString(worksheet.Cells[row, 4].Value).Trim();
                        enblocSnapshot.Cargo = Convert.ToString(worksheet.Cells[row, 5].Value).Trim();
                        enblocSnapshot.IsoCode = Convert.ToString(worksheet.Cells[row, 6].Value).Trim();
                        enblocSnapshot.SealNo1 = Convert.ToString(worksheet.Cells[row, 7].Value).Trim();
                        enblocSnapshot.SealNo2 = Convert.ToString(worksheet.Cells[row, 8].Value).Trim();
                        enblocSnapshot.SealNo3 = Convert.ToString(worksheet.Cells[row, 9].Value).Trim();
                        enblocSnapshot.ImdgClass = Convert.ToString(worksheet.Cells[row, 10].Value).Trim();
                        enblocSnapshot.ReferTemrature = Convert.ToString(worksheet.Cells[row, 11].Value).Trim();
                        enblocSnapshot.OogDeatils = Convert.ToString(worksheet.Cells[row, 12].Value).Trim();
                        enblocSnapshot.ContainerGrossDetails = Convert.ToString(worksheet.Cells[row, 13].Value).Trim();
                        enblocSnapshot.CargoDescription = Convert.ToString(worksheet.Cells[row, 14].Value).Trim();
                        enblocSnapshot.BlNumber = Convert.ToString(worksheet.Cells[row, 15].Value).Trim();
                        enblocSnapshot.Name = Convert.ToString(worksheet.Cells[row, 16].Value).Trim();
                        enblocSnapshot.ItemNo = Convert.ToString(worksheet.Cells[row, 17].Value).Trim();
                        enblocSnapshot.DisposalMode = Convert.ToString(worksheet.Cells[row, 18].Value).Trim();
                        enblocSnapshot.CreatedBy = 0;//Convert.ToString(worksheet.Cells[row, 19].Value).Trim();

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
                var lstEnblocSnapshot = (List<LoadedEnblocSnapshot>)baselstEnblocSnapshot;
                var enblocFromSnapshot = lstEnblocSnapshot.First();
                string vesselno = enblocFromSnapshot.Vessel.Split(' ').ToList().Aggregate((x, y) => x.Trim() + y.Trim()) + enblocFromSnapshot.Voyage.ToString();

                LoadedEnbloc objEnbloc = new LoadedEnbloc()
                {
                    Vessel = enblocFromSnapshot.Vessel,
                    Voyage = enblocFromSnapshot.Voyage,
                    VesselNo = vesselno,
                    AgentName = enblocFromSnapshot.AgentName,
                    ViaNo = enblocFromSnapshot.ViaNo,
                    PermissionDate = enblocFromSnapshot.PermissionDate,
                    DepotName = enblocFromSnapshot.DepotName,
                    TransactionId = enblocFromSnapshot.TransactionId,
                    CreatedBy = 0
                };
                //Save to DB
                new EmpezarRepository<LoadedEnbloc>().Add(objEnbloc);


                List<LoadedEnblocContainers> lstLoadedEnblocContainers = new List<LoadedEnblocContainers>();
                lstEnblocSnapshot.ForEach(enblocContainer =>
                {
                    lstLoadedEnblocContainers.Add(new LoadedEnblocContainers()
                    {
                        TransactionId = enblocContainer.TransactionId,
                        Vessel = enblocContainer.Vessel,
                        Voyage = enblocContainer.Voyage,
                        VesselNo = vesselno,
                        Srl = enblocContainer.Srl,
                        ContainerNo = enblocContainer.ContainerNo,
                        ContainerSize = Convert.ToInt16(enblocContainer.ContainerType.Substring(0, 2)),
                        ContainerType = enblocContainer.ContainerType.Substring(2, 2),
                        Wt = enblocContainer.Wt,
                        Cargo = enblocContainer.Cargo,
                        IsoCode = enblocContainer.IsoCode,
                        SealNo1 = enblocContainer.SealNo1,
                        SealNo2 = enblocContainer.SealNo2,
                        SealNo3 = enblocContainer.SealNo3,
                        ImdgClass = enblocContainer.ImdgClass,
                        ReferTemrature = enblocContainer.ReferTemrature,
                        OogDeatils = enblocContainer.OogDeatils,
                        ContainerGrossDetails = enblocContainer.ContainerGrossDetails,
                        CargoDescription = enblocContainer.CargoDescription,
                        BlNumber = enblocContainer.BlNumber,
                        Name = enblocContainer.Name,
                        ItemNo = enblocContainer.ItemNo,
                        DisposalMode = enblocContainer.DisposalMode,
                        CreatedBy = 0
                    });
                });
                //Save to DB
                new EmpezarRepository<LoadedEnblocContainers>().AddRange(lstLoadedEnblocContainers);

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
            return new EmpezarRepository<LoadedEnbloc>().IsExists(x => x.VesselNo == vesselno);
        }

        public override ValidationResult ValidateEnblocData<T>(IEnumerable<T> lstEnblocSnapshot)
        {
            LoadedEnblocValidatorCollectionValidator validator = new LoadedEnblocValidatorCollectionValidator();
            ValidationResult results = validator.Validate((List<LoadedEnblocSnapshot>)lstEnblocSnapshot);
            return results;
        }




    }



}