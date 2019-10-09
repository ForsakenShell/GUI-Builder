/*
 * ControllerPosition.cs
 *
 * Calculate ATC controller positions to reflect the overall heirarchy of the mod.
 *
 */

using Maths;


namespace AnnexTheCommonwealth
{
    
    public static class ControllerPosition
    {
        
        public const float Controller_Z_Top             = 16384.0f;
        public const float Controller_Z_Separation      =   512.0f;
        public const float Controller_XY_Separation     =   512.0f;
        public const float Component_Z_Separation       =    64.0f;
        public const float Component_X_Separation       =   256.0f;
        public const float Component_Y_Separation       =     0.0f;
        
        public const float ZPOS_Settlement              = Controller_Z_Top;
        public const float ZPOS_SubDivision             = ZPOS_Settlement  - Controller_Z_Separation;
        public const float ZPOS_BorderEnabler           = ZPOS_SubDivision - Controller_Z_Separation;
        public const float ZPOS_Component               = ZPOS_SubDivision - Component_Z_Separation;
        
        public static Vector3f CalculateRelativeFrom( Vector3f source, Vector3f reference, float xydistance, float zpos )
        {
            var delta = reference - source;
            delta.Z = 0;
            
            var len = delta.Length2D;
            delta /= len;
            delta *= xydistance;
            
            return new Vector3f(
                source.X + delta.X,
                source.Y + delta.Y,
                zpos );
        }
        
    }
    
}
