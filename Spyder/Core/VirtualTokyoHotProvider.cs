using System;
using System.Collections.Generic;
using System.Text;
using HtmlAgilityPack;
using Spyder.Utils;

namespace Spyder.Core {
    class VirtualTokyoHotProvider : AbstractProvider {

        public VirtualTokyoHotProvider () {
            maker = getMaker();
            siteBaseUri = new Uri("http://www.tokyo-hot.com/");
            pageBaseUri = new Uri("http://www.tokyo-hot.com/j/");
            webCharset = Encoding.GetEncoding("Shift_JIS");
        }

        public virtual void execute (bool isUpdate) {
            throw new NotImplementedException();
        }

        protected HtmlNode getSeriesNode (HtmlNode e) {
            return e.SelectSingleNode("./td/tr/td/div");
        }

        protected HtmlNode getThumbnailNode (HtmlNode e) {
            return e.SelectSingleNode("./td/div/a/img");
        }

        protected virtual HtmlNode getLabelNode (HtmlNode e) {
            throw new NotImplementedException();
        }

        protected virtual HtmlNode getPublishDateNode (HtmlNode e) {
            throw new NotImplementedException();
        }

        protected string getThumbnail (HtmlNode node) {
            string imgUrl = node.Attributes["src"].Value;
            Uri thumbnailUri = new Uri(siteBaseUri, imgUrl);
            return thumbnailUri.ToString();
        }

        protected string getPageHref (string href) {
            Uri pageUri = new Uri(pageBaseUri, href);
            return pageUri.ToString();
        }

        protected Maker getMaker () {
            Maker result = null;
            Maker mak = new Maker();
            mak.name = "TOKYO-HOT";
            result = dao.QueryOrInsert(mak);
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

        protected virtual string getTitle (HtmlNode node) {
            throw new NotImplementedException();
        }

        protected virtual string getCoverHref (Movie movie) {
            throw new NotImplementedException();
        }

        protected virtual Series getSeries (HtmlNode node) {
            throw new NotImplementedException();
        }

        protected virtual List<string> getActressName (HtmlNode node) {
            throw new NotImplementedException();
        }

    }
}
