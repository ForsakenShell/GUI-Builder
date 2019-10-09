/*
 * BorderNode.cs
 *
 * A border node and group of nodes.
 *
 */
using System;
using System.Collections.Generic;

using Maths;
using AnnexTheCommonwealth;
using Engine;

namespace GUIBuilder
{
    
    public class BorderNodeGroup
    {
        public int                  BorderIndex;
        public int                  NIFIndex;
        public Vector2i             Cell;
        public Vector3f             Placement;
        
        public string               NIFFilePath;
        
        public Vector3i             MinBounds{ get{ return BorderNode.MinBounds( Nodes ); } }
        public Vector3i             MaxBounds{ get{ return BorderNode.MaxBounds( Nodes ); } }
        
        public List<BorderNode>     Nodes;
        
        public NIFBuilder.Mesh      Mesh;
        
        public BorderNodeGroup(
            Vector2i cell,
            List<BorderNode> nodes,
            string meshSuffix,
            string meshSubPath,
            string filePrefix,
            string location,
            string fileSuffix,
            int borderIndex = -1,
            string neighbour = "",
            int subIndex = -1,
            string forcedNIFPath = "",
            string forcedNIFFile = "" )
        {
            BorderIndex             = borderIndex;
            NIFIndex                = subIndex;
            Cell                    = new Vector2i( cell );
            Nodes                   = nodes;
            
            if( ( !string.IsNullOrEmpty( forcedNIFPath ) )||( !string.IsNullOrEmpty( forcedNIFFile ) ) )
            {
                //targetSuffix
                if( ( !string.IsNullOrEmpty( forcedNIFPath ) )&&( forcedNIFPath[ forcedNIFPath.Length - 1 ] != '\\' ) )
                    forcedNIFPath += @"\";
                NIFFilePath = string.Format( @"Meshes\{0}{1}", forcedNIFPath, forcedNIFFile );
            }
            else
            {
                NIFFilePath         = NIFBuilder.Mesh.BuildFilePath(
                    meshSuffix, meshSubPath,
                    filePrefix,
                    location,
                    fileSuffix,
                    borderIndex,
                    neighbour, subIndex );
            }
            //DebugLog.Write( string.Format( "GUIBuilder.BorderNodeGroup.cTor() :: {0} :: {1} :: {2}", cell.ToString(), nodes.Count, NIFFilePath ) );
        }
        
        public void CentreAndPlaceNodes()
        {
            //DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "CentreAndPlaceNodes()" } );
            //DumpGroupNodes( Nodes, "Pre-process nodes" );
            Placement = BorderNode.Centre( Nodes, true );
            BorderNode.CentreNodes( Nodes, Placement );
            // Cell may be set incorrectly while generating the nodes, recalculate it from the final mesh position
            Cell = Placement.WorldspaceToCellGrid();
            //DumpGroupNodes( Nodes, "Post-process nodes" );
            //DebugLog.CloseIndentLevel();
        }
        
        public static BorderNodeGroup FindGroupFromCell( List<BorderNodeGroup> nodeGroups, Vector2i cell )
        {
            foreach( var nodeGroup in nodeGroups )
                if( nodeGroup.Cell == cell ) return nodeGroup;
            return null;
        }
        
        public bool BuildMesh( float gradientHeight, float groundOffset, float groundSink, uint[] insideColours, uint[] outsideColours )
        {
            //DebugLog.Write( string.Format( "\n{0} :: Build() :: Start :: {1}:{2}:{3}", "GUIBuilder.NIFBuilder", gradientHeight, groundOffset, groundSink ) );
            Mesh = new NIFBuilder.Mesh( this, gradientHeight, groundOffset, groundSink, insideColours, outsideColours );
            //DebugLog.Write( string.Format( "{0} :: Build() :: Complete :: {1}:{2}:{3}\n", "GUIBuilder.NIFBuilder", gradientHeight, groundOffset, groundSink ) );
            return Mesh != null;
        }
        
        public static List<BorderNodeGroup> SplitAcrossCells(
            List<BorderNode> clonedNodeList,
            string meshSuffix,
            string meshSubPath,
            string filePrefix,
            string location,
            string fileSuffix,
            int borderIndex,
            string neighbour,
            int initialSubIndex
            )
        {
            //DebugLog.OpenIndentLevel( new [] { "GUIBuilder.BorderNodeGroup", "SplitAcrossCells()",  "Start", clonedNodeList.Count.ToString(), location, neighbour } );
            //DumpGroupNodes( clonedNodeList, "Input nodes:" );
            
            var nodeGroups = new List<BorderNodeGroup>();
            int nifIndex = initialSubIndex;
            int currentIndex = 0;
            while( true )
            {
                bool splitNodes = false;
                var groupNodes = new List<BorderNode>();
                var currentCell = clonedNodeList[ currentIndex ].CellGrid;
                //DebugLog.Write( string.Format( "\nGUIBuilder.BorderNodeGroup.SplitAcrossCells() :: cell {2} :: Scan from {0} {1}", currentIndex, clonedNodeList[ currentIndex ].P.ToString(), currentCell.ToString() ) );
                int splitIndex = CellBoundarySplitIndex( currentCell, clonedNodeList, currentIndex );
                if( splitIndex > currentIndex )
                {
                    //DebugLog.Write( string.Format( "GUIBuilder.BorderNodeGroup.SplitAcrossCells() :: cell {2} :: Split {0} to {1}", currentIndex, splitIndex, currentCell.ToString() ) );
                    
                    // Add all nodes from currentIndex to splitIndex - 1
                    for( int i = currentIndex; i < splitIndex; i++ )
                        groupNodes.Add( clonedNodeList[ i ] );
                    
                    // Compute new node from splitIndex - 1 and splitIndex for the cell boundary
                    var injectStartNode = GetCellEdgeIntersectNode( currentCell, clonedNodeList[ splitIndex - 1 ], clonedNodeList[ splitIndex ] );
                    
                    // Clone the new start node as an end node in the current cell
                    var injectEndNode = new BorderNode( currentCell, injectStartNode.P, injectStartNode.Floor, BorderNode.NodeType.EndPoint );
                    
                    // Inject the new nodes into the node list at the cell boundary
                    clonedNodeList.Insert( splitIndex    , injectEndNode   );
                    clonedNodeList.Insert( splitIndex + 1, injectStartNode );
                    
                    // First node in the group is now a start node
                    var n0 = groupNodes[ 0 ];
                    n0.Type = BorderNode.NodeType.StartPoint;
                    groupNodes.Add( injectEndNode );   // Add end node to the node group
                    
                    splitIndex++;   // Step past the injected end node to the injected start node
                    splitNodes = true;
                }
                else
                {
                    splitIndex = clonedNodeList.Count - 1;
                    var j = splitIndex;
                    //DebugLog.Write( string.Format( "GUIBuilder.BorderNodeGroup.SplitAcrossCells() :: cell {2} :: Tail {0} to {1}", currentIndex, j, currentCell.ToString() ) );
                    
                    // Add all nodes from currentIndex to node count - 1
                    for( int i = currentIndex; i <= splitIndex; i++ )
                        groupNodes.Add( clonedNodeList[ i ] );
                    j = groupNodes.Count - 1;
                    groupNodes[ 0 ].Type = BorderNode.NodeType.StartPoint;
                    groupNodes[ j ].Type = BorderNode.NodeType.EndPoint;
                }
                
                //DebugLog.Write( string.Format( "GUIBuilder.BorderNodeGroup.SplitAcrossCells() :: cell {2} :: nodes {3} to {4} :: {0} to {1}", groupNodes[ 0 ].P.ToString(), groupNodes[ groupNodes.Count - 1 ].P.ToString(), currentCell.ToString(), currentIndex, splitIndex ) );
                
                var existingGroup = FindGroupFromCell( nodeGroups, currentCell );
                if( existingGroup == null )
                {
                    // Add as a new group
                    var newGroup = new BorderNodeGroup(
                        currentCell, groupNodes,
                        meshSuffix, meshSubPath,
                        filePrefix, location,
                        fileSuffix, borderIndex,
                        neighbour, nifIndex );
                    nodeGroups.Add( newGroup );
                    nifIndex++;
                    //DebugLog.Write( string.Format( "GUIBuilder.BorderNodeGroup.SplitAcrossCells() :: cell {1} :: Added as new group :: NIFIndex = {0}", newGroup.NIFIndex, currentCell.ToString() ) );
                }
                else
                {   // Add to the existing group
                    var j = groupNodes.Count - 1;
                    var startNodeMatch = existingGroup.FindNodeAt( groupNodes[ j ].P, BorderNode.NodeType.StartPoint );
                    if( startNodeMatch >= 0 )
                    {
                        // Last node of the new group matches a starting node in the existing group
                        existingGroup.Nodes[ startNodeMatch ].Type = BorderNode.NodeType.MidPoint;
                        for( int i = j - 1; i >= 0; i-- )
                            existingGroup.Nodes.Insert( startNodeMatch, groupNodes[ i ] );
                    }
                    else
                    {
                        // Add the nodes as their own set to the mesh group
                        for( int i = 0; i <= j; i++ )
                            existingGroup.Nodes.Add( groupNodes[ i ] );
                    }
                    //DebugLog.Write( string.Format( "GUIBuilder.BorderNodeGroup.SplitAcrossCells() :: cell {1} :: Merged with existing group :: NIFIndex = {0}", existingGroup.NIFIndex, currentCell.ToString() ) );
                }
                
                //DebugLog.Write();
                
                if( !splitNodes )
                    break;
                
                // splitIndex is now a cell boundary so the next search should be from there
                currentIndex = splitIndex;
            }
            //DebugLog.CloseIndentLevel( "nodeGroups", nodeGroups );
            return nodeGroups;
        }
        
        #region Try to find the best form from keywords in the EditorID
        
        static void DumpOriginalFormsAndKeys( string callerName, List<Engine.Plugin.Form> originalForms, List<string> keys )
        {
            return;
            DebugLog.WriteLine( string.Format( "\nGUIBuilder.BorderNodeGroup :: {0}", callerName ) );
            if( !originalForms.NullOrEmpty() )
            {
                DebugLog.WriteLine( string.Format( "\toriginalForms: {0}", originalForms.Count ) );
                for( int i = 0; i < originalForms.Count; i++  )
                {
                    var form = originalForms[ i ];
                    DebugLog.WriteLine( string.Format( "\t\t[ {0} ] = \"{1}\" - 0x{2} - \"{3}\"", i, form.Signature, form.GetFormID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ).ToString( "X8" ), form.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) ) );
                }
            }
            if( !keys.NullOrEmpty() )
            {
                DebugLog.WriteLine( string.Format( "\tkeys: {0}", keys.Count ) );
                for( int i = 0; i < keys.Count; i++  )
                {
                    var key = keys[ i ];
                    DebugLog.WriteLine( string.Format( "\t\t[ {0} ] = \"{1}\"", i, key ) );
                }
            }
        }
        
        void DumpBestMatchFound( Engine.Plugin.Form match, int bestMatchIndex, int bestMatchCount )
        {
            return;
            DebugLog.WriteLine( string.Format(
                "\tmatch result:\n\t\tindex = {0}\n\t\tform = \"{1}\" - 0x{2} - \"{3}\"\n\t\tscore = {4}",
                bestMatchIndex,
                ( match == null ? null : match.Signature ),
                ( match == null ? Engine.Plugin.Constant.FormID_Invalid : match.GetFormID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) ).ToString( "X8" ),
                ( match == null ? "unresolved" : match.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) ),
                bestMatchCount ) );
        }
        
        // NIFFile in Mesh and original Form EditorID need to contain all the keys in the set to match
        public Engine.Plugin.Forms.Static BestStaticFormFromOriginalsFor( List<Engine.Plugin.Form> originalForms, List<string> keys, bool removeFoundItemFromList )
        {
            //DumpOriginalFormsAndKeys( "BestStaticFormFromOriginalsFor()", originalForms, keys );
            if( ( originalForms.NullOrEmpty() )||( keys.NullOrEmpty() ) ) return null;
            var bestMatchIndex = -1;
            var bestMatchCount = -1;
            var ofCount = originalForms.Count;
            for( int i = 0; i < ofCount; i++ )
            {
                var form = originalForms[ i ];
                if( form.GetType() != typeof( Engine.Plugin.Forms.Static ) ) continue;
                var formEDID = form.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                if( string.IsNullOrEmpty( formEDID ) ) continue;
                var j = Mesh.nifFile.CountCommonMatchesBetweenStrings( formEDID, keys, StringComparison.InvariantCultureIgnoreCase );
                if( j > bestMatchCount )
                {
                    bestMatchCount = j;
                    bestMatchIndex = i;
                }
            }
            var match = bestMatchIndex < 0
                ? null
                : originalForms[ bestMatchIndex ] as Engine.Plugin.Forms.Static;
            //DumpBestMatchFound( match, bestMatchIndex, bestMatchCount );
            if( ( removeFoundItemFromList )&&( match != null ) )
                originalForms.Remove( match );
            return match;
        }
        
        public Engine.Plugin.Forms.ObjectReference BestObjectReferenceFromOriginalsFor( List<Engine.Plugin.Form> originalForms, uint originalStaticFormID, bool removeFoundItemFromList )
        {
            //DumpOriginalFormsAndKeys( "BestObjectReferenceFromOriginalsFor()", originalForms, null );
            if( originalForms.NullOrEmpty() ) return null;
            // disable once InvokeAsExtensionMethod
            if( !Engine.Plugin.Constant.ValidFormID( originalStaticFormID ) ) return null;
            var bestMatchIndex = -1;
            var ofCount = originalForms.Count;
            var match = (Engine.Plugin.Forms.ObjectReference)null;
            for( int i = 0; i < ofCount; i++ )
            {
                var refr = originalForms[ i ] as Engine.Plugin.Forms.ObjectReference;
                if( refr == null  ) continue;
                if( originalStaticFormID == refr.GetName( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) )
                {
                    bestMatchIndex = i;
                    match = refr;
                    break;
                }
            }
            //DumpBestMatchFound( match, bestMatchIndex, 1 );
            if( ( removeFoundItemFromList )&&( match != null ) )
                originalForms.Remove( match );
            return match;
        }
        
        #endregion
        
        public static void DumpGroupNodes( List<BorderNode> nodes, string s )
        {
            //DebugLog.OpenIndentLevel( s );
            for( int i = 0; i < nodes.Count; i++ )
                DebugLog.WriteLine( "node[ " + i.ToString() + " ] = " + nodes[ i ].ToString() );
            //DebugLog.CloseIndentLevel();
        }
        
        int FindNodeAt( Vector3f p, BorderNode.NodeType type )
        {
            for( int i = 0; i < Nodes.Count; i++ )
                if( ( Nodes[ i ].Type == type )&&( p == Nodes[ i ].P ) )return i;
            return -1;
        }
        
        static int CellBoundarySplitIndex( Vector2i referenceCell, List<BorderNode> nodes, int startIndex )
        {
            var nCount = nodes.Count;
            if( ( startIndex < 0 )||( startIndex > nCount - 2 ) ) return -1;
            
            for( int i = startIndex; i < nCount; i++ )
            {
                var cNode = nodes[ i ];
                var t = 
                    ( cNode.CellGrid != referenceCell )&&               // Different cells, and;
                    ( !OnCellBoundry( cNode.P, referenceCell ) );       // Current is not on reference cell edge, or;
                    //( !OnCellBoundry( pNode.P, cNode.CellGrid ) );      // Previous is not on Current cell edge
                if( t )
                {
                    //DebugLog.Write( string.Format( "GUIBuilder.BorderNodeGroup :: CellBoundarySplitIndex() :: {0} :: {1} = {2} {3}", referenceCell.ToString(), i, cNode.P.ToString(), cNode.CellGrid.ToString() ) );
                    return i;                                           // Current should be split off
                }
            }
            
            return -1;
        }
        
        static bool OnCellBoundry( float x, float y, Vector2i cell )
        {
            var x0 =   cell.X       * Engine.Constant.WorldMap_Resolution;
            var x1 = ( cell.X + 1 ) * Engine.Constant.WorldMap_Resolution;
            var y0 =   cell.Y       * Engine.Constant.WorldMap_Resolution;
            var y1 = ( cell.Y + 1 ) * Engine.Constant.WorldMap_Resolution;
            
            //if( ( x < x0 )||( x > x1 )||( y < y0 )||( y > y1 ) ) return false;
            
            return
                x.ApproximatelyEquals( x0 ) ||
                x.ApproximatelyEquals( x1 ) ||
                y.ApproximatelyEquals( y0 ) ||
                y.ApproximatelyEquals( y1 );
        }
        
        static bool OnCellBoundry( Vector3f pos, Vector2i cell )
        {
            return OnCellBoundry( pos.X, pos.Y, cell );
        }
        
        static bool OnCellBoundry( Vector2f pos, Vector2i cell )
        {
            return OnCellBoundry( pos.X, pos.Y, cell );
        }
        
        static BorderNode GetCellEdgeIntersectNode( Vector2i referenceCell, BorderNode start, BorderNode end )
        {
            /*
            DebugLog.Write( string.Format(
                "GUIBuilder.BorderNodeGroup :: GetCellEdgeIntersectNode() :: Start :: {0} :: {1} {2} :: {3} {4}",
                referenceCell.ToString(),
                start.P.ToString(), start.CellGrid.ToString(),
                end.P.ToString(), end.CellGrid.ToString() ) );
            */
            
            // Vertical plane
            var cellV = new Vector2i( referenceCell );
            var intersectV = new Vector2f( float.MinValue, float.MinValue );
            var lengthV = -1.0f;
            
            // Horizontal plane
            var cellH = new Vector2i( referenceCell );
            var intersectH = new Vector2f( float.MinValue, float.MinValue );
            var lengthH = -1.0f;
            
            // start -> end overall
            var pos1 = new Vector2f( start.P.X, start.P.Y );
            var pos2 = new Vector2f( end.P.X, end.P.Y );
            var lengthO = ( pos2 - pos1 ).Length;
            
            // Test planes
            int testPlanes = 0;
            
            if( end.CellGrid.X < referenceCell.X ){ testPlanes |= 1;            }
            if( end.CellGrid.X > referenceCell.X ){ testPlanes |= 1; cellV.X++; }
            if( end.CellGrid.Y < referenceCell.Y ){ testPlanes |= 2;            }
            if( end.CellGrid.Y > referenceCell.Y ){ testPlanes |= 2; cellH.Y++; }
            
            // Intersect length
            var cellI = Vector2i.Zero;
            var lengthI = -1.0f;
            
            if( ( testPlanes & 1 ) != 0 )
            {
                var top     = new Vector2f( cellV.X * Engine.Constant.WorldMap_Resolution, ( 1 + ( cellV.Y > end.CellGrid.Y ? cellV.Y : end.CellGrid.Y ) ) * Engine.Constant.WorldMap_Resolution );
                var bottom  = new Vector2f( cellV.X * Engine.Constant.WorldMap_Resolution, (     ( cellV.Y < end.CellGrid.Y ? cellV.Y : end.CellGrid.Y ) ) * Engine.Constant.WorldMap_Resolution );
                
                if( end.CellGrid.X < referenceCell.X ) cellV.X--;
                
                if( pos1.X.ApproximatelyEquals( top.X ) )           // Point 1 is on the boundary itself - Should this ever happen?  Does this make sense?
                    intersectV = new Vector2f( pos1 );
                else if( pos2.X.ApproximatelyEquals( top.X ) )      // Point 2 is on the boundary itself
                    intersectV = new Vector2f( pos2 );
                else                                                // Compute intersection with cell boundary
                    Geometry.Collision.LineLineIntersect( bottom, top, pos1, pos2, out intersectV );
                
                // Length of vertical plane intersect
                lengthV = ( intersectV - pos1 ).Length;
                
                //DebugLog.Write( string.Format( "GUIBuilder.BorderNodeGroup :: GetCellEdgeIntersectNode() :: Vertical Test {0} - {1} :: {2} :: {3} :: {4}", top.ToString(), bottom.ToString(), intersectV.ToString(), cellV.ToString(), lengthV ) );
                
            }
            
            if( ( testPlanes & 2 ) != 0 )
            {
                var left    = new Vector2f( (     ( cellH.X < end.CellGrid.X ? cellH.X : end.CellGrid.X ) ) * Engine.Constant.WorldMap_Resolution, cellH.Y * Engine.Constant.WorldMap_Resolution );
                var right   = new Vector2f( ( 1 + ( cellH.X > end.CellGrid.X ? cellH.X : end.CellGrid.X ) ) * Engine.Constant.WorldMap_Resolution, cellH.Y * Engine.Constant.WorldMap_Resolution );
                
                if( end.CellGrid.Y < referenceCell.Y ) cellH.Y--;
                
                if( pos1.Y.ApproximatelyEquals( left.Y ) )          // Point 1 is on the boundary itself - Should this ever happen?  Does this make sense?
                    intersectH = new Vector2f( pos1 );
                else if( pos2.Y.ApproximatelyEquals( left.Y ) )     // Point 2 is on the boundary itself
                    intersectH = new Vector2f( pos2 );
                else                                                // Compute intersection with cell boundary
                    Geometry.Collision.LineLineIntersect( left, right, pos1, pos2, out intersectH );
                
                // Length of horizontal plane intersect
                lengthH = ( intersectH - pos1 ).Length;
                
                //DebugLog.Write( string.Format( "GUIBuilder.BorderNodeGroup :: GetCellEdgeIntersectNode() :: Horizontal Test {0} - {1} :: {2} :: {3} :: {4}", left.ToString(), right.ToString(), intersectH.ToString(), cellH.ToString(), lengthH ) );
            }
            
            // Pick closest plane intersect
            var intersectPos = Vector3f.Zero;
            
            if( ( lengthV >= 0.0f )&&( ( lengthH < 0.0f )||( lengthV < lengthH ) ) )
            {
                //DebugLog.Write( "GUIBuilder.BorderNode :: GetCellEdgeIntersectNode() :: Vertical intersect picked" );
                lengthI = lengthV / lengthO;
                intersectPos = new Vector3f( intersectV );
                cellI = cellV;
            }
            else if( ( lengthH >= 0.0f )&&( ( lengthV < 0.0f )||( lengthH < lengthV ) ) )
            {
                //DebugLog.Write( "GUIBuilder.BorderNode :: GetCellEdgeIntersectNode() :: Horizontal intersect picked" );
                lengthI = lengthH / lengthO;
                intersectPos = new Vector3f( intersectH );
                cellI = cellH;
            }
            
            // Compute result vector Z and node floor
            intersectPos.Z = start.P.Z + ( ( end.P.Z - start.P.Z ) * lengthI );
            var intersectFloor = start.Floor + ( ( end.Floor - start.Floor ) * lengthI );
            
            /*
            DebugLog.Write( string.Format(
                "GUIBuilder.BorderNodeGroup :: GetCellEdgeIntersectNode() :: Complete :: {0} -> {1} :: {2} {3} :: {4} {5}",
                referenceCell.ToString(), cellI.ToString(),
                start.P.ToString(), start.CellGrid.ToString(),
                end.P.ToString(), end.CellGrid.ToString() ) );
            */
            return new BorderNode( cellI, intersectPos, intersectFloor, BorderNode.NodeType.EndPoint );
        }
        
        public override string ToString()
        {
            var c = Nodes.Count;
            var result = string.Format( "Nodes: Contains {0} elements{1}{2}", c, DebugLog.FormatNewLineChar, DebugLog.OpenIndentChar );
            for( int i = 0; i < c; i++ )
                result += string.Format( "{0}[ {1} ] = {2}", DebugLog.FormatNewLineChar, i, Nodes[ i ].ToString() );
            result += string.Format( "{0}{1}", DebugLog.FormatNewLineChar, DebugLog.CloseIndentChar );
            return result;
        }
        
    }
    
    /// <summary>
    /// Description of BorderNode.
    /// </summary>
    public class BorderNode
    {
        
        public const float MIN_NODE_LENGTH = 4.0f;
        
        public const float MIN_SLOPE_ALLOWANCE = 0.0001f;
        
        public enum NodeType
        {
            MidPoint = 0,
            StartPoint = 1,
            EndPoint = 2
        }
        
        // Z component of P is the "surface" (either ground or water height, which ever is higher)
        public Vector3f             P;
        // Floor is the lowest point the mesh must reach to (always ground height)
        public float                Floor;
        
        public NodeType             Type;
        
        public int                  verts;
        
        public int[]                iVerts;
        public int[]                oVerts;
        
        public bool                 FloorVertexMatchesGroundVertex( float gOffset, float gSink )
        { return ( Floor + gSink ).ApproximatelyEquals( P.Z + gOffset ); }
        
        public Vector2i             CellGrid;
        
        public Vector2f             P2
        { get { return new Vector2f( P.X, P.Y ); } }
        
        public BorderNode( Vector2i cellGrid, Vector3f p, float floor, NodeType type )
        {
            CellGrid                = new Vector2i( cellGrid );
            P                       = new Vector3f( p );
            Floor                   = floor;
            Type                    = type;
        }
        
        public BorderNode Clone()
        {
            return new BorderNode( CellGrid, P, Floor, Type );
        }
        
        public override string ToString()
        {
            return string.Format(
                "P = {0} :: Floor = {1} :: CellGrid = {2} :: Type = {3}",
                P.ToString(),
                Floor,
                CellGrid.ToString(),
                Type.ToString() );
        }
        
        public static List<BorderNode> GenerateBorderNodes( Engine.Plugin.Forms.Worldspace worldspace, List<EdgeFlag> flags, float approximateNodeLength, float slopeAllowance, Engine.Plugin.Forms.Static forcedZ )
        {
            if( ( worldspace == null )||( flags.NullOrEmpty() ) )
                return null;
            var refPoints = new List<Vector3f>();
            foreach( var flag in flags )
            {
                var p = new Vector3f( flag.Reference.GetPosition( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
                if( ( forcedZ == null )||( forcedZ.GetFormID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) != flag.Reference.GetName( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) ) )
                    p.Z = float.MinValue;
                
                refPoints.Add( p );
            }
            return GenerateBorderNodes( worldspace.PoolEntry, refPoints, approximateNodeLength, slopeAllowance );
        }
        
        public static List<BorderNode> GenerateBorderNodes( Engine.Plugin.Forms.Worldspace worldspace, List<Engine.Plugin.Forms.ObjectReference> references, float approximateNodeLength, float slopeAllowance, Engine.Plugin.Forms.Static forcedZ )
        {
            if( ( worldspace == null )||( references.NullOrEmpty() ) )
                return null;
            var refPoints = new List<Vector3f>();
            foreach( var reference in references )
            {
                var p = new Vector3f( reference.GetPosition( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
                if( ( forcedZ == null )||( forcedZ.GetFormID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) != reference.GetName( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) ) )
                    p.Z = float.MinValue;
                
                refPoints.Add( p );
            }
            return GenerateBorderNodes( worldspace.PoolEntry, refPoints, approximateNodeLength, slopeAllowance );
        }
        
        static void DumpReferencePoints( IList<Vector3f> refPoints, string s )
        {
            DebugLog.OpenIndentLevel( new [] { "GUIBuilder.BorderNode", "DumpReferencePoints()", refPoints.Count.ToString(), s } );
            for( int i = 0; i < refPoints.Count; i++ )
                DebugLog.WriteLine( string.Format(
                    "\trefPoint[ {0} ] = ( {1}, {2} )", i,
                    refPoints[ i ].WorldspaceToCellGrid().ToString(),
                    refPoints[ i ].ToString()
                    ) );
            DebugLog.CloseIndentLevel();
        }
        
        static List<BorderNode> GenerateBorderNodes( GodObject.WorldspaceDataPool.PoolEntry wpEntry, IList<Vector3f> refPoints, float nodeLength, float slopeAllowance )
        {
            //DebugLog.OpenIndentLevel( "GUIBuilder.BorderNode :: GenerateBorderNodes()" );
            List<BorderNode> nodes = null;
            
            if( refPoints.NullOrEmpty() )
            {
                //DebugLog.WriteLine( "refPoints is NULL or EMPTY!" );
                goto localAbort;
            }
            
            if( wpEntry == null )
            {
                //DebugLog.WriteLine( "wpEntry is NULL!" );
                goto localAbort;
            }
            
            if( !wpEntry.LoadHeightMapData() )
            {
                //DebugLog.WriteLine( "LoadHeightMapData() returned false" );
                goto localAbort;
            }
            
            nodeLength     = nodeLength     < BorderNode.MIN_NODE_LENGTH     ? BorderNode.MIN_NODE_LENGTH     : nodeLength;
            slopeAllowance = slopeAllowance < BorderNode.MIN_SLOPE_ALLOWANCE ? BorderNode.MIN_SLOPE_ALLOWANCE : slopeAllowance;
            
            //DumpReferencePoints( refPoints, string.Format( "nodeLength = {0} :: slopeAllowance = {1}", nodeLength, slopeAllowance ) );
            
            var lowestFloor = float.MaxValue;
            var rCount = refPoints.Count - 1;
            nodes = new List<BorderNode>();
            for( int i = 0; i < rCount; i++ )
            {
                // Reference points
                var rp0 = refPoints[ i     ];
                var rp1 = refPoints[ i + 1 ];
                
                // Does this segment have forced Z?
                var forcedZRef = ( rp0.Z > float.MinValue );
                var forcedZStride = ( forcedZRef )&&( rp1.Z > float.MinValue );
                
                // Position delta & length
                var rpd = ( rp1 - rp0 );
                var rd2d = rpd.Length2D;
                
                // Number of chunks for approximate stride
                var chunks = (int)Math.Round( rd2d / nodeLength );
                if( chunks < 1 ) chunks = 1;
                
                // Actual stride for reference points
                var stride = rpd / chunks;
                if( !forcedZStride ) stride.Z = 0.0f; // Not between forced Z markers, follow terrain
                
                // Initial position is current reference point
                var lastPos = new Vector3f( rp0 );
                float lh = wpEntry.LandHeightAtWorldPos( lastPos.X, lastPos.Y );
                if( lh < lowestFloor ) lowestFloor = lh;
                float wh;
                if( !forcedZRef )
                {   // Not forced Z reference, use terrain
                    wh = wpEntry.WaterHeightAtWorldPos( lastPos.X, lastPos.Y );
                    lastPos.Z = lh > wh ? lh : wh; // If the land is above the water, use the land, otherwise the water surface
                }
                
                // Add node from current position
                var refNode = new BorderNode( lastPos.WorldspaceToCellGrid(), lastPos, lh, NodeType.MidPoint );
                nodes.Add( refNode );
                
                // Stride to next reference point
                var lastNode = refNode;
                for( int j = 0; j < chunks - 1; j++ )
                {
                    // Next position and reference point
                    var curPos = lastPos + stride;
                    lh = wpEntry.LandHeightAtWorldPos( curPos.X, curPos.Y );
                    if( lh < lowestFloor ) lowestFloor = lh;
                    if( !forcedZStride )
                    {   // Not between forced Z markers, follow terrain
                        wh = wpEntry.WaterHeightAtWorldPos( curPos.X, curPos.Y );
                        curPos.Z = lh > wh ? lh : wh; // If the land is above the water, use the land, otherwise the water surface
                    }
                    
                    // Get slopes of the last node to the reference node as well as the current node
                    // to the reference node, if they are the same (or close enough) then update the
                    // last node with the current node data; this will eliminate extraneous nodes.
                    var addThisNode = true;
                    var updateRefNode = false;
                    if( refNode != lastNode ) // Don't try to merge a reference node with itself
                    {
                        var lrLength = ( lastNode.P2 - refNode.P2 ).Length;
                        var lrSlope = ( lastNode.P.Z - refNode.P.Z ) / lrLength;
                        
                        var curP2 = new Vector2f( curPos );
                        var crLength = ( curP2 - refNode.P2 ).Length;
                        var crSlope = ( curPos.Z - refNode.P.Z ) / crLength;
                        
                        if( lrSlope.ApproximatelyEquals( crSlope, slopeAllowance ) )
                            addThisNode = false;
                        else
                            updateRefNode = true;
                    }
                    
                    if( addThisNode )
                    {
                        // Set the reference node to the last node and add a new node from current position
                        if( updateRefNode ) refNode = lastNode;
                        lastNode = new BorderNode( curPos.WorldspaceToCellGrid(), curPos, lh, NodeType.MidPoint );
                        nodes.Add( lastNode );
                    }
                    else
                    {
                        // Update the last nodes position and floor, maintaining the reference node
                        lastNode.P = new Vector3f( curPos );
                        lastNode.CellGrid = curPos.WorldspaceToCellGrid();
                        lastNode.Floor = lh;
                    }
                    
                    // Current position = Next position
                    lastPos = curPos;
                }
            }
            {   // Add last reference point
                var rp0 = refPoints[ rCount ];
                
                // Does reference have forced Z?
                var forcedZRef = ( rp0.Z > float.MinValue );
                
                var lastPos = new Vector3f( rp0 );
                float lh = wpEntry.LandHeightAtWorldPos( lastPos.X, lastPos.Y );
                if( lh < lowestFloor ) lowestFloor = lh;
                if( !forcedZRef )
                {   // Not forced Z reference, use terrain
                    var wh = wpEntry.WaterHeightAtWorldPos( lastPos.X, lastPos.Y );
                    lastPos.Z = lh > wh ? lh : wh; // If the land is above the water, use the land, otherwise the water surface
                }
                
                // Add node from last position
                var refNode = new BorderNode( lastPos.WorldspaceToCellGrid(), lastPos, lh, NodeType.MidPoint );
                nodes.Add( refNode );
            }
            //BorderNodeGroup.DumpGroupNodes( nodes, "Generated nodes from linked refs" );
            
            // Check if the first and last refs are the same for a complete loop
            var nCount = nodes.Count;
            if( MergeNodes( nodes, 0, nCount - 1, BorderNode.MIN_NODE_LENGTH, NodeType.MidPoint ) )
                nCount--;   // Merged, update the node count
            else
            {   // First and last nodes are different points
                nodes[ 0 ].Type = NodeType.StartPoint;
                nodes[ nCount - 1 ].Type = NodeType.EndPoint;
            }
            
            //BorderNodeGroup.DumpGroupNodes( nodes, "Merged list from generated nodes from linked refs" );
            // If there's not enough nodes to generate a mesh with, return nothing
            if( nodes.Count < 2 ) return null;
            
            // Update all the node floors with the lowest value for the entire mesh
            foreach( var node in nodes )
                node.Floor = lowestFloor;
            
        localAbort:
            //DebugLog.CloseIndentLevel( "nodes", nodes );
            return nodes;
        }
        
        static bool MergeNodes( List<BorderNode> list, int dest, int src, float tollerance, NodeType newType )
        {
            var n0 = list[ dest ];
            var n1 = list[ src ];
            var result = MergeNodeData( n0, n1, tollerance, newType );
            if( result )
                list.RemoveAt( src );    // Now remove the redundant node
            return result;
        }
        
        static bool MergeNodeData( BorderNode dest, BorderNode src, float tollerance, NodeType newType )
        {
            // Compare by tollerance to account for floating point inaccuracies of the last stride between reference points
            if( ( dest.P2 - src.P2 ).Length > tollerance )
                return false;       //  ...Do nothing
            // dest and src node share the same x,y (or close enough); update dest data from src
            // Use the higher z pos and lower floor then update the node type
            dest.P.Z = Math.Max( dest.P.Z, src.P.Z );
            dest.Floor = Math.Min( dest.Floor, src.Floor );
            dest.Type = newType;       // Set the new type if they are merged
            return true;
        }
        
        public static Vector3f Centre( List<BorderNode> nodes, bool averageZ = false )
        {
            var minX = float.MaxValue;
            var maxX = float.MinValue;
            var minY = float.MaxValue;
            var maxY = float.MinValue;
            var minZ = float.MaxValue;
            var maxZ = float.MinValue;
            var totZ = 0.0f;
            foreach( var node in nodes )
            {
                if( node.P.X < minX ) minX = node.P.X;
                if( node.P.X > maxX ) maxX = node.P.X;
                if( node.P.Y < minY ) minY = node.P.Y;
                if( node.P.Y > maxY ) maxY = node.P.Y;
                if( averageZ )
                    totZ += node.P.Z;
                {
                    if( node.P.Z < minZ ) minZ = node.P.Z;
                    if( node.P.Z > maxZ ) maxZ = node.P.Z;
                }
            }
            return new Vector3f(
                minX + ( maxX - minX ) * 0.5f,
                minY + ( maxY - minY ) * 0.5f,
                averageZ ?
                    totZ / nodes.Count
                    : minZ + ( maxZ - minZ ) * 0.5f
               );
        }
        
        public static Vector3i MinBounds( List<BorderNode> nodes )
        {
            //DebugLog.OpenIndentLevel( new [] { "GUIBuilder.BorderNode", "MinBounds" } );
            var minX = float.MaxValue;
            var minY = float.MaxValue;
            var minZ = float.MaxValue;
            foreach( var node in nodes )
            {
                //DebugLog.WriteLine( "node = " + node.P.ToString() );
                if( node.P.X < minX ) minX = node.P.X;
                if( node.P.Y < minY ) minY = node.P.Y;
                if( node.P.Z < minZ ) minZ = node.P.Z;
            }
            var result = new Vector3i( minX, minY, minZ );
            //DebugLog.CloseIndentLevel( "MinBounds", result.ToString() );
            return result;
        }
        
        public static Vector3i MaxBounds( List<BorderNode> nodes )
        {
            var maxX = float.MinValue;
            var maxY = float.MinValue;
            var maxZ = float.MinValue;
            foreach( var node in nodes )
            {
                if( node.P.X > maxX ) maxX = node.P.X;
                if( node.P.Y > maxY ) maxY = node.P.Y;
                if( node.P.Z > maxZ ) maxZ = node.P.Z;
            }
            return new Vector3i( maxX, maxY, maxZ );
        }
        
        public static void CentreNodes( List<BorderNode> nodes, Vector3f centre )
        {
            foreach( var node in nodes )
            {
                node.P -= centre;
                node.Floor -= centre.Z;
            }
        }
        
    }
    
}
