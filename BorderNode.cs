/*
 * BorderNode.cs
 *
 * A finalized border segment.
 *
 * User: 1000101
 * Date: 04/12/2017
 * Time: 11:12 AM
 * 
 */
using System;

namespace Border_Builder
{
    /// <summary>
    /// Description of BorderNode.
    /// </summary>
    public class BorderNode
    {
        public Maths.Vector3f A,B;
        
        public float Length
        {
            get
            {
                return ( A - B ).Length;
            }
        }
        
        public float Angle
        {
            // NOTE:  THIS IS NOT A TRUE 3D ANGLE!  THIS IS A 2D ANGLE BETWEEN THE X & Y COMPONENTS!
            get
            {
                return ( new Maths.Vector2f( B.X, B.Y ) - new Maths.Vector2f( A.X, A.Y ) ).Angle;
            }
        }
        
        public BorderNode( Maths.Vector3f a, Maths.Vector3f b )
        {
            A = new Maths.Vector3f( a );
            B = new Maths.Vector3f( b );
        }
        
        public BorderNode( Maths.Vector2f a, Maths.Vector2f b )
        {
            A = new Maths.Vector3f( a.X, a.Y, 0f );
            B = new Maths.Vector3f( b.X, b.Y, 0f );
        }
        
        public BorderNode( BorderSegment segment )
        {
            A = new Maths.Vector3f( segment.P0.X, segment.P0.Y, 0f );
            B = new Maths.Vector3f( segment.P1.X, segment.P1.Y, 0f );
        }
        
        public new string ToString()
        {
            return string.Format( "A={0}, B={1}, Length={2}, Angle={3}", A.ToString(), B.ToString(), Length.ToString(), Angle.ToString() );
        }
        
    }
}
