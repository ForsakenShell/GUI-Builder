using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using XeLib.Internal;

namespace XeLib.API
{
    public static class Setup
    {
        
        public enum GameMode
        {
            GmFnv = 0,
            GmF03 = 1,
            GmTes4 = 2,
            GmTes5 = 3,
            GmSse = 4,
            GmFo4 = 5
        }
        
        public enum LoaderState : byte
        {
            IsInactive = 0,
            IsActive = 1,
            IsDone = 2,
            IsError = 3
        }
        
        public static readonly IList<GameInfo> Games = new ReadOnlyCollection<GameInfo>(
            new[]
            {
                new GameInfo( "Fallout NV"  , "FalloutNV"   , GameMode.GmFnv    , "FalloutNV.exe"   ),
                new GameInfo( "Fallout 3"   , "Fallout3"    , GameMode.GmF03    , "Fallout3.exe"    ),
                new GameInfo( "Oblivion"    , "Oblivion"    , GameMode.GmTes4   , "Oblivion.exe"    ),
                new GameInfo( "Skyrim"      , "Skyrim"      , GameMode.GmTes5   , "TESV.exe"        ),
                new GameInfo( "Skyrim SE"   , "Skyrim"      , GameMode.GmSse    , "SkyrimSE.exe"    ),
                new GameInfo( "Fallout 4"   , "Fallout4"    , GameMode.GmFo4    , "Fallout4.exe"    )
            });
        
        public struct LoadOrderItem
        {
            public string Filename;
            public List<string> Masters;
            
            public LoadOrderItem( string filename, List<string> masters )
            {
                Filename = filename;
                Masters = masters;
            }
        }
        
        public static bool SetGamePath( string gamePath )
        {
            return Functions.SetGamePath( gamePath );
        }
        
        public static bool SetLanguage( string language )
        {
            return Functions.SetLanguage( language );
        }
        
        public static bool SetGameMode( GameMode mode )
        {
            return Functions.SetGameMode( Convert.ToInt32( mode ) );
        }
        
        public static List<LoadOrderItem> GetLoadOrder( bool allowGhosts = false )
        {
            int len;
            if( ( !Functions.GetLoadOrder( out len ) )||( len < 1 ) ) return null;
            var unfiltered = Helpers.GetResultString( len ).Split( new [] { '\r', '\n' } );
            if( unfiltered.NullOrEmpty() ) return null;
            var lo = new List<LoadOrderItem>();
            for( int i = 0; i < unfiltered.Length; i++ )
            {
                if( string.IsNullOrEmpty( unfiltered[ i ] ) ) continue;
                if( ( !allowGhosts )&&( unfiltered[ i ].EndsWith( ".ghost", StringComparison.InvariantCultureIgnoreCase ) ) ) continue;
                lo.Add( new LoadOrderItem( unfiltered[ i ], FileHandle.GetMastersOf( unfiltered[ i ] ) ) );
            }
            return lo;
        }
        
        public static List<string> GetActivePlugins()
        {
            int len;
            if( ( !Functions.GetActivePlugins( out len ) )||( len < 1 ) ) return null;
            var pap = Helpers.GetResultString( len );
            return pap.Split( new [] { '\r', '\n' } ).Where( s => !string.IsNullOrEmpty( s ) ).ToList();
        }
        
        public static bool LoadPlugins( List<string> loadOrder, bool smartLoad = true )
        {
            if( loadOrder.NullOrEmpty() ) return false;
            var plo = string.Empty;
            for( int i = 0; i < loadOrder.Count; i++ )
            {
                if( i > 0 ) plo += "\n";
                plo += loadOrder[ i ];
            }
            return Functions.LoadPlugins( plo, smartLoad );
        }
        
        public static bool LoadPlugin( string filename )
        {
            return Functions.LoadPlugin( filename );
        }
        
        public static THandle LoadPluginHeaderEx<THandle>( string filename ) where THandle : ElementHandle
        {
            uint resHandle;
            return ( Functions.LoadPluginHeader( filename, out resHandle ) )&&( resHandle != ElementHandle.BaseXHandleValue )
                ? Helpers.CreateHandle<THandle>( resHandle )
                : null;
        }
        
        public static bool BuildReferencesEx( uint uHandle, bool sync = true )
        {
            return Functions.BuildReferences( uHandle, sync );
        }
        
        public static bool UnloadPluginEx( uint uHandle )
        {
            return Functions.UnloadPlugin( uHandle );
        }
        
        public static LoaderState GetLoaderStatus()
        {
            byte state;
            Functions.GetLoaderStatus( out state );
            return (LoaderState)state;
        }
        
        public static string GetGamePath( GameMode mode )
        {
            int len;
            return ( Functions.GetGamePath( Convert.ToInt32(mode), out len ) )&&( len > 0 )
                ? Helpers.GetResultString( len )
                : null;
        }
        
        public static FileHandle[] GetLoadedFiles( bool excludeHardcoded = true )
        {
            DebugLog.OpenIndentLevel( new [] { "XeLib.API.Setup", "GetLoadedFiles()", "excludeHardcoded = " + excludeHardcoded } );
            var fileHandles = Elements.GetElementsEx<FileHandle>( ElementHandle.BaseXHandleValue );
            
            if( excludeHardcoded )
            {
                var filteredHandles = Array.FindAll( fileHandles, f => !f.Filename.EndsWith( ".Hardcoded.dat", StringComparison.InvariantCultureIgnoreCase ) );
                foreach( var handle in fileHandles )
                    if( !filteredHandles.Contains( handle ) )
                        handle.Dispose();
                fileHandles = filteredHandles;
            }
            
            DebugLog.CloseIndentLevel();
            return fileHandles;
        }
        
        public static string[] GetLoadedFileNames( bool excludeHardcoded = true )
        {
            var fileHandles = GetLoadedFiles( excludeHardcoded );
            
            var fileNames = new string[ fileHandles.Length ];
            for( int i = 0; i < fileHandles.Length; i++ )
            {
                fileNames[ i ] = fileHandles[ i ].Filename;
                fileHandles[ i ].Dispose();
            }
            
            return fileNames;
        }
        
        public struct GameInfo
        {
            public readonly string Name;
            public readonly string ShortName;
            public readonly GameMode Mode;
            public readonly string ExeName;
            
            public GameInfo(string name, string shortName, GameMode mode, string exeName)
            {
                Name = name;
                ShortName = shortName;
                Mode = mode;
                ExeName = exeName;
            }
        }

    }
}