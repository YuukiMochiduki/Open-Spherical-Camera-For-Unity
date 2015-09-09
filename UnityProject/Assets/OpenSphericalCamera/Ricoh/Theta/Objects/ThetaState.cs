namespace OpenSphericalCamera.Ricoh
{
    using System.Collections.Generic;
    using OpenSphericalCamera;

    public class ThetaState : State
    {
        /// <summary>
        /// Continuous shooting status 
        /// "shooting" or "idle"
        /// </summary>
        public string _captureStatus;

        /// <summary>
        /// Recorded time (sec.) of video being shot
        /// </summary>
        public int _recordedTime;

        /// <summary>
        /// Remaining time (sec.) of video being shot
        /// </summary>
        public int _recordableTime;

        /// <summary>
        /// Last saved file ID
        /// </summary>
        public string _latestFileUri;

        /// <summary>
        /// Charging status
        /// One of "charging", "charged", "disconnect"
        /// </summary>
        public string _batteryState;

        /// <summary>
        /// Camera error information
        /// </summary>
        public List<string> _cameraError;
    }
}