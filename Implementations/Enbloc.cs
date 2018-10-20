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

namespace Enbloc
{
    public class Enbloc
    {
        Mail mailService = null;

        public Enbloc()
        {
            this.mailService = new Mail();
        }

        public void processUnreadEmails()
        {
            try
            {
                //Get All Unread Emails
                var baseEmails = mailService.getUnreadEmailsByLabel("INBOX");

                //Log object baseEmails here -- use async method for logging 
                if (baseEmails.Success)
                {
                    BaseReturn<Dictionary<string, string>> baseObject = new BaseReturn<Dictionary<string, string>>();

                    //Parallel.ForEach(baseEmails.Data, email =>
                    baseEmails.Data.ForEach(email =>
                    {
                        if (validateMail(email))
                        {
                            switch (email.Subject.Trim().ToLower())
                            {
                                case "enbloc.empty":
                                    baseObject = new Empty(mailService).processEmail(email);
                                    break;
                                case "enbloc.loaded":
                                    baseObject = new Loaded(mailService).processEmail(email);
                                    break;
                                default:
                                    Dictionary<string, string> obj = new Dictionary<string, string>();
                                    obj.Add("[{{transactionNo}}]", Convert.ToString(email.TransactionId));
                                    baseObject.Code = (int)EnumTemplateCode.InvalidEmailSubject;
                                    baseObject.Data = obj;
                                    break;
                            }
                            ReplyToEmail(email, baseObject);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                //Log exception
            }
            finally
            {
                mailService.Dispose();
            }

        }

        private bool validateMail(Email email)
        {
            bool success = false;
            BaseReturn<Dictionary<string, string>> baseObject = new BaseReturn<Dictionary<string, string>>();
            Dictionary<string, string> obj = new Dictionary<string, string>();

            //EmailNotWhiteListed,
            //MaxEmailLimitReached,

            //ReplyToEmail(email, baseObject);
            success = true;
            return success;
        }

        public void ReplyToEmail(Email email, BaseReturn<Dictionary<string, string>> baseObject)
        {
            string replyMessage = "";
            string template = getTemplate(baseObject);
            StringBuilder result = new StringBuilder(template);
            if (baseObject.Data != null)
            {
                baseObject.Data.ToList().ForEach(obj =>
                {
                    result.Replace(obj.Key, obj.Value);
                });
            }
            replyMessage = result.ToString();
            mailService.sendMailReply(email, replyMessage);
        }


        private static string getTemplate(BaseReturn<Dictionary<string, string>> baseObject)
        {
            string template = "";
            switch ((EnumTemplateCode)baseObject.Code)
            {
                case EnumTemplateCode.ErrorOccured:
                    template = "enbloc/Templates/ErrorOccured.html";
                    break;
                case EnumTemplateCode.EmailIdNotListed:
                    template = "enbloc/Templates/EmailIdNotListed.html";
                    break;
                case EnumTemplateCode.MaxEmailLimitReached:
                    template = "enbloc/Templates/MaxEmailLimitReached.html";
                    break;
                case EnumTemplateCode.InvalidEmailSubject:
                    template = "enbloc/Templates/InvalidEmailSubject.html";
                    break;
                case EnumTemplateCode.NoExcelAttachment:
                    template = "enbloc/Templates/NoAttachment.html";
                    break;
                case EnumTemplateCode.ExcelNoRowsLimitReached:
                    template = "enbloc/Templates/ExcelNoRowsLimitReached.html";
                    break;
                case EnumTemplateCode.InvalidExcelFormat:
                    string errors = "<li>" + baseObject.Data.Select(x => x.Value).Distinct().Aggregate((y, z) => y + "</li><li>" + z) + "</li>";
                    baseObject.Data.Add("[{{errors}}]",errors);
                    template = "enbloc/Templates/InvalidExcelFormat.html";
                    break;
                case EnumTemplateCode.EmailProcessed:
                    template = "enbloc/Templates/EmailProcessed.html";
                    break;
                default:
                    template = "";
                    break;
            }
            return File.ReadAllText(template);

        }



    }
}