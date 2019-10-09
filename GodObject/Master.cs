/*
 * Master.cs
 * 
 * Required master file base class required by GUIBuilder, some masters are optional and functionality dependant on those masters will be disabled.
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace GodObject
{
    
    #region Master files
    
    public static class Master
    {
        const string                    XmlNode                     = "AlwaysSelectMasters";
        const string                    XmlMaster                   = "Master";
        const string                    XmlFilename                 = "Filename";
        const string                    XmlAlwaysSelect             = "AlwaysSelect";
        
        public static class Filename
        {
            public static readonly string Fallout4                  = "Fallout4.esm";
            public static readonly string AnnexTheCommonwealth      = "AnnexTheCommonwealth.esm";
            public static readonly string SimSettlements            = "SimSettlements.esm";
        }
        
        public class File : Engine.Plugin.Interface.ISyncedGUIObject
        {
            
            public Engine.Plugin.File   Mod                         = null;
            public string               Name                        = null;
            
            bool                       _UserDeleteable              = false;
            public bool                 UserDeleteable              { get { return _UserDeleteable; } }
            
            bool                       _UserToggleable              = false;
            public bool                 UserToggleable              { get { return _UserToggleable; } }
            
            bool                       _AlwaysSelect                = false;
            public bool                 AlwaysSelect
            {
                get { return _AlwaysSelect; }
                set
                {
                    if( _UserToggleable )
                    {
                        _AlwaysSelect = value;
                        var node = GetXmlNode( true );
                        if( node != null )
                            XmlConfig.WriteNodeValue( node, XmlAlwaysSelect, _AlwaysSelect.ToString(), true );
                    }
                    SendObjectDataChangedEvent();
                }
            }
            
            public bool                 Loaded                      { get { return Mod != null; } }
            
            public                      File( string name, bool userDeleteable, bool userToggleable, bool alwaysSelect )
            {
                //DebugLog.WriteLine( new [] { this.GetType().ToString(), "cTor()", name } );
                Name            = name;
                _UserDeleteable = userDeleteable;
                _UserToggleable = userToggleable;
                _AlwaysSelect   = alwaysSelect;
            }
            
            public XmlNode              GetXmlNode( bool createNode = false )
            {
                var masters = XmlConfig.GetNodes( XmlNode + '/' + XmlMaster  );
                if( masters != null )
                {
                    foreach( XmlNode master in masters )
                    {
                        var name = XmlConfig.ReadNodeValue( master, XmlFilename, null );
                        if( ( name != null )&&( Name.InsensitiveInvariantMatch( name ) ) )
                           return master;
                    }
                }
                if( !createNode )
                    return null;
                var almnode = XmlConfig.GetNode( XmlNode );
                if( almnode == null )
                {
                    almnode = XmlConfig.MakeXPath( XmlNode );
                    if( almnode == null )
                        return null;
                }
                
                var node = XmlConfig.AppendNode( almnode, XmlMaster );
                if( node == null )
                    return null;
                XmlConfig.WriteNodeValue( node, XmlFilename, Name );
                return node;
            }
            
            bool                        FileExists
            {
                get
                {
                    return System.IO.File.Exists( GodObject.Paths.Fallout4Data + Name );
                }
            }
            
            #region ISyncedListViewObject
            
            public Engine.Plugin.File[] Files
            {
                get
                {
                    var f = GodObject.Plugin.Data.Files.Find( Name );
                    return f == null
                        ? null
                        : new []{ f };
                }
            }
            
            public string[] Filenames
            {
                get
                {
                    return new [] { Name };
                }
            }
            
            public string               Filename                    { get { return Name; } }
            
            public string               Signature
            {
                get
                {
                    return Name.EndsWith( ".esm", StringComparison.InvariantCultureIgnoreCase )
                        ? "Master (ESM)"
                        : Name.EndsWith( ".esl", StringComparison.InvariantCultureIgnoreCase )
                        ? "Light Plugin (ESL)"
                        : Name.EndsWith( ".esp", StringComparison.InvariantCultureIgnoreCase )
                        ? "Plugin (ESP)"
                        : "Unknown";
                }
            }
            
            public uint                 LoadOrder
            {
                get
                {
                    if( !GodObject.Plugin.IsLoaded )
                        return FindInLoadOrder( Name );
                    return Mod != null
                        ? Mod.LoadOrder
                        : 0xFF;
                }
            }
            
            /*
            public uint                 FormID                      { get { return LoadOrder << 24; } }
            public string               EditorID
            {
                get { return Name; }
                set { throw new NotImplementedException(); }
            }
            */
            
            public uint                 GetFormID( Engine.Plugin.TargetHandle target )
            {
                return LoadOrder << 24;
            }
            public void                 SetFormID( Engine.Plugin.TargetHandle target, uint value )
            {
                throw new NotImplementedException();
            }
            
            public string               GetEditorID( Engine.Plugin.TargetHandle target )
            {
                return Name;
            }
            public void                 SetEditorID( Engine.Plugin.TargetHandle target, string value )
            {
                throw new NotImplementedException();
            }
            
            public Engine.Plugin.ConflictStatus ConflictStatus
            {
                get
                {
                    return !FileExists
                        ? Engine.Plugin.ConflictStatus.Invalid
                        : _UserToggleable
                        ? Engine.Plugin.ConflictStatus.NewForm
                        : Engine.Plugin.ConflictStatus.Uneditable;
                }
            }
            
            /*
            public bool                 IsModified
            {
                get
                {
                    return
                        ( Mod != null )&&
                        ( Mod.IsModified );
                }
            }
            */
            
            public string               ExtraInfo { get { return null; } }
            
            public event EventHandler   ObjectDataChanged;
            
            public virtual void         SupressObjectDataChangedEvents() {}
            public virtual void         ResumeObjectDataChangedEvents( bool sendevent ) {}
            
            public void                 SendObjectDataChangedEvent()
            {
                EventHandler handler = ObjectDataChanged;
                if( handler != null )
                    handler( this, null );
            }
            
            public bool                 ObjectChecked( bool checkedValue )
            {
                AlwaysSelect = checkedValue;
                return AlwaysSelect;
            }
            
            public bool                 InitialCheckedOrSelectedState()
            {
                return AlwaysSelect;
            }
            
            #endregion
            
        }
        
        // These should be protected but C# is retarded
        internal static List<XeLib.API.Setup.LoadOrderItem> LoadOrder = XeLib.API.Setup.GetLoadOrder();
        internal static uint FindInLoadOrder( string filename )
        {
            for( int i = 0; i < LoadOrder.Count; i++ )
                if( filename.InsensitiveInvariantMatch( LoadOrder[ i ].Filename ) ) return (uint)i;
            return 0xFF;
        }
        
        static List<File> _Files = null;
        public static List<File> Files
        {
            get
            {
                //DebugLog.OpenIndentLevel( new [] { "GodObject.Master", "Files" } );
                if( _Files == null ) GetAlwaysSelectMasters();
                //DebugLog.CloseIndentLevel();
                return _Files;
            }
        }
        
        static void GetAlwaysSelectMasters()
        {
            //DebugLog.OpenIndentLevel( new [] { "GodObject.Master", "GetAlwaysSelectMasters()" } );
            var files = new List<File>();
            foreach( var loi in LoadOrder )
            {
                if( !loi.Filename.EndsWith( ".esm", StringComparison.InvariantCultureIgnoreCase ) ) continue;
                bool userDeleteable =
                    !Filename.Fallout4.InsensitiveInvariantMatch( loi.Filename ) &&
                    !Filename.AnnexTheCommonwealth.InsensitiveInvariantMatch( loi.Filename ) &&
                    !Filename.SimSettlements.InsensitiveInvariantMatch( loi.Filename );;
                bool userToggleable = !Filename.Fallout4.InsensitiveInvariantMatch( loi.Filename );
                bool alwaysSelect = Filename.Fallout4.InsensitiveInvariantMatch( loi.Filename ) || Filename.AnnexTheCommonwealth.InsensitiveInvariantMatch( loi.Filename );
                files.Add( new File( loi.Filename, userDeleteable, userToggleable, alwaysSelect ) );
            }
            var masters = XmlConfig.GetNodes( XmlNode + '/' + XmlMaster );
            if( masters != null )
            {
                foreach( XmlNode node in masters )
                {
                    var name = XmlConfig.ReadNodeValue( node, XmlFilename, null );
                    if( name != null )
                    {
                        var alwaysSelect = XmlConfig.ReadNodeValue( node, XmlAlwaysSelect, "false" ).InsensitiveInvariantMatch( "true" );
                        var index = FindEx( files, name );
                        //DebugLog.Write( name + " " + alwaysLoad.ToString() );
                        if( index < 0 )
                            files.Add( new File( name, true, true, alwaysSelect ) );
                        else
                            files[ index ].AlwaysSelect = alwaysSelect;
                    }
                }
            }
            _Files = files;
            //DebugLog.CloseIndentLevel();
        }
        
        public static int Find( string name )
        {
            //DebugLog.WriteLine( new [] { "GodObject.Master", "Find()", name } );
            return FindEx( Files, name );
        }
        
        static int FindEx( List<File> files, string name )
        {
            //DebugLog.WriteLine( new [] { "GodObject.Master", "FindEx()", name } );
            if( files.NullOrEmpty() ) return -1;
            for( int i = 0; i < files.Count; i++ )
                if( name.InsensitiveInvariantMatch( files[ i ].Name ) )
                    return i;
            return -1;
        }
        
        public static File Fallout4
        {
            get
            {
                var i = Find( Filename.Fallout4 );
                return i < 0
                    ? null
                    : Files[ i ];
            }
            
        }
        
        public static File AnnexTheCommonwealth
        {
            get
            {
                var i = Find( Filename.AnnexTheCommonwealth );
                return i < 0
                    ? null
                    : Files[ i ];
            }
            
        }
        
        public static File SimSettlements
        {
            get
            {
                var i = Find( Filename.SimSettlements );
                return i < 0
                    ? null
                    : Files[ i ];
            }
            
        }
        
        public static bool Add( string name, bool alwaysSelect )
        {
            //DebugLog.WriteLine( new [] { "GodObject.Master", "Add()", name } );
            var index = Find( name );
            if( index >= 0 )
            {
                Files[ index ].AlwaysSelect = alwaysSelect;
                return true;
            }
            var master = new File( name, true, true, alwaysSelect );
            if( master == null )
                return false;
            Files.Add( master );
            return true;
        }
        
    }
    
    #endregion
    
}
