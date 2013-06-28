using System;
using System.Text;


namespace Spyder.Core {
    public abstract class AbstractProvider {
        public AbstractProvider () {
            this.dao = Dao.getInstance();
        }

        protected Encoding webCharset { get; set; }
        protected string startPage { get; set; }
        protected Dao dao { get; set; }
        protected Maker maker { get; set; }
        protected Uri siteBaseUri { get; set; }
        protected Uri pageBaseUri { get; set; }
    }
}
