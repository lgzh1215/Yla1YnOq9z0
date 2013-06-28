using System;

namespace AVSyncer.Movies {
	public class Studio {
		private int id;
		private String name;
		private String description;

		public Studio () {
		}

		public Studio (int id, String name, String description) {
			if (description == "")
				description = name;
			this.id = id;
			this.name = name;
			this.description = description;
		}

		public Studio (System.Xml.Linq.XElement element) {
			this.id = Convert.ToInt32(element.Attribute("Id").Value);
			this.name = element.Attribute("Name").Value;
			this.description = element.Attribute("Description").Value;
		}

		public int Id {
			get {
				return this.id;
			}
			set {
				this.id = value;
			}
		}

		public String Name {
			get {
				return this.name;
			}
			set {
				this.name = value;
			}
		}

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
