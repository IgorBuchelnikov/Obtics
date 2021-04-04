using System;
using System.Collections.Generic;
using SL = System.Linq;
using System.Text;

namespace Obtics.Collections
{
    /// <summary>
    /// Indicates the historic relation between two <see cref="VersionNumber"/> values.
    /// </summary>
    public enum VersionRelation
    {
        /// <summary>
        /// The first <see cref="VersionNumber"/> is before or is the same as the second <see cref="VersionNumber"/>.
        /// (first &lt;= second)
        /// </summary>
        Past,
        /// <summary>
        /// The first <see cref="VersionNumber"/> is the <see cref="VersionNumber"/> directly following the second <see cref="VersionNumber"/>.
        /// (first == second + 1)
        /// </summary>
        Next,
        /// <summary>
        /// The first <see cref="VersionNumber"/> is further in the future than the next <see cref="VersionNumber"/> after the second <see cref="VersionNumber"/>.
        /// (first &gt; second + 1)
        /// </summary>
        Future
    };

    /// <summary>
    /// ContentVersion is an abstraction of the version number of an object. Whenever an object changes and it has a ContentVersion
    /// this ContentVersion should become original VersionNumber.Next.
    /// 
    /// VersionNumbers can be compared to see if the are equal, are in the past, (far)future or is next in line.
    /// </summary>
    public struct VersionNumber : IEquatable<VersionNumber>
    {
        private VersionNumber( int value )
        {
            _Value = value;
        }

        int _Value;

        /// <summary>
        /// Compares this <see cref="VersionNumber"/> to a current <see cref="VersionNumber"/> and returns its historic relation to that current <see cref="VersionNumber"/>.
        /// </summary>
        /// <param name="current">The <see cref="VersionNumber"/> to compare this <see cref="VersionNumber"/> to.</param>
        /// <returns>A <see cref="VersionRelation"/> value indicating the historic relation of this <see cref="VersionNumber"/> to the <paramref name="current"/> <see cref="VersionNumber"/>.</returns>
        public VersionRelation IsInRelationTo(VersionNumber current)
        {
            unchecked
            {
                var r = _Value - current._Value;

                return
                    r < 1 ? VersionRelation.Past :
                    r == 1 ? VersionRelation.Next :
                    VersionRelation.Future;
            }
        }

        /// <summary>
        /// Compares this <see cref="VersionNumber"/> to a nullable current <see cref="VersionNumber"/> and returns its historic relation to that current <see cref="VersionNumber"/>.
        /// When the current <see cref="VersionNumber"/> is null, this <see cref="VersionNumber"/> will always be regarded as in the <see cref="SequenceRelation.Future"/>.
        /// </summary>
        /// <param name="current">The nullable <see cref="VersionNumber"/> to compare this <see cref="VersionNumber"/> to.</param>
        /// <returns>A <see cref="VersionRelation"/> value indicating the historic relation of this <see cref="VersionNumber"/> to the nullable <paramref name="current"/> <see cref="VersionNumber"/>.
        /// If the <paramref name="current"/> <see cref="VersionNumber"/> is null then the result will be <see cref="SequenceRelation.Future"/>.</returns>
        public VersionRelation IsInRelationTo(VersionNumber? current)
        {
            return
                current.HasValue ? IsInRelationTo(current.Value) :
                VersionRelation.Future;
        }

        /// <summary>
        /// Gives the <see cref="VersionNumber"/> that is next after this <see cref="VersionNumber"/>.
        /// </summary>
        /// <remarks>
        /// Always:
        /// <code>
        /// this.Next.IsInRelationTo(this) == SequenceRelation.Next;
        /// </code>
        /// </remarks>
        public VersionNumber Next
        { get { unchecked { return new VersionNumber(_Value + 1); } } }

        /// <summary>
        /// Gives the <see cref="VersionNumber"/> where this <see cref="VersionNumber"/> is next after.
        /// </summary>
        /// <remarks>
        /// Always:
        /// <code>
        /// this.IsInRelationTo(this.Previous) == SequenceRelation.Next;
        /// </code>
        /// </remarks>
        public VersionNumber Previous
        { get { unchecked { return new VersionNumber(_Value - 1); } } }

        #region IEquatable<ContentVersion> Members

        /// <summary>
        /// Compares this <see cref="VersionNumber"/> to another and returns true if they are equal.
        /// </summary>
        /// <param name="other">The other <see cref="VersionNumber"/> to equality compare this <see cref="VersionNumber"/> to.</param>
        /// <returns>Returns true if this <see cref="VersionNumber"/> equals the <paramref name="other"/> <see cref="VersionNumber"/>.</returns>
        public bool Equals(VersionNumber other)
        {
            return _Value == other._Value;
        }

        #endregion

        /// <summary>
        /// Override of object.Equals method.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with.</param>
        /// <returns>True if <paramref name="obj"/> is a <see cref="VersionNumber"/> and this <see cref="VersionNumber"/> equals <paramref name="obj"/> as <see cref="VersionNumber"/>.</returns>
        public override bool Equals(object obj)
        { return obj is VersionNumber && Equals((VersionNumber)obj); }

        /// <summary>
        /// Override of object.GetHashCode
        /// </summary>
        /// <returns>A hashcode for this <see cref="VersionNumber"/>.</returns>
        public override int GetHashCode()
        { return _Value.GetHashCode(); }

        /// <summary>
        /// Equality operator; Compares two <see cref="VersionNumber"/> values for equality.
        /// </summary>
        /// <param name="firstVersionNumber">The first <see cref="VersionNumber"/> to compare.</param>
        /// <param name="secondVersionNumber">The second <see cref="VersionNumber"/> to compare.</param>
        /// <returns>True if the <paramref name="firstVersionNumber"/> and <paramref name="secondVersionNumber"/> <see cref="VersionNumber"/>s are equal and false otherwise.</returns>
        public static bool operator ==(VersionNumber firstVersionNumber, VersionNumber secondVersionNumber)
        { return firstVersionNumber.Equals(secondVersionNumber); }

        /// <summary>
        /// Inequality operator; Compares two <see cref="VersionNumber"/> values for inequality.
        /// </summary>
        /// <param name="firstVersionNumber">The first <see cref="VersionNumber"/> to compare.</param>
        /// <param name="secondVersionNumber">The second <see cref="VersionNumber"/> to compare.</param>
        /// <returns>True if the <paramref name="firstVersionNumber"/> and <paramref name="secondVersionNumber"/> <see cref="VersionNumber"/>s are not equal and false otherwise.</returns>
        public static bool operator !=(VersionNumber firstVersionNumber, VersionNumber secondVersionNumber)
        { return !firstVersionNumber.Equals(secondVersionNumber); }

        /// <summary>
        /// Converts the <see cref="VersionNumber"/> to a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "(sn " + _Value + ")";
        }
    }
}
