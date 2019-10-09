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

namespace Maths
{
    
    public static class Constant
    {
        
        public const float DEG_TO_RAD = (float)( Math.PI / 180.0d );
        
        // Can't be a non-zero difference less than epsilon so double it
        public const float FLOAT_EPSILON = float.Epsilon * 2f;
        public const double DOUBLE_EPSILON = Double.Epsilon * 2f;
        
    }
    
    #region Approximate equality
    
    public static class Equation
    {
        
        public static bool ApproximatelyEquals( this float left, float right, float threshold = Constant.FLOAT_EPSILON )
        {
            return Math.Abs( left - right ) < threshold;
        }
        
        public static bool ApproximatelyEquals( this double left, double right, double threshold = Constant.DOUBLE_EPSILON )
        {
            return Math.Abs( left - right ) < threshold;
        }
        
        public static bool ApproximatelyEquals( this float left, double right, double threshold = Constant.DOUBLE_EPSILON )
        {
            return ApproximatelyEquals( (double)left, right, threshold );
        }
        
        public static bool ApproximatelyEquals( this double left, float right, double threshold = Constant.DOUBLE_EPSILON )
        {
            return ApproximatelyEquals( left, (double)right, threshold );
        }
        
    }
    
    #endregion
    
    #region Math clamp
    
    public static class Clamps
    {
        
        public static T Clamp<T>( this T value, T lower, T upper ) where T : IComparable<T>
        {
            return value.CompareTo( lower ) < 0 ? lower :
                value.CompareTo( upper ) > 0 ? upper :
                value;
        }
        
    }
    
    #endregion
    
    #region Math Lerp
    
    public static class Lerps
    {
        
        public static float Lerp( float min, float max, float amount )
        {
            return min + ( max - min ) * amount;
        }
        
        public static float InverseLerp( float min, float max, float amount )
        {
            return max - ( max - min ) * amount;
        }
        
    }
    
    #endregion
    
}