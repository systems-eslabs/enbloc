using System;
using Newtonsoft.Json;
using System.Collections.Generic;


namespace Enbloc
{
    public static class Config
    {
        static Dictionary<string, string> _config = null;
        static string _enblocwhitelistEmailIds;
        static string _enblocwhitelistDomains;
        

        static Config()
        {
            string JsonString = System.IO.File.ReadAllText("./config/config.json");
            _config = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonString);

            setConfigurations();
        }


        public static string enblocwhitelistEmailIds
        {
            get
            {
                return _enblocwhitelistEmailIds;
            }
        }

          public static string enblocwhitelistDomains
        {
            get
            {
                return _enblocwhitelistDomains;
            }
        }
        static void setConfigurations()
        {
            _enblocwhitelistEmailIds = _config["enblocwhitelistEmailIds"];
            _enblocwhitelistDomains = _config["enblocwhitelistDomains"];
          
        }


    }


}