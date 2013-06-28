using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using Spyder.Core;

namespace Spyder {
    class Program {
        static void Main (string[] args) {
            //httpclient();
            //getD2PassSitesInit();
            //getD2PassSites();
            D2PassProvider d2 = new D2PassProvider();
            d2.execute(false);
        }

        static void done () {
            Dao.getInstance().deleteByQuery(new Movie(), "n0499");
            TokyoHotProvider tkh = new TokyoHotProvider();
            tkh.execute(false);
            TokyoHotTKProvider ttk = new TokyoHotTKProvider();
            ttk.execute(false);
        }

        static void httpclient () {
            //String url = "https://sns.d2pass.com/ajax/reviewsByAll?page=2&limit=10&_=1351616910408";
            String url = "https://sns.d2pass.com/ajax/ajaxSearch/?k=+&rfrom=&rto=&bm_promo=&bm=0&f=all&s=new&p=6";
            var request = (HttpWebRequest) WebRequest.Create(url);
            request.Method = "GET";
            request.Referer = "http://sns.d2pass.com/review/latest";

            //var response = (HttpWebResponse) request.GetResponse();
            //String responseText = "";
            //using (var streamReader = new StreamReader(response.GetResponseStream())) {
            //responseText = streamReader.ReadToEnd();
            //System.Diagnostics.Debug.Write(responseText);
            paserHTML();
            //desJSON(responseText);
            //}

        }

        static void getD2PassSitesInit () {
            String url = "http://www.d2pass.com/guide/2/";
            var request = (HttpWebRequest) WebRequest.Create(url);
            request.Method = "GET";
            request.Referer = "http://www.d2pass.com/guest/";
            var response = (HttpWebResponse) request.GetResponse();
            String responseText = "";
            using (var streamReader = new StreamReader(response.GetResponseStream())) {
                responseText = streamReader.ReadToEnd();
            }
            System.Diagnostics.Debug.Write(responseText);
        }

        static void getD2PassSites () {
            AVSyncerDataContext dc = new AVSyncerDataContext();
            HtmlDocument doc = new HtmlDocument();
            doc.Load(@"..\..\sites.html", Encoding.UTF8);
            var sitesNodes = doc.DocumentNode.CssSelect(".list-siteinfo");
            foreach (var siteinfo in sitesNodes) {
                try {
                    var maker = new Maker();
                    maker.name = siteinfo.CssSelect(".sitename-label").First().InnerText.Trim();
                    maker.description = siteinfo.CssSelect(".comment").First().InnerText.Trim();
                    maker.url = siteinfo.CssSelect(".sitename-label").First().GetNextSibling("td").CssSelect("a").First().Attributes["href"].Value;
                    dc.Maker.InsertOnSubmit(maker);
                    dc.SubmitChanges();
                } catch (Exception e) {
                    System.Diagnostics.Debug.Write(e.Message + "\n" + e.StackTrace + "\n");
                    continue;
                }

            }

            System.Diagnostics.Debug.Write("\n");
        }

        static void paserHTML () {
            try {
                HtmlDocument doc = new HtmlDocument();
                doc.Load(@"..\..\doc.html", Encoding.UTF8);
                //doc.Load(stream, Encoding.UTF8);
                if (doc.ParseErrors != null && doc.ParseErrors.Count() > 0) {
                    // Handle any parse errors as required
                    System.Diagnostics.Debug.Write(doc.ParseErrors.Count());
                }
                //HtmlNode node = doc.DocumentNode.SelectSingleNode("//div");

                var maker = new Maker();
                var moviePanels = doc.DocumentNode.CssSelect(".movie-panel");
                foreach (var moviePanel in moviePanels) {
                    var sitename = moviePanel.CssSelect("mPanel-sitename");
                    foreach (var node in sitename) {
                        maker.name = node.InnerText;
                        maker.description = node.InnerText;
                    }
                }

                System.Diagnostics.Debug.Write("---------------------\n");
                //System.Diagnostics.Debug.Write(node.Attributes["id"].Value + "\n");
                System.Diagnostics.Debug.Write("---------------------\n");
            } catch (Exception e) {
                System.Diagnostics.Debug.Write(e.StackTrace);
            }
        }

        static void desJSON (string json) {
            try {
                AVSyncerDataContext dc = new AVSyncerDataContext();
                Dictionary<string, int> MakerList = new Dictionary<string, int>();

                var tt = json.Substring(6, json.Length - 9);
                JavaScriptSerializer ser = new JavaScriptSerializer();
                Item xx = ser.Deserialize<Item>(tt);
                foreach (var review in xx.reviews) {
                    if (!MakerList.ContainsKey(review.site_name)) {
                        MakerList.Add(review.site_name, 0);
                    }
                }
                //foreach (var key in MakerList.Keys) {
                //    Maker maker = new Maker();
                //    maker.name = key;
                //    maker.description = key;
                //    dc.Maker.InsertOnSubmit(maker);
                //    dc.SubmitChanges();
                //}
                System.Diagnostics.Debug.Write("");
            } catch (Exception e) {
                System.Diagnostics.Debug.Write(e.StackTrace);
            }
        }

    }
}
