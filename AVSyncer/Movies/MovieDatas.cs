using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Linq;

namespace AVSyncer.Movies {
	static class Extensions {
		public static string SerializeToString (this XmlSerializer s, object o, string ns) {
			var n = new XmlSerializerNamespaces();
			n.Add("", ns);
			var builder = new System.Text.StringBuilder();
			var settings = new System.Xml.XmlWriterSettings {
				OmitXmlDeclaration = true,
				Indent = true
			};
			using (var writer = System.Xml.XmlWriter.Create(builder, settings)) {
				s.Serialize(writer, o, n);
			}
			return builder.ToString();
		}
	}

	[XmlRoot]
	public class MovieDatas {
		private String version;
		private List<Maker> makerList;
		private List<Label> labelList;
		private List<Series> seriesList;
		private List<Actress> actressList;
		private List<Movie> movieList;
		private List<ActressMovie> actressMovieList;

		public MovieDatas Deserialize () {
			String xmlFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "uncAV.xml");
			MovieDatas movieDatas = Deserialize(xmlFile);
			return movieDatas;
		}

		public MovieDatas Deserialize (String file) {
			MovieDatas movieDatas = null;
			try {
				//Assembly asm = Assembly.GetExecutingAssembly();
				//Stream xmlStream = asm.GetManifestResourceStream("AVSyncer.uncAV.xml");
				XmlSerializer s = new XmlSerializer(typeof(MovieDatas));
				TextReader Reader = new StreamReader(file);
				movieDatas = (MovieDatas)s.Deserialize(new System.Xml.XmlTextReader(Reader));
			} catch (Exception ex) {
				Console.Write(ex.StackTrace);
			}
			return movieDatas;
		}

		public void Serialize (String fileName) {
			try {
				XmlSerializer s = new XmlSerializer(typeof(MovieDatas));
				TextWriter Writer = new StreamWriter(fileName);
				Writer.Write(s.SerializeToString(this, "urn:AVSyncer.Movies"));
				Writer.Close();
			} catch (Exception ex) {
				Console.Write(ex.StackTrace);
			}
		}

		public MovieManager Transform () {
			MovieManager mm = new MovieManager();
			mm.Version = version;
			mm.Maker = initMaker();
			mm.Label = initLabel();
			mm.Series = initSeries();
			mm.Movie = initMovie();
			mm.Actress = initActress();
			mm.ActressMovie = initActressMovie(mm.Movie);
			return mm;
		}

		#region public prop
		[XmlAttribute]
		public String Version {
			get {
				return version;
			}
			set {
				this.version = value;
			}
		}

		[XmlArray]
		public List<Maker> MakerList {
			get {
				return makerList;
			}
			set {
				this.makerList = value;
			}
		}

		[XmlArray]
		public List<Label> LabelList {
			get {
				return labelList;
			}
			set {
				this.labelList = value;
			}
		}

		[XmlArray]
		public List<Series> SeriesList {
			get {
				return seriesList;
			}
			set {
				this.seriesList = value;
			}
		}

		[XmlArray]
		public List<Actress> ActressList {
			get {
				return actressList;
			}
			set {
				this.actressList = value;
			}
		}

		[XmlArray]
		public List<Movie> MovieList {
			get {
				return movieList;
			}
			set {
				this.movieList = value;
			}
		}

		[XmlArray]
		public List<ActressMovie> ActressMovieList {
			get {
				return actressMovieList;
			}
			set {
				this.actressMovieList = value;
			}
		}
		#endregion

		private Dictionary<int, Maker> initMaker () {
			Dictionary<int, Maker> MakerList = new Dictionary<int, Maker>();
			try {
				foreach (Maker maker in makerList) {
					MakerList.Add(maker.Id, maker);
				}
			} catch (Exception ex) {
				Console.WriteLine(ex.StackTrace);
			}
			return MakerList;
		}

		private Dictionary<int, Label> initLabel () {
			Dictionary<int, Label> LabelList = new Dictionary<int, Label>();
			try {
				foreach (Label label in labelList) {
					LabelList.Add(label.Id, label);
				}
			} catch (Exception ex) {
				Console.WriteLine(ex.StackTrace);
			}
			return LabelList;
		}

		private Dictionary<int, Series> initSeries () {
			Dictionary<int, Series> SeriesList = new Dictionary<int, Series>();
			try {
				foreach (Series series in seriesList) {
					SeriesList.Add(series.Id, series);
				}
			} catch (Exception ex) {
				Console.WriteLine(ex.StackTrace);
			}
			return SeriesList;
		}

		private Dictionary<int, Actress> initActress () {
			Dictionary<int, Actress> ActressList = new Dictionary<int, Actress>();
			try {
				foreach (Actress actress in actressList) {
					ActressList.Add(actress.Id, actress);
				}
			} catch (Exception ex) {
				Console.WriteLine(ex.StackTrace);
			}
			return ActressList;
		}

		private Dictionary<int, Movie> initMovie () {
			Dictionary<int, Movie> MovieList = new Dictionary<int, Movie>();
			try {
				foreach (Movie movie in movieList) {
					MovieList.Add(movie.Id, movie);
				}
			} catch (Exception ex) {
				Console.WriteLine(ex.StackTrace);
			}
			return MovieList;
		}

		private Dictionary<int, List<int>> initActressMovie (Dictionary<int, Movie> Movie) {
			Dictionary<int, List<int>> ActressMovie = new Dictionary<int, List<int>>();
			try {
				foreach (ActressMovie am in actressMovieList) {
					Movie[am.MovieId].Actress.Add(am.ActressId);
					if (!ActressMovie.ContainsKey(am.ActressId)) {
						ActressMovie[am.ActressId] = new List<int>();
					}
					ActressMovie[am.ActressId].Add(am.MovieId);
				}
			} catch (Exception ex) {
				Console.WriteLine(ex.StackTrace);
			}
			return ActressMovie;
		}
	}
}
