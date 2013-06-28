using System;
using System.Xml.Serialization;

namespace AVSyncer.Movies {
	[XmlRoot]
	public class Actress {
		private int id;
		private String name;

		public Actress () {
		}

		public Actress (int id, String name) {
			this.id = id;
			this.name = name;
		}

		public Actress (System.Xml.Linq.XElement element) {
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
	}
}
