using System;
using System.Collections;
using System.Collections.Generic;

namespace Frontier.Manifests
{
	public class Manifest
	{ }

	public class ManifestSeries<T> : Manifest, IList<T> where T : Manifest
	{
		private List<T> values = new();

		public T this[int index] { get => values[index]; set => values[index] = value; }

		public int Count => values.Count;

		public bool IsReadOnly => ((ICollection<T>)values).IsReadOnly;

		public static ManifestSeries<T> Create<K>(ICollection<K> objects, Converter<K, T> converter)
		{
			ManifestSeries<T> manifest = new()
			{
				values = new List<K>(objects).ConvertAll(converter)
			};

			return manifest;
		}

		public void Add(T item)
		{
			values.Add(item);
		}

		public void Clear()
		{
			values.Clear();
		}

		public bool Contains(T item)
		{
			return values.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			values.CopyTo(array, arrayIndex);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return values.GetEnumerator();
		}

		public int IndexOf(T item)
		{
			return values.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			values.Insert(index, item);
		}

		public bool Remove(T item)
		{
			return values.Remove(item);
		}

		public void RemoveAt(int index)
		{
			values.RemoveAt(index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return values.GetEnumerator();
		}
	}
}

