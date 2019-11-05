/*
 * CachedGetSetIXHandle.cs
 *
 * Cached IXHandle get/set value class.  This is intended for time sensitive loops as XeLib is otherwise slow.
 *
 * NOTE:  Caching requires a handle reference and a fully qualifiable TValue.  This means that the program memory usage may drastically increase.
 *
 */

using System;
using XeLib;


namespace Engine.Plugin.Extensions
{
    /// <summary>
    /// Cached IXHandle get/set value class.  This is intended for time sensitive loops as XeLib is otherwise slow.
    /// NOTE:  Caching requires a handle reference and a fully qualifiable TValue.  This means that the program memory usage may drastically increase.
    /// </summary>
    public class CachedGetSetIXHandle< TValue > : IDisposable
    {
        
        public delegate TResult             GetFunctionDelegate<out TResult>( ElementHandle handle );
        public delegate void                SetActionDelegate<in TIn>( ElementHandle handle, TIn obj );
        
        ElementHandle                       cached_Handle               = null;
        TValue                             _value;
        
        GetFunctionDelegate< TValue >       GetFunc;
        SetActionDelegate< TValue >         SetAction;
        
        public                              CachedGetSetIXHandle( GetFunctionDelegate< TValue > getFunc, SetActionDelegate< TValue > setAction )
        {
            GetFunc = getFunc;
            SetAction = setAction;
        }
        
        public void                         Dispose()
        {
            cached_Handle = null;
            _value = default( TValue );
            GetFunc = null;
            SetAction = null;
        }
        
        public TValue                       GetValue( ElementHandle handle )
        {
            if( GetFunc == null ) throw new NotImplementedException();
            if( ( cached_Handle == null )||( cached_Handle != handle ) )
            {
                _value = GetFunc( handle );
                cached_Handle = handle;
            }
            return _value;
        }
        
        public void                         SetValue( ElementHandle handle, TValue value )
        {
            if( SetAction == null ) throw new NotImplementedException();
            SetAction( handle, value );
            _value = value;
            cached_Handle = handle;
        }
        
    }
    
}
