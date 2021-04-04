using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.Security.Permissions;

namespace Obtics.Configuration
{
#if !SILVERLIGHT
    /// <summary>
    /// Permission needed to be allowed to modify and extend the core functionality of Obtics from code.
    /// </summary>
    public sealed class ExtendObticsPermission : CodeAccessPermission, IUnrestrictedPermission
    {
        PermissionState _State;

        /// <summary>
        /// Create an ExtendObticsPermission object.
        /// </summary>
        /// <param name="state"><see cref="PermissionState"/> value indicating if permission or not is given.</param>
        public ExtendObticsPermission(PermissionState state)
        { _State = state; }

        /// <summary>
        /// Copy the current ExtendObticsPermission object. 
        /// </summary>
        /// <returns>A copy of the current ExtendObticsPermission object.</returns>
        public override IPermission Copy()
        { return new ExtendObticsPermission(_State); }

        static readonly string tagName = typeof(ExtendObticsPermission).Name;
        const string stateAttributeName = "state";

        /// <summary>
        /// Converts the current ExtendObticsPermission object to an XML tag.
        /// </summary>
        /// <returns></returns>
        public override SecurityElement ToXml()
        {
            var res = new SecurityElement(tagName);
            res.AddAttribute(stateAttributeName, _State.ToString());
            return res;
        }

        /// <summary>
        /// Initializes the current ExtendObticsPermission object from an XML tag.
        /// </summary>
        /// <param name="elem">The XML tag to initialize the current ExtendObticsPermission object with.</param>
        public override void FromXml(SecurityElement elem)
        {
            if (elem.Tag != tagName)
                throw new ArgumentException("SecurityElement does not have expected tag name.");

            var stateAttributeValue = elem.Attribute(stateAttributeName);

            if(string.IsNullOrEmpty(stateAttributeValue))
                _State = PermissionState.None;
            else
                try
                {
                    _State = (PermissionState)Enum.Parse(typeof(PermissionState), elem.Attribute("state"));
                }
                catch (ArgumentException ae)
                {
                    throw new ArgumentException("SecurityElement has invalid state attribute", ae);
                }
        }


        ExtendObticsPermission CheckTarget(IPermission target)
        {
            var other = target as ExtendObticsPermission;

            if (other == null && target != null)
                throw new ArgumentException("target is not an ExtendObticsPermission");

            return other;
        }

        /// <summary>
        /// Creates and returns a permission that is the intersection of the current permission and the specified permission.
        /// </summary>
        /// <param name="target">A permission to intersect with the current permission. It must be of the same type as the current permission.</param>
        /// <returns>A new permission that represents the intersection of the current permission and the specified permission. This new permission is a null reference if the intersection is empty.</returns>
        public override IPermission Intersect(IPermission target)
        {
            var other = CheckTarget(target);
            return 
                other == null ? null : 
                _State == PermissionState.Unrestricted ? other : 
                this; 
        }

        /// <summary>
        /// Creates a permission that is the union of the current permission and the specified permission.
        /// </summary>
        /// <param name="target">A permission to combine with the current permission. It must be of the same type as the current permission.</param>
        /// <returns>A new permission that represents the union of the current permission and the specified permission.</returns>
        public override IPermission Union(IPermission target)
        {
            var other = CheckTarget(target);
            return other != null && _State == PermissionState.None ? other : this; 
        }

        /// <summary>
        /// Determines whether the current permission is a subset of the specified permission.
        /// </summary>
        /// <param name="target">A permission that is to be tested for the subset relationship. This permission must be of the same type as the current permission.</param>
        /// <returns>true if the current permission is a subset of the specified permission; otherwise, false.</returns>
        public override bool IsSubsetOf(IPermission target)
        {
            var other = CheckTarget(target);
            return other != null && (other._State == PermissionState.Unrestricted || other._State == _State);
        }


        #region IUnrestrictedPermission Members

        /// <summary>
        /// Returns a value indicating whether the current permission is unrestricted.
        /// </summary>
        /// <returns>true if the current permission is unrestricted; otherwise, false.</returns>
        public bool IsUnrestricted()
        { return _State == PermissionState.Unrestricted; }

        #endregion
    }

    /// <summary>
    /// Allows security actions for <see cref="ExtendObticsPermission"/> to be applied to code using declarative security. 
    /// </summary>
    [SerializableAttribute, AttributeUsageAttribute(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed class ExtendObticsPermissionAttribute : CodeAccessSecurityAttribute
    {
        /// <summary>
        /// Initializes a new instance of the ExtendObticsPermissionAttribute class with the specified SecurityAction.
        /// </summary>
        /// <param name="action">One of the <see cref="SecurityAction"/> values.</param>
        public ExtendObticsPermissionAttribute(SecurityAction action) 
            : base(action) 
        { }

        /// <summary>
        /// Creates and returns a new <see cref="ExtendObticsPermission"/>.
        /// </summary>
        /// <returns>A <see cref="ExtendObticsPermission"/> that corresponds to this attribute.</returns>
        public override IPermission CreatePermission()
        { return new ExtendObticsPermission(PermissionState.Unrestricted); }
    }
#endif
}
