/*
 * FormIDs.cs
 *
 * Formlist FormIDs field.
 *
 */

using System.Collections.Generic;

using XeLib;
using XeLib.API;

using Engine.Plugin.Extensions;


namespace Engine.Plugin.Forms.Fields.Formlist
{
    
    public class FormIDs : RawField
    {
        
        ElementHandle                  _LastHandle = null;
        List<uint>                     _FormIDs = null;
        
        public FormIDs( Form form ) : base( form, "FormIDs" ) {}
        
        #region Internal management
        
        void                            ClearCachedResults()
        {
            _LastHandle = null;
            _FormIDs    = null;
        }
        
        public List<uint>               GetFormIDs( TargetHandle target )
        {
            var h = Form.HandleFromTarget( target );
            if( _LastHandle == h ) return _FormIDs.Clone();
            
            ClearCachedResults();
            
            _LastHandle = h;
            
            if( !HasValue( target ) )
                return null;
            
            var handles = h.GetElements<ElementHandle>( XPath, false, false );
            if( ( handles == null )||( handles.Length == 0 ) )
            {
                var s = Messages.GetMessages();
                var m = "ElementHandle.GetElements() == null";
                if( !string.IsNullOrEmpty( s ) )
                {
                    m += "\n";
                    m += s;
                }
                DebugLog.WriteWarning( m );
                return null;
            }
            
            var hLen = handles.Length;
            if( hLen > 0 )
            {
                var results = new uint[ hLen ];
                for( int i = 0; i < hLen; i++ )
                {
                    var handle = handles[ i ];
                    results[ i ] = handle.GetUIntValue();
                    handle.Dispose();
                }
                _FormIDs = new List<uint>( results );
            }

            return _FormIDs.Clone();
        }
        
        #endregion

        public override string          ToString( TargetHandle target, string format = null )
        {
            return null;
        }
        
    }
    
}
