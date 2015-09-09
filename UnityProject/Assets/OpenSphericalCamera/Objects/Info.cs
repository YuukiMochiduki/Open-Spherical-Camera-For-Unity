namespace OpenSphericalCamera
{
    using MiniJSON;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class Info
    {
        public string manufacturer;

        public string model;

        public string serialNumber;

        public string firmwareVersion;

        public string supportUrl;

        public bool gps;

        public bool gyro;

        public long uptime;

        public List<string> api = new List<string>();

        public Endpoints endpoints = new Endpoints();

        public virtual void ParseJson(string json) 
        {
            try
            {
                IDictionary dict = (IDictionary)Json.Deserialize(json);

                JSONUtil.DictionaryToObjectFiled(dict, this);

                /*if (dict.Contains("endpoints"))
                {
                    JSONUtil.DictionaryToObjectFiled((IDictionary)dict["endpoints"], endpoints);                    
                }*/
            }
            catch
            {            
                throw new Exception("Parse Json Error");
            }
        }
    }
}
    
