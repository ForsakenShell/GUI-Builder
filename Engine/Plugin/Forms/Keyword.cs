/*
 * Keyword.cs
 * 
 * KeYWorD form class.
 * 
 */

using sdColor = System.Drawing.Color;

using XeLib;


namespace Engine.Plugin.Forms
{
    
    [Attributes.FormAssociation( "KYWD", "Keyword", true )]
    public class Keyword : Form
    {
        
        #region Common Fallout 4 Form fields
        
        Fields.Shared.FullName _FullName;
        Fields.Keyword.Color _Color;
        Fields.Keyword.Notes _Notes;
        
        #endregion
        
        #region Allocation & Disposal
        
        #region Allocation
        
        //public Keyword() : base() {}
        
        public Keyword( string filename, uint formID ) : base( filename, formID ) {}
        
        //public Keyword( Plugin.File mod, Interface.IDataSync ancestor, Handle handle ) : base( mod, ancestor, handle ) {}
        public Keyword( Interface.ICollection container, Interface.IXHandle ancestor, FormHandle handle ) : base( container, ancestor, handle ) {}
        
        public override void CreateChildFields()
        {
            _FullName = new Fields.Shared.FullName( this );
            _Color = new Fields.Keyword.Color( this );
            _Notes = new Fields.Keyword.Notes( this );
        }
        
        #endregion
        
        #endregion
        
        #region Properties
        
        public string GetFullName( TargetHandle target )
        {
            return _FullName.GetValue( target );
        }
        public void SetFullName( TargetHandle target, string value )
        {
            _FullName.SetValue( target, value );
        }
        
        public sdColor GetColor( TargetHandle target )
        {
            return _Color.GetValue( target );
        }
        public void SetColor( TargetHandle target, sdColor value )
        {
            _Color.SetValue( target, value );
        }
        
        public string GetNotes( TargetHandle target )
        {
            return _Notes.GetValue( target );
        }
        public void SetNotes( TargetHandle target, string value )
        {
            _Notes.SetValue( target, value );
        }
        
        #endregion
        
        #region Debugging
        
        public override void DebugDumpChild( TargetHandle target )
        {
            if( _FullName.HasValue( target ) )
                DebugLog.WriteLine( string.Format( "\tFull Name: \"{0}\"", _FullName.ToString( target ) ) );
            if( _Color.HasValue( target ) )
                DebugLog.WriteLine( string.Format( "\tColor: {0}", _Color.ToString( target ) ) );
            if( _Notes.HasValue( target ) )
                DebugLog.WriteLine( string.Format( "\tNotes: \"{0}\"", _Notes.ToString( target ) ) );
        }
        
        #endregion
        
    }
    
}
