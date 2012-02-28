using System;
using System.Collections.Generic;
using System.Text;

namespace We7.Framework
{
    public class RegistEvent
    {
        public static bool HasRegister<TEvent>()
        {
            return Current.HasRegister<TEvent>();
        }

        public static void Register<TEvent>()
        {
            Current.Register<TEvent>();
        }

        public static void Register<TEvent>(object instance)
        {
            Current.Register<TEvent>(instance);
        }

        private static EventsManager Current
        {
            get
            {
                return EventsManager.Instance;
            }
        }

    }
}
