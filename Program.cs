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

                new Enbloc().processUnreadEmails();

            }
            catch (Exception ex)
            {

            }
        }

    }
}
