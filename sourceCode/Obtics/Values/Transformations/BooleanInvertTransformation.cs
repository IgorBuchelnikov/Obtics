
namespace Obtics.Values.Transformations
{
    internal sealed class BooleanInvertTransformation : NCSourcedObjectToVP<bool, IInternalValueProvider<bool>>
    {
        public static BooleanInvertTransformation Create(IInternalValueProvider<bool> source)
        {
            if (source == null)
                return null;

            return Carrousel.Get<BooleanInvertTransformation, IInternalValueProvider<bool>>(source);
        }

        protected internal override INotifyChanged GetSource(int i)
        { return i == 0 ? _Prms : null; }

        protected override bool ProtectedGetValue()
        { return !_Prms.Value; }

        protected override void ProtectedSetValue(bool value)
        { _Prms.Value = !value; }

        public override bool IsReadOnly
        { get{ return _Prms.IsReadOnly; } }

        protected override object ProcessSourceChangedNotification(object sender, INCEventArgs args)
        { return args.IsReadOnlyEvent ? args : base.ProcessSourceChangedNotification(sender, args); }
    }
}
