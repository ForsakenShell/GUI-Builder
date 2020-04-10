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

using SetEditorID = GUIBuilder.FormImport.Operations.SetEditorID;
using Operations = GUIBuilder.FormImport.Operations;
using Priority = GUIBuilder.FormImport.Priority;
using Shape = Engine.Plugin.Forms.Fields.ObjectReference.Primitive.PrimitiveType;


namespace GUIBuilder
{

    public static class VolumeBatch
    {
        
        public static Geometry.ConvexHull.OptimalBoundingBox CalculateOptimalSandboxVolume(
            Engine.Plugin.TargetHandle target,
            List<Vector2f> hull,
            Engine.Plugin.Forms.Worldspace worldspace,
            bool skipZScan,
            float fSandboxCylinderBottom, float fSandboxCylinderTop,
            float volumeMargin,
            float sandboxSink,
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

            var volOffset = fSandboxCylinderBottom + sandboxSink;
            var halfHeight = fSandboxCylinderTop > Math.Abs( fSandboxCylinderBottom )
                ? fSandboxCylinderTop
                : Math.Abs( fSandboxCylinderBottom );

            var optVol = Maths.Geometry.ConvexHull.MinBoundingBox( hull );
            optVol.Height = halfHeight * 2.0f;

            var wsdp = skipZScan ? null : worldspace?.PoolEntry;
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
            optVol.Z += volOffset;
            var size = optVol.Size;
            optVol.Size = new Vector3f(
                size.X + volumeMargin,
                size.Y + volumeMargin,
                size.Z + volumeMargin );

            return optVol;
        }

        public static bool NormalizeBuildVolumes(
            ref List<GUIBuilder.FormImport.ImportBase> list,
            Engine.Plugin.TargetHandle target,
            Engine.Plugin.Forms.ObjectReference controller,
            string ownerName,
            string layerEditorIDFormat,
            string volumeEditorIDFormat,
            List<Vector2f> hull,
            List<Engine.Plugin.Forms.ObjectReference> volumes,
            Engine.Plugin.Forms.Worldspace worldspace,
            bool skipZScan,
            Engine.Plugin.Forms.ObjectReference linkRef,
            Engine.Plugin.Forms.Keyword linkKeyword,
            Engine.Plugin.Forms.Activator[] volumeBases,
            int preferedVolumeBase,
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
            var wsdp = skipZScan ? null : worldspace?.PoolEntry;
            if( ( !skipZScan ) &&( wsdp == null ) )
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
                var xFID = x.GetFormID( Engine.Plugin.TargetHandle.Master );
                var yFID = y.GetFormID( Engine.Plugin.TargetHandle.Master );
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
                    volumeCorners[ i ] = volumes[ i ].GetCorners( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );

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
                    var p = volumes[ i ].GetPosition( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                    var b = volumes[ i ].Primitive.GetBounds( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
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
                controller.GetLayer( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ),
                layerEditorIDFormat,
                ownerName, -1,
                out string useLayerEditorID
                );

            #endregion

            #region Generate imports for all the build volumes

            for( int i = 0; i < volumes.Count; i++ )
            {
                var volume = volumes[ i ];
                //var oldVolumeEditorID = volume.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                //var useVolumeEditorID = !string.IsNullOrEmpty( oldVolumeEditorID )
                //    ? oldVolumeEditorID
                //    : volumeEditorIDFormat.FormatEditorID( ownerName, i + 1 );
                var useVolumeEditorID = SetEditorID.FormatEditorID( volumeEditorIDFormat, ownerName, i + 1 );

                var volumeBase = volume.GetName<Engine.Plugin.Forms.Activator>( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                if( !volumeBases.Contains( volumeBase ) )
                    volumeBase = volumeBases[ preferedVolumeBase ];

                var bounds = new Vector3f( volume.Primitive.GetBounds( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
                var pos = new Vector3f( volume.GetPosition( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
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
                    volumeBase.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ),
                    cell,
                    pos,
                    volume.GetRotation( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ),
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
            Engine.Plugin.Forms.ObjectReference volumeRef,
            string volumeRefEditorID,
            Engine.Plugin.Forms.Activator volumeBase,
            string volumeBaseEditorID,
            Engine.Plugin.Forms.Cell cell,
            Maths.Vector3f position,
            Maths.Vector3f rotation,
            Maths.Vector3f primitiveSize,
            System.Drawing.Color primitiveColor,
            Engine.Plugin.Forms.ObjectReference linkedRef,
            Engine.Plugin.Forms.Keyword linkKeyword,
            bool invertLinkedRefDirection,
            Engine.Plugin.Forms.Layer layer,
            string layerEditorID,
            uint recordFlags,
            Type attachScript )
        {

            var import = new GUIBuilder.FormImport.ImportBase(
                            importSignature,
                            priority,
                            false,
                            volumeRef,
                            volumeRefEditorID,
                            cell );

            import.AddOperation( new Operations.SetRecordFlags( import, recordFlags ) );

            import.AddOperation( new Operations.SetEditorID( import, volumeRefEditorID ) );

            if( volumeBase != null )
                import.AddOperation( new Operations.SetReferenceBaseForm( import, volumeBase ) );
            else if( !string.IsNullOrEmpty( volumeBaseEditorID ) )
                import.AddOperation( new Operations.SetReferenceBaseForm( import, volumeBaseEditorID ) );

            import.AddOperation( new Operations.SetReferencePosition( import, position ) );
            import.AddOperation( new Operations.SetReferenceRotation( import, rotation ) );

            import.AddOperation( new Operations.SetReferencePrimitiveBounds( import, primitiveSize ) );
            import.AddOperation( new Operations.SetReferencePrimitiveColor( import, primitiveColor ) );
            import.AddOperation( new Operations.SetReferencePrimitiveUnknown( import, 0.3f ) );
            import.AddOperation( new Operations.SetReferencePrimitiveShape( import, Shape.Box ) );

            if( ( linkedRef != null )&&( linkKeyword != null ) )
            {
                import.AddOperation( new Operations.SetReferenceLinkedRef( import,
                    linkedRef, linkKeyword,
                    invertLinkedRefDirection,
                    LinkedRefChanged
                ) );
            }

            if( layer != null )
                import.AddOperation( new Operations.SetReferenceLayer( import, layer ) );
            else if( !string.IsNullOrEmpty( layerEditorID ) )
                import.AddOperation( new Operations.SetReferenceLayer( import, layerEditorID ) );

            import.AddOperation( new Operations.SetReferenceLocationReference( import ) );
            
            if( attachScript != null )
                import.AddOperation( new Operations.AddPapyrusScript( import, attachScript ) );
            
            GUIBuilder.FormImport.ImportBase.AddToList(
                ref list,
                import );
        }

        static bool LinkedRefChanged( Engine.Plugin.Forms.ObjectReference reference, bool linked )
        {   // TODO:  Put this somewhere more appropriate, it will likely be used by other imports
        
            if( reference == null ) return true;
        
            reference.SendObjectDataChangedEvent( reference );
        
            return true;
        }

        public static Engine.Plugin.Forms.Layer GetRecommendedLayer(
            ref List<GUIBuilder.FormImport.ImportBase> list,
            List<Engine.Plugin.Forms.ObjectReference> volumes,
            Engine.Plugin.Forms.Layer preferedLayer,
            string layerEditorIDFormat,
            string ownerName,
            int index,
            out string useLayerEditorID )
        {
            Engine.Plugin.Forms.Layer result = preferedLayer;
            useLayerEditorID = null;

            if( !volumes.NullOrEmpty() )
            {
                var vLayers = new List<Engine.Plugin.Forms.Layer>();
                var vScores = new List<int>();
                var hScore = (int)0;
                var hIndex = -1;
                foreach( var volume in volumes )
                {
                    var layerFormID = volume.GetLayerFormID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                    if( Engine.Plugin.Constant.ValidFormID( layerFormID ) )
                    {
                        var layer = GodObject.Plugin.Data.Root.Find<Engine.Plugin.Forms.Layer>( layerFormID );
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
                result = result ?? GodObject.Plugin.Data.Root.Find<Engine.Plugin.Forms.Layer>( useLayerEditorID );
                if(
                    ( result == null )||
                    ( !useLayerEditorID.InsensitiveInvariantMatch( result.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) ) )
                )
                {
                    var import = new FormImport.ImportBase(
                        "Layer",
                        Priority.Form_Layer,
                        false,
                        typeof( Engine.Plugin.Forms.Layer ),
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
