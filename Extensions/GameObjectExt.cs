﻿namespace Utilities
{
    using System;
    using System.Collections.Generic;

    using UnityEngine;

    public static class GameObjectExt
    {
        public static void SetMeshColor(this GameObject thisObject, Color color)
        {
            thisObject.GetComponent<MeshRenderer>().material.color = color;
        }

        public static T GetComponentAnywhere<T>(this GameObject gameObject) where T : Component
        {
            return gameObject.GetComponent<T>() ?? gameObject.GetComponentInChildren<T>(true);
        }

        public static T GetComponentWithInterface<T>(this GameObject thisObject) where T : class
        {
            Component[] components = thisObject.GetComponents<Component>();
            for (var i = 0; i < components.Length; i++)
            {
                if (typeof(T).IsAssignableFrom(components[i].GetType()))
                {
                    return components[i] as T;
                }
            }

            return null;
        }

        public static T[] GetComponentsWithInterface<T>(this GameObject thisObject) where T : class
        {
            var interfaces = new List<T>();
            Component[] components = thisObject.GetComponents<Component>();
            for (var i = 0; i < components.Length; i++)
            {
                var potentialImplementingTypes = new List<Type>();

                if (components[i] == null)
                {
                    continue;
                }

                Type componentType = components[i].GetType();
                potentialImplementingTypes.Add(componentType);
                IEnumerable<Type> parentTypes = componentType.GetParentTypes();
                potentialImplementingTypes.AddRange(parentTypes);

                foreach (Type type in potentialImplementingTypes)
                {
                    if (typeof(T).IsAssignableFrom(type))
                    {
                        interfaces.Add(components[i] as T);
                        break;
                    }
                }
            }

            return interfaces.ToArray();
        }

        public static T[] GetComponentsOfGenericType<T>(this GameObject thisObject) where T : class
        {
            Component[] components = thisObject.GetComponents<Component>();
            var componentsOfType = new List<T>();

            foreach (Component component in components)
            {
                if (ReflectionExt.IsAssignableToGenericType(component.GetType(), typeof(T)))
                {
                    componentsOfType.Add(component as T);
                }
            }

            return componentsOfType.ToArray();
        }

        public static T GetOrAddComponent<T>(this GameObject thisGameObject) where T : Component
        {
            var component = thisGameObject.GetComponent<T>();

            if (component == null)
            {
                component = thisGameObject.AddComponent<T>();
            }

            return component;
        }
    }
}
