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
        
        public struct CornerData
        {
            public Maths.Vector2f Position;
            public bool Anchored;
        }
        
        public Maths.Vector3f Position;
        public Maths.Vector3f Rotation;
        public Maths.Vector3f Size;
        public Maths.Vector2i Cell;
        public int Reference;
        
        Maths.Vector2f Position2D
        {
            get
            {
                return new Maths.Vector2f( Position.X, Position.Y );
            }
        }
        
        CornerData[] _corners;
        
        public BuildVolume( Maths.Vector3f position, Maths.Vector3f rotation, Maths.Vector3f size, Maths.Vector2i cell, int reference )
        {
            Position = position;
            Rotation = rotation;
            Size = size;
            Cell = cell;
            Reference = reference;
            CalculateCornerPositions();
        }
        
        public void CalculateCornerPositions( bool overrideAndClearAnchoring = false )
        {
            // Restrict rotation to 0->360
            /*
            while( rotation.Z < 0f )
            {
                rotation.Z += 360f;
            }
            rotation.Z %= 360f;
            */
           
            // Half size and Vector2f for corner positions and rotation
            var hSize = Size / 2f;
            var p2 = Position2D;
            var newCorners = new CornerData[ 4 ];
            
            // Define the rect corners counter-clockwise,
            // rotated in the inverse from screenspace to worldspace
            newCorners[ 0 ].Position = Maths.Vector2f.RotateAround( new Maths.Vector2f( p2.X - hSize.X, p2.Y - hSize.Y ), p2, -Rotation.Z );
            newCorners[ 1 ].Position = Maths.Vector2f.RotateAround( new Maths.Vector2f( p2.X + hSize.X, p2.Y - hSize.Y ), p2, -Rotation.Z );
            newCorners[ 2 ].Position = Maths.Vector2f.RotateAround( new Maths.Vector2f( p2.X + hSize.X, p2.Y + hSize.Y ), p2, -Rotation.Z );
            newCorners[ 3 ].Position = Maths.Vector2f.RotateAround( new Maths.Vector2f( p2.X - hSize.X, p2.Y + hSize.Y ), p2, -Rotation.Z );
            
            var firstSet = _corners.NullOrEmpty();
            overrideAndClearAnchoring = overrideAndClearAnchoring && !firstSet;
            
            if( ( overrideAndClearAnchoring )||( firstSet ) )
            {
                _corners = newCorners;
            }
            else
            {
                for( int i = 0; i < 4; i++ )
                    if( !_corners[ i ].Anchored )
                        _corners[ i ].Position = newCorners[ i ].Position;
            }
        }
        
        /// <summary>
        /// Modifies the specified corner to the new position.  If forceSquare is true, this function will also
        /// recalculate the entire volumes length, width and, position using the new position of the corner and
        /// the position of it's opposite corner; maintaining rotation and height as well as squareness of the
        /// volume.
        /// </summary>
        /// <param name="corner">Index of the corner to change</param>
        /// <param name="p">New position of the corner</param>
        /// <param name="forceSquare">Forces the volume to maintain it's squareness if true (all corners are 90 degree angles), otherwise just set the corner and forget about it</param>
        /// <param name="anchorCorner">When true, this corner cannot be modified once it has been moved.</param>
        /// <returns>True is the corner was set [and the volume was recalculated] or false if it was not for any reason.</returns>
        public bool MoveCorner( int corner, Maths.Vector2f p, bool forceSquare = true, bool anchorCorner = false )
        {
            // Invalid corner or the corner is anchored?
            if( ( corner < 0 )||( corner > 3 ) ) return false;
            if( _corners[ corner ].Anchored ) return false;
            
            if( !forceSquare )
            {   // Set it and forget it
                _corners[ corner ].Position = p;
                _corners[ corner ].Anchored = anchorCorner;
                return true;
            }
            
            // Get the neighbouring corner indicies
            int sideM1 = ( corner + 3 ) % 4; // corner -1
            int sideP1 = ( corner + 1 ) % 4; // corner +1
            
            // If either neighbouring corner is anchored we won't be able to modify this corner
            // TODO:  Allow modification of this corners position as long as it maintains side corner positions
            if(
                ( _corners[ sideM1 ].Anchored )||
                ( _corners[ sideP1 ].Anchored )
            ) return false;
            
            // Opposite corner index and position
            int opposite  = ( corner + 2 ) % 4;
            var oP = _corners[ opposite ].Position;
            
            // 2d centre position of volume
            var p2 = Position2D;
            
            // Convert the new position into an unrotated position
            var urP = p.RotateAround( p2, Rotation.Z );
            
            // Unrotate the opposite corner position
            var uroP = oP.RotateAround( p2, Rotation.Z );
            
            // Compute the delta between the unrotated opposite corners
            var delta = new Maths.Vector2f(
                Math.Abs( urP.X - uroP.X ),
                Math.Abs( urP.Y - uroP.Y ) );
            
            // The new size of the volume is the delta of the opposite corners
            Size = new Maths.Vector3f(
                delta.X,
                delta.Y,
                Size.Z );
            
            // The new position of the volume is the average of the opposite corners
            Position = new Maths.Vector3f(
                ( p.X + oP.X ) / 2f,
                ( p.Y + oP.Y ) / 2f,
                Position.Z );
            
            // Recalculcate the positions of all the corners, overriding anchoring
            CalculateCornerPositions( true );
            
            // Finally, anchor this corner as appropriate
            _corners[ corner ].Anchored = anchorCorner;
            
            // Corner set [and anchored] and, volume was recalculated
            return true;
        }
        
        /// <summary>
        /// Check whether a specific corner is anchored
        /// </summary>
        /// <param name="corner">Index of the corner to check</param>
        /// <returns>True/False depending on whether the corner position is anchored.</returns>
        public bool CornerIsAnchored( int corner, bool lookAtNeighbours )
        {
            if( ( corner < 0 )||( corner > 3 ) ) return false;
            
            var thisAnchored = _corners[ corner ].Anchored;
            if( ( !lookAtNeighbours )||( thisAnchored ) ) return thisAnchored;
            
            // Check the neighbouring corner indicies
            int sideM1 = ( corner + 3 ) % 4; // corner -1
            if( _corners[ sideM1 ].Anchored ) return true;
            
            int sideP1 = ( corner + 1 ) % 4; // corner +1
            return _corners[ sideP1 ].Anchored;
        }
        
        /// <summary>
        /// Forces a corners anchoring.
        /// </summary>
        /// <param name="corner">Index of the corner to force</param>
        /// <param name="anchoring">New anchoring of the corner</param>
        public void ForceCornerAnchoring( int corner, bool anchoring )
        {
            if( ( corner < 0 )||( corner > 3 ) ) return;
            _corners[ corner ].Anchored = anchoring;
        }
        
        /// <summary>
        /// Copy of the internal corner array position fields
        /// </summary>
        /// <returns>Array of Vector2f containing the worldspace transform positions of the corners of the volume.</returns>
        public Maths.Vector2f[] Corners
        {
            get
            {   // "Fast" "no-calculations needed" clone
                var clone = new Maths.Vector2f[ 4 ];
                for( int index = 0; index < 4; index++ )
                    clone[ index ] = new Maths.Vector2f( _corners[ index ].Position );
                return clone;
            }
        }
        /*
        
        /// <summary>
        /// Scaled copy of the internal corner array
        /// </summary>
        /// <param name="scale"></param>
        /// <returns></returns>
        public Maths.Vector2f[] Corners( float scale )
        {
            var clone = Corners();
            for( int index = 0; index < 4; index++ )
                clone[ index ] *= scale;
            return clone;
        }
        
        /// <summary>
        /// Translated copy of the internal corner array
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Maths.Vector2f[] Corners( Maths.Vector2f offset )
        {
            var clone = Corners();
            for( int index = 0; index < 4; index++ )
                clone[ index ] += offset;
            return clone;
        }
        
        /// <summary>
        /// Translated then scaled copy of the internal corner array
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public Maths.Vector2f[] Corners( Maths.Vector2f offset, float scale )
        {
            var clone = Corners( offset );
            for( int index = 0; index < 4; index++ )
                clone[ index ] *= scale;
            return clone;
        }
        
        /// <summary>
        /// Scaled then translated copy of the internal corner array
        /// </summary>
        /// <param name="scale"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Maths.Vector2f[] Corners( float scale, Maths.Vector2f offset )
        {   // Scaled then translated
            var clone = Corners( scale );
            for( int index = 0; index < 4; index++ )
                clone[ index ] += offset;
            return clone;
        }
        
        */
       
    }
        
}
