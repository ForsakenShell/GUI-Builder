using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUIBuilder.Windows
{
    public interface IEnableControlForm
    {
        void SetEnableState( bool enabled );
        void Close();
    }
}
