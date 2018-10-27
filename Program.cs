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
using Serilog;
using Serilog.Sinks.GoogleCloudLogging;
using Serilog.Exceptions;
namespace Enbloc
{

    class Program
    {



        static void Main(string[] args)
        {
            try
            {

                Log.Logger = new LoggerConfiguration()
                .Enrich.WithExceptionDetails()
                //.WriteTo.GoogleCloudLogging(new GoogleCloudLoggingSinkOptions("vijay-angular"))
                .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();


                //Only If Local Env
                var gcpCredentaialPath = "./config/client_secret.json";
                System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", gcpCredentaialPath);
                Log.Information("Application Start");
                new Enbloc().processUnreadEmails();

            }
            catch (Exception ex)
            {

            }
        }

    }
}
