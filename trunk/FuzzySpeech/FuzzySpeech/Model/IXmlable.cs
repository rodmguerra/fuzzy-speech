using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace FuzzySpeech.Model
{
    interface IXmlable
    {
        void FromXml(XmlDocument xmlDocument);
        XmlDocument ToXml();
    }
}
