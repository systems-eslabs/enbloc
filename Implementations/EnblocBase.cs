using System;
using System.Collections.Generic;
using EmailService;
using OfficeOpenXml;
using Common;
using System.IO;
using System.Linq;
using FluentValidation.Results;
using System.Text;
using Enbloc.Entities;
using System.Net.Mail;

namespace Enbloc
{
    public abstract class EnblocBase
    {
        Mail mailService = null;

        public EnblocBase(Mail mailService)
        {
            this.mailService = mailService;
        }


        public BaseReturn<Dictionary<string, string>> processEmail<T>(Email email)
        {
            BaseReturn<Dictionary<string, string>> baseObject = new BaseReturn<Dictionary<string, string>>();
            List<T> lstEnblocSnapshot = new List<T>();
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


        protected BaseReturn<Dictionary<string, string>> GetEmailAttachments(Email email)
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


        public BaseReturn<Dictionary<string, string>> ProcessEmailAttachments<T>(Email email, List<T> lstEnblocSnapshot)
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


        public BaseReturn<Dictionary<string, string>> ValidateEnbloc<T>(Email email, List<T> lstEnblocSnapshot)
        {
            BaseReturn<Dictionary<string, string>> baseObject = new BaseReturn<Dictionary<string, string>>();
            Dictionary<string, string> obj = new Dictionary<string, string>();
            obj.Add("transactionNo", Convert.ToString(email.TransactionId));

            try
            {
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

                ValidationResult results = ValidateEnblocData(lstEnblocSnapshot);
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


        public abstract void ProcessEnbloc<T>(FileInfo file, string programCode, int transactionId, IEnumerable<T> lstEnblocSnapshot);


        public abstract BaseReturn<Dictionary<string, string>> SaveEnblocToDB<T>(Email email, IEnumerable<T> lstEnblocSnapshot);


        public abstract bool IsVesselVoyageExists(string vessel, string voyage);

        public abstract ValidationResult ValidateEnblocData<T>(IEnumerable<T> lstEnblocSnapshot);

        public static int GetColumnIndexByName(ExcelWorksheet ws, string columnName)
        {
            return ws.Cells["1:1"].First(c => c.Value.ToString() == columnName).Start.Column;
        }

    }
}