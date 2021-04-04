using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CustomMapping
{
    public class Person : INotifyPropertyChanged
    {
        public Person(string name, DateTime birthdate)
        {
            _Name = name;
            _Birthdate = birthdate;
        }

        public override string ToString()
        { return (Name == null ? "<null>" : Name) + (Birthdate == null ? "(?)" : "(" + Birthdate.ToString() + ")"); }

        #region Name property

        string _Name;

        /// <summary>
        /// NamePropertyName
        /// </summary>
        public const string NamePropertyName = "Name";

        /// <summary>
        /// Name
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set
            {
                if (value != _Name)
                {
                    _Name = value;
                    OnPropertyChanged(NamePropertyName);
                }
            }
        }

        #endregion

        #region Birthdate property

        DateTime _Birthdate;

        /// <summary>
        /// BirthdatePropertyName
        /// </summary>
        public const string BirthdatePropertyName = "Birthdate";

        /// <summary>
        /// Birthdate
        /// </summary>
        public DateTime Birthdate
        {
            get { return _Birthdate; }
            set
            {
                if (value != _Birthdate)
                {
                    _Birthdate = value;
                    OnPropertyChanged(BirthdatePropertyName);
                }
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        #endregion
    }
}
