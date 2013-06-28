using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AVSyncer.Movies {
	[XmlRoot]
	public class Movie {
		private int id;
		private String mvid;
		private String title;
		private int maker;
		private int label;
		private int series;
		private String rating;
		private String cid;
		private DateTime publishDate;
		private DateTime releaseDate;
		private String pageHref;
		private String coverHref;
		private List<int> actress = new List<int>();

		public Movie () {
		}

		public Movie (int id, String mvid, String title,
			int makerId, int labelId, int seriesId, String rating,
			String cid, DateTime publishDate, DateTime releaseDate,
			String pageHref, String coverHref) {
			this.id = id;
			this.mvid = mvid;
			this.title = title;
			this.maker = makerId;
			this.label = labelId;
			this.series = seriesId;
			this.rating = rating;
			this.cid = cid;
			this.publishDate = publishDate;
			this.releaseDate = releaseDate;
			this.pageHref = pageHref;
			this.coverHref = coverHref;
		}

		public Movie (System.Xml.Linq.XElement element) {
			this.id = Convert.ToInt32(element.Attribute("Id").Value);
			this.mvid = element.Attribute("Mvid").Value;
			this.title = element.Attribute("Title").Value;
			this.maker = Convert.ToInt32(element.Attribute("Maker").Value);
			this.label = Convert.ToInt32(element.Attribute("Label").Value);
			this.series = Convert.ToInt32(element.Attribute("Series").Value);
			this.rating = element.Attribute("Rating").Value;
			this.cid = element.Attribute("Cid").Value;
			this.publishDate = Convert.ToDateTime(element.Attribute("PublishDate").Value);
			this.releaseDate = Convert.ToDateTime(element.Attribute("ReleaseDate").Value);
			this.pageHref = element.Attribute("PageHref").Value;
			this.coverHref = element.Attribute("CoverHref").Value;
		}

		[XmlAttribute]
		public int Id {
			get {
				return this.id;
			}
			set {
				this.id = value;
			}
		}

		[XmlAttribute]
		public String Mvid {
			get {
				return this.mvid;
			}
			set {
				this.mvid = value;
			}
		}

		[XmlAttribute]
		public String Title {
			get {
				return this.title;
			}
			set {
				this.title = value;
			}
		}

		[XmlAttribute]
		public int Maker {
			get {
				return this.maker;
			}
			set {
				this.maker = value;
			}
		}

		[XmlAttribute]
		public int Label {
			get {
				return this.label;
			}
			set {
				this.label = value;
			}
		}

		[XmlAttribute]
		public int Series {
			get {
				return this.series;
			}
			set {
				this.series = value;
			}
		}

		[XmlAttribute]
		public String Rating {
			get {
				return this.rating;
			}
			set {
				this.rating = value;
			}
		}

		[XmlAttribute]
		public String Cid {
			get {
				return this.cid;
			}
			set {
				this.cid = value;
			}
		}

		[XmlAttribute]
		public DateTime PublishDate {
			get {
				return this.publishDate;
			}
			set {
				this.publishDate = value;
			}
		}

		[XmlAttribute]
		public DateTime ReleaseDate {
			get {
				return this.releaseDate;
			}
			set {
				this.releaseDate = value;
			}
		}

		[XmlAttribute]
		public String PageHref {
			get {
				return this.pageHref;
			}
			set {
				this.pageHref = value;
			}
		}

		[XmlAttribute]
		public String CoverHref {
			get {
				return this.coverHref;
			}
			set {
				this.coverHref = value;
			}
		}

		[XmlIgnoreAttribute]
		public List<int> Actress {
			get {
				return this.actress;
			}
			set {
				this.actress = value;
			}
		}
	}
}
