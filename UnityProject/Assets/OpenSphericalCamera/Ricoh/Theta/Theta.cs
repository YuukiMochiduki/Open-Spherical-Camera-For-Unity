namespace OpenSphericalCamera.Ricoh
{    
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class Theta : OscDevice
    {
        /// <summary>
        /// Retrieve data type : 
        /// full = fullsize
        /// thumb = thumbnail the image or video
        /// </summary>
        public enum DataType { full, thumb };

        /// <summary>
        /// Default ip address of theta
        /// </summary>
        public static readonly string DefaultIpAddress = "192.168.1.1";

        /// <summary>
        /// Default httpport of theta
        /// </summary>
        public static readonly string DefaultHttpPort = "80";

        /// <summary>
        /// Called when GetThetaState completed
        /// </summary>
        /// <param name="fingerprint">Fingerprint (unique identifier) of the current camera state.</param>
        /// <param name="state">State of Theta that is vender specific state data</param>
        /// <param name="error">Error</param>
        public delegate void OnCompleteGetThetaState(string fingerprint, ThetaState state, Error error);

        /// <summary>
        /// Set the ipaddress and http port of this Theta object
        /// </summary>
        /// <param name="ipAddress">Ip address</param>
        /// <param name="httpPort">Http port</param>
        public void SetIpAdress(string ipAddress, string httpPort)
        {
            this.ipAddress = ipAddress;

            this.httpPort = httpPort;
        }

        /// <summary>
        /// Retrieve state attributes of the Theta. 
        /// </summary>
        /// <param name="callback">delegate void OnCompleteGetThetaState(string fingerprint, ThetaState state, Error error)</param>
        public void GetState(OnCompleteGetThetaState callback)
        {
            base.GetState(typeof(ThetaState), (_fingerprint, _state, _error ) => 
            {
                callback(_fingerprint, (ThetaState)_state, _error);
            });
        }

        /// <summary>
        /// Called when ListImages completed
        /// </summary>
        /// <param name="entries">The list of entrys that vender specific of theta.</param>
        /// <param name="totalEntries">Total number of entries in storage.</param>
        /// <param name="continuationToken"> (Optional) Set only if the result is incomplete (incomplete means any listing that does not include the last image). To fetch remaining entries, the client should call listImages command again with the token.</param>
        /// <param name="error">Error</param>
        public delegate void OnCompleteThetaListImages(List<ThetaEntry> entries, int totalEntries, string continuationToken, Error error);

        /// <summary>
        /// Lists all images in the camera.
        /// </summary>
        /// <param name="entryCount">Desired number of entries to return.</param>
        /// <param name="maxSize">Maximum size of thumbnail images; max(thumbnail_width, thumbnail_height). Required only when includeThumb is true</param>
        /// <param name="continuationToken">An opaque continuation token of type string, returned by previous listImages call, used to retrieve next images. Omit this parameter for the first listImages call. Note: The camera is responsible for the algorithm that generates the token. If continuation token is null, this parameter is not used.</param>
        /// <param name="includeThumb">Defaults to true. Use false to omit thumbnail images from the result.</param>
        /// <param name="callback">delegate void OnCompleteThetaListImages(List&lt;ThetaEntry&gt; entries, int totalEntries, string continuationToken, Error error)</param>
        public void ListImages(int entryCount, int maxSize, string continuationToken, bool includeThumb, OnCompleteThetaListImages callback)
        {
            base.ListImages(typeof(ThetaEntry), entryCount, maxSize, continuationToken, includeThumb, (_entries, _totalEntries, _continuationToken, _error) => 
            {
                if (_error != null)
                {
                    callback(new List<ThetaEntry>(), _totalEntries, _continuationToken, _error);
                }
                else
                {
                    callback(_entries.ConvertAll(x => (ThetaEntry)x), _totalEntries, _continuationToken, _error);
                }
            });
        }

        /// <summary>
        /// Returns a full-size or scaled image given its URI. Input parameters include resolution. This is the only command that should return
        /// </summary>
        /// <param name="fileUri">URI of the target file. Manufacturers decide whether to use absolute or relative URIs. Clients may treat this as an opaque identifier.</param>
        /// <param name="type">enum DataType { full, thumb }</param>
        /// <param name="callback">Action&lt;byte[], Error&gt;</param>
        public void GetImage(string fileUri, DataType type, Action<byte[], Error> callback)
        {
            CommandRequest request = new CommandRequest("camera.getImage");

            request.AddParameter("fileUri", fileUri);

            request.AddParameter("_type", type.ToString());

            CommandExecute(request, (response) =>
            {
                callback(response.bytes, response.error);
            });
        }

        /// <summary>
        /// Turn off the wireless LAN
        /// </summary>
        /// <param name="callback">Action&lt;Error&gt;</param>
        public void FinishWlan(Action<Error> callback)
        {
            CommandRequest request = new CommandRequest("camera._finishWlan");

            request.AddParameter("sessionId", currentSessionId);

            CommandExecute(request, (response) =>
            {
                callback(response.error);
            });
        }

        /// <summary>
        /// Start the interval still image capturing or video recording.
        /// Capture mode is selected by captureMode of Option.
        /// </summary>
        /// <param name="callback">Action&lt;Error&gt;</param>
        public void StartCapture(Action<Error> callback)
        {
            CommandRequest request = new CommandRequest("camera._startCapture");

            request.AddParameter("sessionId", currentSessionId);

            CommandExecute(request, (response) =>
            {
                callback(response.error);
            });
        }

        /// <summary>
        /// Stop the capturing.
        /// </summary>
        /// <param name="callback">Action&lt;Error&gt;</param>
        public void StopCapture(Action<Error> callback)
        {
            CommandRequest request = new CommandRequest("camera._stopCapture");

            request.AddParameter("sessionId", currentSessionId);

            CommandExecute(request, (response) =>
            {
                callback(response.error);
            });
        }

        public delegate void OnCompleteListAll(List<ThetaEntry> entries, int totalEntries, string continuationToken, Error error);

        /// <summary>
        /// List all images
        /// </summary>
        /// <param name="entryCount">The list of entrys that vender specific of theta.</param>
        /// <param name="continuationToken"> (Optional) An opaque continuation token of type string, returned by previous listImages call, used to retrieve next images. </param>
        /// <param name="detail"></param>
        /// <param name="sort">sort string should be &quot;newest&quot; or &quot;oldest&quot;</param>
        /// <param name="callback">delegate void OnCompleteListAll(List&lt;ThetaEntry&gt; entries, int totalEntries, string continuationToken, Error error)</param>        
        public void ListAll(int entryCount, string continuationToken, bool detail, string sort, OnCompleteListAll callback)
        {
            CommandRequest request = new CommandRequest("camera._listAll");

            request.AddParameter("entryCount", entryCount);

            if(!string.IsNullOrEmpty(continuationToken))
                request.AddParameter("continuationToken", continuationToken);

            if(!detail)
                request.AddParameter("detail", detail);

            if (sort == "oldest")
                request.AddParameter("sort", sort);

            CommandExecute(request, (response) =>
            {
                if (response.error != null)
                {
                    callback(new List<ThetaEntry>(), 0, "", response.error);
                }
                else
                {
                    try
                    {
                        var entries = new List<ThetaEntry>();

                        if (response.results.Contains("entries"))
                        {
                            var list = (IList)response.results["entries"];

                            foreach (IDictionary dict in list)
                            {
                                var entry = new ThetaEntry();

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

                        callback(new List<ThetaEntry>(), 0, null, response.error);
                    }
                }
            });
        }

        /// <summary>
        /// Retrieve video data
        /// </summary>
        /// <param name="fileUri"></param>
        /// <param name="dataType"></param>
        /// <param name="callback">Action&lt;byte[], Error&gt; callback</param>
        public void GetVideo(string fileUri, DataType dataType, Action<byte[], Error> callback)
        {
            CommandRequest request = new CommandRequest("camera._getVideo");

            request.AddParameter("fileUri", fileUri);

            request.AddParameter("type", dataType.ToString());

            CommandExecute(request, (response) =>
            {
                callback(response.bytes, response.error);
            });
        }

        /// <summary>
        /// Retrieve live preview bynary data. The data format is &quot;Equirectangular&quot; and &quot;MotionJPEG&quot;.
        /// </summary>
        public void GetLivePreview(Action<byte[], Error> callback)
        {
            CommandRequest request = new CommandRequest("camera._getLivePreview");

            request.AddParameter("sessionId", currentSessionId);
            
            CommandExecute(request, (response) =>
            {
                callback(response.bytes, response.error);
            });
        }
    }
}

