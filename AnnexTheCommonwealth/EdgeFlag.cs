/*
 * EdgeFlag.cs
 *
 * Fake script reference.  This isn't an actual Papyrus script for ATC but is used by
 * GUIBuilder to calculate and store information regarding edge flags.
 *
 */

using System.Collections.Generic;


namespace AnnexTheCommonwealth
{
    
    [Engine.Plugin.Attributes.ScriptAssociation( "ESM:ATC:EdgeFlag" )]
    public class EdgeFlag : Engine.Plugin.PapyrusScript
    {
        
        public Dictionary<uint,SubDivision> kywdSubDivision;
        
        public override bool EditorScript
        {
            get { return true; }
        }
        
        #region Constructor
        
        public EdgeFlag( Engine.Plugin.Forms.ObjectReference reference ) : base( reference )
        {
            kywdSubDivision = new Dictionary<uint, SubDivision>();
        }
        
        #endregion
        
        #region Public Properties
        
        public bool AssociatedWithSubDivision( uint formid )
        {
            //DebugLog.OpenIndentLevel( new [] { this.IDString, "formid = 0x" + formid.ToString( "X8" ) }, true );
            
            var result = true;
            
            foreach( var ks in kywdSubDivision )
                if( ks.Value.GetFormID( Engine.Plugin.TargetHandle.Master ) == formid )
                    goto localReturnResult;
            result = false;
            
        localReturnResult:
            //DebugLog.CloseIndentLevel( result.ToString() );
            return result;
        }
        
        public bool AssociatedWithSubDivision( SubDivision subdivision )
        {
            return
                ( subdivision == null )||
                ( AssociatedWithSubDivision( subdivision.GetFormID( Engine.Plugin.TargetHandle.Master ) ) );
        }
        
        public bool HasAnySharedAssociations( EdgeFlag otherFlag, uint excludeSubDivision = Engine.Plugin.Constant.FormID_None )
        {
            //DebugLog.OpenIndentLevel( new [] { this.IDString, "excludeSubDivision = 0x" + excludeSubDivision.ToString( "X8" ) + "\n", "otherFlag = " + otherFlag.IDString }, true );
            
            var result = true;
            
            foreach( var ks in kywdSubDivision )
            {
                var ksfid = ks.Value.GetFormID( Engine.Plugin.TargetHandle.Master );
                if( ksfid == excludeSubDivision )
                    continue;
                if( otherFlag.AssociatedWithSubDivision( ksfid ) )
                    goto localReturnResult;
            }
            
            result = false;
            
        localReturnResult:
            //DebugLog.CloseIndentLevel( result.ToString() );
            return result;
        }
        
        #endregion
        
        public override List<string> MouseOverExtra
        {
            get
            {
                var lrs = Reference.LinkedRefs;
                if( ( kywdSubDivision.Count == 0 )&&( lrs.GetCount( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) == 0 ) )
                    return null;
                var moel = new List<string>();
                
                if( kywdSubDivision.Count > 0 )
                {
                    moel.Add( "Associated Sub-Divisions:" );
                    foreach( var ks in kywdSubDivision )
                    {
                        var k = GodObject.Plugin.Data.Root.Find<Engine.Plugin.Forms.Keyword>( ks.Key );
                        moel.Add(
                            string.Format(
                                "\tSub-Division: {0} :: Keyword: {1}",
                                ks.Value.IDString,
                                k == null ? "[null]" : k.IDString ) );
                    }
                }
                
                if( lrs.GetCount( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) > 0 )
                {
                    moel.Add( "Linked Flags:" );
                    for( int i = 0; i < lrs.GetCount( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ); i++ )
                    {
                        var r = lrs.GetReference( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired, i );
                        var k = lrs.GetKeyword( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired, i );
                        moel.Add(
                            string.Format(
                                "\tEdgeFlag: {0} :: Keyword: {1}",
                                r.IDString,
                                ( k == null ? "[null]" : k.IDString )
                                ) );
                    }
                }
                
                return moel;
            }
        }
        
    }
    
}
