using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUIBuilder.Windows
{

    public delegate bool SetEnableStateHandler( object sender, bool enable );

    public interface IEnableControlForm
    {

        event SetEnableStateHandler OnSetEnableState;
        
        bool                        SetEnableState( object sender, bool enable );

        void                        Close();
        bool                        InvokeRequired { get; }
        object                      Invoke( Delegate method, params object[] args );
    }
}
