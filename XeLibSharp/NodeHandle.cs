using System;
using XeLib.API;
using XeLib.Internal;

namespace XeLib
{
    [HandleMapping()]
    public class NodeHandle : ElementHandle
    {
        public NodeHandle( uint uHandle ) : base( uHandle ) {}
        
        protected override void ReleaseXHandle( uint uHandle )
        {
            if( uHandle != BaseXHandleValue )
                Functions.ReleaseNodes( uHandle );
        }
        
    }
}
