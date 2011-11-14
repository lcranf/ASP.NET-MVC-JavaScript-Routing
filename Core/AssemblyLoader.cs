using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;
using System;

namespace JsRouting.Core
{
    /// <summary>
    /// Assembly loader
    /// </summary>
    public static class AssemblyLoader
    {
        /// <summary>
        /// Gets a container with the loaded dependencies
        /// </summary>
        /// <param name="assemblies">Assemblies to scan</param>
        /// <returns>Container containing various dependencies to form the JavaScript routing definitions</returns>
        public static RouteConstructionContainer Load(IEnumerable<string> assemblies)
        {
            var container = new RouteConstructionContainer();

            foreach (var assembly in assemblies)
            {
                container.AddAssembly(Assembly.LoadFrom(assembly));
            }

            return container;
        }
    }

    public sealed class RouteConstructionContainer
    {
        private readonly IList<IRouteSource> sources = new List<IRouteSource>();
        private readonly IList<IRouteInterceptor> interceptors = new List<IRouteInterceptor>();
        private readonly IList<Type> controllerTypes = new List<Type>();
        private readonly IList<IControllerActionInterceptor> controllerActionInterceptors = new List<IControllerActionInterceptor>();
        private readonly IList<IJavaScriptAddition> additions = new List<IJavaScriptAddition>();

        public IEnumerable<IJavaScriptAddition> Additions
        {
            get
            {
                return additions;
            }
        }

        public IEnumerable<IControllerActionInterceptor> ControllerActionInterceptors
        {
            get
            {
                return controllerActionInterceptors;
            }
        }

        public IEnumerable<Type> ControllerTypes
        {
            get
            {
                return controllerTypes;
            }
        }

        public IEnumerable<IRouteInterceptor> Interceptors
        {
            get
            {
                return interceptors;
            }
        }

        public IEnumerable<IRouteSource> Sources
        {
            get
            {
                return sources;
            }
        }

        internal void AddAssembly(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                object value = null;

                bool publicCtr = type.GetConstructor(new Type[0]) != null;

                if (type.IsA<IRouteSource>() && publicCtr)
                {
                    sources.Add((IRouteSource)(value = Activator.CreateInstance(type)));
                }

                if (type.IsA<IRouteInterceptor>() && publicCtr)
                {
                    interceptors.Add((IRouteInterceptor)(value ?? (value = Activator.CreateInstance(type))));
                }

                if (type.IsA<ControllerBase>())
                {
                    controllerTypes.Add(type);
                }

                if (type.IsA<IControllerActionInterceptor>() && publicCtr)
                {
                    controllerActionInterceptors.Add((IControllerActionInterceptor)(value ?? (value = Activator.CreateInstance(type))));
                }

                if (type.IsA<IJavaScriptAddition>() && publicCtr)
                {
                    additions.Add((IJavaScriptAddition)(value ?? (value = Activator.CreateInstance(type))));
                }
            }
        }
    }

    internal static class TypeExt
    {
        public static bool IsA<T>(this Type sub)
        {
            return typeof(T).IsAssignableFrom(sub);
        }
    }
}