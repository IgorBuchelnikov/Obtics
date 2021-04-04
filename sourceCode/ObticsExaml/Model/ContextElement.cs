using System;
using System.Collections.Generic;
using System.Text;

namespace ObticsExaml.Model
{
    class ContextElement : IContextElement
    {
        public ContextElement(Shop context)
        { _Context = context; }

        #region IContextElement Members

        protected Shop _Context;

        public IShop Context
        { get { return _Context; } }

        #endregion
    }
}
