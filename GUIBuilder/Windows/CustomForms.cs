/*
 * Created by SharpDevelop.
 * Date: 03/01/2020
 * Time: 8:45 PM
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace GUIBuilder.Windows
{
    /// <summary>
    /// Description of CustomForms.
    /// </summary>
    public partial class CustomForms : WindowBase
    {


        static readonly string[]   WSDS_KYWD_DetectionForms = new [] { GUIBuilder.WorkshopBatch.WSDS_KYWD_BorderGenerator , GUIBuilder.WorkshopBatch.WSDS_KYWD_BorderLink };
        static readonly string[]   WSDS_STAT_DetectionForms = new [] { GUIBuilder.WorkshopBatch.WSDS_STAT_TerrainFollowing, GUIBuilder.WorkshopBatch.WSDS_STAT_ForcedZ    };
        static readonly string[]   WSDS_LCRT_DetectionForms = new [] { GUIBuilder.WorkshopBatch.WSDS_LCRT_BorderWithBottom };

        /// <summary>
        /// Use GodObject.Windows.GetWindow<CustomForms>() to create this Window
        /// </summary>
        public CustomForms() : base( true )
        {
            InitializeComponent();
            this.OnSetEnableState += new SetEnableStateHandler( this.OnFormSetEnableState );
        }


        #region GodObject.XmlConfig.IXmlConfiguration


        public override string XmlNodeName { get { return "CustomFormsWindow"; } }


        #endregion


        #region Form Management


        #region UI Thread Lock

        object _UI_ThreadLock = new object();
        int _UI_ThreadLock_Counter = 0;
        TimeSpan _UI_ThreadTimer = TimeSpan.Zero;

        bool UpdatingFromThread
        {
            get
            {
                lock( _UI_ThreadLock )
                {
                    return _UI_ThreadLock_Counter > 0;
                }
            }
        }

        void UI_ThreadLock_Increment( bool disableForm, bool pushStatusBar, string statusMessageKey )
        {
            lock( _UI_ThreadLock )
            {
                DebugLog.WriteCaller( false );
                _UI_ThreadLock_Counter++;
                if( _UI_ThreadLock_Counter == 1 )
                {
                    if( disableForm )
                        SetEnableState( false );

                    if( pushStatusBar )
                    {
                        var m = GodObject.Windows.GetWindow<Main>();

                        m.PushStatusMessage();
                        m.SetCurrentStatusMessage( statusMessageKey.Translate() );

                        m.StartSyncTimer();
                        _UI_ThreadTimer = m.SyncTimerElapsed();
                    }
                }
            }
        }

        void UI_ThreadLock_Decrement( bool enableForm, bool popStatusBar )
        {
            lock( _UI_ThreadLock )
            {
                DebugLog.WriteStrings( null, new string[] { "enableForm = " + enableForm.ToString(), "popStatusBar = " + popStatusBar.ToString() }, false, true, false, false );
                if( _UI_ThreadLock_Counter > 0 )
                    _UI_ThreadLock_Counter--;
                if( _UI_ThreadLock_Counter == 0 )
                {
                    if( popStatusBar )
                    {
                        var m = GodObject.Windows.GetWindow<Main>();
                        m.StopSyncTimer( _UI_ThreadTimer );
                        m.PopStatusMessage();
                    }
                    if( enableForm )
                        SetEnableState( true );
                }
            }
        }

        #endregion


        #region UI Events

        void CustomForms_OnLoad( object sender, EventArgs e )
        {

            // TODO:  Add an MRU style list in the combobox pulled from the Workspace
            cbWorkshopContainerFilter.Text = GUIBuilder.WorkshopBatch.WSDS_CONT_WorkshopWorkbench;
            
            StartRepopulationThreads();
            
            cbRestrictWorkshopBorderKeywords.Text = string.Format(
                "{0}:\n{1}",
                "CustomFormsWindow.Restrict".Translate(),
                GodObject.Plugin.Data.Files.Working.Filename );
        }

        /// <summary>
        /// Handle window specific global enable/disable events.
        /// </summary>
        /// <param name="enabled">Enable state to set</param>
        void cbRestrictWorkshopBorderKeywordsChanged( object sender, EventArgs e )
        {
            StartRepopulationThreads();
        }

        void OnFormSetEnableState( bool enabled )
        {
            // Only handle UI events when the Window is enabled so threads populating UI controls don't trigger the events
            if( enabled )
            {
                cbRestrictWorkshopBorderKeywords.CheckStateChanged += cbRestrictWorkshopBorderKeywordsChanged;
                cbWorkshopBordeRefBorderWithBottom.SelectedIndexChanged += cbWorkshopBordeRefBorderWithBottomSelectedIndexChanged;
                cbWorkshopBorderMarkerTerrainFollowing.SelectedIndexChanged += cbWorkshopBorderMarkerTerrainFollowingSelectedIndexChanged;
                cbWorkshopBorderMarkerForcedZ.SelectedIndexChanged += cbWorkshopBorderMarkerForcedZSelectedIndexChanged;
                cbWorkshopKeywordBorderGenerator.SelectedIndexChanged += cbWorkshopKeywordBorderGeneratorSelectedIndexChanged;
                cbWorkshopKeywordBorderLink.SelectedIndexChanged += cbWorkshopKeywordBorderLinkSelectedIndexChanged;
            }
            else
            {
                cbRestrictWorkshopBorderKeywords.CheckStateChanged -= cbRestrictWorkshopBorderKeywordsChanged;
                cbWorkshopBordeRefBorderWithBottom.SelectedIndexChanged -= cbWorkshopBordeRefBorderWithBottomSelectedIndexChanged;
                cbWorkshopBorderMarkerTerrainFollowing.SelectedIndexChanged -= cbWorkshopBorderMarkerTerrainFollowingSelectedIndexChanged;
                cbWorkshopBorderMarkerForcedZ.SelectedIndexChanged -= cbWorkshopBorderMarkerForcedZSelectedIndexChanged;
                cbWorkshopKeywordBorderGenerator.SelectedIndexChanged -= cbWorkshopKeywordBorderGeneratorSelectedIndexChanged;
                cbWorkshopKeywordBorderLink.SelectedIndexChanged -= cbWorkshopKeywordBorderLinkSelectedIndexChanged;
            }
        }

        #endregion


        #region Common Thread Methods

        void StartRepopulationThreads()
        {
            // Start container search first as it will take the longest so it can work while this thread starts the other threads
            StartThread_RepopulateWorkshopContainers();

            // Start smaller, faster search threads after starting the heavier thread searching through containers
            StartThread_RepopulateWorkshopNodeDetectionForms();
        }

        void OnRepopulationThreadStarted()
        {
            DebugLog.WriteCaller( false );
            UI_ThreadLock_Increment( true, true, "CustomFormsWindow.SearchingForForms" );
        }

        void OnRepopulationThreadFinished()
        {
            DebugLog.WriteCaller( false );
            UI_ThreadLock_Decrement( true, true );
        }

        #endregion

        #endregion


        #region Workshop Border Detection Forms

        void cbWorkshopKeywordBorderGeneratorSelectedIndexChanged( object sender, EventArgs e )
        {
            GUIBuilder.CustomForms.WorkshopBorderGeneratorKeyword = GUIBuilder.CustomForms.FormFromComboBox<Engine.Plugin.Forms.Keyword>( cbWorkshopKeywordBorderGenerator );
        }
        
        void cbWorkshopKeywordBorderLinkSelectedIndexChanged( object sender, EventArgs e )
        {
            GUIBuilder.CustomForms.WorkshopBorderLinkKeyword = GUIBuilder.CustomForms.FormFromComboBox<Engine.Plugin.Forms.Keyword>( cbWorkshopKeywordBorderLink );
        }
        
        void cbWorkshopBorderMarkerTerrainFollowingSelectedIndexChanged( object sender, EventArgs e )
        {
            GUIBuilder.CustomForms.WorkshopTerrainFollowingMarker = GUIBuilder.CustomForms.FormFromComboBox<Engine.Plugin.Forms.Static>( cbWorkshopBorderMarkerTerrainFollowing );
        }
        
        void cbWorkshopBorderMarkerForcedZSelectedIndexChanged( object sender, EventArgs e )
        {
            GUIBuilder.CustomForms.WorkshopTerrainFollowingMarker = GUIBuilder.CustomForms.FormFromComboBox<Engine.Plugin.Forms.Static>( cbWorkshopBorderMarkerForcedZ );
        }
        
        void cbWorkshopBordeRefBorderWithBottomSelectedIndexChanged( object sender, EventArgs e )
        {
            GUIBuilder.CustomForms.WorkshopBorderWithBottomRef = GUIBuilder.CustomForms.FormFromComboBox<Engine.Plugin.Forms.LocationRef>( cbWorkshopBordeRefBorderWithBottom );
        }

        bool StartThread_RepopulateWorkshopNodeDetectionForms()
        {
            var filter = cbRestrictWorkshopBorderKeywords.Checked
                ? (int)GodObject.Plugin.Data.Files.Working.LoadOrder
                : -1;

            bool anyThreadsStarted = true;

            anyThreadsStarted &= StartComboBoxRepopulationThreadEx(
                new ComboBox[] { cbWorkshopKeywordBorderGenerator, cbWorkshopKeywordBorderLink },
                WSDS_KYWD_DetectionForms,
                typeof( Engine.Plugin.Forms.Keyword ),
                filter );

            anyThreadsStarted &= StartComboBoxRepopulationThreadEx(
                new ComboBox[] { cbWorkshopBorderMarkerTerrainFollowing, cbWorkshopBorderMarkerForcedZ },
                WSDS_STAT_DetectionForms,
                typeof( Engine.Plugin.Forms.Static ),
                filter );

            anyThreadsStarted &= StartComboBoxRepopulationThreadEx(
                new ComboBox[] { cbWorkshopBordeRefBorderWithBottom },
                WSDS_LCRT_DetectionForms,
                typeof( Engine.Plugin.Forms.LocationRef ),
                filter );

            return anyThreadsStarted;
        }

        bool StartComboBoxRepopulationThreadEx( ComboBox[] comboBoxes, string[] suffixes, Type formType, int filter )
        {
            return GUIBuilder.CustomForms.StartComboBoxRepopulationThread(
                this,
                comboBoxes,
                suffixes,
                formType,
                filter,
                OnRepopulationThreadStarted,
                OnRepopulationThreadFinished );
        }

        #endregion


        #region Workshop Containers

        void btnApplyWorkshopContainerFilterClick( object sender, EventArgs e )
        {
            StartThread_RepopulateWorkshopContainers();
        }
        
        void btnApplyWorkshopContainerSelectionClick( object sender, EventArgs e )
        {
            SetEnableState( false );

            GUIBuilder.CustomForms.WorkshopWorkbenches = lvWorkshopContainers.GetSelectedSyncObjects();

            SetEnableState( true );
        }

        bool StartThread_RepopulateWorkshopContainers()
        {
            return GUIBuilder.CustomForms.StartSyncedListViewRepopulationThread(
                this,
                lvWorkshopContainers,
                GUIBuilder.CustomForms.WorkshopWorkbenches,
                cbWorkshopContainerFilter.Text,
                cbRestrictWorkshopBorderKeywords.Checked
                    ? (int)GodObject.Plugin.Data.Files.Working.LoadOrder
                    : -1,
                OnRepopulationThreadStarted,
                OnRepopulationThreadFinished
                );
        }

        #endregion

    }
}
