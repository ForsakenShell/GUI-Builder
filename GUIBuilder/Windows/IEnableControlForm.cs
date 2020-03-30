using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUIBuilder.Windows
{

    public delegate void SetEnableStateHandler( bool enabled );

    public interface IEnableControlForm
    {

        event SetEnableStateHandler OnSetEnableState;

        void                        SetEnableState( bool enabled );

        void                        Close();
        bool                        InvokeRequired { get; }
        object                      Invoke( Delegate method, params object[] args );
    }
}
