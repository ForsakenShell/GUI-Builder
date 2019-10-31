/*
 * SyncedListView.cs
 *
 * Generic synced control for listing objects.
 *
 */
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using Engine;
using AnnexTheCommonwealth;
using System.Collections.Generic;


namespace GUIBuilder.Windows.Controls
{
    
    public enum SyncedColumnID
    {
        Checkbox,
        LoadOrder,
        Filename,
        SignatureType,
        FormID,
        EditorID,
        ExtraInfo,
        Invalid
    }
    
    public enum SyncedSortByColumns
    {
        Unsorted,
        LoadOrder,
        Filename,
        Type,
        FormID,
        EditorID,
        ExtraInfo,
        Custom
    }
    
    public enum SyncedSortDirections
    {
        Ascending,
        Descending
    }
    
    /// <summary>
    /// Description of SyncedListView.
    /// </summary>
    public partial class SyncedListView<TSync> : UserControl, GodObject.XmlConfig.IXmlConfiguration
        where TSync : class, Engine.Plugin.Interface.ISyncedGUIObject
    {
        
        // disable StaticFieldInGenericType
        static readonly string XmlSortColumn = "SortBy";
        static readonly string XmlSortDirection = "SortDirection";
        
        static System.Drawing.Font fontBold = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        static System.Drawing.Font fontNormal = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        
        bool onLoadComplete = false;
        
        [Browsable( false )]
        public GodObject.XmlConfig.IXmlConfiguration XmlParent
        {
            get
            {
                var p = Parent;
                while( p != null )
                {
                    var xp = p as GodObject.XmlConfig.IXmlConfiguration;
                    if( xp != null ) return xp;
                    p = p.Parent;
                }
                return null;
            }
        }
        
        [Browsable( false )]
        public string XmlKey { get { return this.Name; } }
        
        [Browsable( false )]
        public string XmlPath { get{ return GodObject.XmlConfig.XmlPathTo( this ); } }
        
        struct Column : GodObject.XmlConfig.IXmlConfiguration
        {
            public static readonly string XmlWidth = "Width";
            
            SyncedListView<TSync> Parent;
            public SyncedColumnID ID;
            public string Name;
            public int DefaultWidth;
            public bool ForceDefaultWidth;
            public bool Show;
            public System.Windows.Forms.ColumnHeader Header;
            public int Index;
            
            public GodObject.XmlConfig.IXmlConfiguration XmlParent { get { return Parent; } }
            
            public string XmlKey { get { return this.Name; } }
            
            public string XmlPath { get{ return GodObject.XmlConfig.XmlPathTo( this ); } }
            
            public Column( SyncedListView<TSync> parent, SyncedColumnID id, string name, int defaultWidth, bool forceDefaultWidth, bool show )
            {
                Parent = parent;
                ID = id;
                Name = name;
                DefaultWidth = defaultWidth;
                ForceDefaultWidth = forceDefaultWidth;
                Show = show;
                Header = null;
                Index = -1;
                if( show )
                {
                    if( !ForceDefaultWidth )
                        DefaultWidth = GodObject.XmlConfig.ReadInt( XmlPath, XmlWidth, defaultWidth );
                    Header = new System.Windows.Forms.ColumnHeader();
                    Header.Text = string.IsNullOrEmpty( Name ) ? "" : Name.Translate();
                    Header.Width = DefaultWidth;
                    Parent.lvSyncObjects.Columns.Add( Header );
                    Index = Parent.lvSyncObjects.Columns.IndexOf( Header );
                    Parent.ColumnCount++;
                }
            }
        }
        
        public class SyncItem
        {
            TSync _SyncObject;
            public TSync GetSyncObject()            { return _SyncObject; }
            ListViewItem _ListItem;
            public ListViewItem GetListItem()       { return _ListItem; }
            
            bool _InListView;
            SyncedListView<TSync> _Parent;
            
            bool _Checked;
            public bool Checked
            {
                get
                {
                    return _Checked;
                }
                set
                {
                    SetChecked( value );
                }
            }
            void SetChecked( bool value )
            {
                if( _ListItem == null ) return;
                if( _Parent.InvokeRequired )
                {
                    _Parent.Invoke( (Action)delegate() { SetChecked( value ); }, null );
                }
                _ListItem.Checked = _SyncObject.ObjectChecked( value );
                _Checked = _ListItem.Checked;
            }
            
            bool _Selected;
            public bool Selected
            {
                get
                {
                    return _Selected;
                }
                set
                {
                    SetSelected( value );
                }
            }
            void SetSelected( bool value )
            {
                if( _ListItem == null ) return;
                if( _Parent.InvokeRequired )
                {
                    _Parent.Invoke( (Action)delegate() { SetSelected( value ); }, null );
                }
                _ListItem.Selected = value;
                _Selected = value;
            }
            
            
            public SyncItem( SyncedListView<TSync> parent, TSync o )
            {
                _Parent = parent;
                _SyncObject = o;
                _InListView = false;
                /*
                DebugLog.Write( string.Format(
                    "{0} :: cTor() :: {1} :: {2} :: {3}",
                    this.GetType().ToString(),
                    parent,
                    o,
                    _InListView ) );
                */
            }
            
            public bool InListView { get{ return( _InListView )&&( _ListItem != null ); } }
            
            public void AddToListView()
            {
                //DebugLog.Write( string.Format(
                //    "{0} :: AddToListView() :: _InListView ? {1} :: O ? {2} :: I ? {3}",
                //    this.GetType().ToString(),
                //    _InListView,
                //    O,
                //    I ) );
                if( ( _SyncObject == null )||( _InListView ) ) return;
                //InvokeInParent();
                /*
                if( _Parent.InvokeRequired )
                {
                    _Parent.Invoke( (Action)delegate() { AddToListView(); }, null );
                    return;
                }
                */
                if( _ListItem == null )
                {
                    //DebugLog.Write( string.Format(
                    //    "{0} :: AddToListView() :: Creating ListView item",
                    //    this.GetType().ToString() ) );
                    
                    if( _Parent.ColumnCount < 1 ) return;
                    var row = RowData();
                    if( row == null ) return;
                    _ListItem = new ListViewItem( row );
                    if( _ListItem == null ) return;
                    var startCorS = _SyncObject.InitialCheckedOrSelectedState();
                    var useChecks = _Parent._CheckboxColumn;
                    Checked = useChecks && startCorS;
                    Selected = !useChecks && startCorS;
                    _ListItem.UseItemStyleForSubItems = true;
                }
                //DebugLog.Write( string.Format(
                //    "{0} :: AddToListView() :: Updating and adding ListView item",
                //    this.GetType().ToString() ) );
                _ListItem.BackColor = _SyncObject.ConflictStatus.GetConflictStatusBackColor();
                _Parent.AddToUpdateList( this );
                UpdateEventHandlers( true );
                _InListView = true;
            }
            
            public void UnRegisterFromListView()
            {
                if( !_InListView ) return;
                //_Parent.AddRemoveSyncItemToListView( this, false ); // <-- Calls this function, infinite circular recursion, wheeee!
                _Parent.AddToUpdateList( this );
                UpdateEventHandlers( false );
                _InListView = false;
            }
            
            string[] RowData()
            {
                if( _Parent.ColumnCount < 1 ) return null;
                //DebugLog.Write( string.Format(
                //    "{0} :: RowData() :: Enter :: _Parent.ColumnCount = {1}",
                //    this.GetType().ToString(),
                //    _Parent.ColumnCount ) );
                var row = new string[ _Parent.ColumnCount ];
                if( _Parent._CheckboxColumn  ) SetRowData( row, SyncedColumnID.Checkbox     , ""                                     );
                if( _Parent._LoadOrderColumn ) SetRowData( row, SyncedColumnID.LoadOrder    , _SyncObject.LoadOrder.ToString( "X2" ) );
                if( _Parent._FilenameColumn  ) SetRowData( row, SyncedColumnID.Filename     , _SyncObject.Filenames[ 0 ]             );
                if( _Parent._TypeColumn      ) SetRowData( row, SyncedColumnID.SignatureType, _SyncObject.Signature                  );
                if( _Parent._FormIDColumn    ) SetRowData( row, SyncedColumnID.FormID       , _SyncObject.GetFormID( Engine.Plugin.TargetHandle.Master ).ToString( "X8" ) );
                if( _Parent._EditorIDColumn  ) SetRowData( row, SyncedColumnID.EditorID     , _SyncObject.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
                if( _Parent._ExtraInfoColumn ) SetRowData( row, SyncedColumnID.ExtraInfo    , _SyncObject.ExtraInfo                  );
                //DebugLog.Write( string.Format(
                //    "{0} :: RowData() :: Exit :: _Parent.ColumnCount = {1} :: row.Length = {2}",
                //    this.GetType().ToString(),
                //    _Parent.ColumnCount,
                //    row.Length ) );
                return row;
            }
            
            void SetRowData( string[] row, SyncedColumnID id, string value )
            {
                var index = _Parent.GetColumnItemIndex( id );
                //DebugLog.Write( string.Format(
                //    "{0} :: SetRowData() :: index = {1} :: ColumnID = {2} :: value = {3}",
                //    this.GetType().ToString(),
                //    index,
                //    id, value ) );
                if( index < 0 ) return;
                if( index >= row.Length ) return;
                row[ index ] = value;
            }
            
            void SetItemData( SyncedColumnID id, string value )
            {
                if( ( _SyncObject == null )||( _ListItem == null ) ) return;
                var index = _Parent.GetColumnItemIndex( id );
                if( index < 0 ) return;
                if( index >= _ListItem.SubItems.Count ) return;
                var si = _ListItem.SubItems[ index ];
                si.Text = value;
                si.Font = _SyncObject.Files.Length > 1 //IsModified
                    ? fontBold
                    : fontNormal;
            }
            
            public void SyncListViewItemWithObject()
            {
                //DebugLog.Write( string.Format( "{0} :: SyncListViewItemWithObject()", this.GetType().ToString() ) );
                //if( ( O == null )||( !_InListView ) ) return;
                if( _SyncObject == null ) return;
                if( _Parent.InvokeRequired )
                {
                    _Parent.Invoke( (Action)delegate() { SyncListViewItemWithObject(); }, null );
                    return;
                }
                _ListItem.BackColor = _SyncObject.ConflictStatus.GetConflictStatusBackColor();
                if( _Parent._LoadOrderColumn ) SetItemData( SyncedColumnID.LoadOrder    , _SyncObject.LoadOrder.ToString( "X2" ) );
                if( _Parent._FilenameColumn  ) SetItemData( SyncedColumnID.Filename     , _SyncObject.Files[ 0 ].Filename        );
                if( _Parent._TypeColumn      ) SetItemData( SyncedColumnID.SignatureType, _SyncObject.Signature                  );
                if( _Parent._FormIDColumn    ) SetItemData( SyncedColumnID.FormID       , _SyncObject.GetFormID( Engine.Plugin.TargetHandle.Master ).ToString( "X8" ) );
                if( _Parent._EditorIDColumn  ) SetItemData( SyncedColumnID.EditorID     , _SyncObject.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
                if( _Parent._ExtraInfoColumn ) SetItemData( SyncedColumnID.ExtraInfo    , _SyncObject.ExtraInfo                  );
            }
            
            public void UpdateEventHandlers( bool register )
            {
                //DebugLog.Write( string.Format( "\n{0} :: UpdateEventHandlers() :: register = {1}", this.GetType().ToString(), register ) );
                if( _SyncObject == null )
                    return;
                if( register )
                {
                    if( _ListItem == null )
                        return;
                    _SyncObject.ObjectDataChanged += OnSyncObjectDataChanged;
                }
                else
                    _SyncObject.ObjectDataChanged -= OnSyncObjectDataChanged;
            }
            
            #region Custom Events
            
            // Update the list view if the sync object changed
            void OnSyncObjectDataChanged( object sender, EventArgs e )
            {
                //DebugLog.Write( string.Format( "{0} :: OnSyncObjectDataChanged", this.GetType().ToString() ) );
                var syncobject = sender as TSync;
                if( ( syncobject == null )||( syncobject != _SyncObject ) ) return;
                SyncListViewItemWithObject();
            }
            
            #endregion
            
        }
        
        public Comparison<SyncItem> CustomAscendingSort = null;
        public Comparison<SyncItem> CustomDescendingSort = null;
        Comparison<SyncItem> Sorter = null;
        
        bool _AllowOverrideColumnSorting = true;
        bool _AllowHidingItems = true;
        
        protected bool _CheckboxColumn = false;
        protected bool _LoadOrderColumn = false;
        protected bool _FilenameColumn = false;
        protected bool _TypeColumn = false;
        protected bool _FormIDColumn = true;
        protected bool _EditorIDColumn = true;
        protected bool _ExtraInfoColumn = false;
        
        Column[] Columns = null;
        protected int ColumnCount = -1;
        
        Type _SyncedEditorFormType = null;
        
        List<ListViewItem> _ItemUpdateList = null;
        
        List<SyncItem> _SyncItems = null;
        SyncedSortByColumns _SortByColumn = SyncedSortByColumns.Unsorted;
        SyncedSortDirections _SortDirection = SyncedSortDirections.Ascending;
        
        public SyncedListView()
        {
            InitializeComponent();
        }
        
        void SyncedListViewLoad( object sender, EventArgs e )
        {
            //DebugLog.Write( string.Format( "\n{0} :: SyncedListViewLoad() :: Name = {1}", this.GetType().ToString(), this.Name ) );
            this.Translate( true );
            
            RecreateColumns();
            Sorter = null;
            if( AllowOverrideColumnSorting )
            {
                _SortByColumn = (SyncedSortByColumns)GodObject.XmlConfig.ReadInt( XmlPath, XmlSortColumn, (int)_SortByColumn );
                _SortDirection = (SyncedSortDirections)GodObject.XmlConfig.ReadInt( XmlPath, XmlSortColumn, (int)_SortDirection );
            }
            onLoadComplete = true;
        }
        
        #region Column Management
        
        void RecreateColumns()
        {
            onLoadComplete = false;
            RegisterListViewForEvents( false );
            lvSyncObjects.Columns.Clear();
            ColumnCount = 0;
            Columns = new Column[]{
                new Column( this, SyncedColumnID.Checkbox     , ""                        ,  24, true , _CheckboxColumn  ),
                new Column( this, SyncedColumnID.LoadOrder    , "SyncedListView.LoadOrder",  28, true , _LoadOrderColumn ),
                new Column( this, SyncedColumnID.Filename     , "SyncedListView.Filename" , 160, false, _FilenameColumn  ),
                new Column( this, SyncedColumnID.SignatureType, "SyncedListView.Type"     ,  60, false, _TypeColumn      ),
                new Column( this, SyncedColumnID.FormID       , "Form.FormID"             ,  80, true , _FormIDColumn    ),
                new Column( this, SyncedColumnID.EditorID     , "Form.EditorID"           , 256, false, _EditorIDColumn  ),
                new Column( this, SyncedColumnID.ExtraInfo    , "SyncedListView.ExtraInfo", 512, false, _ExtraInfoColumn )
            };
            RegisterListViewForEvents( true );
            lvSyncObjects.Enabled = true;
            onLoadComplete = true;
        }
        
        protected int GetColumnItemIndex( SyncedColumnID id )
        {
            var idx = (int)id;
            if( ( idx < 0 )||( idx >= Columns.Length ) )
                return -1;
            return Columns[ idx ].Index;
        }
        
        void UpdateColumnWidth( int index )
        {
            if( Columns.NullOrEmpty() )
                return;
            foreach( var column in Columns )
            {
                if( column.Index == index )
                {
                    if( column.ForceDefaultWidth )
                        column.Header.Width = column.DefaultWidth;
                    else if( onLoadComplete )
                        GodObject.XmlConfig.WriteInt( column.XmlPath, Column.XmlWidth, column.Header.Width, true );
                }
            }
        }
        
        #endregion
        
        #region Form Designer Properties
        
        public bool AllowOverrideColumnSorting
        {
            get{ return _AllowOverrideColumnSorting; }
            set
            {
                _AllowOverrideColumnSorting = value;
                Sorter = null;
            }
        }
        
        public bool AllowHidingItems
        {
            get{ return _AllowHidingItems; }
            set
            {
                _AllowHidingItems = value;
                tsmiDividerHideUnchanged.Visible = value;
                tsmiHideUnchanged.Visible = value;
            }
        }
        
        public SyncedSortByColumns SortByColumn
        {
            get{ return _SortByColumn; }
            set
            {
                _SortByColumn = value;
                GodObject.XmlConfig.WriteInt( XmlPath, XmlSortColumn, (int)_SortByColumn, true );
                Sorter = null;
                RepopulateListView();
            }
        }
        
        public SyncedSortDirections SortDirection
        {
            get{ return _SortDirection; }
            set
            {
                _SortDirection = value;
                GodObject.XmlConfig.WriteInt( XmlPath, XmlSortDirection, (int)_SortDirection, true );
                Sorter = null;
                RepopulateListView();
            }
        }
        
        public bool CheckBoxes
        {
            get { return _CheckboxColumn; }
            set
            {
                _CheckboxColumn = value;
                lvSyncObjects.CheckBoxes = value;
                if( onLoadComplete )
                    RecreateColumns();
            }
        }
        
        public bool LoadOrderColumn
        {
            get { return _LoadOrderColumn; }
            set
            {
                _LoadOrderColumn = value;
                if( onLoadComplete )
                    RecreateColumns();
            }
        }
        
        public bool FilenameColumn
        {
            get { return _FilenameColumn; }
            set
            {
                _FilenameColumn = value;
                if( onLoadComplete )
                    RecreateColumns();
            }
        }
        
        public bool TypeColumn
        {
            get { return _TypeColumn; }
            set
            {
                _TypeColumn = value;
                if( onLoadComplete )
                    RecreateColumns();
            }
        }
        
        public bool FormIDColumn
        {
            get { return _FormIDColumn; }
            set
            {
                _FormIDColumn = value;
                if( onLoadComplete )
                    RecreateColumns();
            }
        }
        
        public bool EditorIDColumn
        {
            get { return _EditorIDColumn; }
            set
            {
                _EditorIDColumn = value;
                if( onLoadComplete )
                    RecreateColumns();
            }
        }
        
        public bool ExtraInfoColumn
        {
            get { return _ExtraInfoColumn; }
            set
            {
                _ExtraInfoColumn = value;
                if( onLoadComplete )
                    RecreateColumns();
            }
        }
        
        public bool MultiSelect
        {
            get { return lvSyncObjects.MultiSelect; }
            set { lvSyncObjects.MultiSelect = value; }
        }
        
        #endregion
        
        #region External Sync Item API
        
        bool _SyncSetObjects = false;
        [Browsable( false )]
        public List<TSync> SyncObjects
        {
            get
            {
                while( _SyncSetObjects )
                    System.Threading.Thread.Sleep( 0 );
                if( _SyncItems.NullOrEmpty() )
                    return null;
                var list = new List<TSync>();
                foreach( var syncitem in _SyncItems )
                    list.Add( syncitem.GetSyncObject() );
                return list;
            }
            set
            {
                while( _SyncSetObjects )
                    System.Threading.Thread.Sleep( 0 );
                _SyncSetObjects = true;
                //DebugLog.Write( string.Format( "\n{0} :: SyncObjects_set() :: value ? {1}", this.GetType().ToString(), value == null ? "false" : "true" ) );
                EnableListView( false );            // Repopulating will re-enable
                AddRemoveSyncItemsToList( false );  // Remove all items, this will also de-register them for all events
                if( value.NullOrEmpty() )
                {
                    _SyncItems = null;              // Just clear the list of synced objects and items
                    EnableListView( true );
                    _SyncSetObjects = false;
                    return;
                }
                _SyncItems = new List<SyncItem>();  // Create and populate a new list
                foreach( var syncobject in value )
                    if( syncobject != null )
                        _SyncItems.Add( new SyncItem( this, syncobject ) );
                RepopulateListViewInternal();               // Repopulate the list view, this will register the new list for all appropriate events
            }
        }
        
        public List<TSync> GetSelectedSyncObjects()
        {
            while( _SyncSetObjects )
                System.Threading.Thread.Sleep( 0 );
            //if( _SyncSetObjects ) return null;
            if( _SyncItems.NullOrEmpty() ) return null;
            var list = new List<TSync>();
            var byCheckbox = _CheckboxColumn;
            foreach( var syncitem in _SyncItems )
            {
                if( syncitem.InListView )
                {
                    if(
                        ( (  byCheckbox )&&( syncitem.Checked  ) )||
                        ( ( !byCheckbox )&&( syncitem.Selected ) )
                    )
                    {
                        list.Add( syncitem.GetSyncObject() );
                    }
                }
            }
            return list.Count == 0
                ? null
                : list;
        }
        
        [Browsable( false )]
        public bool AnythingSelected
        {
            get
            {
                while( _SyncSetObjects )
                    System.Threading.Thread.Sleep( 0 );
                //if( _SyncSetObjects ) return false;
                if( _SyncItems.NullOrEmpty() ) return false;
                var byCheckbox = _CheckboxColumn;
                foreach( var syncitem in _SyncItems )
                {
                    if( syncitem.InListView )
                    {
                        if(
                            ( (  byCheckbox )&&( syncitem.Checked  ) )||
                            ( ( !byCheckbox )&&( syncitem.Selected ) )
                        )
                            return true;
                    }
                }
                return false;
            }
        }
        
        public List<TSync> GetVisibleSyncObjects()
        {
            while( _SyncSetObjects )
                System.Threading.Thread.Sleep( 0 );
            if( _SyncItems.NullOrEmpty() ) return null;
            var list = new List<TSync>();
            foreach( var syncitem in _SyncItems )
            {
                if( syncitem.InListView )
                    list.Add( syncitem.GetSyncObject() );
            }
            return list.Count == 0
                ? null
                : list;
        }
        
        public TSync SyncObjectFromListViewItem( ListViewItem item )
        {
            while( _SyncSetObjects )
                System.Threading.Thread.Sleep( 0 );
            var syncitem = SyncItemFromListViewItem( item );
            return syncitem == null
                ? null
                : syncitem.GetSyncObject();
        }
        
        [Browsable( false )]
        public Type SyncedEditorFormType
        {
            get{ return _SyncedEditorFormType; }
            set
            {
                _SyncedEditorFormType = value;
                if( value != null )
                {
                    lvSyncObjects.MouseClick += lvSyncObjectItemMouseClick;
                    lvSyncObjects.ItemActivate += lvSyncObjectsItemActivate;
                }
                else
                {
                    lvSyncObjects.MouseClick -= lvSyncObjectItemMouseClick;
                    lvSyncObjects.ItemActivate -= lvSyncObjectsItemActivate;
                }
            }
        }
        
        #endregion
        
        #region Control management
        
        public void RepopulateListView()
        {
            if( !onLoadComplete ) return;
            while( _SyncSetObjects )
                System.Threading.Thread.Sleep( 0 );
            _SyncSetObjects = true;
            //DebugLog.Write( string.Format( "\n{0} :: RepopulateListView() :: _SyncItems ? {1}", this.GetType().ToString(), _SyncItems == null ? "false" : "true" ) );
            RepopulateListViewInternal();
        }
        
        void RepopulateListViewInternal()
        {
            //DebugLog.Write( string.Format( "\n{0} :: RepopulateListViewInternal() :: _SyncItems ? {1}", this.GetType().ToString(), _SyncItems == null ? "false" : "true" ) );
            var wt = WorkerThreadPool.CreateWorker( RepopulateListViewThread, null );
            if( wt != null )
                wt.Start();
            else
                throw new Exception( string.Format(
                    "{0} :: RepopulateListViewInternal() :: Unable to create worker thread to repopulate sync list",
                    this.GetType().ToString()
                   ) );
        }
        
        void RepopulateListViewThread()
        {
            if( this.InvokeRequired )
            {
                //DebugLog.WriteLine( string.Format( "\n{0} :: RepopulateListViewThread() :: UI Thread Invoke\n", this.GetType().ToString() ) );
                //try{
                    this.Invoke( (Action)delegate() { RepopulateListViewThread(); }, null );
                //} catch( Exception e ){
                //    var m = string.Format( "An exception occured attempting UI Thread Invoke\n{0}\n{1}", e.ToString(), e.StackTrace );
                //    Console.WriteLine( string.Format( "{0} :: {1} :: {2}", this.GetType().ToString(), "RepopulateListViewThread()", m ) );
                //    DebugLog.WriteError( this.GetType().ToString(), "RepopulateListViewThread()", m );
                //}
                return;
            }
            if( !_SyncItems.NullOrEmpty() )
            {
                //DebugLog.Write( string.Format( "\n{0} :: RepopulateListViewThread() :: Start\n", this.GetType().ToString() ) );
                _SyncSetObjects  = true;
                EnableListView( false );
                var hideUnecessaryImports = tsmiHideUnchanged.Checked;
                
                SortObjects();
                
                // First, remove them all
                AddRemoveSyncItemsToList( false );
                
                // Now build a list of items to add
                _ItemUpdateList = new List<ListViewItem>();
                //DebugLog.Write( string.Format( "{0} :: RepopulateListViewThread() :: List :: Start", this.GetType().ToString() ) );
                foreach( var syncitem in _SyncItems )
                {
                    //DebugLog.Write( string.Format( "{0} :: RepopulateListViewThread() :: List :: ConflictStatus", this.GetType().ToString() ) );
                    var conflictStatus = syncitem.GetSyncObject().ConflictStatus;
                    
                    //DebugLog.Write( string.Format( "{0} :: RepopulateListViewThread() :: List :: AddToListView", this.GetType().ToString() ) );
                    if(
                        ( !hideUnecessaryImports )||
                        ( conflictStatus == Engine.Plugin.ConflictStatus.NewForm )||
                        ( conflictStatus == Engine.Plugin.ConflictStatus.RequiresOverride )
                    )
                        syncitem.AddToListView();
                }
                //DebugLog.Write( string.Format( "{0} :: RepopulateListViewThread() :: List :: Complete", this.GetType().ToString() ) );
                AddRemoveList( true );  // And add them
                _ItemUpdateList = null; // Done with the list
                
                //DebugLog.Write( string.Format( "\n{0} :: RepopulateListViewThread() :: Complete", this.GetType().ToString() ) );
            }
            EnableListView( true );
            _SyncSetObjects = false;
        }
        
        void RegisterListViewForEvents( bool register )
        {
            //DebugLog.Write( string.Format( "\n{0} :: RegisterListViewForEvents() :: register = {1}\n{2}", this.GetType().ToString(), register, System.Environment.StackTrace ) );
            if( register )
            {
                lvSyncObjects.ColumnClick           += lvSyncObjectsColumnClick;
                lvSyncObjects.ColumnWidthChanged    += lvSyncObjectsColumnWidthChanged;
                lvSyncObjects.ColumnWidthChanging   += lvSyncObjectsColumnWidthChanging;
                lvSyncObjects.ItemChecked           += lvSyncObjectsItemChecked;
                lvSyncObjects.ItemSelectionChanged  += lvSyncObjectsItemSelectionChanged;
            }
            else
            {
                lvSyncObjects.ColumnClick           -= lvSyncObjectsColumnClick;
                lvSyncObjects.ColumnWidthChanged    -= lvSyncObjectsColumnWidthChanged;
                lvSyncObjects.ColumnWidthChanging   -= lvSyncObjectsColumnWidthChanging;
                lvSyncObjects.ItemChecked           -= lvSyncObjectsItemChecked;
                lvSyncObjects.ItemSelectionChanged  -= lvSyncObjectsItemSelectionChanged;
            }
        }
        
        void EnableListView( bool enable )
        {
            if( this.InvokeRequired )
            {
                this.Invoke( (Action)delegate() { EnableListView( enable ); }, null );
                return;
            }
            //DebugLog.Write( string.Format( "\n{0} :: EnableListView() :: enable = {1}", this.GetType().ToString(), enable ) );
            RegisterListViewForEvents( enable );
            //lvSyncObjects.Enabled = enable;
        }
        
        #region Sorting Delegates
        
        static int SortLoadOrderAsc( SyncItem x, SyncItem y )
        {   return x.GetSyncObject().LoadOrder > y.GetSyncObject().LoadOrder ? 1 : x.GetSyncObject().LoadOrder < y.GetSyncObject().LoadOrder ? -1 : 0; }
        static int SortLoadOrderDes( SyncItem x, SyncItem y )
        {   return x.GetSyncObject().LoadOrder < y.GetSyncObject().LoadOrder ? 1 : x.GetSyncObject().LoadOrder > y.GetSyncObject().LoadOrder ? -1 : 0; }
        
        static int SortFilenameAsc( SyncItem x, SyncItem y )
        {   return string.Compare( x.GetSyncObject().Files[ 0 ].Filename, y.GetSyncObject().Files[ 0 ].Filename, StringComparison.InvariantCultureIgnoreCase ); }
        static int SortFilenameDes( SyncItem x, SyncItem y )
        {   return string.Compare( y.GetSyncObject().Files[ 0 ].Filename, x.GetSyncObject().Files[ 0 ].Filename, StringComparison.InvariantCultureIgnoreCase ); }
        
        static int SortSignatureAsc( SyncItem x, SyncItem y )
        {   return string.Compare( x.GetSyncObject().Signature, y.GetSyncObject().Signature, StringComparison.InvariantCultureIgnoreCase ); }
        static int SortSignatureDes( SyncItem x, SyncItem y )
        {   return string.Compare( x.GetSyncObject().Signature, y.GetSyncObject().Signature, StringComparison.InvariantCultureIgnoreCase ); }
        
        static int SortFormIDAsc( SyncItem x, SyncItem y )
        {   return x.GetSyncObject().GetFormID( Engine.Plugin.TargetHandle.Master ) > y.GetSyncObject().GetFormID( Engine.Plugin.TargetHandle.Master ) ? 1 : x.GetSyncObject().GetFormID( Engine.Plugin.TargetHandle.Master ) < y.GetSyncObject().GetFormID( Engine.Plugin.TargetHandle.Master ) ? -1 : 0; }
        static int SortFormIDDes( SyncItem x, SyncItem y )
        {   return x.GetSyncObject().GetFormID( Engine.Plugin.TargetHandle.Master ) < y.GetSyncObject().GetFormID( Engine.Plugin.TargetHandle.Master ) ? 1 : x.GetSyncObject().GetFormID( Engine.Plugin.TargetHandle.Master ) > y.GetSyncObject().GetFormID( Engine.Plugin.TargetHandle.Master ) ? -1 : 0; }
        
        static int SortEditorIDAsc( SyncItem x, SyncItem y )
        {   return string.Compare( x.GetSyncObject().GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ), y.GetSyncObject().GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ), StringComparison.InvariantCultureIgnoreCase ); }
        static int SortEditorIDDes( SyncItem x, SyncItem y )
        {   return string.Compare( y.GetSyncObject().GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ), x.GetSyncObject().GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ), StringComparison.InvariantCultureIgnoreCase ); }
        
        static int SortExtraInfoAsc( SyncItem x, SyncItem y )
        {   return string.Compare( x.GetSyncObject().ExtraInfo, y.GetSyncObject().ExtraInfo, StringComparison.InvariantCultureIgnoreCase ); }
        static int SortExtraInfoDes( SyncItem x, SyncItem y )
        {   return string.Compare( x.GetSyncObject().ExtraInfo, y.GetSyncObject().ExtraInfo, StringComparison.InvariantCultureIgnoreCase ); }
        
        void GetSorter()
        {
            Sorter = null;
            switch( _SortByColumn )
            {
                case SyncedSortByColumns.LoadOrder:
                    if( _SortDirection == SyncedSortDirections.Ascending ) Sorter = SortLoadOrderAsc;
                    else Sorter = SortLoadOrderDes;
                    break;
                    
                case SyncedSortByColumns.Filename:
                    if( _SortDirection == SyncedSortDirections.Ascending ) Sorter = SortFilenameAsc;
                    else Sorter = SortFilenameDes;
                    break;
                    
                case SyncedSortByColumns.Type:
                    if( _SortDirection == SyncedSortDirections.Ascending ) Sorter = SortSignatureAsc;
                    else Sorter = SortSignatureDes;
                    break;
                    
                case SyncedSortByColumns.FormID:
                    if( _SortDirection == SyncedSortDirections.Ascending ) Sorter = SortFormIDAsc;
                    else Sorter = SortFormIDDes;
                    break;
                    
                case SyncedSortByColumns.EditorID:
                    if( _SortDirection == SyncedSortDirections.Ascending ) Sorter = SortEditorIDAsc;
                    else Sorter = SortEditorIDDes;
                    break;
                    
                case SyncedSortByColumns.ExtraInfo:
                    if( _SortDirection == SyncedSortDirections.Ascending ) Sorter = SortExtraInfoAsc;
                    else Sorter = SortExtraInfoDes;
                    break;
                    
                case SyncedSortByColumns.Custom:
                    if( _SortDirection == SyncedSortDirections.Ascending ) Sorter = CustomAscendingSort;
                    else Sorter = CustomDescendingSort;
                    break;
                    
            }
        }
        
        #endregion
        
        #region Sync Item management
        
        void SortObjects()
        {
            if( ( _SyncItems.NullOrEmpty() )||( _SortByColumn == SyncedSortByColumns.Unsorted ) ) return;
            if( Sorter == null )
                GetSorter();
            if( Sorter != null )
                _SyncItems.Sort( Sorter );
        }
        
        SyncItem SyncItemFromListViewItem( ListViewItem item )
        {
            if( ( _SyncItems.NullOrEmpty() )||( item == null ) ) return null;
            foreach( var syncitem in _SyncItems )
                if( syncitem.GetListItem() == item )
                    return syncitem;
            return null;
        }
        
        SyncItem SyncItemFromSyncObject( TSync syncobject )
        {
            if( ( _SyncItems.NullOrEmpty() )||( syncobject == null ) ) return null;
            foreach( var syncitem in _SyncItems )
                if( syncitem.GetSyncObject() == syncobject )
                    return syncitem;
            return null;
        }
        
        void UpdateCheckSelectionForSyncItem( SyncItem syncitem, bool checkState )
        {
            var byCheckbox = lvSyncObjects.CheckBoxes;
            if( byCheckbox )
                syncitem.Checked = checkState;
            else
            {
                syncitem.Checked = false;
                syncitem.Selected = checkState;
            }
        }
        
        void UpdateListViewCheckSelection( bool selectChangedOrNewOnly, bool checkState )
        {
            
            foreach( var item in lvSyncObjects.Items )
            {
                var lvi = (ListViewItem)item;
                if( lvi == null ) continue;
                var syncitem = SyncItemFromListViewItem( lvi );
                if( syncitem == null ) continue;
                
                if( !selectChangedOrNewOnly )
                {
                    UpdateCheckSelectionForSyncItem( syncitem, checkState );
                }
                else
                {
                    //var syncitem = SyncItemFromListViewItem( lvi );
                    //if( syncitem == null )
                    //{
                    //    UpdateCheckSelectionForSyncItem( lvi, false );
                    //    continue;
                    //}
                    var conflictStatus = syncitem.GetSyncObject().ConflictStatus;
                    lvi.BackColor = conflictStatus.GetConflictStatusBackColor();
                    UpdateCheckSelectionForSyncItem(
                        syncitem, 
                        ( checkState )&&
                        (
                            ( conflictStatus == Engine.Plugin.ConflictStatus.NewForm )||
                            ( conflictStatus == Engine.Plugin.ConflictStatus.RequiresOverride )
                        ) );
                }
            }
        }
        
        protected void AddToUpdateList( SyncItem syncitem )
        {
            //DebugLog.Write( string.Format( "\n{0} :: AddToUpdateList() :: syncitem = {1}", this.GetType().ToString(), syncitem ) );
            if( _ItemUpdateList == null ) return;
            //if( _ItemUpdateList.IndexOf( syncitem.GetListItem() ) >= 0 ) return;
            _ItemUpdateList.AddOnce( syncitem.GetListItem() );
        }
        
        void AddRemoveSyncItemsToList( bool addremove )
        {
            //DebugLog.Write( string.Format( "\n{0} :: AddRemoveSyncItemsToList() :: addremove = {1}", this.GetType().ToString(), addremove ) );
            if( _SyncItems.NullOrEmpty() ) return;
            
            // null for remove to just unregister, will clear as a whole after
            _ItemUpdateList = null;
            if( addremove )
                _ItemUpdateList = new List<ListViewItem>();
            
            foreach( var syncitem in _SyncItems )
                AddRemoveSyncItemToListView( syncitem, addremove );
            
            AddRemoveList( addremove );
            _ItemUpdateList = null;
        }
        
        void AddRemoveList( bool addremove )
        {
            if( this.InvokeRequired )
            {
                this.Invoke( (Action)delegate() { AddRemoveList( addremove ); }, null );
                return;
            }
            //DebugLog.Write( string.Format( "\n{0} :: AddRemoveList() :: list ? {1} :: addremove = {2}", this.GetType().ToString(), _ItemUpdateList == null ? "false" : "true", addremove ) );
            if( addremove )
            {
                if( !_ItemUpdateList.NullOrEmpty() )
                    lvSyncObjects.Items.AddRange( _ItemUpdateList.ToArray() );
            }
            else
            {
                if( _ItemUpdateList.NullOrEmpty() )
                    lvSyncObjects.Items.Clear();
                else
                    foreach( var item in _ItemUpdateList )
                        lvSyncObjects.Items.Remove( item );
            }
        }
        
        void AddRemoveSyncItemToListView( SyncItem syncitem, bool addremove )
        {
            if( syncitem == null ) return;
            if( addremove )
                syncitem.AddToListView();
            else
                syncitem.UnRegisterFromListView();
        }
        
        #endregion
        
        #endregion
        
        #region GUI Events
        
        #region Menu Object Clicks
        
        void cmsSyncObjectsClosed( object sender, ToolStripDropDownClosedEventArgs e )
        {
            tsmiEditObject.Visible = false;
            tsmiDividerEditObject.Visible = false;
        }
        
        void tsmiEditObjectClick( object sender, EventArgs e )
        {
            ShowEditorForm();
        }
        
        void tsmiSelectAllClick( object sender, EventArgs e )
        {
            // All checked
            UpdateListViewCheckSelection( false, true );
        }
        
        void tsmiSelectNoneClick( object sender, EventArgs e )
        {
            // None checked
            UpdateListViewCheckSelection( false, false );
        }
        
        void tsmiOnlyChangedOrNewClick( object sender, EventArgs e )
        {
            // Only changed or new checked
            UpdateListViewCheckSelection( true, true );
        }
        
        void tsmiHideUnchangedClick( object sender, EventArgs e )
        {
            // Toggle hiding all same-as-parent sub-divisions
            tsmiHideUnchanged.Checked = !tsmiHideUnchanged.Checked;
            RepopulateListView();
            //lvSyncObjects.Enabled = true;
        }
        
        #endregion
        
        #region ListView Clicks
        
        void lvSyncObjectsColumnClick( object sender, ColumnClickEventArgs e )
        {
            if( !_AllowOverrideColumnSorting )
                return;
            var lastColumn = _SortByColumn;
            if( ( _LoadOrderColumn )&&( e.Column == GetColumnItemIndex( SyncedColumnID.LoadOrder     ) ) ) _SortByColumn = SyncedSortByColumns.LoadOrder;
            if( ( _FilenameColumn  )&&( e.Column == GetColumnItemIndex( SyncedColumnID.Filename      ) ) ) _SortByColumn = SyncedSortByColumns.Filename;
            if( ( _TypeColumn      )&&( e.Column == GetColumnItemIndex( SyncedColumnID.SignatureType ) ) ) _SortByColumn = SyncedSortByColumns.Type;
            if( ( _FormIDColumn    )&&( e.Column == GetColumnItemIndex( SyncedColumnID.FormID        ) ) ) _SortByColumn = SyncedSortByColumns.FormID;
            if( ( _EditorIDColumn  )&&( e.Column == GetColumnItemIndex( SyncedColumnID.EditorID      ) ) ) _SortByColumn = SyncedSortByColumns.EditorID;
            if( ( _ExtraInfoColumn )&&( e.Column == GetColumnItemIndex( SyncedColumnID.ExtraInfo     ) ) ) _SortByColumn = SyncedSortByColumns.ExtraInfo;
            var lastDirection = _SortDirection;
            if( lastColumn == _SortByColumn )
                _SortDirection = _SortDirection == SyncedSortDirections.Ascending ? SyncedSortDirections.Descending : SyncedSortDirections.Ascending;
            if( lastColumn != _SortByColumn )
                GodObject.XmlConfig.WriteInt( XmlPath, XmlSortColumn, (int)_SortByColumn, true );
            if( lastDirection != _SortDirection )
                GodObject.XmlConfig.WriteInt( XmlPath, XmlSortDirection, (int)_SortDirection, true );
            Sorter = null;
            RepopulateListView();
        }
        
        void lvSyncObjectsColumnWidthChanged( object sender, ColumnWidthChangedEventArgs e )
        {
            UpdateColumnWidth( e.ColumnIndex );
        }
        
        void lvSyncObjectsColumnWidthChanging( object sender, ColumnWidthChangingEventArgs e )
        {
            UpdateColumnWidth( e.ColumnIndex );
        }
        
        void lvSyncObjectItemMouseClick( object sender, MouseEventArgs e )
        {
            if( ( e.Button == MouseButtons.Right )&&( SyncedEditorFormType != null ) )
            {
                tsmiEditObject.Visible = true;
                tsmiDividerEditObject.Visible = true;
            }
        }
        
        void lvSyncObjectsItemActivate( object sender, EventArgs e )
        {
            ShowEditorForm();
        }
        
        #endregion
        
        #endregion
        
        #region Custom Events and Callbacks
        
        event EventHandler _ItemSelectionChanged;
        
        public event EventHandler ItemSelectionChanged
        {
            add { _ItemSelectionChanged += value; }
            remove { ItemSelectionChanged -= value; }
        }
        
        void lvSyncObjectsItemChecked( object sender, ItemCheckedEventArgs e )
        {
            var syncitem = SyncItemFromListViewItem( e.Item );
            if( syncitem == null ) return;
            syncitem.Checked = syncitem.GetListItem().Checked;
        }
        
        void lvSyncObjectsItemSelectionChanged( object sender, ListViewItemSelectionChangedEventArgs e )
        {
            var lie = e as ListViewItemSelectionChangedEventArgs;
            if( lie != null )
            {
                var syncitem = SyncItemFromListViewItem( lie.Item );
                if( syncitem != null )
                    syncitem.Selected = lie.IsSelected;
            }
            EventHandler handler = _ItemSelectionChanged;
            if( handler != null )
                handler( this, e );
        }
        
        void ShowEditorForm()
        {
            if( SyncedEditorFormType == null ) return;
            while( _SyncSetObjects )
                System.Threading.Thread.Sleep( 0 );
            if( _SyncItems.NullOrEmpty() ) return;
            if( lvSyncObjects.Items.Count == 0 ) return;
            _SyncSetObjects = true;
            //DebugLog.Write( string.Format( "\n{0} :: ShowEditorForm() :: SyncedEditorFormType = {1}", this.GetType().ToString(), SyncedEditorFormType.ToString() ) );
            
            foreach( var syncitem in _SyncItems )
            {
                if( !syncitem.InListView ) continue;
                if( !syncitem.Selected ) continue;
                
                var editorFormObject = Activator.CreateInstance( _SyncedEditorFormType, new Object[] { syncitem.GetSyncObject() } );
                if( editorFormObject == null ) continue;
                
                var editorForm = editorFormObject as FormEditor.SyncedFormEditor<TSync>;
                if( editorForm == null ) continue;
                
                editorForm.Show();
                
            }
            
            _SyncSetObjects = false;
        }
        
        #endregion
        
   }
}
