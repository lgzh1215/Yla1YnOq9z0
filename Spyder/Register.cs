using System;
using System.IO;
using System.Net;
using System.Text;
using HtmlAgilityPack;

namespace SSWSyncer {

    class Register {

        private Encoding encoding = Encoding.GetEncoding("utf-8");

        private WebClient webClient = new WebClient();

        public HtmlDocument SSWRequest () {
            Encoding encoding = Encoding.GetEncoding("utf-8");
            String requestUri = "http://passport.xyz-soft.com/users/new";
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(requestUri);
            request.Method = "GET";
            request.Referer = "http://passport.xyz-soft.com/users/new";

            HttpWebResponse response = (HttpWebResponse) request.GetResponse();
            HtmlDocument doc = new HtmlDocument();
            doc.Load(response.GetResponseStream(), encoding);
            return doc;
        }

        public HtmlDocument SSWRequest2 () {
            String requestUri = "http://passport.xyz-soft.com/register";
            webClient.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.31 (KHTML, like Gecko) Chrome/26.0.1410.64 Safari/537.31");
            Stream stream = webClient.OpenRead(requestUri);
            HtmlDocument doc = new HtmlDocument();
            doc.Load(stream, encoding);
            return doc;
        }
    }
}
