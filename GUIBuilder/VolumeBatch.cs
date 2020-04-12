/*
 * VolumeBatch.cs
 *
 * Batch functions for primitive volumes related to Workshops and Sub-Divisions.
 *
 */
using System;
using System.Collections.Generic;
using System.Linq;

using Maths;

using GUIBuilder.Interface;
using GUIBuilder.FormImport;

using Engine.Plugin;
using Engine.Plugin.Forms;
using Engine.Plugin.Forms.Fields;

using SetEditorID = GUIBuilder.FormImport.Operations.SetEditorID;
using Operations = GUIBuilder.FormImport.Operations;
using Priority = GUIBuilder.FormImport.Priority;
using Shape = Engine.Plugin.Forms.Fields.ObjectReference.Primitive.PrimitiveType;


namespace GUIBuilder
{

    public static class VolumeBatch
    {
        
        public delegate Layer       PreferedLayerFunctionDelegate<TController>( ref List<ImportBase> imports, TargetHandle target, ObjectReference sandbox, TController controller, out string preferedLayerEditorID );

        public static Geometry.ConvexHull.OptimalBoundingBox CalculateOptimalSandboxVolume(
            TargetHandle target,
            List<Vector2f> hull,
            Worldspace worldspace,
            bool scanTerrain,
            float fSandboxCylinderBottom, float fSandboxCylinderTop,
            float volumePadding,
            float hintZ )
        {
            if( hull.NullOrEmpty() )
            {
                DebugLog.WriteError( "No hull to calculate a bounding volume of" );
                return null;
            }
            if( fSandboxCylinderTop <= 0.0f )
            {
                DebugLog.WriteError( "fSandboxCylinderTop must be greater than 0" );
                return null;
            }
            if( fSandboxCylinderBottom >= 0.0f )
            {
                DebugLog.WriteError( "fSandboxCylinderBottom must be less than 0" );
                return null;
            }

            var volOffset = fSandboxCylinderBottom;
            var halfHeight = fSandboxCylinderTop > Math.Abs( fSandboxCylinderBottom )
                ? fSandboxCylinderTop
                : Math.Abs( fSandboxCylinderBottom );

            var optVol = Maths.Geometry.ConvexHull.MinBoundingBox( hull );
            optVol.Height = halfHeight * 2.0f;

            var wsdp = scanTerrain ? worldspace?.PoolEntry : null;
            if( wsdp != null )
            {
                var volumeCorners = new Vector2f[][]{ optVol.Corners };
                float minZ, maxZ, avgZ, avgWaterZ;
                if( wsdp.ComputeZHeightsFromVolumes( target, volumeCorners, out minZ, out maxZ, out avgZ, out avgWaterZ, showScanlineProgress: true ) )
                {
                    var zUse = avgZ;                                        // Start with the average land height
                    if( ( zUse - volOffset ) + fSandboxCylinderBottom > minZ ) zUse = minZ; // Move down to make sure the lowest point is inside the volume
                    if( avgWaterZ > zUse ) zUse = avgWaterZ;                // Move up to the average water surface
                    optVol.Z = zUse - volOffset;
                }
                else
                {
                    DebugLog.WriteError( "Could not compute Z coords from worldspace heightmap" );
                    optVol.Z = hintZ;
                }
            }
            else
                optVol.Z = hintZ;

            // Add offset and margin to final result
            optVol.Z -= volOffset;
            var size = optVol.Size;
            optVol.Size = new Vector3f(
                size.X + volumePadding,
                size.Y + volumePadding,
                size.Z + volumePadding );

            return optVol;
        }

        public static void GenerateSandboxes<TController>(
            Windows.Main m,
            ref List<ImportBase> imports,
            TargetHandle target,
            List<TController> controllers,
            bool createMissing,
            bool ignoreExisting,
            bool scanTerrain,
            float cylinderTop,
            float cylinderBottom,
            float volumePadding,
            uint volumeRefRecordFlags,
            string volumeEditorIDFormat,
            Engine.Plugin.Forms.Activator volumeRefBase,
            System.Drawing.Color volumeRefColor,
            Keyword linkKeyword,
            bool invertLinkedRefDirection,
            PreferedLayerFunctionDelegate<TController> funcPreferedLayer
            ) where TController : PapyrusScript, WorkshopController
        {
            if( ( !createMissing )&&( ignoreExisting ) )
                return; // So, uh...do nothing, der?

            DebugLog.OpenIndentLevel();
            m.PushStatusMessage();
            m.SetCurrentStatusMessage( "ControllerBatch.CalculatingSandboxes".Translate() );
            string msg;
            m.StartSyncTimer();
            var fStart = m.SyncTimerElapsed();

            foreach( var controller in controllers )
            {
                m.PushStatusMessage();
                msg = string.Format( "ControllerBatch.CheckingSandboxFor".Translate(), controller.GetEditorID( target ) );
                m.SetCurrentStatusMessage( msg );

                var borderMarkers = controller.BorderMarkers;
                var sandbox = controller.SandboxVolume;
                if(
                    ( sandbox != null )&&
                    (
                        sandbox.LinkedRefs.GetLinkedRef(
                            TargetHandle.WorkingOrLastFullRequired,
                            GodObject.CoreForms.Fallout4.Keyword.WorkshopLinkedPrimitive.GetFormID( TargetHandle.Master )
                        ) != null
                    )
                )
                {
                    // Make a new sandbox so the build volume[s] are separate volumes
                    sandbox = null;
                    // TODO: Add WorkshopImport before updating the WorkshopScript ObjectReference for the sandbox volume linked ref
                }
                if(
                    ( ( sandbox != null )&&( ignoreExisting ) )||
                    ( ( sandbox == null )&&( !createMissing ) )||
                    ( borderMarkers.NullOrEmpty() )
                )
                {
                    DebugLog.WriteStrings( null, new [] {
                        "--- SKIPPING ---",
                        "controller = " + controller.IDString,
                        "sandbox = " + sandbox.NullSafeIDString(),
                        "borderMarkers = " + ( borderMarkers.NullOrEmpty() ? 0 : borderMarkers.Count ).ToString(),
                        "createMissing = " + createMissing.ToString(),
                        "ignoreExisting = " + ignoreExisting.ToString()
                    }, true, true, false, false, false );
                    m.PopStatusMessage();
                    continue;
                }

                DebugLog.OpenIndentLevel( controller.IDString, false );

                msg = string.Format( "ControllerBatch.CalculatingSandboxFor".Translate(), controller.GetEditorID( target ) );
                m.SetCurrentStatusMessage( msg );
                m.StartSyncTimer();
                var tStart = m.SyncTimerElapsed();

                var hintZ = 0.0f;
                var buildVolumes = controller.BuildVolumes;
                if( !buildVolumes.NullOrEmpty() )
                {
                    foreach( var volume in buildVolumes )
                        hintZ += volume.GetPosition( target ).Z;
                    hintZ /= buildVolumes.Count;
                }
                else if( sandbox != null )
                    hintZ = sandbox.GetPosition( target ).Z;
                else
                    hintZ = controller.Reference.GetPosition( target ).Z;

                // Use border marker reference points instead of build volumes so we can work with less points that are accurate enough
                // also, don't need to calculate any corner/intersection vertexes and the associated problems that go with it.
                var points = new List<Vector2f>();
                foreach( var marker in borderMarkers )
                    points.Add( new Vector2f( marker.GetPosition( target ) ) );

                var hull = Maths.Geometry.ConvexHull.MakeConvexHull( points );

                var osv = VolumeBatch.CalculateOptimalSandboxVolume(
                    target,
                    hull,
                    controller.Reference.Worldspace,
                    scanTerrain,
                    cylinderBottom,
                    cylinderTop,
                    volumePadding,
                    hintZ
                );

                if( osv == null )
                    DebugLog.WriteLine( string.Format( "Unable to calculate sandbox for {0}", controller.IDString ) );
                else
                {
                    DebugLog.WriteStrings( null, new[] {
                        string.Format(
                            "Position = {0} -> {1}",
                            sandbox == null ? "[null]" : sandbox.GetPosition( target ).ToString(),
                            osv.Size.ToString() ),
                        string.Format(
                            "Size = {0} -> {1}",
                            sandbox == null ? "[null]" : sandbox.Primitive.GetBounds( target ).ToString(),
                            osv.Position.ToString() ),
                        string.Format(
                            "Z Rotation = {0} -> {1}",
                            sandbox == null ? "[null]" : sandbox.GetRotation( target ).Z.ToString(),
                            osv.Rotation.Z.ToString() )
                        }, false, true, false, false );

                    var preferedLayer = funcPreferedLayer( ref imports, target, sandbox, controller, out string preferedLayerEditorID );
                    var sandboxEditorID = SetEditorID.FormatEditorID( volumeEditorIDFormat, controller.QualifiedName );
                    
                    var worldspace = controller.Reference.Worldspace;
                    var cell = worldspace == null
                        ? controller.Reference.Cell
                        : ( volumeRefRecordFlags & (uint)Engine.Plugin.Forms.Fields.Record.Flags.Common.Persistent ) != 0
                        ? worldspace.Cells.Persistent
                        : worldspace.Cells.GetByGrid( Engine.SpaceConversions.WorldspaceToCellGrid( osv.Position.X, osv.Position.Y ) );

                    VolumeBatch.CreateVolumeRefImport( ref imports,
                        "Sandbox Volume",
                        Priority.Ref_SandboxVolume,
                        sandbox,
                        sandboxEditorID,
                        volumeRefBase,
                        cell,
                        osv.Position,
                        osv.Rotation,
                        osv.Size,
                        volumeRefColor,
                        controller.Reference,
                        linkKeyword,
                        invertLinkedRefDirection,
                        preferedLayer,
                        preferedLayerEditorID,
                        volumeRefRecordFlags,
                        null );

                }
                var elapsed = m.StopSyncTimer( tStart );
                m.PopStatusMessage();
                DebugLog.CloseIndentLevel( elapsed );
            }

            m.StopSyncTimer( fStart );
            m.PopStatusMessage();
            DebugLog.CloseIndentLevel();
        }

        public static bool NormalizeBuildVolumes(
            ref List<GUIBuilder.FormImport.ImportBase> list,
            TargetHandle target,
            ObjectReference controller,
            string ownerName,
            string layerEditorIDFormat,
            string volumeEditorIDFormat,
            List<Vector2f> hull,
            List<ObjectReference> volumes,
            Worldspace worldspace,
            bool scanTerrain,
            ObjectReference linkRef,
            Keyword linkKeyword,
            Engine.Plugin.Forms.Activator[] volumeBases,
            int preferedVolumeBaseIndex,
            System.Drawing.Color color,
            uint recordFlags,
            float groundSink,
            float topAbovePeak,
            Type volumeScript )
        {
            if( hull.NullOrEmpty() )
            {
                DebugLog.WriteError( "No hull to normalize volumes of" );
                return false;
            }
            if( volumes.NullOrEmpty() )
            {
                DebugLog.WriteError( "No volumes to normalize" );
                return false;
            }
            var wsdp = scanTerrain ? worldspace?.PoolEntry : null;
            if( ( scanTerrain )&&( wsdp == null ) )
            {
                DebugLog.WriteError( "Worldspace data pool could not be resolved" );
                return false;
            }

            if( topAbovePeak < 0.0f )
            {
                DebugLog.WriteError( "topAbovePeak must be greater than or equal to 0.0" );
                return false;
            }
            if( groundSink > 0.0f )
            {
                DebugLog.WriteError( "groundSink must be less than or equal to 0.0" );
                return false;
            }

            // Sort the volumes by FormID
            volumes.Sort( ( x, y ) =>
            {
                var xFID = x.GetFormID( TargetHandle.Master );
                var yFID = y.GetFormID( TargetHandle.Master );
                return
                    ( xFID < yFID ) ? -1 :
                    ( xFID > yFID ) ?  1 :
                    0;
            } );

            var vCount = volumes.Count;
            float minZ, maxZ, avgZ, avgWaterZ;
            float bottomZ, topZ;
            float volH, posZ;

            #region Find the min, max, average and water Z for the volumes

            if( wsdp != null )
            {
                #region Within a worldspace, look at terrain
                
                var volumeCorners = new Vector2f[ vCount ][];
                for( int i = 0; i < vCount; i++ )
                    volumeCorners[ i ] = volumes[ i ].GetCorners( TargetHandle.WorkingOrLastFullRequired );

                var cNW = volumeCorners.GetCornerNWFrom();
                var cSE = volumeCorners.GetCornerSEFrom();

                if( !wsdp.ComputeZHeightsFromVolumes( target, volumeCorners, out minZ, out maxZ, out avgZ, out avgWaterZ, showScanlineProgress: true ) )
                {
                    DebugLog.WriteError( "Could not compute Z coords from worldspace heightmap" );
                    return false;
                }

                bottomZ = minZ + groundSink;
                topZ = maxZ + topAbovePeak;

                #endregion
            }
            else
            {
                #region Sans worldspace, just get the lowest bottom and highest top from the volumes
                
                minZ = float.MaxValue;
                maxZ = float.MinValue;
                avgZ = 0f;
                avgWaterZ = float.MinValue;

                for( int i = 0; i < vCount; i++ )
                {
                    var p = volumes[ i ].GetPosition( TargetHandle.WorkingOrLastFullRequired );
                    var b = volumes[ i ].Primitive.GetBounds( TargetHandle.WorkingOrLastFullRequired );
                    var hZ = b.Z * 0.5f;
                    minZ = Math.Min( minZ, p.Z - hZ );
                    maxZ = Math.Max( maxZ, p.Z + hZ );
                }

                avgZ = minZ + ( maxZ - minZ ) * 0.5f;

                bottomZ = minZ;
                topZ = maxZ;

                #endregion
            }

            #endregion

            volH = topZ - bottomZ;
            posZ = bottomZ + ( volH * 0.5f );

            DebugLog.WriteStrings( null, new[]{
                "minZ = " + minZ.ToString(),
                "maxZ = " + maxZ.ToString(),
                "avgZ = " + avgZ.ToString(),
                "avgWaterZ = " + avgWaterZ.ToString(),
                "bottomZ = " + bottomZ.ToString(),
                "topZ = " + topZ.ToString(),
                "volH = " + volH.ToString(),
                "posZ = " + posZ.ToString() },
                false, true, false, false, true, true );

            #region Find layer for volumes

            var preferedLayer = GetRecommendedLayer(
                ref list,
                volumes,
                controller.GetLayer( TargetHandle.WorkingOrLastFullRequired ),
                layerEditorIDFormat,
                ownerName, -1,
                out string useLayerEditorID
                );

            #endregion

            #region Generate imports for all the build volumes

            for( int i = 0; i < volumes.Count; i++ )
            {
                var volume = volumes[ i ];
                //var oldVolumeEditorID = volume.GetEditorID( TargetHandle.WorkingOrLastFullRequired );
                //var useVolumeEditorID = !string.IsNullOrEmpty( oldVolumeEditorID )
                //    ? oldVolumeEditorID
                //    : volumeEditorIDFormat.FormatEditorID( ownerName, i + 1 );
                var useVolumeEditorID = SetEditorID.FormatEditorID( volumeEditorIDFormat, ownerName, i + 1 );

                var volumeBase = volume.GetName<Engine.Plugin.Forms.Activator>( TargetHandle.WorkingOrLastFullRequired );
                if( !volumeBases.Contains( volumeBase ) )
                    volumeBase = volumeBases[ preferedVolumeBaseIndex ];

                var bounds = new Vector3f( volume.Primitive.GetBounds( TargetHandle.WorkingOrLastFullRequired ) );
                var pos = new Vector3f( volume.GetPosition( TargetHandle.WorkingOrLastFullRequired ) );
                bounds.Z = volH;
                pos.Z = posZ;
                var cell = worldspace == null
                    ? volume.Cell
                    : ( recordFlags & (uint)Engine.Plugin.Forms.Fields.Record.Flags.Common.Persistent ) != 0
                    ? worldspace.Cells.Persistent
                    : worldspace.Cells.GetByGrid( Engine.SpaceConversions.WorldspaceToCellGrid( pos.X, pos.Y ) );
                
                CreateVolumeRefImport( ref list,
                    "Build Volume",
                    Priority.Ref_BuildVolume,
                    volume,
                    useVolumeEditorID,
                    volumeBase,
                    cell,
                    pos,
                    volume.GetRotation( TargetHandle.WorkingOrLastFullRequired ),
                    bounds,
                    color,
                    linkRef,
                    linkKeyword,
                    false,
                    preferedLayer,
                    useLayerEditorID,
                    recordFlags,
                    volumeScript );
                
            }

            #endregion

            return true;
        }

        public static void CreateVolumeRefImport( ref List<GUIBuilder.FormImport.ImportBase> list,
            string importSignature,
            Priority priority,
            ObjectReference volumeRef,
            string volumeRefEditorID,
            Engine.Plugin.Forms.Activator volumeBase,
            Cell cell,
            Maths.Vector3f position,
            Maths.Vector3f rotation,
            Maths.Vector3f primitiveSize,
            System.Drawing.Color primitiveColor,
            ObjectReference linkedRef,
            Keyword linkKeyword,
            bool invertLinkedRefDirection,
            Layer layer,
            string layerEditorID,
            uint recordFlags,
            Type attachScript )
        {

            var impVolume = new ImportBase(
                            importSignature,
                            priority,
                            false,
                            volumeRef,
                            volumeRefEditorID,
                            cell,
                            true );

            impVolume.AddOperation( new Operations.SetRecordFlags( impVolume, recordFlags ) );

            impVolume.AddOperation( new Operations.SetEditorID( impVolume, volumeRefEditorID ) );

            if( volumeBase != null )
                impVolume.AddOperation( new Operations.SetReferenceBaseForm( impVolume, volumeBase ) );

            impVolume.AddOperation( new Operations.SetReferencePosition( impVolume, position ) );
            impVolume.AddOperation( new Operations.SetReferenceRotation( impVolume, rotation ) );

            impVolume.AddOperation( new Operations.SetReferencePrimitiveBounds( impVolume, primitiveSize ) );
            impVolume.AddOperation( new Operations.SetReferencePrimitiveColor( impVolume, primitiveColor ) );
            impVolume.AddOperation( new Operations.SetReferencePrimitiveUnknown( impVolume, 0.3f ) );
            impVolume.AddOperation( new Operations.SetReferencePrimitiveShape( impVolume, Shape.Box ) );

            if( ( !invertLinkedRefDirection )&&( linkedRef != null ) )
            {
                impVolume.AddOperation( new Operations.SetReferenceLinkedRef( impVolume,
                    linkedRef, linkKeyword,
                    false,
                    LinkedRefChanged
                ) );
            }

            if( layer != null )
                impVolume.AddOperation( new Operations.SetReferenceLayer( impVolume, layer ) );
            else if( !string.IsNullOrEmpty( layerEditorID ) )
                impVolume.AddOperation( new Operations.SetReferenceLayer( impVolume, layerEditorID ) );

            impVolume.AddOperation( new Operations.SetReferenceLocationReference( impVolume ) );
            
            if( attachScript != null )
                impVolume.AddOperation( new Operations.AddPapyrusScript( impVolume, attachScript ) );
            
            ImportBase.AddToList(
                ref list,
                impVolume );

            if( (  invertLinkedRefDirection )&&( linkedRef != null ) )
            {
                // Inverted linked ref requires the target to be in the working file, let's do that
                var impWorkshop = new ImportBase(
                    string.Format( "{0} Link Parent", importSignature ),
                    priority + 1,   // Do it after the volume import
                    false,
                    linkedRef,
                    linkedRef.GetEditorID( TargetHandle.WorkingOrLastFullRequired ),
                    cell );
                
                impWorkshop.AddOperation( new Operations.SetReferenceLinkedRef( impWorkshop,
                    impVolume.Target, linkKeyword,
                    false,
                    LinkedRefChanged
                ) );

                ImportBase.AddToList(
                    ref list,
                    impWorkshop );
            }
        }

        static bool LinkedRefChanged( ObjectReference reference, bool linked )
        {   // TODO:  Put this somewhere more appropriate, it will likely be used by other imports
        
            if( reference == null ) return true;
        
            reference.SendObjectDataChangedEvent( reference );
        
            return true;
        }

        public static Layer GetRecommendedLayer(
            ref List<GUIBuilder.FormImport.ImportBase> list,
            List<ObjectReference> volumes,
            Layer preferedLayer,
            string layerEditorIDFormat,
            string ownerName,
            int index,
            out string useLayerEditorID )
        {
            Layer result = preferedLayer;
            useLayerEditorID = null;

            if( !volumes.NullOrEmpty() )
            {
                var vLayers = new List<Layer>();
                var vScores = new List<int>();
                var hScore = (int)0;
                var hIndex = -1;
                foreach( var volume in volumes )
                {
                    var layerFormID = volume.GetLayerFormID( TargetHandle.WorkingOrLastFullRequired );
                    if( Engine.Plugin.Constant.ValidFormID( layerFormID ) )
                    {
                        var layer = GodObject.Plugin.Data.Root.Find<Layer>( layerFormID );
                        if( layer != null )
                        {
                            int i = vLayers.IndexOf( layer );
                            if( i < 0 )
                            {
                                vLayers.AddOnce( layer );
                                vScores.Add( 0 );
                                i = vLayers.Count - 1;
                            }
                            vScores[ i ]++;
                            if( vScores[ i ] > hScore )
                            {
                                hScore = vScores[ i ];
                                hIndex = i;
                            }
                        }
                    }
                }
                if( hIndex >= 0 )
                    result = vLayers[ hIndex ];
            }
            
            if( string.IsNullOrEmpty( useLayerEditorID ) )
            {
                useLayerEditorID = SetEditorID.FormatEditorID( layerEditorIDFormat, ownerName, index );
                result = result ?? GodObject.Plugin.Data.Root.Find<Layer>( useLayerEditorID );
                if(
                    ( result == null )||
                    ( !useLayerEditorID.InsensitiveInvariantMatch( result.GetEditorID( TargetHandle.WorkingOrLastFullRequired ) ) )
                )
                {
                    var import = new FormImport.ImportBase(
                        "Layer",
                        Priority.Form_Layer,
                        false,
                        typeof( Layer ),
                        result,
                        useLayerEditorID );

                    import.AddOperation( new Operations.SetEditorID( import, useLayerEditorID ) );

                    GUIBuilder.FormImport.ImportBase.AddToList(
                        ref list,
                        import );
                }
            }

            return result;
        }

    }

}
