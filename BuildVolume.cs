/*
 * BuildVolume.cs
 *
 * A build volume for a settlement.
 *
 * User: 1000101
 * Date: 04/12/2017
 * Time: 11:10 AM
 * 
 */
using System;

namespace Border_Builder
{
    /// <summary>
    /// Description of BuildVolume.
    /// </summary>
    public class BuildVolume
    {
        
        public Maths.Vector3f Position;
        public Maths.Vector3f Rotation;
        public Maths.Vector3f Size;
        public Maths.Vector2i Cell;
        public string Reference;
        
        public Maths.Vector2f[] Corners;
        
        public BuildVolume( Maths.Vector3f position, Maths.Vector3f rotation, Maths.Vector3f size, Maths.Vector2i cell, string reference )
        {
            // Restrict rotation to 0->360
            /*
            while( rotation.Z < 0f )
            {
                rotation.Z += 360f;
            }
            rotation.Z %= 360f;
            */
            Position = position;
            Rotation = rotation;
            Size = size;
            Cell = cell;
            Reference = reference;
            var hSize = Size / 2f;
            var p2 = new Maths.Vector2f( Position.X, Position.Y );
            Corners = new Maths.Vector2f[ 4 ];
            // Define the rect corners counterclockwise,
            // Rotate in the inverse from screenspace to worldspace
            Corners[ 0 ] = Maths.Vector2f.RotateAround( new Maths.Vector2f( p2.X - hSize.X, p2.Y - hSize.Y ), p2, -Rotation.Z );
            Corners[ 1 ] = Maths.Vector2f.RotateAround( new Maths.Vector2f( p2.X + hSize.X, p2.Y - hSize.Y ), p2, -Rotation.Z );
            Corners[ 2 ] = Maths.Vector2f.RotateAround( new Maths.Vector2f( p2.X + hSize.X, p2.Y + hSize.Y ), p2, -Rotation.Z );
            Corners[ 3 ] = Maths.Vector2f.RotateAround( new Maths.Vector2f( p2.X - hSize.X, p2.Y + hSize.Y ), p2, -Rotation.Z );
        }
        
        public Maths.Vector2f[] ScaledCorners( float scale )
        {
            return ScaledCorners( scale, Maths.Vector2f.Zero );
        }
        
        public Maths.Vector2f[] ScaledCorners( float scale, Maths.Vector2f offset )
        {
            var scaled = new Maths.Vector2f[ 4 ];
            for( int index = 0; index < 4; index++ )
                scaled[ index ] = ( offset + Corners[ index ] ) * scale;
            return scaled;
        }
    }
        
}
