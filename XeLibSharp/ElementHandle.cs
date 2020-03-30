using System;
using System.Collections.Generic;
using System.Linq;
using XeLib.API;
using XeLib.Internal;

namespace XeLib
{
    
    public static class HandleExtensions
    {
        
        public static bool IsValid( this ElementHandle handle )
        {
            if( ( handle == null )||( handle.Disposed )||( handle.XHandle == ElementHandle.BaseXHandleValue ) )
                return false;
            return Helpers.HandleMatchesElementEx( handle.XHandle, handle.GetType() );
            //return true;
        }
        
        public static int ExistingHandleIndex<THandle>( List<THandle> handles, THandle findHandle ) where THandle : ElementHandle
        {
            return !findHandle.IsValid() || handles.NullOrEmpty() ? -1 : handles.FindIndex( h => ( h == findHandle )||( findHandle.DuplicateOf( h ) ) );
        }
        
        public static int InsertHandleIndex<THandle>( List<THandle> handles, THandle findHandle ) where THandle : ElementHandle
        {
            if( !findHandle.IsValid() ) return -1;
            if( handles.NullOrEmpty() ) return -1;
            int i = handles.FindIndex( h => h.LoadOrder > findHandle.LoadOrder );
            return i < 0 ? handles.Count : i;
        }
        
    }
    
    [AttributeUsage(AttributeTargets.Class)]
    class HandleMapping : Attribute
    {
        // If AllowedElementTypes is null then all are allowed
        public readonly Elements.ElementTypes[] AllowedElementTypes = null;
        
        public HandleMapping()
        {
            AllowedElementTypes = null;
        }
        
        public HandleMapping( Elements.ElementTypes[] allowedElementTypes )
        {
            AllowedElementTypes = allowedElementTypes;
        }
        
    }
    
    [HandleMapping()]
    public class ElementHandle : IDisposable, IEquatable<ElementHandle>
    {
        
        public const uint BaseXHandleValue = 0;
        public static ElementHandle BaseXHandle = Helpers.CreateHandle<ElementHandle>( BaseXHandleValue );
        
        #region XHandle
        
        ElementHandle CloneOf = null;
        
        uint _xHandle;
        public uint XHandle
        {
            get
            {
                if( _Disposed )
                    throw new ObjectDisposedException( this.TypeFullName() );
                return CloneOf != null ? CloneOf.XHandle : _xHandle;
            }
            protected set
            {
                if( _Disposed )
                    throw new ObjectDisposedException( this.TypeFullName() );
                if( CloneOf == null )
                {
                    if( ( _xHandle != BaseXHandleValue )&&( _xHandle != value ) )
                        ReleaseXHandle( _xHandle );
                }
                _xHandle = value;
                CloneOf = null;
            }
        }
        
        public ElementHandle( uint uHandle )
        {
            _xHandle = uHandle;
        }
        
        public ElementHandle( ElementHandle cloneOf )
        {
            CloneOf = cloneOf;
        }
        
        #region IDisposable
        
        bool _Disposed = false;
        public bool Disposed { get { return _Disposed; } }
        
        public void Dispose()
        {
            if( !this.IsValid() )
            {
                DebugLog.WriteError( "Tried to dispose of an invalid handle! :: 0x" + _xHandle.ToString( "X8" ) );
                return;
            }
            //DebugLog.WriteLine( string.Format( "{0} :: Dispose() :: {1} :: _Disposed = {2}", this.FullTypeName(), this.ToString(), _Disposed ) );
            Dispose( true );
            GC.SuppressFinalize( this );
        }
        
        void Dispose( bool disposing )
        {
            if( _Disposed ) return;
            _Disposed = true;
            if( ( disposing )&&( CloneOf == null ) )
                ReleaseXHandle( _xHandle );
            _xHandle = BaseXHandleValue;
            CloneOf = null;
        }
        
        protected virtual void ReleaseXHandle( uint uHandle )
        {
            if( uHandle != BaseXHandleValue )
                Functions.Release( uHandle );
        }
        
        public static void ReleaseHandles<THandle>( THandle[] handles ) where THandle : ElementHandle
        {
            if( handles == null )
                return;
            
            var hCount = handles.Length;
            if( hCount < 1 )
                return;
            
            for( int i = 0; i < hCount; i++ )
                if( handles[ i ].IsValid() ) handles[ i ].Dispose();
        }
        
        public static void ReleaseHandles<THandle>( List<THandle> handles ) where THandle : ElementHandle
        {
            if( handles == null )
                return;
            
            var hCount = handles.Count;
            if( hCount < 1 )
                return;
            
            for( int i = 0; i < hCount; i++ )
                if( handles[ i ].IsValid() ) handles[ i ].Dispose();
        }
        
        #endregion
        
        #region IEquatable
        
        public override int GetHashCode()
        {
            return XHandle.GetHashCode();
        }
        
        public override bool Equals( Object obj )
        {
            if( ReferenceEquals( null, obj ) ) return false;
            if( ReferenceEquals( this, obj ) ) return true;
            return
                ( obj.GetType() == GetType() )&&
                ( Equals( (ElementHandle) obj ) );
        }
        
        public bool Equals( ElementHandle other )
        {
            return
                ( ReferenceEquals( this, other ) )||
                ( this.XHandle == other.XHandle )||
                ( this.DuplicateOf( other ) );
        }
        
        #endregion
        
        #endregion
        
        public override string ToString()
        {
            if( this == null )
                return "[null]";
            if( Disposed )
                return "[disposed]";
            var strXHandle = this.XHandle.ToString( "X8" );
            var strType = this.TypeFullName();
            var strFile = this.Filename;
            var strLO = string.Format( "Load Order = 0x{0}", LoadOrder.ToString( "X2" ) );
            var strExtra = ToStringExtra();
            var strCloned = ( CloneOf == null ? null : " :: Cloned Handle" );
            var strDups = (string)null;
            var hDups = Meta.GetDuplicateXHandlesEx( this.XHandle );
            if( ( hDups != null ) && ( hDups.Length > 0 ) )
            {
                foreach( var hDup in hDups )
                {
                    if( !string.IsNullOrEmpty( strDups ) ) strDups += ", ";
                    strDups += "0x" + hDup.ToString( "X8" );
                }
                strDups = " :: Duplicate XHandles = [" + strDups + "]";
            }
            var str = string.Format(
                "[XHandle = 0x{0} :: {1} :: \"{2}\" :: {3}{4}{5}{6}]",
                strXHandle,
                strType,
                strFile,
                strLO,
                ( strExtra == null ? null : string.Format( " :: {0}", strExtra ) ),
                strCloned,
                strDups
            );
            return str;
        }
        
        public virtual string ToStringExtra()
        {
            return null;
        }
        
        #region Handle Flags and States
        
        public bool IsMaster
        {
            get
            {
                bool result;
                return Functions.IsMaster( _xHandle, out result ) && result;
            }
        }
        
        public bool IsOverride
        {
            get
            {
                bool result;
                return Functions.IsOverride( _xHandle, out result ) && result;
            }
        }
        
        public bool IsWinningOverride
        {
            get
            {
                bool result;
                return Functions.IsWinningOverride( _xHandle, out result ) && result;
            }
        }
        
        public bool IsInjected
        {
            get
            {
                bool result;
                return Functions.IsInjected( _xHandle, out result ) && result;
            }
        }
        
        public bool IsModified
        {
            get
            {
                bool result;
                return Functions.GetIsModified( _xHandle, out result ) && result;
            }
        }
        
        #endregion
        
        #region Handle Meta - ElementType, Signature, etc
        
        public Elements.ElementTypes ElementType
        {
            get
            {
                return Elements.ElementTypeEx( this.XHandle );
            }
        }
        
        public Elements.DefTypes DefType
        {
            get
            {
                return Elements.DefTypeEx( this.XHandle );
            }
        }
        
        public string[] GetEnumOption( string path )
        {
            return ElementValues.GetEnumOptionEx( this.XHandle, path );
        }
        
        public virtual string Signature
        {
            get
            {
                return ElementValues.SignatureEx( this.XHandle );
            }
        }
        
        public string Name
        {
            get
            {
                return ElementValues.NameEx( this.XHandle );
            }
        }
        
        public string LongName
        {
            get
            {
                return ElementValues.LongNameEx( this.XHandle );
            }
        }
        
        public string DisplayName
        {
            get
            {
                return ElementValues.DisplayNameEx( this.XHandle );
            }
        }
        
        public string PlacementName
        {
            get
            {
                return ElementValues.PlacementNameEx( this.XHandle );
            }
        }
        
        #endregion
        
        #region Paths
        
        public string Path
        {
            get
            {
                return ElementValues.PathEx( this.XHandle );
            }
        }
        
        public string LongPath
        {
            get
            {
                return ElementValues.LongPathEx( this.XHandle );
            }
        }
        
        public string LocalPath
        {
            get
            {
                return ElementValues.LocalPathEx( this.XHandle );
            }
        }
        
        #endregion
        
        #region Files
        
        public string Filename
        {
            get
            {
                var path = Path;
                //DebugLog.Write( string.Format( "{0} :: Filename :: 0x{1} :: Path = \"{2}\"", this.FullTypeName(), _xHandle.ToString( "X8" ), path ) );
                var si = path.IndexOf( '\\' );
                var fName = si > 0
                    ? path.Substring( 0, si )
                    : path;
                return fName;
            }
        }
        
        public FileHandle FileHandle
        {
            get
            {
                var fHandle = Files.FileByName( Filename );
                //DebugLog.Write( string.Format( this.FullTypeName() + " :: FileHandle :: 0x{0} :: \"{1}\"", fHandle.ToString(), fName ) );
                return fHandle;
            }
        }
        
        public virtual uint LoadOrder
        {
            get
            {
                var lo = Setup.GetLoadOrder( false );
                if( lo.NullOrEmpty() ) return 0xFFFFFFFF;
                var loi = lo.FindIndex( file => file.Filename.InsensitiveInvariantMatch( Filename ) );
                return loi < 0 ? 0xFFFFFFFF : ( uint )loi;
            }
        }
        
        public bool Requires( string filename )
        {
            var masters = FileHandle.GetMastersOf( Filename );
            return !masters.NullOrEmpty() && masters.Any( f => f.InsensitiveInvariantMatch( filename ) );
        }
        
        #endregion
        
        #region Records
        
        public FormHandle[] GetContainerRecordTree()
        {
            // Build the tree of forms
            var refTree = new System.Collections.Generic.List<FormHandle>();
            var hContainer = this;
            while( hContainer.IsValid() )
            {
                var rhContainer = hContainer as FormHandle;
                if( rhContainer != null )
                    refTree.Insert( 0, rhContainer );
                hContainer = hContainer.GetContainerRecord();
            }
            //DebugLog.WriteList<FormHandle>( this.FullTypeName() + " :: GetContainerRecordTree() :: result", refTree );
            return refTree.ToArray();
        }
        
        public FormHandle GetContainerRecord()
        {
            return Records.GetContainerRecordEx( this.XHandle );
        }
        
        public FormHandle GetRootContainerRecord()
        {
            return Records.GetRootContainerRecordEx( this.XHandle );
        }
        
        public FormHandle GetRecord( uint uFormID, bool searchMasters = false )
        {
            return uFormID != 0
                ? Records.GetRecordEx( this.XHandle, uFormID, searchMasters )
                : null;
        }
        
        public FormHandle[] GetRecords( string search, bool includeOverrides = false )
        {
            return Records.GetRecordsEx( this.XHandle, search, includeOverrides );
        }
        
        public FormHandle GetMasterRecord( uint uFormID, bool searchMasters = false )
        {
            return uFormID != 0
                ? Records.FindMasterRecordEx( this.XHandle, uFormID, searchMasters )
                : null;
        }
        
        #endregion
        
        public THandle FindParentBySignature<THandle>( string signature ) where THandle : ElementHandle
        {
            uint uCurrent = XHandle;
            while( true )
            {
                var cSig = ElementValues.SignatureEx( uCurrent );
                if( cSig == signature )
                    return Helpers.CreateHandle<THandle>( uCurrent );
                
                uint nContainer;
                var gcRes = XeLib.Internal.Functions.GetContainer( uCurrent, out nContainer );
                if( uCurrent != XHandle )
                    Meta.ReleaseEx( uCurrent );
                if( !gcRes ) break;
                uCurrent = nContainer;
            }
            return null;
        }
        
        #region Handle Aliasing
        
        public THandle Clone<THandle>() where THandle : ElementHandle
        {
            return Helpers.CreateHandle<THandle>( this );
        }
        
        public bool DuplicateOf( ElementHandle other )
        {
            if(
                ( !this.IsValid() )||
                ( !other.IsValid() )
            )   return false;
            var result = other == this;
            if( !result )
            {
                var hDups = Meta.GetDuplicateXHandlesEx( this.XHandle );
                if( ( hDups != null )&&( hDups.Length > 0 ) )
                {
                    foreach( var hDup in hDups )
                    {
                        //DebugLog.Write( string.Format( "clone handle :: 0x{0}", h.ToString() ) );
                        if( hDup != ElementHandle.BaseXHandleValue )
                        {
                            if( hDup == other.XHandle )
                            {
                                result = true;
                                break;
                            }
                        }
                    }
                }
            }
            //DebugLog.Write( string.Format( "XeLib.Handle.CloneOf()\n\tthis = 0x{0}\n\tother = 0x{1}\n\tresult = {2}", this.ToString(), other.ToString(), result ) );
            return result;
        }
        
        #endregion
        
        #region Elements
        
        public bool HasElement( string path )
        {
            return Elements.HasElementEx( this.XHandle, path );
        }
        
        public bool RemoveElement( string path )
        {
            return Elements.RemoveElementEx( this.XHandle, path );
        }
        
        public int ElementCount()
        {
            return Elements.ElementCountEx( this.XHandle );
        }
        
        public ElementHandle GetChildGroup()
        {
            return Elements.GetElementEx<ElementHandle>( this.XHandle, "Child Group" );
        }
        
        public THandle[] GetElements<THandle>( string path = "", bool sort = false, bool filter = false ) where THandle : ElementHandle
        {
            return Elements.GetElementsEx<THandle>( this.XHandle, path, sort, filter );
        }
        
        public THandle GetElement<THandle>( string path ) where THandle : ElementHandle
        {
            return Elements.GetElementEx<THandle>( this.XHandle, path );
        }
        
        public THandle AddElement<THandle>( string path ) where THandle : ElementHandle
        {
            return Elements.AddElementEx<THandle>( this.XHandle, path );
        }
        
        public THandle CopyElement<THandle>( ElementHandle dst, bool asNew = false ) where THandle : ElementHandle
        {
            return Elements.CopyElementEx<THandle>( this.XHandle, dst.XHandle, asNew );
        }
        
        public THandle AddArrayItem<THandle>( string path, string subpath, string value ) where THandle : ElementHandle
        {
            return Elements.AddArrayItemEx<THandle>( this.XHandle, path, subpath, value );
        }

        #region Element Values

        /// <summary>
        /// DO NOT USE!  DOES NOT WORK AS THE UNDERLYING FUNCTION DOES NOT WORK FOR ANYTHING BEYOND RESOURCE FILES!
        /// </summary>
        /// <returns></returns>
        public virtual byte[] GetRawBytes()
        {
            return ElementValues.GetRawBytesEx( this.XHandle, "" );
        }


        #region String Values

        public virtual string GetValue()
        {
            return ElementValues.GetValueEx( this.XHandle, "" );
        }
        
        public virtual string GetValueEx( string path )
        {
            return ElementValues.GetValueEx( this.XHandle, path );
        }
        
        public virtual bool SetValue( string value )
        {
            return ElementValues.SetValueEx( this.XHandle, "", value );
        }
        
        public virtual bool SetValueEx( string path, string value )
        {
            return ElementValues.SetValueEx( this.XHandle, path, value );
        }
        
        #endregion
        
        #region Bool Values
        
        public virtual bool GetBoolValue()
        {
            return ElementValues.GetBoolValueEx( this.XHandle, "" );
        }
        
        public virtual bool GetBoolValueEx( string path )
        {
            return ElementValues.GetBoolValueEx( this.XHandle, path );
        }
        
        public virtual bool SetBoolValue( bool value )
        {
            return ElementValues.SetBoolValueEx( this.XHandle, "", value );
        }
        
        public virtual bool SetBoolValueEx( string path, bool value )
        {
            return ElementValues.SetBoolValueEx( this.XHandle, path, value );
        }

        #endregion

        #region Byte Values

        public virtual sbyte GetSByteValue()
        {
            return ElementValues.GetSByteValueEx( this.XHandle, "" );
        }

        public virtual sbyte GetSByteValueEx( string path )
        {
            return ElementValues.GetSByteValueEx( this.XHandle, path );
        }

        public virtual bool SetSByteValue( sbyte value )
        {
            return ElementValues.SetSByteValueEx( this.XHandle, "", value );
        }

        public virtual bool SetSByteValueEx( string path, sbyte value )
        {
            return ElementValues.SetSByteValueEx( this.XHandle, path, value );
        }

        public virtual byte GetUByteValue()
        {
            return ElementValues.GetUByteValueEx( this.XHandle, "" );
        }

        public virtual byte GetUByteValueEx( string path )
        {
            return ElementValues.GetUByteValueEx( this.XHandle, path );
        }

        public virtual bool SetUByteValue( byte value )
        {
            return ElementValues.SetUByteValueEx( this.XHandle, "", value );
        }

        public virtual bool SetUByteValueEx( string path, byte value )
        {
            return ElementValues.SetUByteValueEx( this.XHandle, path, value );
        }

        #endregion

        #region Integer Values

        public virtual int GetIntValue()
        {
            return ElementValues.GetIntValueEx( this.XHandle, "" );
        }
        
        public virtual int GetIntValueEx( string path )
        {
            return ElementValues.GetIntValueEx( this.XHandle, path );
        }
        
        public virtual bool SetIntValue( int value )
        {
            return ElementValues.SetIntValueEx( this.XHandle, "", value );
        }
        
        public virtual bool SetIntValueEx( string path, int value )
        {
            return ElementValues.SetIntValueEx( this.XHandle, path, value );
        }
        
        public virtual uint GetUIntValue()
        {
            return ElementValues.GetUIntValueEx( this.XHandle, "" );
        }
        
        public virtual uint GetUIntValueEx( string path )
        {
            return ElementValues.GetUIntValueEx( this.XHandle, path );
        }
        
        public virtual bool SetUIntValue( uint value )
        {
            return ElementValues.SetUIntValueEx( this.XHandle, "", value );
        }
        
        public virtual bool SetUIntValueEx( string path, uint value)
        {
            return ElementValues.SetUIntValueEx( this.XHandle, path, value );
        }
        
        #endregion
        
        #region Real Number Values
        
        public virtual double GetDoubleValue()
        {
            return ElementValues.GetDoubleValueEx( this.XHandle, "" );
        }
        
        public virtual double GetDoubleValueEx( string path )
        {
            return ElementValues.GetDoubleValueEx( this.XHandle, path );
        }
        
        public virtual bool SetDoubleValue( double value )
        {
            return ElementValues.SetDoubleValueEx( this.XHandle, "", value );
        }
        
        public virtual bool SetDoubleValueEx( string path, double value )
        {
            return ElementValues.SetDoubleValueEx( this.XHandle, path, value );
        }
        
        public virtual float GetFloatValue()
        {
            return ElementValues.GetFloatValueEx( this.XHandle, "" );
        }
        
        public virtual float GetFloatValueEx( string path )
        {
            return ElementValues.GetFloatValueEx( this.XHandle, path );
        }
        
        public virtual bool SetFloatValue( float value )
        {
            return ElementValues.SetFloatValueEx( this.XHandle, "", value );
        }
        
        public virtual bool SetFloatValueEx( string path, float value )
        {
            return ElementValues.SetFloatValueEx( this.XHandle, path, value );
        }
        
        #endregion
        
        #region Flags
        
        public bool GetFlag( string path, string name )
        {
            return ElementValues.GetFlagEx( this.XHandle, path, name );
        }
        
        public bool SetFlag( string path, string name, bool value )
        {
            return ElementValues.SetFlagEx( this.XHandle, path, name, value );
        }
        
        public string[] GetEnabledFlags( string path )
        {
            return ElementValues.GetEnabledFlagsEx( this.XHandle, path );
        }
        
        public bool SetEnabledFlags( string path, string[] flags )
        {
            return ElementValues.SetEnabledFlagsEx( this.XHandle, path, flags );
        }
        
        public string[] GetAllFlags( string path )
        {
            return ElementValues.GetAllFlagsEx( this.XHandle, path );
        }
        
        #endregion
        
        #endregion
        
        #endregion
        
        #region Debug
        
        public void DebugDumpChildElements( bool recurseIntoChildren = false, int indentLevel = 1 )
        {
            var bar = GetElements<XeLib.ElementHandle>();
            if( bar.NullOrEmpty() )
                return;
            var indents = "";
            int i = indentLevel;
            while( i > 0 )
            {
                indents += "\t";
                i--;
            }
            Console.WriteLine( indents + string.Format( "Element \"{0}\" contains {1} child elements", Name,  bar.Length ) );
            for( int j = 0; j < bar.Length; j++ )
            {
                Console.WriteLine( indents + string.Format( "\t[ {0} ] :: {1} :: \"{2}\" = \"{3}\"", j, bar[ j ].ElementType.ToString(), bar[ j ].Name, bar[ j ].GetValue() ) );
                if( recurseIntoChildren )
                    bar[ j ].DebugDumpChildElements( true, indentLevel + 2 );
                bar[ j ].Dispose();
            }
        }
        
        #endregion
        
    }
}