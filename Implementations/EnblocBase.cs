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

        protected EnblocBase(Mail mailService)
        {
            this.mailService = mailService;
        }


        protected BaseReturn<Dictionary<string, string>> processEmail<T>(Email email)
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
                    obj.Add("errors" + Guid.NewGuid().ToString(), "No attachment(s) found.");
                    baseObject.Code = (int)EnumTemplateCode.ErrorOccuredEmail;
                    baseObject.Data = obj;
                    return baseObject;
                }

                var attachments = email.Attachments.Where(attachment => attachment.Filename.ToLower().EndsWith(FileType.XLSX) || attachment.Filename.ToLower().EndsWith(FileType.XLS)).ToList();
                if (attachments.Count != 1)
                {
                    baseObject.Success = false;
                    obj.Add("errors" + Guid.NewGuid().ToString(), "Email should contain exactly one excel attachment.");
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


        protected BaseReturn<Dictionary<string, string>> ProcessEmailAttachments<T>(Email email, List<T> lstEnblocSnapshot)
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


        protected BaseReturn<Dictionary<string, string>> ValidateEnbloc<T>(Email email, List<T> lstEnblocSnapshot)
        {
            BaseReturn<Dictionary<string, string>> baseObject = new BaseReturn<Dictionary<string, string>>();
            Dictionary<string, string> obj = new Dictionary<string, string>();
            obj.Add("transactionNo", Convert.ToString(email.TransactionId));

            try
            {
                if (lstEnblocSnapshot.Count == 0)
                {
                    obj.Add("errors" + Guid.NewGuid().ToString(), "Excel Attachment does not contain any container data.");
                    baseObject.Success = false;
                    baseObject.Code = (int)EnumTemplateCode.ErrorOccuredExcel;
                    baseObject.Data = obj;
                    return baseObject;
                }

                if (lstEnblocSnapshot.Count > 1000)
                {
                    obj.Add("errors" + Guid.NewGuid().ToString(), "Maximum 1000 rows are allowed in the Excel Attachment.");
                    baseObject.Success = false;
                    baseObject.Code = (int)EnumTemplateCode.ErrorOccuredExcel;
                    baseObject.Data = obj;
                    return baseObject;
                }

                //Add  other specific validations
                var IsOtherValid = ValidateOtherData(email, lstEnblocSnapshot);
                if (!IsOtherValid.Success)
                {
                    baseObject.Success = false;
                    baseObject.Code = (int)EnumTemplateCode.ErrorOccuredExcel;
                    baseObject.Data = IsOtherValid.Data;
                    return baseObject;
                }


                ValidationResult results = ValidateEnblocData(lstEnblocSnapshot);
                if (!results.IsValid)
                {
                    results.Errors.ToList().ForEach(result =>
                    {
                        obj.Add("errors" + Guid.NewGuid().ToString(), result.ErrorMessage + ", For value '" + result.AttemptedValue + "'");
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


        protected abstract void ProcessEnbloc<T>(FileInfo file, string programCode, int transactionId, IEnumerable<T> lstEnblocSnapshot);

        protected abstract BaseReturn<Dictionary<string, string>> SaveEnblocToDB<T>(Email email, IEnumerable<T> lstEnblocSnapshot);

        protected abstract ValidationResult ValidateEnblocData<T>(IEnumerable<T> lstEnblocSnapshot);

        // protected abstract bool IsEnblocExists<T>(IEnumerable<T> lstEnblocSnapshot);

        protected abstract BaseReturn<Dictionary<string, string>> ValidateOtherData<T>(Email email, IEnumerable<T> lstEnblocSnapshot);

        protected static int GetColumnIndexByName(ExcelWorksheet ws, string columnName)
        {
            return ws.Cells["1:1"].First(c => c.Value.ToString() == columnName).Start.Column;
        }
    }
}