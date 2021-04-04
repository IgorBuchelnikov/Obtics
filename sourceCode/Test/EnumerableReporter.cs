using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Specialized;

namespace Test
{
    /// <summary>
    ///This is an object that is a client to an observable collection (like Collection Transformations)
    ///Listens for changes in the underlying collection and dumps information about those changes
    ///to the console.
    /// </summary>
    class EnumerableReporter
    {
        public EnumerableReporter(IEnumerable source, string prefix)
        {
            _Source = source;
            _Prefix = prefix;

            //register for change notifications
            var ncc = (INotifyCollectionChanged)source;

            if (ncc != null)
                ncc.CollectionChanged += ncc_CollectionChanged;

            //initialize the buffer (after registering for change notifications)
            _Buffer.Clear();
            _Buffer.AddRange(System.Linq.Enumerable.Cast<object>(source));

            //write some info to the console
            Console.Out.WriteLine(prefix + " Initialy ->");
            Dump("");
            Console.Out.WriteLine("");
            Console.Out.WriteLine("");
        }

        void ncc_CollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            //Dump info about our old status
            Console.Out.WriteLine(_Prefix + " " + args.Action.ToString() + " ->");
            Dump("");
            Console.Out.WriteLine("");

            //update the buffer
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    _Buffer.InsertRange(args.NewStartingIndex, System.Linq.Enumerable.Cast<object>(args.NewItems));
                    break;
                case NotifyCollectionChangedAction.Move:
                    _Buffer.RemoveRange(args.OldStartingIndex, args.NewItems.Count);
                    _Buffer.InsertRange(args.NewStartingIndex, System.Linq.Enumerable.Cast<object>(args.NewItems));
                    break;
                case NotifyCollectionChangedAction.Remove:
                    _Buffer.RemoveRange(args.OldStartingIndex, args.OldItems.Count);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    _Buffer.RemoveRange(args.NewStartingIndex, args.OldItems.Count);
                    _Buffer.InsertRange(args.NewStartingIndex, System.Linq.Enumerable.Cast<object>(args.NewItems));
                    break;
                //case NotifyCollectionChangedAction.Reset:
                default:
                    _Buffer.Clear();
                    _Buffer.AddRange(System.Linq.Enumerable.Cast<object>(_Source));
                    break;
            }

            //dump info about our new status.
            Dump("");
            Console.Out.WriteLine("");
            Console.Out.WriteLine("");
        }

        //write our buffer out, item for item.
        void Dump(string msg)
        {
            Console.Out.Write(msg + "[");
            bool first = true;

            foreach (object item in _Buffer)
            {
                if (!first)
                    Console.Out.Write(", ");

                first = false;

                Console.Out.Write(item == null ? "<null>" : item.ToString());
            }

            Console.Out.Write("]");
        }

        IEnumerable _Source;
        string _Prefix;
        List<object> _Buffer = new List<object>();
    }
}
