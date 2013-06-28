using System;
using System.IO;
using System.Collections.Generic;

namespace AVSyncer.Movies {
	public class MovieInfo {
		public static String Formatting = "[%Label%][%Mvid%][%Actress%]%Title%%ext%";
		public static String Struct = "%Maker%/%Mvid%";
		private Movie mv;
		private FileInfo fileInfo = null;

		public MovieInfo (Movie mv) {
			this.mv = mv;
		}

		public MovieInfo (Movie mv, FileInfo fileInfo) {
			this.mv = mv;
			this.fileInfo = fileInfo;
		}

		public String Mvid {
			get {
				return this.mv.Mvid;
			}
			set {
			}
		}

		public String Title {
			get {
				return this.mv.Title;
			}
			set {
			}
		}

		public String Rating {
			get {
				return this.mv.Rating;
			}
			set {
			}
		}

		public String Maker {
			get {
				return MovieManager.getInstance().Maker[mv.Maker].Name;
			}
			set {
			}
		}

		public String Label {
			get {
				return MovieManager.getInstance().Label[mv.Label].Name;
			}
			set {
			}
		}

		public String Series {
			get {
				return MovieManager.getInstance().Series[mv.Series].Name;
			}
			set {
			}
		}

		public String Cid {
			get {
				return this.mv.Cid;
			}
			set {
			}
		}

		public String Actress {
			get {
				List<String> act = new List<string>();
				foreach (int actressId in mv.Actress) {
					act.Add(MovieManager.getInstance().Actress[actressId].Name);
				}
				return string.Join(",", act);
			}
			set {
			}
		}

		public DateTime PublishDate {
			get {
				return this.mv.PublishDate;
			}
		}

		public Uri PageHref {
			get {
				Uri result;
				if (this.mv.PageHref != "") {
					result = new Uri(this.mv.PageHref);
				} else {
					result = null;
				}
				return result;
			}
			set {
			}
		}

		public Uri CoverHref {
			get {
				Uri result;
				if (this.mv.CoverHref != "") {
					result = new Uri(this.mv.CoverHref);
				} else {
					result = null;
				}
				return result;
			}
			set {
			}
		}

		public String FileName {
			get {
				String result = Formatting;
				result = result.Replace("%Maker%", Maker);
				result = result.Replace("%Label%", Label);
				result = result.Replace("%Mvid%", Mvid);
				result = result.Replace("%Series%", Series);
				result = result.Replace("%Actress%", Actress);
				result = result.Replace("%Title%", Title);
				if (fileInfo != null) {
					result = result.Replace("%ext%", fileInfo.Extension);
				} else {
					result = result.Replace("%ext%", ".avi");
				}
				return result;
			}
			set {
			}
		}

		public String StructPath {
			get {
				String result = Struct;
				result = result.Replace("%Maker%", Maker);
				result = result.Replace("%Label%", Label);
				result = result.Replace("%Mvid%", Mvid);
				result = result.Replace("%Series%", Series);
				result = result.Replace("%Actress%", Actress);
				result = result.Replace("%Title%", Title);
				result = Path.Combine(result.Split(new Char[] { '/', '\\' }));
				return result;
			}
			set {
			}
		}
	}
}
