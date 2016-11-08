
using System;
using System.IO;

public static class ObjectUtil {
    
    public static T FindObject<T>()
        where T : UnityEngine.Object {
        return UnityEngine.Object.FindObjectOfType(typeof(T)) as T;
    }

    public static T[] FindObjects<T>()
        where T : UnityEngine.Object {
        return UnityEngine.Object.FindObjectsOfType(typeof(T)) as T[];
    }

    /*
    public U GetFieldValue<T, U>(T obj, string fieldName) {
        
        object val = null;
        
        if (Has(obj, fieldName)) {
            
            string type = attributes[fieldName];
            
            if (type == "dict") {
                if (typeof(BaseDataObject).IsAssignableFrom(obj.GetType())) {                                         
                    if (((BaseDataObject)obj).ContainsKey(fieldName)) {                        
                        val = ((BaseDataObject)obj)[fieldName];
                    } 
                }
            }
            else if (type == "field") {
                System.Reflection.FieldInfo field = obj.GetType().GetField(fieldName);
                if (field != null) {
                    val = field.GetValue(obj);
                }
            }
            else if (type == "property") {
                System.Reflection.PropertyInfo prop = obj.GetType().GetProperty(fieldName);                
                if (prop != null) {
                    if (prop.Name == fieldName) {
                        val = prop.GetValue(obj, null);
                    }
                }
            }
        }
        
        try { 
            return (U)val;
        }
        catch (Exception e) {
            LogUtil.Log(e);
            return default(U);
        }
    }
    
    public void SetFieldValue<T>(static T obj, string fieldName, object fieldValue) {
        //bool hasSet = false;
        
        if (Has(obj, fieldName)) {
            string type = attributes[fieldName];
            if (type == "dict") {
                if (typeof(BaseDataObject).IsAssignableFrom(obj.GetType())) {                                         
                    if (((BaseDataObject)obj).ContainsKey(fieldName)) {                        
                        ((BaseDataObject)obj)[fieldName] = fieldValue;
                    }   
                }
            }
            else if (type == "field") {
                System.Reflection.FieldInfo field = obj.GetType().GetField(fieldName);
                if (field != null) {
                    field.SetValue(obj, fieldValue);
                }
            }
            else if (type == "property") {
                System.Reflection.PropertyInfo prop = obj.GetType().GetProperty(fieldName);                
                if (prop != null) {
                    if (prop.Name == fieldName) {
                        prop.SetValue(obj, fieldValue, null);
                    }
                }
            }
        }
    }
    */
}