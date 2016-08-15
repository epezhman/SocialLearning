using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace UT.SL.Helper.Common
{
    public static class XSSUtils
    {
        public static string ParseXmlAttrs(XElement element)
        {
            string temp = "";
            foreach (var attr in element.Attributes())
            {
                if (!attr.Value.ToLower().Contains("javascript") && attr.Name.ToString().ToLower().StartsWith("on"))
                {
                    if (element.Name.ToString().ToLower() == "img" && attr.Name.ToString().ToLower() == "src" && attr.Value.ToLower().Contains("://"))
                    {

                    }
                    else
                        temp += string.Format(" {0}='{1}' ", attr.Name, attr.Value);
                }
            }

            return temp;
        }

        public static string ParseXmlNode(XElement element)
        {
            if (element.Name.ToString().ToLower() == "script")
                return System.Web.HttpUtility.HtmlEncode(element.ToString());
            return string.Format("<{0} {1}>{2}</{0}>", element.Name, ParseXmlAttrs(element), element.Value);
        }

        public static string TraceXml(XElement element, int n)
        {
            if (n > 20) return "";

            string temp = "";
            if (element.Elements().Count() == 0)
                return ParseXmlNode(element);
            else
            {
                string nested = "<{0} {1}>\r\n{2}\r\n</{0}>";

                foreach (var el in element.Elements())
                {
                    temp += TraceXml(el, n + 1);
                }

                nested = string.Format(nested, element.Name.ToString(), ParseXmlAttrs(element), temp);
                return nested;
            }
        }

        public static string StripHTMLXSS(this string htmltext)
        {
            string text = "<xml>" + htmltext.Replace("&nbsp;", " ") + "</xml>";
            string temp = "";
            try
            {
                XDocument html = XDocument.Parse(text);

                foreach (var root in html.Elements())
                    foreach (var element in root.Elements())
                    {

                        temp += string.Format("{0}", TraceXml(element, 1));
                    }

                return temp;
            }
            catch 
            {
                return StripHTMLXSS(StripHTMLXSS(htmltext));
            }
        }

        static string StripXSSText(string text)
        {
            return text.Replace("script", " ").Replace("onload", " ").Replace("javascript", " ");
        }

        public static bool IsValidReturnUrl(this string url)
        {
            string[] blacklist = { "http", "://", "//", "www", "..", "/\\" };
            bool rs = !string.IsNullOrEmpty(url);
            for (int i = 0; i < blacklist.Length && rs; i++)
            {
                var b = blacklist[i];
                rs = rs && !url.Contains(b);
            }
            return rs;
        }
    }
}
