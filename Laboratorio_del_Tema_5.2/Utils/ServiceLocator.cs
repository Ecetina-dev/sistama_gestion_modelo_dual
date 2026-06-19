using System;
using System.Collections.Generic;

namespace Laboratorio_del_Tema_5_2.Utils
{
    /// <summary>
    /// Contenedor de inyección de dependencias minimalista para WinForms.
    /// Registra servicios al inicio y los resuelve por constructor injection.
    /// </summary>
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, Func<object>> _factories = 
            new Dictionary<Type, Func<object>>();
        private static readonly Dictionary<Type, object> _singletons = 
            new Dictionary<Type, object>();

        /// <summary>
        /// Registra un servicio como singleton (una sola instancia para toda la app).
        /// </summary>
        public static void RegisterSingleton<T>(T instance) where T : class
        {
            _singletons[typeof(T)] = instance;
            _factories[typeof(T)] = () => instance;
        }

        /// <summary>
        /// Registra un servicio transitorio (nueva instancia cada vez).
        /// </summary>
        public static void Register<T>(Func<T> factory) where T : class
        {
            _factories[typeof(T)] = () => factory();
        }

        /// <summary>
        /// Resuelve un servicio registrado.
        /// </summary>
        public static T Resolve<T>() where T : class
        {
            if (_factories.TryGetValue(typeof(T), out var factory))
                return (T)factory();

            throw new InvalidOperationException(
                $"Servicio no registrado: {typeof(T).Name}. Registralo en Program.cs con ServiceLocator.Register.");
        }
    }
}
