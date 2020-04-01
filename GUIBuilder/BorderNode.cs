/*
 * BorderNode.cs
 *
 * A border node and group of nodes.
 *
 */
using System;
using System.Collections.Generic;
using System.Linq;

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
        
        public bool                 HasBottom
        {
            get
            {
                if( Nodes.NullOrEmpty() ) return false;
                return Nodes.Any( n => n.HasBottom );
            }
        }

        public void CentreAndPlaceNodes()
        {
            //DebugLog.OpenIndentLevel( new [] { this.FullTypeName(), "CentreAndPlaceNodes()" } );
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
            DebugLog.OpenIndentLevel( new[] { "gradientHeight = " + gradientHeight.ToString(), "groundOffset = " + groundOffset.ToString(), "groundSink = " + groundSink.ToString() }, false, false, false, false, true );
            Mesh = new NIFBuilder.Mesh( this, gradientHeight, groundOffset, groundSink, insideColours, outsideColours );
            DebugLog.CloseIndentLevel();
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
                    var injectEndNode = new BorderNode( currentCell, injectStartNode.P, injectStartNode.Floor, BorderNode.NodeType.EndPoint, false );
                    
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
            DebugLog.WriteLine( string.Format( "GUIBuilder.BorderNodeGroup :: {0}", callerName ) );
            if( !originalForms.NullOrEmpty() )
            {
                DebugLog.WriteLine( string.Format( "\toriginalForms: {0}", originalForms.Count ) );
                for( int i = 0; i < originalForms.Count; i++  )
                {
                    var form = originalForms[ i ];
                    DebugLog.WriteLine( string.Format( "\t\t[ {0} ] = \"{1}\" - {2}", i, form.Signature, form.IDString ) );
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
                "\tmatch result:\n\t\tindex = {0}\n\t\tform = \"{1}\" - {2}\n\t\tscore = {3}",
                bestMatchIndex,
                ( match == null ? null : match.Signature ),
                (
                    match == null
                    ? "[null]"
                    : match.IDString
                ),
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
            DebugLog.OpenIndentLevel( s, false );
            if( !nodes.NullOrEmpty() )
            {
                for( int i = 0; i < nodes.Count; i++ )
                {
                    //DebugLog.WriteLine( "node[ " + i.ToString() + " ] = " + nodes[ i ].ToString() );
                    var i1 = i == nodes.Count - 1 ? 0 : i + 1;
                    var r0 = nodes[ i ].P;
                    var r1 = nodes[ i1 ].P;
                    var dist = (double)( r1 - r0 ).Length2D;
                    var slope = ( (double)r1.Z - (double)r0.Z ) / dist;
                    var angle = Maths.Geometry.Angle( r0.X, r0.Y, r1.X, r1.Y );
                    DebugLog.WriteLine( string.Format(
                        "node[ {0} ] = {1} :: To [ {2} ] = Length = {3}, Slope = {4}, Angle = {5}", i,
                        nodes[ i ].ToString(),
                        i1,
                        dist.ToString(),
                        slope.ToString(),
                        angle.ToString()
                        ) );
                }
            }
            DebugLog.CloseIndentLevel();
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
            return new BorderNode( cellI, intersectPos, intersectFloor, BorderNode.NodeType.EndPoint, false );
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

        public const float DEFAULT_NODE_LENGTH = 128.0f;
        public const float DEFAULT_ANGLE_ALLOWANCE = 2.5f;
        public const float DEFAULT_SLOPE_ALLOWANCE = 0.01f;

        public const float MIN_NODE_LENGTH = 4.0f;
        public const float MIN_ANGLE_ALLOWANCE = 1.5f;
        public const float MIN_SLOPE_ALLOWANCE = 0.001f;
        
        public enum NodeType
        {
            MidPoint = 0,
            StartPoint = 1,
            EndPoint = 2
        }

        /// <summary>
        /// Z component of P is the "surface" (either ground or water height, which ever is higher)
        /// </summary>
        public Vector3f             P;

        /// <summary>
        /// Floor is the lowest point the mesh must reach to (always ground height)
        /// </summary>
        public float                Floor;
        
        public NodeType             Type;
        
        /// <summary>
        /// Vertex Count
        /// </summary>
        public int                  vCount;
        
        /// <summary>
        /// Resolved inside vertex indexes for node frop top (0) to bottom (1/2) for this node
        /// </summary>
        public int[]                iVertex;
        
        /// <summary>
        /// Resolved outside vertex indexes for node frop top (0) to bottom (1/2) for this node
        /// </summary>
        public int[]                oVertex;

        /// <summary>
        /// This node was flagged as _BorderWithBottom
        /// </summary>
        public bool                 HasBottom;

        public bool                 FloorVertexMatchesGroundVertex( float groundOffset, float groundSink )
        { return ( Floor + groundSink ).ApproximatelyEquals( P.Z + groundOffset ); }
        
        public Vector2i             CellGrid;
        
        public Vector2f             P2
        { get { return new Vector2f( P.X, P.Y ); } }
        
        public BorderNode( Vector2i cellGrid, Vector3f p, float floor, NodeType type, bool hasBottom )
        {
            CellGrid                = new Vector2i( cellGrid );
            P                       = new Vector3f( p );
            Floor                   = floor;
            Type                    = type;
            HasBottom               = hasBottom;
        }
        
        public BorderNode           Clone()
        {
            return new BorderNode( CellGrid, P, Floor, Type, HasBottom );
        }
        
        public override string      ToString()
        {
            return string.Format(
                "P = {0} :: Floor = {1} :: CellGrid = {2} :: Type = {3}",
                P.ToString(),
                Floor,
                CellGrid.ToString(),
                Type.ToString() );
        }
        
        public static List<BorderNode> GenerateBorderNodes( Engine.Plugin.Forms.Worldspace worldspace, List<EdgeFlag> flags, float approximateNodeLength, double angleAllowance, double slopeAllowance, Engine.Plugin.Forms.Static forcedZ )
        {
            if( ( worldspace == null )||( flags.NullOrEmpty() ) )
                return null;
            var forcedZFID = forcedZ?.GetFormID( Engine.Plugin.TargetHandle.Master );
            var refPoints = new List<Vector3f>();
            foreach( var flag in flags )
            {
                var p = new Vector3f( flag.Reference.GetPosition( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
                if( ( forcedZ == null )||( forcedZFID != flag.Reference.GetName( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) ) )
                    p.Z = float.MinValue;
                
                refPoints.Add( p );
            }
            return GenerateBorderNodes( worldspace.PoolEntry, refPoints, approximateNodeLength, angleAllowance, slopeAllowance, float.MinValue );
        }
        
        public static List<BorderNode> GenerateBorderNodes( Engine.Plugin.Forms.Worldspace worldspace, List<Engine.Plugin.Forms.ObjectReference> references, float approximateNodeLength, double angleAllowance, double slopeAllowance, Engine.Plugin.Forms.Static forcedZ, Engine.Plugin.Forms.LocationRef borderWithBottomRef )
        {
            if( references.NullOrEmpty() )
                return null;
            var anyNonForcedZMarkers = false;
            var statWFZ = forcedZ?.GetFormID( Engine.Plugin.TargetHandle.Master );
            var bottomZ = float.MaxValue;
            var refPoints = new List<Vector3f>();
            foreach( var reference in references )
            {
                var p = new Vector3f( reference.GetPosition( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
                if( ( forcedZ == null ) || ( statWFZ != reference.GetName( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) ) )
                {
                    p.Z = float.MinValue;
                    anyNonForcedZMarkers = true;
                    bottomZ = float.MinValue;
                }
                else if( ( !anyNonForcedZMarkers )&&( borderWithBottomRef != null ) && ( reference.LocationRefTypes.HasLocationRef( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired, borderWithBottomRef ) ) )
                    if( p.Z < bottomZ ) bottomZ = p.Z;

                refPoints.Add( p );
            }
            if( ( worldspace == null )&&( anyNonForcedZMarkers ) )
                return null;
            return GenerateBorderNodes( worldspace?.PoolEntry, refPoints, approximateNodeLength, angleAllowance, slopeAllowance, bottomZ );
        }

        static void DumpReferencePoints( IList<Vector3f> refPoints, string extra )
        {
            DebugLog.OpenIndentLevel( extra, false );
            if( !refPoints.NullOrEmpty() )
            {
                for( int i = 0; i < refPoints.Count; i++ )
                {
                    var i1 = i == refPoints.Count - 1 ? 0 : i + 1;
                    var r0 = refPoints[ i ];
                    var r1 = refPoints[ i1 ];
                    var dist = (double)( r1 - r0 ).Length2D;
                    var slope = ( (double)r1.Z - (double)r0.Z ) / dist;
                    var angle = Maths.Geometry.Angle( r0.X, r0.Y, r1.X, r1.Y );
                    DebugLog.WriteLine( string.Format(
                        "refPoint[ {0} ] = ( {1}, {2} ) :: To [ {3} ] = Length = {4}, Slope = {5}, Angle = {6}", i,
                        refPoints[ i ].WorldspaceToCellGrid().ToString(),
                        refPoints[ i ].ToString(),
                        i1,
                        dist.ToString(),
                        slope.ToString(),
                        angle.ToString()
                        ) );
                }
            }
            DebugLog.CloseIndentLevel();
        }
        
        static List<BorderNode> GenerateBorderNodes( GodObject.WorldspaceDataPool.PoolEntry wpEntry, IList<Vector3f> refPoints, float nodeLength, double angleAllowance, double slopeAllowance, float bottomZ )
        {
            DebugLog.OpenIndentLevel();
            List<BorderNode> nodes = null;

            var borderWithBottom = bottomZ > float.MinValue;

            if( refPoints.NullOrEmpty() )
            {
                DebugLog.WriteLine( "refPoints is NULL or EMPTY!" );
                goto localAbort;
            }

            if( !borderWithBottom )
            {
                if( wpEntry == null )
                {
                    DebugLog.WriteLine( "wpEntry is NULL!" );
                    goto localAbort;
                }

                if( !wpEntry.LoadHeightMapData() )
                {
                    DebugLog.WriteLine( "LoadHeightMapData() returned false" );
                    goto localAbort;
                }
            }
            
            nodeLength     = nodeLength     < BorderNode.MIN_NODE_LENGTH     ? BorderNode.MIN_NODE_LENGTH     : nodeLength;
            angleAllowance = angleAllowance < BorderNode.MIN_ANGLE_ALLOWANCE ? BorderNode.MIN_ANGLE_ALLOWANCE : angleAllowance;
            slopeAllowance = slopeAllowance < BorderNode.MIN_SLOPE_ALLOWANCE ? BorderNode.MIN_SLOPE_ALLOWANCE : slopeAllowance;

            //DumpReferencePoints( refPoints, "Input Marker Points" );

            // Generate all nodes first

            DebugLog.OpenIndentLevel( "Calculating nodes from reference objects", false );

            if( borderWithBottom )
            {
                var rCount = refPoints.Count;
                nodes = new List<BorderNode>();
                for( int i = 0; i < rCount; i++ )
                {
                    // Reference point
                    var rp0 = new Vector3f( refPoints[ i ] );

                    // Set the bottom Z
                    rp0.Z = bottomZ;

                    // Add the node
                    nodes.Add( new BorderNode( rp0.WorldspaceToCellGrid(), rp0, bottomZ, NodeType.MidPoint, true ) );
                }
            }
            else
            {
                var lowestFloor = float.MaxValue;
                var rCount = refPoints.Count - 1;
                nodes = new List<BorderNode>();
                for( int i = 0; i < rCount; i++ )
                {
                    DebugLog.OpenIndentLevel( string.Format( "Starting scan of ref marker {0} -> {1}", i, i + 1 ), false );

                    // Reference points
                    var rp0 = new Vector3f( refPoints[ i     ] );
                    var rp1 = new Vector3f( refPoints[ i + 1 ] );

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

                    var debugStartNodeCount = nodes.Count;

                    // Add node from current position
                    nodes.Add( new BorderNode( lastPos.WorldspaceToCellGrid(), lastPos, lh, NodeType.StartPoint, false ) );

                    // Stride to next reference point
                    for( int j = 0; j < chunks; j++ )
                    {
                        // Next position and reference point
                        var newPos = lastPos + stride;
                        lh = wpEntry.LandHeightAtWorldPos( newPos.X, newPos.Y );
                        if( lh < lowestFloor ) lowestFloor = lh;
                        if( !forcedZStride )
                        {   // Not between forced Z markers, follow terrain
                            wh = wpEntry.WaterHeightAtWorldPos( newPos.X, newPos.Y );
                            newPos.Z = lh > wh ? lh : wh; // If the land is above the water, use the land, otherwise the water surface
                        }
                        else if( newPos.Z < bottomZ ) bottomZ = newPos.Z;

                        nodes.Add( new BorderNode( newPos.WorldspaceToCellGrid(), newPos, lh,
                            (
                                j == chunks - 1
                                ? NodeType.EndPoint
                                : NodeType.MidPoint
                            ),
                            false ) );

                        // Current position = Next position
                        lastPos = newPos;
                    }

                    var debugEndNodeCount = nodes.Count;
                    DebugLog.WriteLine( string.Format( "Added {0} nodes = {1} - {2}", ( debugEndNodeCount - debugStartNodeCount ), debugStartNodeCount, ( debugEndNodeCount - 1 ) ) );
                    DebugLog.CloseIndentLevel();
                }

                // Update all the node floors with the lowest value for the entire set
                foreach( var node in nodes )
                    node.Floor = lowestFloor;
            }

            BorderNodeGroup.DumpGroupNodes( nodes, string.Format( "Generated Nodes :: nodeLength = {0} :: angleAllowance = {1} :: slopeAllowance = {2} :: borderWithBottom = {3}", nodeLength, angleAllowance, slopeAllowance, borderWithBottom ) );
            DebugLog.CloseIndentLevel();

            DebugLog.OpenIndentLevel( "Optimizing nodes from position, angle and slope", false );

            // Merge any nodes that have the same 2D position
            for( int i = 0; i < nodes.Count; )
            {
                var i1 = ( i + 1 ) % nodes.Count;
                if( !PositionMerge( nodes, i, i1, BorderNode.MIN_NODE_LENGTH ) )
                    i++;
            }

            // Check the first and last points are propery mid points or start and end points
            if( nodes[ nodes.Count - 1 ].Type == NodeType.MidPoint )
                nodes[ 0 ].Type = NodeType.MidPoint;
            if( nodes[ 0 ].Type == NodeType.StartPoint )
                nodes[ nodes.Count - 1 ].Type = NodeType.EndPoint;

            // Merge nodes for slope if they are colinear
            for( int i = 0; i < nodes.Count; )
            {
                var i1 = ( i + 1 ) % nodes.Count;
                var i2 = ( i + 2 ) % nodes.Count;
                if( !SlopeMerge( nodes, i, i1, i2, angleAllowance, slopeAllowance ) )
                    i++;
            }

            DebugLog.CloseIndentLevel();
            BorderNodeGroup.DumpGroupNodes( nodes, string.Format( "Optimized Nodes :: nodeLength = {0} :: angleAllowance = {1} :: slopeAllowance = {2} :: borderWithBottom = {3}", nodeLength, angleAllowance, slopeAllowance, borderWithBottom ) );

            //BorderNodeGroup.DumpGroupNodes( nodes, "Merged list from generated nodes from linked refs" );
            // If there's not enough nodes to generate a mesh with, return nothing
            if( nodes.Count < 2 ) return null;
            
        localAbort:
            BorderNodeGroup.DumpGroupNodes( nodes, string.Format( "Final Node Set :: nodeLength = {0} :: angleAllowance = {1} :: slopeAllowance = {2} :: borderWithBottom = {3}", nodeLength, angleAllowance, slopeAllowance, borderWithBottom ) );
            //DebugLog.CloseIndentLevel( "nodes", nodes );
            DebugLog.CloseIndentLevel();
            return nodes;
        }

        static bool SlopeMerge( List<BorderNode> list, int i0, int i1, int i2, double angleAllowance, double slopeAllowance )
        {
            var n0 = list[ i0 ];
            var n1 = list[ i1 ];
            var n2 = list[ i2 ];

            // Can only slope merge if the middle point is actually a mid point
            if( n1.Type != NodeType.MidPoint )
                return false;       // ...Did nothing

            // Are they co-linear?
            var angle02 = Maths.Geometry.Angle( n0.P.X, n0.P.Y, n2.P.X, n2.P.Y );
            var angle12 = Maths.Geometry.Angle( n1.P.X, n1.P.Y, n2.P.X, n2.P.Y );
            if( !angle02.ApproximatelyEquals( angle12, angleAllowance ) )
                return false;       // ...Did nothing

            // Now compare slopes
            var len02 = (double)( n0.P2 - n2.P2 ).Length;
            var slope02 = ( (double)n0.P.Z - (double)n2.P.Z ) / len02;

            var len12 = (double)( n1.P2 - n2.P2 ).Length;
            var slope12 = ( (double)n1.P.Z - (double)n2.P.Z ) / len12;

            if( !slope02.ApproximatelyEquals( slope12, slopeAllowance ) )
                return false;       // ...Did nothing

            // i0.i2 and i1.i2 share the same slope (or close enough);
            // Use the lower floor for i0 and remove i1
            n0.Floor = Math.Min( n0.Floor, n1.Floor );

            list.RemoveAt( i1 );
            DebugLog.WriteLine( string.Format( "Merged node {0} into {1} for slope and angle with {2} :: slopes = {3} ? {4} :: angles = {5} ? {6}", i1, i0, i2, slope02, slope12, angle02, angle12 ) );
            return true;            // Removed a node
        }

        static bool PositionMerge( List<BorderNode> list, int i0, int i1, float tollerance )
        {
            var n0 = list[ i0 ];
            var n1 = list[ i1 ];

            // Compare 2S distance between nodes
            if( ( n0.P2 - n1.P2 ).Length > tollerance )
                return false;       //  ...Did nothing
            
            // i0 and i1 share the same x,y (or close enough);
            // Use the higher z pos and lower floor, change the node type if an end point becomes a middle point
            n0.P.Z = Math.Max( n0.P.Z, n1.P.Z );
            n0.Floor = Math.Min( n0.Floor, n1.Floor );
            if( ( n0.Type == NodeType.EndPoint ) && ( n1.Type == NodeType.StartPoint ) )
            {
                n0.P.X = n1.P.X;    // Use the start point position of the merged node
                n0.P.Y = n1.P.Y;
                n0.Type = NodeType.MidPoint;
            }
            list.RemoveAt( i1 );
            DebugLog.WriteLine( string.Format( "Merge node {0} into {1} for position :: {2} ? {3}", i1, i0, n0.P2.ToString(), n1.P2.ToString() ) );
            return true;            // Removed a node
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
                else
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
