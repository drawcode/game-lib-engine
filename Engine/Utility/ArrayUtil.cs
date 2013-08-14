using System;
using System.Collections.Generic;

public static class ArrayUtil {

    /// <summary>
    /// Filter the given array into an array of a derived type
    /// </summary>
    /// <returns>
    /// A new array that contains only the items in src that
    /// are of the given derived type U
    /// </returns>
    /// <param name='src'>
    /// Array to filter
    /// </param>
    /// <typeparam name='T'>
    /// The item type of the array to filter
    /// </typeparam>
    /// <typeparam name='U'>
    /// The type, derived from T, to filter the array to
    /// </typeparam>
    ///
    public static U[] Filter<T, U>(T[] src)
        where U : T {
        var res = new List<U>();
        foreach (var item in src) {
            if (item is U)
                res.Add((U)item);
        }
        return res.ToArray();
    }

    /// <summary>
    /// Filter the given array into a list of a derived type
    /// </summary>
    /// <returns>
    /// A new list that contains only the items in src that
    /// are of the given derived type U
    /// </returns>
    /// <param name='src'>
    /// Array to filter
    /// </param>
    /// <typeparam name='T'>
    /// The item type of the array to filter
    /// </typeparam>
    /// <typeparam name='U'>
    /// The type, derived from T, to filter the array to
    /// </typeparam>
    ///
    public static List<U> FilterToList<T, U>(T[] src)
        where U : T {
        var res = new List<U>();
        foreach (var item in src) {
            if (item is U)
                res.Add((U)item);
        }
        return res;
    }
}