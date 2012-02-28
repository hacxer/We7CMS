using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace We7.Framework
{
    public sealed class AssemblyHelper
    {
        private static object objLock = new object();

        private static AssemblyHelper instance;

        private RegexOptions options = RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline;

        List<Assembly> asms = new List<Assembly>();

        public static AssemblyHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (objLock)
                    {
                        instance = new AssemblyHelper();
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asm"></param>
        /// <returns></returns>
        public Type[] GetTypesByAssembly(Assembly asm)
        {
            return GetTypesByAssembly(asm, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asm"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public Type[] GetTypesByAssembly(Assembly asm, string pattern)
        {
            List<Type> list = new List<Type>();
            Type[] types = asm.GetTypes();
            foreach (Type item in types)
            {
                if (!string.IsNullOrEmpty(pattern))
                {
                    if (Regex.IsMatch(item.FullName, pattern, options))
                    {
                        list.Add(item);
                    }
                }
                else
                {
                    return types;
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asm"></param>
        /// <returns></returns>
        public Type[] GetTypesFromAssembly(Assembly asm)
        {
            return (Type[])asm.GetCustomAttributes(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asm"></param>
        /// <returns></returns>
        public Type[] GetTypesFromAssemblyByAttribute<T>(Assembly asm)
        {
            return (Type[])asm.GetCustomAttributes(typeof(T), false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<T> GetAttributeByType<T>(Type type)
        {
            List<T> list = new List<T>();
            object[] objs = type.GetCustomAttributes(typeof(T), false);
            if (objs != null && objs.Length > 0)
            {
                foreach (T item in objs)
                {
                    list.Add(item);
                }
            }
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Assembly[] GetPluginsAssemblies()
        {
            List<Assembly> list = new List<Assembly>();
            foreach (Assembly item in CurrentDomainAssemblies())
            {
                if (item.FullName.StartsWith("We7.Plugin"))
                {
                    if (!list.Contains(item))
                    {
                        list.Add(item);
                    }
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Assembly[] GetSysAssemblies()
        {
            List<Assembly> list = new List<Assembly>();
            foreach (Assembly item in CurrentDomainAssemblies())
            {
                if (!item.FullName.StartsWith("We7.CMS.Web") && (item.FullName.StartsWith("We7") || item.FullName.StartsWith("WebEngine2007")))
                {
                    if (!list.Contains(item))
                    {
                        list.Add(item);
                    }

                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Pattern"></param>
        /// <returns></returns>
        public Assembly[] GetAssemblies(string Pattern)
        {
            List<Assembly> list = new List<Assembly>();

            foreach (Assembly item in CurrentDomainAssemblies())
            {
                if (Regex.IsMatch(item.FullName, Pattern, options))
                {
                    if (!list.Contains(item))
                    {
                        list.Add(item);
                    }
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private List<Assembly> CurrentDomainAssemblies()
        {
            if (asms.Count <= 0)
            {
                asms.AddRange(AppDomain.CurrentDomain.GetAssemblies());
            }
            return asms;
        }
    }
}
