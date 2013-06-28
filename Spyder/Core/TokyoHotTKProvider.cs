using System;
using System.Collections;
using System.Collections.Generic;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using Spyder.Utils;

namespace Spyder.Core {
    class TokyoHotTKProvider : VirtualTokyoHotProvider {

        public TokyoHotTKProvider ()
            : base() {
            startPage = "http://www.tokyo-hot.com/j/k_video0000_j.html";
            //startPage = @"..\..\thk.htm";
        }

        public override void execute (bool isUpdate) {
            HtmlDocument html = HAPUtil.Load(startPage, webCharset);
            //HtmlDocument html = HAPUtil.Load(startPage, webCharset, true);
            ArrayList selectors = new ArrayList(new string[] { "body table table", "body table table table" });
            foreach (var s in selectors) {
                try {
                    foreach (var e in html.DocumentNode.CssSelect((string) s)) {
                        var mvid = "";
                        foreach (var a in e.CssSelect("a")) {
                            if (!a.Attributes.Contains("href"))
                                continue;
                            var href = a.Attributes["href"].Value;
                            mvid = RegexUtil.MatcheGroups("k[0-9]{4}", href, 0);
                            Movie mv = dao.findMovieByMvid(mvid);
                            if (mvid != "" && (mv == null || isUpdate)) {
                                if (mv == null) {
                                    mv = new Movie();
                                }
                                List<Actress> actrs = new List<Actress>();
                                Series series = getSeries(e);
                                Label label = getLabel(getLabelNode(e));
                                mv.mvid = mvid;
                                mv.pageHref = getPageHref(href);
                                mv.publishDate = getPublishDate(getPublishDateNode(e));
                                mv.releaseDate = mv.publishDate;
                                mv.thumbnail = getThumbnail(getThumbnailNode(e));
                                mv.coverHref = getCoverHref(mv);
                                mv.title = getTitle(getTitleNode(e));

                                var xpName = getActressName(getActressNode(e));
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
                } catch (Exception) {
                    continue;
                }
            }
        }

        private HtmlNode getTitleNode (HtmlNode e) {
            var result = e.SelectSingleNode("./td/tr/td/div");
            if (result == null) {
                result = e.SelectSingleNode("./td/div/tr/td/div");
            }
            return result;
        }

        private HtmlNode getActressNode (HtmlNode e) {
            return getTitleNode(e);
        }

        protected override HtmlNode getLabelNode (HtmlNode e) {
            var result = e.SelectSingleNode("./td/tr/td/tr/td/tr/td/tr/td/div");
            if (result == null) {
                result = e.SelectSingleNode("./td/div/tr/td/tr/td/tr/td/tr/td/div");
            }
            return result;
        }

        protected override HtmlNode getPublishDateNode (HtmlNode e) {
            var result = e.SelectSingleNode("./td/tr/td/tr/td/div");
            if (result == null) {
                result = e.SelectSingleNode("./td/div/tr/td/tr/td/div");
            }
            return result;
        }

        protected override string getCoverHref (Movie movie) {
            string attrName = "value";
            HtmlDocument html = HAPUtil.Load(movie.pageHref, webCharset);
            HtmlNode node = null;
            // head swf
            var param = html.DocumentNode.SelectNodes("/html/body/table/td/table[2]//object/param");
            if (param != null) {
                foreach (HtmlNode p in param) {
                    if (p.Attributes["name"].Value == "movie") {
                        node = p;
                        break;
                    }
                }
            }
            string imgUrl = node.Attributes[attrName].Value;
            Uri coverUri = new Uri(pageBaseUri, imgUrl);
            return coverUri.ToString();
        }

        protected override string getTitle (HtmlNode node) {
            var text = node.InnerText.Trim();
            var p = RegexUtil.MatcheGroups(@"\-[ |　](餌食牝.*)[ |　]\-", text, 1).Trim();
            return p;
        }

        protected override List<string> getActressName (HtmlNode node) {
            List<string> result = new List<string>();
            var text = node.InnerText;
            var p = RegexUtil.MatcheGroups(@"\-[ |　]餌食牝(.*)[ |　]\-", text, 1).Trim();
            if (p != "") {
                result.Add(p.Trim());
            } else {
                throw new NotImplementedException();
            }
            return result;
        }

        protected override Series getSeries (HtmlNode node) {
            Series result = null;
            Series ser = new Series();
            ser.name = "チーム木村";
            result = dao.QueryOrInsert(ser);
            return result;
        }

    }
}
