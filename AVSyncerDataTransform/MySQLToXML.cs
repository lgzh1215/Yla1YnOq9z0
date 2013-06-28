using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using AVSyncer.Movies;
using MySql.Data.MySqlClient;
using System.Xml.Serialization;

namespace AVSyncerDataTransform {
	class MySQLToXML {
		private const string UNCAV_FILE = "uncAV.xml";
		static string fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, UNCAV_FILE);
		static string newFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + @"..\..\..\AVSyncer\", UNCAV_FILE);

		static void Main (string[] args) {
			DB2Xml();
			//test();
		}

		private static void test () {
			MovieDatas md = new MovieDatas().Deserialize(@"C:\inetpub\wwwroot\AVSyncer\AVSyncer\uncAV.xml");
			MovieManager mm = md.Transform();
			md = null;
			Console.Write("");
		}

		private static void DB2Xml () {
			Database db = Database.getInstance();
			MovieDatas md = new MovieDatas();
			//md.Version = DateTime.Now.Ticks.ToString();
			md.Version = DateTime.Now.Year + "." + DateTime.Now.Month + "." + DateTime.Now.Day + "." + (int)DateTime.Now.TimeOfDay.TotalMinutes;
			//DateTime.Now.TimeOfDay.TotalMinutes
			md.MakerList = initMakerList();
			md.LabelList = initLabelList();
			md.SeriesList = initSeriesList();
			md.ActressList = initActressList();
			md.MovieList = initMovieList();
			md.ActressMovieList = initAMList();
			md.Serialize(fileName);
			if (File.Exists(newFileName)) {
				File.Delete(newFileName);
			}
			File.Copy(fileName, newFileName, true);
		}

		private static List<Maker> initMakerList () {
			List<Maker> MakerList = new List<Maker>();
			try {
				String sql = "select * from maker";
				MySqlCommand cmd = new MySqlCommand(sql, Database.getInstance().Connection);
				MySqlDataReader row = cmd.ExecuteReader();
				if (!row.HasRows) {
					Console.WriteLine("No data.");
					row.Close();
				} else {
					while (row.Read()) {
						MakerList.Add(new Maker(row.GetInt32(0), row.GetString(1), row.GetString(2)));
					}
					row.Close();
				}
			} catch (MySql.Data.MySqlClient.MySqlException ex) {
				Console.WriteLine("Error " + ex.Number + " : " + ex.Message);
			}
			return MakerList;
		}

		private static List<Label> initLabelList () {
			List<Label> LabelList = new List<Label>();
			try {
				String sql = "select * from label";
				MySqlCommand cmd = new MySqlCommand(sql, Database.getInstance().Connection);
				MySqlDataReader row = cmd.ExecuteReader();
				if (!row.HasRows) {
					Console.WriteLine("No data.");
					row.Close();
				} else {
					while (row.Read()) {
						LabelList.Add(new Label(row.GetInt32(0), row.GetString(1)));
					}
					row.Close();
				}
			} catch (MySql.Data.MySqlClient.MySqlException ex) {
				Console.WriteLine("Error " + ex.Number + " : " + ex.Message);
			}
			return LabelList;
		}

		private static List<Series> initSeriesList () {
			List<Series> SeriesList = new List<Series>();
			try {
				String sql = "select * from series";
				MySqlCommand cmd = new MySqlCommand(sql, Database.getInstance().Connection);
				MySqlDataReader row = cmd.ExecuteReader();
				if (!row.HasRows) {
					Console.WriteLine("No data.");
					row.Close();
				} else {
					while (row.Read()) {
						SeriesList.Add(new Series(row.GetInt32(0), row.GetString(1)));
					}
					row.Close();
				}
			} catch (MySql.Data.MySqlClient.MySqlException ex) {
				Console.WriteLine("Error " + ex.Number + " : " + ex.Message);
			}
			return SeriesList;
		}

		private static List<Actress> initActressList () {
			List<Actress> ActressList = new List<Actress>();
			try {
				String sql = "select * from actress";
				MySqlCommand cmd = new MySqlCommand(sql, Database.getInstance().Connection);
				MySqlDataReader row = cmd.ExecuteReader();
				if (!row.HasRows) {
					Console.WriteLine("No data.");
					row.Close();
				} else {
					while (row.Read()) {
						ActressList.Add(new Actress(row.GetInt32(0), row.GetString(1)));
					}
					row.Close();
				}
			} catch (MySql.Data.MySqlClient.MySqlException ex) {
				Console.WriteLine("Error " + ex.Number + " : " + ex.Message);
			}
			return ActressList;
		}

		private static List<Movie> initMovieList () {
			List<Movie> MovieList = new List<Movie>();
			try {
				String sql = "select * from movie";
				MySqlCommand cmd = new MySqlCommand(sql, Database.getInstance().Connection);
				MySqlDataReader row = cmd.ExecuteReader();
				if (!row.HasRows) {
					Console.WriteLine("No data.");
					row.Close();
				} else {
					while (row.Read()) {
						Movie mv = null;
						mv = new Movie(
							row.GetInt32(0),
							row.GetString(1),
							row.GetString(2),
							row.GetInt32(3),
							row.GetInt32(4),
							row.GetInt32(5),
							row.IsDBNull(6) ? null : row.GetString(6),
							row.IsDBNull(7) ? null : row.GetString(7),
							row.IsDBNull(8) ? DateTime.MinValue : (row.GetMySqlDateTime(8).ToString() == "0000/0/0") ? DateTime.MinValue : row.GetDateTime(8),
							row.IsDBNull(9) ? DateTime.MinValue : (row.GetMySqlDateTime(9).ToString() == "0000/0/0") ? DateTime.MinValue : row.GetDateTime(9),
							row.IsDBNull(10) ? null : row.GetString(10),
							row.IsDBNull(11) ? null : row.GetString(11)
						);
						MovieList.Add(mv);
					}
					row.Close();
				}
			} catch (MySql.Data.MySqlClient.MySqlException ex) {
				Console.WriteLine("Error " + ex.Number + " : " + ex.Message);
			} catch (Exception e) {
				Console.WriteLine("Error: " + e.StackTrace);
			}
			return MovieList;
		}

		private static List<ActressMovie> initAMList () {
			List<ActressMovie> amList = new List<ActressMovie>();
			try {
				String sql = "select * from actress_movie";
				MySqlCommand cmd = new MySqlCommand(sql, Database.getInstance().Connection);
				MySqlDataReader row = cmd.ExecuteReader();
				if (!row.HasRows) {
					Console.WriteLine("No data.");
					row.Close();
				} else {
					while (row.Read()) {
						int actressId = row.GetInt32(0);
						int movieId = row.GetInt32(1);
						amList.Add(new ActressMovie(actressId, movieId));
					}
					row.Close();
				}
			} catch (MySql.Data.MySqlClient.MySqlException ex) {
				Console.WriteLine("Error " + ex.Number + " : " + ex.Message);
			}
			return amList;
		}

	}
}
