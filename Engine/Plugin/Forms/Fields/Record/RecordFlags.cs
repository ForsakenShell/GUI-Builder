/*
 * RecordFlags.cs
 *
 * Record Flags field used by all forms.
 *
 */

using System;
using XeLib;


namespace Engine.Plugin.Forms.Fields.Record
{
    
    public class Flags : ValueField<uint>
    {
        
        #region Record Flags Values
        
        [Flags]
        public enum Common : uint
        {
            NonOccluder                     = 0x00000010,
            Deleted                         = 0x00000020,
            HiddenFromLocalMap              = 0x00000200,
            HeadtrackMarker                 = 0x00000400,
            Persistent                      = 0x00000400,
            UsedAsPlatform                  = 0x00000800,
            Ignored                         = 0x00001000,
            PackInUseOnly                   = 0x00002000,
            PartialForm                     = 0x00004000,
            HasDistantLOD                   = 0x00008000,
            IsMarker                        = 0x00800000,
            Obstacle                        = 0x02000000,
            NavMeshGeneration_Filter        = 0x04000000,
            NavMeshGeneration_BoundingBox   = 0x08000000,
            NavMeshGeneration_Ground        = 0x40000000
        }
        
        [Flags]
        public enum ACTI : uint
        {
            NeverFades                      = 0x00000004,
            HeadingMarker                   = 0x00000080,
            MustUpdateAnims                 = 0x00000100,
            RandomAnimStart                 = 0x00010000,
            Dangerous                       = 0x00020000,
            IgnoreObjectInteraction         = 0x00100000,
            ChildCanUse                     = 0x20000000
        }
        
        [Flags]
        public enum CELL : uint
        {
            NoPreVis                        = 0x00000080,
            OffLimits                       = 0x00020000,
            Compressed                      = 0x00040000,
            CantWait                        = 0x00080000
        }
        
        [Flags]
        public enum GLOB : uint
        {
            Constant                        = 0x00000040
        }
        
        [Flags]
        public enum KYWD : uint
        {
            Restricted                      = 0x00008000
        }
        
        [Flags]
        public enum MISC : uint
        {
            CalcFromComponents              = 0x00000800
        }
        
        [Flags]
        public enum PERK : uint
        {
            NonPlayable                     = 0x00000004
        }
        
        [Flags]
        public enum PKIN : uint
        {
            Prefab                          = 0x00000200
        }
        
        [Flags]
        public enum REFR : uint
        {
            GroundPiece                     = 0x00000010,
            LODRespectsEnableState          = 0x00000100,
            HiddenFromLocalMap              = 0x00000200,
            InitiallyDisabled               = 0x00000800,
            VisibleWhenDistant              = 0x00008000,
            IsFullLOD                       = 0x00010000,
            Filter_CollisionGeometry        = 0x04000000,
            BoundingBox_CollisionGeometry   = 0x08000000,
            ReflectedByAutoWater            = 0x10000000,
            DontHavokSettle                 = 0x20000000,
            NoRespawn                       = 0x40000000,
            Multibound                      = 0x80000000
        }
        
        [Flags]
        public enum STAT : uint
        {
            HeadingMarker                   = 0x00000004,
            HasTreeLOD                      = 0x00000040,
            AddOnLODObject                  = 0x00000080,
            UsesHDLODTexture                = 0x00020000,
            HasCurrents                     = 0x00080000,
            ShowInWorldMap_SkyCellOnly      = 0x10000000
        }
        
        [Flags]
        public enum TERM : uint
        {
            Unknown_4                       = 0x00000010,
            Unknown_13                      = 0x00002000,
            RandomAnimStart                 = 0x00010000
        }
        
        [Flags]
        public enum WRLD : uint
        {
            CantWait                        = 0x00080000
        }
        
        #endregion
        
        #region Flags Strings
        
        #region Common
        
        const string _Deleted                   = "Deleted";
        const string _HasDistantLOD             = "Has Distant LOD";
        const string _HeadingMarker             = "Heading Marker";
        const string _HeadtrackMarker           = "Headtrack Marker";
        const string _HiddenFromLocalMap        = "Hidden From Local Map";
        const string _Ignored                   = "Ignored";
        const string _IsMarker                  = "Is Marker";
        const string _NavMeshGenerationBoundingBox = "NavMesh Generation - Bounding Box";
        const string _NavMeshGenerationFilter   = "NavMesh Generation - Filter";
        const string _NavMeshGenerationGround   = "NavMesh Generation - Ground";
        const string _NonOccluder               = "Non Occluder";
        const string _Obstacle                  = "Obstacle";
        const string _PackInUseOnly             = "Pack-In Use Only";
        const string _PartialForm               = "Partial Form";
        const string _Persistent                = "Persistent";
        const string _UsedAsPlatform            = "Used as Platform";
        
        #endregion
        
        #region CELL - Cells
        
        const string _Compressed                = "Compressed";
        const string _CantWait                  = "Can't Wait";
        const string _NoPreVis                  = "No Pre Vis";
        const string _OffLimits                 = "Off Limits";
        
        #endregion
        
        #region REFR - Object References
        
        const string _BoundingBoxCollisionGeometry = "Bounding Box (Collision Geometry)";
        const string _DontHavokSettle           = "Don't Havok Settle";
        const string _FilterCollisionGeometry   = "Filter (Collision Geometry)";
        const string _GroundPiece               = "Ground Piece";
        const string _InitiallyDisabled         = "Initially Disabled";
        const string _IsFullLOD                 = "Is Full LOD";
        const string _LODRespectEnableState     = "LOD Respects Enable State";
        const string _Multibound                = "Multibound";
        const string _NoRespawn                 = "No Respawn";
        const string _VisibleWhenDistant        = "Visible when distant";
        const string _ReflectedByAutoWater      = "Reflected By Auto Water";
        
        #endregion
        
        #region STAT - Statics
        
        const string _AddOnLODObject            = "Add-On LOD Object";
        const string _HasCurrents               = "Has Currents";
        const string _HasTreeLOD                = "Had Tree LOD";
        const string _UsesHDLODTexture          = "Uses HD LOD Texture";
        const string _ShowInWorldMapSkyCellOnly = "Show In World Map (Sky Cell Only)";
        
        #endregion
        
        #region ACTI - Activators
        
        const string _ChildCanUse               = "Child Can Use";
        const string _Dangerous                 = "Dangerous";
        const string _IgnoreObjectInteraction   = "Ignore Object Interaction";
        const string _NeverFades                = "Never Fades";
        const string _MustUpdateAnims           = "Must Update Anims";
        const string _RandomAnimStart           = "Random Anim Star";
        
        #endregion
        
        #endregion
        
        const string                   _Flags                       = "Record Flags";
        
        FormHandle                      cached_Handle               = null;
        uint                           _value;
        
        #region Base Overrides
        
        public                          Flags( Form form ) : base( form, "Record Header" ) {}
        
        public override uint            GetValue( TargetHandle target )
        {
            var h = HandleFromTarget( target ) as XeLib.FormHandle;
            if( ( cached_Handle != null )&&( h == cached_Handle ) ) return _value;
            cached_Handle = h;
            _value = ReadUInt( h, _Flags );
            return _value;
        }
        
        public override void            SetValue( TargetHandle target, uint value )
        {
            var h = HandleFromTarget( target ) as XeLib.FormHandle;
            cached_Handle = h;
            _value = value;
            WriteUInt( _Flags, value, true );
        }
        
        public override string          ToString( TargetHandle target, string format = null )
        {
            var h = HandleFromTarget( target ) as XeLib.FormHandle;
            if( !h.IsValid() ) throw new InvalidCastException();
            var setFlags = h.GetEnabledFlags( BuildPath( _Flags ) );
            string result = string.Format( "0x{0} - " + GetValue( target ).ToString( "X8" ) );
            foreach( var f in setFlags )
            {
                if( !string.IsNullOrEmpty( f ) )
                    result += string.Format( "{0}, ", f );
            }
            
            return result;
        }
        
        #endregion
        
        
        #region Actual Getter/Setter
        
        public string[]                 AllFlags( TargetHandle target )
        {
            var h = HandleFromTarget( target ) as XeLib.FormHandle;
            return h.GetRecordFlags();
        }
        
        public bool                     GetFlag( TargetHandle target, string flag )
        {
            var h = HandleFromTarget( target ) as XeLib.FormHandle;
            return h.GetRecordFlag( flag );
        }
        
        public void                     SetFlag( TargetHandle target, string flag, bool value )
        {
            // TODO:  Make this code more generic instead of just targetting GUIBuilder's needs
            if( target != TargetHandle.Working ) throw new ArgumentException( "target is invalid for field!" );
            var h = HandleFromTarget( target ) as XeLib.FormHandle;
            if( !CreateRootElement( true, false ) ) return;
            h.SetRecordFlag( flag, value );
            Form.SendObjectDataChangedEvent();
        }
        
        #endregion
        
        /*
        
        #region Common
        
        public bool Deleted
        {
            get { return GetFlag( _Deleted ); }
            set { SetFlag( _Deleted, value ); }
        }
        
        public bool HasDistantLOD
        {
            get { return GetFlag( _HasDistantLOD ); }
            set { SetFlag( _HasDistantLOD, value ); }
        }
        
        public bool HeadingMarker
        {
            get { return GetFlag( _HeadingMarker ); }
            set { SetFlag( _HeadingMarker, value ); }
        }
        
        public bool HeadtrackMarker
        {
            get { return GetFlag( _HeadtrackMarker ); }
            set { SetFlag( _HeadtrackMarker, value ); }
        }
        
        public bool HiddenFromLocalMap
        {
            get { return GetFlag( _HiddenFromLocalMap ); }
            set { SetFlag( _HiddenFromLocalMap, value ); }
        }
        
        public bool Ignored
        {
            get { return GetFlag( _Ignored ); }
            set { SetFlag( _Ignored, value ); }
        }
        
        public bool IsMarker
        {
            get { return GetFlag( _IsMarker ); }
            set { SetFlag( _IsMarker, value ); }
        }
        
        public bool NavMeshGenerationBoundingBox
        {
            get { return GetFlag( _NavMeshGenerationBoundingBox ); }
            set { SetFlag( _NavMeshGenerationBoundingBox, value ); }
        }
        
        public bool NavMeshGenerationFilter
        {
            get { return GetFlag( _NavMeshGenerationFilter ); }
            set { SetFlag( _NavMeshGenerationFilter, value ); }
        }
        
        public bool NavMeshGenerationGround
        {
            get { return GetFlag( _NavMeshGenerationGround ); }
            set { SetFlag( _NavMeshGenerationGround, value ); }
        }
        
        public bool NonOccluder
        {
            get { return GetFlag( _NonOccluder ); }
            set { SetFlag( _NonOccluder, value ); }
        }
        
        public bool Obstacle
        {
            get { return GetFlag( _Obstacle ); }
            set { SetFlag( _Obstacle, value ); }
        }
        
        public bool PackInUseOnly
        {
            get { return GetFlag( _PackInUseOnly ); }
            set { SetFlag( _PackInUseOnly, value ); }
        }
        
        public bool PartialForm
        {
            get { return GetFlag( _PartialForm ); }
            set { SetFlag( _PartialForm, value ); }
        }
        */
        
        public bool GetPersistent( TargetHandle target )
        {
            return GetFlag( target, _Persistent );
        }
        public void SetPersistent( TargetHandle target, bool value )
        {
            SetFlag( target, _Persistent, value );
        }
        
        /*
        public bool UsedAsPlatform
        {
            get { return GetFlag( _UsedAsPlatform ); }
            set { SetFlag( _UsedAsPlatform, value ); }
        }
        
        #endregion
        
        
        #region CELL - Cells
        
        public bool CantWait
        {
            get { return GetFlag( _CantWait ); }
            set { SetFlag( _CantWait, value ); }
        }
        
        public bool Compressed
        {
            get { return GetFlag( _Compressed ); }
            set { SetFlag( _Compressed, value ); }
        }
        
        public bool NoPreVis
        {
            get { return GetFlag( _NoPreVis ); }
            set { SetFlag( _NoPreVis, value ); }
        }
        
        public bool OffLimits
        {
            get { return GetFlag( _OffLimits ); }
            set { SetFlag( _OffLimits, value ); }
        }
        
        #endregion
        
        
        #region REFR - Object References
        
        public bool BoundingBoxCollisionGeometry
        {
            get { return GetFlag( _BoundingBoxCollisionGeometry ); }
            set { SetFlag( _BoundingBoxCollisionGeometry, value ); }
        }
        
        public bool DontHavokSettle
        {
            get { return GetFlag( _DontHavokSettle ); }
            set { SetFlag( _DontHavokSettle, value ); }
        }
        
        public bool FilterCollisionGeometry
        {
            get { return GetFlag( _FilterCollisionGeometry ); }
            set { SetFlag( _FilterCollisionGeometry, value ); }
        }
        
        public bool GroundPiece
        {
            get { return GetFlag( _GroundPiece ); }
            set { SetFlag( _GroundPiece, value ); }
        }
        
        public bool InitiallyDisabled
        {
            get { return GetFlag( _InitiallyDisabled ); }
            set { SetFlag( _InitiallyDisabled, value ); }
        }
        
        public bool IsFullLOD
        {
            get { return GetFlag( _IsFullLOD ); }
            set { SetFlag( _IsFullLOD, value ); }
        }
        
        public bool LODRespectsEnableState
        {
            get { return GetFlag( _LODRespectEnableState ); }
            set { SetFlag( _LODRespectEnableState, value ); }
        }
        
        public bool Multibound
        {
            get { return GetFlag( _Multibound ); }
            set { SetFlag( _Multibound, value ); }
        }
        
        public bool NoRespawn
        {
            get { return GetFlag( _NoRespawn ); }
            set { SetFlag( _NoRespawn, value ); }
        }
        
        public bool ReflectedByAutoWater
        {
            get { return GetFlag( _ReflectedByAutoWater ); }
            set { SetFlag( _ReflectedByAutoWater, value ); }
        }
        
        public bool VisibleWhenDistant
        {
            get { return GetFlag( _VisibleWhenDistant ); }
            set { SetFlag( _VisibleWhenDistant, value ); }
        }
        
        #endregion
        
        
        #region STAT - Statics
        
        public bool AddOnLODObject
        {
            get { return GetFlag( _AddOnLODObject ); }
            set { SetFlag( _AddOnLODObject, value ); }
        }
        
        public bool HasCurrents
        {
            get { return GetFlag( _HasCurrents ); }
            set { SetFlag( _HasCurrents, value ); }
        }
        
        public bool HasTreeLOD
        {
            get { return GetFlag( _HasTreeLOD ); }
            set { SetFlag( _HasTreeLOD, value ); }
        }
        
        public bool ShowInWorldMapSkyCellOnly
        {
            get { return GetFlag( _ShowInWorldMapSkyCellOnly ); }
            set { SetFlag( _ShowInWorldMapSkyCellOnly, value ); }
        }
        
        public bool UsesHDLODTexture
        {
            get { return GetFlag( _UsesHDLODTexture ); }
            set { SetFlag( _UsesHDLODTexture, value ); }
        }
        
        #endregion
        
        
        #region ACTI - Activators
        
        public bool ChildCanUse
        {
            get { return GetFlag( _ChildCanUse ); }
            set { SetFlag( _ChildCanUse, value ); }
        }
        
        public bool Dangerous
        {
            get { return GetFlag( _Dangerous ); }
            set { SetFlag( _Dangerous, value ); }
        }
        
        public bool IgnoreObjectInteraction
        {
            get { return GetFlag( _IgnoreObjectInteraction ); }
            set { SetFlag( _IgnoreObjectInteraction, value ); }
        }
        
        public bool NeverFades
        {
            get { return GetFlag( _NeverFades ); }
            set { SetFlag( _NeverFades, value ); }
        }
        
        public bool MustUpdateAnims
        {
            get { return GetFlag( _MustUpdateAnims ); }
            set { SetFlag( _MustUpdateAnims, value ); }
        }
        
        #endregion
        
        */
       
    }
    
}
