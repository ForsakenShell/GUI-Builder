/*
 * SubDivisionBatchWindow.cs
 *
 * Insert description here.
 *
 * User: 1000101
 * Date: 23/11/2018
 * Time: 1:51 PM
 * 
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GUIBuilder.Windows
{
    
    /// <summary>
    /// Description of SubDivisionBatch.
    /// </summary>
    public partial class SubDivisionBatch : WindowBase
    {

        /// <summary>
        /// Use GodObject.Windows.GetWindow<SubDivisionBatch>() to create this Window
        /// </summary>
        public SubDivisionBatch() : base( true )
        {
            InitializeComponent();
        }


        #region GodObject.XmlConfig.IXmlConfiguration


        public override string XmlNodeName { get { return "SubDivisionBatchWindow"; } }


        #endregion


        void SubDivisionBatch_OnLoad( object sender, EventArgs e )
        {
            lvSubDivisions.SyncedEditorFormType = typeof( FormEditor.SubDivision );
            GodObject.Plugin.Data.SubDivisions.ObjectDataChanged += OnSubDivisionListChanged;
            UpdateSubDivisionList();
        }

        void OnFormClosing( object sender, FormClosingEventArgs e )
        {
            GodObject.Plugin.Data.SubDivisions.ObjectDataChanged -= OnSubDivisionListChanged;
        }
        
        #region Sync'd list monitoring
        
        void UpdateSubDivisionList()
        {
            var subdivisions = GodObject.Plugin.Data.SubDivisions.ToList( false );
            lvSubDivisions.SyncObjects = subdivisions;
        }
        
        void OnSubDivisionListChanged( object sender, EventArgs e )
        {
            UpdateSubDivisionList();
        }
        
        #endregion
        
        void btnCheckMissingElementsClick( object sender, EventArgs e )
        {
            GodObject.Windows.SetEnableState( false );
            WorkerThreadPool.CreateWorker( THREAD_CheckMissingElements, null ).Start();
        }
        
        void THREAD_CheckMissingElements()
        {
            var subdivisions = lvSubDivisions.GetSelectedSyncObjects();
            
            if( subdivisions.NullOrEmpty() )
            {
                GodObject.Windows.SetEnableState( true );
                return;
            }
            
            var m = GodObject.Windows.GetWindow<GUIBuilder.Windows.Main>();
            m.PushStatusMessage();
            m.SetCurrentStatusMessage( "SubDivisionBatchWindow.CheckingElements".Translate() );
            m.StartSyncTimer();
            var tStart = m.SyncTimerElapsed();
            
            GUIBuilder.SubDivisionBatch.CheckMissingElements(
                subdivisions,
                cbElementBorderEnablers.Checked,
                cbElementSandboxVolumes.Checked );

            m.StopSyncTimer( tStart );
            m.PopStatusMessage();
            GodObject.Windows.SetEnableState( true );
        }
        
        void btnOptimizeSandboxVolumesClick( object sender, EventArgs e )
        {
            GodObject.Windows.SetEnableState( false );
            WorkerThreadPool.CreateWorker( THREAD_OptimizeSandboxVolumes, null ).Start();
        }
        
        void THREAD_OptimizeSandboxVolumes()
        {
            var subdivisions = lvSubDivisions.GetSelectedSyncObjects();
            
            if( subdivisions.NullOrEmpty() )
            {
                GodObject.Windows.SetEnableState( true );
                return;
            }
            
            var m = GodObject.Windows.GetWindow<GUIBuilder.Windows.Main>();
            m.PushStatusMessage();
            m.StartSyncTimer();
            var tStart = m.SyncTimerElapsed();
            
            List<GUIBuilder.FormImport.ImportBase> list = null;
            
            GUIBuilder.SubDivisionBatch.GenerateSandboxes( ref list, subdivisions, m, false, false );
            
            bool allImportsMatchTarget = false;
            FormImport.ImportBase.ShowImportDialog( list, false, ref allImportsMatchTarget );

            m.StopSyncTimer( tStart );
            m.PopStatusMessage();
            GodObject.Windows.SetEnableState( true );
        }
        
        void btnNormalizeBuildVolumesClick(object sender, EventArgs e)
        {
            GodObject.Windows.SetEnableState( false );
            WorkerThreadPool.CreateWorker( THREAD_NormalizeBuildVolumes, null ).Start();
        }
        
        void THREAD_NormalizeBuildVolumes()
        {
            var subdivisions = lvSubDivisions.GetSelectedSyncObjects();
            
            if( subdivisions.NullOrEmpty() )
            {
                GodObject.Windows.SetEnableState( true );
                return;
            }
            
            var m = GodObject.Windows.GetWindow<GUIBuilder.Windows.Main>();
            m.PushStatusMessage();
            m.StartSyncTimer();
            var tStart = m.SyncTimerElapsed();
            
            List<GUIBuilder.FormImport.ImportBase> list = null;
            
            GUIBuilder.SubDivisionBatch.NormalizeBuildVolumes( ref list, subdivisions, m, false );
            
            bool allImportsMatchTarget = false;
            FormImport.ImportBase.ShowImportDialog( list, false, ref allImportsMatchTarget );

            m.StopSyncTimer( tStart );
            m.PopStatusMessage();
            GodObject.Windows.SetEnableState( true );
        }
        
    }
}
