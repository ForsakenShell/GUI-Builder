/*
 * GenCollection.cs
 * 
 * Generic functions for collections (arrays, lists, etc)
 * 
 * User: 1000101
 * Date: 27/11/2017
 * Time: 1:17 PM
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;


/// <summary>
/// Description of GenCollection.
/// </summary>
public static class GenCollection
{
    
    #region List Helpers
    
    public static int FindIndex<T>( this T[] array, T item )
    {
        for( int i = 0; i < array.Length; i++ )
            if( item.Equals( array[ i ] ) )
                return i;
        return -1;
    }
    
    /// <summary>
    /// Is the list null or contain 0 elements?
    /// </summary>
    /// <param name="list">List to test for null or empty</param>
    /// <returns></returns>
    public static bool NullOrEmpty<T>( this IList<T> list )
    {
        return ( list == null )||( list.Count == 0 );
    }

    public static bool Contains<T>( this List<T> list, Predicate<T> match )
    {
        if( list.NullOrEmpty() ) return false;
        if( match == null ) return false;
        return list.FindIndex( match ) >= 0;
    }
    
    /// <summary>
    /// Does the list contain all elements of the other list?
    /// </summary>
    /// <param name="list">The list to check the contents of</param>
    /// <param name="other">The other list with the items to check</param>
    /// <returns></returns>
    public static bool ContainsAllElementsOf<T>( this IList<T> list, IList<T> other )
    {
        if( list.NullOrEmpty() ) return false;
        if( other.NullOrEmpty() ) return false;
        foreach( var item in other )
            if( !list.Contains( item ) ) return false;
        return true;
    }
    
    public static List<T> Clone<T>( this IList<T> list )
    {
        if( list.NullOrEmpty() ) return null;
        var l = new List<T>();
        foreach( var i in list )
            l.Add( i );
        return l;
    }
    
    public static bool AddOnce<T>( this IList<T> list, T item )
    {
        if( list == null ) return false;
        if( list.Contains( item ) ) return false;
        list.Add( item );
        return true;
    }
    
    public static bool AddOnce<T>( this IList<T> list, IList<T> items )
    {
        bool result = false;
        if( list == null ) return result;
        if( items.NullOrEmpty() ) return result;
        foreach( var item in items )
            result |= list.AddOnce( item );
        return result;
    }
    
    public static void AddAll<T>( this IList<T> list, IList<T> items )
    {
        if( list == null ) return;
        if( items.NullOrEmpty() ) return;
        foreach( var item in items )
            list.Add( item );
    }
    
    #endregion
    
}
