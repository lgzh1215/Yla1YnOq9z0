using System;
using MySql.Data.MySqlClient;

namespace AVSyncerDataTransform {
	public class Database {
		private static Database instance = null;
		private MySqlConnection connection;

		public Database () {
			this.connection = initConnection();
		}

		public static Database getInstance () {
			if (instance == null) {
				instance = new Database();
			}
			return instance;
		}

		public MySqlConnection Connection {
			get {
				return this.connection;
			}
		}

		private MySqlConnection initConnection () {
			string dbHost = "127.0.0.1";
			string dbUser = "root";
			string dbPass = "root";
			string dbName = "ghchen";

			// 如果有特殊的編碼在database後面請加上;CharSet=編碼, utf8請使用utf8_general_ci 
			string connStr = "server=" + dbHost + ";uid=" + dbUser + ";pwd=" + dbPass + ";database=" + dbName + ";CharSet=utf8;Allow Zero Datetime=true";
			MySqlConnection conn = new MySqlConnection(connStr);
			// 連線到資料庫 
			try {
				conn.Open();
				return conn;
			} catch (MySql.Data.MySqlClient.MySqlException ex) {
				switch (ex.Number) {
					case 0:
						Console.WriteLine("無法連線到資料庫.");
						break;
					case 1042:
						Console.WriteLine("Unable to connect to any of the specified MySQL hosts.");
						break;
					case 1045:
						Console.WriteLine("使用者帳號或密碼錯誤,請再試一次.");
						break;
				}
				return null;
			}
		}
	}
}
