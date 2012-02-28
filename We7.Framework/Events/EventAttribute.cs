using System;
using System.Collections.Generic;
using System.Text;

namespace We7.Framework
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class EventAttribute : Attribute
    {
        public string ID
        {
            get;
            set;
        }

        public string Descript
        {
            get;
            set;
        }

        public int ScanRate
        {
            get;
            set;
        }
    }
}
