namespace OpenSphericalCamera
{
    using MiniJSON;
    using UnityEngine;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class OscDevice : MonoBehaviour
    {
        public static readonly string Scheme = "http";

        public string currentSessionId { get; protected set; }

        public string ipAddress = "";

        public string httpPort = "";

        public delegate void OnCompleteGetInfo(Info info, Error error);

        public delegate void OnCompleteGetState(string fingerprint, State state, Error error);

        public delegate void OnCompleteCheckForUpdates(string stateFingerPrint, Error error);

        public delegate void OnCompleteCommand(CommandResponse response);
       
        #region Protocols

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback">delegate void OnCompleteGetInfo(Info info, Error error)</param>
        public void GetInfo(OnCompleteGetInfo callback)
        {
            StartCoroutine(GetInfoCoroutine(typeof(Info), callback));
        }

        public void GetInfo(Type typeOfInfo, OnCompleteGetInfo callback)
        {
            StartCoroutine(GetInfoCoroutine(typeOfInfo, callback));
        }


        private IEnumerator GetInfoCoroutine(Type typeOfInfo, OnCompleteGetInfo callback)
        {
            WWW www = new WWW(Scheme + "://" + ipAddress + ":" + httpPort + "/osc/info");

            yield return www;

            if (!string.IsNullOrEmpty(www.error))
            {
                if (string.IsNullOrEmpty(www.text))
                {
                    Error error = new Error();

                    error.message = www.error;

                    callback(null, error);
                }
                else
                {
                    callback(null, new Error(www.text));
                }
            }
            else
            {
                try
                {
                    var info = (Info) System.Activator.CreateInstance(typeOfInfo);

                    info.ParseJson(www.text);

                    callback(info, null);
                }
                catch(Exception ex)
                {
                    Error error = new Error();

                    error.message = ex.Message;

                    callback(null, error);
                }
            }

            // Test Code
            /*yield return 1;

            string test = "{        \"manufacturer\": \"RICOH\",        \"model\": \"RICOH THETA S\",        \"serialNumber\": \"00001234\",        \"firmwareVersion\": \"1.0.0\",        \"supportUrl\": \"https://theta360.com/en/support/\",        \"endpoints\": {            \"httpPort\": 80,            \"httpUpdatesPort\": 80        },        \"gps\": false,        \"gyro\": false,       \"uptime\": 67,        \"api\": [            \"/osc/info\",            \"/osc/state\",            \"/osc/checkForUpdates\",            \"/osc/commands/execute\",            \"/osc/commands/status\"        ]}";

            info.ParseJson(test);

            callback(info, null);*/
        }

        /// <summary>
        /// Acquires the camera status.
        /// Changes in the state object content can be checked with CheckForUpdates.
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="httpPort"></param>
        /// <param name="state"></param>
        /// <param name="callback">delegate void OnCompleteGetState(string fingerprint, State state, Error error)</param>
        public void GetState(State state, OnCompleteGetState callback)
        {
            StartCoroutine(GetStateCoroutine(state, callback));
        }

        private IEnumerator GetStateCoroutine(State state, OnCompleteGetState callback)
        {
            WWWForm form = new WWWForm();

            form.AddField("dummy", "dummy");

            WWW www = new WWW(Scheme + "://" + ipAddress + ":" + httpPort + "/osc/state", form);

            yield return www;

            if (!string.IsNullOrEmpty(www.error))
            {
                if (string.IsNullOrEmpty(www.text))
                {
                    Error error = new Error();

                    error.message = www.error;

                    callback(null, null, error);
                }
                else
                {
                    callback(null, null, new Error(www.text));
                }
            }
            else
            {
                try
                {
                    string fingerprint;

                    state.Parse(www.text, out fingerprint);

                    callback(fingerprint, state, null);
                }
                catch (Exception ex)
                {
                    Error error = new Error();

                    error.message = ex.Message;

                    callback(null, null, error);
                }
            }
        }

        /// <summary>
        /// Acquires the current status ID, and checks for changes to the State status.
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="httpPort"></param>
        /// <param name="stateFingerprint"></param>
        /// <param name="callback">delegate void OnCompleteCheckForUpdates(string stateFingerPrint, Error error)</param>
        public void CheckForUpdates(string stateFingerprint, OnCompleteCheckForUpdates callback)
        {
            StartCoroutine(CheckForUpdatesCoroutine(stateFingerprint, callback));
        }

        private IEnumerator CheckForUpdatesCoroutine(string stateFingerprint, OnCompleteCheckForUpdates callback)
        {
            WWWForm form = new WWWForm();

            form.AddField("stateFingerprint", stateFingerprint);

            WWW www = new WWW(Scheme + "://" + ipAddress + ":" + httpPort + "/osc/checkForUpdates", form);

            yield return www;

            if (!string.IsNullOrEmpty(www.error))
            {
                if (string.IsNullOrEmpty(www.text))
                {
                    Error error = new Error();

                    error.message = www.error;

                    callback(null, error);
                }
                else
                {
                    callback(null, new Error(www.text));
                }
            }
            else
            {
                try
                {
                    string _stateFingerprint = "";

                    IDictionary dict = (IDictionary)Json.Deserialize(www.text);

                    if (dict.Contains("stateFingerprint"))
                    {
                        _stateFingerprint = (string)dict["stateFingerprint"];
                    }

                    callback(_stateFingerprint, null);
                }
                catch (Exception ex)
                {
                    Error error = new Error();

                    error.message = ex.Message;

                    callback(null, error);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="callback">delegate void OnCompleteCommand(CommandResponse response)</param>
        public void CommandExecute(CommandRequest request, Action<CommandResponse> callback)
        {
            StartCoroutine(CommandExecuteCoroutine(request, callback));
        }

        private IEnumerator CommandExecuteCoroutine(CommandRequest request, Action<CommandResponse> callback)
        {
            WWWForm form = new WWWForm();

            form.AddField("name", request.name);

            form.AddField("parameters", request.InputParameters);

            WWW www = new WWW(Scheme + "://" + ipAddress + ":" + httpPort + "/osc/checkForUpdates", form);

            yield return www;

            CommandResponse response = new CommandResponse();

            if (!string.IsNullOrEmpty(www.error))
            {
                if (string.IsNullOrEmpty(www.text))
                {
                    Error error = new Error();

                    error.message = www.error;

                    response.error = error;

                    callback(response);
                }
                else
                {
                    response.error =  new Error(www.text);

                    callback(response);
                }
            }
            else
            {
                try
                {
                    if(!string.IsNullOrEmpty(www.text))
                        response.Parse(www.text);

                    if (www.bytes != null)
                        response.bytes = www.bytes;

                    callback(response);
                }
                catch (Exception ex)
                {
                    Error error = new Error();

                    error.message = ex.Message;

                    response.error = error;

                    callback(response);
                }
            }
        }

        private void CommandTest(string testJson, CommandRequest request, Action<CommandResponse> callback)
        {
            Debug.Log("parameters = " + request.InputParameters);

            CommandResponse response = new CommandResponse();

            try
            {
                response.Parse(testJson);

                callback(response);
            }
            catch (Exception ex)
            {
                Error error = new Error();

                error.message = ex.Message;

                response.error = error;

                callback(response);
            }
        }

        #endregion

        #region Commands

        public delegate void OnCompleteStartSession(string sessionId, int timeout, Error error);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback">delegate void OnCompleteStartSession(string sessionId, int timeout, Error error)</param>
        public virtual void StartSession(OnCompleteStartSession callback)
        {
            CommandRequest request = new CommandRequest("camera.startSession");

            CommandExecute(request, (response) => 
            {
                if (response.error != null)
                {
                    callback(null, -1, response.error);
                }
                else
                {
                    try
                    {
                        string sessionId = (response.results.Contains("sessionId")) ? (string)response.results["sessionId"] : null;

                        int timeout = (response.results.Contains("timeout")) ? (int)((long)response.results["timeout"]) : -1;

                        currentSessionId = sessionId;

                        callback(sessionId, timeout, null);
                    }
                    catch(System.Exception ex)
                    {
                        Error error = new Error();

                        error.message = ex.Message;

                        callback(null, -1, error);
                    }
                }
            });
        }

        public delegate void OnCompleteUpdateSession(string sessionId, int timeout, Error error);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback">delegate void OnCompleteUpdateSession(string sessionId, int timeout, Error error)</param>
        public virtual void UpdateSession(OnCompleteUpdateSession callback)
        {
            CommandRequest request = new CommandRequest("camera.updateSession");

            CommandExecute(request, (response) =>
            {
                if (response.error != null)
                {
                    callback(null, -1, response.error);
                }
                else
                {
                    try
                    {
                        string sessionId = (response.results.Contains("sessionId")) ? (string)response.results["sessionId"] : null;

                        int timeout = (response.results.Contains("timeout")) ? (int)((long)response.results["timeout"]) : -1;

                        currentSessionId = sessionId;

                        callback(sessionId, timeout, null);
                    }
                    catch (System.Exception ex)
                    {
                        Error error = new Error();

                        error.message = ex.Message;

                        callback(null, -1, error);
                    }
                }
            });
        }

        public delegate void OnCompleteCloseSession(string sessionId, Error error);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback">delegate void OnCompleteCloseSession(string sessionId, Error error)</param>
        public void CloseSession(OnCompleteCloseSession callback)
        {
            CommandRequest request = new CommandRequest("camera.closeSession");

            CommandExecute(request, (response) =>
            {
                if (response.error != null)
                {
                    callback(null, response.error);
                }
                else
                {
                    try
                    {
                        string sessionId = (response.results.Contains("sessionId")) ? (string)response.results["sessionId"] : null;

                        sessionId = "";

                        callback(sessionId, null);
                    }
                    catch (System.Exception ex)
                    {
                        Error error = new Error();

                        error.message = ex.Message;

                        callback(null, error);
                    }
                }
            });
        }

        public delegate void OnCompleteTakePicture(string fileUri, Error error);

        public void TakePicture(OnCompleteTakePicture callback)
        {
            if (string.IsNullOrEmpty(currentSessionId))
            {
                Error error = new Error();

                error.message = "Session is not started.";

                callback(null, error);
            }

            CommandRequest request = new CommandRequest("camera.takePicture");

            request.AddParameter("sessionId", currentSessionId);

            CommandExecute(request, (response) =>
            {
                if (response.error != null)
                {
                    callback(null, response.error);
                }
                else
                {
                    try
                    {
                        string fileUri = (response.results.Contains("fileUri")) ? (string)response.results["fileUri"] : null;

                        callback(fileUri, null);
                    }
                    catch (System.Exception ex)
                    {
                        Error error = new Error();

                        error.message = ex.Message;

                        callback(null, error);
                    }
                }
            });
        }

        public delegate void OnCompleteListImages(List<Entry> entries, int totalEntries, string continuationToken, Error error);

        public void ListImages(int entryCount, int maxSize, string continuationToken, bool includeThumb, OnCompleteListImages callback)
        {
            ListImages(Type.GetType("Entry"), entryCount, maxSize, continuationToken, includeThumb, callback);
        }

        public void ListImages(Type entryType, int entryCount, int maxSize, string continuationToken, bool includeThumb, OnCompleteListImages callback)
        {
            CommandRequest request = new CommandRequest("camera.listImages");

            request.AddParameter("entryCount", entryCount);

            request.AddParameter("maxSize", maxSize);

            request.AddParameter("continuationToken", continuationToken);

            request.AddParameter("includeThumb", includeThumb);

            //string testJson = "{   \"results\": { \"entries\": [        {            \"name\": \"R0010016.JPG\",            \"uri\": \"100RICOH/R0010016.JPG\",            \"size\": 4214389,            \"dateTimeZone\": \"2015:07:10 11:00:35+09:00\",            \"width\": 5376,            \"height\": 2688,            \"thumbnail\": \"(base64_binary)\",            \"_thumbSize\": 3136       },        {            \"name\": \"R0010015.JPG\",            \"uri\": \"100RICOH/R0010015.JPG\",            \"size\": 4217265,            \"dateTimeZone\": \"2015:07:10 11:00:34+09:00\",            \"width\": 5376,            \"height\": 2688,            \"thumbnail\": \"(base64_binary)\",            \"_thumbSize\": 3136        }    ],    \"totalEntries\": 16,    \"continuationToken\": \"14\" }}";

            //CommandTest(testJson, request, (response)=>
            CommandExecute(request, (response)=>
            {
                if (response.error != null)
                {
                    callback(new List<Entry>(), 0, "", response.error);
                }
                else
                {
                    try
                    {
                        var entries = new List<Entry>();

                        if (response.results.Contains("entries"))
                        {
                            var list = (IList)response.results["entries"];

                            foreach (IDictionary dict in list)
                            {
                                var entry = (Entry) System.Activator.CreateInstance(entryType);

                                JSONUtil.DictionaryToObjectFiled(dict, entry);

                                entries.Add(entry);
                            }
                        }

                        int _totalEntries = (response.results.Contains("totalEntries")) ? (int)((long)response.results["totalEntries"]) : 0;

                        string _continuationToken = (response.results.Contains("continuationToken")) ? (string)response.results["continuationToken"] : null;

                        callback(entries, _totalEntries, _continuationToken, null);

                    }
                    catch (System.Exception ex)
                    {
                        Error error = new Error();

                        error.message = ex.Message;

                        callback(new List<Entry>(), 0, null, response.error);
                    }
                }
            });

        }

        public void Delete(string fileUri, Action<Error> callback)
        {
            CommandRequest request = new CommandRequest("camera.delete");

            request.AddParameter("fileUri", fileUri);

            CommandExecute(request, (response) =>
            {
                callback(response.error);
            });
        }

        public void GetImage(string fileUri, Action<byte[], Error> callback)
        {
            GetImage(fileUri, -1, callback);
        }

        public void GetImage(string fileUri, long maxSize, Action<byte[], Error> callback)
        {
            CommandRequest request = new CommandRequest("camera.getImage");

            request.AddParameter("fileUri", fileUri);

            if (maxSize > 0)
            {
                request.AddParameter("maxSize", maxSize);
            }

            CommandExecute(request, (response) =>
            {
                callback(response.bytes, response.error);
            });
        }

        /// <summary>
        /// Returns file metadata given its URI. The image header lists the Exif and XMP fields.
        /// </summary>
        /// <param name="fileUri">the target file. Manufacturers decide whether to use absolute or relative URIs. Clients may treat this as an opaque identifier.</param>
        /// <param name="callback"></param>
        public void GetMetaData(string fileUri, Action<ImageMeta, Error> callback)
        {
            CommandRequest request = new CommandRequest("camera.getMetadata");

            request.AddParameter("fileUri", fileUri);

            CommandExecute(request, (response) =>
            {
                if (response.error != null)
                {
                    callback(null, response.error);
                }
                else
                {
                    ImageMeta meta = new ImageMeta();

                    try
                    {
                        meta.Parse(response.results);

                        callback(meta, null);
                    }
                    catch (System.Exception ex)
                    {
                        Error error = new Error();

                        error.message = ex.Message;

                        callback(null, response.error);
                    }                 
                }
            });
        }

        public void GetOptions(string sessionId, string[] optionNames, Action<Options, Error> callback)
        {
            GetOptions(typeof(Options), sessionId, optionNames, callback);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeOfOption"></param>
        /// <param name="sessionId"></param>
        /// <param name="optionNames"></param>
        /// <param name="callback">Action(Options, Error)</param>
        public void GetOptions(Type typeOfOption, string sessionId, string[] optionNames, Action<Options, Error> callback)
        {
            CommandRequest request = new CommandRequest("camera.getOptions");

            request.AddParameter("sessionId", sessionId);

            request.AddParameter("optionNames", Json.Serialize(optionNames));

            CommandExecute(request, (response) =>
            {
                if (response.error != null)
                {
                    callback(null, response.error);
                }
                else
                {
                    try
                    {
                        var options = Activator.CreateInstance(typeOfOption);

                        JSONUtil.DictionaryToObjectFiled(response.results, options);

                        callback((Options)options, null);
                    }
                    catch (System.Exception ex)
                    {
                        Error error = new Error();

                        error.message = ex.Message;

                        callback(null, response.error);
                    }
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="options"></param>
        /// <param name="callback"> Action<Error></param>
        public void SetOptions(string sessionId, Options options, Action<Error> callback)
        {
            CommandRequest request = new CommandRequest("camera.setOptions");

            request.AddParameter("sessionId", sessionId);

            if (options != null)
            {
                string json = Json.Serialize(options);

                Debug.Log(json);

                request.AddParameter("options", json);
            }

            CommandExecute(request, (response) =>
            {
                callback(response.error);
            });
        }

        #endregion
    }
}