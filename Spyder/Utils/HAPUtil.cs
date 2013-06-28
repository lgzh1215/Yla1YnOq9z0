using System.IO;
using System.Net;
using System.Text;
using HtmlAgilityPack;
using System;

namespace Spyder.Utils {
    class HAPUtil {

        public static HtmlDocument Load (string startPage, Encoding encoding, bool isLocal = false) {
            HtmlDocument html = new HtmlDocument();
            if (isLocal) {
                html.Load(startPage, encoding);
            } else {
                Load(startPage, encoding);
            }
            return html;
        }

        private static HtmlDocument Load (string requestUri, Encoding encoding) {
            var doc = new HtmlDocument();
            WebClient client = new WebClient();
            Uri site = new Uri(requestUri);
            using (Stream dataStream = client.OpenRead(site))
            using (TextReader reader = new StreamReader(dataStream, encoding)) {
                doc.Load(reader);
            }
            return doc;
        }

        public static HtmlDocument D2PassRequest (string requestUri, Encoding encoding) {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(requestUri);
            request.Method = "GET";
            request.Referer = "http://sns.d2pass.com/review/latest";

            HttpWebResponse response = (HttpWebResponse) request.GetResponse();
            HtmlDocument doc = new HtmlDocument();
            doc.Load(response.GetResponseStream(), encoding);
            return doc;
        }


    }
}
