/*
 * GenStream.cs
 * 
 * Generic functions for steams
 * 
 */
using System;
using System.Linq;


/// <summary>
/// Description of GenStream.
/// </summary>
public static class GenStream
{
    
    #region Binary Stream Writer
    
    public static void WriteStringRaw( this System.IO.BinaryWriter stream, string str )
    {
        var count = str.Length;
        if( count < 1 ) return;
        var chars = new char[ count ];
        for( int i = 0; i < count; i++ )
            chars[ i ] = str[ i ];
        stream.Write( chars );
    }
    
    public static void WriteByteSizedZString( this System.IO.BinaryWriter stream, string str )
    {
        var count = (byte)( str.Length + 1 );
        stream.Write( count );
        if( count > 1 )
            stream.WriteStringRaw( str );
        stream.Write( (byte)0 );
    }
    
    public static void WriteUIntSizedString( this System.IO.BinaryWriter stream, string str )
    {
        var count = (uint)str.Length;
        stream.Write( count );
        if( count < 1 ) return;
        stream.WriteStringRaw( str );
    }
    
    #endregion
    
}
