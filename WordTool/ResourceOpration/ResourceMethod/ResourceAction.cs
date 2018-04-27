using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace WordTool
{
    public class ResourceAction
    {
        //private static readonly string tableMappingResourceUrl = System.Windows.Forms.Application.StartupPath.Substring(0, System.Windows.Forms.Application.StartupPath.IndexOf("\\bin")) + "\\ResourceOpration\\Resource\\CustomResource.resx";
        private static readonly string tableMappingResourceUrl = System.Windows.Forms.Application.StartupPath + "\\CustomResource.resx";

        public static void RemoveAndAddNewResEntry(string resFileName, string key, string value)
        {
            bool isKeyExist = false;
            using (ResXResourceReader reader = new ResXResourceReader(resFileName))
            {
                using (ResXResourceWriter writer = new ResXResourceWriter(resFileName))
                {
                    IDictionaryEnumerator de = reader.GetEnumerator();
                    while (de.MoveNext())
                    {
                        if (!de.Entry.Key.ToString().Equals(key, StringComparison.InvariantCultureIgnoreCase))
                        {
                            writer.AddResource(de.Entry.Key.ToString(), de.Entry.Value.ToString());
                        }
                        else
                        {
                            isKeyExist = true;
                        }
                    }
                    if (!isKeyExist)
                    {
                        writer.AddResource(key, value);
                    }
                }
            }
        }

        public static void AddMapping()
        {
            var mappingDic = new Dictionary<string, string>();
            mappingDic.Add("TOEIC", "Language_TOEIC");
            mappingDic.Add("CET4", "Language_CET4");
            mappingDic.Add("CET6", "Language_CET6");
            mappingDic.Add("Custom", "Language_Custom");
            mappingDic.Add("Level1", "Language_Level1");
            mappingDic.Add("Level2", "Language_Level2");
            mappingDic.Add("Level3", "Language_Level3");
            mappingDic.Add("Other", "Language_Other");
            using (ResXResourceWriter writer = new ResXResourceWriter(tableMappingResourceUrl))
            {
                foreach (var item in mappingDic)
                {
                    writer.AddResource(item.Key, item.Value);
                }
            }
        }

        public static Dictionary<string, string> GetTableNameDictionary()
        {
            var result = new Dictionary<string, string>();
            using (ResXResourceReader reader = new ResXResourceReader(tableMappingResourceUrl))
            {
                IDictionaryEnumerator de = reader.GetEnumerator();
                while (de.MoveNext())
                {
                    //if (de.Entry.Key.ToString().Equals(value, StringComparison.InvariantCultureIgnoreCase))
                    //{
                    //    writer.AddResource(de.Entry.Key.ToString(), de.Entry.Value.ToString());
                    //}
                    if (!result.ContainsKey(de.Entry.Key.ToString()))
                    {
                        result.Add(de.Entry.Key.ToString(), de.Entry.Value.ToString());
                    }
                }
            }
            return result;
        }

        public static string GetTableNameBySelectedOne(string value)
        {
            using (ResXResourceReader reader = new ResXResourceReader(tableMappingResourceUrl))
            {
                IDictionaryEnumerator de = reader.GetEnumerator();
                while (de.MoveNext())
                {
                    //if (de.Entry.Key.ToString().Equals(value, StringComparison.InvariantCultureIgnoreCase))
                    //{
                    //    writer.AddResource(de.Entry.Key.ToString(), de.Entry.Value.ToString());
                    //}
                    if (de.Entry.Key.ToString().Equals(value, StringComparison.OrdinalIgnoreCase))
                    {
                        return de.Entry.Value.ToString();
                    }
                }
            }
            return string.Empty;
        }
    }
}
