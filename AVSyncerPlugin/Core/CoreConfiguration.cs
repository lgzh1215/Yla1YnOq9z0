using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;

namespace AVSyncerPlugin.Core {
	public class CoreConfiguration {
		private static string CONFIG_FILE = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AVSyncer", "config.xml");
		private static CoreConfiguration instance = null;
		private XDocument xml = null;
		private ObservableCollection<String> selectpaths = new ObservableCollection<String>();
		private ObservableCollection<String> formattings = new ObservableCollection<String>();
		private ObservableCollection<String> structs = new ObservableCollection<String>();

		public CoreConfiguration () {
		}

		public static CoreConfiguration getInstance () {
			if (instance == null) {
				instance = new CoreConfiguration();
				try {
					instance.readConfiguration();
				} catch (Exception ex) {
					Console.WriteLine(ex);
					instance.readDefaultConfiguration();
				}
			}
			return instance;
		}

		public ObservableCollection<String> SelectPaths {
			get {
				return this.selectpaths;
			}
			set {
				this.selectpaths = value;
			}
		}
		
		public ObservableCollection<String> Formattings {
			get {
				return this.formattings;
			}
			set {
				this.formattings = value;
			}
		}
		
		public ObservableCollection<String> Structs {
			get {
				return this.structs;
			}
			set {
				this.structs = value;
			}
		}

		public int SelectedPath {
			get;
			set;
		}

		public int SelectedFormat {
			get;
			set;
		}

		public int SelectedStruct {
			get;
			set;
		}

		public void readConfiguration () {
			xml = XDocument.Load(CONFIG_FILE);
			var q0 = from e in xml.Descendants("Format")
						select e;
			foreach (XElement s in q0)
				formattings.Add(s.Attribute("text").Value);
			var q1 = (from e in xml.Descendants("SelectedFormat")
						select e).Distinct();
			foreach (XElement s in q1)
				SelectedFormat = Convert.ToInt32(s.Value);

			var q2 = from e in xml.Descendants("Struct")
						select e;
			foreach (XElement s in q2)
				structs.Add(s.Attribute("text").Value);
			var q3 = (from e in xml.Descendants("SelectedStruct")
						select e).Distinct();
			foreach (XElement s in q3)
				SelectedStruct = Convert.ToInt32(s.Value);

			var q4 = xml.Descendants("Path");
			foreach (var s in q4)
				selectpaths.Add(s.Value);
			var q5 = xml.Descendants("SelectedPath").Distinct();
			foreach (var s in q5)
				SelectedPath = Convert.ToInt32(s.Value);
		}

		public void writeConfiguration () {
			try {
				TextWriter Writer = new StreamWriter(CONFIG_FILE);
				XElement root = new XElement("AVSyncer");

				XElement xSelectPaths = new XElement("SelectPaths");
				foreach (string s in selectpaths) {
					XElement xPath = new XElement("Path");
					xPath.SetValue(s);
					xSelectPaths.Add(xPath);
				}
				root.Add(xSelectPaths);
				XElement xSelectedPath = new XElement("SelectedPath");
				xSelectedPath.Value = SelectedPath.ToString();
				root.Add(xSelectedPath);

				XElement xFormattings = new XElement("Formattings");
				foreach (string s in formattings) {
					string vs = "";
					foreach (char ic in System.IO.Path.GetInvalidFileNameChars())
						vs = s.Replace(ic.ToString(), "");
					XElement xFormat = new XElement("Format");
					xFormat.SetAttributeValue("text", vs);
					xFormattings.Add(xFormat);
				}
				root.Add(xFormattings);
				XElement xSelectedFormat = new XElement("SelectedFormat");
				xSelectedFormat.Value = SelectedFormat.ToString();
				root.Add(xSelectedFormat);

				XElement xStructs = new XElement("Structs");
				foreach (string s in structs) {
					string vs = "";
					foreach (char ic in System.IO.Path.GetInvalidPathChars())
						vs = s.Replace(ic.ToString(), "");
					vs = vs.Replace("..", "");
					XElement xStruct = new XElement("Struct");
					xStruct.SetAttributeValue("text", vs);
					xStructs.Add(xStruct);
				}
				root.Add(xStructs);
				XElement xSelectedStruct = new XElement("SelectedStruct");
				xSelectedStruct.Value = SelectedStruct.ToString();
				root.Add(xSelectedStruct);

				Writer.Write(root);
				Writer.Close();
			} catch (Exception ex) {
				Console.WriteLine(ex);
			}
		}

		public void readDefaultConfiguration () {
			try {
				selectpaths = new ObservableCollection<string> { "" };
				formattings = new ObservableCollection<string> {
					"",
					"[%Maker%][%Mvid%][%Actress%]%Title%%ext%", 
					"[%Maker%][%Mvid%][%Actress%]%Title%.avi",
					"[%Maker%][%Series%][%Mvid%][%Actress%]%Title%%ext%",
					"[%Maker%][%Series%][%Mvid%][%Actress%]%Title%.avi"
				};
				structs = new ObservableCollection<string> {
					"",
					"%Maker%/%Mvid%"
				};
				SelectedPath = 0;
				SelectedFormat = 1;
				SelectedStruct = 1;
			} catch (Exception ex) {
				Console.WriteLine(ex);
			}
		}
	}
}
