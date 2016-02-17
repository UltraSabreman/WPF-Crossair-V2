using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Crosshair {
	public class ViewModelNotifier : INotifyPropertyChanged {
		private Dictionary<String, Object> propertyValues = new Dictionary<String, Object>();
		public event PropertyChangedEventHandler PropertyChanged;

		protected void SetProp<T>(T value, [CallerMemberName] string property = null) {
			propertyValues[property] = value;
			OnPropertyChanged(property);
		}
		protected T GetProp<T>([CallerMemberName] string property = null) {
            if (!String.IsNullOrEmpty(property) && propertyValues.ContainsKey(property)) {
                try {
                    return (T)propertyValues[property];
                } catch (Exception) { }
            }

            return default(T);
        }

        protected void OnPropertyChanged(string propertyName) {
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
