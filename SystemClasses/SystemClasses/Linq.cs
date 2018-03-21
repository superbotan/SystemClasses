using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace System.Linq
{  
    public class DSO : Dictionary<string, object>
    { }
    public class LO : List<object>
    { }
    public static class Linq
    {
        /// <summary>
        /// Собирает все факты ошибок (идея: ошибок по идее быть не должно в принципе!)
        /// </summary>
        public static event Action<Exception> OnGlobalException;

        public static void OnErrorExecute(Exception ex)
        {
            OnGlobalException?.Invoke(ex);
        }

        /// <summary>
        /// try check every element with value with object.Equals()
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public static bool In<T>(this T value, params T[] elements)
        {
            bool res = false;
            if (elements != null)
            {
                for(int i = 0; i< elements.Length && !res; i++)
                {
                    res = res || object.Equals(value, elements[i]);
                }
            }
            return res;
        }
        public static bool Like(this string t, string s)
        {
            return t == s || t != null && s != null & t.IndexOf(s) >= 0;
        }

        public static Tuple<string, int> IndexOfAnyItem(this string str, string[] items, int start_index = 0, StringComparison sc = StringComparison.OrdinalIgnoreCase)
        {
            int index = -1;
            string item = null;

            foreach (var s in items)
            {
                if (s != null)
                {
                    int ix = str.IndexOf(s, start_index, sc);
                    if (ix > -1 && (index > ix || index == -1))
                    {
                        index = ix;
                        item = s;
                    }
                }
            }

            return new Tuple<string, int>(item, index);
        }

        public static bool ContainsAll(this string[] s, string[] t)
        {
            return !s.Distinct().Any(f => !t.Contains(f));
        }

        public static long TryParseLongOrDefault(this string value, long def = 0)
        {
            long res;
            if (long.TryParse(value, out res))
            {
                return res;
            }
            else
            {
                return def;
            }
        }


        public static T TryParseOrDefault<T>(this object o, T def = default(T))
        {
            if (o == null)
                return def;

            if (o is T)
            {
                return (T)o;
            }
            if (typeof(T) == typeof(long?) || typeof(T) == typeof(long))
            {
                return o is long ? (T)o : o is int ? (T)(object)(long)(int)o : long.TryParse(o.ToString(), out var r) ? (T)(object)r : def;
            }
            if (typeof(T) == typeof(bool?) || typeof(T) == typeof(bool))
            {
                return o is bool ? (T)o : bool.TryParse(o.ToString(), out var r) ? (T)(object)r : def;
            }
            if (typeof(T) == typeof(DateTime?) || typeof(T) == typeof(DateTime))
            {
                return o is DateTime ? (T)o : DateTime.TryParse(o.ToString(), out var r) ? (T)(object)r : def;
            }
            if (typeof(T) == typeof(TimeSpan?) || typeof(T) == typeof(TimeSpan))
            {
                return o is TimeSpan ? (T)o : TimeSpan.TryParse(o.ToString(), out var r) ? (T)(object)r : def;
            }
            if (typeof(T) == typeof(double?) || typeof(T) == typeof(double))
            {
                return o is decimal ? (T)(object)(double)(decimal)o : o is long ? (T)(object)(double)(long)o : o is int ? (T)(object)(double)(int)o : o is double ? (T)o : double.TryParse(o.ToString(), out var r) ? (T)(object)r : def;
            }
            if (typeof(T) == typeof(decimal?) || typeof(T) == typeof(decimal))
            {
                return o is decimal ? (T)o : o is long ? (T)(object)(decimal)(long)o : o is int ? (T)(object)(decimal)(int)o : o is double ? (T)(object)(decimal)(double)o : decimal.TryParse(o.ToString(), out var r) ? (T)(object)r : def;
            }

            return def;
        }


        public static string StripHtmlTagsUsingRegex(this string inputString)
        {
            if (inputString == null) return null;
            return Regex.Replace(inputString, @"<[^>]*>", String.Empty);
        }
        public static bool IsNullOrWhiteSpace(this string s)
        {
            return string.IsNullOrWhiteSpace(s);
        }
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        public static bool IsNullOrEmpty<T>(this List<T> s)
        {
            return s == null || s.Count == 0;
        }

        public static bool IsNullOrEmpty<T>(this T[] s)
        {
            return s == null || s.Length == 0;
        }

        public static T GetValueOrDefault<K, T>(this Dictionary<K, T> source, K key, T defValue = default(T))
        {
            T res;
            if (!source.TryGetValue(key, out res))
            {
                res = defValue;
            }
            return res;
        }

        public static bool AddIfNotExists<K, T>(this Dictionary<K, T> source, K key, T value)
        {
            if (source != null)
            {
                if (!source.ContainsKey(key))
                {
                    source.Add(key, value);
                    return true;
                }
            }
            return false;
        }
        public static bool AddOrUpdate<K, T>(this Dictionary<K, T> source, K key, T value)
        {
            if (source != null)
            {
                if (!source.ContainsKey(key))
                {
                    source.Add(key, value);
                    return true;
                }
                else
                {
                    source[key] = value;
                }
            }
            return false;
        }

        public static void Add<K, T>(this Dictionary<K, T> source, Dictionary<K, T> values)
        {
            foreach (var v in values)
            {
                source.Add(v.Key, v.Value);
            }
        }

        public static void AddOrUpdate<K, T>(this Dictionary<K, T> source, Dictionary<K, T> values)
        {
            foreach (var v in values)
            {
                source.AddOrUpdate(v.Key, v.Value);
            }
        }

        public static void AddIfNotExists<K, T>(this Dictionary<K, T> source, Dictionary<K, T> values)
        {
            foreach (var v in values)
            {
                source.AddIfNotExists(v.Key, v.Value);
            }
        }

        public static void ForEach<T>(this T[] source, Action<T> a)
        {
            if (source != null)
            {
                for (int i = 0; i < source.Length; i++)
                {
                    a(source[i]);
                }
            }
        }

        public static Dictionary<string, object> TryGetFromJson(this string json, Action<Exception> a_exc = null)
        {
            if (json == null)
            {
                return new Dictionary<string, object>();
            }
            try
            {
                object s = JsonConvert.DeserializeObject(json);
                object o = GetFromDeserilize(s);
                if (o is Dictionary<string, object>)
                {
                    return o as Dictionary<string, object>;
                }
                else
                {
                    Dictionary<string, object> v = new Dictionary<string, object>() { { "", o } };
                    return v;
                }
            }
            catch (Exception ex)
            {
                a_exc?.Invoke(ex);
                OnGlobalException?.Invoke(ex);

                return new Dictionary<string, object>();
            }
        }

        public static void ForEach<T>(this T[] source, Action<T> a_first, Action<T> a)
        {
            if (source != null)
            {
                for (int i = 0; i < source.Length; i++)
                {
                    if (i == 0)
                    { a_first(source[i]); }
                    else
                    { a(source[i]); }
                }
            }
        }

        private static object GetFromDeserilize(object s)
        {
            if (s == null)
            {
                return null;
            }
            else if (s is Newtonsoft.Json.Linq.JObject)
            {
                DSO res = new DSO();
                var j_obj = s as Newtonsoft.Json.Linq.JObject;
                foreach (var jtoken in j_obj.Children())
                {
                    res.Add(((Newtonsoft.Json.Linq.JProperty)jtoken).Name
                        , GetFromDeserilize(((Newtonsoft.Json.Linq.JProperty)jtoken).Value));
                }
                return res;
            }
            else if (s is Newtonsoft.Json.Linq.JArray)
            {
                var j_obj = s as Newtonsoft.Json.Linq.JArray;
                LO obj = new LO();
                foreach (var jtoken in j_obj.Children())
                {
                    obj.Add(GetFromDeserilize(jtoken));
                }
                return obj;
            }
            else if (s is Newtonsoft.Json.Linq.JValue)
            {
                var j_obj = s as Newtonsoft.Json.Linq.JValue;
                return j_obj.Value;
            }
            else
            {
                throw new Exception("unknown json type");
            }
        }

        public static string TryGetJson(this object obj, string root_name = null, Action<Exception> a_exc = null)
        {
            if (root_name == null)
            {
                try
                {
                    var s = JsonConvert.SerializeObject(obj);
                    return s;
                }
                catch (Exception ex)
                {
                    a_exc?.Invoke(ex);
                    OnGlobalException?.Invoke(ex);
                    return null;
                }
            }
            else
            {
                Dictionary<string, object> obj_dict = new Dictionary<string, object>() { { root_name, obj } };
                return TryGetJson(obj_dict, a_exc);
            }
        }
        public static string TryGetJson(this Dictionary<string, object> obj, Action<Exception> a_exc = null)
        {
            try
            {
                var s = JsonConvert.SerializeObject(obj ?? new Dictionary<string, object>());
                return s;
            }
            catch (Exception ex)
            {
                a_exc?.Invoke(ex);
                OnGlobalException?.Invoke(ex);
                return null;
            }
        }

        public static bool ContainsElement(this Dictionary<string, object> obj, params string[] p)
        {
            if (obj == null || p.Length == 0 || p == null)
            {
                return false;
            }
            bool b = obj.ContainsKey(p[0]);
            if (!b)
            {
                return b;
            }
            else if (p.Length == 1)
            {
                return b;
            }
            else if (!(obj[p[0]] is Dictionary<string, object>))
            {
                return false;
            }
            else
            {
                var p2 = p.ToList();
                p2.RemoveAt(0);
                b = ((Dictionary<string, object>)obj[p[0]]).ContainsElement(p2.ToArray());
                return b;
            }
        }

        public static T? IfDefault<T>(this T obj, T? obj2)
            where T : struct
        {
            return obj.Equals(default(T)) ? obj2 : obj;
        }

        public static string IfDefault(this string obj, string obj2)
        {
            return obj == null ? obj2 : obj;
        }
        public static object GetElement_DO(this object obj, params string[] p)
        {
            return GetElement<object>(obj as Dictionary<string, object>, p);
        }
        public static T GetElement_DO<T>(this object obj, params string[] p)
        {
            return GetElement<T>(obj as Dictionary<string, object>, p);
        }
        public static object GetElement(this Dictionary<string, object> obj, params string[] p)
        {
            return obj.GetElement<object>(p);
        }
        public static T GetElement<T>(this Dictionary<string, object> obj, params string[] p)
        {
            if (obj == null || p.Length == 0 || p == null || !obj.ContainsKey(p[0]))
            {
                return default(T);
            }
            object o = obj[p[0]];
            if (p.Length == 1)
            {
                if (typeof(T) == typeof(object))
                {
                    return (T)o;
                }
                else
                {
                    if (o == null)
                        return default(T);

                    if (typeof(T) == typeof(long?))
                    {
                        return o is long ? (T)o : o is int ? (T)o : default(T);
                    }
                    if (typeof(T) == typeof(bool?))
                    {
                        return o is bool ? (T)o : default(T);
                    }
                    if (typeof(T) == typeof(DateTime?))
                    {
                        return o is DateTime ? (T)o : default(T);
                    }
                    if (typeof(T) == typeof(TimeSpan?))
                    {
                        return o is TimeSpan ? (T)o : default(T);
                    }
                    if (typeof(T) == typeof(decimal?))
                    {
                        return o is decimal ? (T)o : o is long ? (T)o : o is int ? (T)o : default(T);
                    }

                    return o is T ? (T)o : default(T);
                }
            }
            else if (!(o is Dictionary<string, object>))
            {
                return default(T);
            }
            else
            {
                var p2 = p.ToList();
                p2.RemoveAt(0);
                T val = ((Dictionary<string, object>)o).GetElement<T>(p2.ToArray());
                return val;
            }
        }
        public static void SetElement<T>(this Dictionary<string, object> obj, T value, params string[] p)
        {
            if (obj == null || p.Length == 0 || p == null)
            {
                return;
            }
            if (p.Length == 1)
            {
                obj.AddOrUpdate(p[0], value);
            }
            else
            {
                object o = obj.ContainsKey(p[0]) ? obj[p[0]] : null;
                if (!(o is Dictionary<string, object>))
                {
                    o = new DSO();
                    obj.AddOrUpdate(p[0], o);
                }
                var p2 = p.ToList();
                p2.RemoveAt(0);
                (o as Dictionary<string, object>).SetElement(value, p2.ToArray());
            }            
        }
    }

}
