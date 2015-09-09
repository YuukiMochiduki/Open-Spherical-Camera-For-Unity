namespace OpenSphericalCamera
{
    using MiniJSON;
    using System.Collections;

    public class Error
    {
        public ErrorMeta meta;

        public string code;

        public string message;

        public static Error Parse(string json)
        {
            Error error = new Error(json);

            return error;
        }

        public Error() { }

        public Error (string json)
        {
            try
            {                
                meta = new ErrorMeta();

                IDictionary dict = (IDictionary)Json.Deserialize(json);

                JSONUtil.DictionaryToObjectFiled(dict, meta);

                if (dict.Contains("error"))
                {
                    IDictionary dictError = (IDictionary)dict["error"];

                    JSONUtil.DictionaryToObjectFiled(dictError, this);
                }
            }
            catch
            {
                message = "Parse Json error";
            }
        }
    }
}