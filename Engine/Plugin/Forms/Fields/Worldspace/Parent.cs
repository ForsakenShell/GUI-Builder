/*
 * Parent.cs
 *
 * Worldspace Parent fields.
 *
 * Worldspaces can be parented, we need to redirect to the parent for some fields if they are
 */

using System;
using XeLib;


namespace Engine.Plugin.Forms.Fields.Worldspace
{
    
    public class Parent : RawField
    {
        
        public enum Flags : uint
        {
            UseLandData             = 0x00000001,
            UseLODData              = 0x00000002,
            UseMapData              = 0x00000004,
            UseWaterData            = 0x00000008,
            UseClimateData          = 0x00000010,
            UseImageSpaceData       = 0x00000020,
            UseSkyCell              = 0x00000040

        }
        
        const string                   _XPath                       = "Parent";
        
        const string                   _ParentWorldspace            = "WNAM";
        const string                   _ParentingFlags              = @"PNAM\Flags";
        
        CachedUIntField                 cached_Parent;
        CachedUIntField                 cached_Flags;
        
        public                          Parent( Form form ) : base( form, _XPath )
        {
            cached_Parent = new CachedUIntField( form, _XPath, _ParentWorldspace );
            cached_Flags  = new CachedUIntField( form, _XPath, _ParentingFlags   );
        }
        
        public Forms.Worldspace         GetParentWorldspace( TargetHandle target )
        {
            var parentID = GetParentWorldspaceID( target );
            return GodObject.Plugin.Data.Root.Find<Forms.Worldspace>( parentID, true );
        }

        public uint                     GetParentWorldspaceID( TargetHandle target )                { return cached_Parent.GetValue( target ); }
        public void                     SetParentWorldspaceID( TargetHandle target, uint value )    { cached_Parent.SetValue( target, value ); }
        
        public uint                     GetParentingFlags( TargetHandle target )                    { return cached_Flags.GetValue( target ); }
        public void                     SetParentingFlags( TargetHandle target, uint value )        { cached_Flags.SetValue( target, value ); }
        
        public override string          ToString( TargetHandle target, string format = null )
        {
            return string.Format(
                string.IsNullOrEmpty( format ) ? "Parent FormID = {0} :: Flags = {1}" : format,
                cached_Parent.ToString( target ),
                cached_Flags.ToString( target ) );
        }
        
    }
    
}
