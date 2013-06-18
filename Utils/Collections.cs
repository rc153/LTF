using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using System.IO;
using SharpCompress.Reader;

namespace System.Collections.Generic
{
    public static class Synchrone
    {
        public static void ForEach<TSource>(IEnumerable<TSource> source, Action<TSource> body)
        {
            foreach (TSource item in source)
                body(item);
        }
    }

    public static class StringExtensions
    {
        public static string ReplaceAny(this string source, char[] toReplace, char byThis)
        {
            var hashtable = toReplace.ToDictionary(c => c);
            var result = source.Select(c => hashtable.ContainsKey(c) ? byThis : c);
            return String.Concat(result);
        }
    }

    public static class Stream2LinqExtension
    {
        public static IEnumerable<string> EnumerateAllLinesFromZip(this Stream stream)
        {
            try
            {
                IReader reader = ReaderFactory.Open(stream);
                reader.MoveToNextEntry();
                return reader.OpenEntryStream().EnumerateAllLines();
            }
            catch (InvalidOperationException)
            {
                return Enumerable.Empty<string>();
            }
        }

        public static IEnumerable<string> EnumerateAllLines(this Stream stream)
        {
            return new StreamReader(stream).EnumerateAllLines();
        }

        public static void WriteAllLines(this Stream stream, IEnumerable<string> data)
        {
            new StreamWriter(stream).WriteAllLines(data);
        }

        public static IEnumerable<string> EnumerateAllLines(this StreamReader reader)
        {
            string result = reader.ReadLine();
            while (result != null)
            {
                yield return result;
                result = reader.ReadLine();
            }
            if (reader.BaseStream.CanSeek) reader.BaseStream.Position = 0;
            else reader.Close();
        }

        public static void WriteAllLines(this StreamWriter writer, IEnumerable<string> data)
        {
            foreach (string line in data) writer.WriteLine(line);
            if (writer.BaseStream.CanSeek) writer.BaseStream.Position = 0;
            else writer.Close();
        }
    }

    // why all these WPF things are looking so old style?
    public interface INotifyPropertyChanged<out T>
    {
        event Action<T, string> PropertyChanged;
    }

    public interface INotifyCollectionChanged<out T1, out T2>
    {
        event Action<T1, T2, NotifyCollectionChangedAction> CollectionChanged;
    }

    // I don't understand the non generic ICollection interface on the queue and stack...
    // here are helpers if you want to use linked list instead
    public static class QueueStack
    {
        public static T GetAndRemoveFirst<T>(this LinkedList<T> source)
        {
            T result = source.First.Value;
            source.RemoveFirst();
            return result;
        }

        public static T GetAndRemoveLast<T>(this LinkedList<T> source)
        {
            T result = source.Last.Value;
            source.RemoveLast();
            return result;
        }
    }

    public static class ListUtil
    {
        public static IEnumerable<T> Sample<T>(this IList<T> source, int nb = 100)
        {
            Random r = new Random();
            for (int i = 0; i < nb; i++)
                yield return source[r.Next(source.Count)];
        }
    }

    public static class EnumerableUtils
    {
        public static IEnumerable<T> Sample<T>(this IEnumerable<T> source, double prob = 0.1)
        {
            Random r = new Random();
            foreach (var item in source)
                if (r.NextDouble() < prob)
                    yield return item;
        }

        public static IEnumerable<TSource> Generate<TSource>(TSource start, Func<TSource, TSource> step)
        {
            TSource current = start;
            while (true)
            {
                yield return current;
                current = step(current);
            }
        }

        public static IEnumerable<T> Even<T>(this IEnumerable<T> input)
        {
            int i = 1;
            foreach (T item in input)
            {
                if (i++ % 2 == 0)
                    yield return item;
            }
        }

        public static IEnumerable<T> Odd<T>(this IEnumerable<T> input)
        {
            int i = 1;
            foreach (T item in input)
            {
                if (i++ % 2 == 1)
                    yield return item;
            }
        }

        public static string Join<T>(this IEnumerable<T> enumerable, string glue)
        {
            return enumerable.Aggregate(new StringBuilder(), (sb, item) => sb.Length == 0 ? sb.Append(item) : sb.Append(glue).Append(item)).ToString();
        }

        public static IEnumerable<TResult> Map<T, TResult>(this IEnumerable<T> collection, Func<T, TResult> predicate)
        {
            return collection.Select(predicate);
        }

        public static TResult Reduce<TValue, TResult>(this IEnumerable<TValue> collection, TResult init, Func<TResult, TValue, TResult> predicate)
        {
            return collection.Aggregate(init, predicate);
        }

        public static IQueryable<R> MapReduce<S, M, K, R>(this IQueryable<S> source,
                                 Expression<Func<S, IEnumerable<M>>> mapper,
                                 Expression<Func<M, K>> keySelector,
                                 Expression<Func<K, IEnumerable<M>, R>> reducer)
        {
            return source.SelectMany(mapper).GroupBy(keySelector, reducer);
        }

        // untested
        public static IEnumerable<TResult> MapReduce<TSource, TMap, TKey, TResult>(this IEnumerable<TSource> source,
                                 Func<TSource, IEnumerable<TMap>> mapper,
                                 Func<TMap, TKey> keySelector,
                                 Func<TKey, IEnumerable<TMap>, TResult> reducer)
        {
            return source.AsParallel().SelectMany(mapper).GroupBy(keySelector, reducer);
        }
    }

    // seriously, how can they create a read only list and then call it read only collection, while interfaces are proper ???
    public class ReadOnlyCollection<T> : IReadOnlyCollection<T>
    {
        private ICollection<T> collection;

        public ReadOnlyCollection(ICollection<T> collection)
        {
            if (collection == null) throw new ArgumentNullException("collection");
            this.collection = collection;
        }

        public int Count
        {
            get { return collection.Count; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
