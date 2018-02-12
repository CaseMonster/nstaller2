using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace nstaller2
{
    class XML
    {
        public XML()
        {
        }

        public static Settings ReadXML(string s)
        {

            var doc = XDocument.Load(s, LoadOptions.PreserveWhitespace);
            var set = doc.Element("nstaller2");
            Settings settings = new Settings
            (
                set.Element("bindir").Value,
                set.Element("checkdotnet").Value,
                set.Element("checkagent").Value,
                set.Element("checkav").Value,
                set.Element("checkprobe").Value,
                set.Element("bindotnet").Value,
                set.Element("binagent").Value,
                set.Element("binav86").Value,
                set.Element("binav64").Value,
                set.Element("binprobe").Value
            );

            return settings;
        }
    }
}
