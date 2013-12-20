using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

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
}
