using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Obtics.Values;
using Obtics;

namespace CustomMapping
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();

            _Persons.Add(new Person("John", new DateTime(1970, 4, 20)));
            _Persons.Add(new Person("Mary", new DateTime(1968, 3, 12)));
            _Persons.Add(new Person("Jack", new DateTime(1974, 7, 2)));
            _Persons.Add(new Person("Carrol", new DateTime(1972, 10, 16)));
            _Persons.Add(new Person("Steve", new DateTime(1973, 4, 24)));
            _Persons.Add(new Person("Angela", new DateTime(1980, 2, 14)));
        }

        ObservableCollection<Person> _Persons = new ObservableCollection<Person>();

        public ObservableCollection<Person> Persons { get { return _Persons; } }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            _Persons.Add(new Person("?", DateTime.Now));
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            var selectedPerson = PersonsBox.SelectedItem as Person; 

            if (selectedPerson != null)
                _Persons.Remove(selectedPerson);
        }

        
        public IValueProvider<Person> YoungestPerson
        { 
            get 
            {
                return
                    _YoungestPersonF(this)
                        .OnException((Exception ex) => null)
                ;
            } 
        }

        //Thanks to the registration in MyEnumerable ExpressionObserver
        //can create a meaningful live expression here.
        static Func<Window1, IValueProvider<Person>> _YoungestPersonF =
            ExpressionObserver.Compile(
                (Window1 t) =>
                    t.Persons.ElementWithMax( p => p.Birthdate )
            );


        
        public IEnumerable<Person> NamesStartingWithPersons
        { get { return ValueProvider.Cascade(_NamesStartingWithPersonsF(this)); } }

        static Func<Window1, IValueProvider<IEnumerable<Person>>> _NamesStartingWithPersonsF =
            ExpressionObserver.Compile(
                (Window1 t) =>
                    t.Persons.StartingWith( t.WhenInitialized().Value == null ? string.Empty : t.WhenInitialized().Value.NamesStartingWithBox.Text )
            );        
                

    }
}
