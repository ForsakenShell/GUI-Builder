/*
 * GenConversion.cs
 * 
 * Functions for conversion between generic types
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;

using System.Drawing;

using SDL2;
using SDL2ThinLayer;

using GUIBuilder;


/// <summary>
/// Description of GenConversion.
/// </summary>
public static class GenConversion
{
    
    public abstract class Converter
    {
        readonly Type from; // Type of the instance to convert.
        readonly Type to;   // Type that the instance will be converted to.
        
        // Internal, because we'll provide the only implementation...
        // ...that's also why we don't check if the arguments are null.
        internal Converter( Type from, Type to )
        {
            this.from = from;
            this.to = to;
        }
        
        public Type From { get { return this.from; } }
        public Type To { get { return this.to; } }
        
        public abstract object Convert( object obj );
    }
    
    // Sealed, because this is meant to be the only implementation.
    public sealed class Converter<TFrom, TTo> : Converter
    {
        Func<TFrom, TTo> converter; // Converter is strongly typed.
        
        public Converter( Func<TFrom, TTo> converter )
            : base( typeof( TFrom ), typeof( TTo ) ) // Can't send null types to the base.
        {
            if( converter == null )
                throw new ArgumentNullException( "converter", "Converter must not be null." );
            
            this.converter = converter;
        }
        
        public override object Convert( object obj )
        {
            if( !( obj is TFrom ) )
            {
                var msg = string.Format( "Object is not of the type {0}.", this.From.FullName );
                throw new ArgumentException( msg, "obj" );
            }
            
            // Can throw exception, it's ok.
            return this.converter.Invoke( (TFrom)obj );
        }
    }
    
    static readonly List<Converter>     Converters = new List<Converter>{
        new Converter<bool,string>( b => b.ToString() ),
        new Converter<string, bool>( s => s.InsensitiveInvariantMatch( "true" ) ),
        
        new Converter<int,string>( i => i.ToString() ),
        new Converter<string,int>( s => int.Parse( s ) ),
        
        new Converter<uint,string>( i => i.ToString() ),
        new Converter<string,uint>( s => uint.Parse( s ) ),
        
        new Converter<float,string>( f => f.ToString() ),
        new Converter<string,float>( s => float.Parse( s ) ),
        
        new Converter<double,string>( d => d.ToString() ),
        new Converter<string,double>( s => double.Parse( s ) ),
        
        new Converter<SDL.SDL_Point,string>( p => Extensions.ToString( p ) ),
        new Converter<string,SDL.SDL_Point>( s =>
            {
                SDL.SDL_Point p;
                s.TryParseSDLPoint( out p );
                return p;
            } ),
        
        new Converter<Point,string>( p => Extensions.ToString( p.ToSDLPoint() ) ),
        new Converter<string,Point>( s =>
            {
                SDL.SDL_Point p;
                s.TryParseSDLPoint( out p );
                return p.ToPoint();
            } ),
        
        new Converter<Size,string>( p => Extensions.ToString( p.ToSDLPoint() ) ),
        new Converter<string,Size>( s =>
            {
                SDL.SDL_Point p;
                s.TryParseSDLPoint( out p );
                return new Size( p.x, p.y );
            } )
    };
    
    public static bool TryCast<T>( this object obj, out T result )
    {
        if( obj is T )
        {
            result = (T)obj;
            return true;
        }
        
        // If it's null, we can't get the type.
        if( obj != null )
        {
            var converter = Converters.FirstOrDefault( c => ( c.From == obj.GetType() )&&( c.To == typeof( T ) ) );
            
            // Use the converter if there is one.
            if( converter != null )
                try
                {
                    result = (T)converter.Convert( obj );
                    return true;
                }
                catch( Exception )
                {
                    // Ignore - "Try*" methods don't throw exceptions.
                }
        }
        
        result = default( T );
        return false;
    }
    
}
