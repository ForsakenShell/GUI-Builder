/*
 * Cell.cs
 * 
 * CELL form class.
 * 
 */

using System;

using XeLib;


namespace Engine.Plugin.Forms
{
    
    [Attributes.FormAssociation( "CELL", "Cell", true, typeof( Engine.Plugin.Collections.Cells ), new Type[]{ typeof( Engine.Plugin.Forms.Landscape ), typeof( Engine.Plugin.Forms.ObjectReference ) } )]
    public class Cell : Form
    {
        
        #region Cell Form fields
        
        Fields.Shared.FullName _FullName;
        Fields.Cell.Flags _Flags;
        Fields.Cell.CellGrid _CellGrid;
        Fields.Cell.WaterHeight _WaterHeight;
        
        Engine.Plugin.Forms.Landscape _Landscape;

        #endregion
        
        #region Allocation & Disposal
        
        #region Allocation
        
        //public Cell() : base() {}
        
        public Cell( string filename, uint formID ) : base( filename, formID ) {}
        
        //public Cell( Plugin.File mod, Interface.IDataSync ancestor, Handle handle ) : base( mod, ancestor, handle ) {}
        public Cell( Collection parentCollection, Interface.IXHandle ancestor, FormHandle handle ) : base( parentCollection, ancestor, handle ) {}
        
        public override void CreateChildFields()
        {
            _FullName    = new Fields.Shared.FullName( this );
            _Flags       = new Fields.Cell.Flags( this );
            _CellGrid    = new Fields.Cell.CellGrid( this );
            _WaterHeight = new Engine.Plugin.Forms.Fields.Cell.WaterHeight( this );
        }
        
        #endregion
        
        #endregion
        
        #region [Un]Load Cell
        
        /*  Child containers are handled from the base class now
        public override bool PostLoad()
        {
            var result = _ObjectReferences.LoadFrom( this );
            if( !result )
                DebugLog.Write( string.Format( "{0} :: ObjectReferences.LoadFrom returned false!", this.FullTypeName() ) );
            return result;
        }
        */
        
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
        
        public uint GetFlags( TargetHandle target )
        {
            return _Flags.GetValue( target );
        }
        public void SetFlags( TargetHandle target, uint value )
        {
            _Flags.SetValue( target, value );
        }
        
        public Fields.Cell.CellGrid CellGrid
        {
            get
            {
                return _CellGrid;
            }
        }
        
        public Fields.Cell.WaterHeight WaterHeight
        {
            get
            {
                return _WaterHeight;
            }
        }
        
        public bool GetIsInterior( TargetHandle target )
        {
            var f = GetFlags( target );
            return ( f & (uint)Engine.Plugin.Forms.Fields.Cell.Flags.Flag.IsInteriorCell ) != 0;
        }
        
        public bool GetIsPackInCell( TargetHandle target )
        {
            var f = GetFlags( target );
            //DebugLog.WriteLine( string.Format(
            //    "{0} :: Cell Flags: 0x{1}",
            //    IDString,
            //    f.ToString( "X8" ) ) );
            return ( f & (uint)Engine.Plugin.Forms.Fields.Cell.Flags.Flag.PackInCell ) != 0;
        }
        
        public Engine.Plugin.Collection ObjectReferences
        {
            get
            {
                return CollectionFor<Engine.Plugin.Forms.ObjectReference>() as Engine.Plugin.Collection;
            }
        }

        public Engine.Plugin.Forms.Landscape Landscape
        {
            get
            {
                if( _Landscape == null )
                {
                    var landscapes = CollectionFor<Engine.Plugin.Forms.Landscape>();
                    if( ( landscapes != null )&&( landscapes.LoadAllForms( false ) ) )
                    {
                        var allLandscapes = landscapes.ToList<Engine.Plugin.Forms.Landscape>();
                        if( !allLandscapes.NullOrEmpty() )
                            _Landscape = allLandscapes[ 0 ];
                    }
                }
                return _Landscape;
            }
        }

        public Worldspace Worldspace
        {
            get
            {
                return Ancestor as Worldspace;
            }
        }
        
        #endregion
        
        #region Debugging
        
        public override void DebugDumpChild( TargetHandle target )
        {
            if( _FullName.HasValue( target ) )
                DebugLog.WriteLine( string.Format( "\tFull Name: \"{0}\"", _FullName.ToString( target ) ) );
            if(
                ( !RecordFlags.GetPersistent( target ) )&&
                ( _CellGrid.HasValue( target ) )
            )
                DebugLog.WriteLine( string.Format( "\tGrid: {0}", _CellGrid.ToString( target ) ) );
            if( _Flags.HasValue( target ) )
                DebugLog.WriteLine( string.Format( "\tFlags: {0}", _Flags.ToString( target ) ) );
            if( _WaterHeight.HasValue( target ) )
                DebugLog.WriteLine( string.Format( "\tWater Height: {0}", _WaterHeight.ToString( target ) ) );
            var w = Worldspace;
            if( w != null )
                DebugLog.WriteLine( string.Format( "\tWorldspace: {0}", w.ToString() ) );
        }
        
        #endregion
        
        /*
        public override string ToString( )
        {
            var bTS = base.ToString();
            var gTS = RecordFlags.Persistent
                ? " - Persistent cell"
                : CellGrid.HasValue()
                    ? " - " + CellGrid.ToString()
                    : null;
            var w = Worldspace;
            var wTS = w != null ? " in " + w.ToString() : null;
            return string.Format( "{0}{1}{2}", bTS, gTS, wTS );
        }
        */
       
    }
    
}
