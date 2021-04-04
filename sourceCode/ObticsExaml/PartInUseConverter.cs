using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using Obtics.Values;
using ObticsExaml.Model;
using System.Linq;

namespace ObticsExaml
{
    /// <summary>
    /// This converter decorates an IPart object or a sequence of IPart objects with information
    /// if the Part is associated with any product via an IPartProduct object.
    /// </summary>
    class PartInUseConverter : IValueConverter
    {
        IShop _Context;

        public IShop Context
        {
            get { return _Context; }
            set { _Context = value; }
        }

        #region IValueConverter Members

        Func<IEnumerable<IPart>, IValueProvider<IEnumerable<Pair<IPart, bool>>>> ConvertEnumF =
            ExpressionObserver.Compile(
                (IEnumerable<IPart> seq) =>
                    seq.Select(p => new Pair<IPart, bool>(p, p.ProductParts.Any(pp => pp.PartCount.Value > 0)))
            );

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //Wrap the IPart object(s) in a Pair<> struct where the Second field indicates
            //if the IPart object is 'in use' or not.

            var seq = value as IEnumerable<IPart>;
            if (seq != null)
                return ConvertEnumF(seq).Cascade();

            var part = value as IPart;
            if (part != null)
                return new Pair<IPart, bool>(part, part.ProductParts.Any( pp => pp.PartCount.Value > 0 ));

            return default(Pair<IPart, bool>);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //COnvert back, simply extract the IPart object from a Pair<> struct.

            var pair = (Pair<IPart, bool>?)value;
            if (pair != null)
                return pair.Value.First;

            return null;
        }

        #endregion
    }
}
