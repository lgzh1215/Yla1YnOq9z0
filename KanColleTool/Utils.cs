using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace KanColleTool {

    static public class Utils {

        static public string valueOfUTC (string value) {
            string result = "";
            if (value != "" && value != "0") {
                DateTime date = parseUTC(value);
                result = date.ToString("HH:mm:ss");
            }
            return result;
        }

        static public DateTime parseUTC (string value) {
            long utc = long.Parse(value);
            DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime date = start.AddMilliseconds(utc).ToLocalTime();
            return date;
        }

        static public TimeSpan countSpan (string value) {
            DateTime eta = parseUTC(value);
            TimeSpan span = eta - DateTime.Now;
            return span;
        }

        static public T FindChild<T> (DependencyObject parent, string childName) where T : DependencyObject {
            if (parent == null) {
                return null;
            }
            T foundChild = null;
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < childrenCount; i++) {
                var child = VisualTreeHelper.GetChild(parent, i);
                T childType = child as T;

                if (childType == null) {
                    foundChild = FindChild<T>(child, childName);

                    if (foundChild != null)
                        break;
                } else
                    if (!string.IsNullOrEmpty(childName)) {
                        var frameworkElement = child as FrameworkElement;

                        if (frameworkElement != null && frameworkElement.Name == childName) {
                            foundChild = (T) child;
                            break;
                        } else {
                            foundChild = FindChild<T>(child, childName);

                            if (foundChild != null) {
                                break;
                            }
                        }
                    } else {
                        foundChild = (T) child;
                        break;
                    }
            }
            return foundChild;
        }
    }

    #region converter
    public class UriToImageConverter : IValueConverter {
        public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            try {
                if (value == null) {
                    return null;
                }
                if (value is string) {
                    value = new Uri((string) value);
                }
                if (value is Uri) {
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    bi.DecodePixelWidth = 170;
                    bi.UriSource = (Uri) value;
                    Int32Rect rect = new Int32Rect(0, 50, 170, 60);
                    if (parameter is Int32Rect) {
                        rect = (Int32Rect) parameter;
                        bi.DecodePixelWidth = Math.Max(bi.DecodePixelWidth, rect.Width);
                    }
                    bi.EndInit();
                    CroppedBitmap cb = new CroppedBitmap(bi, rect);
                    return cb;
                }
                return null;
            } catch (Exception ex) {
                Debug.Print(ex.ToString());
                return "";
            }

        }

        public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    public class JArrayIndexConverter : IMultiValueConverter {
        public object Convert (object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (values == null) {
                return null;
            }
            JToken node = values[0] as JToken;
            if (node == null) {
                return null;
            }
            return String.Join(", ", node.ToList());
        }

        public object[] ConvertBack (object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    public class JArrayConverter : IValueConverter {
        public object Convert (object values, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (values == null) {
                return null;
            }
            JToken node = values as JToken;
            if (node == null) {
                return null;
            }
            return String.Join(", ", node.ToList());
        }

        public object ConvertBack (object value, Type targetTypes, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
    #endregion

    public class DataChangedEventArgs : EventArgs {
        private readonly JToken data;
        public DataChangedEventArgs (JToken data) {
            this.data = data;
        }
        public JToken Data {
            get { return this.data; }
        }
    }

    public class NavigateEventArgs : EventArgs {
        private readonly JToken data;
        private readonly string type;
        public NavigateEventArgs (string type, JToken data) {
            this.type = type;
            this.data = data;
        }
        public JToken Data {
            get { return this.data; }
        }
        public string Type {
            get { return this.type; }
        }
    }

    public class BattleEventArgs : EventArgs {
        private readonly JToken data;
        private readonly string type;
        public BattleEventArgs (string type, JToken data) {
            this.type = type;
            this.data = data;
        }
        public JToken Data {
            get { return this.data; }
        }
        public string Type {
            get { return this.type; }
        }
    }

}
