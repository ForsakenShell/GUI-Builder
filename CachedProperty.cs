/*
 * CachedProperty.cs
 *
 * Cached Property class, allows "properties" to be cached.  This is intended for when the getter/setters are otherwise slow functions.
 *
 * NOTE:  Caching requires a bool and a fully qualifiable TValue.  This means that the program memory usage may drastically increase.
 *
 */

using System;

/// <summary>
/// Cached Property class, allows "properties" to be cached.  This is intended for when the getter/setters are otherwise slow functions.
/// NOTE:  Caching requires a bool and a fully qualifiable TValue.  This means that the program memory usage may drastically increase.
/// </summary>
public class CachedProperty< TValue >
{
    bool                                cached                      = false;
    TValue                             _value;
    
    readonly Func< TValue >             GetFunc;
    readonly Action< TValue >           SetAction;
    
    public                              CachedProperty( Func< TValue > getFunc, Action< TValue > setAction )
    {
        this.GetFunc = getFunc;
        this.SetAction = setAction;
    }
    
    public TValue                       Value
    {
        get
        {
            if( GetFunc == null ) throw new NotImplementedException();
            if( !cached )
                _value = GetFunc();
            cached = true;
            return _value;
        }
        set
        {
            if( SetAction == null ) throw new NotImplementedException();
            SetAction( value );
            _value = value;
            cached = true;
        }
    }
    
}

