/*
 * Landscape.cs
 * 
 * Cell LANDscape form class.
 * 
 */

using System;

using Maths;

using XeLib;
using XeLibHelper;


namespace Engine.Plugin.Forms
{
    
    [Attributes.FormAssociation( "LAND", "Landscape", false )]
    public class Landscape : Form
    {

        #region Common Fallout 4 Form fields

        Fields.Landscape.Heightmap _Heightmap;

        #endregion

        #region Allocation & Disposal

        #region Allocation

        //public Landscape() : base() {}

        public Landscape( string filename, uint formID ) : base( filename, formID ) {}

        //public Landscape( Plugin.File mod, Interface.IDataSync ancestor, Handle handle ) : base( mod, ancestor, handle ) {}
        public Landscape( Collection parentCollection, Interface.IXHandle ancestor, FormHandle handle ) : base( parentCollection, ancestor, handle ) {}
        
        public override void CreateChildFields()
        {
            _Heightmap = new Fields.Landscape.Heightmap( this );
        }

        #endregion

        #endregion

        #region Properties

        public Fields.Landscape.Heightmap Heightmap
        {
            get
            {
                return _Heightmap;
            }
        }

        public Cell Cell
        {
            get
            {
                return Ancestor as Cell;
            }
        }

        #endregion

        #region Debugging

        public override void DebugDumpChild( TargetHandle target )
        {
        }
        
        #endregion
        
        /*
        public override string ToString( )
        {
            return string.Format(
                "\"{0}\" - 0x{1}{3} - \"{2}\"",
                Signature,
                FormID.ToString( "X8" ),
                EditorID,
                RecordFlags.Persistent ? "[P]" : null
            );
        }
        */
        
    }
    
}
