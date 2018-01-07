/*
 * Maths.cs
 *
 * Root class and extension functions dealing with common maths.
 *
 * User: 1000101
 * Date: 01/12/2017
 * Time: 9:30 AM
 * 
 */
using System;

namespace Border_Builder
{
    /// <summary>
    /// Description of Maths.
    /// </summary>
    public static partial class Maths
    {
        
        public const float PI_TO_RAD = (float)( Math.PI / 180.0d );
        
        #region Approximate equality
        
        // Can't be a non-zero difference less than epsilon so double it
        public const float FLOAT_EPSILON = float.Epsilon * 2f;
        public const double DOUBLE_EPSILON = Double.Epsilon * 2f;
        
        public static bool ApproximatelyEquals( this float left, float right, float threshold = FLOAT_EPSILON )
        {
            return Math.Abs( left - right ) < threshold;
        }
        
        public static bool ApproximatelyEquals( this double left, double right, double threshold = DOUBLE_EPSILON )
        {
            return Math.Abs( left - right ) < threshold;
        }
        
        public static bool ApproximatelyEquals( this float left, double right, double threshold = DOUBLE_EPSILON )
        {
            return ApproximatelyEquals( (double)left, right, threshold );
        }
        
        public static bool ApproximatelyEquals( this double left, float right, double threshold = DOUBLE_EPSILON )
        {
            return ApproximatelyEquals( left, (double)right, threshold );
        }
        
        #endregion
        
        #region Math clamp
        
        public static T Clamp<T>( this T value, T lower, T upper ) where T : IComparable<T>
        {
            return value.CompareTo( lower ) < 0 ? lower :
                value.CompareTo( upper ) > 0 ? upper :
                value;
        }
        
        #endregion
        
        #region Math Lerp
        
        public static float Lerp( float min, float max, float amount )
        {
            return min + ( max - min ) * amount;
        }
        
        public static float InverseLerp( float min, float max, float amount )
        {
            return max - ( max - min ) * amount;
        }
        
        #endregion
        
    }
}
