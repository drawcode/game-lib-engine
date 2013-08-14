using System;
using System.Collections.Generic;

public static class ListUtil {

    public static List<T> New<T>(params T[] vals) {
        return new List<T>(vals);
    }

    /// <summary>
    /// Filter the given list into a list of a derived type
    /// </summary>
    /// <returns>
    /// A new list that contains only the items in src that
    /// are of the given derived type U
    /// </returns>
    /// <param name='src'>
    /// List to filter
    /// </param>
    /// <typeparam name='T'>
    /// The item type of the list to filter
    /// </typeparam>
    /// <typeparam name='U'>
    /// The type, derived from T, to filter the list to
    /// </typeparam>
    ///
    public static List<U> Filter<T, U>(List<T> src)
        where U : T {
        var res = new List<U>();
        foreach (var item in src) {
            if (item is U)
                res.Add((U)item);
        }
        return res;
    }
}