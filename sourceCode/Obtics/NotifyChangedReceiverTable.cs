using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Obtics.Values;

#if SILVERLIGHT
using TvdP.Collections;
#else
using System.Collections.Concurrent;
#endif

namespace Obtics
{
    struct NotifyChangedReceiverTable
    {
        //This class becomes the event consumer container when the number of registered consumers exceeds 32 (or so).
        //It allows registration and unregistration in constant time. It also reserves space for the external cache
        //and a reset counter that allows cancelation of previous events when a reset is comming up.
        sealed class UnlimitedReceiverTable : ConcurrentDictionary<IReceiveChangeNotification, IReceiveChangeNotification>
        {

            public bool RemoveReceiver(IReceiveChangeNotification item)
            {
                IReceiveChangeNotification dummy;
                return this.TryRemove(item, out dummy);
            }

            //token value that indicates that the _ExternalCache holds no value. (just null IS a value)
            public static object _NullValue = new object();

            //object reference to the value stored in the external cache.
            public volatile object _ExternalCache = _NullValue;

            //reset count; the number of reset events that have been sent through this ReceiverTable instance.
            //when an event is being broadcast to all clients this value is checked. If it changes it means a
            //reset is comming up so continued broadcasting of the current event is 'pointless' and can be cancelled.
            public volatile int _ResetCount = 0;
        }

        //token value that indicates that the _NCReceivers reference is locked for maintenance (growing)
        static readonly object _NCReceiverLock = new object();

        //object refence to either
        //1. A single Receiver directly
        //2. An array of upto 32 receivers
        //3. A ReceiverTable instance with an unlimited number of receivers.
        volatile object _NCReceivers;

        //The current number of receivers refered to by _NCReceivers
        volatile int _NCReceiverCount;

        public int NCReceiverCount
        { get { return _NCReceiverCount; } }

        public bool SubscribeINC(IReceiveChangeNotification receiver)
        {
            while (true)
            {
                object handler;

                //aquire current handler
                while (true)
                {
                    handler = _NCReceivers;

                    if (!object.ReferenceEquals(handler, _NCReceiverLock))
                        break;

                    //while handler refers to our lock token wait.
                    Thread.Sleep(0);
                }

                if (handler == null)
                {
                    //handler completely empty. This will probably be the first and only handler registered.
#pragma warning disable 420
                    if (Interlocked.CompareExchange(ref _NCReceivers, receiver, null) != null)
#pragma warning restore 420
                        continue; //obviously some other thread beat us to it.. try again
                    else
                        break; //success insert
                }
                else
                {
                    var existingReceiver = handler as IReceiveChangeNotification;

                    if (existingReceiver != null)
                    {
                        //handler is a direct reference to a receiver.
                        //we must replace it with an array containing the existing receiver
                        //and our new receiver.
                        if (object.ReferenceEquals(receiver, existingReceiver))
                            return false; //wait.. the existing receiver is our receiver... ?? .. no insert

                        var receiverArray = new IReceiveChangeNotification[4];

                        receiverArray[0] = existingReceiver;
                        receiverArray[1] = receiver;

#pragma warning disable 420
                        if (Interlocked.CompareExchange(ref _NCReceivers, receiverArray, handler) != handler)
#pragma warning restore 420
                            //handler already replaced by other thread... try again.
                            continue;

                        break; //success insert
                    }
                    else
                    {
                        var receiverArray = handler as IReceiveChangeNotification[];

                        if (receiverArray != null)
                        {
                            //handler is an array of receivers.

                            //try insert in free spot
                            var receiverArrayLength = receiverArray.Length;
                            int i = 0;

                            //first check if already present.
                            for (; i < receiverArrayLength; ++i)
                                if (object.ReferenceEquals(receiverArray[i], receiver))
                                    return false; //already present.. no insert

                            //ok, insert in empty spot
                            for (i = 0; i < receiverArrayLength; ++i)
                                if (Interlocked.CompareExchange(ref receiverArray[i], receiver, null) == null)
                                    break;

                            if (i != receiverArrayLength)
                            {
                                //insert succeeded
                                //check array not replaced
                                if (!object.ReferenceEquals(_NCReceivers, handler))
                                    continue; //some other thread did that.. start all over again.
                                else
                                    break; //success insert
                            }

                            //couldn't find free spot.. no room.. grow
                            //lock first.
#pragma warning disable 420
                            if (!object.ReferenceEquals(Interlocked.CompareExchange(ref _NCReceivers, _NCReceiverLock, handler), handler))
#pragma warning restore 420
                                continue; //some other thread changed handler.. try again

                            //we own lock;                            

                            if (receiverArrayLength < 32)
                            {
                                //array size not yet 32
                                //new array, copy contents
                                var newArray = new IReceiveChangeNotification[receiverArrayLength << 1];
                                receiverArray.CopyTo(newArray, 0);

                                //add our receiver
                                newArray[receiverArrayLength] = receiver;

                                //unlocked.. success insert
                                _NCReceivers = newArray;
                                break;
                            }
                            else
                            {
                                //size already 32.. create table
                                var newTable = new UnlimitedReceiverTable();

                                for (int j = 0; j < receiverArrayLength; ++j)
                                {
                                    var rec = receiverArray[j];

                                    //an entry might already have been removed before we locked
                                    if (rec != null)
                                        newTable.TryAdd(rec, rec);
                                }

                                newTable.TryAdd(receiver, receiver);

                                //unlocked.. success insert
                                _NCReceivers = newTable;

                                break;
                            }
                        }
                        else
                        {
                            //handler can now only be a table. just insert.
                            //A table never gets replaced.
                            if (((UnlimitedReceiverTable)handler).TryAdd(receiver, receiver))
                                break; //success insert
                            else
                                return false; //already present.. no insert                            
                        }
                    }
                }
            }

            //we get here then a successfull insert has taken place.
#pragma warning disable 420
            Interlocked.Increment(ref _NCReceiverCount);
#pragma warning restore 420

            return true;
        }

        public bool UnsubscribeINC(IReceiveChangeNotification receiver)
        {
            while (true)
            {
                object handler;

                while (true)
                {
                    handler = _NCReceivers;

                    if (!object.ReferenceEquals(handler, _NCReceiverLock))
                        break;

                    Thread.Sleep(0);
                }

                if (handler == null)
                    return false;

                var existingReceiver = handler as IReceiveChangeNotification;

                if (existingReceiver != null)
                {
                    if (!object.ReferenceEquals(receiver, existingReceiver))
                        return false;

#pragma warning disable 420
                    if (!object.ReferenceEquals(Interlocked.CompareExchange(ref _NCReceivers, null, existingReceiver), existingReceiver))
#pragma warning restore 420
                        continue; //try again
                    else
                        break; //success delete
                }
                else
                {
                    var existingArray = handler as IReceiveChangeNotification[];

                    if (existingArray != null)
                    {
                        //remove when found
                        int i = 0, end = existingArray.Length;

                        for (; i < end; ++i)
                            if (object.ReferenceEquals(Interlocked.CompareExchange(ref existingArray[i], null, receiver), receiver))
                                break; //found and replaced with null

                        if (i == end)
                            return false; //not in the array.. no problem, nothing done

                        //check if still using existingArray
                        if (!object.ReferenceEquals(_NCReceivers, existingArray))
                            continue; //array replaced.. try again
                        else
                            break; //success delete
                    }
                    else
                    {
                        {
                            IReceiveChangeNotification dummy;

                            //must have table;
                            if (((UnlimitedReceiverTable)handler).TryRemove(receiver, out dummy))
                                break; //successful delete
                            else
                                return false;
                        }

                        //a table never gets replaced.. success
                    }
                }
            }

#pragma warning disable 420
            Interlocked.Decrement(ref _NCReceiverCount);
#pragma warning restore 420

            return true;
        }


#if PARALLEL
        public void SendMessage(object sender, INCEventArgs message, bool parallelizationForbidden)
        {
            bool parallel = !parallelizationForbidden && Tasks.SuggestParallelization;
#else
        public void SendMessage(object sender, INCEventArgs message)
        {
#endif
            if (message != null)
            {
                //We shouldn't receive exceptions from eventhandlers. If we do
                //we can't deal with them and the application should explode!
                object handler;

                while (true)
                {
                    handler = _NCReceivers;

                    if (!object.ReferenceEquals(handler, _NCReceiverLock))
                        break;

                    Thread.Sleep(0);
                }

                if (handler != null)
                {
                    var receiver = handler as IReceiveChangeNotification;

                    if (receiver != null)
                        receiver.NotifyChanged(sender, message);
                    else
                    {
                        var receiverArray = handler as IReceiveChangeNotification[];

                        if (receiverArray != null)
                        {
#if PARALLEL
                            if(parallel && DelayedActionRegistry.IsIn())
                            {
                                var stack = new Stack<Tasks.Future<IEnumerable<Action>>>();

                                for (int i = 0, end = receiverArray.Length; i < end; ++i)
                                {
                                    var r = receiverArray[i];

                                    if (r != null)
                                        stack.Push(
                                            Tasks.CreateFuture(
                                                () => DelayedActionRegistry.Reenter(() => r.NotifyChanged(sender, message))
                                            )
                                        );
                                }

                                while (stack.Count != 0)
                                {
                                    var evts = Tasks.GetResult(stack.Pop());
                                    DelayedActionRegistry.Register(evts);
                                }
                            }
                            else if (parallel)
                            {
                                for (int i = 0, end = receiverArray.Length; i < end; ++i)
                                {
                                    var r = receiverArray[i];

                                    if (r != null)
                                        Tasks.CreateTask(
                                            (_) => r.NotifyChanged(sender, message),
                                            false
                                        );
                                }
                            }
#endif
                            for (int i = 0, end = receiverArray.Length; i < end; ++i)
                            {
                                var r = receiverArray[i];

                                if (r != null)
                                    r.NotifyChanged(sender, message);
                            }
                        }
                        else
                        {
                            var receiverTable = (UnlimitedReceiverTable)handler;

#pragma warning disable 420
                            var resetCount = (
                                    message.Type == INCEventArgsTypes.CollectionReset ||
                                    message.Type == INCEventArgsTypes.ValueChanged 
                                ) ? Interlocked.Increment(ref receiverTable._ResetCount) : receiverTable._ResetCount;
#pragma warning restore 420

#if PARALLEL
                            if (parallel && DelayedActionRegistry.IsIn())
                            {
                                var stack = new Stack<Tasks.Future<IEnumerable<Action>>>();

                                //Creating a local copy of the receivertable contents first 
                                //appears slightly more performant in a multithreaded environment
                                //probably there is less congestion in this way.
                                foreach (var receiverItem in receiverTable.Values.ToArray())
                                {
                                    var r = receiverItem;

                                    stack.Push(
                                        Tasks.CreateFuture(
                                            () =>
                                                DelayedActionRegistry.Reenter(
                                                    () =>
                                                    {
                                                        if (resetCount == receiverTable._ResetCount)
                                                            r.NotifyChanged(sender, message);
                                                    }
                                                )
                                        )
                                    );
                                }

                                while (stack.Count != 0)
                                {
                                    var evts = Tasks.GetResult(stack.Pop());
                                    DelayedActionRegistry.Register(evts);
                                }
                            }
                            else if(parallel)
                                //Creating a local copy of the receivertable contents first 
                                //appears slightly more performant in a multithreaded environment
                                //probably there is less congestion in this way.
                                foreach (var receiverItem in receiverTable.Values.ToArray())
                                {
                                    var r = receiverItem;

                                    Tasks.CreateTask(
                                       _ =>
                                       {
                                           //if we get overtaken by a reset, cancel our events    
                                           if (resetCount == receiverTable._ResetCount)
                                               r.NotifyChanged(sender, message);
                                       },
                                       false
                                   );
                                }
                            else

#endif
                            //Creating a local copy of the receivertable contents first 
                            //appears slightly more performant in a multithreaded environment
                            //probably there is less congestion in this way.
                            foreach (var receiverItem in receiverTable.Values.ToArray())
                            {
                                //if we get overtaken by a reset, cancel our events    
                                if (resetCount != receiverTable._ResetCount)
                                    break;

                                receiverItem.NotifyChanged(sender, message);
                            }
                        }
                    }
                }
            }
        }

        public bool IsPivotNode
        { get { return _NCReceivers is UnlimitedReceiverTable; } }


        //Try to get a value from the external cache.
        //This method can be called without locking the object.
        public bool TryGetExternalCache(out object cachedValue)
        {
            var table = _NCReceivers as UnlimitedReceiverTable;

            if (table != null)
            {
                cachedValue = table._ExternalCache;
                return cachedValue != UnlimitedReceiverTable._NullValue;
            }

            cachedValue = UnlimitedReceiverTable._NullValue;
            return false;
        }

        //Set a value in the external cache. The object needs to be locked.
        public bool SetExternalCache(object cacheValue)
        {
            var table = _NCReceivers as UnlimitedReceiverTable;

            if (table != null)
            {
                table._ExternalCache = cacheValue;
                return true;
            }

            return false;
        }

        //Clears the external cache. The object needs to be locked.
        public bool ClearExternalCache()
        {
            var table = _NCReceivers as UnlimitedReceiverTable;

            if (table != null)
            {
                table._ExternalCache = UnlimitedReceiverTable._NullValue;
                return true;
            }

            return false;
        }
    }
}
