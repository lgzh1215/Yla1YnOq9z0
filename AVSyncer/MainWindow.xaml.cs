using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using AVSyncer.Movies;
using AVSyncerPlugin.Core;

namespace AVSyncer {
	public partial class MainWindow : Window {
		private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(MainWindow));

		private String[] filePaths;
		private Queue<vo> avs = new Queue<vo>();
		private MovieManager mm = MovieManager.getInstance();
		private List<MovieInfo> movieList = new List<MovieInfo>();
		private vo currItem = null;
		private int total = 0;
		private CoreConfiguration conf = CoreConfiguration.getInstance();
		private String currentDirectory;
		private String targetPath;
		private Boolean transection = false;

		#region vo
		public class vo {
			public vo (String mvid, String fileName) {
				this.mvid = mvid;
				this.fileName = fileName;
			}
			public String fileName {
				get;
				set;
			}
			public String mvid {
				get;
				set;
			}
		}
		#endregion

		public MainWindow () {
			InitializeComponent();
			InitializeConfig();
		}

		private void InitializeConfig () {
			Title = String.Format("AVSyncer({0}), AVDB({1}) - twbbs.ghchen.com",
				Assembly.GetExecutingAssembly().GetName().Version.ToString(),
				mm.Version);
			cbxSelectedPath.SetBinding(ItemsControl.ItemsSourceProperty, new Binding {
				Source = conf.SelectPaths
			});
			cbxSelectedPath.SelectedIndex = conf.SelectedPath;
			cbxTF.SetBinding(ItemsControl.ItemsSourceProperty, new Binding {
				Source = conf.Formattings
			});
			cbxTF.SelectedIndex = conf.SelectedFormat;
			cbxStruct.SetBinding(ItemsControl.ItemsSourceProperty, new Binding {
				Source = conf.Structs
			});
			cbxStruct.SelectedIndex = conf.SelectedStruct;
		}

		private void btnChange_Click (object sender, RoutedEventArgs e) {
			String originalPath = cbxSelectedPath.SelectedItem.ToString();
			if (textBoxChangeTo.Text.Length > 255) {
				textBoxChangeTo.Text = textBoxChangeTo.Text.Substring(0, 250);
			}
			String oldName = Path.Combine(originalPath, textBoxOrignal.Text);
			String newName = Path.Combine(originalPath, targetPath, textBoxChangeTo.Text);
			Directory.CreateDirectory(Path.Combine(originalPath, targetPath));
			try {
				File.Move(oldName, newName);
				log.Info(String.Format("move: {0} -> {1}", oldName, newName));
			} catch (IOException ex) {
				log.Error(ex);
			}
			FileInfo info = new FileInfo(newName);
			DateTime newDate = (DateTime)labChangeToDate.Content;
			info.CreationTime = newDate;
			info.LastWriteTime = newDate;
			if (avs.Count == 0) {
				btnChange.IsEnabled = false;
				btnCancel.IsEnabled = false;
			} else {
				displayItem();
			}
		}

		private void btnCancel_Click (object sender, RoutedEventArgs e) {
			if (avs.Count == 0) {
				btnChange.IsEnabled = false;
				btnCancel.IsEnabled = false;
			} else {
				displayItem();
			}
		}

		public void searchFolder () {
			avs.Clear();
			movieList.Clear();
			foreach (String fileName in filePaths) {
				Boolean isRecognize = false;
				String[] ave = {
					@"n[0-9][0-9][0-9][0-9]",
					@"[0-9][0-9][0-9][0-9][0-9][0-9]_[0-9][0-9][0-9]",
					@"[0-9][0-9][0-9][0-9][0-9][0-9]-[0-9][0-9][0-9]",
					@"KG-[0-9][0-9]",
					@"KP-[0-9][0-9][0-9]",
					@"RED[0-9][0-9][0-9]",
					@"RHJ-[0-9][0-9][0-9]",
					@"[B|C|P|S]T-[0-9][0-9]",
					@"DT-[0-9][0-9][0-9]",
					@"CWP-[0-9][0-9]",
					@"SMD-[0-9][0-9]",
					@"DRC-[0-9][0-9][0-9]",
					@"MKD-.?[0-9][0-9]",
					@"gachi[0-9][0-9][0-9]",
					@"gachip[0-9][0-9][0-9]",
					@"gachig[0-9][0-9][0-9]",
					@"[0-9][0-9][0-9][0-9][0-9][0-9]_[0-9][0-9]",
					@"ori[0-9][0-9][0-9]",
					@"gol[0-9][0-9][0-9]",
					@"pla[0-9][0-9][0-9][0-9]"
				};
				foreach (String p in ave) {
					isRecognize = parserMvid(p, fileName);
					if (isRecognize) {
						break;
					}
				}

				if (!isRecognize) {
					foreach (String s in matches(@"Gachinco", fileName, 0)) {
						foreach (String v in matches(@"Vol.[0-9][0-9][0-9]", fileName, 0)) {
							avs.Enqueue(new vo("gachi" + v.Substring(4), fileName));
							isRecognize = true;
							break;
						}
						foreach (String v in matches(@"Pre.[0-9][0-9][0-9]", fileName, 0)) {
							avs.Enqueue(new vo("gachip" + v.Substring(4), fileName));
							isRecognize = true;
							break;
						}
						foreach (String v in matches(@"gac.[0-9][0-9][0-9]", fileName, 0)) {
							avs.Enqueue(new vo("gachig" + v.Substring(4), fileName));
							isRecognize = true;
							break;
						}
					}
				}
				if (!isRecognize) {
					foreach (String s in matches(@"Tokyo.Hot", fileName, 0)) {
						foreach (String v in matches(@"Vol.[0-9][0-9][0-9]", fileName, 0)) {
							avs.Enqueue(new vo("n0" + v.Substring(4), fileName));
							isRecognize = true;
							break;
						}
					}
				}

				if (!isRecognize) {
					FileInfo info = new FileInfo(fileName);
					if (info.Name.Substring(0, 1) != "[") {
						int dot = info.Name.IndexOf('.', 0);
						int ecil = info.Name.IndexOf(']', 0);
						int offset = dot < ecil ? dot : ecil;
						if (dot == -1 && ecil != -1) {
							offset = ecil;
						} else if (dot != -1 && ecil == -1) {
							offset = dot;
						}
						String v = info.Name.Substring(0, offset);
						avs.Enqueue(new vo(v, fileName));
						isRecognize = true;
					}
				}
				if (!isRecognize) {
					avs.Enqueue(new vo(fileName, fileName));
				}
			}
			total = avs.Count;
			displayItem();
		}

		public Boolean parserMvid (String pattern, String fileName) {
			foreach (String s in matches(pattern, fileName, 0)) {
				avs.Enqueue(new vo(s, fileName));
				return true;
			}
			return false;
		}

		public void displayItem () {
			int isdiff = 0;
			if (avs.Count > 0) {
				Movie mv = null;
				currItem = avs.Dequeue();
				isdiff = perpForSync(currItem.mvid, currItem.fileName, ref mv);
				labCounting.Content = (total - avs.Count) + "/" + total;
			} else {
				btnChange.IsEnabled = false;
				btnCancel.IsEnabled = false;
				textBoxOrignal.Text = "";
				textBoxChangeTo.Text = "";
				labOrignalDate.Content = "";
				labChangeToDate.Content = "";
				cbxSelectedPath.IsReadOnly = false;
			}
		}

		public int perpForSync (String mvid, String oldfile, ref Movie mv) {
			mv = mm.getMovieByMvid(mvid);
			if (File.Exists(oldfile)) {
				FileInfo oldInfo = new FileInfo(oldfile);
				textBoxOrignal.Text = oldInfo.Name;
				labOrignalDate.Content = oldInfo.LastWriteTime;
				if (mv == null) {
					textBoxChangeTo.Text = "";
					labChangeToDate.Content = "";
					targetPath = "";
					return -1;
				} else {
					dataGrid1.ItemsSource = null;
					MovieInfo mvi = new MovieInfo(mv, oldInfo);
					movieList.Add(mvi);
					dataGrid1.ItemsSource = movieList;
					textBoxChangeTo.Text = mvi.FileName.Replace("%ext%", oldInfo.Extension);
					labChangeToDate.Content = mv.PublishDate;
					targetPath = mvi.StructPath;
					return String.Compare(oldInfo.Name, mvi.FileName);
				}
			} else {
				return 0;
			}
		}

		public static IEnumerable matches (String pPattern, String pText, int pGroupId) {
			Regex r = new Regex(pPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
			for (Match m = r.Match(pText); m.Success; m = m.NextMatch())
				yield return m.Groups[pGroupId].Value;
		}

		private void textBoxSearch_TextChanged (object sender, TextChangedEventArgs e) {
			Dictionary<int, Movie>.ValueCollection vm = mm.Movie.Values;
			var qm = from ss in vm
					 where ss.Mvid.ToLowerInvariant().Contains(textBoxSearch.Text.ToLowerInvariant()) ||
					 ss.Title.ToLowerInvariant().Contains(textBoxSearch.Text.ToLowerInvariant())
					 select new MovieInfo(ss);
			Dictionary<int, Actress>.ValueCollection va = mm.Actress.Values;
			var qa = from ss in va
					 where ss.Name.Contains(textBoxSearch.Text)
					 select ss;
			dataGrid1.ItemsSource = null;
			List<MovieInfo> xx = new List<MovieInfo>();
			xx.AddRange(qm);
			foreach (Actress actress in qa) {
				if (mm.ActressMovie.ContainsKey(actress.Id)) {
					foreach (int movieId in mm.ActressMovie[actress.Id]) {
						xx.Add(new MovieInfo(mm.Movie[movieId]));
					}
				}
			}
			dataGrid1.ItemsSource = xx;
		}

		private void dataGrid1_SelectionChanged (object sender, SelectionChangedEventArgs e) {
			System.Windows.Controls.DataGrid grid = sender as System.Windows.Controls.DataGrid;
			if (grid.SelectedItem == null) {
				btnCopy.IsEnabled = false;
			} else {
				btnCopy.IsEnabled = true;
			}
		}

		private void btnCopy_Click (object sender, RoutedEventArgs e) {
			MovieInfo item = (MovieInfo)dataGrid1.SelectedItem;
			if (item != null) {
				Movie mv = null;
				transection = true;
				perpForSync(item.Mvid, currItem.fileName, ref mv);
				transection = false;
			}
		}

		private void textBoxChangeTo_TextChanged (object sender, TextChangedEventArgs e) {
			if (!transection) {
				buildFormatedMovieInfo();
			}
		}

		private void btnSelect_Click (object sender, RoutedEventArgs e) {
			System.Windows.Forms.FolderBrowserDialog fbDialog = new System.Windows.Forms.FolderBrowserDialog();
			fbDialog.SelectedPath = currentDirectory;
			if (fbDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK && fbDialog.SelectedPath != "") {
				currentDirectory = fbDialog.SelectedPath;
				checkComboxAndVar(cbxSelectedPath, conf.SelectPaths, currentDirectory);
				cbxSelectedPath.IsReadOnly = true;
			}
		}

		private void cbxSelectedPath_SelectionChanged (object sender, SelectionChangedEventArgs e) {
			var comboBox = (ComboBox)sender;
			try {
				currentDirectory = comboBox.SelectedValue.ToString();
				if (Directory.Exists(currentDirectory)) {
					var entries = Directory.EnumerateFileSystemEntries(currentDirectory);
					var fp = new List<String>();
					foreach (var entry in entries) {
						if (File.Exists(entry)) {
							fp.Add(entry);
						}
					}
					filePaths = fp.ToArray();
					btnChange.IsEnabled = true;
					btnCancel.IsEnabled = true;
					cbxSelectedPath.IsReadOnly = true;
					conf.SelectedPath = comboBox.SelectedIndex;
					conf.writeConfiguration();
					searchFolder();
				}
			} catch (Exception ex) {
				Console.WriteLine(ex);
			}
		}
		
		private void dataGrid1_Click (object sender, RoutedEventArgs e) {
			Type t = e.OriginalSource.GetType();
			if (t.Name == "Hyperlink") {
				var destination = ((Hyperlink)e.OriginalSource).NavigateUri;
				Process.Start(destination.ToString());
			}
		}

		private void checkComboxAndVar (ComboBox comboBox, ObservableCollection<String> source, String path) {
			var newItem = path;
			if (!source.Contains(newItem)) {
				if (source.Count >= 10) {
					source.Remove(source[0]);
				}
				source.Add(newItem);
			}
			comboBox.SelectedItem = newItem;
		}

		private void checkComboxAndVar (ComboBox comboBox, ObservableCollection<String> source, char[] invalidChars) {
			comboBox.Text = comboBox.Text.Replace("..", "");
			foreach (char ic in invalidChars)
				comboBox.Text = comboBox.Text.Replace(ic.ToString(), "");
			comboBox.Text = comboBox.Text.Trim();
			var newItem = comboBox.Text;
			if (!source.Contains(newItem)) {
				if (source.Count >= 10) {
					source.Remove(source[0]);
				}
				source.Add(newItem);
			}
			comboBox.SelectedItem = newItem;
		}

		private void cbxTF_LostFocus (object sender, RoutedEventArgs e) {
			var comboBox = (ComboBox)sender;
			if (comboBox.SelectedItem != null)
				return;
			checkComboxAndVar(comboBox, conf.Formattings, System.IO.Path.GetInvalidFileNameChars());
		}

		private void cbxTF_PreviewTextInput (object sender, System.Windows.Input.TextCompositionEventArgs e) {
			var comboBox = (ComboBox)sender;
			if (e.Text == "\r") {
				checkComboxAndVar(comboBox, conf.Formattings, System.IO.Path.GetInvalidFileNameChars());
			}
		}
		
		private void cbxTF_SelectionChanged (object sender, SelectionChangedEventArgs e) {
			var comboBox = (ComboBox)sender;
			if (comboBox.SelectedValue != null) {
				if (!transection) {
					buildFormatedMovieInfo();
				}
				conf.SelectedFormat = comboBox.SelectedIndex;
				conf.writeConfiguration();
			}
		}
		
		private void cbxStruct_LostFocus (object sender, RoutedEventArgs e) {
			var comboBox = (ComboBox)sender;
			if (comboBox.SelectedItem != null)
				return;
			checkComboxAndVar(comboBox, conf.Structs, System.IO.Path.GetInvalidPathChars());
		}

		private void cbxStruct_PreviewTextInput (object sender, System.Windows.Input.TextCompositionEventArgs e) {
			var comboBox = (ComboBox)sender;
			if (e.Text == "\r") {
				checkComboxAndVar(comboBox, conf.Structs, System.IO.Path.GetInvalidPathChars());
			}
		}

		private void cbxStruct_SelectionChanged (object sender, SelectionChangedEventArgs e) {
			var comboBox = (ComboBox)sender;
			if (comboBox.SelectedValue != null) {
				if (!transection) {
					buildFormatedMovieInfo();
				}
				conf.SelectedStruct = comboBox.SelectedIndex;
				conf.writeConfiguration();
			}
		}

		private void Window_Closed (object sender, EventArgs e) {
			System.Windows.Application.Current.Shutdown();
		}

		private String buildFormatedMovieInfo () {
			String result = "";
			transection = true;
			if (textBoxChangeTo.Text.Length > 255) {
				textBoxChangeTo.Text = textBoxChangeTo.Text.Substring(0, 250);
			}

			if (cbxTF.SelectedValue != null) {
				MovieInfo.Formatting = cbxTF.SelectedValue.ToString();
			}
			if (cbxStruct.SelectedValue != null) {
				MovieInfo.Struct = cbxStruct.SelectedValue.ToString();
			}

			if (currItem != null) {
				Movie mv = mm.getMovieByMvid(currItem.mvid);
				FileInfo oldInfo = new FileInfo(currItem.fileName);
				if (mv != null) {
					MovieInfo mvi = new MovieInfo(mv, oldInfo);
					textBoxChangeTo.Text = mvi.FileName.Replace("%ext%", oldInfo.Extension);
					labChangeToDate.Content = mv.PublishDate;
					targetPath = mvi.StructPath;

					String originalPath = cbxSelectedPath.SelectedItem.ToString();
					result = Path.Combine(originalPath, targetPath, textBoxChangeTo.Text);
					int shift = 2;
					while (File.Exists(result)) {
						mvi = new MovieInfo(mv, oldInfo);
						textBoxChangeTo.Text = mvi.FileName.Insert(mvi.FileName.LastIndexOf('.'), " (" + (shift++) + ")");
						result = Path.Combine(originalPath, targetPath, textBoxChangeTo.Text);
					}
					if (avs.Count > 0) {
						if (textBoxChangeTo.Text.Length > 0) {
							btnChange.IsEnabled = true;
						} else {
							btnChange.IsEnabled = false;
						}
					}
				}
			}
			transection = false;
			return result;
		}
	}
}
