using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace Obtics
{

    internal abstract class ObservableObjectBase<TPrms> 
    {
        #region bit flags

        //Boolean state flags are stored in a single Int32 value (assuming we'll 
        //never have more than 32 flags in a dirived class) to reduce memory consumption
        //Otherwise each boolean flag would get it's own 32bit Word for storage. 

        volatile Int32 _Flags;

        protected struct FlagsState
        {
            public Int32 _OldState;
            public Int32 _NewState;

            public bool GetBitFlag(Int32 mask)
            { return (_NewState & mask) != 0; }

            public bool SetBitFlag(Int32 mask, bool value)
            {
                if (value)
                    _NewState |= mask;
                else
                    _NewState &= ~mask;

                return value;
            }

            public Int32 GetNum(Int32 mask, Int32 offset)
            { return (_NewState & mask) >> offset; }

            public Int32 SetNum(Int32 mask, Int32 offset, Int32 value)
            {
                _NewState = (_NewState & ~mask) | (value << offset) & mask;
                return value;
            }

            public bool IsLocked
            { get { return (_OldState & LockTokenMask) != 0; } }
        }

        protected bool PeekFlag(Int32 mask)
        { return (_Flags & mask) != 0; }

        protected bool GetFlags(out FlagsState flagState)
        {
            Int32 flags = _Flags;

            flagState = new FlagsState { _NewState = flags, _OldState = flags }; 

            var res = (flags & LockTokenMask) == 0;

            if (!res)
                Thread.Sleep(0);

            return res;
        }

        protected bool Lock(ref FlagsState flagsState)
        {
            if ((flagsState._OldState & LockTokenMask) != 0)
                return true;

#pragma warning disable 420
            var res = Interlocked.CompareExchange(ref _Flags, flagsState._OldState | LockTokenMask, flagsState._OldState) == flagsState._OldState;
#pragma warning restore 420

            if(res)
                flagsState._OldState |= LockTokenMask;
            else
                Thread.Sleep(0);

            return res;
        }

        protected bool GetAndLockFlags(out FlagsState flags)
        { return GetFlags(out flags) && Lock(ref flags); }

        protected bool Commit(ref FlagsState flagsState)
        {
#pragma warning disable 420
            var res = 
                flagsState._NewState == flagsState._OldState 
                || Interlocked.CompareExchange(ref _Flags, flagsState._NewState, flagsState._OldState) == flagsState._OldState
            ;
#pragma warning restore 420

            if (res)
                flagsState._OldState = flagsState._NewState;
            else
                Thread.Sleep(0);

            return res;
        }

        const Int32 LockTokenMask = 1 << 0;

        protected const Int32 BitFlagIndexEnd = 1;

        #endregion

        protected TPrms _Prms;

        internal virtual void Initialize(TPrms prms)
        {
            //no locking, called once at construction
            _Prms = prms;
        }

#if HLG

        public const int LoggSize = 256;
        public Tuple<int, object>[] _Logg = new Tuple<int, object>[LoggSize];
        public Tuple<int, object>[] _LoggView;

            public int _LoggPos = 0;

            public void Logg(object sender, object msg)
            {
                //if (sender is Obtics.Collections.Transformations.ExtremityAggregate<int> || sender is Obtics.Values.Transformations.ExceptionTransformation<int, InvalidOperationException>)
                //{
                    _LoggView = null;
                    var pos = Interlocked.Increment(ref _LoggPos);
                    _Logg[(pos - 1) & (LoggSize - 1)] = Tuple.Create(pos, msg);
                //}
            }

            public Tuple<int, object>[] LogView
            {
                get
                {
                    if (_LoggView == null)
                    {
                        _LoggView = new Tuple<int, object>[LoggSize];

                        for (int i = 0; i < LoggSize; ++i)
                            _LoggView[i] = _Logg[(_LoggPos - i - 1) & (LoggSize - 1)];

                    }

                    return _LoggView;
                }
            }


#endif

    }
}
