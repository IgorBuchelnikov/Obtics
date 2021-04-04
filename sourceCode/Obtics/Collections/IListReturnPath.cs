using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obtics.Values;

namespace Obtics.Collections
{
    public interface IListReturnPath<T> 
    {
        // Summary:
        //     Gets a value indicating whether the System.Collections.Generic.ICollection<T>
        //     is read-only.
        //
        // Returns:
        //     true if the System.Collections.Generic.ICollection<T> is read-only; otherwise,
        //     false.
        IValueProvider<bool> IsReadOnly(IList<T> list);

        // Summary:
        //     Adds an item to the System.Collections.Generic.ICollection<T>.
        //
        // Parameters:
        //   item:
        //     The object to add to the System.Collections.Generic.ICollection<T>.
        //
        // Exceptions:
        //   System.NotSupportedException:
        //     The System.Collections.Generic.ICollection<T> is read-only.
        void Add(IList<T> list, T item);
        //
        // Summary:
        //     Removes all items from the System.Collections.Generic.ICollection<T>.
        //
        // Exceptions:
        //   System.NotSupportedException:
        //     The System.Collections.Generic.ICollection<T> is read-only.
        void Clear(IList<T> list);

        //
        // Summary:
        //     Removes the first occurrence of a specific object from the System.Collections.Generic.ICollection<T>.
        //
        // Parameters:
        //   item:
        //     The object to remove from the System.Collections.Generic.ICollection<T>.
        //
        // Returns:
        //     true if item was successfully removed from the System.Collections.Generic.ICollection<T>;
        //     otherwise, false. This method also returns false if item is not found in
        //     the original System.Collections.Generic.ICollection<T>.
        //
        // Exceptions:
        //   System.NotSupportedException:
        //     The System.Collections.Generic.ICollection<T> is read-only.
        bool Remove(IList<T> list, T item);

        bool IsSynchronized(IList<T> list);

        object SyncRoot(IList<T> list);

        //
        // Summary:
        //     Inserts an item to the System.Collections.Generic.IList<T> at the specified
        //     index.
        //
        // Parameters:
        //   index:
        //     The zero-based index at which item should be inserted.
        //
        //   item:
        //     The object to insert into the System.Collections.Generic.IList<T>.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     index is not a valid index in the System.Collections.Generic.IList<T>.
        //
        //   System.NotSupportedException:
        //     The System.Collections.Generic.IList<T> is read-only.
        void Insert(IList<T> list, int index, T item);
        //
        // Summary:
        //     Removes the System.Collections.Generic.IList<T> item at the specified index.
        //
        // Parameters:
        //   index:
        //     The zero-based index of the item to remove.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     index is not a valid index in the System.Collections.Generic.IList<T>.
        //
        //   System.NotSupportedException:
        //     The System.Collections.Generic.IList<T> is read-only.
        void RemoveAt(IList<T> list, int index);

        void ReplaceAt(IList<T> list, int index, T item);
    }
}
