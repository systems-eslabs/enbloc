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
                                    obj.Add("[{{transactionNo}}]",Convert.ToString(email.TransactionId));
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

            ReplyToEmail(email, baseObject);
            success = true;
            return success;
        }

        public void ReplyToEmail(Email email, BaseReturn<Dictionary<string, string>> baseObject)
        {
            string replyMessage = "";
            string template = getTemplate((EnumTemplateCode)baseObject.Code);
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


        private static string getTemplate(EnumTemplateCode templateCode)
        {
            string template = "";
            switch (templateCode)
            {
                case EnumTemplateCode.ErrorOccured:
                    template = @"../Templates/ErrorOccured.html";
                    break;
                case EnumTemplateCode.EmailIdNotListed:
                    template = @"../Templates/EmailIdNotListed.html";
                    break;
                case EnumTemplateCode.MaxEmailLimitReached:
                    template = @"../Templates/MaxEmailLimitReached.html";
                    break;
                case EnumTemplateCode.InvalidEmailSubject:
                    template = @"../Templates/InvalidEmailSubject.html";
                    break;
                case EnumTemplateCode.NoExcelAttachment:
                    template = @"../Templates/NoAttachment.html";
                    break;
                case EnumTemplateCode.ExcelNoRowsLimitReached:
                    template = @"../Templates/ExcelNoRowsLimitReached.html";
                    break;
                case EnumTemplateCode.InvalidExcelFormat:
                    template = @"../Templates/InvalidExcelFormat.html";
                    break;
                case EnumTemplateCode.EmailProcessed:
                    template = @"../Templates/EmailProcessed.html";
                    break;
                default:
                    template = "";
                    break;
            }
            return File.ReadAllText(template);

        }



    }
}