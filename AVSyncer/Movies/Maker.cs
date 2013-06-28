using System;
using System.Xml.Serialization;

namespace AVSyncer.Movies {
	[XmlRoot]
	public class Maker {
		private int id;
		private String name;
		private String description;

		public Maker () {
		}

		public Maker (int id, String name, String description) {
			if (description == "")
				description = name;
			this.id = id;
			this.name = name;
			this.description = description;
		}

		public Maker (System.Xml.Linq.XElement element) {
			this.id = Convert.ToInt32(element.Attribute("Id").Value);
			this.name = element.Attribute("Name").Value;
			this.description = element.Attribute("Description").Value;
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

		[XmlAttribute]
		public String Description {
			get {
				return this.description;
			}
			set {
				this.description = value;
			}
		}

		public override string ToString () {
			return this.name;
		}
	}
}
