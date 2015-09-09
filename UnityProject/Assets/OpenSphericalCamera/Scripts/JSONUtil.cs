using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class JSONUtil {

    public static void DictionaryToObjectFiled(IDictionary dictionary, object obj)
    {
        try
        {
            FieldInfo[] fields = obj.GetType().GetFields();

            foreach (var field in fields)
            {
                if (dictionary.Contains(field.Name))
                {
                    if (dictionary[field.Name].GetType() == typeof(Dictionary<string, object>))
                    {
                        DictionaryToObjectFiled((IDictionary)dictionary[field.Name], field.GetValue(obj));
                    }
                    else if (dictionary[field.Name].GetType() == typeof(List<object>))
                    {
                        object fldVal = field.GetValue(obj);

                        Type itemType = field.FieldType.GetGenericArguments()[0];

                        MethodInfo addMethod = fldVal.GetType().GetMethod("Add", new Type[] { itemType });

                        var list = (IList)dictionary[field.Name];

                        if (IsDefinedObject(itemType))
                        {
                            foreach(var e in list)
                            {
                                var definedObject = System.Activator.CreateInstance(itemType);

                                DictionaryToObjectFiled((IDictionary)e, definedObject);

                                addMethod.Invoke(fldVal, new object[] { definedObject });
                            }
                        }
                        else
                        {
                            foreach (var e in list)
                            {
                                addMethod.Invoke(fldVal, new object[] { e });
                            }
                        }
                    }
                    else
                    {
                        SetValue(obj, field, dictionary[field.Name]);
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogException(ex);

            throw new System.Exception("Parse Json Error");
        }
    }

    private static void SetValue(object obj, FieldInfo field, object value)
    {
        if (field.FieldType == typeof(bool))
        {
            field.SetValue(obj, (bool)value);
        }
        else if (field.FieldType == typeof(string))
        {
            field.SetValue(obj, (string)value);
        }
        else if (field.FieldType == typeof(long))
        {
            field.SetValue(obj, (long)value);
        }
        else if (field.FieldType == typeof(double))
        {
            field.SetValue(obj, (double)value);
        }
    }

    private static bool IsDefinedObject(Type type)
    {
        if (type == typeof(bool))
        {
            return false;
        }
        else if (type == typeof(string))
        {
            return false;
        }
        else if (type == typeof(long))
        {
            return false;
        }
        else if (type == typeof(double))
        {
            return false;
        }

        return true;
    }
}
