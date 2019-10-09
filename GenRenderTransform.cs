/*
 * GenRenderTransform.cs
 * 
 * Generic functions for RenderTransform
 * 
 */
using System;
using System.Collections.Generic;


public static class GenRenderTransform
{
    
    #region Render Transform
    
    public static bool ReadyForUse( this GUIBuilder.Windows.RenderChild.RenderTransform transform )
    {
        return
            ( transform != null )&&
            ( transform.Renderer != null )&&
            ( transform.Renderer.IsReady );
    }
    
    #endregion
    
    public static Maths.Vector2i SizeOfLines( this List<string> list, SDL2ThinLayer.SDLRenderer.Font font, out int tallest )
    {
        int widest = -1;
        tallest = -1;
        foreach( var s in list )
        {
            if( !string.IsNullOrEmpty( s ) )
            {
                var fts = font.TextSize( s );
                widest  = Math.Max( widest , fts.Width  );
                tallest = Math.Max( tallest, fts.Height );
            }
        }
        return new Maths.Vector2i( widest, tallest * list.Count );
    }
    
}
