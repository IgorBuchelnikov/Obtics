This implicitly observable object LINQ variation in this project allows very easy creation of fully observable LINQ statements. All that needs to be done is include the following using statement in the source code

{{
//using System.Linq;
using Obtics.Collections.ImplicitObservable;
}}

If you would have an (observable) collection of objects with on observable property like:

{{
    class Person : INotifyPropertyChanged
    {
        string _FirstName;

        public const string FirstNamePropertyName = "FirstName";

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

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}}

Then a method like:

{{
    class Test 
    {
        public IEnumerable<Person> OrderedByFirstName( IEnumerable<Person> source )
        {
            from p in source
            orderby p.FirstName
            select p;
        }
    }
}}

Will return an observable sequence (The result IEnumerable<Person> supports INotifyCollectionChanged and INotifyPropertyChanged) with it's items sorted by FirstName. The result sequence will be +reordered+ whenever the FirstName property of any of it's items changes value. The developer doesn't have to do any event registration or create an IValueProvider out of the FirstName property.