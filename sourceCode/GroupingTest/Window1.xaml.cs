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
using OE = Obtics.Collections.ObservableEnumerable;

namespace GroupingTest
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Observable list of integers we are going to do our calculations with.
        /// </summary>
        //ObservableCollection<int> _Integers = new ObservableCollection<int>();

        Obtics.Collections.ObservableDictionary<int, int> _Integers = new Obtics.Collections.ObservableDictionary<int, int>();

        /// <summary>
        /// Method that calculates the number of bits set in an integer value.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        static int Bits(int x)
        {
            var c = 0;

            for (int i = 0; i < 32; ++i)
            {
                if ((x & 1) == 1)
                    ++c;

                x >>= 1;
            }

            return c;
        }

        /// <summary>
        /// The calculating method. Takes all integers from _Integers
        /// does some filtering and groups them by number of bits set.
        /// 
        /// Returns a sequence of pairs containing {#bits,#integers having bits set}
        /// </summary>
        public IEnumerable<KeyValuePair<int, int>> BitScore
        {
            get
            {
                return
                    //Do sorting on the highest level
                    //Sorting here tells all sources (arguments of the OrderBy method) 
                    //that the original order of elements is NOT important
                    //This will result in some significant optimization.
                    OE.OrderBy(
                        //async; a worker thread will update the _Integers collection
                        //This method will make sure that all change notifications 1 get buffered
                        //2 get processed by the UI thread dispatcher. (Actually the dispatcher of the
                        //thread that calls the getter of the BitScore property) 
                        OE.Async(
                            //We use Execute here directly. This is fine because we will not call
                            //the BitScore getter very often.
                            ExpressionObserver.Execute(
                                () =>
                                    from kvp in
                                        (
                                            (
                                                //filter integers, group and count
                                                from kvp in _Integers
                                                where kvp.Key % 3 == 1
                                                group kvp by Bits(kvp.Key) into g
                                                select new KeyValuePair<int, int>(g.Key, g.Sum(gkvp => gkvp.Value))
                                            ).Union(
                                                //insert padding values for those bit counts that
                                                //we don't have integers for
                                                from x in Enumerable.Range(0, 33)
                                                select new KeyValuePair<int, int>(x, 0)
                                            )
                                        )
                                    //for each #bits we can have 2 pairs. One with nr of integers and one with 0
                                    //group these and sum the 0 and nr of integers values
                                    //the result will be a sequence of #bits paired with either 0 or the #integers
                                    //with that amount of bits set.
                                    group kvp.Value by kvp.Key into kvpg
                                    select new KeyValuePair<int, int>(kvpg.Key, kvpg.Sum())

                            ).Cascade() //we always need to do that.
                        ),kvp => kvp.Key
                    );
            }
        }

        /// <summary>
        /// Safe parse method. Returns null and not an exception when parsing fails.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        static int? Parse(string text)
        {
            int res;
            var valid = int.TryParse(text, out res);
            return valid ? (int?)res : null;
        }

        /// <summary>
        /// Calculates the sum of total manipulations on the _Integers collection for display on the screen.
        /// Demonstrates the use of the WhenInitialized() extension method.
        /// </summary>
        public IValueProvider<int?> TotalManipulations
        {
            get
            {
                return ExpressionObserver.Execute(
                    () => Parse(this.WhenInitialized().Value.textBox1.Text) + Parse(this.WhenInitialized().Value.textBox2.Text)
                );
            }
        }

        #region CollectionLoad ; The number of integers that will be inserted in the _Integers collection
        IValueProvider<int> _CollectionLoad = ValueProvider.Dynamic(50000);

        /// <summary>
        /// The number of integers that will be inserted in the _Integers collection.
        /// </summary>
        public IValueProvider<int> CollectionLoad { get { return _CollectionLoad.Async(); } }
        #endregion

        #region PostManipulations ; The number of manipulations on the _Integers collection after it has been fully loaded
        IValueProvider<int> _PostManipulations = ValueProvider.Dynamic(100000);

        /// <summary>
        /// The number of manipulations on the _Integers collection after it has been fully loaded.
        /// </summary>
        public IValueProvider<int> PostManipulations { get { return _PostManipulations.Async(); } }
        #endregion

        #region WorkTime ; The continualy updated ammount time the current run took.
        IValueProvider<TimeSpan> _WorkTime = ValueProvider.Dynamic(TimeSpan.Zero);

        /// <summary>
        /// The continualy updated ammount time the current run took.
        /// </summary>
        public IValueProvider<TimeSpan> WorkTime { get { return _WorkTime.Async(); } }
        #endregion

        #region Progress ; The factor of completion 0 -> 1 of the current run
        IValueProvider<double> _Progress = ValueProvider.Dynamic(0.0);

        /// <summary>
        /// The factor of completion 0 -> 1 of the current run
        /// </summary>
        public IValueProvider<double> Progress { get { return _Progress.Async(); } }
        #endregion

        #region Running ; If there is a run running
        IValueProvider<bool> _Running = ValueProvider.Dynamic(false);

        /// <summary>
        /// If there is a run running. Can't start a new run when there is.
        /// </summary>
        public IValueProvider<bool> Running { get { return _Running.Async(); } }
        #endregion

        /// <summary>
        /// The inverted value of Running
        /// </summary>
        public IValueProvider<bool> NotRunning { get { return _Running.Invert().Async(); } }

        #region Cancel ; Set true to cancel a current Run.
        IValueProvider<bool> _Cancel = ValueProvider.Dynamic(false);

        /// <summary>
        /// Set true to cancel a current Run.
        /// </summary>
        public IValueProvider<bool> Cancel { get { return _Cancel.Async(); } }
        #endregion

        /// <summary>
        /// Method executed by worker thread. Clear integer collection, fill it and manipulate it.
        /// </summary>
        void Run()
        {
            unchecked
            {
                //clear
                _Integers.Clear();

                //set some counters
                Progress.Value = 0.0;
                WorkTime.Value = TimeSpan.Zero;
                DateTime startTime = DateTime.Now;
                var load = CollectionLoad.Value;
                var pm = PostManipulations.Value;
                var totalWork = load + pm;
                var workStep = totalWork / 100;
                var workCount = 0;
                var rnd = new Random(DateTime.Now.Millisecond);

                //fill _Integers collection wit hrandom integers
                for (int i = 0; i < load; ++i)
                {
                    int v = rnd.Next() + rnd.Next();
                    int count;

                    _Integers[v] = _Integers.TryGetValue(v, out count) ? count + 1 : 1;

                    if (++workCount % workStep == 0)
                    {
                        //advance progress and time counter
                        Progress.Value = (double)workCount / (double)totalWork;
                        WorkTime.Value = DateTime.Now - startTime;
                    }

                    if (Cancel.Value)
                        return;
                }

                var keys = new List<int>(_Integers.Keys);
                var w = keys.Count;

                //Manipulate the filled integer collection.
                for (int i = 0; i < pm; ++i)
                {
                    if (w == 0)
                    {
                        keys = new List<int>(_Integers.Keys);
                        w = keys.Count;
                    }

                    var v = keys[--w];

                    int count;

                    count = _Integers[v];

                    if (count == 1)
                        _Integers.Remove(v);
                    else
                        _Integers[v] = count - 1;

                    v = rnd.Next() + rnd.Next();

                    _Integers[v] = _Integers.TryGetValue(v, out count) ? count + 1 : 1;

                    if (++workCount % workStep == 0)
                    {
                        //advance progress and time counter
                        Progress.Value = (double)workCount / (double)totalWork;
                        WorkTime.Value = DateTime.Now - startTime;
                    }

                    if (Cancel.Value)
                        return;                
                }

                //Clear progress counter and set final processing time.
                Progress.Value = 0.0;
                WorkTime.Value = DateTime.Now - startTime;
            }
        }


        /// <summary>
        /// Start button clicked; start a new worker thread.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (!Running.Value)
            {
                //Set Running and Clear cancel flag.
                Cancel.Value = false;
                Running.Value = true;

                System.Threading.ThreadPool.QueueUserWorkItem(
                    new System.Threading.WaitCallback(
                        delegate(object dummy)
                        {
                            try
                            {
                                Run();
                            }
                            finally
                            {
                                //When done clear Running and Cancel flags
                                Running.Value = false;
                                Cancel.Value = false;
                            }
                        }
                    )
                );
            }
        }

        /// <summary>
        /// Cancel button clicked; Set the Cancel flag to signal the worker thread.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if (Running.Value)
                Cancel.Value = true;
        }
    }
}
