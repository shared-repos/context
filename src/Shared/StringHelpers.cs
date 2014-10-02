using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Context
{
    internal static class StringHelpers
    {
        public static string CreateCompoundIdentifier(params string[] parts)
        {
            return string.Join(".", parts);
        }

        public static string[] GetCompoundIdentifierParts(string identifier)
        {
            return identifier.Split('.');
        }

        public static string Join(string separator, params string[] strings)
        {
            return Join(separator, (IEnumerable)strings);
        }

        public static string Join(string separator, IEnumerable objects)
        {
            if (objects == null)
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder();
            foreach (object val in objects)
            {
                if (val == null)
                {
                    continue;
                }

                string str = val.ToString();
                if (!string.IsNullOrEmpty(str))
                {
                    sb.Append(str);
                    sb.Append(separator);
                }
            }

            int length = sb.Length;
            if (length == 0)
            {
                return string.Empty;
            }

            return sb.ToString(0, length - separator.Length);
        }

        public static Dictionary<string, string> ParseDictionary(string text)
        {
            return ParseDictionary(text, "=", ";");
        }

        public static Dictionary<string, string> ParseDictionary(string text, string valueSeparator, params string[] itemsSeparator)
        {
            string[] arr = text.Split(itemsSeparator, StringSplitOptions.None);
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach (string str in arr)
            {
                int i = str.IndexOf(valueSeparator);
                if (i < 0)
                {
                    if (str != string.Empty)
                    {
                        dictionary.Add(str, null);
                    }
                }
                else
                {
                    dictionary.Add(str.Substring(0, i), str.Substring(i + 1));
                }
            }

            return dictionary;
        }

        public static void ParseTypedDictionary(string text, Dictionary<string, string> items)
        {
            ParseTypedDictionary(text, items, "=", ";");
        }

        public static void ParseTypedDictionary(string text, Dictionary<string, string> items, string valueSeparator, string itemsSeparator)
        {
            Dictionary<string, int> map = new Dictionary<string, int>(items.Count);
            foreach (KeyValuePair<string, string> entry in items)
            {
                string prefix = entry.Key + valueSeparator;
                string key = itemsSeparator + prefix;
                int i = text.IndexOf(key);
                if (i >= 0)
                {
                    map[entry.Key] = i + key.Length;
                    continue;
                }

                if (text.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
                {
                    map[entry.Key] = prefix.Length;
                    continue;
                }
            }

            int len = text.Length;
            if (text.EndsWith(itemsSeparator, StringComparison.InvariantCultureIgnoreCase))
            {
                len -= itemsSeparator.Length;
            }

            foreach (KeyValuePair<string, int> entry in map)
            {
                int i = entry.Value;
                int j = len;
                foreach (KeyValuePair<string, int> item in map)
                {
                    if (item.Value <= j && item.Value > i)
                    {
                        j = item.Value - itemsSeparator.Length - item.Key.Length - valueSeparator.Length;
                    }
                }

                items[entry.Key] = text.Substring(i, j - i);
            }
        }

        public static bool IsDashSymbol(char ch)
        {
            char[] dashSymbols = new char[] {
                (char)0x2043, // hyphen bullet
                (char)0x2212 // minus
            };

            if (Char.GetUnicodeCategory(ch) == System.Globalization.UnicodeCategory.DashPunctuation)
            {
                return true;
            }

            foreach (char hyphen in dashSymbols)
            {
                if (ch == hyphen)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
