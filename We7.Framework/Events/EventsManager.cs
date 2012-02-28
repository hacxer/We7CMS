using System;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;
using System.IO;
using We7.Framework.Config;

namespace We7.Framework
{
    public sealed class EventsManager
    {
        /// <summary>
        /// 
        /// </summary>
        private Dictionary<Type, object> Events;

        private static object objLock = new object();

        private static EventsManager _instance;

        private static Timer eventTimer;

        private readonly string RegisteEvents = "/_data/Cache/";

        private readonly string _file = "RegisteEvents";

        /// <summary>
        /// 
        /// </summary>
        EventsManager()
        {
            Events = new Dictionary<Type, object>();

            string cache = Util.Utils.GetMapPath(RegisteEvents);
            if (!Directory.Exists(cache))
            {
                Directory.CreateDirectory(cache);
            }
        }

        public bool HasRegister<TEvent>()
        {
            Type type = typeof(TEvent);
            return Events.ContainsValue(Activator.CreateInstance(type));
        }

        public void Register<TEvent>()
        {
            Type type = typeof(TEvent);
            Register(Activator.CreateInstance(type), type);
        }

        public void Register<TEvent>(object instance)
        {
            Type type = typeof(TEvent);
            Register(instance, type);
        }

        public void Register(object instance, Type type)
        {
            if (!this.Events.ContainsKey(type))
            {
                Events.Add(type, instance);
                Util.Serializer.SaveAsBinary(Events, Path.Combine(Util.Utils.GetMapPath(RegisteEvents), _file));
            }
        }

        public static EventsManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (objLock)
                    {
                        if (_instance == null)
                        {
                            _instance = new EventsManager();
                        }
                    }
                }
                return _instance;
            }
        }

        public void Start()
        {
            int outTime = 6;
            int.TryParse(GeneralConfigs.GetConfig().EventCallbackTime.ToString(), out outTime);
            eventTimer = new Timer(new TimerCallback(EventCallback), _instance, outTime, outTime);
        }

        public void Stop()
        {
            eventTimer.Dispose();
        }

        public void EventCallback(object sender)
        {
            if (File.Exists(Path.Combine(Util.Utils.GetMapPath(RegisteEvents), _file)))
            {
                try
                {
                    Dictionary<Type, object> events = (Dictionary<Type, object>)Util.Serializer.LoadBinaryFile(Path.Combine(Util.Utils.GetMapPath(RegisteEvents), _file));

                    if (events != null)
                    {
                        if (events.Count != Events.Count)
                        {
                            foreach (KeyValuePair<Type, object> item in events)
                            {
                                Events.Remove(item.Key);
                                Events.Add(item.Key, Activator.CreateInstance(item.Key));
                            }
                        }
                        else
                        {
                            foreach (KeyValuePair<Type, object> it in Events)
                            {
                                if (it.Value == null)
                                {
                                    Activator.CreateInstance(it.Key);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog(typeof(EventsManager), ex);
                }
            }
        }
    }
}
