/*
 * ErrorTypes.cs
 *
 * Error states for batch imports.
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
    
    public enum ErrorTypes
    {
        Undefined,
        Parse,
        Resolve,
        Import
    }
    
}
