/*
 * Ray.cs
 *
 * A basic 3D ray.
 *
 */

namespace Maths
{
    
    public struct Ray
    {
        
        public Vector3f         Origin;
        public Vector3f         Direction;

        public                  Ray( Vector3f origin, Vector3f direction )
        {
            Origin      = new Vector3f( origin );
            Direction   = Vector3f.Normal( direction );
        }

        public override string  ToString()
        {
            return string.Format( "Origin = {0} : Direction = {1}", Origin.ToString(), Direction.ToString() );
        }

    }

}
