/*
 * WorkshopScript.cs
 *
 * This groups all related data to workshops.
 *
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

using GUIBuilder;

using Maths;

using Engine;
using Engine.Plugin;

using XeLib;
using XeLib.API;
using XeLibHelper;

namespace Fallout4
{
    
    /// <summary>
    /// Description of Workshop.
    /// </summary>
    [Engine.Plugin.Attributes.ScriptAssociation( "WorkshopScript" )]
    public class WorkshopScript : Engine.Plugin.PapyrusScript
    {

        // Forms for workshop borders
        //Engine.Plugin.Forms.Keyword _BorderGeneratorKeyword = null;
        //Engine.Plugin.Forms.Keyword _GUIBuilder.CustomForms.WorkshopBorderLinkKeyword = null;

        //Engine.Plugin.Forms.LocationRef _BorderWithBottomRef = null;


        List<Engine.Plugin.Forms.ObjectReference> _BorderMarkers = null;
        public List<GUIBuilder.BorderNode> BorderNodes = null;
        
        Engine.Plugin.Forms.ObjectReference _Border = null;
        
        // Build volumes for workshops
        List<Engine.Plugin.Forms.ObjectReference> _BuildVolumes = null;
        
        // Pulled from handle for quick access
        
        public string LocationName = null;
        
        #region Constructor
        
        public WorkshopScript( Engine.Plugin.Forms.ObjectReference reference ) : base( reference )
        {
            // TODO : DO THIS PROPERLY!
            LocationName = string.Format( "{0} - No location", reference.IDString );
        }

        #endregion

        #region Public Properties
        
        public override ConflictStatus ConflictStatus
        {
            get
            {
                return
                    ( GUIBuilder.CustomForms.WorkshopBorderGeneratorKeyword != null ) && ( GetFirstBorderMarker( false ) != null )
                    ? ConflictStatus.RequiresOverride
                    : base.ConflictStatus;
            }
        }

        public string NameFromEditorID
        {
            get
            {
                // TODO:  FIX ME FOR PROPER NAMESPACE PREFIXES AND STRING SCANNING, THIS CODE SUX!
                var foo = Reference.GetEditorID( Engine.Plugin.TargetHandle.LastValid );
                foo = foo.StripFrom( "WorkshopREF", StringComparison.InvariantCultureIgnoreCase );
                foo = foo.StripFrom( "Workshop", StringComparison.InvariantCultureIgnoreCase );
                foo = foo.StripFrom( "Reference", StringComparison.InvariantCultureIgnoreCase );
                return foo;
            }
        }
        
        public Engine.Plugin.Forms.ObjectReference BorderReference
        {
            get
            {
                if( _Border == null )
                {
                    var forms = Form.References;
                    if( forms.NullOrEmpty() )
                        return null;
                    
                    var wlbae = GodObject.CoreForms.Fallout4.Keyword.WorkshopLinkedBuildAreaEdge;
                    if( wlbae == null )
                        return null;

                    var wlbaeFID = wlbae.GetFormID( Engine.Plugin.TargetHandle.Master );

                    foreach( var form in forms )
                    {
                        var refr = form as Engine.Plugin.Forms.ObjectReference;
                        if( refr == null )
                            continue;
                        var refrRef = refr.LinkedRefs.GetLinkedRef( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired, wlbaeFID );
                        if( ( refrRef != null )&&( refrRef == Reference ) )
                        {
                            _Border = refr;
                            break;
                        }
                    }
                }
                /*
                DebugLog.Write( string.Format(
                    "Fallout4.WorkshopScript.BorderReference :: 0x{0} \"{1}\"",
                    ( _Border == null ? 0 : _Border.FormID ).ToString( "X8" ),
                    _Border == null ? "" : _Border.EditorID ) );
                */
                return _Border;
            }
            set
            {
                var wlbae = GodObject.CoreForms.Fallout4.Keyword.WorkshopLinkedBuildAreaEdge;
                if( wlbae == null )
                    return;
                
                // Unlink the current border
                var current = BorderReference;
                if( ( current != null )&&( current != value ) )
                    current.LinkedRefs.Remove( Engine.Plugin.TargetHandle.Working, this.GetFormID( Engine.Plugin.TargetHandle.Master ), Engine.Plugin.Constant.FormID_None );
                
                // Link the new border
                if( ( value != null )&&( value != _Border ) )
                {
                    var index = value.LinkedRefs.FindKeywordIndex( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired, wlbae.GetFormID( Engine.Plugin.TargetHandle.Master ) );
                    if( index >= 0 )
                    {   // Redirect the existing link to the new ref
                        value.LinkedRefs.SetReference( Engine.Plugin.TargetHandle.Working, index, Reference );
                    }
                }
                
                _Border = value;
            }
        }
        
        public Engine.Plugin.Forms.Static BorderStatic
        {
            get
            {
                var border = BorderReference;
                if( border == null ) return null;
                //var cStatics = GodObject.Plugin.Data.Root.GetContainer<Engine.Plugin.Forms.Static>( true, false );
                //if( cStatics == null ) return null;
                //var stat = cStatics.Find<Engine.Plugin.Forms.Static>( border.Name );
                var stat = GodObject.Plugin.Data.Root.Find<Engine.Plugin.Forms.Static>( border.GetNameFormID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
                //var stat = border == null
                //    ? null
                //    : GodObject.Plugin.Data.Statics.Find( border.Name );
                /*
                DebugLog.Write( string.Format(
                    "Fallout4.WorkshopScript.BorderStatic :: border.Name = 0x{2} :: 0x{0} \"{1}\"",
                    ( stat == null ? 0 : stat.FormID ).ToString( "X8" ),
                    ( stat == null ? "" : stat.EditorID ),
                    ( border == null ? 0 : border.Name ).ToString( "X8" ) ) );
                */
                return stat;
            }
        }
        
        public Engine.Plugin.Forms.ObjectReference SandboxVolume
        {
            get
            {
                return Reference.LinkedRefs.GetLinkedRef( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired, GodObject.CoreForms.Fallout4.Keyword.WorkshopLinkSandbox.GetFormID( Engine.Plugin.TargetHandle.Master ) );
            }
        }
        
        public List<Engine.Plugin.Forms.ObjectReference> BuildVolumes
        {
            get
            {
                if( _BuildVolumes == null )
                {
                    var forms = Form.References;
                    if( forms.NullOrEmpty() )
                        return null;
                    
                    var wlp = GodObject.CoreForms.Fallout4.Keyword.WorkshopLinkedPrimitive;
                    if( wlp == null )
                        return null;
                    
                    var list = new List<Engine.Plugin.Forms.ObjectReference>();
                    
                    foreach( var form in forms )
                    {
                        var refr = form as Engine.Plugin.Forms.ObjectReference;
                        if( refr == null )
                            continue;
                        var refrRef = refr.LinkedRefs.GetLinkedRef( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired, wlp.GetFormID( Engine.Plugin.TargetHandle.Master ) );
                        if( ( refrRef != null )&&( refrRef == Reference ) )
                        {
                            list.Add( refr );
                        }
                    }
                    if( !list.NullOrEmpty() )
                        _BuildVolumes = list;
                }
                /*
                DebugLog.Write( string.Format(
                    "Fallout4.WorkshopScript.BuildVolumes :: count = {0}",
                    ( _BuildVolumes.NullOrEmpty() ? 0 : _BuildVolumes.Count ) ) );
                */
                return _BuildVolumes;
            }
        }
        
        #endregion
        

        Engine.Plugin.Forms.ObjectReference GetFirstBorderMarker( bool warnOnMissingLink )
        {
            Engine.Plugin.Forms.ObjectReference result = null;
            
            if( GUIBuilder.CustomForms.WorkshopBorderGeneratorKeyword == null )
            {
                DebugLog.WriteError( string.Format( "Keyword:  {0} = null!", GUIBuilder.WorkshopBatch.WSDS_KYWD_BorderGenerator ) );
                goto localAbort;
            }

            var forms = Form.References;
            if( forms.NullOrEmpty() )
            {
                DebugLog.WriteError( "Workshop has no other forms referencing it" );
                goto localAbort;
            }

            var kywdWBG = GUIBuilder.CustomForms.WorkshopBorderGeneratorKeyword.GetFormID( TargetHandle.Master );

            foreach( var form in forms )
            {
                var refr = form as Engine.Plugin.Forms.ObjectReference;
                if( refr == null )
                    continue;
                
                var lr = refr.LinkedRefs.GetLinkedRef( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired, kywdWBG );
                if( lr == null )
                    continue;
                if( lr != Reference )
                {
                    DebugLog.WriteError( string.Format( "Linked Ref does not match workshop\nworkshop = {0}\nlinked ref = {1}\nreference = {2}\nkeyword = {3}", Reference.IDString, lr.IDString, refr.IDString, GUIBuilder.CustomForms.WorkshopBorderGeneratorKeyword.IDString ) );
                    goto localAbort;
                }

                result = refr;
                break;
            }

            if( ( result == null )&&( warnOnMissingLink ) )
                DebugLog.WriteWarning( string.Format( "Workshop {0} does not have any references linked to it with keyword {1}\nThis warning \"may\" be safely ignored for this workshop \"depending\" on the context in which it was given.\neg, you didn't setup and link any border markers for it but accidentally selected the workshop when generating nodes.", this.IDString, GUIBuilder.CustomForms.WorkshopBorderGeneratorKeyword.IDString ) );

            localAbort:
            return result;
        }
        
        public List<Engine.Plugin.Forms.ObjectReference> GetBorderMarkers()
        {
            if( _BorderMarkers.NullOrEmpty() )
            {
                var list = (List<Engine.Plugin.Forms.ObjectReference>)null;

                DebugLog.OpenIndentLevel( new string[] {
                    "workshop = " + this.IDString,
                    GUIBuilder.WorkshopBatch.WSDS_KYWD_BorderGenerator + " = " + GUIBuilder.CustomForms.WorkshopBorderGeneratorKeyword.IDString,
                    GUIBuilder.WorkshopBatch.WSDS_KYWD_BorderLink + " = " + GUIBuilder.CustomForms.WorkshopBorderLinkKeyword.IDString
                    }, true, true, false );
                
                var abort = false;
                if( GUIBuilder.CustomForms.WorkshopBorderGeneratorKeyword == null )
                {
                    DebugLog.WriteError( string.Format( "Keyword:  {0} = null!", GUIBuilder.WorkshopBatch.WSDS_KYWD_BorderGenerator ) );
                    abort = true;
                }
                if( GUIBuilder.CustomForms.WorkshopBorderLinkKeyword == null )
                {
                    DebugLog.WriteError( string.Format( "Keyword:  {0} = null!", GUIBuilder.WorkshopBatch.WSDS_KYWD_BorderLink ) );
                    abort = true;
                }
                if( abort ) goto localAbort;

                var firstMarker = GetFirstBorderMarker( true );
                if( firstMarker == null )
                {
                    //GetFirstBorderMarker() will throw a warning to the log, no need to write a reundant error here
                    //DebugLog.WriteError( "GetFirstBorderNode() returned null!" );
                    goto localAbort;
                }

                var kywdWBL = GUIBuilder.CustomForms.WorkshopBorderLinkKeyword.GetFormID( TargetHandle.Master );

                var node = firstMarker.LinkedRefs.GetLinkedRef( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired, kywdWBL );
                if( node == null )
                    goto localAbort;

                list = new List<Engine.Plugin.Forms.ObjectReference>();

                var closedLoop = false;
                list.Add( firstMarker );
                list.Add( node );
                while( true )
                {
                    node = node.LinkedRefs.GetLinkedRef( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired, kywdWBL );
                    if( node == null )
                        break;
                    list.Add( node );
                    closedLoop = ( node == firstMarker );
                    if( closedLoop )
                        break;
                }

                if( !closedLoop )
                {
                    var efCount = list.Count;
                    DebugLog.WriteWarning( string.Format( "Workshop border markers do not form a complete loop.\n\tWorkshop = {0}\n\tFlag count = {1}\n\tFirst = {2}\n\tLast = {3}", this.IDString, efCount, list[ 0 ].IDString, list[ efCount - 1 ].IDString ) );
                }
                
                _BorderMarkers = list;
        localAbort:
                DebugLog.CloseIndentList( "nodes", list, true, true );
            }

            return _BorderMarkers;
        }
        

        public void ClearBorderMarkersAndNodes( bool sendchangedevent )
        {
            _BorderMarkers = null;
            BorderNodes = null;
            if( sendchangedevent )
                SendObjectDataChangedEvent( this );
        }
        
    }
    
}
