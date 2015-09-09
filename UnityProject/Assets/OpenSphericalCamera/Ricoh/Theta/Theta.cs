namespace OpenSphericalCamera.Ricoh
{    
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class Theta : OscDevice
    {
        public enum ImageType { full, thumb };

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

        public void GetImage(string fileUri, ImageType type, Action<byte[], Error> callback)
        {
            CommandRequest request = new CommandRequest("camera.getImage");

            request.AddParameter("fileUri", fileUri);

            request.AddParameter("_type", type.ToString());

            CommandExecute(request, (response) =>
            {
                callback(response.bytes, response.error);
            });
        }
    }
}

