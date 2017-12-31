/*
 * RenderTransform
 *
 * Handles all the specifics of rendering and scaling for the selected worldspace, import mod [and, parent volume]
 * respecting clipping and scaling.
 *
 * User: 1000101
 * Date: 19/12/2017
 * Time: 9:16 AM
 * 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Border_Builder
{
    
    public class RenderTransform
    {
        
        public enum Heightmap
        {
            Land,
            Water
        }
        
        // Source data pointers
        public bbWorldspace worldspace;
        public bbImportMod importMod;
        public VolumeParent specificVolume;
        
        // Clipper inputs held for reference
        public Maths.Vector2i cellNW;
        public Maths.Vector2i cellSE;
        public float scale;
        
        // Computed values from inputs
        public Maths.Vector2i hmCentre;
        
        // Size of selected region in cells
        public Maths.Vector2i cmSize;
        
        // Heightmap offset and size
        public Maths.Vector2i hmOffset;
        public Maths.Vector2i hmSize;
        
        // Worldmap offset and size
        public Maths.Vector2f wmOffset;
        public Maths.Vector2f wmSize;
        
        public Maths.Vector2f hmTransform;
        public Maths.Vector2f wmTransform;
        
        // Render target
        public Bitmap bmpTarget;
        public Graphics gfxTarget;
        public GraphicsUnit guTarget;
        public ImageAttributes iaTarget;
        public Rectangle rectTarget;
        
        public void Dispose( bool fullClean = false )
        {
            worldspace = null;
            importMod = null;
            specificVolume = null;
            cellNW = Maths.Vector2i.Zero;
            cellSE = Maths.Vector2i.Zero;
            scale = 0f;
            hmCentre = Maths.Vector2i.Zero;
            cmSize = Maths.Vector2i.Zero;
            hmOffset = Maths.Vector2i.Zero;
            hmSize = Maths.Vector2i.Zero;
            wmOffset = Maths.Vector2f.Zero;
            wmSize = Maths.Vector2f.Zero;
            hmTransform = Maths.Vector2f.Zero;
            wmTransform = Maths.Vector2f.Zero;
            gfxTarget.Dispose();
            if( fullClean )
                bmpTarget.Dispose(); 
            bmpTarget = null;
            gfxTarget = null;
            guTarget = (GraphicsUnit)0;
            iaTarget.Dispose();
            iaTarget = null;
            rectTarget = Rectangle.Empty;
        }
        
        // Constructor
        public RenderTransform( bbWorldspace _worldspace, bbImportMod _importMod, VolumeParent _specificVolume, Maths.Vector2i _cellNW, Maths.Vector2i _cellSE, float _scale )
        {
            // Shadow cache
            cellNW = _cellNW;
            cellSE = _cellSE;
            scale = _scale;
            
            worldspace = _worldspace;
            importMod = _importMod;
            specificVolume = _specificVolume;
            
            hmCentre = _worldspace.HeightMapOffset;
            
            // Compute
            cmSize = new Maths.Vector2i(
                Math.Abs( cellSE.X - cellNW.X ) + 1,
                Math.Abs( cellNW.Y - cellSE.Y ) + 1 );
            
            hmOffset = new Maths.Vector2i(
                hmCentre.X + cellNW.X * (int)bbConstant.HeightMap_Resolution,
                hmCentre.Y - cellNW.Y * (int)bbConstant.HeightMap_Resolution );
            hmSize = new Maths.Vector2i(
                cmSize.X * (int)bbConstant.HeightMap_Resolution,
                cmSize.Y * (int)bbConstant.HeightMap_Resolution );
            
            wmOffset = new Maths.Vector2f(
                (float)cellNW.X * bbConstant.WorldMap_Resolution,
                (float)cellNW.Y * bbConstant.WorldMap_Resolution );
            wmSize = new Maths.Vector2f(
                (float)cmSize.X * bbConstant.WorldMap_Resolution,
                (float)cmSize.Y * bbConstant.WorldMap_Resolution );
            
            hmTransform = new Maths.Vector2f(
                -cellNW.X * bbConstant.WorldMap_Resolution * scale,
                -cellSE.Y * bbConstant.WorldMap_Resolution * scale );
            
            wmTransform = new Maths.Vector2f(
                -wmOffset.X * scale,
                 wmOffset.Y * scale );
            
            rectTarget = new Rectangle(
                0,
                0,
                (int)( wmSize.X * scale ),
                (int)( wmSize.Y * scale ) );
            
            bmpTarget = new Bitmap( rectTarget.Width, rectTarget.Height );
            gfxTarget = Graphics.FromImage( bmpTarget );
            
            guTarget = GraphicsUnit.Pixel;
            iaTarget = new ImageAttributes();
            iaTarget.SetWrapMode( System.Drawing.Drawing2D.WrapMode.Clamp );
            
        }
        
        
        #region Render to targets
        
        public void RenderToPictureBox( PictureBox target )
        {
            if( target == null ) return;
            target.Size = new Size( rectTarget.Width, rectTarget.Height );
            target.Image = bmpTarget;
        }
        
        public void RenderToPNG()
        {
            // Export the [whole] map to a PNG
            var filename = specificVolume == null ? worldspace.FormID : specificVolume.FormID + ".png";
            bmpTarget.Save( filename, System.Drawing.Imaging.ImageFormat.Png );
        }
        
        #endregion
        
        #region Drawing Primitives (cellspace (heightmap) to transform target)
        
        public void DrawLineCellTransform( Pen pen, float x0, float y0, float x1, float y1 )
        {
            var r0 = hmTransform + new Maths.Vector2f( x0 * bbConstant.HeightMap_To_Worldmap * scale, y0 * bbConstant.HeightMap_To_Worldmap * scale );
            var r1 = hmTransform + new Maths.Vector2f( x1 * bbConstant.HeightMap_To_Worldmap * scale, y1 * bbConstant.HeightMap_To_Worldmap * scale );
            gfxTarget.DrawLine( pen, r0.X, r0.Y, r1.X, r1.Y );
        }
        
        public void DrawRectCellTransform( Pen pen, Maths.Vector2f p0, Maths.Vector2f p1 )
        {
            DrawLineCellTransform( pen, p0.X, p0.Y, p1.X, p0.Y );
            DrawLineCellTransform( pen, p1.X, p0.Y, p1.X, p1.Y );
            DrawLineCellTransform( pen, p1.X, p1.Y, p0.X, p1.Y );
            DrawLineCellTransform( pen, p0.X, p1.Y, p0.X, p0.Y );
        }
        
        #endregion
        
        #region Drawing Primitives (worldspace to transform target)
        
        public void DrawTextWorldTransform( string text, float x, float y, Pen pen, Font font = null, Brush brush = null )
        {
            if( font == null )
                font = new Font( FontFamily.GenericSansSerif, 12f, FontStyle.Regular, GraphicsUnit.Pixel );
            if( brush == null )
                brush = new SolidBrush( pen.Color );
            var rx = wmTransform.X + x * scale;
            var ry = wmTransform.Y - y * scale;
            gfxTarget.DrawString( text, font, brush, rx, ry );
        }
        
        public void DrawLineWorldTransform( Pen pen, float x0, float y0, float x1, float y1 )
        {
            var rx0 = wmTransform.X + x0 * scale;
            var ry0 = wmTransform.Y - y0 * scale;
            var rx1 = wmTransform.X + x1 * scale;
            var ry1 = wmTransform.Y - y1 * scale;
            gfxTarget.DrawLine( pen, rx0, ry0, rx1, ry1 );
        }
        
        public void DrawLineWorldTransform( Pen pen, Maths.Vector2f p0, Maths.Vector2f p1 )
        {
            DrawLineWorldTransform( pen, p0.X, p0.Y, p1.X, p1.Y );
        }
        
        public void DrawLineWorldTransform( Pen pen, Maths.Vector3f p0, Maths.Vector3f p1 )
        {
            DrawLineWorldTransform( pen, p0.X, p0.Y, p1.X, p1.Y );
        }
        
        public void DrawRectWorldTransform( Pen pen, Maths.Vector2f p0, Maths.Vector2f p1 )
        {
            DrawLineWorldTransform( pen, p0.X, p0.Y, p1.X, p0.Y );
            DrawLineWorldTransform( pen, p1.X, p0.Y, p1.X, p1.Y );
            DrawLineWorldTransform( pen, p1.X, p1.Y, p0.X, p1.Y );
            DrawLineWorldTransform( pen, p0.X, p1.Y, p0.X, p0.Y );
        }
        
        public void DrawRectWorldTransform( Pen pen, Maths.Vector3f p0, Maths.Vector3f p1 )
        {
            DrawLineWorldTransform( pen, p0.X, p0.Y, p1.X, p0.Y );
            DrawLineWorldTransform( pen, p1.X, p0.Y, p1.X, p1.Y );
            DrawLineWorldTransform( pen, p1.X, p1.Y, p0.X, p1.Y );
            DrawLineWorldTransform( pen, p0.X, p1.Y, p0.X, p0.Y );
        }
        
        public void DrawPolyWorldTransform( Pen pen, Maths.Vector2f[] p )
        {
            var last = p.Length - 1;
            for( int index = 0; index < p.Length - 1; index++ )
                DrawLineWorldTransform( pen, p[ index ], p[ index + 1 ] );
            DrawLineWorldTransform( pen, p[ last ], p[ 0 ] );
        }
        
        #endregion
        
        #region Heightmap Drawing
        
        public void DrawLandMap()
        {
            if( worldspace.LandHeight_Bitmap == null ) return;
            gfxTarget.DrawImage( worldspace.LandHeight_Bitmap, rectTarget, hmOffset.X, hmOffset.Y, hmSize.X, hmSize.Y, guTarget, iaTarget );
        }
        
        public void DrawWaterMap()
        {
            if( worldspace.WaterHeight_Bitmap == null ) return;
            gfxTarget.DrawImage( worldspace.WaterHeight_Bitmap, rectTarget, hmOffset.X, hmOffset.Y, hmSize.X, hmSize.Y, guTarget, iaTarget );
        }
        
        #endregion
        
        #region Build Volumes Drawing
        
        void DrawBuildVolume( VolumeParent volumeParent )
        {
            // Only show build volumes for the appropriate worldspace
            if( worldspace.EditorID != volumeParent.WorldspaceEDID )
                return;
            
            var pen = new Pen( Color.FromArgb( 255, 191, 0, 191 ) );
            
            foreach( var buildVolume in volumeParent.BuildVolumes )
                DrawPolyWorldTransform( pen, buildVolume.Corners );
            
        }
        
        public void DrawBuildVolumes()
        {
            if( specificVolume != null )
            {
                DrawBuildVolume( specificVolume );
            }
            else if( importMod != null )
                foreach( var volumeParent in importMod.VolumeParents )
                    DrawBuildVolume( volumeParent );
        }
        
        #endregion
        
        #region Parent Border Drawing
        
        void DrawParentBorder( VolumeParent volumeParent )
        {
            // Only show build volumes for the appropriate worldspace
            if( ( worldspace.EditorID != volumeParent.WorldspaceEDID )||( volumeParent.BorderNodes == null ) )
                return;
            
            //int r = 255;
            //int g = 0;
            //int b = 127;
            
            var pen = new Pen( Color.FromArgb( 255, 0, 255, 0 ) );
            
            for( var i = 0; i < volumeParent.BorderNodes.Count; i++ )
            {
                var node = volumeParent.BorderNodes[ i ];
                /*
                pen = new Pen( Color.FromArgb( 255, r, g, b ) );
                r -= 32;
                g += 32;
                b += 64;
                if( r <   0 ) r += 255;
                if( g > 255 ) g -= 255;
                if( b > 255 ) b -= 255;
                */
                var niP = ( node.A + node.B ) * 0.5f;
                DrawTextWorldTransform( i.ToString(), niP.X, niP.Y, pen );
                DrawLineWorldTransform( pen, node.A, node.B );
            }
            
        }
        
        public void DrawParentBorders()
        {
            if( specificVolume != null )
            {
                DrawParentBorder( specificVolume );
            }
            else if( importMod != null )
                foreach( var volumeParent in importMod.VolumeParents )
                    DrawParentBorder( volumeParent );
        }
        
        #endregion
        
        #region Cell Grid Drawing
        
        public void DrawCellGrid()
        {
            var pen = new Pen( Color.FromArgb( 127, 127, 127, 0 ) );
            
            for( int y = cellSE.Y; y <= cellNW.Y; y++ )
            {
                if( ( y >= worldspace.CellSE.Y )&&( y <= worldspace.CellNW.Y ) )
                {
                    for( int x = cellNW.X; x <= cellSE.X; x++ )
                    {
                        if( ( x >= worldspace.CellNW.X )&&( x <= worldspace.CellSE.X ) )
                        {
                            var p0 = new Maths.Vector2f(   x       * bbConstant.WorldMap_Resolution    ,   y       * bbConstant.WorldMap_Resolution     );
                            var p1 = new Maths.Vector2f( ( x + 1 ) * bbConstant.WorldMap_Resolution - 1, ( y - 1 ) * bbConstant.WorldMap_Resolution + 1 );
                            DrawRectWorldTransform( pen, p0, p1 );
                        }
                    }
                }
            }
        }
        
        #endregion
        
    }
}
