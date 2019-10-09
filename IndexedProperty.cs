/*
 * IndexedProperty.cs
 *
 * Indexed Property class, allows "properties" to use indexers.
 *
 */

using System;


/// <summary>
/// Indexed Property class, allows "properties" to use indexers.
/// </summary>
public class IndexedProperty< TIndex, TValue >
{
    
    readonly Action< TIndex, TValue > SetAction;
    readonly Func< TIndex, TValue > GetFunc;
    
    public IndexedProperty( Func< TIndex, TValue > getFunc, Action< TIndex, TValue > setAction )
    {
        this.GetFunc = getFunc;
        this.SetAction = setAction;
    }
    
    public TValue this[ TIndex index ]
    {
        get
        {
            if( GetFunc == null ) throw new NotImplementedException();
            return GetFunc( index );
        }
        set
        {
            if( SetAction == null ) throw new NotImplementedException();
            SetAction( index, value );
        }
    }
    
}

