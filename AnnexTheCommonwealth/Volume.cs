/*
 * Volume.cs
 *
 * An ATC volume primitive.
 *
 */

using System;

using Maths;


namespace AnnexTheCommonwealth
{
    
    /// <summary>
    /// Description of Volume.
    /// </summary>
    [Engine.Plugin.Attributes.ScriptAssociation( "ESM:ATC:Volume" )]
    public class Volume : Engine.Plugin.PapyrusScript
    {
        
        public const int ANCHOR_SELF = 1000;
        public const int ANCHOR_NEIGHBOUR = 50;
        public const int ANCHOR_OPPOSITE = 0;
        
        // These are calculated from the actual position, rotation and size of the volume
        public struct CornerData
        {
            public Vector2f Position;
            public bool Anchored;
            public int Anchoring;
        }
        
        Engine.Plugin.TargetHandle _lastTarget;
        CornerData[] _corners;
        
        public Vector2f GetPosition2D( Engine.Plugin.TargetHandle target )
        {
            var refPos = Reference.GetPosition( target );
            return new Vector2f( refPos.X, refPos.Y );
        }
        
        #region Constructor
        
        public Volume( Engine.Plugin.Forms.ObjectReference reference ) : base( reference )
        {
            ObjectDataChanged += OnSyncObjectDataChanged;
        }
        
        #endregion
        
        #region Custom Events
        
        // Clear any cached values to force the change to be properly reflected
        void OnSyncObjectDataChanged( object sender, EventArgs e )
        {
            _lastTarget = Engine.Plugin.TargetHandle.None;
            _corners = null;
        }
        
        #endregion
        
        public Vector3f GetSize( Engine.Plugin.TargetHandle target )
        {
            var primitive = Reference.Primitive;
            return primitive.GetBounds( target );
        }
        
        public void CalculateCornerPositions( Engine.Plugin.TargetHandle target, bool overrideAndClearAnchoring = false )
        {
            // Restrict rotation to 0->360
            /*
            while( rotation.Z < 0f )
            {
                rotation.Z += 360f;
            }
            rotation.Z %= 360f;
            */
            
            _lastTarget = target;
            
            // Half size and Vector2f for corner positions and rotation
            var _size = Reference.Primitive.GetBounds( target );
            var _rotation = Reference.GetRotation( target );
            var hSize = _size * 0.5f;
            var p2 = GetPosition2D( target );
            var newCorners = new CornerData[ 4 ];
            
            // Define the rect corners counter-clockwise,
            // rotated in the inverse from screenspace to worldspace
            newCorners[ 0 ].Position = Vector2f.RotateAround( new Vector2f( p2.X - hSize.X, p2.Y - hSize.Y ), p2, -_rotation.Z );
            newCorners[ 1 ].Position = Vector2f.RotateAround( new Vector2f( p2.X + hSize.X, p2.Y - hSize.Y ), p2, -_rotation.Z );
            newCorners[ 2 ].Position = Vector2f.RotateAround( new Vector2f( p2.X + hSize.X, p2.Y + hSize.Y ), p2, -_rotation.Z );
            newCorners[ 3 ].Position = Vector2f.RotateAround( new Vector2f( p2.X - hSize.X, p2.Y + hSize.Y ), p2, -_rotation.Z );
            
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
        public bool MoveCorner( int corner, Vector2f p, bool forceSquare = true, bool anchorCorner = false )
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
            
            // Get the position, rotation, and size
            var _position = Reference.GetPosition( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
            var _rotation = Reference.GetRotation( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
            var _primitive = Reference.Primitive;
            var _size = _primitive.GetBounds( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
            
            // 2d centre position of volume
            var p2 = GetPosition2D( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
            
            // Convert the new position into an unrotated position
            var urP = p.RotateAround( p2, _rotation.Z );
            
            // Unrotate the opposite corner position
            var uroP = oP.RotateAround( p2, _rotation.Z );
            
            // Compute the delta between the unrotated opposite corners
            var delta = new Vector2f(
                Math.Abs( urP.X - uroP.X ),
                Math.Abs( urP.Y - uroP.Y ) );
            
            // The new size of the volume is the delta of the opposite corners
            _size = new Vector3f(
                delta.X,
                delta.Y,
                _size.Z );
            _primitive.SetBounds( Engine.Plugin.TargetHandle.Working, _size );
            
            // The new position of the volume is the average of the opposite corners
            _position = new Vector3f(
                ( p.X + oP.X ) / 2f,
                ( p.Y + oP.Y ) / 2f,
                _position.Z );
            Reference.SetPosition( Engine.Plugin.TargetHandle.Working, _position );
            
            // Recalculcate the positions of all the corners, overriding anchoring
            CalculateCornerPositions( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired, true );
            
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
        public Vector2f[] GetCorners( Engine.Plugin.TargetHandle target )
        {
            if( target != _lastTarget )
                CalculateCornerPositions( target, true );
            var clone = new Vector2f[ 4 ];
            for( int index = 0; index < 4; index++ )
                clone[ index ] = new Vector2f( _corners[ index ].Position );
            return clone;
        }
        
    }
    
}
