/*
 * NodeSegment.cs
 *
 * Defines a single segment of a volumes border.
 *
 */
using System;

using Maths;

/*
namespace GUIBuilder
{
    /// <summary>
    /// Description of NodeSegment.
    /// </summary>
    public class NodeSegment
    {
        public int Volume;
        public Vector2f P0, P1;
        
        // This is meta only for debugging, once the nodes are created this value means nothing
        public int segIndex;
        
        public float Length
        {
            get
            {
                return ( P0 - P1 ).Length;
            }
        }
        
        public float Angle
        {
            get
            {
                return Normal.Angle;
            }
        }
        
        public Vector2f Normal
        {
            get
            {
                var delta = P1 - P0;
                delta.Normalize();
                return delta;
            }
        }
        
        public NodeSegment( int volume, Vector2f p0, Vector2f p1 )
        {
            Volume = volume;
            P0 = new Vector2f( p0 );
            P1 = new Vector2f( p1 );
        }
        
        public NodeSegment( NodeSegment segment )
        {
            Volume = segment.Volume;
            P0 = new Vector2f( segment.P0 );
            P1 = new Vector2f( segment.P1 );
        }
        
        public bool IsCoincidentalWith( NodeSegment other, float threshold = Maths.Constant.FLOAT_EPSILON )
        {
            if(
                ( ( this.P0 - other.P0 ).Length.ApproximatelyEquals( 0f, threshold ) )&&
                ( ( this.P1 - other.P1 ).Length.ApproximatelyEquals( 0f, threshold ) )
            ) return true;
            return(
                ( ( this.P0 - other.P1 ).Length.ApproximatelyEquals( 0f, threshold ) )&&
                ( ( this.P1 - other.P0 ).Length.ApproximatelyEquals( 0f, threshold ) )
            );
        }
        
        public new string ToString()
        {
            return string.Format( "Volume={0}, segIndex={5}, P0={1}, P1={2}, Length={3}, Angle={4}", Volume, P0.ToString(), P1.ToString(), Length.ToString(), Angle.ToString(), segIndex );
        }
        
    }
    
}
*/
