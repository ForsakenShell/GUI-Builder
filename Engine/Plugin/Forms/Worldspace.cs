/*
 * Worldspace.cs
 * 
 * WoRLDspace form class.
 * 
 */

using System;

using Maths;

using XeLib;


namespace Engine.Plugin.Forms
{
    
    [Attributes.FormAssociation( "WRLD", "Worldspace", true, new Type[]{ typeof( Engine.Plugin.Forms.Cell ) } )]
    public class Worldspace : Form
    {
        
        #region Common Fallout 4 Form fields
        
        Fields.Shared.FullName _FullName;
        Fields.Worldspace.Location _Location;
        Fields.Worldspace.LandData _LandData;
        Fields.Worldspace.MapData _MapData;
        Fields.Worldspace.WorldBounds _Bounds;
        
        #endregion
        
        GodObject.WorldspaceDataPool.PoolEntry _PoolEntry = null;
        
        public GodObject.WorldspaceDataPool.PoolEntry PoolEntry
        {
            get
            {
                if( _PoolEntry == null )
                 _PoolEntry = GodObject.WorldspaceDataPool.GetPoolEntry( this );
                return _PoolEntry;
            }
        }
        
        public static Cell GetCellForRefr( Worldspace worldspace, Vector3f refPosition, uint refrFlags )
        {
            if( ( refrFlags & (uint)Fields.Record.Flags.Common.Persistent ) != 0 )
                return worldspace.Cells.Persistent;
            
            var grid = refPosition.WorldspaceToCellGrid();
            return worldspace.Cells.GetByGrid( grid );
        }
        
        #region Allocation & Disposal
        
        #region Allocation
        
        //public Worldspace() : base() {}
        
        public Worldspace( string filename, uint formID ) : base( filename, formID ) {}
        
        //public Worldspace( Plugin.File mod, Interface.IDataSync ancestor, Handle handle ) : base( mod, ancestor, handle ) {}
        public Worldspace( Collection parentCollection, Interface.IXHandle ancestor, FormHandle handle ) : base( parentCollection, ancestor, handle )
        {
            //DebugLog.OpenIndentLevel( this.IDString, true );
            //DebugDump( TargetHandle.Master );
            //DebugLog.CloseIndentLevel();
        }
        
        public override void CreateChildFields()
        {
            _FullName = new Fields.Shared.FullName( this );
            _Location = new Fields.Worldspace.Location( this );
            _LandData = new Engine.Plugin.Forms.Fields.Worldspace.LandData( this );
            _MapData = new Fields.Worldspace.MapData( this );
            _Bounds = new Fields.Worldspace.WorldBounds( this );
            //_Cells = new Containers.Cells( this );
        }
        
        #endregion
        
        #region Disposal
        
        protected override void Dispose( bool disposing )
        {
            if( Disposed )
                return;
            
            base.Dispose( disposing );
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
        
        public uint GetLocation( TargetHandle target )
        {
            return _Location.GetValue( target );
        }
        public void SetLocation( TargetHandle target, uint value )
        {
            _Location.SetValue( target, value );
        }
        
        public Fields.Worldspace.LandData LandData
        {
            get
            {
                return _LandData;
            }
        }
        
        public Fields.Worldspace.MapData MapData
        {
            get
            {
                return _MapData;
            }
        }
        
        public Fields.Worldspace.WorldBounds WorldBounds
        {
            get
            {
                return _Bounds;
            }
        }
        
        public Collections.Cells Cells
        {
            get
            {
                return CollectionFor<Engine.Plugin.Forms.Cell>() as Collections.Cells;
            }
        }
        
        #endregion
        
        #region Debugging
        
        public bool DebugDumpCells = false;
        public override void DebugDumpChild( TargetHandle target )
        {
            if( _FullName.HasValue( target ) )
                DebugLog.WriteLine( string.Format( "\tFull Name: \"{0}\"", _FullName.ToString( target ) ) );
            if( _Location.HasValue( target ) )
                DebugLog.WriteLine( string.Format( "\tLocation: {0}", _Location.ToString( target ) ) );
            if( _LandData.HasValue( target ) )
                DebugLog.WriteLine( string.Format( "\tLand Data: {0}", _LandData.ToString( target ) ) );
            if( _MapData.HasValue( target ) )
                DebugLog.WriteLine( string.Format( "\tMap Data: {0}", _MapData.ToString( target ) ) );
            if( _Bounds.HasValue( target ) )
                DebugLog.WriteLine( string.Format( "\tBounds: {0}", _Bounds.ToString( target ) ) );
            if( DebugDumpCells )
            {
                var cContainer = Cells;
                DebugLog.WriteLine( string.Format( "\tCell count: {0}", cContainer.Count ) );
                var cells = cContainer.ToList<Engine.Plugin.Forms.Cell>();
                for( var i = 0; i < cells.Count; i++ )
                    cells[i].DebugDump( target );
            }
        }
        
        #endregion
        
    }
    
}
