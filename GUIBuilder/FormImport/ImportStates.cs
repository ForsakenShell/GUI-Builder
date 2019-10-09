/*
 * ImportStates.cs
 *
 * Current state of imports.
 *
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Linq;

using Maths;
using Fallout4;
using AnnexTheCommonwealth;

namespace GUIBuilder.FormImport
{
    
    public enum ImportStates
    {
        Unparsed,
        Parsed,
        Resolved,
        Imported,
        Error
    }
    
}
