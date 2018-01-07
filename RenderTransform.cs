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
        bbWorldspace worldspace;
        bbImportMod importMod;
        public List<VolumeParent> renderVolumes;
        
        // Clipper inputs held for reference
        Maths.Vector2i cellNW;
        Maths.Vector2i cellSE;
        float scale;
        
        #if DEBUG
        
        // Debug render options
        public bool debugRenderBuildVolumes;
        public bool debugRenderBorders;
        
        #endif
        
        // Computed values from inputs
        Maths.Vector2i hmCentre;
        
        // Size of selected region in cells
        Maths.Vector2i cmSize;
        
        // Heightmap offset and size
        Maths.Vector2i hmOffset;
        Maths.Vector2i hmSize;
        
        // Worldmap offset and size
        Maths.Vector2f wmOffset;
        Maths.Vector2f wmSize;
        
        Maths.Vector2f hmTransform;
        Maths.Vector2f wmTransform;
        
        // Render target
        Bitmap bmpTarget;
        Graphics gfxTarget;
        GraphicsUnit guTarget;
        ImageAttributes iaTarget;
        Rectangle rectTarget;
        PictureBox pbTarget;
        
        // High light parents and volumes
        public List<VolumeParent> hlParents = null;
        public List<BuildVolume> hlVolumes = null;
        
        #region cTor, dTor, sortOf
        
        public void Dispose()
        {
            worldspace = null;
            importMod = null;
            renderVolumes = null;
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
            bmpTarget.Dispose(); 
            iaTarget.Dispose();
            bmpTarget = null;
            gfxTarget = null;
            guTarget = (GraphicsUnit)0;
            iaTarget = null;
            rectTarget = Rectangle.Empty;
        }
        
        public RenderTransform( PictureBox _pbTarget, bbWorldspace _worldspace, bbImportMod _importMod )
        {
            pbTarget = _pbTarget;
            worldspace = _worldspace;
            importMod = _importMod;
        }
        
        #endregion
        
        #region Render State Manipulation
        
        public void UpdateTransform( Maths.Vector2i _cellNW, Maths.Vector2i _cellSE, float _scale )
        {
            bool mustRebuildBuffers = ( bmpTarget == null )||( gfxTarget == null )||( iaTarget == null );
            
            cellNW = _cellNW;
            cellSE = _cellSE;
            scale = _scale;
            hmCentre = worldspace.HeightMapOffset;
            
            // Compute
            cmSize = bbGlobal.SizeOfCellRange( cellNW, cellSE );
            
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
            
            var rect = new Rectangle(
                0,
                0,
                (int)( wmSize.X * scale ),
                (int)( wmSize.Y * scale ) );
            
            if( rect != rectTarget )
            {
                rectTarget = rect;
                mustRebuildBuffers = true;
            }
            
            if( !mustRebuildBuffers ) return;
            
            bool doGC = false;
            if( gfxTarget != null )
            {
                gfxTarget.Dispose();
                gfxTarget = null;
                doGC = true;
            }
            if( bmpTarget != null )
            {
                bmpTarget.Dispose(); 
                bmpTarget = null;
                doGC = true;
            }
            if( iaTarget != null )
            {
                iaTarget.Dispose();
                iaTarget = null;
            }
            
            if( doGC )
                System.GC.Collect( System.GC.MaxGeneration );

            bmpTarget = new Bitmap( rectTarget.Width, rectTarget.Height );
            gfxTarget = Graphics.FromImage( bmpTarget );
            
            guTarget = GraphicsUnit.Pixel;
            iaTarget = new ImageAttributes();
            iaTarget.SetWrapMode( System.Drawing.Drawing2D.WrapMode.Clamp );
        }
        
        #endregion
        
        #region Public Accessors
        
        public bbWorldspace                     Worldspace              { get { return worldspace; } }
        public bbImportMod                      ImportMod               { get { return importMod; } }
        
        public Maths.Vector2i                   CellNW                  { get { return cellNW; } }
        public Maths.Vector2i                   CellSE                  { get { return cellSE; } }
        
        public List<VolumeParent>               HighlightParents        { get { return hlParents; } }
        public List<BuildVolume>                HighlightVolumes        { get { return hlVolumes; } }
        
        #endregion
        
        public static float CalculateScale( Maths.Vector2i cells, Maths.Vector2i worldCells )
        {
            var scaleNS = (float)( (float)cells.Y / (float)worldCells.Y );
            var scaleEW = (float)( (float)cells.X / (float)worldCells.X );
            return Maths.InverseLerp( bbConstant.MinZoom, bbConstant.MaxZoom, scaleNS > scaleEW ? scaleNS : scaleEW );
        }
        
        public Maths.Vector2f MouseToWorldspace( float mX, float mY )
        {
            var sX = ( mX / scale );
            var sY = ( mY / scale );
            var wX = ( (float)cellNW.X * bbConstant.WorldMap_Resolution ) + sX;
            var wY = ( (float)cellNW.Y * bbConstant.WorldMap_Resolution ) - sY;
            return new Maths.Vector2f( wX, wY );
        }
        
        public Maths.Vector2i MouseToCellGrid( float mX, float mY )
        {
            var sX = ( mX / scale );
            var sY = ( mY / scale );
            var wX = cellNW.X + (int)( sX / bbConstant.WorldMap_Resolution );
            var wY = cellNW.Y - (int)( sY / bbConstant.WorldMap_Resolution );
            return new Maths.Vector2i( wX, wY );
        }
        
        #region Render Scene
        
        public void RenderScene( bool renderLand, bool renderWater, bool renderCellGrid, bool renderBuildVolumes, bool renderBorders )
        {
            if( renderLand )
                DrawLandMap();
            
            if( renderWater )
                DrawWaterMap();
            
            if( renderCellGrid )
                DrawCellGrid();
            
            if( renderBuildVolumes )
                DrawBuildVolumes();
            
            if( renderBorders )
                DrawParentBorders();
            
            pbTarget.Size = new Size( rectTarget.Width, rectTarget.Height );
            pbTarget.Image = bmpTarget;
        }
        
        public void RenderToPNG()
        {
            // Export the [whole] map to a PNG
            if(
                ( !renderVolumes.NullOrEmpty() )&&
                ( renderVolumes.Count > 1 )
               ) return;
            var filename = renderVolumes.NullOrEmpty() ? worldspace.FormID : renderVolumes[ 0 ].FormID + ".png";
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
        
        public void DrawTextWorldTransform( string text, float x, float y, Pen pen, float size = 12f, Font font = null, Brush brush = null )
        {
            if( font == null )
                font = new Font( FontFamily.GenericSerif, size, FontStyle.Regular, GraphicsUnit.Pixel );
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
        
        //public bool debugRenderBuildVolumes;
        //public bool debugRenderBorders;
        
        void DrawBuildVolume( VolumeParent volumeParent )
        {
            // Only show build volumes for the appropriate worldspace
            if( worldspace.EditorID != volumeParent.WorldspaceEDID )
                return;
            
            #if DEBUG
            if(
                ( !debugRenderBuildVolumes )||
                ( volumeParent.BorderSegments.NullOrEmpty() )
            )
            #endif
            {
                int rb = 223;
                var penHigh = new Pen( Color.FromArgb( 255, rb, 0, rb ) );
                rb -= 48;
                var penMid = new Pen( Color.FromArgb( 255, rb, 0, rb ) );
                rb -= 48;
                var penLow = new Pen( Color.FromArgb( 255, rb, 0, rb ) );
                
                foreach( var buildVolume in volumeParent.BuildVolumes )
                {
                    var usePen = penLow;
                    if( hlParents.NullOrEmpty() )
                    {
                        usePen = penHigh;
                    }
                    else if(
                        ( !hlVolumes.NullOrEmpty() )&&
                        ( hlVolumes.Contains( buildVolume ) )
                    )
                    {
                        usePen = penHigh;
                    }
                    else if( hlParents.Contains( volumeParent ) )
                    {
                        usePen = penMid;
                    }
                
                    DrawPolyWorldTransform( usePen, buildVolume.Corners );
                }
            }
            #if DEBUG
            else
            {
                int r = 255;
                int g = 0;
                int b = 127;
                
                for( var i = 0; i < volumeParent.BorderSegments.Count; i++ )
                {
                    var segment = volumeParent.BorderSegments[ i ];
                    
                    var pen = new Pen( Color.FromArgb( 255, r, g, b ) );
                    
                    r -= 32;
                    g += 32;
                    b += 64;
                    if( r <   0 ) r += 255;
                    if( g > 255 ) g -= 255;
                    if( b > 255 ) b -= 255;
                    
                    var niP = ( segment.P0 + segment.P1 ) * 0.5f;
                    DrawTextWorldTransform( i.ToString(), niP.X, niP.Y, pen, 10f );
                    DrawLineWorldTransform( pen, segment.P0, segment.P1 );
                }
            }
            #endif
        }
        
        public void DrawBuildVolumes()
        {
            if( renderVolumes.NullOrEmpty() )
            {
                foreach( var volumeParent in importMod.VolumeParents )
                    DrawBuildVolume( volumeParent );
            }
            else
            {
                foreach( var volumeParent in renderVolumes )
                    DrawBuildVolume( volumeParent );
            }
        }
        
        #endregion
        
        #region Parent Border Drawing
        
        void DrawParentBorder( VolumeParent volumeParent )
        {
            // Only show build volumes for the appropriate worldspace
            if( ( worldspace.EditorID != volumeParent.WorldspaceEDID )||( volumeParent.BorderNodes == null ) )
                return;
            
            int g = 255;
            if(
                ( !hlParents.NullOrEmpty() )&&
                ( !hlParents.Contains( volumeParent ) )
            )   g >>= 1;
            
            var pen = new Pen( Color.FromArgb( 255, 0, g, 0 ) );
            
            for( var i = 0; i < volumeParent.BorderNodes.Count; i++ )
            {
                var node = volumeParent.BorderNodes[ i ];
                
                /*
                if(
                    ( !hlVolumes.NullOrEmpty() )&&
                    ( hlVolumes.Contains( node.Volume ) )
                )   g >>= 1;
                */
                
               DrawLineWorldTransform( pen, node.A, node.B );
                
                #if DEBUG
                if( debugRenderBorders )
                {
                    var niP = ( node.A + node.B ) * 0.5f;
                    DrawTextWorldTransform( i.ToString(), niP.X, niP.Y, pen );
                }
                #endif
            }
            
        }
        
        public void DrawParentBorders()
        {
            if( renderVolumes.NullOrEmpty() )
            {
                foreach( var volumeParent in importMod.VolumeParents )
                    DrawParentBorder( volumeParent );
            }
            else
            {
                foreach( var volumeParent in renderVolumes )
                    DrawParentBorder( volumeParent );
            }
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
