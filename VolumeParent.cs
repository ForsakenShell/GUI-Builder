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
        
        #region Debug dumps
        
        void DumpSegmentList( string listName, List<BorderSegment> list )
        {
            #if DEBUG
            DebugLog.Write( string.Format( "{0}\n{{\nCount = {1}", listName, list.Count ) );
            for( int i = 0; i < list.Count; i++ )
            {
                var segment = list[ i ];
                segment.segIndex = i;
                DebugLog.Write( string.Format( "Segment: {0} : {1}", i, segment.ToString() ) );
            }
            DebugLog.Write( "}" );
            #endif
        }
        
        void DumpNodeList( string listName, List<BorderNode> list )
        {
            #if DEBUG
            DebugLog.Write( string.Format( "{0}\n{{\nCount = {1}", listName, list.Count ) );
            for( int i = 0; i < list.Count; i++ )
            {
                var node = list[ i ];
                DebugLog.Write( string.Format( "Node: {0} : {1}", i, node.ToString() ) );
            }
            DebugLog.Write( "}" );
            #endif
        }
        
        #endregion
        
        #region Bounding box and cells of entire build volume
        
        public Maths.Vector2f CornerNW
        {
            get
            {
                float minX = float.MaxValue;
                float maxY = float.MinValue;
                foreach( var volume in BuildVolumes )
                {
                    var corners = volume.Corners;
                    foreach( var corner in corners )
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
                foreach( var volume in BuildVolumes )
                {
                    var corners = volume.Corners;
                    foreach( var corner in corners )
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
                var cell = corner.WorldspaceToCellGrid();
                if( ( corner.X < 0 )&&( cell.X * bbConstant.WorldMap_Resolution > corner.X ) )
                    cell.X--;
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
                var cell = corner.WorldspaceToCellGrid();
                if( ( corner.X < 0 )&&( cell.X * bbConstant.WorldMap_Resolution > corner.X ) )
                    cell.X--;
                if( ( corner.Y > 0 )&&( cell.Y * bbConstant.WorldMap_Resolution < corner.Y ) )
                    cell.Y++;
                return cell;
            }
        }
        
        #endregion
        
        #region Build Border
        
        public void BuildBorders()
        {
            const float threshold = 1f;
            
            DebugLog.Write( string.Format( "BuildBorders()\n{{\nVolume = {0}", FormID ) );
            
            var worldspace = bbGlobal.WorldspaceFromEditorID( WorldspaceEDID );
            if( worldspace == null )
            {
                DebugLog.Write( "} - Unable to load the worldspace that contains the volume!" );
                return;
            }
            
            // Go through each volume and find all the segments
            if( !CreateSegmentsFromBuildVolumes( threshold ) )
            {
                DebugLog.Write( "} - Failed to create line segments from overlapping volume!" );
                return;
            }
            
            // Create the final border nodes from the list of all border segments
            if( !CreateNodesFromSegments( threshold ) )
            {
                DebugLog.Write( "} - Failed to create border nodes from line segments!" );
                return;
            }
            
            // Segments no longer needed
            #if DEBUG
            #else
            BorderSegments = null;
            #endif
            DebugLog.Write( "}" );
        }
        
        bool CreateSegmentsFromBuildVolumes( float threshold )
        {
            DebugLog.Write( string.Format( "CreateSegmentsFromBuildVolumes()\n{{\nthreshold = {0}", threshold ) );
            
            // In [param] sanity..
            if( BuildVolumes.NullOrEmpty() )
            {
                DebugLog.Write( "} - BorderVolumes is empty!" );
                return false;
            }
            
            // I wonder what this line does...
            BorderSegments = new List<BorderSegment>();
            
            // Create all the segments
            int count = BuildVolumes.Count;
            for( int index = 0; index < count; index++ )
            {
                CreateSegmentsForVolume( index, threshold );
            }
            
            // Post-create segment sanity check
            if( BorderSegments.NullOrEmpty() )
            {
                DebugLog.Write( "} - Attempted to create line segments from overlapping volumes but BorderSegments is empty!" );
                return false;
            }
            
            // Remove coincidental segments
            RemoveCoincidentalSegments( threshold );
            
            // Post-coincidental segment sanity check
            if( BorderSegments.Count < 1 )
            {
                DebugLog.Write( "} - Attempted to create line segments from overlapping volumes but BorderSegments is empty!" );
                return false;
            }
            
            // Dump segments to the log
            DumpSegmentList( "BorderSegments", BorderSegments );
            
            DebugLog.Write( "}" );
            return true;
        }
        
        bool CreateNodesFromSegments( float threshold )
        {
            DebugLog.Write( string.Format( "CreateNodesFromSegments()\n{{\nthreshold = {0}", threshold ) );
            
            // In [param] sanity..
            if( BorderSegments.NullOrEmpty() )
            {
                DebugLog.Write( "} - BorderSegments is empty!" );
                return false;
            }
            
            // Get the outside edge segments from the list of all segments
            var outsideSegments = OutsideEdgeSegments( BorderSegments, threshold );
            if( outsideSegments.NullOrEmpty() )
            {
                DebugLog.Write( "} - Unable to calculate a complete outside edge loop from line segments!" );
                return false;
            }
            
            // Dump outside edge segments to the log
            DumpSegmentList( "outsideSegments", outsideSegments );
            
            // Translate segments into border nodes
            BorderNodes = new List<BorderNode>();
            foreach( var segment in outsideSegments )
                BorderNodes.Add( new BorderNode( segment ) );
                
            // Dump border nodes to the log
            DumpNodeList( "BorderNodes", BorderNodes );
            
            DebugLog.Write( "}" );
            return true;
        }
        
        void RemoveCoincidentalSegments( float threshold )
        {
            DebugLog.Write( string.Format( "RemoveCoincidentalSegments()\n{{\nthreshold = {0}", threshold ) );
            var count = BorderSegments.Count;
            for( int index = 0; index < count - 1; index++ )
            {
                for( int index2 = index + 1; index2 < count; index2++ )
                {
                    if( BorderSegments[ index ].IsCoincidentalWith( BorderSegments[ index2 ], threshold ) )
                    {
                        DebugLog.Write( string.Format( "Removing coincidental segment {1} which matches {0}", index, index2 ) );
                        BorderSegments.RemoveAt( index2 );
                        count--;
                    }
                }
            }
            DebugLog.Write( "}" );
        }
        
        void CreateSegmentsForVolume( int vIndex, float threshold )
        {
            DebugLog.Write( string.Format( "CreateSegmentsForVolume()\n{{\nvIndex = {0}\nthreshold = {1}", vIndex, threshold ) );
            
            // Get the build volume and it's corners
            var volume = BuildVolumes[ vIndex ];
            var corners = volume.Corners;
            
            // Try to add each edge as a border segment
            TryAddBorderSegment( vIndex, corners[ 0 ], corners[ 1 ], threshold ); // Edge 1
            TryAddBorderSegment( vIndex, corners[ 1 ], corners[ 2 ], threshold ); // Edge 2
            TryAddBorderSegment( vIndex, corners[ 2 ], corners[ 3 ], threshold ); // Edge 3
            TryAddBorderSegment( vIndex, corners[ 3 ], corners[ 0 ], threshold ); // Edge 4
            
            DebugLog.Write( "}" );
        }
        
        void TryAddBorderSegment( int vIndex, Maths.Vector2f p0, Maths.Vector2f p1, float threshold )
        {
            DebugLog.Write( string.Format( "TryAddBorderSegment()\n{{\nvIndex = {0}\np0 = {1}\np1 = {2}", vIndex, p0.ToString(), p1.ToString() ) );
            
            // Create the segment and split it for intersection with any another volume
            var segment = new BorderSegment( vIndex, p0, p1 );
            SplitSegmentForIntersections( segment, threshold );
            
            DebugLog.Write( "}" );
        }
        
        void SplitSegmentForIntersections( BorderSegment segment, float threshold )
        {
            DebugLog.Write( string.Format( "SplitSegmentForIntersections()\n{{\nsegment = {0}", segment.ToString() ) );
            
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
                DebugLog.Write( string.Format( "{{\nVolume = {0}\nindex = {1}", segment.Volume, index ) );
                {
                    // Edge 1 (0,1)
                    DebugLog.Write( "Test edge 1\n{" );
                    segmentTail = null;
                    if( SplitSegmentForIntersectWith( segment, corners[ 0 ], corners[ 1 ], out segmentTail, threshold ) )
                        if( segmentTail != null )
                            SplitSegmentForIntersections( segmentTail, threshold );
                    DebugLog.Write( "}" );
                    
                    // Edge 2 (1,2)
                    DebugLog.Write( "Test edge 2\n{" );
                    segmentTail = null;
                    if( SplitSegmentForIntersectWith( segment, corners[ 1 ], corners[ 2 ], out segmentTail, threshold ) )
                        if( segmentTail != null )
                            SplitSegmentForIntersections( segmentTail, threshold );
                    DebugLog.Write( "}" );
                    
                    // Edge 3 (2,3)
                    DebugLog.Write( "Test edge 3\n{" );
                    segmentTail = null;
                    if( SplitSegmentForIntersectWith( segment, corners[ 2 ], corners[ 3 ], out segmentTail, threshold ) )
                        if( segmentTail != null )
                            SplitSegmentForIntersections( segmentTail, threshold );
                    DebugLog.Write( "}" );
                    
                    // Edge 4 (3,0)
                    DebugLog.Write( "Test edge 4\n{" );
                    segmentTail = null;
                    if( SplitSegmentForIntersectWith( segment, corners[ 3 ], corners[ 0 ], out segmentTail, threshold ) )
                        if( segmentTail != null )
                            SplitSegmentForIntersections( segmentTail, threshold );
                    DebugLog.Write( "}" );
                }
                DebugLog.Write( "}" );
            }
            
            BorderSegments.Add( segment );
                
            DebugLog.Write( "}" );
        }
        
        bool SplitSegmentForIntersectWith( BorderSegment segment, Maths.Vector2f p0, Maths.Vector2f p1, out BorderSegment segmentTail, float threshold )
        {
            DebugLog.Write( string.Format( "SplitSegmentForIntersectWith()\n{{\nsegment= {0}\np0 = {1}\np1= {2}", segment.ToString(), p0.ToString(), p1.ToString() ) );
            
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
                ( collision == Maths.Geometry.CollisionType.VertexMatch )|| // Ignore vertex matches
                ( collision == Maths.Geometry.CollisionType.EndPoint )      // Ignore end-point matches
            )
            {
                DebugLog.Write( string.Format( "}} - result = {0}", collision.ToString() ) );
                return false;
            }
            
            // TODO: Handle co-linear collisions more appropriately to reduce the final set of segments
            
            DebugLog.Write( string.Format( "intersection = {0}", result.ToString() ) );
            
            // Clip the segment off at the intersection point
            var oldP1 = segment.P1;
            segment.P1 = new Maths.Vector2f( result );
            
            // Create a new segment, test if it's contained in another volume and if it's not then return the new segment as the tail of the original segment
            segmentTail = new BorderSegment( segment.Volume, result, oldP1 );
            DebugLog.Write( string.Format( "segmentTail = {0}", segmentTail.ToString() ) );
            
            // Segment was split 
            DebugLog.Write( string.Format( "}} - result = {1} - Segment split - segment = {0}", segment.ToString(), collision.ToString() ) );
            return true;
        }
        
        List<BorderSegment> OutsideEdgeSegments( List<BorderSegment> segments, float threshold )
        {
            DebugLog.Write( "OutsideEdgeSegments()\n{" );
            var segmentCount = segments.Count;
            int startSegment = 0;
            
            DebugLog.Write( "Finding left-most starting segment...\n{" );
            for( int i = 1; i < segmentCount; i++ )
            {
                // Pick left-most P0
                if( segments[ i ].P0.X < segments[ startSegment ].P0.X )
                {
                    DebugLog.Write( string.Format( "Index: {0} ? {1} :: Left-more: {2} ? {3}", i, startSegment, segments[ i ].P0.X, segments[ startSegment ].P0.X ) );
                    startSegment = i;
                }
                else if( ( segments[ i ].P0 - segments[ startSegment ].P0 ).Length.ApproximatelyEquals( 0f, threshold ) )
                {
                    // Found a matching P0, compare the cross products to find the segment that is left-most
                    var ip = segments[ i ].Normal;
                    var sp = segments[ startSegment ].Normal;
                    var cross = Maths.Vector2f.Cross( sp, ip );
                    DebugLog.Write( string.Format( "Index: {0} ? {1} :: 'X' matches, CrossProduct: {2}", startSegment, i, cross ) );
                    if( cross < 0f )
                    {
                        startSegment = i;
                    }
                }
            }
            DebugLog.Write( string.Format( "}} - Segment {0} picked", startSegment ) );
            
            DebugLog.Write( "Following outside edge from start...\n{" );
            var tmpLst = new List<BorderSegment>();
            {
                tmpLst.Add( segments[ startSegment ] );
                var currentSegment = startSegment;
                do{
                    DebugLog.Write( string.Format( "currentSegment = {0} :: {1}\n{{", currentSegment, segments[ currentSegment ].ToString() ) );
    
                    int nextSegment = -1;
                    for( int i = 0; i < segmentCount; i++ )
                    {
                        // Skip self
                        if( i == currentSegment ) continue;
                        
                        // P0 of segment must match P1 of last segment
                        if( !( segments[ i ].P0 - segments[ currentSegment ].P1 ).Length.ApproximatelyEquals( 0f, threshold ) ) continue;
                        
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
                                DebugLog.Write( string.Format( "Index: {0} -> {1} :: First Found", currentSegment, i ) );
                                nextSegment = i;
                            }
                            // Set or not, continue to the next possible segment
                            continue;
                        }
                        
                        {
                            var ip = segments[ i ].Normal;
                            var np = segments[ nextSegment ].Normal;
                            var cross = Maths.Vector2f.Cross( np, ip );
                            DebugLog.Write( string.Format( "Index: {0} -> {1} ? {2} = {4} :: CrossProduct: {3}", currentSegment, nextSegment, i, cross, cross > 0f ? i : nextSegment ) );
                            if( cross < 0f )
                            {
                                nextSegment = i;
                            }
                        }
                    }
                    if( nextSegment < 0 )
                    {
                        DebugLog.Write( string.Format( "}} - ERROR:  NO EDGE FOUND LINKING FROM: {0} :: {1}", currentSegment, segments[ currentSegment ].ToString() ) );
                        break;
                    }
                    if( nextSegment != startSegment )
                    {
                        tmpLst.Add( segments[ nextSegment ] );
                    }
                    DebugLog.Write( string.Format( "}} - nextSegment = {0} :: {1}", nextSegment, segments[ nextSegment ].ToString() ) );
                    currentSegment = nextSegment;
                }while( currentSegment != startSegment );
            }
            DebugLog.Write( "}" );
            
            // Didn't find a complete loop, wth?
            #if DEBUG
            #else
            if( currentSegment != startSegment )
                tmpLst = null;
            #endif
            
            DebugLog.Write( "}" );
            return tmpLst;
        }
        
       #endregion
        
    }
    
}
