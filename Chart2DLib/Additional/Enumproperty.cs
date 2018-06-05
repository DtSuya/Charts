using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace Readearth.Chart2D.Additional
{
    public static class EnumProperty
    {

        #region 获取描述
        //获取枚举值对应的描述
        public static string GetDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes =
                    (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
        }

        //获取list中出现的枚举值的描述
        public static List<string> GetDescriptions<T>(List<Enum> envalue)
        {
            Type type = typeof(T);
            List<String> deslist = new List<String>();

            foreach (Enum value in envalue)
            {
                FieldInfo x = type.GetField(value.ToString());
                DescriptionAttribute[] attributes =
                        (DescriptionAttribute[])x.GetCustomAttributes(typeof(DescriptionAttribute), false);
                deslist.Add((attributes.Length > 0) ? attributes[0].Description : value.ToString());
            }

            return deslist;
        }
        
        //获取此枚举类型所有描述
        public static List<string> GetDescriptions<T>()
        {
            Type type = typeof(T);
            List<String> deslist = new List<String>();

            foreach (FieldInfo x in type.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                DescriptionAttribute[] attributes =
                    (DescriptionAttribute[])x.GetCustomAttributes(typeof(DescriptionAttribute), false);
                deslist.Add((attributes.Length > 0) ? attributes[0].Description : x.Name);
            }

            return deslist;
        }

        #endregion 

        //从描述获取枚举值
        public static Enum GetEnumfromDes<T>(string description)
        {
            Type type = typeof(T);
            string[] names = Enum.GetNames(type);
            foreach (string name in names)
            {
                if (GetDescription((Enum)Enum.Parse(type, name)) == description)
                    return (Enum)Enum.Parse(type, name);
            }
            return null;

        }

        public static List<string> GetNames<T>(List<Enum> envalue)
        {
            Type type = typeof(T);
            List<string> itemnames = new List<string>();
            foreach (Enum value in envalue)
            {
                string name = Enum.GetName(type, value);
                if(name != null)
                    itemnames.Add(name);
            }
            return itemnames;
        }

        public static List<string> GetAllNames<T>()
        {
            Type type = typeof(T);
            List<string> itemnames = new List<string>();
            string[] names = Enum.GetNames(type);

            foreach (string name in names)
            {
                itemnames.Add(name);
            }
            return itemnames;
        }
    }
}
