using System;
using System.Collections.Generic;
using System.Text;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using Spyder.Utils;

namespace Spyder.Core {
    class D2PassProvider : AbstractProvider {

        public D2PassProvider () {
            //siteBaseUri = new Uri("http://www.tokyo-hot.com/");
            //pageBaseUri = new Uri("http://www.tokyo-hot.com/j/");
            webCharset = Encoding.GetEncoding("utf-8");
            startPage = "https://sns.d2pass.com/ajax/ajaxSearch/?k=*&rfrom=&rto=&bm_promo=&bm=0&f=movies&s=new&p=1";
            startPage = "https://sns.d2pass.com/search?k=%E3%83%A2%E3%83%87%E3%82%B3%E3%83%AC+%E6%9D%8F%E5%A0%82%E3%81%AA%E3%81%A4&p=&bm=&f=&s=new&rfrom=&rto=&bm_promo=";
        }

        public virtual void execute (bool isUpdate) {
            HtmlDocument html = HAPUtil.D2PassRequest(startPage, webCharset);
            foreach (var e in html.DocumentNode.CssSelect(".movie-panel")) {
                Maker mak = getMaker(getMakerNode(e));
                if (mak.enable) {
                    Movie mv = new Movie();
                    mv.mvid = getMvid(e);
                    mv.title = getTitle(getTitleNode(e));
                    mv.thumbnail = getThumbnail(getThumbnailNode(e));
                    mv.rating = getRating(getRatingNode(e));
                    var xpName = getActressName(getActressNode(e));
                    List<Actress> actrs = new List<Actress>();
                    ActressMovie actmv = new ActressMovie();
                    foreach (var xpA in xpName) {
                        Actress actr = new Actress();
                        actr.name = xpA;
                        //actr = dao.QueryOrInsert(actr);
                        actrs.Add(actr);
                    }
                }
            }
        }

        protected string getMvid (HtmlNode e) {
            return e.GetAttributeValue("id");
        }

        protected HtmlNode getActressNode (HtmlNode e) {
            return e.SelectSingleNode("./div[@class='mPanel-actor']/a");
        }

        protected HtmlNode getRatingNode (HtmlNode e) {
            return e.SelectSingleNode("./div[@class='review']");
        }

        protected HtmlNode getMakerNode (HtmlNode e) {
            return e.SelectSingleNode("./div[@class='mPanel-sitename']");
        }

        protected HtmlNode getTitleNode (HtmlNode e) {
            return e.SelectSingleNode("./div[@class='mPanel-title']");
        }

        protected HtmlNode getThumbnailNode (HtmlNode e) {
            return e.SelectSingleNode("./div//img[@id='img-transition']");
        }

        protected double getRating (HtmlNode node) {
            Double x = 0.0;
            var rate = node.SelectSingleNode("./div");
            var cls = rate.GetAttributeValue("class");
            var g = RegexUtil.MatcheGetGroup(@"rate_(\d)_(\d)", cls);
            if (g != null) {
                x = Double.Parse(g[1].Value + "." + g[2].Value);
            }
            return x;
        }

        protected Maker getMaker (HtmlNode node) {
            Maker result = null;
            var text = node.InnerText.Trim();
            if (text != "") {
                Maker mak = new Maker();
                mak.name = text;
                result = dao.QueryOrInsert(mak);
            }
            return result;
        }

        protected virtual string getTitle (HtmlNode node) {
            return node.InnerText.Trim();
        }

        protected string getThumbnail (HtmlNode node) {
            string imgUrl = node.Attributes["src"].Value;
            Uri thumbnailUri = new Uri(imgUrl);
            return thumbnailUri.ToString();
        }

        protected virtual List<string> getActressName (HtmlNode node) {
            //var x = "杏堂なつ 春野優 宮川怜 (RAIKA)";
            //var r = RegexUtil.MatcheGroups(@"\(.*\)", x, 0);

            List<string> result = new List<string>();
            var text = node.InnerText;
            var sp = text.Split(new char[] { ' ' });
            foreach (var s in sp) {
                var r = RegexUtil.MatcheGroups(@"\(.*\)", s, 0);
                if (r == "") {
                }
            }
            result.AddRange(sp);
            return result;
        }
        








        protected Label getLabel (HtmlNode node) {
            Label result = null;
            var text = node.InnerText.Trim();
            if (text != "") {
                Label lab = new Label();
                lab.name = text;
                result = dao.QueryOrInsert(lab);
            }
            return result;
        }

        protected DateTime getPublishDate (HtmlNode node) {
            string dateString = RegexUtil.MatcheGroups(@"更新日(\d{2}-\d{2}-\d{4})", node.InnerText, 1);
            string format = "MM-dd-yyyy";
            try {
                var result = DateTime.ParseExact(dateString, format, null);
                return result;
            } catch (FormatException e) {
                Console.WriteLine("{0} is not in the correct format.", dateString);
                throw e;
            }
        }

        protected HtmlNode getSeriesNode (HtmlNode e) {
            return e.SelectSingleNode("./td/tr/td/div");
        }
        
        protected string getPageHref (string href) {
            Uri pageUri = new Uri(pageBaseUri, href);
            return pageUri.ToString();
        }
        protected virtual Series getSeries (HtmlNode node) {
            throw new NotImplementedException();
        }
        protected virtual string getCoverHref (Movie movie) {
            throw new NotImplementedException();
        }
        protected virtual HtmlNode getLabelNode (HtmlNode e) {
            throw new NotImplementedException();
        }
        protected virtual HtmlNode getPublishDateNode (HtmlNode e) {
            throw new NotImplementedException();
        }
    }
}
