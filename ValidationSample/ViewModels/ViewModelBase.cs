using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ValidationSample.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged, IDataErrorInfo
    {
        private Dictionary<string, Func<string>> _validationHandlers = new Dictionary<string, Func<string>>();

        public event PropertyChangedEventHandler PropertyChanged;

        public string Error
        {
            get
            {
                if(_validationHandlers.Any(p => p.Value() != null))
                {
                    return "Validation errors occurred";
                }
                return null;
            }
        }

        public string this[string columnName]
        {
            get
            {
                return _validationHandlers[columnName]();
            }
        }

        protected virtual void AddValidationHandler(string propertyName, Func<string> handler)
        {
            _validationHandlers.Add(propertyName, handler);
        }

        protected void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
