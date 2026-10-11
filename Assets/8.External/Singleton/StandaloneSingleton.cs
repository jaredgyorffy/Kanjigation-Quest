using UnityEngine;

namespace Core.Singleton
{
    public abstract class StandaloneSingleton<T> : StandaloneSingleton where T : class, new()
    {
        static T _Instance = null;
        static readonly object _InstanceLock = new object();

        public static T Instance
        {
            get
            {
                lock (_InstanceLock)
                {
                    if (_Instance != null)
                    {
                        return _Instance;
                    }

                    _Instance = new T();
                    (_Instance as StandaloneSingleton).Initialize();

                    return _Instance;
                }
            }
        }

        internal override void Initialize()
        {
            Application.quitting += OnQuit;
            OnInitialize();
        }

        protected virtual void OnInitialize() { }

        private void OnQuit()
        {
            _Instance = null;
            Application.quitting -= OnQuit;
        }
    }

    public abstract class StandaloneSingleton
    {
        internal abstract void Initialize();
        public virtual void OnBootstrapped() { }
    }
}