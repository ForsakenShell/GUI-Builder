/*
 * ${FILE}
 *
 * Insert description here.
 *
 * User: 1000101
 * Date: 08/01/2018
 * Time: 4:17 PM
 * 
 */
using System;
using System.Collections.Generic;

namespace Border_Builder
{
    /// <summary>
    /// Description of WeldPoints.
    /// </summary>
    public class WeldPoint
    {
        public VolumeParent Parent;
        public BuildVolume Volume;
        public int CornerIndex;
        
        public WeldPoint( VolumeParent parent, BuildVolume volume, int cornerIndex )
        {
            Parent = parent;
            Volume = volume;
            CornerIndex = cornerIndex;
        }
        
        public bool Anchored( bool lookAtNeighbours )
        {
            return Volume.CornerIsAnchored( CornerIndex, lookAtNeighbours );
        }
        
        public void SetAnchored( bool value )
        {
            Volume.ForceCornerAnchoring( CornerIndex, value );
        }
        
        public Maths.Vector2f Position
        {
            get
            {
                return Volume.Corners[ CornerIndex ];
            }
        }
        
        public static void FilterWeldPoints( List<WeldPoint> points, bool includeUnAnchored, bool includeAnchored, bool checkNeighbourAnchoring, bool resetAnchoring, List<WeldPoint> exclusionList = null )
        {
            if( !exclusionList.NullOrEmpty() )
            {
                // Exclude any points in the exclusion list
                for( int i = points.Count - 1; i >= 0; i-- )
                    if( exclusionList.Contains( points[ i ] ) ) points.RemoveAt( i );
            }
            if( !includeUnAnchored )
            {
                // Discard any which aren't anchored
                for( int i = points.Count - 1; i >= 0; i-- )
                    if( !points[ i ].Anchored( checkNeighbourAnchoring ) ) points.RemoveAt( i );
            }
            
            if( !includeAnchored )
            {
                // Discard any which are anchored
                for( int i = points.Count - 1; i >= 0; i-- )
                    if( points[ i ].Anchored( checkNeighbourAnchoring ) ) points.RemoveAt( i );
            }
            if( resetAnchoring )
            {
                // Clear the anchoring
                foreach( var point in points )
                    point.SetAnchored( false );
            }
        }
        
        public static void WeldVolumeVerticies( List<VolumeParent> parents, VolumeParent parent, BuildVolume volume, float threshold, bool weldToOtherParents )
        {
            if(
                ( parents.NullOrEmpty() )||
                ( parent == null )||
                ( volume == null )
            )   return;
            
            var worldspace = bbGlobal.WorldspaceFromEditorID( parent.WorldspaceEDID );
            if( worldspace == null ) return;
            
            for( int index = 0; index < 4; index++ )
            {
                var corners = volume.Corners;
                var points = WeldPoint.FindWeldableCorners( worldspace, parents, corners[ index ], threshold, weldToOtherParents, parent );
                if( points.Count > 1 )
                {   // If it's only one point then it's itself
                    var weldPoint = WeldPoint.CalculateWeldPoint( points );
                    WeldPoint.WeldCornersTo( weldPoint, points, true, true );
                }
            }
        }
        
        public static void WeldCornersTo( Maths.Vector2f weldPoint, List<WeldPoint> points, bool forceSquare = true, bool anchorCorners = true )
        {
            if( points.NullOrEmpty() ) return;
            foreach( var point in points )
            {
                point.Volume.MoveCorner( point.CornerIndex, weldPoint, forceSquare, anchorCorners );
            }
        }
        
        public static List<WeldPoint> FindWeldableCorners( bbWorldspace worldspace, List<VolumeParent> parents, Maths.Vector2f origin, float threshold, bool weldToOtherParents, VolumeParent specificParent )
        {
            var points = new List<WeldPoint>();
            
            if(
                ( weldToOtherParents )&&
                ( !parents.NullOrEmpty() )&&
                ( worldspace != null )
            )
            {
                // Find weldable corners from all parents
                foreach( var parent in parents )
                {
                    if( parent.WorldspaceEDID == worldspace.EditorID )
                    {
                        AddWeldableCornersFrom( parent, ref origin, threshold, ref points );
                    }
                }
            }
            else if( specificParent != null )
            {
                // Find weldable corner for the specific parent
                AddWeldableCornersFrom( specificParent, ref origin, threshold, ref points );
            }
            
            return points;
        }
        
        static void AddWeldableCornersFrom( VolumeParent parent, ref Maths.Vector2f origin, float threshold, ref List<WeldPoint> points )
        {
            foreach( var volume in parent.BuildVolumes )
            {
                for( int index = 0; index < 4; index++ )
                {
                    var corners = volume.Corners;
                    if( origin.DistanceFrom( corners[ index ] ) < threshold )
                    {
                        // Add point and modify the origin
                        points.Add( new WeldPoint( parent, volume, index ) );
                        origin = WeldPoint.CalculateWeldPoint( points );
                    }
                }
            }
        }
        
        public static Maths.Vector2f CalculateWeldPoint( List<WeldPoint> points )
        {
            var result = new Maths.Vector2f();
            foreach( var point in points )
            {
                var corners = point.Volume.Corners;
                result += corners[ point.CornerIndex ];
            }
            result /= points.Count;
            return result;
        }
        
    }
}
