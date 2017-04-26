using System;
namespace CaomaoFramework
{
    public class Singleton<T> where T : new()
    {
        private static T s_singleton = default(T);

        private static object s_objectLock = new object();

        public static T singleton
        {
            get
            {
                if (null == Singleton<T>.s_singleton)
                {
                    lock (Singleton<T>.s_objectLock)
                    {
                        if (null == Singleton<T>.s_singleton)
                        {
                            Singleton<T>.s_singleton = ((default(T) == null) ? Activator.CreateInstance<T>() : default(T));
                        }
                    }
                }
                return Singleton<T>.s_singleton;
            }
        }

        protected Singleton()
        {
        }
    }
}
