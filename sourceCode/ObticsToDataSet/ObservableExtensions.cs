using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using Obtics.Values;
using System.Linq.Expressions;
using OE = Obtics.Collections.ObservableEnumerable;
using System.Runtime.CompilerServices;
using TvdP.Collections;

namespace Obtics.Data
{
    public static class ObservableExtensions
    {
        internal static void TryGetValue<TKey, TValue>(this ConditionalWeakTable<TKey, TValue> table, TKey key, Action<TKey, TValue> action)
            where TKey : class
            where TValue : class
        {
            TValue item;

            if (table.TryGetValue(key, out item))
                action(key, item);
        }

        static ConditionalWeakTable<Object, Object> _LocalLockTable = new ConditionalWeakTable<object, object>();

        internal static TValue GetOrAddUnderLock<TKey, TValue>(this ConditionalWeakTable<TKey, TValue> table, TKey key, Func<TKey, TValue> creator)
            where TKey : class
            where TValue : class
        {
            TValue res;

            if (!table.TryGetValue(key, out res))
                //Lock local lock object bound to key. This allows for better concurrency.
                lock (_LocalLockTable.GetOrCreateValue(key))
                    res = table.GetValue(key, new ConditionalWeakTable<TKey,TValue>.CreateValueCallback(creator));

            return res;
        }
        

        class TableEventDispatcher
        {
            DataTable _Table;

            internal TableEventDispatcher(DataTable table)
            {
                table.ColumnChanged += ColumnChangedHandler;
                table.RowChanged += RowChangedHandler;
                table.RowChanging += RowChangingHandler;
                table.RowDeleted += RowDeletedHandler;
                table.RowDeleting += RowDeletingHandler;
                table.TableCleared += TableClearedHandler;
                table.Columns.CollectionChanged += Columns_CollectionChangedHandler;

                _Table = table;
            }

            void SetVersionChange()
            {
                unchecked
                {
                    if (_VersionChange)
                    {
                        _DataRowCollectionVersionMap.TryGetValue(
                            _Table.Rows,
                            (_, versionProvider) => versionProvider.Value += 1
                        );

                        _VersionChange = false;
                    }
                }
            }

            void TableClearedHandler(object sender, DataTableClearEventArgs e)
            {
                _VersionChange = true;
                SetVersionChange(); 
            }

            void Columns_CollectionChangedHandler(object sender, System.ComponentModel.CollectionChangeEventArgs e)
            {
                foreach (DataColumn column in _Table.Columns)
                {
                    _ColumnOrdinalMap.TryGetValue(
                        column,
                        (c, ordinalProvider) => ordinalProvider.Value = c.Ordinal
                    );
                }
            }

            bool _VersionChange;

            void TestVersionChange(DataRowChangeEventArgs e)
            {
                var row = e.Row;
                var rowState = row.RowState;

                switch (e.Action)
                {
                    case DataRowAction.Delete:
                        _VersionChange = true;
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

                _VersionChange = rowState == DataRowState.Detached ;
            }


            void RowDeletingHandler(object sender, DataRowChangeEventArgs e)
            {
                TestVersionChange(e);
            }

            void RowDeletedHandler(object sender, DataRowChangeEventArgs e)
            {
                _RowRowStateMap.TryGetValue(e.Row, (row, rowRowStateProvider) => rowRowStateProvider.Value = row.RowState);

                SetVersionChange();
            }

            void RowChangingHandler(object sender, DataRowChangeEventArgs e)
            {
                TestVersionChange(e);
            }

            void RowChangedHandler(object sender, DataRowChangeEventArgs e)
            {                
                var row = e.Row;

                _RowRowStateMap.TryGetValue(row, (r, rowRowStateProvider) => rowRowStateProvider.Value = r.RowState);

                SetVersionChange();

                if (e.Action == DataRowAction.Rollback && row.RowState != DataRowState.Detached)
                {
                    for (int i = 0, end = row.ItemArray.Length; i < end; ++i)
                    {
                        _RowColumnMap_TryGetValue(
                            row,
                            i,
                            DataRowVersion.Current,
                            (r,ix,vp) => vp.Value = r[ix, DataRowVersion.Current]
                        );

                        _RowColumnMap_TryGetValue(
                            row,
                            i,
                            DataRowVersion.Proposed,
                            (r, ix, vp) => vp.Value = r[ix, DataRowVersion.Proposed]
                        );
                    }
                }
                else if (e.Action == DataRowAction.Commit && row.RowState != DataRowState.Detached)
                {
                    for (int i = 0, end = e.Row.ItemArray.Length; i < end; ++i)
                    {
                        _RowColumnMap_TryGetValue(
                            row,
                            i,
                            DataRowVersion.Original,
                            (r, ix, vp) => vp.Value = r[ix, DataRowVersion.Original]
                        );
                    }
                }
            }

            void ColumnChangedHandler(object sender, DataColumnChangeEventArgs e)
            {
                _RowColumnMap_TryGetValue(
                    e.Row,
                    e.Column.Ordinal,
                    DataRowVersion.Current,
                    (r, ix, vp) => vp.Value = e.ProposedValue
                );

                _RowColumnMap_TryGetValue(
                    e.Row,
                    e.Column.Ordinal,
                    DataRowVersion.Proposed,
                    (r, ix, vp) => vp.Value = e.ProposedValue
                );

                IValueProvider<object[]> iap;

                if (_RowItemArrayMap.TryGetValue(e.Row, out iap))
                {
                    var ia = e.Row.ItemArray;

                    if (!System.Linq.Enumerable.SequenceEqual(iap.Value, ia))
                        iap.Value = (object[])ia.Clone();
                }
            }
        }

        static ConditionalWeakTable<DataTable, TableEventDispatcher> _TableDispatchersMap = new ConditionalWeakTable<DataTable, TableEventDispatcher>();

        private static void EnsureTableDispatcher(DataTable table)
        { _TableDispatchersMap.GetOrAddUnderLock(table, t => new TableEventDispatcher(t)); }









        //DataTable

        public static DataRowCollection Rows(DataTable table)
        {
            EnsureTableDispatcher(table);
            var tp = DataRowCollectionAdapter.AdapterClass.GetTableProvider(table.Rows);
            tp.Value = table;
            return table.Rows;
        }

        public static EnumerableRowCollection<DataRow> AsEnumerable(DataTable source)
        {
            var pRes = DataTableExtensions.AsEnumerable(source);

            EnumerableRowCollectionAdapter._Carrousel.GetValue(pRes, r => (Obtics.Collections.IVersionedEnumerable)OE.Select(OE.OfType<object>(source.Rows), obj => (DataRow)obj));

            return pRes;
        }

        //TypedTableBase

        #region Select extension
        public static EnumerableRowCollection<S> Select<TRow, S>(TypedTableBase<TRow> source, Func<TRow, IValueProvider<S>> selector) where TRow : DataRow
        {
            var pRes = TypedTableBaseExtensions.Select(source, r => selector(r).Value);

            EnumerableRowCollectionAdapter._Carrousel.GetValue(pRes, r => (Obtics.Collections.IVersionedEnumerable<S>)OE.Select(source, selector));

            return pRes;
        }

        public static EnumerableRowCollection<S> Select<TRow, S>(TypedTableBase<TRow> source, Func<TRow, S> selector) where TRow : DataRow
        {
            var pRes = TypedTableBaseExtensions.Select(source, selector);

            EnumerableRowCollectionAdapter._Carrousel.GetValue(pRes, r => (Obtics.Collections.IVersionedEnumerable<S>)OE.Select(source, selector));

            return pRes;
        }
        #endregion

        #region Where extension
        public static EnumerableRowCollection<TRow> Where<TRow>(TypedTableBase<TRow> source, Func<TRow, bool> selector) where TRow : DataRow
        {
            var pRes = TypedTableBaseExtensions.Where(source, selector);

            EnumerableRowCollectionAdapter._Carrousel.GetValue(pRes, r => (Obtics.Collections.IVersionedEnumerable<TRow>)OE.Where(source, selector));

            return pRes;
        }

        public static EnumerableRowCollection<TRow> Where<TRow>(TypedTableBase<TRow> source, Func<TRow, IValueProvider<bool>> selector) where TRow : DataRow
        {
            var pRes = TypedTableBaseExtensions.Where(source, r => selector(r).Value);

            EnumerableRowCollectionAdapter._Carrousel.GetValue(pRes, r => (Obtics.Collections.IVersionedEnumerable<TRow>)OE.Where(source, selector));

            return pRes;
        }
        #endregion

        #region AsEnumerable extension
        public static EnumerableRowCollection<TRow> AsEnumerable<TRow>(TypedTableBase<TRow> source) where TRow : DataRow
        {
            var pRes = TypedTableBaseExtensions.AsEnumerable(source);

            EnumerableRowCollectionAdapter._Carrousel.GetValue(pRes, r => (Obtics.Collections.IVersionedEnumerable<TRow>)OE.OfType<TRow>(source));

            return pRes;
        }
        #endregion

        #region OrderBy extension
        public static OrderedEnumerableRowCollection<TRow> OrderBy<TRow, TKey>(TypedTableBase<TRow> source, Func<TRow, TKey> keySelector) where TRow : DataRow
        { return OrderBy(source, keySelector, Comparer<TKey>.Default); }

        public static OrderedEnumerableRowCollection<TRow> OrderBy<TRow, TKey>(TypedTableBase<TRow> source, Func<TRow, TKey> keySelector, IComparer<TKey> comparer) where TRow : DataRow
        {
            var pRes = TypedTableBaseExtensions.OrderBy(source, keySelector, comparer);

            EnumerableRowCollectionAdapter._Carrousel.GetValue(pRes, r => (Obtics.Collections.IVersionedEnumerable<TRow>)OE.OrderBy(source, keySelector, comparer));

            return pRes;
        }

        public static OrderedEnumerableRowCollection<TRow> OrderBy<TRow, TKey>(TypedTableBase<TRow> source, Func<TRow, IValueProvider<TKey>> keySelector) where TRow : DataRow
        { return OrderBy(source, keySelector, Comparer<TKey>.Default); }

        public static OrderedEnumerableRowCollection<TRow> OrderBy<TRow, TKey>(TypedTableBase<TRow> source, Func<TRow, IValueProvider<TKey>> keySelector, IComparer<TKey> comparer) where TRow : DataRow
        {
            var pRes = TypedTableBaseExtensions.OrderBy(source, r => keySelector(r).Value, comparer);

            EnumerableRowCollectionAdapter._Carrousel.GetValue(pRes, r => (Obtics.Collections.IVersionedEnumerable<TRow>)OE.OrderBy(source, keySelector, comparer));

            return pRes;
        }
        #endregion

        #region OrderByDescending extension
        public static OrderedEnumerableRowCollection<TRow> OrderByDescending<TRow, TKey>(TypedTableBase<TRow> source, Func<TRow, TKey> keySelector) where TRow : DataRow
        { return OrderByDescending(source, keySelector, Comparer<TKey>.Default); }

        public static OrderedEnumerableRowCollection<TRow> OrderByDescending<TRow, TKey>(TypedTableBase<TRow> source, Func<TRow, TKey> keySelector, IComparer<TKey> comparer) where TRow : DataRow
        {
            var pRes = TypedTableBaseExtensions.OrderByDescending(source, keySelector, comparer);

            EnumerableRowCollectionAdapter._Carrousel.GetValue(pRes, r => (Obtics.Collections.IVersionedEnumerable<TRow>)OE.OrderByDescending(source, keySelector, comparer));

            return pRes;
        }

        public static OrderedEnumerableRowCollection<TRow> OrderByDescending<TRow, TKey>(TypedTableBase<TRow> source, Func<TRow, IValueProvider<TKey>> keySelector) where TRow : DataRow
        { return OrderByDescending(source, keySelector, Comparer<TKey>.Default); }

        public static OrderedEnumerableRowCollection<TRow> OrderByDescending<TRow, TKey>(TypedTableBase<TRow> source, Func<TRow, IValueProvider<TKey>> keySelector, IComparer<TKey> comparer) where TRow : DataRow
        {
            var pRes = TypedTableBaseExtensions.OrderByDescending(source, r => keySelector(r).Value, comparer);

            EnumerableRowCollectionAdapter._Carrousel.GetValue(pRes, r => (Obtics.Collections.IVersionedEnumerable<TRow>)OE.OrderByDescending(source, keySelector, comparer));

            return pRes;
        }
        #endregion

        //EnumerableRowCollection

        #region Select extension
        public static EnumerableRowCollection<S> Select<TRow, S>(EnumerableRowCollection<TRow> source, Func<TRow, S> selector) where TRow : DataRow
        {
            var pRes = EnumerableRowCollectionExtensions.Select(source, selector);

            EnumerableRowCollectionAdapter._Carrousel.GetValue(pRes, r => (Obtics.Collections.IVersionedEnumerable<S>)OE.Select(source, selector));

            return pRes;
        }

        public static EnumerableRowCollection<S> Select<TRow, S>(EnumerableRowCollection<TRow> source, Func<TRow, IValueProvider<S>> selector) where TRow : DataRow
        {
            var pRes = EnumerableRowCollectionExtensions.Select(source, r => selector(r).Value);

            EnumerableRowCollectionAdapter._Carrousel.GetValue(pRes, r => (Obtics.Collections.IVersionedEnumerable<S>)OE.Select(source, selector));

            return pRes;
        }
        #endregion

        #region Cast extension
        public static EnumerableRowCollection<S> Cast<S>(EnumerableRowCollection source)
        {
            var pRes = EnumerableRowCollectionExtensions.Cast<S>(source);

            EnumerableRowCollectionAdapter._Carrousel.GetValue(pRes, r => (Obtics.Collections.IVersionedEnumerable<S>)OE.Cast<S>(source));

            return pRes;
        }
        #endregion

        #region Where extension
        public static EnumerableRowCollection<TRow> Where<TRow>(EnumerableRowCollection<TRow> source, Func<TRow, bool> selector) where TRow : DataRow
        {
            var pRes = EnumerableRowCollectionExtensions.Where(source, selector);

            EnumerableRowCollectionAdapter._Carrousel.GetValue(pRes, r => (Obtics.Collections.IVersionedEnumerable<TRow>)OE.Where(source, selector));

            return pRes;
        }

        public static EnumerableRowCollection<TRow> Where<TRow>(EnumerableRowCollection<TRow> source, Func<TRow, IValueProvider<bool>> selector) where TRow : DataRow
        {
            var pRes = EnumerableRowCollectionExtensions.Where(source, r => selector(r).Value);

            EnumerableRowCollectionAdapter._Carrousel.GetValue(pRes, r => (Obtics.Collections.IVersionedEnumerable<TRow>)OE.Where(source, selector));

            return pRes;
        }
        #endregion

        #region OrderBy extension
        public static OrderedEnumerableRowCollection<TRow> OrderBy<TRow, TKey>(EnumerableRowCollection<TRow> source, Func<TRow, TKey> keySelector) where TRow : DataRow
        { return OrderBy(source, keySelector, Comparer<TKey>.Default); }

        public static OrderedEnumerableRowCollection<TRow> OrderBy<TRow, TKey>(EnumerableRowCollection<TRow> source, Func<TRow, TKey> keySelector, IComparer<TKey> comparer) where TRow : DataRow
        {
            var pRes = EnumerableRowCollectionExtensions.OrderBy(source, keySelector, comparer);

            EnumerableRowCollectionAdapter._Carrousel.GetValue(pRes, r => (Obtics.Collections.IVersionedEnumerable<TRow>)OE.OrderBy(source, keySelector, comparer));

            return pRes;
        }

        public static OrderedEnumerableRowCollection<TRow> OrderBy<TRow, TKey>(EnumerableRowCollection<TRow> source, Func<TRow, IValueProvider<TKey>> keySelector) where TRow : DataRow
        { return OrderBy(source, keySelector, Comparer<TKey>.Default); }

        public static OrderedEnumerableRowCollection<TRow> OrderBy<TRow, TKey>(EnumerableRowCollection<TRow> source, Func<TRow, IValueProvider<TKey>> keySelector, IComparer<TKey> comparer) where TRow : DataRow
        {
            var pRes = EnumerableRowCollectionExtensions.OrderBy(source, r => keySelector(r).Value, comparer);

            EnumerableRowCollectionAdapter._Carrousel.GetValue(pRes, r => (Obtics.Collections.IVersionedEnumerable<TRow>)OE.OrderBy(source, keySelector, comparer));

            return pRes;
        }
        #endregion

        #region OrderByDescending extension
        public static OrderedEnumerableRowCollection<TRow> OrderByDescending<TRow, TKey>(EnumerableRowCollection<TRow> source, Func<TRow, TKey> keySelector) where TRow : DataRow
        { return OrderByDescending(source, keySelector, Comparer<TKey>.Default); }

        public static OrderedEnumerableRowCollection<TRow> OrderByDescending<TRow, TKey>(EnumerableRowCollection<TRow> source, Func<TRow, TKey> keySelector, IComparer<TKey> comparer) where TRow : DataRow
        {
            var pRes = EnumerableRowCollectionExtensions.OrderByDescending(source, keySelector, comparer);

            EnumerableRowCollectionAdapter._Carrousel.GetValue(pRes, r => (Obtics.Collections.IVersionedEnumerable<TRow>)OE.OrderByDescending(source, keySelector, comparer));

            return pRes;
        }
        public static OrderedEnumerableRowCollection<TRow> OrderByDescending<TRow, TKey>(EnumerableRowCollection<TRow> source, Func<TRow, IValueProvider<TKey>> keySelector) where TRow : DataRow
        { return OrderByDescending(source, keySelector, Comparer<TKey>.Default); }

        public static OrderedEnumerableRowCollection<TRow> OrderByDescending<TRow, TKey>(EnumerableRowCollection<TRow> source, Func<TRow, IValueProvider<TKey>> keySelector, IComparer<TKey> comparer) where TRow : DataRow
        {
            var pRes = EnumerableRowCollectionExtensions.OrderByDescending(source, r => keySelector(r).Value, comparer);

            EnumerableRowCollectionAdapter._Carrousel.GetValue(pRes, r => (Obtics.Collections.IVersionedEnumerable<TRow>)OE.OrderByDescending(source, keySelector, comparer));

            return pRes;
        }
        #endregion

        #region ThenBy extension
        public static OrderedEnumerableRowCollection<TRow> ThenBy<TRow, TKey>(OrderedEnumerableRowCollection<TRow> source, Func<TRow, TKey> keySelector) where TRow : DataRow
        { return ThenBy(source, keySelector, Comparer<TKey>.Default); }

        public static OrderedEnumerableRowCollection<TRow> ThenBy<TRow, TKey>(OrderedEnumerableRowCollection<TRow> source, Func<TRow, TKey> keySelector, IComparer<TKey> comparer) where TRow : DataRow
        {
            var pRes = EnumerableRowCollectionExtensions.ThenBy(source, keySelector, comparer);

            Collections.IVersionedEnumerable carrouselItem;

            if (EnumerableRowCollectionAdapter._Carrousel.TryGetValue(source, out carrouselItem))
            {
                var observableSource = carrouselItem as IOrderedEnumerable<TRow>;
                EnumerableRowCollectionAdapter._Carrousel.GetValue(pRes, r => (Obtics.Collections.IVersionedEnumerable<TRow>)OE.ThenBy(observableSource, keySelector, comparer));
            }

            return pRes;
        }
        public static OrderedEnumerableRowCollection<TRow> ThenBy<TRow, TKey>(OrderedEnumerableRowCollection<TRow> source, Func<TRow, IValueProvider<TKey>> keySelector) where TRow : DataRow
        { return ThenBy(source, keySelector, Comparer<TKey>.Default); }

        public static OrderedEnumerableRowCollection<TRow> ThenBy<TRow, TKey>(OrderedEnumerableRowCollection<TRow> source, Func<TRow, IValueProvider<TKey>> keySelector, IComparer<TKey> comparer) where TRow : DataRow
        {
            var pRes = EnumerableRowCollectionExtensions.ThenBy(source, r => keySelector(r).Value, comparer);

            Collections.IVersionedEnumerable item;

            if (EnumerableRowCollectionAdapter._Carrousel.TryGetValue(source, out item))
            {
                var observableSource = item as IOrderedEnumerable<TRow>;

                if (observableSource != null)
                    EnumerableRowCollectionAdapter._Carrousel.GetValue(pRes, r => (Obtics.Collections.IVersionedEnumerable<TRow>)OE.ThenBy(observableSource, keySelector, comparer));
            }

            return pRes;
        }
        #endregion

        #region ThenByDescending extension
        public static OrderedEnumerableRowCollection<TRow> ThenByDescending<TRow, TKey>(OrderedEnumerableRowCollection<TRow> source, Func<TRow, TKey> keySelector) where TRow : DataRow
        { return ThenByDescending(source, keySelector, Comparer<TKey>.Default); }

        public static OrderedEnumerableRowCollection<TRow> ThenByDescending<TRow, TKey>(OrderedEnumerableRowCollection<TRow> source, Func<TRow, TKey> keySelector, IComparer<TKey> comparer) where TRow : DataRow
        {
            var pRes = EnumerableRowCollectionExtensions.ThenByDescending(source, keySelector, comparer);

            Collections.IVersionedEnumerable item;

            if (EnumerableRowCollectionAdapter._Carrousel.TryGetValue(source, out item))
            {
                var observableSource = item as IOrderedEnumerable<TRow>;

                if (observableSource != null)
                    EnumerableRowCollectionAdapter._Carrousel.GetValue(pRes, r => (Obtics.Collections.IVersionedEnumerable<TRow>)OE.ThenByDescending(observableSource, keySelector, comparer));
            }

            return pRes;
        }

        public static OrderedEnumerableRowCollection<TRow> ThenByDescending<TRow, TKey>(OrderedEnumerableRowCollection<TRow> source, Func<TRow, IValueProvider<TKey>> keySelector) where TRow : DataRow
        { return ThenByDescending(source, keySelector, Comparer<TKey>.Default); }

        public static OrderedEnumerableRowCollection<TRow> ThenByDescending<TRow, TKey>(OrderedEnumerableRowCollection<TRow> source, Func<TRow, IValueProvider<TKey>> keySelector, IComparer<TKey> comparer) where TRow : DataRow
        {
            var pRes = EnumerableRowCollectionExtensions.ThenByDescending(source, r => keySelector(r).Value, comparer);

            Collections.IVersionedEnumerable item;

            if (EnumerableRowCollectionAdapter._Carrousel.TryGetValue(source, out item))
            {
                var observableSource = item as IOrderedEnumerable<TRow>;

                if (observableSource != null)
                    EnumerableRowCollectionAdapter._Carrousel.GetValue(pRes, r => (Obtics.Collections.IVersionedEnumerable<TRow>)OE.ThenByDescending(observableSource, keySelector, comparer));
            }

            return pRes;
        }
        #endregion

        //DataRow >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

        #region RowState property

        static ConditionalWeakTable<DataRow, IValueProvider<DataRowState>> _RowRowStateMap = new ConditionalWeakTable<DataRow, IValueProvider<DataRowState>>();

        static IValueProvider<DataRowState> GetRowRowStateProvider(DataRow row)
        {
            return
                _RowRowStateMap.GetValue(
                    row,
                    r =>
                    {
                        EnsureTableDispatcher(r.Table);
                        return ValueProvider.Dynamic(r.RowState);
                    }
                )
            ;
        }

        public static IValueProvider<DataRowState> RowState(DataRow row)
        { return GetRowRowStateProvider(row).ReadOnly(); }

        #endregion

        #region Item property       

        //class RowData
        //{
        //    public WeakReference[] _Original;
        //    public WeakReference[] _Current;
        //    public WeakReference[] _Proposed;
        //    WeakReference _RowState;
        //}

        //static ConcurrentWeakDictionaryStrongValues<DataRow, RowData> _RowRowDataMap = new ConcurrentWeakDictionaryStrongValues<DataRow, RowData>();

        //static IValueProvider<object> FindRowItem(DataRow row, int columnIndex, DataRowVersion rowVersion)
        //{
        //    var rowData = _RowRowDataMap[row];

        //    if (rowData != null)
        //    {
        //        var versions =
        //            rowVersion == DataRowVersion.Current ? rowData._Current :
        //            rowVersion == DataRowVersion.Original ? rowData._Original :
        //            rowData._Proposed;

        //        if (versions != null && columnIndex < versions.Length)
        //        {
        //            var wr = versions[columnIndex];

        //            if (wr != null)
        //                return (IValueProvider<object>)wr.Target;
        //        }
        //    }

        //    return null;
        //}

        //static IValueProvider<object> GetRowItem(DataRow row, int columnIndex, DataRowVersion rowVersion)
        //{
        //    var rowData = _RowRowDataMap[row] ?? _RowRowDataMap.GetOldest(row, new RowData());

        //    WeakReference[] versions;

        //    switch (rowVersion)
        //    {
        //        case DataRowVersion.Current:
        //            versions = rowData._Current ?? (rowData._Current = new WeakReference[row.ItemArray.Length]);

        //            if(versions == null || versions.Length <= columnIndex)
        //            {
        //                versions = new WeakReference[row.ItemArray.Length];
        //                rowData._Current.CopyTo(versions,0);
        //            }

        //            break;
        //        case DataRowVersion.Original:
        //            versions = rowData._Original ?? (rowData._Original = new WeakReference[row.ItemArray.Length]);
        //            break;
        //        case DataRowVersion.Proposed:
        //            versions = rowData._Proposed ?? (rowData._Proposed = new WeakReference[row.ItemArray.Length]);
        //            break;
        //        default:
        //            throw new ArgumentException("Unexpected rewVersion");
        //    }
   
        //    var versions =
        //        rowVersion == DataRowVersion.Current ? rowData._Current :
        //        rowVersion == DataRowVersion.Original ? rowData._Original :
        //        rowData._Proposed;


        //    if (versions != null && columnIndex < versions.Length)
        //    {
        //        var wr = versions[columnIndex];

        //        if (wr != null)
        //            return (IValueProvider<object>)wr.Target;
        //    }

        //    return null;
        //}

        static WeakDictionary<DataRow,Tuple<int,DataRowVersion>,IValueProvider<object>> _RowColumnMap = new WeakDictionary<DataRow,Tuple<int,DataRowVersion>,IValueProvider<object>>();

        static IValueProvider<object> _RowColumnMap_Get(DataRow row, int columnIndex, DataRowVersion rowVersion)
        {
            return
                _RowColumnMap.GetOrAdd(
                    row,
                    Tuple.Create(columnIndex, rowVersion),
                    (r, t) =>
                    {
                        EnsureTableDispatcher(r.Table);
                        return ValueProvider.Dynamic<object>(r[columnIndex]);
                    }
                )
            ;
        }

        static void _RowColumnMap_TryGetValue(DataRow row, int columnIndex, DataRowVersion rowVersion, Action<DataRow,int,IValueProvider<object>> action)
        {
            IValueProvider<object> item;

            if (_RowColumnMap.TryGetValue(row, Tuple.Create(columnIndex, rowVersion), out item))
                action(row,columnIndex,item);
        }

        public static IValueProvider<object> Item(DataRow row, int columnIndex, DataRowVersion dataRowVersion)
        { return _RowColumnMap_Get(row, columnIndex, dataRowVersion).ReadOnly(); }

        public static IValueProvider<object> Item(DataRow row, DataColumn column, DataRowVersion dataRowVersion)
        { return Ordinal(column).Select(ord => _RowColumnMap_Get(row, ord, dataRowVersion)); }

        public static IValueProvider<object> Item(DataRow row, string columnName, DataRowVersion dataRowVersion)
        { return Item(row, row.Table.Columns[columnName], dataRowVersion); }

        public static IValueProvider<object> Item(DataRow row, int columnIndex)
        { return Item(row, columnIndex, DataRowVersion.Current); }

        public static IValueProvider<object> Item(DataRow row, DataColumn column)
        { return Item(row, column, DataRowVersion.Current); }

        public static IValueProvider<object> Item(DataRow row, string columnName)
        { return Item(row, columnName, DataRowVersion.Current); }

        #endregion

        #region ItemArray property

        static ConditionalWeakTable<DataRow, IValueProvider<object[]>> _RowItemArrayMap = new ConditionalWeakTable<DataRow, IValueProvider<object[]>>();

        static IValueProvider<object[]> GetRowItemArrayProvider(DataRow row)
        {
            return
                _RowItemArrayMap.GetValue(
                    row,
                    r =>
                    {
                        EnsureTableDispatcher(r.Table);
                        return ValueProvider.Dynamic((object[])r.ItemArray.Clone());
                    }
                )
            ;
        }

        public static IValueProvider<object[]> ItemArray(DataRow row)
        { return GetRowItemArrayProvider(row).ReadOnly(); }

        #endregion

        #region Field extension method

        public static IValueProvider<TType> Field<TType>(DataRow row, int columnIndex, DataRowVersion dataRowVersion)
        { return Item(row, columnIndex, dataRowVersion).Cast<TType>(); }

        public static IValueProvider<TType> Field<TType>(DataRow row, DataColumn column, DataRowVersion dataRowVersion)
        { return Item(row, column, dataRowVersion).Cast<TType>(); }

        public static IValueProvider<TType> Field<TType>(DataRow row, string columnName, DataRowVersion dataRowVersion)
        { return Item(row, columnName, dataRowVersion).Cast<TType>(); }

        public static IValueProvider<TType> Field<TType>(DataRow row, int columnIndex)
        { return Field<TType>(row, columnIndex, DataRowVersion.Current); }

        public static IValueProvider<TType> Field<TType>(DataRow row, DataColumn column)
        { return Field<TType>(row, column, DataRowVersion.Current); }

        public static IValueProvider<TType> Field<TType>(DataRow row, string columnName)
        { return Field<TType>(row, columnName, DataRowVersion.Current); }

        #endregion

        //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

        //DataRowCollection >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

        static IEnumerable<object> DataRowKeyValues(DataRow row)
        { return OE.Select(row.Table.PrimaryKey, column => Item(row, column.Ordinal)); }

        static ConditionalWeakTable<DataRowCollection, IValueProvider<long>> _DataRowCollectionVersionMap = new ConditionalWeakTable<DataRowCollection, IValueProvider<long>>();

        static IValueProvider<long> GetDataRowCollectionVersionProvider(DataRowCollection dataRowCollection)
        {
            return
                _DataRowCollectionVersionMap.GetValue(
                    dataRowCollection,
                    drc =>
                    {
                        //EnsureTableDispatcher(table); can't get to table from here
                        return ValueProvider.Dynamic(0L);
                    }
                )
            ;
        }

        public static IValueProvider<int> Count(DataRowCollection dataRowCollection)
        { return OE.Aggregate( dataRowCollection, () => dataRowCollection.Count ); }

        public static IValueProvider<DataRow> Item(DataRowCollection dataRowCollection, int index)
        { return OE.Aggregate( dataRowCollection, () => dataRowCollection[index] ); }


        sealed class ArrayEqualityComparer<TElement> : IEqualityComparer<TElement[]>
        {
            #region IEqualityComparer<TElement[]> Members

            public bool Equals(TElement[] x, TElement[] y)
            {
                return
                    x == null ? y == null :
                    y == null ? false :
                    x.SequenceEqual( y ) ;
            }

            public int GetHashCode(TElement[] obj)
            { return obj.Aggregate(0, ( agr, elt ) => agr + ( elt == null ? 39482093 : elt.GetHashCode() ) ); }

            #endregion

            public static readonly ArrayEqualityComparer<TElement> Instance = new ArrayEqualityComparer<TElement>();
        }


        public static IValueProvider<bool> Contains(DataRowCollection dataRowCollection, IValueProvider<object> key)
        { return _ContainsF(dataRowCollection, key.AsEnumerable()); }
        
        public static IValueProvider<bool> Contains(DataRowCollection dataRowCollection, object[] keys)
        { return _ContainsF(dataRowCollection, OE.Static(keys)); }

        static Func<DataRowCollection, IEnumerable<object>, IValueProvider<bool>> _ContainsF =
            ExpressionObserver.Compile(
                (DataRowCollection dataRowCollection, IEnumerable<object> keys) =>
                    ((IDictionary<Object[],DataRow>)dataRowCollection.OfType<object>().Select(drObj => (DataRow)drObj).ToDictionary(dr => DataRowKeyValues(dr).ToArray(), ArrayEqualityComparer<object>.Instance)).ContainsKey(keys.ToArray())
            );        
                


        public static IValueProvider<DataRow> Find(DataRowCollection dataRowCollection, IValueProvider<object> key)
        { return _FindF(dataRowCollection, key.AsEnumerable()); }

        public static IValueProvider<DataRow> Find(DataRowCollection dataRowCollection, object[] keys)
        { return _FindF(dataRowCollection, OE.Static(keys)); }

        static Func<DataRowCollection, IEnumerable<object>, IValueProvider<DataRow>> _FindF =
            ExpressionObserver.Compile(
                (DataRowCollection dataRowCollection, IEnumerable<object> keys) =>
                    ((IDictionary<Object[], DataRow>)dataRowCollection.OfType<object>().Select(drObj => (DataRow)drObj).ToDictionary(dr => DataRowKeyValues(dr).ToArray(), ArrayEqualityComparer<object>.Instance))[keys.ToArray()] 
            );        


        public static IValueProvider<int> IndexOf(DataRowCollection dataRowCollection, DataRow item)
        { return OE.Aggregate(dataRowCollection, () => dataRowCollection.IndexOf(item)); }


        // <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<


        //DataColumn

        static ConditionalWeakTable<DataColumn, IValueProvider<int>> _ColumnOrdinalMap = new ConditionalWeakTable<DataColumn, IValueProvider<int>>();

        static IValueProvider<int> GetColumnOrdinalProvider(DataColumn column)
        {
            return
                _ColumnOrdinalMap.GetValue(
                    column,
                    c =>
                    {
                        EnsureTableDispatcher(c.Table);
                        return ValueProvider.Dynamic(c.Ordinal);
                    }
                )
            ;
        }

        public static IValueProvider<int> Ordinal(DataColumn column)
        { return GetColumnOrdinalProvider(column).ReadOnly(); }
    }
}
