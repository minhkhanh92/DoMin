using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Do_min
{
    public class XL_XML
    {
        public static XmlElement Doc_goc(string duongdan)
        {
            XmlElement goc = null;
            XmlDocument doc = new XmlDocument();
            doc.Load(duongdan);
            goc = doc.DocumentElement;

            return goc;
        }

        public static XmlElement Tao_goc(string tenthe)
        {
            XmlElement goc = null;
            XmlDocument tailieu = new XmlDocument();
            goc = tailieu.CreateElement(tenthe);
            tailieu.AppendChild(goc);

            return goc;
        }

        public static XmlElement Tao_nut(string tenthe, XmlElement nutcha)
        {
            XmlElement nut = null;
            XmlDocument tailieu = nutcha.OwnerDocument;
            nut = tailieu.CreateElement(tenthe);
            nutcha.AppendChild(nut);

            return nut;
        }

        public static XmlElement Tao_nut(XmlElement nutnguon, XmlElement nutcha, bool dequi)
        {
            XmlElement nut = null;
            XmlDocument tailieu = nutcha.OwnerDocument;
            nut = (XmlElement)tailieu.ImportNode(nutnguon, dequi);
            nutcha.AppendChild(nut);

            return nut;
        }

        public static void Ghi_nut(XmlElement nut, string duongdan)
        {
            XmlDocument tailieu = nut.OwnerDocument;
            tailieu.Save(duongdan);
        }

    }
}
