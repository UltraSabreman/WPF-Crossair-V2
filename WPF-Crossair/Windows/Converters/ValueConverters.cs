using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WPF_Crosshair {
	public class SliderToVolumeConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			String s = ((int)(double)value).ToString();
			return s;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			// Do the conversion from visibility to bool
			throw new NotImplementedException();
		}
	}
	public class BoolToMuteIcon : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			bool val = (bool)value;
			return val ? "UM" : "M";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			// Do the conversion from visibility to bool
			throw new NotImplementedException();
		}
	}

	public class DurationConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			int seconds = (int)value % 60;
			int minutes = (int)((double)(int)value / 60.0);

			return String.Format("{0}:{1}", minutes, seconds.ToString("D2"));
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			// Do the conversion from visibility to bool
			throw new NotImplementedException();
		}
	}

	public class LastPlayedConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			double ticks = (double)value; //convert form seconds to nanoseconds

			DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
			dtDateTime = dtDateTime.AddSeconds(ticks).ToLocalTime();
			return dtDateTime.ToString("h:mm");
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			// Do the conversion from visibility to bool
			throw new NotImplementedException();
		}
	}

	public class TimeInQueueConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			double ticks;
			if (value.GetType() == typeof(double))
				ticks = (double)value; 
			else 
				ticks = (int)value;


			System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
			dtDateTime = dtDateTime.AddSeconds(ticks).ToLocalTime();
			TimeSpan timeInQueue = DateTime.Now - dtDateTime;
			return String.Format("{0}:{1}", timeInQueue.Minutes.ToString(), timeInQueue.Seconds.ToString("D2"));
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			// Do the conversion from visibility to bool
			throw new NotImplementedException();
		}
	}
}
