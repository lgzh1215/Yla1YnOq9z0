using System;
using System.Xml.Serialization;

namespace AVSyncer.Movies {
	[XmlRoot]
	public class Series {
		private int id;
		private String name;

		public Series () {
		}

		public Series (int id, String name) {
			this.id = id;
			this.name = name;
		}

		public Series (System.Xml.Linq.XElement element) {
			this.id = Convert.ToInt32(element.Attribute("Id").Value);
			this.name = element.Attribute("Name").Value;
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
		public String Name {
			get {
				return this.name;
			}
			set {
				this.name = value;
			}
		}

		public override string ToString () {
			return this.name;
		}
	}
}
