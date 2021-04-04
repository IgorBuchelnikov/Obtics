using System;
using System.Text;
using System.Collections.Generic;

#if TVDP_UNITTESTING
using TvdP.UnitTesting;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif


using Obtics.Values;
using Obtics;
using System.ComponentModel;

namespace ObticsUnitTest.Obtics
{
    using System.Linq;
    using EX = global::Obtics.Collections.ObservableEnumerable;

    //Exptension method for Regression5
    public static class stringExtensions
    {
        public static bool IsNotNullOrEmpty(this string str)
        { return !string.IsNullOrEmpty(str); }
    }

    /// <summary>
    /// Summary description for Regression
    /// </summary>
    [TestClass]
    public partial class Regression
    {
        public Regression()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        public interface ITranformer
        {
            IValueProvider<int> Transform(IValueProvider<int> s);
        }

        public class Transformer : ITranformer
        {
            public IValueProvider<int> _Offset = ValueProvider.Dynamic<int>();
            public IValueProvider<int> _Nought = ValueProvider.Dynamic<int>(0);

            #region ITranformer Members

            public IValueProvider<int> Transform(IValueProvider<int> s)
            { return s.Convert( _Offset, _Nought, (v, o, n) => v.Value + o.Value ); }

            #endregion
        }

        public class State
        {
            IValueProvider<int> _SelectedInt = ValueProvider.Dynamic<int>();

            public IValueProvider<int> SelectedInt { get { return _SelectedInt; } }

            IValueProvider<Transformer> _SelectedTransformer = ValueProvider.Dynamic<Transformer>();

            public IValueProvider<Transformer> SelectedTransformer { get { return _SelectedTransformer; } }
        }

        public class Controller
        {
            State _State = new State();

            public State State { get { return _State; } }

            public IValueProvider<int> Result
            {
                get
                {
                    return                        
                        ValueProvider.Convert<IValueProvider<int>,Transformer,IValueProvider<int>>(
                            ValueProvider.Static(State.SelectedInt),
                            State.SelectedTransformer,
                             (_2, _4) => _4.Value.Transform(_2.Value)
                        )
                        .Cascade();
                }
            }
        }

        [TestMethod]
        public void Regression1()
        {
            Controller controller = new Controller();

            controller.State.SelectedTransformer.Value = new Transformer();
            controller.State.SelectedTransformer.Value._Offset.Value = 10;

            int resultValue;

            var result = controller.Result;

            ((INotifyPropertyChanged)result).PropertyChanged +=
                delegate(object sender, PropertyChangedEventArgs args)
                {
                    if (args.PropertyName == SIValueProvider.ValuePropertyName)
                        resultValue = result.Value;
                };

            resultValue = result.Value;

            controller.State.SelectedInt.Value = 1;

            Assert.AreEqual(11, resultValue, "1:Expected 11");

            controller.State.SelectedInt.Value = 2;

            Assert.AreEqual(12, resultValue, "2:Expected 12");

            controller.State.SelectedTransformer.Value._Offset.Value = 20;

            Assert.AreEqual(22, resultValue, "3:Expected 22");

            var transformer = new Transformer();
            transformer._Offset.Value = 30;

            controller.State.SelectedTransformer.Value = transformer;

            Assert.AreEqual(32, resultValue, "4:Expected 32");

            controller.State.SelectedInt.Value = 3;

            Assert.AreEqual(33, resultValue, "5:Expected 33");
        }

        public class R2A<T>
        {
            IValueProvider<T> _PropA = ValueProvider.Dynamic<T>();

            protected virtual IValueProvider<T> PropA
            { get { return _PropA; } }
        }

        public class R2B : R2A<string>
        {
            public R2B()
            { PropA.Value = "abc"; }

            public virtual IValueProvider<int> PropB
            { get { return ExpressionObserver.Execute(() => this.PropA.Value.Length); } }
        }

        [TestMethod]
        public void Regression2()
        {
#if !SILVERLIGHT
            var permission = new System.Security.Permissions.ReflectionPermission(System.Security.Permissions.PermissionState.Unrestricted);

            try
            {
                permission.Demand();
            }
            catch (System.Security.SecurityException)
            {
                //This regression test can be executed only when we have full reflection permission.
                //We want to access a protected property.
                return;
            }
#endif

            R2B r2b = new R2B();
            Assert.AreEqual(3, r2b.PropB.Value);
        }

        [TestMethod]
        public void Regression4()
        {
            var persons = new List<Person>();

            var person1 = new Person { Name = "Sonja" };
            var person2 = new Person { Name = "Conan" };
            var person3 = new Person { Name = "Sonja" };
            var person4 = new Person { Name = "Bilbo" };

            persons.Add(person1);
            persons.Add(person2);
            persons.Add(person3);
            persons.Add(person4);

            var observable = ExpressionObserver.Execute(persons, p => p.Select(item => item.Name).Distinct().OrderBy(name => name)).Cascade();

            var client = new ObticsUnitTest.Helpers.CollectionTransformationClientSNCC<string>();
            client.Source = observable;

            Assert.IsTrue(System.Linq.Enumerable.SequenceEqual(new string[] { "Bilbo", "Conan", "Sonja" }, client.Buffer), "Initial content");
            //Assert.IsTrue(System.Linq.Enumerable.SequenceEqual(new string[] { "Bilbo", "Conan", "Sonja", "Sonja" }, client.Buffer), "Initial content");

            person4.Name = "Gollem";
            person1.Name = "Sam";

            Assert.IsTrue(System.Linq.Enumerable.SequenceEqual(new string[] { "Conan", "Gollem", "Sam", "Sonja" }, client.Buffer), "Updated content");
        }

        public class FClass : INotifyPropertyChanged
        {
            public string Word { get; set; }

            string _Function;

            public string Function
            {
                get { return _Function; }
                set
                {
                    if (_Function != value)
                    {
                        _Function = value;

                        if(PropertyChanged != null)
                            PropertyChanged(this, new PropertyChangedEventArgs("Function"));
                    }
                }
            }

            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion
        }


        [TestMethod]
        public void Regression5()
        {
            var functions = new System.Collections.ObjectModel.ObservableCollection<FClass>();

            functions.Add(new FClass { Function = "A", Word = "1" });
            functions.Add(new FClass { Function = "A", Word = "2" });
            functions.Add(new FClass { Function = "A", Word = "3" });
            functions.Add(new FClass { Function = "B", Word = "1" });
            functions.Add(new FClass { Function = "B", Word = "2" });
            functions.Add(new FClass { Function = "C", Word = "1" });

            var word = "2";

            var query = ExpressionObserver.Execute(
                functions,
                word,
                (fs,wrd) => fs.Where(f => f.Word == wrd && f.Function.IsNotNullOrEmpty() )
                                     .GroupBy(f => f.Function)
                                     .Select(g => g.Key)
                                     .FirstOrDefault()
            );

            var client = new ObticsUnitTest.Helpers.ValueProviderClientNPC<string>();
            client.Source = query;

            Assert.AreEqual("A", client.Buffer, "Unexpected initial value.");

            functions[0].Function = "D";

            Assert.AreEqual("A", client.Buffer, "Unexpected followup value 1.");

            functions[1].Function = "D";

            Assert.AreEqual("D", client.Buffer, "Unexpected followup value 2.");

#if SILVERLIGHT
            var movedItem = functions[4];
            functions.RemoveAt(4);
            functions.Insert(0, movedItem);
#else
            functions.Move(4, 0);
#endif

            Assert.AreEqual("B", client.Buffer, "Unexpected followup value 3.");
        }


        public class R7B : System.Windows.DependencyObject, INotifyPropertyChanged
        {
            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion

            public string Name
            {
                get { return (string)GetValue(NameProperty); }
                set { SetValue(NameProperty, value); }
            }

            // Using a DependencyProperty as the backing store for Name.  This enables animation, styling, binding, etc...
            public static readonly System.Windows.DependencyProperty NameProperty =
                System.Windows.DependencyProperty.Register("Name", typeof(string), typeof(R7B), new System.Windows.PropertyMetadata(string.Empty, NameChangedCallback));

            static void NameChangedCallback(System.Windows.DependencyObject sender, System.Windows.DependencyPropertyChangedEventArgs args)
            {
                var r7b = (R7B)sender;

                if (r7b.PropertyChanged != null)
                    r7b.PropertyChanged(r7b, new PropertyChangedEventArgs("Name"));
            }
        }

        public class R7A : System.Windows.DependencyObject, INotifyPropertyChanged
        {
            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion

            public R7B Child
            {
                get { return (R7B)GetValue(ChildProperty); }
                set { SetValue(ChildProperty, value); }
            }

            R7B _Child2;

            public R7B Child2
            {
                get { return _Child2; }
                set 
                { 
                    _Child2 = value; 
                    if(PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("Child2"));
                }
            }


            // Using a DependencyProperty as the backing store for Child.  This enables animation, styling, binding, etc...
            public static readonly System.Windows.DependencyProperty ChildProperty =
                System.Windows.DependencyProperty.Register("Child", typeof(R7B), typeof(R7A), new System.Windows.PropertyMetadata(null, ChildChangedCallback));

            static void ChildChangedCallback(System.Windows.DependencyObject sender, System.Windows.DependencyPropertyChangedEventArgs args)
            {
                //var r7a = (R7A)sender;

                //if (r7a.PropertyChanged != null)
                //    r7a.PropertyChanged(r7a, new PropertyChangedEventArgs("Child"));
            }
        }

        [TestMethod]
#if SILVERLIGHT
        [UIThread]
#endif
        public void Regression7()
        {
            var r7a = new R7A();

            var query = ExpressionObserver.Execute(r7a, c => c.Child.Name).OnException((NullReferenceException ex) => "Exception");

            var client = new ObticsUnitTest.Helpers.ValueProviderClientNPC<string>();
            client.Source = query;

            Assert.AreEqual("Exception", client.Buffer, "Expected null initialy");

            var r7b = new R7B();

#if !SILVERLIGHT

            //custom dependency properties are NOT observable under silverlight

            r7a.Child = r7b;
            r7b.Name = "Frodo";

            Assert.AreEqual("Frodo", client.Buffer, "Expected 'Frodo'");

            r7b.Name = "Bilbo";

            Assert.AreEqual("Bilbo", client.Buffer, "Expected 'Bilbo'");

            r7b = new R7B();
            r7b.Name = "Sam";
            r7a.Child = r7b;

            Assert.AreEqual("Sam", client.Buffer, "Expected 'Sam'");

#endif
            query = ExpressionObserver.Execute(r7a, c => c.Child2.Name).OnException((NullReferenceException ex) => "Exception");

            client = new ObticsUnitTest.Helpers.ValueProviderClientNPC<string>();
            client.Source = query;

            Assert.AreEqual("Exception", client.Buffer, "Expected null initialy");

            r7b = new R7B();
            r7a.Child2 = r7b;
            r7b.Name = "Frodo";

            Assert.AreEqual("Frodo", client.Buffer, "Expected 'Frodo'");

            r7b.Name = "Bilbo";

            Assert.AreEqual("Bilbo", client.Buffer, "Expected 'Bilbo'");

            r7b = new R7B();
            r7b.Name = "Sam";
            r7a.Child2 = r7b;

            Assert.AreEqual("Sam", client.Buffer, "Expected 'Sam'");
        }
    }
}

namespace ObticsUnitTest.Obtics
{
    using global::Obtics.Collections;
using System.Windows;

    /// <summary>
    /// Summary description for Regression
    /// </summary>
    public partial class Regression
    {
        public class Person : INotifyPropertyChanged
        {
            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion

            string _Name;

            public string Name
            {
                get { return _Name; }
                set
                {
                    if (value != _Name)
                    {
                        _Name = value;

                        if( PropertyChanged != null )
                            PropertyChanged(this, new PropertyChangedEventArgs("Name"));
                    }
                }
            }
        }


        [TestMethod]
        public void Regression3()
        {
            var persons = new List<Person>();

            var person1 = new Person { Name = "Sonja" };
            var person2 = new Person { Name = "Conan" };
            var person3 = new Person { Name = "Sonja" };
            var person4 = new Person { Name = "Bilbo" };

            persons.Add(person1);
            persons.Add(person2);
            persons.Add(person3);
            persons.Add(person4);

            var observable = persons.Select(item => ValueProvider.Properties(item).Get<string>("Name")).Distinct().OrderBy(name => name);

            var client = new ObticsUnitTest.Helpers.CollectionTransformationClientSNCC<string>();
            client.Source = observable;

            Assert.IsTrue(System.Linq.Enumerable.SequenceEqual(new string[] { "Bilbo", "Conan", "Sonja" }, client.Buffer), "Initial content");
            //Assert.IsTrue(System.Linq.Enumerable.SequenceEqual(new string[] { "Bilbo", "Conan", "Sonja", "Sonja" }, client.Buffer), "Initial content");

            person4.Name = "Gollem";
            person1.Name = "Sam";

            Assert.IsTrue(System.Linq.Enumerable.SequenceEqual(new string[] { "Conan", "Gollem", "Sam", "Sonja" }, client.Buffer), "Updated content");
        }

#if !SILVERLIGHT
        public class DoubleObservable : DependencyObject, INotifyPropertyChanged
        {
            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion

            public int MyProperty
            {
                get { return (int)GetValue(MyPropertyProperty); }
                set { SetValue(MyPropertyProperty, value); }
            }

            // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
            public static readonly DependencyProperty MyPropertyProperty =
                DependencyProperty.Register("MyProperty", typeof(int), typeof(DoubleObservable), new PropertyMetadata(0));
        }

        [TestMethod]
        public void Regression6()
        {
            var source = new DoubleObservable();

            var query = ExpressionObserver.Execute(source, s => s.MyProperty + 10);

            var client = new ObticsUnitTest.Helpers.ValueProviderClientNPC<int>();
            client.Source = query;

            Assert.AreEqual(10, client.Buffer, "Initial value should be 10");

            source.MyProperty = 5;

            Assert.AreEqual(15, client.Buffer, "After change value should be 15");
        }
#endif
    }
}