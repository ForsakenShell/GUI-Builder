using System;

namespace XeLib.API
{
    public static class RecordValues
    {
        const string            RecordFlags_Path        = @"Record Header\Record Flags";
        const string            EditorID_Path           = "EDID";
        const string            FullName_Path           = "FULL";
        
        public static string GetEditorIDEx( uint uHandle )
        {
            return Elements.HasElementEx( uHandle, EditorID_Path )
                ? ElementValues.GetValueEx( uHandle, EditorID_Path )
                : null;
        }
        
        public static bool SetEditorIDEx( uint uHandle, string value)
        {
            return ElementValues.SetValueEx( uHandle, EditorID_Path, value );
        }
        
        public static string GetFullNameEx( uint uHandle )
        {
            return ElementValues.GetValueEx( uHandle, FullName_Path );
        }
        
        /*
        public static void TranslateEx( uint uHandle, Double x = 0, Double y = 0, Double z = 0 )
        {
            var position = Elements.GetElementEx( uHandle, @"DATA\Position" );
            ElementValues.SetFloatValueEx( position.XHandle, "X", ElementValues.GetFloatValueEx( position.XHandle, "X" ) + x );
            ElementValues.SetFloatValueEx( position.XHandle, "Y", ElementValues.GetFloatValueEx( position.XHandle, "Y" ) + y );
            ElementValues.SetFloatValueEx( position.XHandle, "Z", ElementValues.GetFloatValueEx( position.XHandle, "Z" ) + z );
        }
        
        public static void Rotate( Handle handle, Double x = 0, Double y = 0, Double z = 0)
        {
            var rotation = Elements.GetElement(handle, @"DATA\Rotation");
            ElementValues.SetFloatValue(rotation, "X", ElementValues.GetFloatValue(rotation, "X") + x);
            ElementValues.SetFloatValue(rotation, "Y", ElementValues.GetFloatValue(rotation, "Y") + y);
            ElementValues.SetFloatValue(rotation, "Z", ElementValues.GetFloatValue(rotation, "Z") + z);
        }
        */
        
        public static string[] GetAllFlagsEx( uint uHandle )
        {
            return ElementValues.GetAllFlagsEx( uHandle, RecordFlags_Path );
        }

        public static bool GetRecordFlagEx( uint uHandle, string name )
        {
            return ElementValues.GetFlagEx( uHandle, RecordFlags_Path, name );
        }

        public static void SetRecordFlagEx( uint uHandle, string name, bool value )
        {
            ElementValues.SetFlagEx( uHandle, RecordFlags_Path, name, value );
        }
    }
}