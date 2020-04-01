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
    /// Use GodObject.Windows.GetWindow<CustomForms>() to create this Window
    /// </summary>
    public partial class CustomForms : WindowBase
    {

        public CustomForms() : base( true )
        {
            InitializeComponent();
            this.SuspendLayout();

            this.ClientLoad += new System.EventHandler( this.OnClientLoad );
            this.OnSetEnableState += new SetEnableStateHandler( this.OnClientSetEnableState );

            this.btnApplyWorkshopContainerSelection.Click += new System.EventHandler( this.OnApplyWorkshopContainerSelectionButtonClick );
            this.btnApplyWorkshopContainerFilter.Click += new System.EventHandler( this.OnApplyWorkshopContainerFilterButtonClick );

            this.lvWorkshopContainers.OnSetSyncObjectsThreadComplete += OnSyncObjectsThreadComplete;

            this.ResumeLayout( true );
        }


        static readonly string[]   WSDS_KYWD_DetectionForms = new [] { GUIBuilder.WorkshopBatch.WSDS_KYWD_BorderGenerator , GUIBuilder.WorkshopBatch.WSDS_KYWD_BorderLink };
        static readonly string[]   WSDS_STAT_DetectionForms = new [] { GUIBuilder.WorkshopBatch.WSDS_STAT_TerrainFollowing, GUIBuilder.WorkshopBatch.WSDS_STAT_ForcedZ    };
        static readonly string[]   WSDS_LCRT_DetectionForms = new [] { GUIBuilder.WorkshopBatch.WSDS_LCRT_BorderWithBottom };


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
                        SetEnableState( this, false );

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
                UI_ThreadLock_Release( enableForm, popStatusBar );
            }
        }


        // MUST HOLD _UI_ThreadLock!
        void UI_ThreadLock_Release( bool enableForm, bool popStatusBar )
        {
            if(
                ( _UI_ThreadLock_Counter == 0 )&&
                ( !lvWorkshopContainers.IsSyncObjectsThreadRunning )
            ) {
                if( popStatusBar )
                {
                    var m = GodObject.Windows.GetWindow<Main>();
                    m.StopSyncTimer( _UI_ThreadTimer );
                    m.PopStatusMessage();
                }
                if( enableForm )
                    SetEnableState( this, true );
            }
        }

        #endregion


        #region UI Events

        void OnClientLoad( object sender, EventArgs e )
        {

            // TODO:  Add an MRU style list in the combobox pulled from the Workspace
            cbWorkshopContainerFilter.Text = GUIBuilder.WorkshopBatch.WSDS_CONT_WorkshopWorkbench;
            
            StartRepopulationThreads();
            
            cbRestrictWorkshopForms.Text = string.Format(
                "{0}:\n{1}",
                "CustomFormsWindow.Restrict".Translate(),
                GodObject.Plugin.Data.Files.Working.Filename );
        }

        void OnRestrictWorkshopFormsChanged( object sender, EventArgs e )
        {
            StartRepopulationThreads();
        }

        /// <summary>
        /// Handle window specific global enable/disable events.
        /// </summary>
        /// <param name="enable">Enable state to set</param>
        bool OnClientSetEnableState( object sender, bool enable )
        {
            var enabled =
                enable &&
                !lvWorkshopContainers.IsSyncObjectsThreadRunning;

            // Only handle UI events when the Window is enabled so threads populating UI controls don't trigger the events
            if( enabled )
            {
                cbRestrictWorkshopForms.CheckStateChanged += OnRestrictWorkshopFormsChanged;
                cbWorkshopKeywordBorderGenerator.SelectedIndexChanged += OnBorderGeneratorChanged;
                cbWorkshopKeywordBorderLink.SelectedIndexChanged += OnBorderLinkChanged;
                cbWorkshopBorderMarkerTerrainFollowing.SelectedIndexChanged += OnTerrainFollowingChanged;
                cbWorkshopBorderMarkerForcedZ.SelectedIndexChanged += OnForcedZChanged;
                cbWorkshopBordeRefBorderWithBottom.SelectedIndexChanged += OnBorderWithBottomChanged;
            }
            else
            {
                cbRestrictWorkshopForms.CheckStateChanged -= OnRestrictWorkshopFormsChanged;
                cbWorkshopKeywordBorderGenerator.SelectedIndexChanged -= OnBorderGeneratorChanged;
                cbWorkshopKeywordBorderLink.SelectedIndexChanged -= OnBorderLinkChanged;
                cbWorkshopBorderMarkerTerrainFollowing.SelectedIndexChanged -= OnTerrainFollowingChanged;
                cbWorkshopBorderMarkerForcedZ.SelectedIndexChanged -= OnForcedZChanged;
                cbWorkshopBordeRefBorderWithBottom.SelectedIndexChanged -= OnBorderWithBottomChanged;
            }
            return enabled;
        }

        void OnSyncObjectsThreadComplete( GUIBuilder.Windows.Controls.SyncedListView<Engine.Plugin.Forms.Container> sender )
        {
            lock( _UI_ThreadLock )
            {
                UI_ThreadLock_Release( true, true );
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

        void OnBorderGeneratorChanged( object sender, EventArgs e )
        {
            GUIBuilder.CustomForms.WorkshopBorderGeneratorKeyword = GUIBuilder.CustomForms.FormFromComboBox<Engine.Plugin.Forms.Keyword>( cbWorkshopKeywordBorderGenerator );
        }
        
        void OnBorderLinkChanged( object sender, EventArgs e )
        {
            GUIBuilder.CustomForms.WorkshopBorderLinkKeyword = GUIBuilder.CustomForms.FormFromComboBox<Engine.Plugin.Forms.Keyword>( cbWorkshopKeywordBorderLink );
        }
        
        void OnTerrainFollowingChanged( object sender, EventArgs e )
        {
            GUIBuilder.CustomForms.WorkshopTerrainFollowingMarker = GUIBuilder.CustomForms.FormFromComboBox<Engine.Plugin.Forms.Static>( cbWorkshopBorderMarkerTerrainFollowing );
        }
        
        void OnForcedZChanged( object sender, EventArgs e )
        {
            GUIBuilder.CustomForms.WorkshopTerrainFollowingMarker = GUIBuilder.CustomForms.FormFromComboBox<Engine.Plugin.Forms.Static>( cbWorkshopBorderMarkerForcedZ );
        }
        
        void OnBorderWithBottomChanged( object sender, EventArgs e )
        {
            GUIBuilder.CustomForms.WorkshopBorderWithBottomRef = GUIBuilder.CustomForms.FormFromComboBox<Engine.Plugin.Forms.LocationRef>( cbWorkshopBordeRefBorderWithBottom );
        }

        bool StartThread_RepopulateWorkshopNodeDetectionForms()
        {
            var filter = cbRestrictWorkshopForms.Checked
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

        void OnApplyWorkshopContainerFilterButtonClick( object sender, EventArgs e )
        {
            StartThread_RepopulateWorkshopContainers();
        }
        
        void OnApplyWorkshopContainerSelectionButtonClick( object sender, EventArgs e )
        {
            SetEnableState( sender, false );

            GUIBuilder.CustomForms.WorkshopWorkbenches = lvWorkshopContainers.GetSelectedSyncObjects();

            SetEnableState( sender, true );
        }

        bool StartThread_RepopulateWorkshopContainers()
        {
            return GUIBuilder.CustomForms.StartSyncedListViewRepopulationThread(
                this,
                lvWorkshopContainers,
                GUIBuilder.CustomForms.WorkshopWorkbenches,
                cbWorkshopContainerFilter.Text,
                cbRestrictWorkshopForms.Checked
                    ? (int)GodObject.Plugin.Data.Files.Working.LoadOrder
                    : -1,
                OnRepopulationThreadStarted,
                OnRepopulationThreadFinished
                );
        }

        #endregion

    }
}
