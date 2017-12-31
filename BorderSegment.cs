/*
 * BorderSegment.cs
 *
 * Defines a single segment of a volumes border.
 *
 * User: 1000101
 * Date: 04/12/2017
 * Time: 11:11 AM
 * 
 */
using System;

namespace Border_Builder
{
    /// <summary>
    /// Description of BorderSegment.
    /// </summary>
    public class BorderSegment
    {
        public int Volume;
        public Maths.Vector2f P0, P1;
        
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
        
        public Maths.Vector2f Normal
        {
            get
            {
                var delta = P1 - P0;
                delta.Normalize();
                return delta;
            }
        }
        
        public BorderSegment( int volume, Maths.Vector2f p0, Maths.Vector2f p1 )
        {
            Volume = volume;
            P0 = new Maths.Vector2f( p0 );
            P1 = new Maths.Vector2f( p1 );
        }
        
        public BorderSegment( BorderSegment segment )
        {
            Volume = segment.Volume;
            P0 = new Maths.Vector2f( segment.P0 );
            P1 = new Maths.Vector2f( segment.P1 );
        }
        
        public new string ToString()
        {
            return string.Format( "Volume={0}, segIndex={5}, P0={1}, P1={2}, Length={3}, Angle={4}", Volume, P0.ToString(), P1.ToString(), Length.ToString(), Angle.ToString(), segIndex );
        }
        
    }
    
}
