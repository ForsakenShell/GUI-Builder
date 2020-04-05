/*
 * VolumeBatch.cs
 *
 * Batch functions for primitive volumes related to Workshops and Sub-Divisions.
 *
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Maths;


namespace GUIBuilder
{

    public static class VolumeBatch
    {
        
        public static Geometry.ConvexHull.OptimalBoundingBox CalculateOptimalSandboxVolume(
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
                if( wsdp.ComputeZHeightsFromVolumes( volumeCorners, out minZ, out maxZ, out avgZ, out avgWaterZ, showScanlineProgress: true ) )
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
                size.X + volumeMargin,
                size.Y + volumeMargin,
                size.Z + volumeMargin );

            return optVol;
        }

        public static bool NormalizeBuildVolumes(
            ref List<GUIBuilder.FormImport.ImportBase> list,
            string ownerName,
            List<Vector2f> hull,
            List<Engine.Plugin.Forms.ObjectReference> volumes,
            Engine.Plugin.Forms.Worldspace worldspace,
            Engine.Plugin.Forms.ObjectReference linkRef,
            Engine.Plugin.Forms.Keyword linkKeyword,
            Engine.Plugin.Forms.Activator volumeBase,
            System.Drawing.Color color,
            uint recordFlags,
            float groundSink = -1024.0f,
            float topAbovePeak = 5120.0f )
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
            var wsdp = worldspace?.PoolEntry;
            if( ( worldspace != null )&&( wsdp == null ) )
            {
                DebugLog.WriteError( "Worldspace data pool could not be resolved" );
                return false;
            }

            if( topAbovePeak <= 0.0f )
            {
                DebugLog.WriteError( "topAbovePeak must be greater than 0" );
                return false;
            }
            if( groundSink >= 0.0f )
            {
                DebugLog.WriteError( "groundSink must be less than 0" );
                return false;
            }

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

                if( !wsdp.ComputeZHeightsFromVolumes( volumeCorners, out minZ, out maxZ, out avgZ, out avgWaterZ, showScanlineProgress: true ) )
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
                    var b = volumes[ i ].Primitive.GetBounds( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                    minZ = Math.Min( minZ, b.Z );
                    maxZ = Math.Min( maxZ, b.Z );
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

            Engine.Plugin.Forms.Layer preferedLayer = null;
            string useLayerEditorID = null;

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
                {
                    preferedLayer = vLayers[ hIndex ];
                    useLayerEditorID = preferedLayer.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                }
            }

            if( string.IsNullOrEmpty( useLayerEditorID ) )
            {
                // TODO:  Don't hardcode this
                useLayerEditorID = string.Format( "ESM_ATC_LAYR_BV_{0}", ownerName );
                preferedLayer = GodObject.Plugin.Data.Root.Find<Engine.Plugin.Forms.Layer>( useLayerEditorID );
                if( preferedLayer == null )
                {
                    GUIBuilder.FormImport.ImportBase.AddToList(
                        ref list,
                        new GUIBuilder.FormImport.ImportLayer(
                            null,
                            useLayerEditorID ) );
                }
            }

            #endregion

            // Generate imports for all the build volumes
            foreach( var volume in volumes )
            {
                var bounds = new Vector3f( volume.Primitive.GetBounds( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
                var pos = new Vector3f( volume.GetPosition( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
                bounds.Z = volH;
                pos.Z = posZ;
                var cell = worldspace == null
                    ? volume.Cell
                    : worldspace.Cells.Persistent;
                GUIBuilder.FormImport.ImportBase.AddToList(
                    ref list,
                    new GUIBuilder.FormImport.ImportBuildVolumeReference(
                        volume,
                        volume.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ),
                        volumeBase,
                        worldspace, cell,
                        pos,
                        volume.GetRotation( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ),
                        bounds,
                        color,
                        linkRef,
                        linkKeyword,
                        preferedLayer,
                        useLayerEditorID,
                        recordFlags ) );
            }

            return true;
        }

    }

}
