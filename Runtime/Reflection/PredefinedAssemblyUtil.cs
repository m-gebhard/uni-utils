using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UniUtils.Extensions;

namespace UniUtils.Reflection
{
    /// <summary>
    /// Utility class for retrieving types from predefined assemblies.
    /// </summary>
    public static class PredefinedAssemblyUtil
    {
        /// <summary>
        /// Gets a list of types that are subclasses of the specified generic base type.
        /// </summary>
        /// <param name="genericBaseType">The generic base type to match.</param>
        /// <exception cref="System.Reflection.ReflectionTypeLoadException">Thrown when there is an error loading types from an assembly.</exception>
        /// <returns>A list of types that are subclasses of the specified generic base type.</returns>
        /// <example>
        /// Example usage:
        /// <code>
        /// List&lt;Type&gt; derivedTypes = TypeUtility.GetTypes(typeof(MyGenericBase&lt;&gt;));
        /// foreach (Type t in derivedTypes)
        /// {
        ///     Debug.Log($"Found: {t.FullName}");
        /// }
        /// </code>
        /// </example>
        public static List<Type> GetTypes(Type genericBaseType)
        {
            List<Type> types = new List<Type>();

            // Loop through all loaded assemblies
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    // Get types defined in the assembly
                    foreach (Type type in assembly.GetTypes())
                    {
                        // Check if the type is a subclass of the specified generic type
                        if (type.IsClass && !type.IsAbstract && HasGenericBaseType(type, genericBaseType))
                        {
                            types.Add(type);
                        }
                    }
                }
                catch (ReflectionTypeLoadException e)
                {
                    throw new ReflectionTypeLoadException(
                        e.Types,
                        e.LoaderExceptions,
                        $"Failed to load types from assembly '{assembly.FullName}': {e.Message}"
                    );
                }
            }

            return types;
        }

        /// <summary>
        /// Checks if a type has the specified generic base type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <param name="genericBaseType">The generic base type to match.</param>
        /// <returns>True if the type has the specified generic base type; otherwise, false.</returns>
        private static bool HasGenericBaseType(Type type, Type genericBaseType)
        {
            Type baseType = type.BaseType;

            while (baseType != null)
            {
                // Check if the base type is a generic type and matches the specified generic base type
                if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == genericBaseType)
                {
                    return true;
                }

                baseType = baseType.BaseType;
            }

            return false;
        }
    }
}