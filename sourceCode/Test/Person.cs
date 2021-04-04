using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Test
{
    /// <summary>
    /// This is a test collection item type.
    /// It is an observable object (it has properties that can change
    /// and a NotifyPropertyChanged implementation)
    /// </summary>
    public class Person : INotifyPropertyChanged
    {
        public Person(string firstName, string lastName)
        {
            _FirstName = firstName;
            _LastName = lastName;
        }

        public override string ToString()
        { return (FirstName == null ? "<null>" : FirstName) + (LastName == null ? "<null>" : LastName); }

        #region FirstName property

        string _FirstName;

        /// <summary>
        /// FirstNamePropertyName
        /// </summary>
        public const string FirstNamePropertyName = "FirstName";

        /// <summary>
        /// FirstName
        /// </summary>
        public string FirstName
        {
            get { return _FirstName; }
            set
            {
                if (value != _FirstName)
                {
                    _FirstName = value;
                    OnPropertyChanged(FirstNamePropertyName);
                }
            }
        }

        #endregion

        #region LastName property

        string _LastName;

        /// <summary>
        /// LastNamePropertyName
        /// </summary>
        public const string LastNamePropertyName = "LastName";

        /// <summary>
        /// LastName
        /// </summary>
        public string LastName
        {
            get { return _LastName; }
            set
            {
                if (value != _LastName)
                {
                    _LastName = value;
                    OnPropertyChanged(LastNamePropertyName);
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
