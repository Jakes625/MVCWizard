using Newtonsoft.Json;
using System;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Core.Utilities.Extensions
{
    public static class HttpUtilitiesExtensions
    {
        /// <summary>
        /// Casts a NameValueCollection (form) to either a model or type.
        /// </summary>
        /// <param name="Form"></param>
        /// <param name="type"></param>
        /// <param name="FormValueName"></param>
        /// <returns></returns>
        public static object CastForm(this NameValueCollection Form, Type type, string FormValueName = null)
        {
            object value = type.DefaultValue();
            try
            {
                if (!type.IsSimple())
                {
                    string json = JsonConvert.SerializeObject(Form.Cast<string>().Where(i => !string.IsNullOrEmpty(i)).ToDictionary(k => k, v => Form[v]));
                    value = JsonConvert.DeserializeObject(json, type);
                }
                else
                {
                    var FormValues = new Dictionary<string, object>();
                    Form.CopyTo(FormValues);

                    if (FormValues.ContainsKey(FormValueName))
                        value = FormValues[FormValueName];
                }
            }
            catch { }

            return value;
        }
    }
}
