using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Collections.Specialized;

namespace Obtics.Collections
{
    /// <summary>
    /// Probeert iets eerst te converteren naar een Ilist of een ICollection.
    /// Lukt dat niet, dan wordt een lijst aangemaakt.
    /// </summary>
    internal static class CollectionsHelper
    {
        /// <summary>
        /// TypedEnumerable
        /// ConvertValue een untyped Enumerable naar een typed Enumerable
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static IEnumerable<TItem> TypedEnumerable<TItem>(IEnumerable source)
        {
            foreach (TItem item in source)
                yield return item;
        }

        public static void Fill<TType>(IList<TType> buffer, IEnumerator<TType> enumerator)
        {
            while (enumerator.MoveNext())
                buffer.Add(enumerator.Current);
        }


        public static IEnumerable<TType> EnumerableFromEnumerator<TType>(IEnumerator<TType> items)
        {
            while (items.MoveNext())
                yield return items.Current;
        }

        public static TType Extract<TType>(this IList<TType> buffer, int index)
        {
            var temp = buffer[index];
            buffer.RemoveAt(index);
            return temp;
        }

        public static void Rotate<TType>(IList<TType> buffer, int begin, int middle, int end)
        {
            //if (middle < begin)
            //{
            //    var tmp = middle;
            //    middle = begin;
            //    begin = tmp;
            //}

            //if (end < begin)
            //{
            //    var tmp = end;
            //    end = begin;
            //    begin = end;
            //}

            //if (end < middle)
            //{
            //    var tmp = end;
            //    end = middle;
            //    middle = end;
            //}

            if (begin == middle || middle == end)
                return;

            int next = middle;
            while (begin != next)
            {
                TType temp = buffer[begin];
                buffer[begin] = buffer[next];
                buffer[next] = temp;

                ++begin;
                ++next;

                if (next == end)
                    next = middle;
                else if (begin == middle)
                    middle = next;
            }
        }

        public static void Replace<TType>(IList<TType> buffer, int index, int oldCount, List<TType> newItems)
        {
            int newCount = newItems.Count;
            
            if (newCount == oldCount)
            {
                for (int i = 0; i < oldCount; ++i)
                    buffer[index + i] = newItems[i];
            }
            else if (newCount < oldCount)
            {
                var bufferAsList = buffer as List<TType>;

                if (bufferAsList != null)
                    bufferAsList.RemoveRange(index, oldCount - newCount);
                else
                    for (int i = 0, end = oldCount - newCount; i < end; ++i)
                        buffer.RemoveAt(index);

                for (int i = 0; i < newCount; ++i)
                    buffer[index + i] = newItems[i];
            }
            else
            {
                for (int i = 0; i < oldCount; ++i)
                    buffer[index + i] = newItems[i];

                var bufferAsList = buffer as List<TType>;

                if (bufferAsList != null)
                    bufferAsList.InsertRange(index + oldCount, newItems.GetRange(oldCount, newCount - oldCount));
                else
                    for (int i = 0, end = newCount - oldCount; i < end; ++i)
                        buffer.Insert(index + oldCount + i, newItems[oldCount + i]);
            }
        }
    }
}
