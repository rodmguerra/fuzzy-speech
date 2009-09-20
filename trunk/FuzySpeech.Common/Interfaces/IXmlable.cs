using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace FuzzySpeech.Common.Interfaces
{
    public interface IXmlable
    {
        void FromXml(XmlDocument xmlDocument);
        XmlDocument ToXml();
    }
}
