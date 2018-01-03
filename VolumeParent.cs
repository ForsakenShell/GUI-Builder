/*
 * VolumeParent.cs
 *
 * Akin to the actual settlement, this groups all related BuildVolumes together and handles processing them as a group.
 *
 * User: 1000101
 * Date: 04/12/2017
 * Time: 11:12 AM
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
    /// <summary>
    /// Description of VolumeParent.
    /// </summary>
    public class VolumeParent
    {
        
        public int EditorID;
        public string FormID;
        public string VolumeKeyword;
        public Maths.Vector3f Position;
        public string Reference;
        public int WorldspaceEDID;
        
        public List<BuildVolume> BuildVolumes;
        public List<BorderNode> BorderNodes;
        
        #if DEBUG
        public
        #endif
        List<BorderSegment> BorderSegments;
        
        bool _welded = false;
        
        public VolumeParent( int editorID, string formID, string volumeKeyword, int worldspaceEDID  )
        {
            EditorID = editorID;
            FormID = formID;
            VolumeKeyword = volumeKeyword;
            WorldspaceEDID = worldspaceEDID;
            BuildVolumes = new List<BuildVolume>();
            BorderNodes = null;
            BorderSegments = null;
        }
        
        #region Bounding box and cells of entire build volume
        
        public Maths.Vector2f CornerNW
        {
            get
            {
                float minX = float.MaxValue;
                float maxY = float.MinValue;
                foreach( var buildVolume in BuildVolumes )
                {
                    foreach( var corner in buildVolume.Corners )
                    {
                        minX = Math.Min( minX, corner.X );
                        maxY = Math.Max( maxY, corner.Y );
                    }
                }
                return new Maths.Vector2f( minX, maxY );
            }
        }
        
        public Maths.Vector2f CornerSE
        {
            get
            {
                float maxX = float.MinValue;
                float minY = float.MaxValue;
                foreach( var buildVolume in BuildVolumes )
                {
                    foreach( var corner in buildVolume.Corners )
                    {
                        maxX = Math.Max( maxX, corner.X );
                        minY = Math.Min( minY, corner.Y );
                    }
                }
                return new Maths.Vector2f( maxX, minY );
            }
        }
        
        public Maths.Vector2i CellNW
        {
            get
            {
                var corner = CornerNW;
                var cell = bbGlobal.WorldSpaceToCellGrid( corner );
                if( ( corner.X < 0 )&&( cell.X * bbConstant.WorldMap_Resolution > corner.X ) )
                    cell.X--;
                //if( ( corner.X > 0 )&&( cell.X * bbConstant.WorldMap_Resolution < corner.X ) )
                //    cell.X++;
                //if( ( corner.Y < 0 )&&( cell.Y * bbConstant.WorldMap_Resolution > corner.Y ) )
                //    cell.Y--;
                if( ( corner.Y > 0 )&&( cell.Y * bbConstant.WorldMap_Resolution < corner.Y ) )
                    cell.Y++;
                return cell;
            }
        }
        
        public Maths.Vector2i CellSE
        {
            get
            {
                var corner = CornerSE;
                var cell = bbGlobal.WorldSpaceToCellGrid( corner );
                if( ( corner.X < 0 )&&( cell.X * bbConstant.WorldMap_Resolution > corner.X ) )
                    cell.X--;
                //if( ( corner.X > 0 )&&( cell.X * bbConstant.WorldMap_Resolution < corner.X ) )
                //    cell.X++;
                //if( ( corner.Y < 0 )&&( cell.Y * bbConstant.WorldMap_Resolution > corner.Y ) )
                //    cell.Y--;
                if( ( corner.Y > 0 )&&( cell.Y * bbConstant.WorldMap_Resolution < corner.Y ) )
                    cell.Y++;
                return cell;
            }
        }
        
        #endregion
        
        #region Corner Welding
        
        public bool Welded
        {
            get
            {
                return _welded;
            }
        }
        
        class WeldPoints
        {
            public int VolumeIndex;
            public int CornerIndex;
            
            public WeldPoints( int volumeIndex, int cornerIndex )
            {
                VolumeIndex = volumeIndex;
                CornerIndex = cornerIndex;
            }
        }
        
        public void WeldVerticies( float threshold )
        {
            if( _welded )
                return;
            
            for( int index = 0; index < BuildVolumes.Count; index++ )
            {
                WeldVolumeVerticies( index, threshold );
            }
            
            _welded = true;
        }
        
        List<WeldPoints> FindWeldableCorners( Maths.Vector2f origin, float threshold )
        {
            var points = new List<WeldPoints>();
            
            for( int index = 0; index < BuildVolumes.Count; index++ )
            {
                for( int index2 = 0; index2 < 4; index2++ )
                {
                    if( origin.DistanceFrom( BuildVolumes[ index ].Corners[ index2 ] ) < threshold )
                    {
                        points.Add( new WeldPoints( index, index2 ) );
                    }
                }
            }
            
            return points;
        }
        
        Maths.Vector2f WeldPoint( List<WeldPoints> points )
        {
            var result = new Maths.Vector2f();
            foreach( var point in points )
                result += BuildVolumes[ point.VolumeIndex ].Corners[ point.CornerIndex ];
            result /= points.Count;
            return result;
        }
        
        void WeldVolumeVerticies( int volumeIndex, float threshold )
        {
            var buildVolume = BuildVolumes[ volumeIndex ];
            for( int index = 0; index < 4; index++ )
            {
                var points = FindWeldableCorners( buildVolume.Corners[ index ], threshold );
                if( points.Count > 1 )
                {   // If it's only one point then it's itself
                    var weldPoint = WeldPoint( points );
                    foreach( var point in points )
                        BuildVolumes[ point.VolumeIndex ].Corners[ point.CornerIndex ] = weldPoint;
                }
            }
        }
        
        #endregion
        
        #region Build Border
        
        public void BuildBorders()
        {
            var worldspace = bbGlobal.WorldspaceFromEditorID( WorldspaceEDID );
            if( worldspace == null )
                return;
            
            DebugLog.Write( string.Format( "BuildBorders()\n{{\nVolume = {0}", FormID ) );
            
            // Create the lists
            BorderNodes = new List<BorderNode>();
            BorderSegments = new List<BorderSegment>();
            
            // Go through each volume and find all the segments
            int count = BuildVolumes.Count;
            for( int index = 0; index < count; index++ )
            {
                CreateSegmentsForVolume( index );
            }
            
            DebugLog.Write( string.Format( "BorderSegments.Count={0}\n{{", BorderSegments.Count.ToString() ) );
            for( int i = 0; i < BorderSegments.Count; i++ )
            {
                var segment = BorderSegments[ i ];
                segment.segIndex = i;
                DebugLog.Write( string.Format( "Segment: {0} : {1}", i, segment.ToString() ) );
            }
            DebugLog.Write( "}" );
            
            // Create outside edge
            var outsideSegments = OutsideEdgeSegments( BorderSegments );
            
            // Translate segments into border nodes
            if( !outsideSegments.NullOrEmpty() )
            {
                DebugLog.Write( string.Format( "outsideSegments.Count={0}\n{{", outsideSegments.Count.ToString() ) );
                for( int i = 0; i < outsideSegments.Count; i++ )
                {
                    var segment = outsideSegments[ i ];
                    DebugLog.Write( string.Format( "Segment: {0} : {1}", i, segment.ToString() ) );
                }
                DebugLog.Write( "}" );
                
            
                foreach( var segment in outsideSegments )
                    BorderNodes.Add( new BorderNode( segment ) );
            
                DebugLog.Write( string.Format( "BorderNodes.Count={0}\n{{", BorderNodes.Count.ToString() ) );
                for( int i = 0; i < BorderNodes.Count; i++ )
                {
                    var node = BorderNodes[ i ];
                    DebugLog.Write( string.Format( "Node: {0} : {1}", i, node.ToString() ) );
                }
                DebugLog.Write( "}" );
                
            }
            else
            {
                DebugLog.Write( "ERROR: No outsideSegments found!" );
            }
            
            // Segments no longer needed
            #if DEBUG
            #else
            BorderSegments = null;
            #endif
            DebugLog.Write( "}" );
        }
        
        List<BorderSegment> OutsideEdgeSegments( List<BorderSegment> segments )
        {
            const float vertex_threshold = 10f;
            
            DebugLog.Write( "OutsideEdgeSegments\n{" );
            var segmentCount = segments.Count;
            int startSegment = 0;
            for( int i = 1; i < segmentCount; i++ )
            {
                // Pick left-most P0
                if( segments[ i ].P0.X < segments[ startSegment ].P0.X )
                {
                    startSegment = i;
                }
                else if( ( segments[ i ].P0 - segments[ startSegment ].P0 ).Length.ApproximatelyEquals( 0f, vertex_threshold ) )
                {
                    // Found a matching P0, compare the cross products to find the segment that is left-most
                    var ip = segments[ i ].Normal;
                    var sp = segments[ startSegment ].Normal;
                    var cross = Maths.Vector2f.Cross( sp, ip );
                    DebugLog.Write( string.Format( "\tIndex: {0} ? {1} :: CrossProduct: {2}", startSegment, i, cross ) );
                    if( cross < 0f )
                    {
                        startSegment = i;
                    }
                }
            }
            
            int edgeIndex = 0;  // Used for debug logging only
            DebugLog.Write( string.Format( "edge {0} : {1}", edgeIndex, segments[ startSegment ].ToString() ) );
            
            var tmpLst = new List<BorderSegment>();
            tmpLst.Add( segments[ startSegment ] );
            var currentSegment = startSegment;
            do{
                int nextSegment = -1;
                for( int i = 0; i < segmentCount; i++ )
                {
                    // Skip self
                    if( i == currentSegment ) continue;
                    
                    // P0 of segment must match P1 of last segment
                    if( !( segments[ i ].P0 - segments[ currentSegment ].P1 ).Length.ApproximatelyEquals( 0f, vertex_threshold ) ) continue;
                    
                    // Led back to start, loop is complete at this point
                    if( i == startSegment )
                    {
                        nextSegment = i;
                        break;
                    }
                    
                    // Skip already used segments
                    if( tmpLst.Contains( segments[ i ] ) ) continue;
                    
                    // Set nextSegment if not already set and this segment is not colinear with the previous segment
                    if( nextSegment == -1 )
                    {
                        //var ip = segments[ i ].Normal;
                        //var cp = segments[ currentSegment ].Normal;
                        //var cross = Maths.Vector2f.Cross( cp, ip );
                        //DebugLog.Write( string.Format( "\tIndex: {0} -> {1} ? {2} = {4} :: CrossProduct: {3}", currentSegment, nextSegment, i, cross, cross > 0f ? i : nextSegment ) );
                        //if( !cross.ApproximatelyEquals( 0f ) )
                        {
                            DebugLog.Write( string.Format( "\tIndex: {0} -> {1} :: First Found", currentSegment, i ) );
                            nextSegment = i;
                        }
                        // Set or not, continue to the next possible segment
                        continue;
                    }
                    
                    {
                        var ip = segments[ i ].Normal;
                        var np = segments[ nextSegment ].Normal;
                        var cross = Maths.Vector2f.Cross( np, ip );
                        DebugLog.Write( string.Format( "\tIndex: {0} -> {1} ? {2} = {4} :: CrossProduct: {3}", currentSegment, nextSegment, i, cross, cross > 0f ? i : nextSegment ) );
                        if( cross < 0f )
                        {
                            nextSegment = i;
                        }
                    }
                }
                if( nextSegment < 0 )
                {
                    DebugLog.Write( string.Format( "ERROR:  NO EDGE FOUND LINKING FROM: {0}", segments[ currentSegment ].ToString() ) );
                    break;
                }
                if( nextSegment != startSegment )
                {
                    edgeIndex++;
                    DebugLog.Write( string.Format( "edge {0} : {1}", edgeIndex, segments[ nextSegment ].ToString() ) );
                    tmpLst.Add( segments[ nextSegment ] );
                }
                currentSegment = nextSegment;
            }while( currentSegment != startSegment );
            
            DebugLog.Write( "}" );
            
            // Didn't find a complete loop, wth?
            #if DEBUG
            #else
            if( currentSegment != startSegment )
                tmpLst = null;
            #endif
            
            return tmpLst;
        }
        
        bool SplitSegmentForIntersectWith( BorderSegment segment, Maths.Vector2f p0, Maths.Vector2f p1, out BorderSegment segmentTail )
        {
            DebugLog.Write( string.Format( "SplitSegmentForIntersectWith()\n\tsegment={0}\n\tp0={1}\n\tp1={2}\n{{", segment.ToString(), p0.ToString(), p1.ToString() ) );
            
            const float threshold = 1f;
            
            // Test results
            Maths.Vector2f result;
            segmentTail = null;
            
            // Test the points against the edges
            
            var collision = Maths.Geometry.Collision.LineLineIntersect(
                segment.P0, segment.P1,                         // Segment points
                p0, p1,                                         // Test edge
                out result,                                     // Intersection point
                threshold                                       // Threshold
               );
            if(
                ( collision == Maths.Geometry.CollisionType.NoCollision )|| // dur?
                ( collision == Maths.Geometry.CollisionType.VertexMatch )   // Ignore end-point matches
            )
            {
                DebugLog.Write( string.Format( "}}\tresult={0}", collision.ToString() ) );
                return false;
            }
            
            // TODO: Handle co-linear collisions more appropriately to reduce the final set of segments
            
            DebugLog.Write( string.Format( "intersection={0}", result.ToString() ) );
            
            // Clip the segment off at the intersection point
            var oldP1 = segment.P1;
            segment.P1 = new Maths.Vector2f( result );
            
            // Create a new segment, test if it's contained in another volume and if it's not then return the new segment as the tail of the original segment
            segmentTail = new BorderSegment( segment.Volume, result, oldP1 );
            DebugLog.Write( string.Format( "segmentTail={0}", segmentTail.ToString() ) );
            
            // Segment was split 
            DebugLog.Write( string.Format( "}}\tresult={1} - Segment split - segment={0}", segment.ToString(), collision.ToString() ) );
            return true;
        }
        
        void SplitSegmentForIntersections( BorderSegment segment )
        {
            DebugLog.Write( string.Format( "SplitSegmentForIntersections()\n\tsegment={0}\n{{", segment.ToString() ) );
            
            int count = BuildVolumes.Count;
            BorderSegment segmentTail;
            
            // Test against other volumes
            for( int index = 0; index < count; index++ )
            {
                // Only test against other volumes (so we don't eliminate ourself)
                if( index == segment.Volume )
                    continue;
                
                // Current volume and it's corners
                var volume = BuildVolumes[ index ];
                var corners = volume.Corners;
                
                // Test the points against the volumes edges
                DebugLog.Write( string.Format( "Volume={0}\nindex={1}\n{{", segment.Volume, index ) );
                
                // Edge 1 (0,1)
                DebugLog.Write( "Test edge 1\n{" );
                segmentTail = null;
                if( SplitSegmentForIntersectWith( segment, corners[ 0 ], corners[ 1 ], out segmentTail ) )
                    if( segmentTail != null )
                        SplitSegmentForIntersections( segmentTail );
                DebugLog.Write( "}" );
                
                // Edge 2 (1,2)
                DebugLog.Write( "Test edge 2\n{" );
                segmentTail = null;
                if( SplitSegmentForIntersectWith( segment, corners[ 1 ], corners[ 2 ], out segmentTail ) )
                    if( segmentTail != null )
                        SplitSegmentForIntersections( segmentTail );
                DebugLog.Write( "}" );
                
                // Edge 3 (2,3)
                DebugLog.Write( "Test edge 3\n{" );
                segmentTail = null;
                if( SplitSegmentForIntersectWith( segment, corners[ 2 ], corners[ 3 ], out segmentTail ) )
                    if( segmentTail != null )
                        SplitSegmentForIntersections( segmentTail );
                DebugLog.Write( "}" );
                
                // Edge 4 (3,0)
                DebugLog.Write( "Test edge 4\n{" );
                segmentTail = null;
                if( SplitSegmentForIntersectWith( segment, corners[ 3 ], corners[ 0 ], out segmentTail ) )
                    if( segmentTail != null )
                        SplitSegmentForIntersections( segmentTail );
                DebugLog.Write( "}" );
                
                DebugLog.Write( "}" );
            }
            
            BorderSegments.Add( segment );
                
            DebugLog.Write( "}" );
        }
        
        void TryAddBorderSegment( int vIndex, Maths.Vector2f p0, Maths.Vector2f p1 )
        {
            DebugLog.Write( string.Format( "TryAddBorderSegment()\n\tvIndex={0}\n\tp0={1}\n\tp1={2}\n{{", vIndex, p0.ToString(), p1.ToString() ) );
            
            // Create the segment and split it for intersection with any another volume
            var segment = new BorderSegment( vIndex, p0, p1 );
            SplitSegmentForIntersections( segment );
            
            DebugLog.Write( "}" );
        }
        
        void CreateSegmentsForVolume( int vIndex )
        {
            DebugLog.Write( string.Format( "CreateSegmentsForVolume()\n\tvIndex={0}\n{{", vIndex ) );
            
            // Get the build volume and it's corners
            var buildVolume = BuildVolumes[ vIndex ];
            var corners = buildVolume.Corners;
            
            // Try to add each edge as a border segment
            TryAddBorderSegment( vIndex, corners[ 0 ], corners[ 1 ] ); // Edge 1
            TryAddBorderSegment( vIndex, corners[ 1 ], corners[ 2 ] ); // Edge 2
            TryAddBorderSegment( vIndex, corners[ 2 ], corners[ 3 ] ); // Edge 3
            TryAddBorderSegment( vIndex, corners[ 3 ], corners[ 0 ] ); // Edge 4
            
            DebugLog.Write( "}" );
        }
        
       #endregion
        
    }
    
}
