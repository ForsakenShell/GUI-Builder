/*
 * Heightmap.cs
 *
 * Landscape Heightmap field.
 *
 */

using System;
using XeLib;
using Engine.Plugin.Extensions;


namespace Engine.Plugin.Forms.Fields.Landscape
{
    
    public class Heightmap : RawField
    {

        const float                         _HeightmapScalar            = 8.0f;
        const int                           _HeightmapSize              = 33;
        
        const string                        _XPath                      = "VHGT";

        const string                        _Offset                     = "Offset";
        const string                        _RowColumn                  = @"Rows\Row #{0}\Columns\Column #{1}";

        ElementHandle                       cached_Handle               = null;
        float[,]                            _Heightmap                  = null;

        public                              Heightmap( Form form ) : base( form, _XPath ) { }

        /*
        public bool                         HasValue( TargetHandle target )
        {
            var h = cache_G.HandleFromTarget( target );
            if( !XeLib.HandleExtensions.IsValid( h ) )
                throw new System.ArgumentException( "target is not valid for field" );
            return cache_G.HasValue( h );
        }
        */

        Engine.Plugin.Forms.Landscape       Landscape { get { return Form as Engine.Plugin.Forms.Landscape; } }

        void                                ReadHeightmap( TargetHandle target )
        {
            var h = Form.HandleFromTarget( target );
            if( cached_Handle == h ) return;

            if( Landscape == null ) throw new Exception( "Cannot cast Form as Landscape!" );
            var cell = Landscape.Cell;
            if( cell == null ) throw new Exception( "Landscape is not part of a Cell!" );
            var ws = cell.Worldspace;
            if( ws == null ) throw new Exception( "Landscape Cell is not part of a Worldspace!" );
            
            cached_Handle = h;

            // Calculate heightmap from combined landscape offsets and default worldspace land height

            var defaultLandHeight = ws.LandData.GetDefaultLandHeight( target );

            var hm = new float[ _HeightmapSize, _HeightmapSize ];

            float offset = ReadFloat( h, _Offset ) * _HeightmapScalar;
            float row_Offset = 0.0f;

            for( int rc = 0; rc < _HeightmapSize * _HeightmapSize; rc++ )
            {
                var row    = rc / _HeightmapSize;
                var column = rc % _HeightmapSize;
                var value  = ReadSByte( h, string.Format( _RowColumn, row, column ) ) * _HeightmapScalar;
                if( column == 0 )
                {
                    row_Offset = 0.0f;
                    offset += value;
                }
                else
                {
                    row_Offset += value;
                }
                hm[ column, row ] = defaultLandHeight + offset + row_Offset;
            }

            // Return finalized heightmap
            _Heightmap = hm;
        }

        public float[,]                     GetHeightmap( TargetHandle target )
        {
            ReadHeightmap( target );
            return _Heightmap;
        }
        public void                         SetHeightmap( TargetHandle target, float[,] value )
        {
            throw new NotImplementedException();
        }
        
        public override string              ToString( TargetHandle target, string format = null )
        {
            return null;
        }
        
    }
    
}
