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
                IDictionary dict = (IDictionary)Json.Deserialize(json);

                if (dict.Contains("name"))
                {
                    this.name = (string)dict["name"];
                }

                if (dict.Contains("state"))
                {
                    string _state = (string)dict["state"];

                    this.state = (Status) System.Enum.Parse(typeof(Status), _state);
                }

                if (dict.Contains("id"))
                {
                    this.id = (string)dict["id"];
                }

                if (dict.Contains("results"))
                {
                    this.results = (IDictionary)dict["results"];
                }

                if (dict.Contains("error"))
                {
                    this.error = Error.Parse((string)dict["error"]);
                }

                if (dict.Contains("progress"))
                {
                    this.progress = (IDictionary)dict["progress"];
                }
            }
            catch
            {
                throw new System.Exception("Parse Json Error");
            }
        }
    }
}