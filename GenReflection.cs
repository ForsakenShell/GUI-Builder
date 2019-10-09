/*
 * GenReflection.cs
 * 
 * Generic functions for reflection
 * 
 * User: 1000101
 * Date: 27/11/2017
 * Time: 1:17 PM
 * 
 */
using System;
using System.IO;
using System.Collections.Generic;
//using System.Drawing;
//using System.Windows.Forms;
//using System.Runtime.InteropServices;
using System.Linq;
using System.Reflection;

/// <summary>
/// Description of GenReflection.
/// </summary>
public static class GenReflection
{
    
    #region Reflection Helpers
    
    public static bool HasInterface<TInterface>( this Type type )
    {
        var iType = typeof( TInterface );
        if( ( iType == null )||( !iType.IsInterface ) ) return false;
        var tName = iType.Name;
        return type.GetInterface( tName ) != null;
        
    }
    
    public static bool HasAttribute<TAttribute>( this MemberInfo memberInfo, bool includeSubClasses = true ) where TAttribute : Attribute
    {
        if( memberInfo == null ) return false;
        TAttribute result;
        return memberInfo.TryGetAttribute( out result, includeSubClasses );
    }
    
    public static bool TryGetAttribute<TAttribute>( this MemberInfo memberInfo, out TAttribute result, bool includeSubClasses = true ) where TAttribute : Attribute
    {
        result = null;
        if( memberInfo == null ) return false;
        var obj = memberInfo.GetCustomAttributes( typeof( TAttribute ), includeSubClasses ).FirstOrDefault();
        result = obj as TAttribute;
        return result != null;
    }
    
    #endregion
    
}
