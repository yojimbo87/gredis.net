using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Gredis.Client
{
    public class GDocument : Dictionary<string, object>
    {

        public long ID
        {
            get
            {
                return this.HasField("@id") ? this.GetField<long>("@id") : 0;
            }
            set { this.SetField("@id", value); }
        }

        public T GetField<T>(string fieldPath)
        {
            Type type = typeof(T);
            T value;

            if (type.IsPrimitive || type.IsArray || (type.Name == "String"))
            {
                value = default(T);
            }
            else
            {
                value = (T)Activator.CreateInstance(type);
            }

            if (fieldPath.Contains("."))
            {
                var fields = fieldPath.Split('.');
                int iteration = 1;
                GDocument embeddedDocument = this;

                foreach (var field in fields)
                {
                    if (iteration == fields.Length)
                    {
                        // if value is collection type, get element type and enumerate over its elements
                        if (value is IList)
                        {
                            Type elementType = ((IEnumerable)value).GetType().GetGenericArguments()[0];
                            IEnumerator enumerator = ((IEnumerable)embeddedDocument[field]).GetEnumerator();

                            while (enumerator.MoveNext())
                            {
                                // if current element is GDocument type which is dictionary<string, object>
                                // map its dictionary data to element instance
                                if (enumerator.Current is GDocument)
                                {
                                    var instance = Activator.CreateInstance(elementType);
                                    ((GDocument)enumerator.Current).MapTo(ref instance);

                                    ((IList)value).Add(instance);
                                }
                                else
                                {
                                    ((IList)value).Add(enumerator.Current);
                                }
                            }
                        }
                        else
                        {
                            value = (T)embeddedDocument[field];
                        }
                        break;
                    }

                    if (embeddedDocument.ContainsKey(field))
                    {
                        embeddedDocument = (GDocument)embeddedDocument[field];
                        iteration++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                if (this.ContainsKey(fieldPath))
                {
                    // if value is list or set type, get element type and enumerate over its elements
                    if (value is IList)
                    {
                        Type elementType = ((IEnumerable)value).GetType().GetGenericArguments()[0];
                        IEnumerator enumerator = ((IEnumerable)this[fieldPath]).GetEnumerator();

                        while (enumerator.MoveNext())
                        {
                            // if current element is GDocument type which is Dictionary<string, object>
                            // map its dictionary data to element instance
                            if (enumerator.Current is GDocument)
                            {
                                var instance = Activator.CreateInstance(elementType);
                                ((GDocument)enumerator.Current).MapTo(ref instance);

                                ((IList)value).Add(instance);
                            }
                            else
                            {
                                ((IList)value).Add(enumerator.Current);
                            }
                        }
                    }
                    else
                    {
                        value = (T)this[fieldPath];
                    }
                }
            }

            return value;
        }

        public GDocument SetField<T>(string fieldPath, T value)
        {
            if (fieldPath.Contains("."))
            {
                var fields = fieldPath.Split('.');
                int iteration = 1;
                GDocument embeddedDocument = this;

                foreach (var field in fields)
                {
                    if (iteration == fields.Length)
                    {
                        if (embeddedDocument.ContainsKey(field))
                        {
                            embeddedDocument[field] = value;
                        }
                        else
                        {
                            embeddedDocument.Add(field, value);
                        }
                        break;
                    }

                    if (embeddedDocument.ContainsKey(field))
                    {
                        embeddedDocument = (GDocument)embeddedDocument[field];
                    }
                    else
                    {
                        // if document which contains the field doesn't exist create it first
                        GDocument tempDocument = new GDocument();
                        embeddedDocument.Add(field, tempDocument);
                        embeddedDocument = tempDocument;
                    }

                    iteration++;
                }
            }
            else
            {
                if (this.ContainsKey(fieldPath))
                {
                    this[fieldPath] = value;
                }
                else
                {
                    this.Add(fieldPath, value);
                }
            }

            return this;
        }

        public bool HasField(string fieldPath)
        {
            bool contains = false;

            if (fieldPath.Contains("."))
            {
                var fields = fieldPath.Split('.');
                int iteration = 1;
                GDocument embeddedDocument = this;

                foreach (var field in fields)
                {
                    if (iteration == fields.Length)
                    {
                        contains = embeddedDocument.ContainsKey(field);
                        break;
                    }

                    if (embeddedDocument.ContainsKey(field))
                    {
                        embeddedDocument = (GDocument)embeddedDocument[field];
                        iteration++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                contains = this.ContainsKey(fieldPath);
            }

            return contains;
        }

        public T To<T>() where T : class, new()
        {
            T genericObject = new T();

            genericObject = (T)ToObject<T>(genericObject, "");

            return genericObject;
        }


        public void MapFrom(object obj)
        {
            if (obj is Dictionary<string, object>)
            {
                foreach (KeyValuePair<string, object> item in (Dictionary<string, object>)obj)
                {
                    this.SetField(item.Key, item.Value);
                }
            }
            else
            {
                Type objType = obj.GetType();

                foreach (PropertyInfo property in objType.GetProperties())
                {
                    this.SetField(property.Name, property.GetValue(obj));
                }
            }
        }

        private T ToObject<T>(T genericObject, string path) where T : class, new()
        {
            Type genericObjectType = genericObject.GetType();

            foreach (PropertyInfo propertyInfo in genericObjectType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                string fieldPath = path + (path != "" ? "." : "") + propertyInfo.Name;

                if ((propertyInfo.PropertyType.IsArray || propertyInfo.PropertyType.IsGenericType))
                {
                    if (this.HasField(fieldPath))
                    {
                        object propertyValue = this.GetField<object>(fieldPath);

                        IList collection = (IList)propertyValue;

                        if (collection.Count > 0)
                        {
                            // create instance of property type
                            object collectionInstance = Activator.CreateInstance(propertyInfo.PropertyType, collection.Count);

                            for (int i = 0; i < collection.Count; i++)
                            {
                                // collection is simple array
                                if (propertyInfo.PropertyType.IsArray)
                                {
                                    ((object[])collectionInstance)[i] = collection[i];
                                }
                                // collection is generic
                                else if (propertyInfo.PropertyType.IsGenericType && (propertyValue is IEnumerable))
                                {
                                    Type elementType = collection[i].GetType();

                                    // generic collection consists of basic types or ORIDs
                                    if (elementType.IsPrimitive ||
                                        (elementType == typeof(string)) ||
                                        (elementType == typeof(DateTime)) ||
                                        (elementType == typeof(decimal)))
                                    {
                                        ((IList)collectionInstance).Add(collection[i]);
                                    }
                                    // generic collection consists of generic type which should be parsed
                                    else
                                    {
                                        // create instance object based on first element of generic collection
                                        object instance = Activator.CreateInstance(propertyInfo.PropertyType.GetGenericArguments()[0], null);

                                        ((IList)collectionInstance).Add(ToObject(instance, fieldPath));
                                    }
                                }
                                else
                                {
                                    object v = Activator.CreateInstance(collection[i].GetType(), collection[i]);

                                    ((IList)collectionInstance).Add(v);
                                }
                            }

                            propertyInfo.SetValue(genericObject, collectionInstance, null);
                        }
                    }
                }
                // property is class except the string type since string values are parsed differently
                else if (propertyInfo.PropertyType.IsClass &&
                    (propertyInfo.PropertyType.Name != "String"))
                {
                    // create object instance of embedded class
                    object instance = Activator.CreateInstance(propertyInfo.PropertyType);

                    propertyInfo.SetValue(genericObject, ToObject(instance, fieldPath), null);
                }
                // property is basic type
                else
                {
                    if (this.HasField(fieldPath))
                    {
                        object propertyValue = this.GetField<object>(fieldPath);

                        propertyInfo.SetValue(genericObject, propertyValue, null);
                    }
                }
            }

            return genericObject;
        }

        // maps document fields to specified object
        // TODO: make it recursive
        private void MapTo(ref object obj)
        {
            if (obj is Dictionary<string, object>)
            {
                obj = this;
            }
            else
            {
                Type objType = obj.GetType();

                foreach (KeyValuePair<string, object> item in this)
                {
                    PropertyInfo property = objType.GetProperty(item.Key);

                    if (property != null)
                    {
                        property.SetValue(obj, item.Value, null);
                    }
                }
            }
        }
    }
}
