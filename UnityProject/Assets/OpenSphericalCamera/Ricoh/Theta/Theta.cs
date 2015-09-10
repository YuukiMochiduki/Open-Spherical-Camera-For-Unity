namespace OpenSphericalCamera.Ricoh
{    
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class Theta : OscDevice
    {
        public enum DataType { full, thumb };

        public static readonly string DefaultIpAddress = "192.168.1.1";

        public static readonly string DefaultHttpPort = "80";

        public delegate void OnCompleteGetThetaState(string fingerprint, ThetaState state, Error error);

        public void SetIpAdress(string ipAddress, string httpPort)
        {
            this.ipAddress = ipAddress;

            this.httpPort = httpPort;
        }

        public void GetState(OnCompleteGetThetaState callback)
        {
            ThetaState state = new ThetaState();

            base.GetState(state, (_fingerprint, _state, _error ) => 
            {
                callback(_fingerprint, (ThetaState)_state, _error);
            });
        }

        public delegate void OnCompleteThetaListImages(List<ThetaEntry> entries, int totalEntries, string continuationToken, Error error);

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

        public void FinishWlan(Action<Error> callback)
        {
            CommandRequest request = new CommandRequest("camera._finishWlan");

            request.AddParameter("sessionId", currentSessionId);

            CommandExecute(request, (response) =>
            {
                callback(response.error);
            });
        }

        public void StartCapture(Action<Error> callback)
        {
            CommandRequest request = new CommandRequest("camera._startCapture");

            request.AddParameter("sessionId", currentSessionId);

            CommandExecute(request, (response) =>
            {
                callback(response.error);
            });
        }

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
        /// 
        /// </summary>
        /// <param name="entryCount"></param>
        /// <param name="continuationToken"> (Optional) An opaque continuation token of type string, returned by previous listImages call, used to retrieve next images. </param>
        /// <param name="detail"></param>
        /// <param name="sort">sort string should be "newest" or "oldest"</param>
        /// <param name="callback">delegate void OnCompleteListAll(List<ThetaEntry> entries, int totalEntries, string continuationToken, Error error)</param>        
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
        /// <param name="callback">Action<byte[], Error> callback</param>
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
        /// Retrieve live preview bynary data. The data format is "Equirectangular" and "MotionJPEG".
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

