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
        public const int                    HeightmapSize               = 33;
        
        const string                        _XPath                      = "VHGT";

        const string                        _Offset                     = "Offset";
        const string                        _RowColumn                  = @"Rows\Row #{0}\Columns\Column #{1}";

        ElementHandle                       cached_Handle               = null;
        float[,]                            _Heightmap                  = null;

        public                              Heightmap( Form form ) : base( form, _XPath ) { }

        Engine.Plugin.Forms.Landscape       Landscape { get { return Form as Engine.Plugin.Forms.Landscape; } }

        void                                ReadHeightmap( TargetHandle target )
        {
            var h = Form.HandleFromTarget( target );
            if( cached_Handle == h ) return;

            //DebugLog.OpenIndentLevel();
            //h.DebugDumpChildElements( true );
            //DebugLog.CloseIndentLevel();

            if( Landscape == null ) throw new Exception( "Cannot cast Form as Landscape!" );
            var cell = Landscape.Cell;
            if( cell == null ) throw new Exception( "Landscape is not part of a Cell!" );
            var ws = cell.Worldspace;
            if( ws == null ) throw new Exception( "Landscape Cell is not part of a Worldspace!" );
            
            //DebugLog.OpenIndentLevel( "Loading heightmap for CELL " + cell.IDString + " Grid " + cell.CellGrid.GetGrid( target ).ToString() + " in WRLD " + ws.IDString, false );

            cached_Handle = h;

            // Calculate heightmap from combined landscape offsets and default worldspace land height

            var defaultLandHeight = ws.LandData.GetDefaultLandHeight( target );

            var hm = new float[ HeightmapSize, HeightmapSize ];

            float offset = ReadFloat( h, _Offset ) * _HeightmapScalar;
            float row_Offset = 0.0f;

            for( int row = 0; row < HeightmapSize; row++ )
            //for( int rc = 0; rc < _HeightmapSize * _HeightmapSize; rc++ )
            {
                //var tmp = "Row " + row.ToString();
                for( int col = 0; col < HeightmapSize; col++ )
                {
                    //var row = rc / _HeightmapSize;
                    //var col = rc % _HeightmapSize;
                    //hm[ row, col ] = ReadSByte( h, string.Format( _RowColumn, row, col ) ); // Raw value for testing
                    var value  = ReadSByte( h, string.Format( _RowColumn, row, col ) ) * _HeightmapScalar;
                    if( col == 0 )
                    {
                        row_Offset = 0.0f;
                        offset += value;
                    }
                    else
                    {
                        row_Offset += value;
                    }
                    hm[ col, row ] = defaultLandHeight + offset + row_Offset;
                    //tmp += " : " + col + "=" + hm[ col, row ];
                }
                //DebugLog.WriteLine( tmp );
            }

            //DebugLog.CloseIndentLevel();

            // Return finalized heightmap
            _Heightmap = hm;
        }

        public float[,]                     GetHeightmap( TargetHandle target )
        {
            ReadHeightmap( target );
            // Make a copy of the array so anyone writing to it doesn't wreck it
            var result = new float[ HeightmapSize, HeightmapSize ];
            for( int row = 0; row < HeightmapSize; row++ )
                for( int col = 0; col < HeightmapSize; col++ )
                    result[ col, row ] = _Heightmap[ col, row ];
            // Return the copy
            return result;
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
