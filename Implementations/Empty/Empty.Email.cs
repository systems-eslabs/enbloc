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
    public partial class Empty
    {
        Mail mailService = null;

        public Empty(Mail mailService)
        {
            this.mailService = mailService;
        }

        public BaseReturn<Dictionary<string, string>> processEmail(Email email)
        {
            BaseReturn<Dictionary<string, string>> baseObject = new BaseReturn<Dictionary<string, string>>();
            List<EmptyEnblocSnapshot> lstEnblocSnapshot = new List<EmptyEnblocSnapshot>();
            baseObject.Success = false;

            baseObject = GetEmailAttachments(email);
            if (baseObject.Success)
            {
                baseObject = ProcessEmailAttachments(email, lstEnblocSnapshot);
            }

            if (baseObject.Success)
            {
                baseObject = ValidateEnbloc(email, lstEnblocSnapshot);
            }

            if (baseObject.Success)
            {
                baseObject = SaveEnblocToDB(email, lstEnblocSnapshot);
            }
            return baseObject;
        }

        private BaseReturn<Dictionary<string, string>> GetEmailAttachments(Email email)
        {
            BaseReturn<Dictionary<string, string>> baseObject = new BaseReturn<Dictionary<string, string>>();
            Dictionary<string, string> obj = new Dictionary<string, string>();
            obj.Add("transactionNo", Convert.ToString(email.TransactionId));

            try
            {
                if (!email.Attachments.Any())
                {
                    baseObject.Success = false;
                    obj.Add("errors", "No attachment(s) found.");
                    baseObject.Code = (int)EnumTemplateCode.ErrorOccuredEmail;
                    baseObject.Data = obj;
                    return baseObject;
                }

                var attachments = email.Attachments.Where(attachment => attachment.Filename.ToLower().EndsWith(FileType.XLSX)).ToList();
                if (attachments.Count != 1)
                {
                    baseObject.Success = false;
                    obj.Add("errors", "Email should contain exactly one excel attachment.");
                    baseObject.Code = (int)EnumTemplateCode.ErrorOccuredEmail;
                    baseObject.Data = obj;
                    return baseObject;
                }

                List<EAttachmentRequest> attachmentRequest = attachments.Select(attachment => new EAttachmentRequest
                {
                    AttachmentId = attachment.AttachmentId,
                    Filename = attachment.Filename
                }).ToList();

                email.Attachments = mailService.getAttachments(email, attachmentRequest).Data;
                baseObject.Success = true;

            }
            catch (Exception ex)
            {
                baseObject.Success = false;
                baseObject.Code = (int)EnumTemplateCode.ErrorOccured;
                baseObject.Data = obj;
            }
            return baseObject;
        }


        private static BaseReturn<Dictionary<string, string>> ProcessEmailAttachments(Email email, List<EmptyEnblocSnapshot> lstEnblocSnapshot)
        {
            BaseReturn<Dictionary<string, string>> baseObject = new BaseReturn<Dictionary<string, string>>();
            Dictionary<string, string> obj = new Dictionary<string, string>();
            obj.Add("transactionNo", Convert.ToString(email.TransactionId));

            try
            {
                FileInfo file = new FileInfo(email.Attachments.First().localUrl);
                ProcessEnbloc(file, "EM", email.TransactionId, lstEnblocSnapshot);
                baseObject.Success = true;
            }
            catch (Exception ex)
            {
                baseObject.Success = false;
                baseObject.Code = (int)EnumTemplateCode.ErrorOccured;
                baseObject.Data = obj;
            }
            return baseObject;
        }

        private static void ProcessEnbloc(FileInfo file, string programCode, int transactionId, List<EmptyEnblocSnapshot> lstEnblocSnapshot)
        {
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

        private static BaseReturn<Dictionary<string, string>> ValidateEnbloc(Email email, List<EmptyEnblocSnapshot> lstEnblocSnapshot)
        {
            BaseReturn<Dictionary<string, string>> baseObject = new BaseReturn<Dictionary<string, string>>();
            Dictionary<string, string> obj = new Dictionary<string, string>();
            obj.Add("transactionNo", Convert.ToString(email.TransactionId));

            try
            {
                EmptyEnblocValidatorCollectionValidator validator = new EmptyEnblocValidatorCollectionValidator();
                if (lstEnblocSnapshot.Count > 1000)
                {
                    obj.Add("errors", "Maximum 1000 rows are allowed in the Excel Attachment.");
                    baseObject.Success = false;
                    baseObject.Code = (int)EnumTemplateCode.ErrorOccuredExcel;
                    baseObject.Data = obj;
                    return baseObject;
                }

                // // if vessel voyage no already exists and enbloc in progress then no processing 
                // if (IsVesselVoyageExists(lstEnblocSnapshot.First().Vessel, ""))
                // {
                //     obj.Add("0", "Vessel Voyage already exists");
                //     baseObject.Success = false;
                //     baseObject.Code = (int)EnumTemplateCode.ErrorOccuredExcel;
                //     baseObject.Data = obj;
                //     return baseObject;
                // }

                ValidationResult results = validator.Validate(lstEnblocSnapshot);
                if (!results.IsValid)
                {
                    int selectIndex = 0;
                    results.Errors.Select(result => result.ErrorMessage + ", For value '" + result.AttemptedValue + "'").Distinct().ToList().ForEach(error =>
                     {
                         obj.Add(Convert.ToString(selectIndex++), error);
                     });
                    baseObject.Success = false;
                    baseObject.Code = (int)EnumTemplateCode.ErrorOccuredExcel;
                    baseObject.Data = obj;
                    return baseObject;
                }

                baseObject.Success = true;
            }
            catch (Exception ex)
            {
                baseObject.Success = false;
                baseObject.Code = (int)EnumTemplateCode.ErrorOccured;
                baseObject.Data = obj;
            }
            return baseObject;
        }

        private static BaseReturn<Dictionary<string, string>> SaveEnblocToDB(Email email, List<EmptyEnblocSnapshot> lstEnblocSnapshot)
        {
            BaseReturn<Dictionary<string, string>> baseObject = new BaseReturn<Dictionary<string, string>>();
            Dictionary<string, string> obj = new Dictionary<string, string>();
            obj.Add("transactionNo", Convert.ToString(email.TransactionId));

            try
            {
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
                        ViaNo = enblocContainer.ViaNo,
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

        private static bool IsVesselVoyageExists(string vessel, string voyage)
        {
            string vesselno = vessel.Split(' ').ToList().Aggregate((x, y) => x.Trim() + y.Trim()) + voyage;
            return new EmpezarRepository<EmptyEnbloc>().IsExists(x => x.VesselNo == vesselno && x.Status != Status.COMPLETED);
        }


        private static int GetColumnIndexByName(ExcelWorksheet ws, string columnName)
        {
            return ws.Cells["1:1"].First(c => c.Value.ToString() == columnName).Start.Column;
        }

    }



}