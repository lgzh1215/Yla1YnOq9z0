using System;
using System.Collections.Generic;
using System.Text;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using Spyder.Utils;

namespace Spyder.Core {
    class TokyoHotProvider : VirtualTokyoHotProvider {

        public TokyoHotProvider ()
            : base() {
            startPage = "http://www.tokyo-hot.com/j/new_video0000_j.html";
            //startPage = @"..\..\th.htm";
        }

        public override void execute (bool isUpdate) {
            HtmlDocument html = HAPUtil.Load(startPage, webCharset);
            //HtmlDocument html = HAPUtil.Load(startPage, webCharset, true);
            foreach (var e in html.DocumentNode.CssSelect("body table table table")) {
                var mvid = "";
                foreach (var a in e.CssSelect("a")) {
                    if (!a.Attributes.Contains("href"))
                        continue;
                    var href = a.Attributes["href"].Value;
                    mvid = RegexUtil.MatcheGroups("n[0-9]{4}", href, 0);
                    Movie mv = dao.findMovieByMvid(mvid);
                    if (mvid != "" && (mv == null || isUpdate)) {
                        HtmlNode titleNode = null;
                        HtmlNode actressNode = null;
                        if (mv == null) {
                            mv = new Movie();
                        }
                        List<Actress> actrs = new List<Actress>();
                        Series series = getSeries(getSeriesNode(e));
                        Label label = getLabel(getLabelNode(e));
                        mv.mvid = mvid;
                        mv.pageHref = getPageHref(href);
                        mv.publishDate = getPublishDate(getPublishDateNode(e));
                        mv.releaseDate = mv.publishDate;
                        mv.thumbnail = getThumbnail(getThumbnailNode(e));
                        mv.coverHref = getCoverHref(mv);

                        if (series != null) {
                            titleNode = e.SelectSingleNode("./td/tr/td/tr/td/div");
                            actressNode = e.SelectSingleNode("./td/tr/td/div");
                        } else {
                            titleNode = e.SelectSingleNode("./td/tr/td/div");
                            actressNode = e.SelectSingleNode("./td/tr/td/tr/td/div");
                        }
                        mv.title = getTitle(titleNode);
                        var xpName = getActressName(actressNode);
                        ActressMovie actmv = new ActressMovie();
                        foreach (var xpA in xpName) {
                            Actress actr = new Actress();
                            actr.name = xpA;
                            actr = dao.QueryOrInsert(actr);
                            actrs.Add(actr);
                        }
                        mv.Maker = maker;
                        mv.Series = series;
                        mv.Label = label;
                        mv = dao.QueryOrInsert(mv);
                        var order = 0;
                        foreach (var actr in actrs) {
                            ActressMovie amv = new ActressMovie();
                            amv.actressId = actr.id;
                            amv.actressOrder = order++;
                            amv.movieId = mv.id;
                            dao.insertOrUpdate(amv);
                        }
                    }
                }
            }
        }

        protected override HtmlNode getLabelNode (HtmlNode e) {
            return e.SelectSingleNode("./td/tr/td/tr/td/tr/td/tr/td/tr/td/div");
        }

        protected override HtmlNode getPublishDateNode (HtmlNode e) {
            return e.SelectSingleNode("./td/tr/td/tr/td/tr/td/div");
        }

        protected override string getCoverHref (Movie movie) {
            string attrName = "src";
            HtmlDocument html = HAPUtil.Load(movie.pageHref, webCharset);
            HtmlNode node = null;
            // swf
            var param = html.DocumentNode.SelectNodes("/html/body/table/td/table[10]//object/param");
            if (param == null) {
                param = html.DocumentNode.SelectNodes("/html/body/table/td/table[9]//object/param");
            }
            if (param == null) {
                param = html.DocumentNode.SelectNodes("/html/body/table/td/table[8]//object/param");
            }
            if (param != null) {
                foreach (HtmlNode p in param) {
                    if (p.Attributes["name"].Value == "movie") {
                        node = p;
                        attrName = "value";
                        break;
                    }
                }
            }
            // jpg
            if (node == null) {
                node = html.DocumentNode.SelectSingleNode("/html/body/table/td/table[9]//img[1]");
            }
            if (node == null) {
                node = html.DocumentNode.SelectSingleNode("/html/body/table/td/table[8]//img[1]");
            }
            if (node == null) {
                node = html.DocumentNode.SelectSingleNode("/html/body/table/td/table[10]//img[1]");
            }
            // head swf
            if (node == null && param == null || RegexUtil.MatcheGroups(@"\.gif$", node.Attributes[attrName].Value, 0) == ".gif") {
                param = html.DocumentNode.SelectNodes("/html/body/table/td/table[3]//td/td//object/param");
                if (param != null) {
                    foreach (HtmlNode p in param) {
                        if (p.Attributes["name"].Value == "movie") {
                            node = p;
                            attrName = "value";
                            break;
                        }
                    }
                }
            }
            string imgUrl = node.Attributes[attrName].Value;
            Uri coverUri = new Uri(pageBaseUri, imgUrl);
            return coverUri.ToString();
        }

        protected override string getTitle (HtmlNode node) {
            var text = node.InnerText.Trim();
            return text;
        }

        protected override Series getSeries (HtmlNode node) {
            Series result = null;
            var text = RegexUtil.MatcheGroups(@"^(鬼(逝?汁?)).-", node.InnerText, 1);
            if (text != "") {
                Series ser = new Series();
                ser.name = text;
                result = dao.QueryOrInsert(ser);
            }
            return result;
        }

        protected override List<string> getActressName (HtmlNode node) {
            List<string> result = new List<string>();
            var text = node.InnerText;
            var p1 = RegexUtil.MatcheGroups(@"\-\-[ |　](.*)\-\-", text, 1).Trim();
            var p2 = RegexUtil.MatcheGroups(@"^鬼(逝?汁?)[ |　]\-[ |　](.*)", text, 2).Trim();
            if (p1 != "") {
                result.Add(p1.Trim());
            } else if (p2 != "") {
                result.Add(p2.Trim());
            } else {
                foreach (var p3 in text.Split(',')) {
                    result.Add(p3.Trim());
                }
            }
            return result;
        }

    }
}
