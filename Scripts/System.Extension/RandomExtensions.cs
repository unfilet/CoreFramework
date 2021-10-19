using System;
using System.Linq;
using System.Collections.Generic;

public static class RandomExtensions
{
	private static readonly Random rand = new Random();

	public static int IndexOf<T>(this IList<T> list, Predicate<T> predicate)
	{
		for (int i = 0; i < list.Count; i++)
			if (predicate.Invoke(list[i]))
				return i;
		return -1;
	}

	/// <summary>
	/// Returns a random element from a list, or null if the list is empty.
	/// </summary>
	/// <typeparam name="T">The type of object being enumerated</typeparam>
	/// <returns>A random element from a list, or null if the list is empty</returns>
	public static T Random<T>(this IEnumerable<T> list)
	{
		return list.Random(rand);
	}

	/// <summary>
	/// Returns a random element from a list, or null if the list is empty.
	/// </summary>
	/// <typeparam name="T">The type of object being enumerated</typeparam>
	/// <param name="rand">An instance of a random number generator</param>
	/// <returns>A random element from a list, or null if the list is empty</returns>
	public static T Random<T>(this IEnumerable<T> list, Random rand)
	{
		if (list != null && list.Count() > 0)
			return list.ElementAt(rand.Next(list.Count()));
		return default(T);
	}
	
	/// <summary>
	/// Returns a shuffled IEnumerable.
	/// </summary>
	/// <typeparam name="T">The type of object being enumerated</typeparam>
	/// <returns>A shuffled shallow copy of the source items</returns>
	public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
	{
		return source.Shuffle(rand);
	}
	
	/// <summary>
	/// Returns a shuffled IEnumerable.
	/// </summary>
	/// <typeparam name="T">The type of object being enumerated</typeparam>
	/// <param name="rand">An instance of a random number generator</param>
	/// <returns>A shuffled shallow copy of the source items</returns>
	public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rand)
	{
		var list = source.ToList();
		list.Shuffle(rand);
		return list;
	}
	
	/// <summary>
	/// Shuffles an IList in place.
	/// </summary>
	/// <typeparam name="T">The type of elements in the list</typeparam>
	public static void Shuffle<T>(this IList<T> list)
	{
		list.Shuffle(rand);
	}
	
	/// <summary>
	/// Shuffles an IList in place.
	/// </summary>
	/// <typeparam name="T">The type of elements in the list</typeparam>
	/// <param name="rand">An instance of a random number generator</param>
	public static void Shuffle<T>(this IList<T> list, Random rand)
	{
		int count = list.Count;
		while (count > 1)
		{
			int i = rand.Next(count--);
			T temp = list[count];
			list[count] = list[i];
			list[i] = temp;
		}
	}

	public static IEnumerable<TSource> DistinctBy<TSource, TKey>
		(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
	{
		HashSet<TKey> knownKeys = new HashSet<TKey>();
		foreach (TSource element in source)
		{
			if (knownKeys.Add(keySelector(element)))
			{
				yield return element;
			}
		}
	}

	public static IList<T> Swap<T>(this IList<T> list, int indexA, int indexB)
	{
		T tmp = list[indexA];
		list[indexA] = list[indexB];
		list[indexB] = tmp;
		return list;
	}

}