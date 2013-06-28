using System.Xml.Serialization;

namespace AVSyncer.Movies {
	[XmlRoot]
	public class ActressMovie {
		private int actressId;
		private int movieId;

		public ActressMovie () {
		}

		public ActressMovie (int actressId, int movieId) {
			this.actressId = actressId;
			this.movieId = movieId;
		}

		[XmlAttribute]
		public int ActressId {
			get {
				return this.actressId;
			}
			set {
				this.actressId = value;
			}
		}

		[XmlAttribute]
		public int MovieId {
			get {
				return this.movieId;
			}
			set {
				this.movieId = value;
			}
		}
	}
}
