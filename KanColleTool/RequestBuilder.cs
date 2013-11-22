using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace KanColleTool {
    class RequestBuilder {

        public string ServerIP {get; set;}

        private void MakeRequests () {
            HttpWebResponse response;

            if (Request_125_6_189_39(out response)) {
                response.Close();
            } else {

            }

            if (Request_125_6_189_39_material(out response)) {
                response.Close();
            } else {
                
            }

            if (Request_125_6_189_39_deck_port(out response)) {
                response.Close();
            } else {
                
            }

            if (Request_125_6_189_39_ndock(out response)) {
                response.Close();
            } else {
                
            }

            if (Request_125_6_189_39_ship2(out response)) {
                response.Close();
            } else {
                
            }

            if (Request_125_6_189_39_basic(out response)) {
                response.Close();
            } else {
                
            }
        }

        private void RequestTemplate (string path, string body, out HttpWebResponse response) {
            response = null;
            string referer = "http://"+ ServerIP + "/kcs/port.swf?version=1.5.1";
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
                body = @"api%5Fverno=1&api%5Ftoken=ba3d6246159e29c3df781481d72549e3bb6a6d19";
                byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(body);
                request.ContentLength = postBytes.Length;
                Stream stream = request.GetRequestStream();
                stream.Write(postBytes, 0, postBytes.Length);
                stream.Close();

                //Get response to request.
                response = (HttpWebResponse) request.GetResponse();
            } catch (WebException e) {
                //ProtocolError indicates a valid HTTP response, but with a non-200 status code (e.g. 304 Not Modified, 404 Not Found)
                if (e.Status == WebExceptionStatus.ProtocolError) {
                    response = (HttpWebResponse) e.Response;
                }
            } catch (Exception e) {
                if (response != null)
                    response.Close();
                throw new Exception("KanColleTool post data error!", e);
            }
        }


        /// <summary>
        /// Tries to request the URL: http://125.6.189.39/kcsapi/api_auth_member/logincheck
        /// </summary>
        /// <param name="response">After the function has finished, will possibly contain the response to the request.</param>
        /// <returns>True if the request was successful; false otherwise.</returns>
        private bool Request_125_6_189_39 (out HttpWebResponse response) {
            response = null;

            try {
                //Create request to URL.
                HttpWebRequest request = (HttpWebRequest) WebRequest.Create("http://125.6.189.39/kcsapi/api_auth_member/logincheck");

                //Set request headers.
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "zh-tw,zh;q=0.8,en-us;q=0.5,en;q=0.3");
                request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
                request.KeepAlive = true;
                request.Referer = "http://125.6.189.39/kcs/port.swf?version=1.5.1";
                request.ContentType = "application/x-www-form-urlencoded";

                //Set request method
                request.Method = "POST";

                // Disable 'Expect: 100-continue' behavior. More info: http://haacked.com/archive/2004/05/15/http-web-request-expect-100-continue.aspx
                request.ServicePoint.Expect100Continue = false;

                //Set request body.
                string body = @"api%5Fverno=1&api%5Ftoken=ba3d6246159e29c3df781481d72549e3bb6a6d19";
                byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(body);
                request.ContentLength = postBytes.Length;
                Stream stream = request.GetRequestStream();
                stream.Write(postBytes, 0, postBytes.Length);
                stream.Close();

                //Get response to request.
                response = (HttpWebResponse) request.GetResponse();
            } catch (WebException e) {
                //ProtocolError indicates a valid HTTP response, but with a non-200 status code (e.g. 304 Not Modified, 404 Not Found)
                if (e.Status == WebExceptionStatus.ProtocolError)
                    response = (HttpWebResponse) e.Response;
                else
                    return false;
            } catch (Exception) {
                if (response != null)
                    response.Close();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tries to request the URL: http://125.6.189.39/kcsapi/api_get_member/material
        /// </summary>
        /// <param name="response">After the function has finished, will possibly contain the response to the request.</param>
        /// <returns>True if the request was successful; false otherwise.</returns>
        private bool Request_125_6_189_39_material (out HttpWebResponse response) {
            response = null;

            try {
                //Create request to URL.
                HttpWebRequest request = (HttpWebRequest) WebRequest.Create("http://125.6.189.39/kcsapi/api_get_member/material");

                //Set request headers.
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "zh-tw,zh;q=0.8,en-us;q=0.5,en;q=0.3");
                request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
                request.KeepAlive = true;
                request.Referer = "http://125.6.189.39/kcs/port.swf?version=1.5.1";
                request.ContentType = "application/x-www-form-urlencoded";

                //Set request method
                request.Method = "POST";

                // Disable 'Expect: 100-continue' behavior. More info: http://haacked.com/archive/2004/05/15/http-web-request-expect-100-continue.aspx
                request.ServicePoint.Expect100Continue = false;

                //Set request body.
                string body = @"api%5Fverno=1&api%5Ftoken=ba3d6246159e29c3df781481d72549e3bb6a6d19";
                byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(body);
                request.ContentLength = postBytes.Length;
                Stream stream = request.GetRequestStream();
                stream.Write(postBytes, 0, postBytes.Length);
                stream.Close();

                //Get response to request.
                response = (HttpWebResponse) request.GetResponse();
            } catch (WebException e) {
                //ProtocolError indicates a valid HTTP response, but with a non-200 status code (e.g. 304 Not Modified, 404 Not Found)
                if (e.Status == WebExceptionStatus.ProtocolError)
                    response = (HttpWebResponse) e.Response;
                else
                    return false;
            } catch (Exception) {
                if (response != null)
                    response.Close();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tries to request the URL: http://125.6.189.39/kcsapi/api_get_member/deck_port
        /// </summary>
        /// <param name="response">After the function has finished, will possibly contain the response to the request.</param>
        /// <returns>True if the request was successful; false otherwise.</returns>
        private bool Request_125_6_189_39_deck_port (out HttpWebResponse response) {
            response = null;

            try {
                //Create request to URL.
                HttpWebRequest request = (HttpWebRequest) WebRequest.Create("http://125.6.189.39/kcsapi/api_get_member/deck_port");

                //Set request headers.
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "zh-tw,zh;q=0.8,en-us;q=0.5,en;q=0.3");
                request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
                request.KeepAlive = true;
                request.Referer = "http://125.6.189.39/kcs/port.swf?version=1.5.1";
                request.ContentType = "application/x-www-form-urlencoded";

                //Set request method
                request.Method = "POST";

                // Disable 'Expect: 100-continue' behavior. More info: http://haacked.com/archive/2004/05/15/http-web-request-expect-100-continue.aspx
                request.ServicePoint.Expect100Continue = false;

                //Set request body.
                string body = @"api%5Fverno=1&api%5Ftoken=ba3d6246159e29c3df781481d72549e3bb6a6d19";
                byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(body);
                request.ContentLength = postBytes.Length;
                Stream stream = request.GetRequestStream();
                stream.Write(postBytes, 0, postBytes.Length);
                stream.Close();

                //Get response to request.
                response = (HttpWebResponse) request.GetResponse();
            } catch (WebException e) {
                //ProtocolError indicates a valid HTTP response, but with a non-200 status code (e.g. 304 Not Modified, 404 Not Found)
                if (e.Status == WebExceptionStatus.ProtocolError)
                    response = (HttpWebResponse) e.Response;
                else
                    return false;
            } catch (Exception) {
                if (response != null)
                    response.Close();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tries to request the URL: http://125.6.189.39/kcsapi/api_get_member/ndock
        /// </summary>
        /// <param name="response">After the function has finished, will possibly contain the response to the request.</param>
        /// <returns>True if the request was successful; false otherwise.</returns>
        private bool Request_125_6_189_39_ndock (out HttpWebResponse response) {
            response = null;

            try {
                //Create request to URL.
                HttpWebRequest request = (HttpWebRequest) WebRequest.Create("http://125.6.189.39/kcsapi/api_get_member/ndock");

                //Set request headers.
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "zh-tw,zh;q=0.8,en-us;q=0.5,en;q=0.3");
                request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
                request.KeepAlive = true;
                request.Referer = "http://125.6.189.39/kcs/port.swf?version=1.5.1";
                request.ContentType = "application/x-www-form-urlencoded";

                //Set request method
                request.Method = "POST";

                // Disable 'Expect: 100-continue' behavior. More info: http://haacked.com/archive/2004/05/15/http-web-request-expect-100-continue.aspx
                request.ServicePoint.Expect100Continue = false;

                //Set request body.
                string body = @"api%5Fverno=1&api%5Ftoken=ba3d6246159e29c3df781481d72549e3bb6a6d19";
                byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(body);
                request.ContentLength = postBytes.Length;
                Stream stream = request.GetRequestStream();
                stream.Write(postBytes, 0, postBytes.Length);
                stream.Close();

                //Get response to request.
                response = (HttpWebResponse) request.GetResponse();
            } catch (WebException e) {
                //ProtocolError indicates a valid HTTP response, but with a non-200 status code (e.g. 304 Not Modified, 404 Not Found)
                if (e.Status == WebExceptionStatus.ProtocolError)
                    response = (HttpWebResponse) e.Response;
                else
                    return false;
            } catch (Exception) {
                if (response != null)
                    response.Close();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tries to request the URL: http://125.6.189.39/kcsapi/api_get_member/ship2
        /// </summary>
        /// <param name="response">After the function has finished, will possibly contain the response to the request.</param>
        /// <returns>True if the request was successful; false otherwise.</returns>
        private bool Request_125_6_189_39_ship2 (out HttpWebResponse response) {
            response = null;

            try {
                //Create request to URL.
                HttpWebRequest request = (HttpWebRequest) WebRequest.Create("http://125.6.189.39/kcsapi/api_get_member/ship2");

                //Set request headers.
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "zh-tw,zh;q=0.8,en-us;q=0.5,en;q=0.3");
                request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
                request.KeepAlive = true;
                request.Referer = "http://125.6.189.39/kcs/port.swf?version=1.5.1";
                request.ContentType = "application/x-www-form-urlencoded";

                //Set request method
                request.Method = "POST";

                // Disable 'Expect: 100-continue' behavior. More info: http://haacked.com/archive/2004/05/15/http-web-request-expect-100-continue.aspx
                request.ServicePoint.Expect100Continue = false;

                //Set request body.
                string body = @"api%5Fsort%5Forder=2&api%5Fsort%5Fkey=2&api%5Fverno=1&api%5Ftoken=ba3d6246159e29c3df781481d72549e3bb6a6d19";
                byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(body);
                request.ContentLength = postBytes.Length;
                Stream stream = request.GetRequestStream();
                stream.Write(postBytes, 0, postBytes.Length);
                stream.Close();

                //Get response to request.
                response = (HttpWebResponse) request.GetResponse();
            } catch (WebException e) {
                //ProtocolError indicates a valid HTTP response, but with a non-200 status code (e.g. 304 Not Modified, 404 Not Found)
                if (e.Status == WebExceptionStatus.ProtocolError)
                    response = (HttpWebResponse) e.Response;
                else
                    return false;
            } catch (Exception) {
                if (response != null)
                    response.Close();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tries to request the URL: http://125.6.189.39/kcsapi/api_get_member/basic
        /// </summary>
        /// <param name="response">After the function has finished, will possibly contain the response to the request.</param>
        /// <returns>True if the request was successful; false otherwise.</returns>
        private bool Request_125_6_189_39_basic (out HttpWebResponse response) {
            response = null;

            try {
                //Create request to URL.
                HttpWebRequest request = (HttpWebRequest) WebRequest.Create("http://125.6.189.39/kcsapi/api_get_member/basic");

                //Set request headers.
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "zh-tw,zh;q=0.8,en-us;q=0.5,en;q=0.3");
                request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
                request.KeepAlive = true;
                request.Referer = "http://125.6.189.39/kcs/port.swf?version=1.5.1";
                request.ContentType = "application/x-www-form-urlencoded";

                //Set request method
                request.Method = "POST";

                // Disable 'Expect: 100-continue' behavior. More info: http://haacked.com/archive/2004/05/15/http-web-request-expect-100-continue.aspx
                request.ServicePoint.Expect100Continue = false;

                //Set request body.
                string body = @"api%5Fverno=1&api%5Ftoken=ba3d6246159e29c3df781481d72549e3bb6a6d19";
                byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(body);
                request.ContentLength = postBytes.Length;
                Stream stream = request.GetRequestStream();
                stream.Write(postBytes, 0, postBytes.Length);
                stream.Close();

                //Get response to request.
                response = (HttpWebResponse) request.GetResponse();
            } catch (WebException e) {
                //ProtocolError indicates a valid HTTP response, but with a non-200 status code (e.g. 304 Not Modified, 404 Not Found)
                if (e.Status == WebExceptionStatus.ProtocolError)
                    response = (HttpWebResponse) e.Response;
                else
                    return false;
            } catch (Exception) {
                if (response != null)
                    response.Close();
                return false;
            }

            return true;
        }
    }
}
