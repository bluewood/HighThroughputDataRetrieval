using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace HighThroughputDateRetrieval
{
    public class ContentToPathConverter : IValueConverter
    {
        readonly static ContentToPathConverter value = new ContentToPathConverter();
        public static ContentToPathConverter Value
        {
            get { return value; }
        }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ContentPresenter cp = (ContentPresenter)value;
            double h = cp.ActualHeight > 10 ? 1.4 * cp.ActualHeight : 10;
            double w = cp.ActualWidth > 10 ? 1.25 * cp.ActualWidth : 10;
            PathSegmentCollection ps = new PathSegmentCollection(4)
            {
                new LineSegment(new Point(1, 0.7 * h), true),
                new BezierSegment(new Point(1, 0.9 * h), new Point(0.1 * h, h), new Point(0.3 * h, h), true),
                new LineSegment(new Point(w, h), true),
                new BezierSegment(new Point(w + 0.6 * h, h), new Point(w + h, 0), new Point(w + h * 1.3, 0), true)
            };
            return ps;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    public class ContentToMarginConverter : IValueConverter
    {
        readonly static ContentToMarginConverter value = new ContentToMarginConverter();
        public static ContentToMarginConverter Value
        {
            get { return value; }
        }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new Thickness(0, 0, -((ContentPresenter)value).ActualHeight, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}