using System;
using System.Collections.Generic;
using System.Text;

namespace InvokerHost.Extensions
{
    public static class DictionaryExtensions
    {
        public static void Add<TKey, TVal>(this Dictionary<TKey, TVal> dict, KeyValuePair<TKey, TVal> item)
        {
            dict.Add(item.Key, item.Value);
        }

        public static void AddRange<TKey, TVal>(this Dictionary<TKey, TVal> dict, IEnumerable<KeyValuePair<TKey, TVal>> items)
        {
            foreach (var item in items)
            {
                dict.Add(item);
            }
        }
    }
}
