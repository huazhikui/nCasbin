using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using NetCasbin.Rabc;

namespace NetCasbin
{
    public static class BuiltInFunctions
    {
        /// <summary>
        /// key1是否匹配key2（类似RESTful路径），key2能包含*
        /// 例如："/foo/bar"匹配"/foo/*"
        /// </summary>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <returns></returns>
        public static Boolean KeyMatch(string key1, string key2)
        {
            int i = key2.IndexOf('*');

            if (i == -1)
            {
                return key1.Equals(key2);
            }

            if (key1.Length > i)
            {
                return key1.Substring(0, i).Equals(key2.Substring(0, i));
            }
            return key1.Equals(key2.Substring(0, i));
        }

        internal static bool ipMatch(string key1, string key2)
        {
            throw new NotImplementedException();
        }

        public static bool KeyMatch2(string key1, string key2)
        {
            key2 = key2.Replace("/*", "/.*");

            Regex regex = new Regex("(.*):[^/]+(.*)");

            while (true)
            {
                if (!key2.Contains("/:"))
                {
                    break;
                }

                key2 = regex.Replace(key2, "$1[^/]+$2");
            }
            return RegexMatch(key1, key2);
        }

        public static bool KeyMatch3(string key1, string key2)
        {
            key2 = key2.Replace("/*", "/.*");

            Regex regex = new Regex("(.*)\\{[^/]+\\}(.*)");
            while (true)
            {
                if (!key2.Contains("/{"))
                {
                    break;
                }

                key2 = regex.Replace(key2, "$1[^/]+$2");
            }
            return RegexMatch(key1, key2);
        }


        public static Boolean RegexMatch(String key1, String key2)
        {
            return Regex.Match(key1, key2).Success;
        }

        delegate bool GCall(string arg1, string arg2, string domain = null);
        internal static AbstractFunction GenerateGFunction(string name, IRoleManager rm)
        {
            bool Call(string arg1, string arg2, string domain = null)
            {
                if (rm == null)
                {
                    return arg1.Equals(arg2);
                }
                else
                {
                    bool res;
                    if (!String.IsNullOrEmpty(domain))
                    {
                        res = rm.HasLink(arg1, arg2, domain);
                        return res;
                    }

                    res = rm.HasLink(arg1, arg2);
                    return res;
                }
            }
            GCall call = Call;
            return new AviatorFunction(name, call);
        }
    }
}
