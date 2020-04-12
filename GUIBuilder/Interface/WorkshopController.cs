/*
 * WorkshopController.cs
 *
 * Interface for code looking at Fallout4.WorkshopScript and AnnexTheCommonwealth.SubDivision
 *
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Engine.Plugin.Forms;

namespace GUIBuilder.Interface
{
    
    public interface WorkshopController
    {
            
            string                          QualifiedName   { get; }
            List<ObjectReference>           BorderMarkers   { get; }
            ObjectReference                 SandboxVolume   { get; }
            List<ObjectReference>           BuildVolumes    { get; }

    }

}
