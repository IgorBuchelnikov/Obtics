using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using Obtics.Values;
using System.Runtime.CompilerServices;

namespace Obtics.Data
{
    class DataRowCollectionAdapter : Obtics.Collections.IVersionedEnumerable<DataRow>, ICollection, INotifyCollectionChanged
    {
        DataTable _Table;
        DataRowCollection _Source;
        int _Index;
        Obtics.Collections.VersionNumber _VersionNumber;

        DataRowCollectionAdapter(DataRowCollection dataRowCollection, DataTable table)
        {
            _Source = dataRowCollection;
            _Table = table;
        }


        #region IVersionedEnumerable<DataRow> Members

        IEnumerator<DataRow> _GetEnumerator()
        {
            foreach (object item in _Source)
                yield return (DataRow)item;
        }

        public Obtics.Collections.IVersionedEnumerator<DataRow> GetEnumerator()
        { return Obtics.Collections.VersionedEnumerator.WithContentVersion(_GetEnumerator(), _VersionNumber); } 

        #endregion

        #region IVersionedEnumerable Members


        public Obtics.Collections.VersionNumber ContentVersion
        { get { return _VersionNumber; } }

        Obtics.Collections.IVersionedEnumerator Obtics.Collections.IVersionedEnumerable.GetEnumerator()
        { return GetEnumerator(); }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        { return GetEnumerator(); }

        #endregion

        #region IEnumerable<DataRow> Members

        IEnumerator<DataRow> IEnumerable<DataRow>.GetEnumerator()
        { return GetEnumerator(); }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        { _Source.CopyTo(array, index); }

        public int Count
        { get { return _Source.Count; } }

        public bool IsSynchronized
        { get { return _Source.IsSynchronized; } }

        public object SyncRoot
        { get { return _Source.SyncRoot; } }

        #endregion

        #region INotifyCollectionChanged Members

        event NotifyCollectionChangedEventHandler _CollectionChanged;

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                if (_CollectionChanged == null)
                {
                    _Table.TableCleared += _Table_TableCleared;
                    _Table.RowChanged += _Table_RowChanged;
                    _Table.RowChanging += _Table_RowChanging;
                    _Table.RowDeleting += _Table_RowChanging;
                    _Table.RowDeleted += _Table_RowChanged;
                }

                _CollectionChanged += value;
            }

            remove
            {
                _CollectionChanged -= value;

                if (_CollectionChanged == null)
                {
                    _Table.TableCleared -= _Table_TableCleared;
                    _Table.RowChanged -= _Table_RowChanged;
                    _Table.RowChanging -= _Table_RowChanging;
                    _Table.RowDeleting -= _Table_RowChanging;
                    _Table.RowDeleted -= _Table_RowChanged;
                }
            }
        }

        void OnCollectionChanged(Obtics.Collections.OrderedNotifyCollectionChangedEventArgs args)
        {
            var cc = _CollectionChanged;
            if (cc != null)
                cc(this, args);
        }

        void _Table_RowChanging(object sender, DataRowChangeEventArgs e)
        {
            var row = e.Row;
            var rowState = row.RowState;

            switch (e.Action)
            {
                case DataRowAction.Delete:
                    _Index = _Source.IndexOf(e.Row);
                    return;
                case DataRowAction.Rollback:
                    if (rowState == DataRowState.Added)
                        goto case DataRowAction.Delete;
                    break;
                case DataRowAction.Commit:
                    if(rowState == DataRowState.Deleted)
                        goto case DataRowAction.Delete;
                    break;
            }

            _Index = rowState == DataRowState.Detached ? -1 : -2 ;
        }

        void _Table_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (_Index != -2)
            {
                var row = e.Row;

                if (row.RowState == DataRowState.Detached)
                    OnCollectionChanged(
                        new Obtics.Collections.OrderedNotifyCollectionChangedEventArgs(
                            _VersionNumber = ContentVersion.Next,
                            NotifyCollectionChangedAction.Remove,
                            row,
                            _Index
                        )
                    );
                else if (_Index == -1)
                    OnCollectionChanged(
                        new Obtics.Collections.OrderedNotifyCollectionChangedEventArgs(
                            _VersionNumber = ContentVersion.Next,
                            NotifyCollectionChangedAction.Add,
                            row,
                            _Source.IndexOf(row)
                        )
                    );
            }
        }

        void _Table_TableCleared(object sender, DataTableClearEventArgs e)
        {
            OnCollectionChanged(
                new Obtics.Collections.OrderedNotifyCollectionChangedEventArgs(
                    _VersionNumber = _VersionNumber.Next,
                    NotifyCollectionChangedAction.Reset
                )
            );
        }

        #endregion

        internal class AdapterClass : Obtics.Collections.ICollectionAdapter
        {
            static ConditionalWeakTable<DataRowCollection, IValueProvider<DataTable>> _TableMap = new ConditionalWeakTable<DataRowCollection, IValueProvider<DataTable>>();

            static ConditionalWeakTable<object, Obtics.Collections.IVersionedEnumerable> _Carrousel = new ConditionalWeakTable<object, Obtics.Collections.IVersionedEnumerable>();


            internal static IValueProvider<DataTable> GetTableProvider(DataRowCollection dataRowCollection)
            { return _TableMap.GetValue(dataRowCollection, _ => ValueProvider.Dynamic<DataTable>()); }

            #region ICollectionAdapter Members

            public Obtics.Collections.IVersionedEnumerable AdaptCollection(object collection)
            {
                if (collection == null)
                    return null;

                return
                    _Carrousel.GetValue(
                        collection,
                        c =>
                        {
                            var dataRowCollection = (DataRowCollection)c;
                            return
                                (Obtics.Collections.IVersionedEnumerable)
                                    GetTableProvider(dataRowCollection)
                                    .Select(
                                        ValueProvider.Static(dataRowCollection),
                                        (tbl, drc) =>
                                            tbl == null ? (IEnumerable<DataRow>)drc.OfType<DataRow>().ToList() :
                                            new DataRowCollectionAdapter(drc, tbl))
                                    .Cascade()
                            ;
                        }
                    )
                ;
            }

            #endregion
        }
    }

    sealed class TypedTableAdapter<TRow> : Obtics.Collections.IVersionedEnumerable<TRow>, INotifyCollectionChanged
        where TRow : DataRow
    {
        TypedTableBase<TRow> _Source;
        int _Index;
        Obtics.Collections.VersionNumber _VersionNumber;

        internal TypedTableAdapter(TypedTableBase<TRow> dataRowCollection)
        {
            _Source = dataRowCollection;
        }


        #region IVersionedEnumerable<TRow> Members

        public Obtics.Collections.IVersionedEnumerator<TRow> GetEnumerator()
        { return Obtics.Collections.VersionedEnumerator.WithContentVersion(_Source.GetEnumerator(), _VersionNumber); }

        #endregion

        #region IVersionedEnumerable Members


        public Obtics.Collections.VersionNumber ContentVersion
        { get { return _VersionNumber; } }

        Obtics.Collections.IVersionedEnumerator Obtics.Collections.IVersionedEnumerable.GetEnumerator()
        { return GetEnumerator(); }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        { return GetEnumerator(); }

        #endregion

        #region IEnumerable<DataRow> Members

        IEnumerator<TRow> IEnumerable<TRow>.GetEnumerator()
        { return GetEnumerator(); }

        #endregion

        #region INotifyCollectionChanged Members

        event NotifyCollectionChangedEventHandler _CollectionChanged;

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                if (_CollectionChanged == null)
                {
                    _Source.TableCleared += _Table_TableCleared;
                    _Source.RowChanged += _Table_RowChanged;
                    _Source.RowChanging += _Table_RowChanging;
                    _Source.RowDeleting += _Table_RowChanging;
                    _Source.RowDeleted += _Table_RowChanged;
                }

                _CollectionChanged += value;
            }

            remove
            {
                _CollectionChanged -= value;

                if (_CollectionChanged == null)
                {
                    _Source.TableCleared -= _Table_TableCleared;
                    _Source.RowChanged -= _Table_RowChanged;
                    _Source.RowChanging -= _Table_RowChanging;
                    _Source.RowDeleting -= _Table_RowChanging;
                    _Source.RowDeleted -= _Table_RowChanged;
                }
            }
        }



        void OnCollectionChanged(Obtics.Collections.OrderedNotifyCollectionChangedEventArgs args)
        {
            var cc = _CollectionChanged;
            if (cc != null)
                cc(this, args);
        }

        void _Table_RowChanging(object sender, DataRowChangeEventArgs e)
        {
            var row = e.Row;
            var rowState = row.RowState;

            switch (e.Action)
            {
                case DataRowAction.Delete:
                    _Index = _Source.Rows.IndexOf(e.Row);
                    return;
                case DataRowAction.Rollback:
                    if (rowState == DataRowState.Added)
                        goto case DataRowAction.Delete;
                    break;
                case DataRowAction.Commit:
                    if (rowState == DataRowState.Deleted)
                        goto case DataRowAction.Delete;
                    break;
            }

            _Index = rowState == DataRowState.Detached ? -1 : -2;
        }

        void _Table_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (_Index != -2)
            {
                var row = e.Row;

                if (row.RowState == DataRowState.Detached)
                    OnCollectionChanged(
                        new Obtics.Collections.OrderedNotifyCollectionChangedEventArgs(
                            _VersionNumber = ContentVersion.Next,
                            NotifyCollectionChangedAction.Remove,
                            row,
                            _Index
                        )
                    );
                else if (_Index == -1)
                    OnCollectionChanged(
                        new Obtics.Collections.OrderedNotifyCollectionChangedEventArgs(
                            _VersionNumber = ContentVersion.Next,
                            NotifyCollectionChangedAction.Add,
                            row,
                            _Source.Rows.IndexOf(row)
                        )
                    );
            }
        }

        void _Table_TableCleared(object sender, DataTableClearEventArgs e)
        {
            OnCollectionChanged(
                new Obtics.Collections.OrderedNotifyCollectionChangedEventArgs(
                    _VersionNumber = _VersionNumber.Next,
                    NotifyCollectionChangedAction.Reset
                )
            );
        }

        #endregion
    }

    public class TypedTableAdapterClass<TRow> : Obtics.Collections.ICollectionAdapter
        where TRow : DataRow
    {
        static ConditionalWeakTable<object, Obtics.Collections.IVersionedEnumerable> _Carrousel = new ConditionalWeakTable<object, Obtics.Collections.IVersionedEnumerable>();
        #region ICollectionAdapter Members

        public Obtics.Collections.IVersionedEnumerable AdaptCollection(object collection)
        { 
            return 
                collection == null ? null : 
                _Carrousel.GetOrAddUnderLock(
                    collection, 
                    c => new TypedTableAdapter<TRow>((TypedTableBase<TRow>)c)
                )
            ; 
        }

        #endregion
    }
}
