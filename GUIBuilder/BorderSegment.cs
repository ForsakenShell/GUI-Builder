/*
 * BorderSegment.cs
 *
 * A complete border between two sub-divisions.
 *
 */
using System;
using System.Collections.Generic;

using AnnexTheCommonwealth;

namespace GUIBuilder
{
    /// <summary>
    /// Description of BorderSegment.
    /// </summary>
    public class BorderSegment
    {
        
        BorderEnabler _enabler = null;
        List<EdgeFlag> _flags = null;
        List<BorderNode> _nodes = null;
        
        public BorderSegment( BorderEnabler enabler, List<EdgeFlag> flags )
        {
            _enabler = enabler;
            _flags = flags;
            _nodes = null;
        }
        
        public List<EdgeFlag> Flags { get{ return _flags; } }
        public List<BorderNode> Nodes { get{ return _nodes; } }
        
        public void GenerateBorderNodes( float approximateNodeLength, double angleAllowance, double slopeAllowance )
        {
            if( ( _enabler == null )||( _enabler.Reference == null )||( _enabler.Reference.Worldspace == null )||( _flags.NullOrEmpty() ) )
                return;
            _nodes = BorderNode.GenerateBorderNodes( _enabler.Reference.Worldspace, _flags, approximateNodeLength, angleAllowance, slopeAllowance, GodObject.CoreForms.ESM_ATC_STAT_SubDivisionEdgeFlag_ForcedZ );
        }
        
    }
    
}
