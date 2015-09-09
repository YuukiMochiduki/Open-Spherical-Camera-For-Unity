namespace OpenSphericalCamera
{
    using MiniJSON;
    using System.Collections;
    using System.Collections.Generic;

    public class CommandResponse
    {
        public enum Status { done, inProgress, error };

        public string name;

        public Status state;

        public string id;

        public IDictionary results;

        public Error error;

        public IDictionary progress;

        public byte[] bytes;

        public void Parse(string json)
        {
            try
            { 
                JSONUtil.DictionaryToObjectFiled( (IDictionary)Json.Deserialize(json), this);
            }
            catch
            {
                throw new System.Exception("Parse Json Error");
            }
        }
    }
}