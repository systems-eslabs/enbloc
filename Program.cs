using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using EmailService;
using OfficeOpenXml;
using Common;
using System.Text;
using Enbloc.Entities;
using FluentValidation.Results;
using Enbloc.DbEntities;

namespace Enbloc
{

    class Program
    {



        static void Main(string[] args)
        {
            try
            {
                //Only If Local Env
                var gcpCredentaialPath = "./config/client_secret.json";
                System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", gcpCredentaialPath);

                Mail mailService = new Mail();
                List<Email> emails = new List<Email>();

                //Get All Unread Emails
                var baseEmails = mailService.getUnreadEmailsByLabel("INBOX"); //Enbloc.Loaded
                                                                              //Log Object
                if (baseEmails.Success)
                {
                    emails = baseEmails.Data;
                    emails.ForEach(email =>
                    {
                        if (validateMail(email))
                        {
                            // switch (email.Subject.Trim().ToLower())
                            // {
                            //     case "enbloc.loaded":
                            //         break;
                            //     case "enbloc.empty":
                            //         break;
                            //     default:
                            //         var replyTemplate = getTemplate(TemplateCodes.InvalidSubject);
                            //         //ReplyToEmail(mailService, mail, replyTemplate);
                            //         break;
                            // }

                            string replyTemplate = processEmail(mailService, email);

                            ReplyToEmail(mailService, email, replyTemplate);
                        }
                    });
                }
                else
                {
                    //return true;
                    //throw exception
                }

            }
            catch (Exception ex)
            {

            }
        }













        //validationCodes
        enum TemplateCodes
        {
            EmailNotWhiteListed,
            MaxEmailLimitReached,
            InvalidSubject, // For all emails that are not classified into labels
            NoAttachment,
            InvalidNoOfAttachment,
            InvalidFormat,
            NoRowsLimitReached,
            EmailProcessed,
            ErrorOccured,
        };



        private static bool validateMail(Email mail)
        {
            //EmailNotWhiteListed,
            //MaxEmailLimitReached,
            return true;
        }

        private static string getTemplate(TemplateCodes templateCode)
        {
            string template = "";
            switch (templateCode)
            {
                case TemplateCodes.EmailNotWhiteListed:
                    template = "Email Id Not White Listed";
                    break;
                case TemplateCodes.MaxEmailLimitReached:
                    template = "Maximum limit Reached";
                    break;
                case TemplateCodes.InvalidSubject:
                    template = "Subject is invalid";
                    break;
                case TemplateCodes.NoAttachment:
                    template = "Missing valid Attachment";
                    break;
                case TemplateCodes.InvalidFormat:
                    template = "Invalid Format";
                    break;
                case TemplateCodes.NoRowsLimitReached:
                    template = "Maximum rows limit reached";
                    break;
                case TemplateCodes.EmailProcessed:
                    template = "Your Enbloc has been processed by <b>Empezar's Bot Technology</b>.<br /><br />Transaction Number for the same is :  {vesselno} <br /><br />To check live status,please click on the below link.<br /> http://elabs-215913.appspot.com/view/Firestore/Enbloc <br /><br /><br />Thank You.";
                    break;
                case TemplateCodes.ErrorOccured:
                    template = "Some error occured while processing your enbloc, please contact our backend team.";
                    break;
            }

            return template;

        }


        public static string processEmail(Mail mailService, Email email)
        {
            BaseReturn<Dictionary<string, string>> baseObject = new BaseReturn<Dictionary<string, string>>();
            List<EmptyEnblocSnapshot> lstEnblocSnapshot = new List<EmptyEnblocSnapshot>();
            baseObject.Success = false;

            baseObject = GetEmailAttachments(mailService, email);
            if (baseObject.Success)
            {
                baseObject = ProcessEmailAttachments(mailService, email, lstEnblocSnapshot);
            }

            if (baseObject.Success)
            {
                baseObject = ValidateEnbloc(lstEnblocSnapshot);
            }          

            if (baseObject.Success)
            {
                baseObject = SaveEnblocToDB(lstEnblocSnapshot);
            }
            return mapTemplateToData(baseObject);
        }

        private static string mapTemplateToData(BaseReturn<Dictionary<string, string>> baseObject)
        {
            string replyMessage = "";
            try
            {
                StringBuilder result = new StringBuilder(getTemplate((TemplateCodes)baseObject.Code));
                if (baseObject.Data != null)
                {
                    baseObject.Data.ToList().ForEach(obj =>
                    {
                        result.Replace(obj.Key, obj.Value);
                    });
                }
                replyMessage = result.ToString();
            }
            catch (Exception ex)
            {
                replyMessage = "Error Occured !";
            }
            return replyMessage;
        }


        private static BaseReturn<Dictionary<string, string>> GetEmailAttachments(Mail mailService, Email email)
        {
            BaseReturn<Dictionary<string, string>> baseObject = new BaseReturn<Dictionary<string, string>>();
            Dictionary<string, string> obj = new Dictionary<string, string>();
            baseObject.Success = true;

            try
            {
                if (!email.Attachments.Any())
                {
                    baseObject.Success = false;
                    baseObject.Code = (int)TemplateCodes.NoAttachment;
                    baseObject.Data = obj;
                    return baseObject;
                }

                var attachments = email.Attachments.Where(attachment => attachment.Filename.ToLower().EndsWith(FileType.XLSX)).ToList();

                if (!attachments.Any())
                {
                    baseObject.Success = false;
                    baseObject.Code = (int)TemplateCodes.NoAttachment;
                    baseObject.Data = obj;
                    return baseObject;
                }

                if (attachments.Count > 1) // Invalid no. of attachments
                {
                    baseObject.Success = false;
                    baseObject.Code = (int)TemplateCodes.InvalidNoOfAttachment;
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
                baseObject.Code = (int)TemplateCodes.ErrorOccured;
                baseObject.Data = obj;
            }
            return baseObject;
        }


        private static BaseReturn<Dictionary<string, string>> ProcessEmailAttachments(Mail mailService, Email email, List<EmptyEnblocSnapshot> lstEnblocSnapshot)
        {
            BaseReturn<Dictionary<string, string>> baseObject = new BaseReturn<Dictionary<string, string>>();
            Dictionary<string, string> obj = new Dictionary<string, string>();
            try
            {
                FileInfo file = new FileInfo(email.Attachments.First().localUrl);
                ProcessEnbloc(file, "EM", email.TransactionId, lstEnblocSnapshot);

                baseObject.Success = true;
            }
            catch (Exception ex)
            {
                baseObject.Success = false;
                baseObject.Code = (int)TemplateCodes.NoAttachment;
                baseObject.Data = obj;
            }
            return baseObject;

            // SaveEnblocSnapshot();
            // SaveEnblocContainerSnapshot();

            // SaveEnbloc();
            // SaveEnblocContainer();
        }


        private static void ProcessEnbloc(FileInfo file, string programCode, int transactionId, List<EmptyEnblocSnapshot> lstEnblocSnapshot)
        {
            //List<EmptyEnblocSnapshot> lstEnblocSnapshot = new List<EmptyEnblocSnapshot>();
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
                        EmptyEnblocSnapshot enblocSnapshot = new EmptyEnblocSnapshot();
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

        private static BaseReturn<Dictionary<string, string>> ValidateEnbloc(List<EmptyEnblocSnapshot> lstEnblocSnapshot)
        {
            BaseReturn<Dictionary<string, string>> baseObject = new BaseReturn<Dictionary<string, string>>();
            Dictionary<string, string> obj = new Dictionary<string, string>();
            try
            {
                EmptyEnblocValidatorCollectionValidator validator = new EmptyEnblocValidatorCollectionValidator();

                if (lstEnblocSnapshot.Count > 1000)
                {
                    baseObject.Success = false;
                    baseObject.Code = (int)TemplateCodes.NoRowsLimitReached;
                    baseObject.Data = obj;
                    return baseObject;
                }

                // if vessel voyage no already exists and enbloc in progress then no processing 

                if (IsVesselVoyageExists(lstEnblocSnapshot.First().Vessel, lstEnblocSnapshot.First().Voyage))
                {
                    baseObject.Success = false;
                    baseObject.Code = (int)TemplateCodes.ErrorOccured;
                    baseObject.Data = obj;
                    return baseObject;
                }

                ValidationResult results = validator.Validate(lstEnblocSnapshot);
                if (!results.IsValid)
                {
                    baseObject.Success = false;
                    baseObject.Code = (int)TemplateCodes.ErrorOccured;
                    baseObject.Data = obj;
                    return baseObject;
                }

                baseObject.Success = true;

            }
            catch (Exception ex)
            {
                baseObject.Success = false;
                baseObject.Code = (int)TemplateCodes.ErrorOccured;
                baseObject.Data = obj;
            }
            return baseObject;
        }
        private static void ReplyToEmail(Mail mailService, Email mail, string replayMsg)
        {
            mailService.sendMailReply(mail, replayMsg);
        }


        private static BaseReturn<Dictionary<string, string>> SaveEnblocToDB(List<EmptyEnblocSnapshot> lstEnblocSnapshot)
        {

            BaseReturn<Dictionary<string, string>> baseObject = new BaseReturn<Dictionary<string, string>>();
            Dictionary<string, string> obj = new Dictionary<string, string>();
            try
            {
                var enblocFromSnapshot = lstEnblocSnapshot.First();
                string vesselno = enblocFromSnapshot.Vessel.Split(' ').ToList().Aggregate((x, y) => x.Trim() + y.Trim()) + enblocFromSnapshot.Voyage.ToString();

                EmptyEnbloc objEnbloc = new EmptyEnbloc()
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

                new EmpezarRepository<EmptyEnbloc>().Add(objEnbloc);


                List<EmptyEnblocContainers> lstEmptyEnblocContainers = new List<EmptyEnblocContainers>();
                lstEnblocSnapshot.ForEach(enblocContainer =>
                {
                    lstEmptyEnblocContainers.Add(new EmptyEnblocContainers()
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

                new EmpezarRepository<EmptyEnblocContainers>().AddRange(lstEmptyEnblocContainers);


                baseObject.Success = true;
                baseObject.Code = (int)TemplateCodes.EmailProcessed;
                baseObject.Data = obj;
            }
            catch (Exception ex)
            {
                baseObject.Success = false;
                baseObject.Code = (int)TemplateCodes.ErrorOccured;
                baseObject.Data = obj;
            }
            return baseObject;
        }

        private static bool IsVesselVoyageExists(string vessel, string voyage)
        {
            string vesselno = vessel.Split(' ').ToList().Aggregate((x, y) => x.Trim() + y.Trim()) + voyage;
            return new EmpezarRepository<EmptyEnbloc>().IsExists(x => x.VesselNo == vesselno);
        }

        //Get Emails 

        // Process Attachments 

        //Push to Enbloc

        //Push to Enbloc Containers 

        //Replay To Emails

        //Error handling 

        //Validations 


    }
}
