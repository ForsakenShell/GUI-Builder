/*
 * Cells.cs
 * 
 * Collection for CELL records, allows searching by Cell Grid in Worldspaces.
 * 
 */

using System.Collections.Generic;

using Maths;

using Engine.Plugin.Interface;

using XeLib;


namespace Engine.Plugin.Collections
{
    
    /// <summary>
    /// Description of Cell.
    /// </summary>
    public class Cells : Collection
    {
        
        const int                               BLOCK_SIZE          = 32;
        const int                               SUB_BLOCK_SIZE      = 8;
        
        Plugin.Forms.Cell                       _Persistent         = null;
        Dictionary<Vector2i,Plugin.Forms.Cell>  _ByGrid             = null;
        
        public                                  Cells( Attributes.ClassAssociation association, IXHandle ancestor )
            : base( association, ancestor )
            {}

        public                                  Cells( Attributes.ClassAssociation association )
            : base( association )
            {}
        
        protected override void                 Dispose( bool disposing )
        {
            // Base will set this, after it's disposed, so check first _then_ call it
            if( Disposed )
                return;
            base.Dispose( disposing );
            
            if( _ByGrid != null )
                _ByGrid.Clear();
            _ByGrid = null;
            
            _Persistent = null;
        }
        
        public Plugin.Forms.Cell                GetByWorldPos( Vector3f coords )
        {
            return GetByWorldPos( coords.X, coords.Y );
        }
        public Plugin.Forms.Cell                GetByWorldPos( float x, float y )
        {
            return GetByGrid( Engine.SpaceConversions.WorldspaceToCellGrid( x, y ) );
        }

        public Plugin.Forms.Cell                GetByGrid( int x, int y )
        {
            return GetByGrid( new Vector2i( x, y ) );
        }
        
        public Plugin.Forms.Cell                GetByGrid( Vector2i coords )
        {
            if( Disposed ) return null;
            lock( CollectionLock )
            {
                //DebugLog.OpenIndentLevel( new [] { "coords = " + coords.ToString(), "Worldspace = " + Worldspace?.IDString }, true );
                var m = GodObject.Windows.GetWindow<GUIBuilder.Windows.Main>();
                //m.PushStatusMessage();
                //m.StartSyncTimer();
                //var tStart = m.SyncTimerElapsed();

                var found = false;
                Plugin.Forms.Cell cell = null;

                if( !this.IsValid() )
                {
                    DebugLog.WriteError( "Cell Collection is not valid!" );
                    goto localReturnResult;
                }

                //if( _ByGrid == null )
                //    DebugLog.WriteLine( "No grid dictionary" + Worldspace == null ? null : string.Format( " for WRLD = {0}", Worldspace.IDString ) );

                // Return now it's been loaded already
                if( ( _ByGrid != null ) && ( _ByGrid.TryGetValue( coords, out cell ) ) )
                {
                    //DebugLog.WriteLine( "Found grid in dictionary" );
                    goto localReturnResult;
                }

                var wsMax = Worldspace.WorldBounds.GetMax( TargetHandle.Master );
                var wsMin = Worldspace.WorldBounds.GetMin( TargetHandle.Master );
                if(
                    ( coords.X > wsMax.X ) || ( coords.X < wsMin.X ) ||
                    ( coords.Y > wsMax.Y ) || ( coords.Y < wsMin.Y )
                )
                {
                    DebugLog.WriteError( "Grid is out of bounds for the worldspace :: " + coords.ToString() + " :: " + wsMax.ToString() + " - " + wsMin.ToString() );
                    goto localReturnResult;
                }

                #region Copy-pasta from XeLib source

                // Because Worldspaces can contain thousands of Cells, Bethesda have broken them into
                // groups because otherwise it would literally take hours to look through them all
                // The Commonwealth has ~36K Cells and it took almost 2 hours on my 4GHz 8-core Amd
                // FX8350 to load them all.

                // Calculate which block and sub-block record the cell is in and load that entire
                // sub-block.

                var block = coords / BLOCK_SIZE;
                if( ( coords.X < 0 ) && ( ( coords.X % BLOCK_SIZE ) != 0 ) ) block.X--;
                if( ( coords.Y < 0 ) && ( ( coords.Y % BLOCK_SIZE ) != 0 ) ) block.Y--;
                var sub = coords / SUB_BLOCK_SIZE;
                if( ( coords.X < 0 ) && ( ( coords.X % SUB_BLOCK_SIZE ) != 0 ) ) sub.X--;
                if( ( coords.Y < 0 ) && ( ( coords.Y % SUB_BLOCK_SIZE ) != 0 ) ) sub.Y--;

                var bName = string.Format( "Block {0}, {1}"    , block.X, block.Y );
                var sName = string.Format( "Sub-Block {0}, {1}", sub.X  , sub.Y   );
                //DebugLog.WriteLine( bName );
                //DebugLog.WriteLine( sName );

                #endregion

                var wsHandles = Worldspace.Handles;

                // Look through every mod handle for the worldspace
                foreach( var wsHandle in wsHandles )
                {
                    ElementHandle[] bHandles = null;

                    var gHandle = wsHandle.GetChildGroup();
                    if( !gHandle.IsValid() )
                        goto localSkipGroup;

                    bHandles = gHandle.GetElements<ElementHandle>();
                    if( bHandles.NullOrEmpty() )
                        goto localSkipGroup;

                    // Look through every block in the worldspace
                    var bCount = bHandles.Length;

                    for( int bIndex = 0; bIndex < bCount; bIndex++ )
                    {
                        ElementHandle[] sbHandles = null;

                        var bHandle = bHandles[ bIndex ];
                        if( !bHandle.IsValid() )
                            goto localSkipBlock;

                        var tbName = bHandle.Name;
                        var bMatch = tbName == bName;
                        //DebugLog.WriteLine( string.Format( "Testing \"{0}\" ? \"{1}\" = {2}", tbName, bName, bMatch.ToString() ) );
                        if( !bMatch )
                            goto localSkipBlock;

                        sbHandles = bHandle.GetElements<ElementHandle>();
                        if( sbHandles.NullOrEmpty() )
                            goto localSkipBlock;

                        // Look through every sub-block in the block
                        var sbCount = sbHandles.Length;
                        for( int sbIndex = 0; sbIndex < sbCount; sbIndex++ )
                        {
                            var sbHandle = sbHandles[ sbIndex ];
                            if( !sbHandle.IsValid() )
                                continue;

                            var tsbName = sbHandle.Name;
                            var sbMatch = tsbName == sName;
                            //DebugLog.WriteLine( string.Format( "Testing \"{0}\" ? \"{1}\" = {2}", tsbName, sName, sbMatch.ToString() ) );
                            if( !sbMatch )
                                continue;

                            // Found the sub-block, now load all the cells in this sub-group
                            if( !found )
                                m.PushStatusMessage();
                            found = true;
                            m.SetCurrentStatusMessage( string.Format( "Cells.LoadingRange".Translate(), tbName, tsbName, wsHandle.Filename ) );
                            //DebugLog.WriteLine( "Loading Cells containing " + coords.ToString() + " from " + tbName + " " + tsbName + " in " + wsHandle.Filename );
                            LoadFromEx( ParentForm, sbHandle, true );
                            break;
                        }

                    localSkipBlock:
                        ElementHandle.ReleaseHandles<ElementHandle>( sbHandles );
                        if( found ) break;
                    }

                localSkipGroup:
                    ElementHandle.ReleaseHandles<ElementHandle>( bHandles );
                    if( gHandle.IsValid() )
                        gHandle.Dispose();
                }

                // Loaded all the cells in the sub-block now (in theory)
                //DebugLog.WriteLine( "Final attempt to find grid in dictionary" );
                if( _ByGrid != null )
                    _ByGrid.TryGetValue( coords, out cell );

            localReturnResult:
                if( found )
                    m.PopStatusMessage();
                //DebugLog.CloseIndentLevel<Plugin.Forms.Cell>( "cell", cell );
                return cell;
            }
        }
        
        public Plugin.Forms.Cell                Persistent
        {
            get
            {
                lock( CollectionLock ) 
                    return _Persistent;
            }
        }
        
        public override bool                    Add( IXHandle syncObject )
        {
            if( Disposed ) return false;
            lock( CollectionLock )
            {
                //DebugLog.OpenIndentLevel( new [] { this.FullTypeName(), "Add()", syncObject.ToString() } );
                var result = false;
                var cell = syncObject as Plugin.Forms.Cell;
                var interiorCell = false;
                var persistentCell = false;

                // Not a cell?  wth?
                if( cell == null )
                {
                    DebugLog.WriteError( string.Format( "Invalid object to add to collection.\nCollection = {0}\nObject = {1}", this.ToString(), syncObject.IDString ) );
                    goto localReturnResult;
                }

                // Do the base addition
                if( !base.AddEx( syncObject, false ) )
                    goto localReturnResult;

                // Don't store interior cells by grid or persistence
                interiorCell = cell.GetIsInterior( Engine.Plugin.TargetHandle.Master );
                if( interiorCell )
                {
                    result = true;
                    goto localReturnResult;
                }

                // Don't store interior cells by grid or persistence
                if( Worldspace == null )
                {
                    result = true;
                    goto localReturnResult;
                }

                // Store the worldspace's unique persistent cell
                persistentCell = cell.RecordFlags.GetPersistent( Engine.Plugin.TargetHandle.Master );
                if( persistentCell )
                {
                    _Persistent = cell;
                    result = true;
                    goto localReturnResult;
                }

                // No grid?
                if( !cell.CellGrid.HasValue( TargetHandle.Master ) )
                    goto localReturnResult;

                // Store the worldspaces exterior cell in the grid dictionary
                _ByGrid = _ByGrid ?? new Dictionary<Vector2i, Engine.Plugin.Forms.Cell>();

                //if( _ByGrid == null )
                //    DebugLog.WriteLine( "No grid dictionary" + Worldspace == null ? null : string.Format( " for WRLD = {0}", Worldspace.IDString ) );

                _ByGrid[ cell.CellGrid.GetGrid( TargetHandle.Master ) ] = cell;

                result = true;

            localReturnResult:
                //DebugLog.WriteLine( new [] { cell == null ? "SyncObject is not a CELL" : ( persistentCell ? "Persistent " : null ) + ( interiorCell ? "Interior " : null ) + string.Format( "CELL = {1}{0}", cell.IDString, ( !interiorCell && !persistentCell ) ? string.Format( "{0} :: ", cell.CellGrid.GetGrid( TargetHandle.Master ).ToString() ) : null ), Worldspace == null ? null : string.Format( "WRLD = {0}", Worldspace.IDString ) } );
                //DebugLog.CloseIndentLevel( "result", result.ToString() );
                return result;
            }
        }
        
        public override void                    Remove( IXHandle syncObject )
        {
            if( Disposed ) return;
            lock( CollectionLock )
            {
                RemoveEx( syncObject, false );
                if( ( syncObject == null )||( _ByGrid == null ) )
                    return;
            
                // Not a cell?  wth?
                var cell = syncObject as Plugin.Forms.Cell;
                if( cell == null )
                    return;
            
                _ByGrid.Remove( cell.CellGrid.GetGrid( TargetHandle.Master ) );
            }
        }
        
        public Plugin.Forms.Worldspace          Worldspace
        {
            get
            {
                return _Ancestor as Plugin.Forms.Worldspace;
            }
        }
        
    }
    
}
