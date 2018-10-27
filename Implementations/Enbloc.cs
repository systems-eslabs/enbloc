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
                        baseObject = validateMail(email);
                        if (baseObject.Success)
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
                                    obj.Add("transactionNo", Convert.ToString(email.TransactionId));
                                    obj.Add("errors", "Email Subject is not recognized.");
                                    baseObject.Code = (int)EnumTemplateCode.ErrorOccuredEmail;
                                    baseObject.Data = obj;
                                    baseObject.Success = false;
                                    break;
                            }
                        }
                        ReplyToEmail(email, baseObject);
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

        private BaseReturn<Dictionary<string, string>> validateMail(Email email)
        {
            BaseReturn<Dictionary<string, string>> baseObject = new BaseReturn<Dictionary<string, string>>();
            Dictionary<string, string> obj = new Dictionary<string, string>();
            obj.Add("transactionNo", Convert.ToString(email.TransactionId));

            try
            {
                MailAddress address = new MailAddress(email.From);

                if (Config.enblocwhitelistDomains.Split(",").Any(id => id == address.Host))
                {
                    obj.Add("errors", "Email Domain Not Listed With Empezar.");
                    baseObject.Success = false;
                    baseObject.Code = (int)EnumTemplateCode.ErrorOccuredEmail;
                    baseObject.Data = obj;
                    return baseObject;
                }

                if (Config.enblocwhitelistEmailIds.Split(",").Any(id => id == email.From.ToLower()))
                {
                    obj.Add("errors", "Email Id Not Listed With Empezar.");
                    baseObject.Success = false;
                    baseObject.Code = (int)EnumTemplateCode.ErrorOccuredEmail;
                    baseObject.Data = obj;
                    return baseObject;
                }

                if (new EmailService.MailService().getEmailCountByEmailId(email.From) > 25)
                {
                    obj.Add("errors", "Email exceeded daily limit.");
                    baseObject.Success = false;
                    baseObject.Code = (int)EnumTemplateCode.ErrorOccuredEmail;
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
            Dictionary<string, string> obj = new Dictionary<string, string>();
            baseObject.Data.ToList().ForEach(x =>
                    {
                        obj.Add("[{{" + x.Key + "}}]", x.Value);
                    });
            baseObject.Data = obj;

            string errors = "";
            switch ((EnumTemplateCode)baseObject.Code)
            {
                case EnumTemplateCode.ErrorOccured:
                    template = "enbloc/Templates/ErrorOccured.html";
                    break;
                case EnumTemplateCode.ErrorOccuredExcel:
                    errors = "<li>" + baseObject.Data.Where(x => x.Key.StartsWith("[{{errors")).Select(x => x.Value).Distinct().Aggregate((y, z) => y + "</li><li>" + z) + "</li>";
                    baseObject.Data.Add("[{{errors}}]", errors);
                    template = "enbloc/Templates/ErrorOccuredExcel.html";
                    break;
                case EnumTemplateCode.ErrorOccuredEmail:
                    errors = "<li>" + baseObject.Data.Where(x => x.Key.StartsWith("[{{errors")).Select(x => x.Value).Distinct().Aggregate((y, z) => y + "</li><li>" + z) + "</li>";
                    baseObject.Data.Add("[{{errors}}]", errors);
                    template = "enbloc/Templates/ErrorOccuredEmail.html";
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