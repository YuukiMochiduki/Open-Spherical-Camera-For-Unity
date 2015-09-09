namespace OpenSphericalCamera
{
    using System.Collections;
    using System.Collections.Generic;

    public class Options
    {
        public string captureMode;

        public List<string> captureModeSupport = new List<string>();

        public double exposureProgram;

        public List<double> exposureProgramSupport;

        public double iso;

        public List<double> isoSupport;

        public double shutterSpeed;

        public List<double> shutterSpeedSupport;

        public double aperture;

        public List<double> apertureSupport;

        public string whiteBalance;

        public List<string> whiteBalanceSupport;

        public double exposureCompensation;

        public List<double> exposureCompensationSupport;

        public FileFormat fileFormat;

        public List<FileFormat> fileFormatSupport;

        public double exposureDelay;

        public List<double> exposureDelaySupport;

        public double sleepDelay;

        public List<double> sleepDelaySupport;

        public double offDelay;

        public List<double> offDelaySupport;

        public double totalSpace;

        public double remainingSpace;

        public double remainingPictures;

        public GpsInfo gpsInfo;

        public string dateTimeZone;

        public bool hdr;

        public bool hdrSupport;

        public ExposureBracket exposureBracket;

        public ExposureBracketSupport exposureBracketSupport;

        public bool gyro;

        public bool gyroSupport;

        public bool gps;

        public bool gpsSupport;

        public string imageStabilization;

        public List<string> imageStabilizationSupport;

        public string wifiPassword;

        public virtual void Parse(IDictionary dictionary)
        {
            try
            {
                JSONUtil.DictionaryToObjectFiled(dictionary, this);

                if (dictionary.Contains("exposureProgramSupport"))
                {
                    IList list = (IList)dictionary["exposureProgramSupport"];

                    foreach (var n in list)
                    {
                        list.Add((double)n);
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
