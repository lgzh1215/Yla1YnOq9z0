using System;
using System.Collections.Generic;
using System.Linq;

namespace AVSyncer.Movies {
	public class MovieManager {
		private static MovieManager instance = null;
		private String version;
		private Dictionary<int, Maker> maker = null;
		private Dictionary<int, Label> label = null;
		private Dictionary<int, Series> series = null;
		private Dictionary<int, Actress> actress = null;
		private Dictionary<int, Movie> movie = null;
		private Dictionary<int, List<int>> actressMovie = null;

		public static MovieManager getInstance () {
			if (instance == null) {
				MovieDatas md = new MovieDatas().Deserialize();
				instance = md.Transform();
				md = null;
			}
			return instance;
		}

		public String Version {
			get {
				return version;
			}
			set {
				this.version = value;
			}
		}

		public Dictionary<int, Maker> Maker {
			get {
				return maker;
			}
			set {
				this.maker = value;
			}
		}

		public Dictionary<int, Label> Label {
			get {
				return label;
			}
			set {
				this.label = value;
			}
		}

		public Dictionary<int, Series> Series {
			get {
				return series;
			}
			set {
				this.series = value;
			}
		}

		public Dictionary<int, Actress> Actress {
			get {
				return actress;
			}
			set {
				this.actress = value;
			}
		}

		public Dictionary<int, Movie> Movie {
			get {
				return movie;
			}
			set {
				this.movie = value;
			}
		}

		public Dictionary<int, List<int>> ActressMovie {
			get {
				return actressMovie;
			}
			set {
				this.actressMovie = value;
			}
		}

		public Movie getMovieByMvid (string mvid) {
			Movie mv = null;
			try {
				Dictionary<int, Movie>.ValueCollection vc = Movie.Values;
				var result = from ss in vc
										 where ss.Mvid.ToLower().Contains(mvid.ToLower())
										 select ss;
				foreach (var m in result) {
					return m;
				}
			} catch (Exception ex) {
				Console.WriteLine(ex);
			}
			return mv;
		}
	}
}
