namespace OpenSphericalCamera.Ricoh
{
    using MiniJSON;
    using System.Collections;
    using System.Collections.Generic;
    using OpenSphericalCamera;

    public class ThetaState : State
    {
        /// <summary>
        /// Continuous shooting status 
        /// "shooting" or "idle"
        /// </summary>
        public string _captureStatus { get; protected set; }

        /// <summary>
        /// Recorded time (sec.) of video being shot
        /// </summary>
        public int _recordedTime { get; protected set; }

        /// <summary>
        /// Remaining time (sec.) of video being shot
        /// </summary>
        public int _recordableTime { get; protected set; }

        /// <summary>
        /// Last saved file ID
        /// </summary>
        public string _latestFileUri { get; protected set; }

        /// <summary>
        /// Charging status
        /// One of "charging", "charged", "disconnect"
        /// </summary>
        public string _batteryState { get; protected set; }

        /// <summary>
        /// Camera error information
        /// </summary>
        public List<string> _cameraError { get; protected set; }

        public override void Parse(string json, out string fingerprint)
        {
            try
            {
                base.Parse(json, out fingerprint);

                IDictionary dict = (IDictionary)Json.Deserialize(json);

                if (dict.Contains("state"))
                {
                    IDictionary stateDictionary = (IDictionary)dict["state"];

                    if (stateDictionary.Contains("_captureStatus"))
                    {
                        this._captureStatus = (string)stateDictionary["_captureStatus"];
                    }

                    if (stateDictionary.Contains("_recordedTime"))
                    {
                        this._recordedTime = (int)((long)stateDictionary["_recordedTime"]);
                    }

                    if (stateDictionary.Contains("_recordableTime"))
                    {
                        this._recordableTime = (int)((long)stateDictionary["_recordableTime"]);
                    }

                    if (stateDictionary.Contains("_latestFileUri"))
                    {
                        this._latestFileUri = (string)stateDictionary["_latestFileUri"];
                    }

                    if (stateDictionary.Contains("_batteryState"))
                    {
                        this._batteryState = (string)stateDictionary["_batteryState"];
                    }

                    this._cameraError = new List<string>();

                    if (stateDictionary.Contains("_cameraError"))
                    {
                        IList list = (IList)stateDictionary["_cameraError"];

                        foreach (var error in list)
                            this._cameraError.Add((string)error);
                    }
                }
            }
            catch
            {
                throw new System.Exception("Parse Json Error");
            }
        }
    }
}