/*
 * CoreForms.cs
 * 
 * Core forms used by GUIBuilder, these forms are loaded as soon as the masters/plugins are loaded.
 * 
 */
using System;

using Master = GodObject.Master;

namespace GodObject
{
    
    #region Base Forms
    
    public static class CoreForms
    {
        // TODO: FIX THESE CONSTANTS FOR ACTUAL GAME SETTING FORMS AND MOVE THEM INTO THEIR PROPER PLACE!
        public const float FO4_fSandboxCylinderBottom   =  -100.0f;
        public const float FO4_fSandboxCylinderTop      =  1280.0f;
        public const float ATC_fSandboxCylinderBottom   = -2560.0f;
        public const float ATC_fSandboxCylinderTop      =  2560.0f;
        
        public static Engine.Plugin.Forms.Container WorkshopWorkbench                       = new Engine.Plugin.Forms.Container  ( Master.Filename.Fallout4      , 0x000C1AEB );
        
        public static Engine.Plugin.Forms.Keyword WorkshopLinkedBuildAreaEdge               = new Engine.Plugin.Forms.Keyword    ( Master.Filename.Fallout4      , 0x001A0DD8 );
        public static Engine.Plugin.Forms.Keyword WorkshopLinkedPrimitive                   = new Engine.Plugin.Forms.Keyword    ( Master.Filename.Fallout4      , 0x000B91E6 );
        
        public static Engine.Plugin.Forms.Layer WorkshopBorderArt                           = new Engine.Plugin.Forms.Layer      ( Master.Filename.Fallout4      , 0x001B2CD4 );
        
        public static Engine.Plugin.Forms.Activator ESM_ATC_ACTI_SubDivision                = new Engine.Plugin.Forms.Activator  ( Master.Filename.AnnexTheCommonwealth   , 0x00037B66 );
        public static Engine.Plugin.Forms.Activator ESM_ATC_ACTI_Settlement                 = new Engine.Plugin.Forms.Activator  ( Master.Filename.AnnexTheCommonwealth   , 0x0003BFB7 );
        public static Engine.Plugin.Forms.Activator ESM_ATC_ACTI_BuildAreaVolume            = new Engine.Plugin.Forms.Activator  ( Master.Filename.AnnexTheCommonwealth   , 0x000888BD );
        public static Engine.Plugin.Forms.Activator ESM_ATC_ACTI_Workshop                   = new Engine.Plugin.Forms.Activator  ( Master.Filename.AnnexTheCommonwealth   , 0x000A4ACD );
        public static Engine.Plugin.Forms.Activator ESM_ATC_ACTI_BorderEnabler              = new Engine.Plugin.Forms.Activator  ( Master.Filename.AnnexTheCommonwealth   , 0x000A5A22 );
        public static Engine.Plugin.Forms.Activator ESM_ATC_ACTI_SpawnMarker                = new Engine.Plugin.Forms.Activator  ( Master.Filename.AnnexTheCommonwealth   , 0x000C4DBD );
        public static Engine.Plugin.Forms.Activator ESM_ATC_ACTI_SandboxVolume              = new Engine.Plugin.Forms.Activator  ( Master.Filename.AnnexTheCommonwealth   , 0x00110BD4 );
        
        // Script Threading
        //public static Engine.Plugin.Forms.Activator ESM_ATC_ACTI_ForcedPersistManager      = new Engine.Plugin.Forms.Activator  ( Master.Filename.AnnexTheCommonwealth   , 0x00110B8B );
        //public static Engine.Plugin.Forms.Activator ESM_ATC_ACTI_ForcedPersistScanner      = new Engine.Plugin.Forms.Activator  ( Master.Filename.AnnexTheCommonwealth   , 0x00110BEE );
        //public static Engine.Plugin.Forms.Activator ESM_ATC_ACTI_EnhancedSandboxScanner    = new Engine.Plugin.Forms.Activator  ( Master.Filename.AnnexTheCommonwealth   , 0x00110BEF );
        //public static Engine.Plugin.Forms.Activator ESM_ATC_ACTI_UpdateSettlementOnLoad    = new Engine.Plugin.Forms.Activator  ( Master.Filename.AnnexTheCommonwealth   , 0x00110BF1 );
        //public static Engine.Plugin.Forms.Activator ESM_ATC_ACTI_UpdateSubDivisionOnLoad   = new Engine.Plugin.Forms.Activator  ( Master.Filename.AnnexTheCommonwealth   , 0x00110BF2 );
        
        public static Engine.Plugin.Forms.Keyword ESM_ATC_KYWD_LinkedWorkshop               = new Engine.Plugin.Forms.Keyword    ( Master.Filename.AnnexTheCommonwealth   , 0x00048D56 );
        public static Engine.Plugin.Forms.Keyword ESM_ATC_KYWD_LinkedSettlement             = new Engine.Plugin.Forms.Keyword    ( Master.Filename.AnnexTheCommonwealth   , 0x00048D58 );
        public static Engine.Plugin.Forms.Keyword ESM_ATC_KYWD_LinkedBorder                 = new Engine.Plugin.Forms.Keyword    ( Master.Filename.AnnexTheCommonwealth   , 0x00048D59 );
        public static Engine.Plugin.Forms.Keyword ESM_ATC_KYWD_LinkedSubDivision            = new Engine.Plugin.Forms.Keyword    ( Master.Filename.AnnexTheCommonwealth   , 0x00066C2F );
        public static Engine.Plugin.Forms.Keyword ESM_ATC_KYWD_LinkedBuildAreaVolume        = new Engine.Plugin.Forms.Keyword    ( Master.Filename.AnnexTheCommonwealth   , 0x00066C30 );
        public static Engine.Plugin.Forms.Keyword ESM_ATC_KYWD_LinkedSandboxVolume          = new Engine.Plugin.Forms.Keyword    ( Master.Filename.AnnexTheCommonwealth   , 0x00084A96 );
        public static Engine.Plugin.Forms.Keyword ESM_ATC_KYWD_LinkedSandboxEdge            = new Engine.Plugin.Forms.Keyword    ( Master.Filename.AnnexTheCommonwealth   , 0x0009EEA5 );
        public static Engine.Plugin.Forms.Keyword ESM_ATC_KYWD_LinkedAttackMarker           = new Engine.Plugin.Forms.Keyword    ( Master.Filename.AnnexTheCommonwealth   , 0x000B2C9B );
        public static Engine.Plugin.Forms.Keyword ESM_ATC_KYWD_LinkedBorderDummy            = new Engine.Plugin.Forms.Keyword    ( Master.Filename.AnnexTheCommonwealth   , 0x000B6994 );
        
        public static Engine.Plugin.Forms.Keyword ESM_ATC_KYWD_SubDivision_EdgeLink_A       = new Engine.Plugin.Forms.Keyword    ( Master.Filename.AnnexTheCommonwealth   , 0x00110AF3 );
        public static Engine.Plugin.Forms.Keyword ESM_ATC_KYWD_SubDivision_EdgeLink_B       = new Engine.Plugin.Forms.Keyword    ( Master.Filename.AnnexTheCommonwealth   , 0x00110AF4 );
        public static Engine.Plugin.Forms.Keyword ESM_ATC_KYWD_SubDivision_EdgeLink_C       = new Engine.Plugin.Forms.Keyword    ( Master.Filename.AnnexTheCommonwealth   , 0x00110AF5 );
        public static Engine.Plugin.Forms.Keyword ESM_ATC_KYWD_SubDivision_EdgeLink_D       = new Engine.Plugin.Forms.Keyword    ( Master.Filename.AnnexTheCommonwealth   , 0x00110AF6 );
        public static Engine.Plugin.Forms.Keyword ESM_ATC_KYWD_SubDivision_EdgeLink_E       = new Engine.Plugin.Forms.Keyword    ( Master.Filename.AnnexTheCommonwealth   , 0x00110AF7 );
        public static Engine.Plugin.Forms.Keyword ESM_ATC_KYWD_SubDivision_EdgeLink_F       = new Engine.Plugin.Forms.Keyword    ( Master.Filename.AnnexTheCommonwealth   , 0x00110AF8 );
        public static Engine.Plugin.Forms.Keyword ESM_ATC_KYWD_SubDivision_EdgeLink_G       = new Engine.Plugin.Forms.Keyword    ( Master.Filename.AnnexTheCommonwealth   , 0x00110AF9 );
        public static Engine.Plugin.Forms.Keyword ESM_ATC_KYWD_SubDivision_EdgeLink_H       = new Engine.Plugin.Forms.Keyword    ( Master.Filename.AnnexTheCommonwealth   , 0x00110AFA );
        
        public static Engine.Plugin.Forms.Layer ESM_ATC_LAYR_SandboxVolumes                 = new Engine.Plugin.Forms.Layer      ( Master.Filename.AnnexTheCommonwealth   , 0x000A8054 );
        public static Engine.Plugin.Forms.Layer ESM_ATC_LAYR_Controllers                    = new Engine.Plugin.Forms.Layer      ( Master.Filename.AnnexTheCommonwealth   , 0x000E3762 );
        public static Engine.Plugin.Forms.Layer ESM_ATC_LAYR_BorderMeshes                   = new Engine.Plugin.Forms.Layer      ( Master.Filename.AnnexTheCommonwealth   , 0x000E3763 );
        public static Engine.Plugin.Forms.Layer ESM_ATC_LAYR_SubDivisionBorderNodes         = new Engine.Plugin.Forms.Layer      ( Master.Filename.AnnexTheCommonwealth   , 0x00110AFB );
        
        public static Engine.Plugin.Forms.Static ESM_ATC_STAT_AttackMarker                  = new Engine.Plugin.Forms.Static     ( Master.Filename.AnnexTheCommonwealth   , 0x000BAE0D );
        public static Engine.Plugin.Forms.Static ESM_ATC_STAT_XMarker                       = new Engine.Plugin.Forms.Static     ( Master.Filename.AnnexTheCommonwealth   , 0x000F0F77 );
        public static Engine.Plugin.Forms.Static ESM_ATC_STAT_SubDivisionEdgeFlag           = new Engine.Plugin.Forms.Static     ( Master.Filename.AnnexTheCommonwealth   , 0x000F6312 );
        public static Engine.Plugin.Forms.Static ESM_ATC_STAT_SubDivisionEdgeFlag_ForcedZ   = new Engine.Plugin.Forms.Static     ( Master.Filename.AnnexTheCommonwealth   , 0x00110CA5 );
        
        public static Engine.Plugin.Form[] Forms = {
            
            // Fallout 4
            
            WorkshopWorkbench,
            
            WorkshopLinkedBuildAreaEdge,
            WorkshopLinkedPrimitive,
            
            WorkshopBorderArt,
            
            // Annex The Commonwealth
            
            ESM_ATC_ACTI_SubDivision,
            ESM_ATC_ACTI_Settlement,
            ESM_ATC_ACTI_BuildAreaVolume,
            ESM_ATC_ACTI_Workshop,
            ESM_ATC_ACTI_BorderEnabler,
            ESM_ATC_ACTI_SpawnMarker,
            ESM_ATC_ACTI_SandboxVolume,
            
            // Script Threading
            //ESM_ATC_ACTI_ForcedPersistManager,
            //ESM_ATC_ACTI_ForcedPersistScanner,
            //ESM_ATC_ACTI_EnhancedSandboxScanner,
            //ESM_ATC_ACTI_UpdateSettlementOnLoad,
            //ESM_ATC_ACTI_UpdateSubDivisionOnLoad,
            
            ESM_ATC_KYWD_LinkedWorkshop,
            ESM_ATC_KYWD_LinkedSettlement,
            ESM_ATC_KYWD_LinkedBorder,
            ESM_ATC_KYWD_LinkedSubDivision,
            ESM_ATC_KYWD_LinkedBuildAreaVolume,
            ESM_ATC_KYWD_LinkedSandboxVolume,
            ESM_ATC_KYWD_LinkedSandboxEdge,
            ESM_ATC_KYWD_LinkedAttackMarker,
            ESM_ATC_KYWD_LinkedBorderDummy,
            
            ESM_ATC_KYWD_SubDivision_EdgeLink_A,
            ESM_ATC_KYWD_SubDivision_EdgeLink_B,
            ESM_ATC_KYWD_SubDivision_EdgeLink_C,
            ESM_ATC_KYWD_SubDivision_EdgeLink_D,
            ESM_ATC_KYWD_SubDivision_EdgeLink_E,
            ESM_ATC_KYWD_SubDivision_EdgeLink_F,
            ESM_ATC_KYWD_SubDivision_EdgeLink_G,
            ESM_ATC_KYWD_SubDivision_EdgeLink_H,
            
            ESM_ATC_LAYR_SandboxVolumes,
            ESM_ATC_LAYR_Controllers,
            ESM_ATC_LAYR_BorderMeshes,
            ESM_ATC_LAYR_SubDivisionBorderNodes,
            
            ESM_ATC_STAT_AttackMarker,
            ESM_ATC_STAT_XMarker,
            ESM_ATC_STAT_SubDivisionEdgeFlag,
            ESM_ATC_STAT_SubDivisionEdgeFlag_ForcedZ
            
        };
        
        static Engine.Plugin.Forms.Static[] _SubDivisionEdgeFlags = {
            ESM_ATC_STAT_SubDivisionEdgeFlag,
            ESM_ATC_STAT_SubDivisionEdgeFlag_ForcedZ
        };
        
        static Engine.Plugin.Forms.Keyword[] _SubDivisionEdgeFlagKeywords = {
            ESM_ATC_KYWD_SubDivision_EdgeLink_A,
            ESM_ATC_KYWD_SubDivision_EdgeLink_B,
            ESM_ATC_KYWD_SubDivision_EdgeLink_C,
            ESM_ATC_KYWD_SubDivision_EdgeLink_D,
            ESM_ATC_KYWD_SubDivision_EdgeLink_E,
            ESM_ATC_KYWD_SubDivision_EdgeLink_F,
            ESM_ATC_KYWD_SubDivision_EdgeLink_G,
            ESM_ATC_KYWD_SubDivision_EdgeLink_H
        };
        
        public static Engine.Plugin.Forms.Static SubDivisionEdgeFlag( uint formID )
        {
            if( !Engine.Plugin.Constant.ValidFormID( formID ) )
                return null;
            foreach( var baseFlag in _SubDivisionEdgeFlags )
                if( baseFlag.GetFormID( Engine.Plugin.TargetHandle.Master ) == formID ) return baseFlag;
            return null;
        }
        
        public static bool IsSubDivisionEdgeFlag( uint formID )
        {
            if( !Engine.Plugin.Constant.ValidFormID( formID ) )
                return false;
            foreach( var baseFlag in _SubDivisionEdgeFlags )
                if( baseFlag.GetFormID( Engine.Plugin.TargetHandle.Master ) == formID ) return true;
            return false;
        }
        
        public static bool IsSubDivisionEdgeFlagKeyword( uint formID )
        {
            if( !Engine.Plugin.Constant.ValidFormID( formID ) )
                return false;
            foreach( var sefk in _SubDivisionEdgeFlagKeywords )
                if( formID == sefk.GetFormID( Engine.Plugin.TargetHandle.Master ) )
                    return true;
            return false;
        }
        
        public static Engine.Plugin.Forms.Keyword SubDivisionEdgeFlagKeyword( uint formID )
        {
            if( !Engine.Plugin.Constant.ValidFormID( formID ) )
                return null;
            foreach( var sefk in _SubDivisionEdgeFlagKeywords )
                if( formID == sefk.GetFormID( Engine.Plugin.TargetHandle.Master ) )
                    return sefk;
            return null;
        }
        
    }
    
    #endregion
    
}

