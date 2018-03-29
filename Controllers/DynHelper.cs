using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;

namespace DynService_v3
{
    internal class DynHelper
    {
        internal static object ParsedValue(object value, Logger log)
        {
            DateTime dateObj;
            Int32 intObj;
            Int64 bigIntObj;
            Decimal doubleObj;
            Guid guidObj;
            Type t = value.GetType();
            Dictionary<string, object> obj = new Dictionary<string, object>();
            if (value is Newtonsoft.Json.Linq.JObject)
            {
                
                Newtonsoft.Json.Linq.JObject j_value = (Newtonsoft.Json.Linq.JObject)value;
                obj = j_value.ToObject<Dictionary<string, object>>();
            }
            else if (value is Dictionary<string, object>)
            {
                obj = (Dictionary<string, object>)value;
            }
            if (obj.ContainsKey("string")) {
                log.Info("String");
                return Convert.ToString(obj["string"]);
            }
            else if (obj.ContainsKey("int"))
            {
                log.Info("Int32");
                return Convert.ToInt32(obj["int"]);
            }
            else if (obj.ContainsKey("double"))
            {
                log.Info("Double");
                return Convert.ToDouble(obj["double"]);
            }
            else if (obj.ContainsKey("optionsetvalue"))
            {
                log.Info("OptionSetValue");
                return new OptionSetValue(Convert.ToInt32(obj["optionsetvalue"]));
            }
            else if (obj.ContainsKey("entity"))
            {
                Newtonsoft.Json.Linq.JObject j_value = (Newtonsoft.Json.Linq.JObject)obj["entity"];
                Dictionary<string, object> entity = j_value.ToObject<Dictionary<string, object>>();
                log.Info("EntityReference");
                return new EntityReference(entity["name"].ToString(), new Guid(entity["id"].ToString()));
            }

            if(Guid.TryParse(value.ToString(), out guidObj))
            {
                log.Info("GUID");
                return guidObj;
            }
            

            if(Int32.TryParse(value.ToString(), out intObj))
            {
                log.Info("Int32");
                return Convert.ToInt32(value);
            }

            if(Int64.TryParse(value.ToString(), out bigIntObj))
            {
                log.Info("Int64");
                return Convert.ToInt64(value);
            }

            if (Decimal.TryParse(value.ToString(), out doubleObj))
            {
                log.Info("Double");
                return Convert.ToDouble(value);
            }

            if (DateTime.TryParse(value.ToString(), out dateObj))
            {
                log.Info("DateTime");
                return Convert.ToDateTime(value);
            }

            bool boolStr;
            if (Boolean.TryParse(value.ToString(), out boolStr))
            {
                log.Info("Boolean");
                return Convert.ToBoolean(value);
            }
            log.Info("String");
            return value.ToString();
        }
    }
}