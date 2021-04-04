using System;
using System.Collections.Generic;
using SL = System.Linq;
using System.Text;
using System.Threading;
using Obtics.Values;

namespace Obtics.Collections
{
    /// <summary>
    /// struct to more easily manipulate a group of delegates
    /// </summary>
    /// <typeparam name="TBase"></typeparam>
    /// <typeparam name="TElement"></typeparam>
    internal struct ListReturnPathFuncStruct<TBase, TElement>
    {
        public Func<TBase,IValueProvider<bool>> _IsReadOnly;
        public Action<TBase,TElement> _Add;
        public Action<TBase> _Clear;
        public Func<TBase,TElement, bool> _Remove;
        public Func<TBase,bool> _IsSynchronized;
        public Func<TBase,object> _SyncRoot;
        public Action<TBase,int, TElement> _Insert;
        public Action<TBase, int> _RemoveAt;
        public Action<TBase,int, TElement> _ReplaceAt;

        public override int GetHashCode()
        { 
            return
                Hasher.CreateFromRef(this.GetType())
                    .AddRef(_IsReadOnly)
                    .AddRef(_Add)
                    .AddRef(_Clear)
                    .AddRef(_Remove)
                    .AddRef(_IsSynchronized)
                    .AddRef(_SyncRoot)
                    .AddRef(_Insert)
                    .AddRef(_RemoveAt)
                    .AddRef(_ReplaceAt);
        }

        #region IEquatable<ListReturnPath<TElement>> Members

        public bool Equals(ref ListReturnPathFuncStruct<TBase, TElement> other)
        {
            return
                ObticsEqualityComparer<Func<TBase,IValueProvider<bool>>>.Default.Equals(this._IsReadOnly, other._IsReadOnly)
                && ObticsEqualityComparer<Action<TBase, TElement>>.Default.Equals(this._Add, other._Add)
                && ObticsEqualityComparer<Action<TBase>>.Default.Equals(this._Clear, other._Clear)
                && ObticsEqualityComparer<Func<TBase,TElement, bool>>.Default.Equals(this._Remove, other._Remove)
                && ObticsEqualityComparer<Func<TBase,bool>>.Default.Equals(this._IsSynchronized, other._IsSynchronized)
                && ObticsEqualityComparer<Func<TBase,object>>.Default.Equals(this._SyncRoot, other._SyncRoot)
                && ObticsEqualityComparer<Action<TBase, int, TElement>>.Default.Equals(this._Insert, other._Insert)
                && ObticsEqualityComparer<Action<TBase, int>>.Default.Equals(this._RemoveAt, other._RemoveAt)
                && ObticsEqualityComparer<Action<TBase, int, TElement>>.Default.Equals(this._ReplaceAt, other._ReplaceAt);
        }

        #endregion
    }

    /// <summary>
    /// Base implementation for IListReturnPath. Public for technical reasons. Not intended to be used from your code.
    /// </summary>
    /// <typeparam name="TBase">Type of the information to be passed as extra parameter to delegates</typeparam>
    /// <typeparam name="TElement">Type of the elements in the extended list.</typeparam>
    public class ListReturnPathBase<TBase,TElement>
    {
        ListReturnPathFuncStruct<TBase, TElement> _Funcs;

        internal ListReturnPathBase(ListReturnPathFuncStruct<TBase, TElement> funcs)
        { _Funcs = funcs; }

        #region IListReturnPath<TElement> Members

        internal IValueProvider<bool> IsReadOnly(IList<TElement> list, TBase b)
        { return _Funcs._IsReadOnly != null ? _Funcs._IsReadOnly(b) : null; }

        internal void Add(IList<TElement> list, TBase b, TElement item)
        {
            if (_Funcs._Add != null)
                _Funcs._Add(b,item);
            else if (_Funcs._Insert != null)
            {
                var syncRoot = SyncRoot(list, b);

                if (syncRoot != null)
                    Monitor.Enter(syncRoot);
                try
                {
                    _Funcs._Insert(b,list.Count, item);
                }
                finally
                {
                    if (syncRoot != null)
                        Monitor.Exit(syncRoot);
                }
            }
            else
                throw new NotSupportedException("Add");
        }

        internal void Clear(IList<TElement> list, TBase b)
        {
            if (_Funcs._Clear != null)
                _Funcs._Clear(b);
            else if (_Funcs._Remove != null)
            {
                var syncRoot = SyncRoot(list, b);

                if (syncRoot != null)
                    Monitor.Enter(syncRoot);
                try
                {
                    foreach (var item in SL.Enumerable.ToArray(list))
                        _Funcs._Remove(b, item);
                }
                finally
                {
                    if (syncRoot != null)
                        Monitor.Exit(syncRoot);
                }
            }
            else if (_Funcs._RemoveAt != null)
            {
                var syncRoot = SyncRoot(list, b);

                if (syncRoot != null)
                    Monitor.Enter(syncRoot);
                try
                {
                    for (int i = list.Count - 1, end = -1; i > end; --i)
                        _Funcs._RemoveAt(b, i);
                }
                finally
                {
                    if (syncRoot != null)
                        Monitor.Exit(syncRoot);
                }
            }
            else
                throw new NotSupportedException("Clear");
        }

        internal bool Remove(IList<TElement> list, TBase b, TElement item)
        {
            if (_Funcs._Remove != null)
                return _Funcs._Remove(b, item);
            else if (_Funcs._RemoveAt != null)
            {
                var syncRoot = SyncRoot(list, b);

                if (syncRoot != null)
                    Monitor.Enter(syncRoot);
                try
                {
                    int ix = list.IndexOf(item);

                    if (ix >= 0)
                    {
                        _Funcs._RemoveAt(b, ix);
                        return true;
                    }

                    return false;
                }
                finally
                {
                    if (syncRoot != null)
                        Monitor.Exit(syncRoot);
                }
            }
            else
                throw new NotSupportedException("Remove");
        }

        internal bool IsSynchronized(IList<TElement> list, TBase b)
        {
            return
                _Funcs._IsSynchronized != null ? _Funcs._IsSynchronized(b) :
                _Funcs._SyncRoot != null ? _Funcs._SyncRoot(b) != null :
                false;
        }

        internal object SyncRoot(IList<TElement> list, TBase b)
        { return _Funcs._SyncRoot == null ? null : _Funcs._SyncRoot(b); }

        internal void Insert(IList<TElement> list, TBase b, int index, TElement item)
        {
            if (_Funcs._Insert != null)
                _Funcs._Insert(b, index, item);
            else
                throw new NotSupportedException("Insert");
        }

        internal void RemoveAt(IList<TElement> list, TBase b, int index)
        {
            if (_Funcs._RemoveAt != null)
                _Funcs._RemoveAt(b, index);
            else
                throw new NotSupportedException("RemoveAt");
        }

        internal void ReplaceAt(IList<TElement> list, TBase b, int index, TElement item)
        {
            if (_Funcs._ReplaceAt != null)
                _Funcs._ReplaceAt(b, index, item);
            else if (_Funcs._RemoveAt != null && _Funcs._Insert != null)
            {
                var syncRoot = SyncRoot(list, b);

                if (syncRoot != null)
                    Monitor.Enter(syncRoot);
                try
                {
                    if (list.Count <= index)
                        throw new ArgumentOutOfRangeException("index");

                    _Funcs._RemoveAt(b, index);
                    _Funcs._Insert(b, index, item);
                }
                finally
                {
                    if (syncRoot != null)
                        Monitor.Exit(syncRoot);
                }
            }
            else
                throw new NotSupportedException("ReplaceAt");
        }

        #endregion

        internal int GetElementsHashCode()
        { return _Funcs.GetHashCode() ; }

        internal bool ElementsEqual(ListReturnPathBase<TBase, TElement> other)
        { return _Funcs.Equals(ref other._Funcs); }
    }

    /// <summary>
    /// Implementation of <see cref="IListReturnPath{TElement}"/>. Use <see cref="ListReturnPathFactory{TElement}"/> to create an instance of this type.
    /// </summary>
    /// <typeparam name="TElement">Type of the elements in the extended list.</typeparam>
    public class ListReturnPath<TElement> : ListReturnPathBase<IList<TElement>,TElement>, IListReturnPath<TElement>, IEquatable<ListReturnPath<TElement>>
    {
        internal ListReturnPath(
            ListReturnPathFuncStruct<IList<TElement>, TElement> funcs
        )
            : base(funcs)
        {
            _HashCode =
                Hasher.CreateFromRef(this.GetType()).AddValue(base.GetElementsHashCode());
        }

        readonly int _HashCode;

        #region IListReturnPath<TElement> Members

        public IValueProvider<bool> IsReadOnly(IList<TElement> list)
        { return base.IsReadOnly(list, list) ; }

        public void Add(IList<TElement> list, TElement item)
        { base.Add(list, list, item); }

        public void Clear(IList<TElement> list)
        { base.Clear(list, list); }

        public bool Remove(IList<TElement> list, TElement item)
        { return base.Remove(list, list, item); }

        public bool IsSynchronized(IList<TElement> list)
        { return base.IsSynchronized(list, list); }

        public object SyncRoot(IList<TElement> list)
        { return base.SyncRoot(list, list); }

        public void Insert(IList<TElement> list, int index, TElement item)
        { Insert(list, list, index, item); }

        public void RemoveAt(IList<TElement> list, int index)
        { base.RemoveAt(list, list, index); }

        public void ReplaceAt(IList<TElement> list, int index, TElement item)
        { base.ReplaceAt(list, list, index, item); }

        #endregion

        public override bool Equals(object obj)
        {
            return
                Equals(obj as ListReturnPath<TElement>);
        }

        public override int GetHashCode()
        { return _HashCode; }

        #region IEquatable<ListReturnPath<TElement>> Members

        public bool Equals(ListReturnPath<TElement> other)
        {
            return
                object.ReferenceEquals(this, other)
                ||
                    other != null
                    && base.ElementsEqual(other);
        }

        #endregion
    }

    /// <summary>
    /// Factory object to create <see cref="ListReturnPath{TElement}"/> instances.
    /// </summary>
    /// <typeparam name="TElement">Type of the elements of the extended list.</typeparam>
    /// <remarks>
    /// C# 3.0 does not yet support optional named parameters. This class is a work arround.
    /// </remarks>
    public class ListReturnPathFactory<TElement> 
    {
        Func<IList<TElement>, IValueProvider<bool>> _IsReadOnly;
        public Func<IList<TElement>, IValueProvider<bool>> IsReadOnly 
        { get { return _IsReadOnly; } set { _IsReadOnly = value; _Cache = null; } }

        Action<IList<TElement>, TElement> _Add;
        public Action<IList<TElement>,TElement> Add
        { get { return _Add; } set { _Add = value; _Cache = null; } }

        Action<IList<TElement>> _Clear;
        public Action<IList<TElement>> Clear
        { get { return _Clear; } set { _Clear = value; _Cache = null; } }

        Func<IList<TElement>, TElement, bool> _Remove;
        public Func<IList<TElement>,TElement, bool> Remove
        { get { return _Remove; } set { _Remove = value; _Cache = null; } }

        Func<IList<TElement>, bool> _IsSynchronized;
        public Func<IList<TElement>,bool> IsSynchronized
        { get { return _IsSynchronized; } set { _IsSynchronized = value; _Cache = null; } }

        Func<IList<TElement>, object> _SyncRoot;
        public Func<IList<TElement>,object> SyncRoot
        { get { return _SyncRoot; } set { _SyncRoot = value; _Cache = null; } }

        Action<IList<TElement>, int, TElement> _Insert;
        public Action<IList<TElement>, int, TElement> Insert
        { get { return _Insert; } set { _Insert = value; _Cache = null; } }

        Action<IList<TElement>, int> _RemoveAt;
        public Action<IList<TElement>, int> RemoveAt
        { get { return _RemoveAt; } set { _RemoveAt = value; _Cache = null; } }

        Action<IList<TElement>, int, TElement> _ReplaceAt;
        public Action<IList<TElement>, int, TElement> ReplaceAt
        { get { return _ReplaceAt; } set { _ReplaceAt = value; _Cache = null; } }

        ListReturnPath<TElement> _Cache;

        public ListReturnPath<TElement> CreateReturnPath()
        {
            return
                _Cache ??
                (
                    _Cache = new ListReturnPath<TElement>(
                        new ListReturnPathFuncStruct<IList<TElement>, TElement>()
                        {
                            _IsReadOnly = IsReadOnly,
                            _Add = Add,
                            _Clear = Clear,
                            _Remove = Remove,
                            _IsSynchronized = IsSynchronized,
                            _SyncRoot = SyncRoot,
                            _Insert = Insert,
                            _RemoveAt = RemoveAt,
                            _ReplaceAt = ReplaceAt
                        }
                    )
                )
            ;
        }

        public static implicit operator ListReturnPath<TElement>(ListReturnPathFactory<TElement> t)
        { return t.CreateReturnPath(); }
    }

    /// <summary>
    /// Factory object to conveniently create return path extended ToList conversions of sequences.
    /// </summary>
    /// <typeparam name="TElement">Type of the elements of the converted sequence and the resulting list.</typeparam>
    public class ToListFactory<TElement>
    {
        public ToListFactory(IEnumerable<TElement> source)
        { 
            _Source = source;
            _ReturnPathFactory = new ListReturnPathFactory<TElement>();
        }

        IEnumerable<TElement> _Source;
        ListReturnPathFactory<TElement> _ReturnPathFactory;

        public IList<TElement> Create()
        { return Obtics.Collections.ObservableEnumerable.ToList(_Source, _ReturnPathFactory.CreateReturnPath()); }

        public ToListFactory<TElement> IsReadOnly(Func<IList<TElement>, IValueProvider<bool>> isReadOnly)
        {
            _ReturnPathFactory.IsReadOnly = isReadOnly;
            return this;
        }

        public ToListFactory<TElement> Add(Action<IList<TElement>, TElement> add)
        {
            _ReturnPathFactory.Add = add;
            return this;
        }

        public ToListFactory<TElement> Clear(Action<IList<TElement>> clear)
        {
            _ReturnPathFactory.Clear = clear;
            return this;
        }

        public ToListFactory<TElement> Remove(Func<IList<TElement>, TElement, bool> remove)
        {
            _ReturnPathFactory.Remove = remove;
            return this;
        }

        public ToListFactory<TElement> IsSynchronized(Func<IList<TElement>, bool> isSynchronized)
        {
            _ReturnPathFactory.IsSynchronized = isSynchronized;
            return this;
        }

        public ToListFactory<TElement> SyncRoot(Func<IList<TElement>, object> syncRoot)
        {
            _ReturnPathFactory.SyncRoot = syncRoot;
            return this;
        }

        public ToListFactory<TElement> Insert(Action<IList<TElement>, int, TElement> insert)
        {
            _ReturnPathFactory.Insert = insert;
            return this;
        }

        public ToListFactory<TElement> RemoveAt(Action<IList<TElement>, int> removeAt)
        {
            _ReturnPathFactory.RemoveAt = removeAt;
            return this;
        }

        public ToListFactory<TElement> ReplaceAt(Action<IList<TElement>, int, TElement> replaceAt)
        {
            _ReturnPathFactory.ReplaceAt = replaceAt;
            return this;
        }        
    }

    /// <summary>
    /// Implementation of <see cref="IListReturnPath{TElement}"/> that passes an extra parameter to its return path method implementing delegates.
    /// </summary>
    /// <typeparam name="TElement">Type of the elements of the extended list.</typeparam>
    /// <typeparam name="TParameter">Type of the extra parameter passed to the return path delegates.</typeparam>
    public class ListReturnPath<TElement,TParameter> : IListReturnPath<TElement>, IEquatable<ListReturnPath<TElement,TParameter>>
    {
        readonly TParameter _Parameter;
        readonly ListReturnPathBase<Tuple<IList<TElement>, TParameter>, TElement> _Base;
        readonly int _HashCode;

        internal ListReturnPath(
            ListReturnPathBase<Tuple<IList<TElement>,TParameter>,TElement> b,
            TParameter parameter
        )            
        {
            _Base = b;
            _Parameter = parameter;
            _HashCode = Hasher.CreateFromRef(this.GetType()).AddValue(_Base.GetElementsHashCode()).Add(_Parameter);
        }

        #region IListReturnPath<TElement> Members

        public IValueProvider<bool> IsReadOnly(IList<TElement> list)
        { return _Base.IsReadOnly(list, Tuple.Create( list, _Parameter) ); }

        public void Add(IList<TElement> list, TElement item)
        { _Base.Add(list, Tuple.Create(list, _Parameter), item); }

        public void Clear(IList<TElement> list)
        { _Base.Clear(list, Tuple.Create(list, _Parameter)); }

        public bool Remove(IList<TElement> list, TElement item)
        { return _Base.Remove(list, Tuple.Create(list, _Parameter), item); }

        public bool IsSynchronized(IList<TElement> list)
        { return _Base.IsSynchronized(list, Tuple.Create(list, _Parameter)); }

        public object SyncRoot(IList<TElement> list)
        { return _Base.SyncRoot(list, Tuple.Create(list, _Parameter)); }

        public void Insert(IList<TElement> list, int index, TElement item)
        { _Base.Insert(list, Tuple.Create(list, _Parameter), index, item); }

        public void RemoveAt(IList<TElement> list, int index)
        { _Base.RemoveAt(list, Tuple.Create(list, _Parameter), index); }

        public void ReplaceAt(IList<TElement> list, int index, TElement item)
        { _Base.ReplaceAt(list, Tuple.Create(list, _Parameter), index, item); }

        #endregion

        public override bool Equals(object obj)
        {
            return
                Equals(obj as ListReturnPath<TElement, TParameter>);
        }

        public override int GetHashCode()
        { return _HashCode; }

        #region IEquatable<ListReturnPathWithParameter<TElement,TParameter>> Members

        public bool Equals(ListReturnPath<TElement, TParameter> other)
        {
            return
                object.ReferenceEquals(this, other)
                ||
                    other != null
                    && ObticsEqualityComparer<TParameter>.Equals(_Parameter, other._Parameter)
                    && _Base.ElementsEqual(other._Base);
        }

        #endregion
    }

    public class ListReturnPathFactory<TElement, TParameter>
    {
        Func<IList<TElement>, TParameter, IValueProvider<bool>> _IsReadOnly;
        public Func<IList<TElement>, TParameter, IValueProvider<bool>> IsReadOnly
        { get { return _IsReadOnly; } set { _IsReadOnly = value; _Cache = null; } }

        Action<IList<TElement>, TParameter, TElement> _Add;
        public Action<IList<TElement>, TParameter, TElement> Add
        { get { return _Add; } set { _Add = value; _Cache = null; } }

        Action<IList<TElement>, TParameter> _Clear;
        public Action<IList<TElement>, TParameter> Clear
        { get { return _Clear; } set { _Clear = value; _Cache = null; } }

        Func<IList<TElement>, TParameter, TElement, bool> _Remove;
        public Func<IList<TElement>, TParameter, TElement, bool> Remove
        { get { return _Remove; } set { _Remove = value; _Cache = null; } }

        Func<IList<TElement>, TParameter, bool> _IsSynchronized;
        public Func<IList<TElement>, TParameter, bool> IsSynchronized
        { get { return _IsSynchronized; } set { _IsSynchronized = value; _Cache = null; } }

        Func<IList<TElement>, TParameter, object> _SyncRoot;
        public Func<IList<TElement>, TParameter, object> SyncRoot
        { get { return _SyncRoot; } set { _SyncRoot = value; _Cache = null; } }

        Action<IList<TElement>, TParameter, int, TElement> _Insert;
        public Action<IList<TElement>, TParameter, int, TElement> Insert
        { get { return _Insert; } set { _Insert = value; _Cache = null; } }

        Action<IList<TElement>, TParameter, int> _RemoveAt;
        public Action<IList<TElement>, TParameter, int> RemoveAt
        { get { return _RemoveAt; } set { _RemoveAt = value; _Cache = null; } }

        Action<IList<TElement>, TParameter, int, TElement> _ReplaceAt;
        public Action<IList<TElement>, TParameter, int, TElement> ReplaceAt
        { get { return _ReplaceAt; } set { _ReplaceAt = value; _Cache = null; } }

        ListReturnPathBase<Tuple<IList<TElement>, TParameter>,TElement> _Cache;

        public ListReturnPath<TElement, TParameter> CreateReturnPath(TParameter parameter)
        {
            if(_Cache == null)
            {
                var fs = new ListReturnPathFuncStruct<Tuple<IList<TElement>, TParameter>, TElement>();

                if(IsReadOnly != null) fs._IsReadOnly = d => IsReadOnly(d.First, d.Second);
                if(Add != null) fs._Add = (d, e) => Add(d.First, d.Second, e);
                if(Clear != null) fs._Clear = d => Clear(d.First, d.Second);
                if(Remove != null) fs._Remove = (d, e) => Remove(d.First, d.Second, e);
                if(IsSynchronized != null) fs._IsSynchronized = d => IsSynchronized(d.First, d.Second);
                if(SyncRoot != null) fs._SyncRoot = d => SyncRoot(d.First, d.Second);
                if(Insert != null) fs._Insert = (d, i, e) => Insert(d.First, d.Second, i, e);
                if(RemoveAt != null) fs._RemoveAt = (d, i) => RemoveAt(d.First, d.Second, i);
                if(ReplaceAt != null) fs._ReplaceAt = (d, i, e) => ReplaceAt(d.First, d.Second, i, e);
            
                _Cache = new ListReturnPathBase<Tuple<IList<TElement>, TParameter>, TElement>(fs);
            }

            return new ListReturnPath<TElement, TParameter>(_Cache, parameter);
        }
    }

    public class ToListFactory<TElement, TParameter>
    {
        public ToListFactory(IEnumerable<TElement> source, TParameter parameter)
        {
            _Source = source;
            _Parameter = parameter;
            _ReturnPathFactory = new ListReturnPathFactory<TElement,TParameter>();
        }

        IEnumerable<TElement> _Source;
        TParameter _Parameter;
        ListReturnPathFactory<TElement, TParameter> _ReturnPathFactory;

        public IList<TElement> Create()
        { return Obtics.Collections.ObservableEnumerable.ToList(_Source, _ReturnPathFactory.CreateReturnPath(_Parameter)); }

        public ToListFactory<TElement, TParameter> IsReadOnly(Func<IList<TElement>, TParameter, IValueProvider<bool>> isReadOnly)
        {
            _ReturnPathFactory.IsReadOnly = isReadOnly;
            return this;
        }

        public ToListFactory<TElement, TParameter> Add(Action<IList<TElement>, TParameter, TElement> add)
        {
            _ReturnPathFactory.Add = add;
            return this;
        }

        public ToListFactory<TElement, TParameter> Clear(Action<IList<TElement>, TParameter> clear)
        {
            _ReturnPathFactory.Clear = clear;
            return this;
        }

        public ToListFactory<TElement, TParameter> Remove(Func<IList<TElement>, TParameter, TElement, bool> remove)
        {
            _ReturnPathFactory.Remove = remove;
            return this;
        }

        public ToListFactory<TElement, TParameter> IsSynchronized(Func<IList<TElement>, TParameter, bool> isSynchronized)
        {
            _ReturnPathFactory.IsSynchronized = isSynchronized;
            return this;
        }

        public ToListFactory<TElement, TParameter> SyncRoot(Func<IList<TElement>, TParameter, object> syncRoot)
        {
            _ReturnPathFactory.SyncRoot = syncRoot;
            return this;
        }

        public ToListFactory<TElement, TParameter> Insert(Action<IList<TElement>, TParameter, int, TElement> insert)
        {
            _ReturnPathFactory.Insert = insert;
            return this;
        }

        public ToListFactory<TElement, TParameter> RemoveAt(Action<IList<TElement>, TParameter, int> removeAt)
        {
            _ReturnPathFactory.RemoveAt = removeAt;
            return this;
        }

        public ToListFactory<TElement, TParameter> ReplaceAt(Action<IList<TElement>, TParameter, int, TElement> replaceAt)
        {
            _ReturnPathFactory.ReplaceAt = replaceAt;
            return this;
        }
    }

    public static class ListReturnPath
    {
        public static ToListFactory<TElement> ToListFactory<TElement>(this IEnumerable<TElement> source)
        { return new ToListFactory<TElement>(source); }

        public static ToListFactory<TElement, TParameter> ToListFactory<TElement, TParameter>(this IEnumerable<TElement> source, TParameter parameter)
        { return new ToListFactory<TElement, TParameter>(source, parameter); }
    }
}
