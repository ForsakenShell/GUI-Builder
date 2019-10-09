/*
 * Constants.cs
 * 
 * Global constants used by the Fallout 4 Engine.
 * 
 */


namespace Engine.Plugin
{
    
    public static partial class Constant
    {
        
        public const uint FormID_None     = 0x00000000;
        public const uint FormID_Invalid  = 0xFFFFFFFF;
        
        public const uint LO_Invalid      = 0x000000FF;
        public const uint LOMask_Invalid  = 0xFF000000;
        
        public static bool ValidFormID( this uint formID )
        {
            return
                ( formID != FormID_None )&&
                ( formID != FormID_Invalid );
        }
        
        public static bool ValidEditorID( this string editorID )
        {
            return !string.IsNullOrEmpty( editorID );
        }
        
        public static bool ValidSignature( this string signature )
        {
            return !string.IsNullOrEmpty( signature );
        }
        
    }
    
}
