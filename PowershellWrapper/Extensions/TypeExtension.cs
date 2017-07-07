using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Reflection;

namespace Powershell.Extensions
{
    internal static class TypeExtension
    {
        internal static void UpdateObjectProperties<T>(this T target, Collection<PSObject> source, bool overwrite)
        {
            if (source != null && source.Count > 0)
            {
                var obj = source[0];
                var sourceType = obj.GetType();
                var targetType = typeof(T);
                var props = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var p in props)
                {
                    var sourceProp = obj.Properties[p.Name];
                    if (sourceProp != null)
                    {
                        if (sourceProp.Value != null)
                        {
                            object value = sourceProp.Value is PSObject ? sourceProp.Value.ToString() : sourceProp.Value;
                            var sourceValue = Convert.ChangeType(value, p.PropertyType);
                            var targetValue = p.GetValue(target);
                            if (targetValue == null || overwrite)
                            {
                                p.SetValue(target, sourceValue);
                            }
                        }
                    }
                }
            }
        }

        internal static T PSConvert<T>(this PSObject obj)
        {
            var type = typeof(T);
            var TObj = Activator.CreateInstance(type);

            var targetProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in targetProperties)
            {
                var sourceProp = obj.Properties[prop.Name];
                if (sourceProp != null)
                {
                    var sourcePropType = Type.GetType(sourceProp.TypeNameOfValue);
                    if (sourcePropType != null && sourcePropType.IsGenericType)
                    {
                        if (sourcePropType.IsValueType && sourcePropType.Name == "Nullable`1")
                        {
                            var value = CreateObjectFromType(sourceProp.Value, sourcePropType, prop.PropertyType);
                            prop.SetValue(TObj, value);
                        }
                        else
                        {
                            var genValue = Convert.ChangeType(sourceProp.Value, sourcePropType);
                            var value = CreateObjectFromGenericType(genValue, sourcePropType, prop.PropertyType);
                            prop.SetValue(TObj, value);
                        }
                    }
                    else
                    {
                        if (sourceProp.Value is PSObject)
                        {
                            prop.SetValue(TObj, Convert.ChangeType(sourceProp.Value.ToString(), prop.PropertyType));
                        }
                        else
                        {
                            if (sourceProp.Value != null)
                            {
                                prop.SetValue(TObj, Convert.ChangeType(sourceProp.Value, prop.PropertyType));
                            }
                        }
                    }

                }
            }

            return (T)TObj;
        }

        internal static object CreateObjectFromType(object input, Type sourceType, Type targetType)
        {
            if (input == null)
                return null;

            if (sourceType.IsValueType && sourceType.Name == "Nullable`1")
            {
                return GetValueFromNullableType(input, sourceType, targetType);
            }
            else if (targetType.IsValueType || targetType.IsPrimitive)
            {
                return Convert.ChangeType(input, targetType);
            }
            else if (sourceType.IsClass && sourceType.Name == "String")
            {
                return sourceType.ToString();
            }
            else
            {
                return CreateObjectFromClassType(input, sourceType, targetType);
            }
        }

        private static object GetValueFromNullableType(object input, Type sourceType, Type targetType)
        {
            var hasValue = (bool)sourceType.GetProperty("HasValue").GetValue(input);
            if (hasValue)
            {
                var value = sourceType.GetProperty("Value").GetValue(input);
                return Convert.ChangeType(value, targetType);
            }
            else
            {
                return Activator.CreateInstance(targetType);
            }
        }

        private static object CreateObjectFromClassType(object input, Type sourceType, Type targetType)
        {
            var targetObject = Activator.CreateInstance(targetType);
            foreach (var targetProp in targetType.GetProperties())
            {
                object value = null;
                var sourceProp = sourceType.GetProperty(targetProp.Name, BindingFlags.Instance | BindingFlags.Public);
                if (sourceProp != null)
                {
                    if (sourceProp.PropertyType.IsGenericType || sourceProp.PropertyType.IsArray)
                    {
                        if (sourceProp.PropertyType.IsValueType && sourceProp.PropertyType.Name == "Nullable`1")
                        {
                            value = CreateObjectFromType(sourceProp.GetValue(input), sourceProp.PropertyType, targetProp.PropertyType);
                        }
                        else
                        {
                            value = CreateObjectFromGenericType(sourceProp.GetValue(input), sourceProp.PropertyType, targetProp.PropertyType);
                        }
                    }
                    else if (sourceProp.PropertyType.IsClass && sourceProp.PropertyType.Name != "String" && targetProp.PropertyType.Name != "String")
                    {
                        value = CreateObjectFromType(sourceProp.GetValue(input), sourceProp.PropertyType, targetProp.PropertyType);
                    }
                    else
                    {
                        value = sourceProp.GetValue(input);
                    }

                    if (value != null)
                    {
                        targetProp.SetValue(targetObject, Convert.ChangeType(value, targetProp.PropertyType));
                    }
                }
            }
            return targetObject;
        }

        private static object CreateObjectFromGenericType(object input, Type sourceType, Type targetType)
        {
            if (input == null)
                return null;

            var sourceItemType = sourceType.GetGenericArguments()[0];
            var targetItemType = targetType.GetGenericArguments()[0];

            var resultObj = Activator.CreateInstance(targetType);
            var propValue = (IList)Convert.ChangeType(input, sourceType);

            foreach (var item in propValue)
            {
                var value = CreateObjectFromType(item, sourceItemType, targetItemType);
                if (value != null)
                {
                    var addMethod = targetType.GetMethod("Add", BindingFlags.Instance | BindingFlags.Public);
                    addMethod.Invoke(resultObj, new object[] { value });
                }
            }

            return resultObj;
        }

    }
}
