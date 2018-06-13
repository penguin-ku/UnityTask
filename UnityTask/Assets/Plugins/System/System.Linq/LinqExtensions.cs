using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Linq
{
    /// <summary>
    /// Linq扩展方法
    /// </summary>
    public static class LinqExtensions
    {
        /// <summary>
        /// 字典获取值
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="p_self"></param>
        /// <param name="p_key"></param>
        /// <param name="p_defaultValue"></param>
        /// <returns></returns>
        internal static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> p_self, TKey p_key, TValue p_defaultValue)
        {
            TValue local;
            if (p_self.TryGetValue(p_key, out local))
            {
                return local;
            }
            return p_defaultValue;
        }

        /// <summary>
        /// 子数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] destinationArray = new T[length];
            Array.Copy(data, index, destinationArray, 0, length);
            return destinationArray;
        }

        /// <summary>
        /// 切割数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static IList<T[]> Cut<T>(this T[] source, int size)
        {
            IList<T[]> list = new List<T[]>();
            int num = ((source.Length + size) - 1) / size;
            int length = source.Length;
            for (int i = 0; i < num; i++)
            {
                int num4 = size;
                if (i == (num - 1))
                {
                    num4 = length - (i * size);
                }
                T[] item = new T[num4];
                for (int j = 0; j < num4; j++)
                {
                    item[j] = source[(i * size) + j];
                }
                list.Add(item);
            }
            return list;
        }

        public class CommonEqualityComparer<T, V> : IEqualityComparer<T>
        {
            private Func<T, V> keySelector;
            private IEqualityComparer<V> comparer;

            public CommonEqualityComparer(Func<T, V> keySelector, IEqualityComparer<V> comparer)
            {
                this.keySelector = keySelector;
                this.comparer = comparer;
            }

            public CommonEqualityComparer(Func<T, V> keySelector)
                : this(keySelector, EqualityComparer<V>.Default)
            { }

            public bool Equals(T x, T y)
            {
                return comparer.Equals(keySelector(x), keySelector(y));
            }

            public int GetHashCode(T obj)
            {
                return comparer.GetHashCode(keySelector(obj));
            }
        }
        public static IEnumerable<T> Distinct<T, V>(this IEnumerable<T> source, Func<T, V> keySelector)
        {
            return source.Distinct(new CommonEqualityComparer<T, V>(keySelector));
        }

        public static IEnumerable<T> Distinct<T, V>(this IEnumerable<T> source, Func<T, V> keySelector, IEqualityComparer<V> comparer)
        {
            return source.Distinct(new CommonEqualityComparer<T, V>(keySelector, comparer));
        }
    }
}
