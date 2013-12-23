using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Media.Imaging;

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
            if (value == null) {
                return null;
            }
            if (value is string) {
                value = new Uri((string) value);
            }
            if (value is Uri) {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.DecodePixelWidth = 150;
                bi.UriSource = (Uri) value;
                bi.EndInit();
                CroppedBitmap cb = new CroppedBitmap(bi, new Int32Rect(0, 50, 150, 60));
                return cb;
            }
            return null;
        }

        public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
    #endregion
}
