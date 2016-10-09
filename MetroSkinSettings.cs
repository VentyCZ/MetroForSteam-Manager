using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroSkinToolkit
{
    class MetroSkinSettings
    {
        #region Properties
        private Dictionary<string, string> props = new Dictionary<string, string>();

        public void SetProperty(string key, string value)
        {
            key = key.ToLower().Replace(" ", "_");

            if (props.ContainsKey(key))
            {
                props[key] = value;
            }
            else
            {
                props.Add(key, value);
            }
        }

        public string GetProperty(string key)
        {
            key = key.ToLower().Replace(" ", "_");

            if (props.ContainsKey(key))
            {
                return props[key];
            }
            else
            {
                return string.Empty;
            }
        }

        public bool GetPropertyBool(string propName)
        {
            propName = propName.ToLower().Replace(" ", "_");

            if (props.ContainsKey(propName))
            {
                return (props[propName].ToLower() == "true" || props[propName] == "1");
            }
            else
            {
                return false;
            }
        }
        #endregion

        public void Load(string basePath)
        {
            //using (StreamReader sm = new StreamReader())
            //{

            //}
        }
    }
}
