using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EmailService;
using OfficeOpenXml;
using enbloc.DbEntities;
using Common;

namespace enbloc
{

    class Program
    {

        static void Main(string[] args)
        {
            //Only If Local Env
            var gcpCredentaialPath = "./config/client_secret.json";
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", gcpCredentaialPath);


            Mail mailService = new Mail();
            List<Email> enblocEmails = new List<Email>();

            //Get All Unread Emails
            var baseEnblocEmails = mailService.getUnreadEmailsByLabel("INBOX"); //Enbloc.Loaded
            if (baseEnblocEmails.Success)
            {
                enblocEmails = baseEnblocEmails.Data;

                enblocEmails.ForEach(mail =>
                {

                    if (mail.Attachments.Any())
                    {
                        GetEmailAttachments(mailService, mail);

                        if (mail.HasValidAttachments)
                        {
                            ProcessEmailAttachments(mailService, mail);
                        }
                    }

                });
            }
            else
            {
                //return true;
                //throw exception
            }

        }

        private static void GetEmailAttachments(Mail mailService, Email mail)
        {
            var attachments = mail.Attachments.Where(attachment => attachment.Filename.ToLower().EndsWith(FileType.XLSX)).ToList();

            if (attachments.Any())
            {
                List<EAttachmentRequest> attachmentRequest = attachments.Select(attachment => new EAttachmentRequest
                {
                    AttachmentId = attachment.AttachmentId,
                    Filename = attachment.Filename
                }).ToList();

                mail.Attachments = mailService.getAttachments(mail.MailId, attachmentRequest).Data;
                mail.HasValidAttachments = true;

            }
        }


        private static void ProcessEmailAttachments(Mail mailService, Email email)
        {
            // SaveEnblocSnapshot();
            // SaveEnblocContainerSnapshot();


            // SaveEnbloc();

            // SaveEnblocContainer();

            email.Attachments.ForEach(mail =>
            {
                FileInfo file = new FileInfo(mail.localUrl);

                using (ExcelPackage package = new ExcelPackage(file))
                {
                    List<EmptyEnblocSnapshot> lstEnblocSnapshot = new List<EmptyEnblocSnapshot>();
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                    int rowCount = worksheet.Dimension.Rows;
                    int ColCount = worksheet.Dimension.Columns;



                    string document_date = Convert.ToString(worksheet.Cells["C1"].Value);
                    string vessel = Convert.ToString(worksheet.Cells["B4"].Value);
                    string voyage = Convert.ToString(worksheet.Cells["D4"].Value);
                    string agent_name = Convert.ToString(worksheet.Cells["B5"].Value);
                    string via_no = Convert.ToString(worksheet.Cells["D5"].Value);

                    string transaction_no ="EM" + email.TransactionId;

                    for (int row = 8; row <= rowCount; row++)
                    {
                        if (Convert.ToString(worksheet.Cells[row, 1].Value).Trim() != "")
                        {
                            EmptyEnblocSnapshot enblocSnapshot = new EmptyEnblocSnapshot();
                            enblocSnapshot.TransactionId = transaction_no;
                            enblocSnapshot.Vessel = vessel;
                            enblocSnapshot.Voyage = voyage;
                            enblocSnapshot.AgentName = agent_name;
                            enblocSnapshot.ViaNo = via_no;
                            enblocSnapshot.Date = document_date;
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
                            enblocSnapshot.CreatedBy = 123;//Convert.ToString(worksheet.Cells[row, 19].Value).Trim();

                            lstEnblocSnapshot.Add(enblocSnapshot);

                        }
                    }
                    new EmpezarRepository<EmptyEnblocSnapshot>().AddRange(lstEnblocSnapshot);


                    ReplyToEmail(mailService, email, transaction_no);
                    // _context.AddRangeAsync(lstEnblocSnapshot);
                    // _context.SaveChanges();

                }
            });


        }


        private static void ReplyToEmail(Mail mailService, Email mail, string vesselno)
        {
            string replayMsg = "Your Enbloc has been processed by <b>Empezar's Bot Technology</b>.<br /><br />Transaction Number for the same is :  " + vesselno + "<br /><br />To check live status,please click on the below link.<br /> http://elabs-215913.appspot.com/view/Firestore/Enbloc <br /><br /><br />Thank You.";
            mailService.sendMailReply(mail.MailId, replayMsg);
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
