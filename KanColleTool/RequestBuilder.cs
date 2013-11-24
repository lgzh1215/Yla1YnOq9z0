using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Web;
using System.Threading;

namespace KanColleTool {
    class RequestBuilder {

        private Fiddler.Session oS;
        public string ServerIP { get; set; }
        public string Token { get; set; }

        public RequestBuilder (Fiddler.Session oS) {
            ServerIP = oS.host;
            NameValueCollection form = HttpUtility.ParseQueryString(oS.GetRequestBodyAsString());
            Token = form["api_token"];
            this.oS = oS;
        }

        public void GotoHomeport () {
            DoActionlog();
            DoLoginCheck();
            DoMaterial();
            DoDeckPort();
            DoNDock();
            DoShip2();
            DoBasic();
        }

        public void DoActionlog () {
            string body = "api%5Fverno=1&api%5Ftoken=" + Token;
            RequestTemplate("api_get_member/actionlog", body);
            Thread.Sleep(100);
        }

        public void DoLoginCheck () {
            string body = "api%5Fverno=1&api%5Ftoken=" + Token;
            RequestTemplate("api_auth_member/logincheck", body);
            Thread.Sleep(100);
        }

        public void DoMaterial () {
            string body = "api%5Fverno=1&api%5Ftoken=" + Token;
            RequestTemplate("api_get_member/material", body);
            Thread.Sleep(100);
        }

        public void DoDeckPort () {
            string body = "api%5Fverno=1&api%5Ftoken=" + Token;
            RequestTemplate("api_get_member/deck_port", body);
            Thread.Sleep(100);
        }

        public void DoNDock () {
            string body = "api%5Fverno=1&api%5Ftoken=" + Token;
            RequestTemplate("api_get_member/ndock", body);
            Thread.Sleep(100);
        }

        public void DoShip2 () {
            string body = "api%5Fsort%5Forder=2&api%5Fsort%5Fkey=2&api%5Fverno=1&api%5Ftoken=" + Token;
            RequestTemplate("api_get_member/ship2", body);
            Thread.Sleep(300);
        }

        public void DoBasic () {
            string body = "api%5Fverno=1&api%5Ftoken=" + Token;
            RequestTemplate("api_get_member/basic", body);
            Thread.Sleep(100);
        }

        public void RequestTemplate (string path, string body) {
            string referer = "http://" + ServerIP + "/kcs/port.swf?version=1.5.1";
            try {
                //Create request to URL.
                string requestUriString = "http://" + ServerIP + "/kcsapi/" + path;
                HttpWebRequest request = (HttpWebRequest) WebRequest.Create(requestUriString);

                //Set request headers.
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "zh-tw,zh;q=0.8,en-us;q=0.5,en;q=0.3");
                request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
                request.KeepAlive = true;
                request.Referer = referer;
                request.ContentType = "application/x-www-form-urlencoded";

                request.Method = "POST";
                request.ServicePoint.Expect100Continue = false;

                //Set request body.
                byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(body);
                request.ContentLength = postBytes.Length;
                Stream stream = request.GetRequestStream();
                stream.Write(postBytes, 0, postBytes.Length);
                stream.Close();
            } catch (Exception e) {
                throw new Exception("KanColleTool post data error!", e);
            }
        }

    }
}
