namespace OpenSphericalCamera
{
    using System.Collections;

    public class ImageMeta
    {
        public Exif exif = new Exif();

        public Xmp xmp = new Xmp();

        public void Parse(IDictionary dictionary)
        {
            try
            {
                if (dictionary.Contains("exif"))
                {
                    JSONUtil.DictionaryToObjectFiled((IDictionary)dictionary["exif"], exif);
                }

                if (dictionary.Contains("xmp"))
                {
                    JSONUtil.DictionaryToObjectFiled((IDictionary)dictionary["xmp"], xmp);
                }
            }
            catch
            {
                throw new System.Exception("Parse Json Error");
            }

        }
    }
}
