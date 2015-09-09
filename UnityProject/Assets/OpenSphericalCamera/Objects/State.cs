namespace OpenSphericalCamera
{
    using MiniJSON;
    using System.Collections;

    public class State
    {
        public string sessionId;

        public double batteryLevel;

        public bool storageChanged;

        public virtual void Parse(string json, out string fingerprint)
        {
            try
            {
                fingerprint = null;

                IDictionary dict = (IDictionary)Json.Deserialize(json);

                if (dict.Contains("fingerprint"))
                {
                    fingerprint = (string)dict["fingerprint"];
                }

                if (dict.Contains("state"))
                {
                    JSONUtil.DictionaryToObjectFiled((IDictionary)dict["state"], this);
                }
            }
            catch
            {
                throw new System.Exception("Parse Json Error");
            }
        }
    }
}