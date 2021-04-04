using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obtics
{
    internal static class DelayedActionRegistry 
    {
        [ThreadStatic]
        static List<Action> _actions;

        public static void Register(Action action)
        {
            if (_actions != null)
                _actions.Add(action);
            else
                action();
        }

        public static void Register(IEnumerable<Action> actions)
        {
            if (_actions != null)
                _actions.AddRange(actions);
            else
                foreach (var action in actions)
                    action();
        }

        public static bool Enter()
        {
            if (_actions == null)
            {
                _actions = new List<Action>();

                return true;
            }

            return false;
        }

        public static IEnumerable<Action> Reenter(Action toDo)
        {
            var oldActions = _actions;

            try
            {
                _actions = new List<Action>();
                toDo();
                return _actions;

            }
            finally
            {
                _actions = oldActions;
            }

        }

        public static bool IsIn()
        { return _actions != null; }

        public static void Leave(bool flag)
        {
            if(flag)
            {
                var actions = _actions;
                _actions = null;

                foreach (var action in actions)
                    action();
            }
        }
    }
}
