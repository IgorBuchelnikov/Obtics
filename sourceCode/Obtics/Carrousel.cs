using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obtics.Collections;
using TvdP.Collections;

namespace Obtics
{
    //Carrousel implementation that will never lose a reference to a live item

    internal class Carrousel
    {
        /// <summary>
        /// Actual Carrousel implementation. Based on ConcurrentWeakHashtable. First tries to find an existing
        /// object. If it doesn't exist it creates a new one.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        static class InternalCarrousel<TKey, TOut> 
            where TOut : class
            where TKey : class
        {
            class KeyComparer : IEqualityComparer<TKey>
            {
                #region IEqualityComparer<TKey> Members

                public bool  Equals(TKey x, TKey y)
                {
 	                return ObticsEqualityComparer<TKey>.Default.Equals(x, y);
                }

                public int  GetHashCode(TKey obj)
                {
 	                return 0;
                }

                #endregion
            }

            static WeakDictionary<TKey, Int32, TOut> _Instance = new WeakDictionary<TKey, Int32, TOut>(new KeyComparer(), EqualityComparer<Int32>.Default);

            public static TOut Get(TKey key, Func<TKey, TOut> creator)
            {
                return _Instance.GetOrAdd(key, ObticsEqualityComparer<TKey>.Default.GetHashCode(key), (k, _) => creator(k));
            }
        }

        //Generic Get methods. Requires created class to be comparable with key.
        public static TOut Get<TOut, TKey>(TKey key, Func<TKey, TOut> creator) 
            where TOut : class 
            where TKey : class
        { return InternalCarrousel<TKey,TOut>.Get(key, creator); }

        public static TOut Get<TOut, TKey1, TKey2>(TKey1 key1, TKey2 key2, Func<Tuple<TKey1, TKey2>, TOut> creator) 
            where TOut : class 
        { return InternalCarrousel<Tuple<TKey1, TKey2>, TOut>.Get(Tuple.Create(key1, key2), creator); }

        public static TOut Get<TOut, TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, Func<Tuple<TKey1, TKey2, TKey3>, TOut> creator) 
            where TOut : class
        { return InternalCarrousel<Tuple<TKey1, TKey2, TKey3>, TOut>.Get(Tuple.Create(key1, key2, key3), creator); }

        public static TOut Get<TOut, TKey1, TKey2, TKey3, TKey4>(TKey1 key1, TKey2 key2, TKey3 key3, TKey4 key4, Func<Tuple<TKey1, TKey2, TKey3, TKey4>, TOut> creator) 
            where TOut : class
        { return InternalCarrousel<Tuple<TKey1, TKey2, TKey3, TKey4>, TOut>.Get(Tuple.Create(key1, key2, key3, key4), creator); }


        //Overrides special for ObservableObjectBase derived objects. Don't require a Creator method to be passed. Already have one.
        static TOut ConstructObservableObject<TOut, TPrms>(TPrms prms) 
            where TOut : ObservableObjectBase<TPrms>, new()
        {
            var res = new TOut();
            res.Initialize(prms);
            return res;
        }

        public static TOut Get<TOut, TKey>(TKey key) 
            where TOut : ObservableObjectBase<TKey>, new()
            where TKey : class
        { return InternalCarrousel<TKey, TOut>.Get(key, (Func<TKey, TOut>)ConstructObservableObject<TOut, TKey>); }

        public static TOut Get<TOut, TKey1, TKey2>(TKey1 key1, TKey2 key2) where TOut : ObservableObjectBase<Tuple<TKey1, TKey2>>, new()
        { return InternalCarrousel<Tuple<TKey1, TKey2>, TOut>.Get(Tuple.Create(key1, key2), (Func<Tuple<TKey1, TKey2>, TOut>)ConstructObservableObject<TOut, Tuple<TKey1, TKey2>>); }

        public static TOut Get<TOut, TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3) where TOut : ObservableObjectBase<Tuple<TKey1, TKey2, TKey3>>, new()
        { return InternalCarrousel<Tuple<TKey1, TKey2, TKey3>, TOut>.Get(Tuple.Create(key1, key2, key3), (Func<Tuple<TKey1, TKey2, TKey3>, TOut>)ConstructObservableObject<TOut, Tuple<TKey1, TKey2, TKey3>>); }

        public static TOut Get<TOut, TKey1, TKey2, TKey3, TKey4>(TKey1 key1, TKey2 key2, TKey3 key3, TKey4 key4) where TOut : ObservableObjectBase<Tuple<TKey1, TKey2, TKey3, TKey4>>, new()
        { return InternalCarrousel<Tuple<TKey1, TKey2, TKey3, TKey4>, TOut>.Get(Tuple.Create(key1, key2, key3, key4), (Func<Tuple<TKey1, TKey2, TKey3, TKey4>, TOut>)ConstructObservableObject<TOut, Tuple<TKey1, TKey2, TKey3, TKey4>>); }
    }
}
