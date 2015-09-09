namespace OpenSphericalCamera
{
    using System.Collections;
    using System.Collections.Generic;

    public class CommandRequest
    {
        public string name;

        Dictionary<string, object> parameters;

        Dictionary<string, object> parametersDicionary;

        public string InputParameters
        {
            get 
            {                 
                return MiniJSON.Json.Serialize(parameters); 
            }
        }

        public CommandRequest()
        {
            Init();
        }

        public CommandRequest(string name)
        {
            this.name = name;

            Init();
        }

        void Init()
        {
            parameters = new Dictionary<string, object>();

            parametersDicionary = new Dictionary<string, object>();

            parameters.Add("parameters", parametersDicionary);
        }

        public void AddParameter(string key, object value)
        {
            parametersDicionary.Add(key, value);
        }

        public void RemoveParameter(string key)
        {
            parametersDicionary.Remove(key);
        }
    }
}
