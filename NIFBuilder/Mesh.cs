/*
 * Mesh.cs
 *
 * Build and write border NIFs
 *
 * User: 1000101
 * Date: 05/02/2019
 * Time: 5:30 PM
 * 
 */
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Maths;
using GUIBuilder;

using EditorIDFormatter = GUIBuilder.CustomForms.EditorIDFormats;


public static partial class NIFBuilder
{
    /// <summary>
    /// Description of Mesh.
    /// </summary>
    public class Mesh
    {
        
        public class MeshVertex
        {
            Mesh                    Parent;
            public Vector3f         Vertex;
            public float            BiTangentX;
            public HalfVector2f     UV;
            public ByteVector3      Normal;
            public byte             BiTangentY;
            public ByteVector3      Tangent;
            public byte             BiTangentZ;
            public uint             Colour;

            public override string ToString()
            {
                return string.Format( "{0} : {1}", Vertex.ToString(), Colour.ToString( "X8" ) );
            }

            public MeshVertex( Mesh parent, Vector3f vertex, Vector2f uv, uint colour )
            {
                Parent  = parent;
                Vertex  = new Vector3f( vertex );
                UV      = new HalfVector2f( uv );
                Colour  = colour;
            }
            
            public void WriteToStream( System.IO.BinaryWriter stream )
            {
                if( Parent.fullPrecisionVerts )
                {
                    Vertex.WriteToStream( stream ); // + 12 (3 x sizeof( float ))
                    stream.Write( BiTangentX );     // +  4 (sizeof( float ))
                }                                   // = 16
                else
                {
                    var hv3 = new HalfVector3f( Vertex );
                    var h = new Half( BiTangentX );
                    hv3.WriteToStream( stream );    // +  6 (3 x Half.SizeOf())
                    h.WriteToStream( stream );      // +  2 (Half.SizeOf())
                }                                   // =  8

                UV.WriteToStream( stream );         // +  6 (3 x Half.SizeOf())
                Normal.WriteToStream( stream );     // +  3
                stream.Write( BiTangentY );         // +  1
                Tangent.WriteToStream( stream );    // +  3
                stream.Write( BiTangentZ );         // +  1
                stream.Write( Colour );             // +  2
                                                    // = 16
            }                                       // 24 or 32 = 16 + 8 or 16

        }
        
        public class MeshTriangle
        {
            public ushort           p1, p2, p3;
            
            Mesh                    parent;

            public override string ToString()
            {
                return string.Format( "{0} : {1} : {2}", p1, p2, p3 );
            }

            public                  MeshTriangle( Mesh mesh, int v1, int v2, int v3 )
            {
                parent = mesh;
                p1 = (ushort)v1;
                p2 = (ushort)v2;
                p3 = (ushort)v3;
            }
            
            Vector3f                RawVertNorm( ushort index )
            {
                return new Vector3f(
                    parent.rawVerts[ index ].X * -0.1f,
                    parent.rawVerts[ index ].Y *  0.1f,
                    parent.rawVerts[ index ].Z *  0.1f );
            }
            
            public Vector3f         Normal()
            {
                var v1 = RawVertNorm( p1 );
                var v2 = RawVertNorm( p2 );
                var v3 = RawVertNorm( p3 );
                
                var s = v2 - v1;
                var t = v3 - v1;
                
                var n = Vector3f.Cross( s, t );
                n.Normalize();
                
                return n;
            }
            
            public void             WriteToStream( System.IO.BinaryWriter stream )
            {
                stream.Write( p1 );
                stream.Write( p2 );
                stream.Write( p3 );
            }
            
        }

        public class MeshBottomReference : IEquatable<MeshBottomReference>
        {
            Mesh                    parent;
            public ushort[]         n;

            public Vector2f[]       poly;

            public MeshBottomReference( Mesh mesh, int v1, int v2, int v3 )
            {
                parent = mesh;
                n = new ushort[ 3 ] { (ushort)v1, (ushort)v2, (ushort)v3 };
                Array.Sort( n );
                var nodes = parent.nodeGroup.Nodes;
                poly = new Vector2f[] { nodes[ n[ 0 ] ].P2, nodes[ n[ 1 ] ].P2, nodes[ n[ 2 ] ].P2 };
            }

            #region Equality (IEquatable)

            public override bool Equals( object obj )
            {
                if( obj is MeshBottomReference )
                    return Equals( (MeshBottomReference)obj ); // use Equals method below
                else
                    return false;
            }

            public bool Equals( MeshBottomReference other )
            {
                if( this.parent != other.parent ) return false;
                for( int i = 0; i < 3; i++ )
                    if( this.n[ i ] != other.n[ i ] ) return false;
                return true;
            }

            public override int GetHashCode()
            {
                // combine the hash codes of all members here (e.g. with XOR operator ^)
                return n[ 0 ].GetHashCode() ^ n[ 1 ].GetHashCode() ^ n[ 2 ].GetHashCode();
            }

            public static bool operator ==( MeshBottomReference left, MeshBottomReference right )
            {
                return left.Equals( right );
            }

            public static bool operator !=( MeshBottomReference left, MeshBottomReference right )
            {
                return !left.Equals( right );
            }

            #endregion

            public float LargestAngle
            {
                get
                {
                    var nodes = parent.nodeGroup.Nodes;
                    var a01 = Maths.Geometry.Angle( nodes[ n[ 0 ] ].P2, nodes[ n[ 1 ] ].P2 );
                    var a12 = Maths.Geometry.Angle( nodes[ n[ 1 ] ].P2, nodes[ n[ 2 ] ].P2 );
                    var a20 = Maths.Geometry.Angle( nodes[ n[ 2 ] ].P2, nodes[ n[ 0 ] ].P2 );
                    var a012 = ( 360.0f + ( a01 - a12 ) ) % 360.0f;
                    var a120 = ( 360.0f + ( a12 - a20 ) ) % 360.0f;
                    var a201 = ( 360.0f + ( a20 - a01 ) ) % 360.0f;
                    var result = a012;
                    if( a120 > result ) result = a120;
                    if( a201 > result ) result = a201;
                    return result;
                }
            }

            bool Intersects( Vector2f a1, Vector2f a2, Vector2f b1, Vector2f b2 )
            {
                var intersection = Maths.Geometry.Collision.LineLineIntersect( a1, a2, b1, b2, out Vector2f r );
                return ( intersection == Geometry.CollisionType.SinglePoint );
            }

            public int IntersectsWithAnyTri( List<MeshBottomReference> tris )
            {
                if( tris.NullOrEmpty() ) return -1;
                
                var nodes = parent.nodeGroup.Nodes;
                for( int i = 0; i < tris.Count; i++ )
                {
                    var tri = tris[ i ];

                    // Edge 0->1
                    if( Intersects( nodes[ n[ 0 ] ].P2, nodes[ n[ 1 ] ].P2, nodes[ tri.n[ 0 ] ].P2, nodes[ tri.n[ 1 ] ].P2 ) ) return i;
                    if( Intersects( nodes[ n[ 0 ] ].P2, nodes[ n[ 1 ] ].P2, nodes[ tri.n[ 1 ] ].P2, nodes[ tri.n[ 2 ] ].P2 ) ) return i;
                    if( Intersects( nodes[ n[ 0 ] ].P2, nodes[ n[ 1 ] ].P2, nodes[ tri.n[ 2 ] ].P2, nodes[ tri.n[ 0 ] ].P2 ) ) return i;

                    // Edge 1->2
                    if( Intersects( nodes[ n[ 1 ] ].P2, nodes[ n[ 2 ] ].P2, nodes[ tri.n[ 0 ] ].P2, nodes[ tri.n[ 1 ] ].P2 ) ) return i;
                    if( Intersects( nodes[ n[ 1 ] ].P2, nodes[ n[ 2 ] ].P2, nodes[ tri.n[ 1 ] ].P2, nodes[ tri.n[ 2 ] ].P2 ) ) return i;
                    if( Intersects( nodes[ n[ 1 ] ].P2, nodes[ n[ 2 ] ].P2, nodes[ tri.n[ 2 ] ].P2, nodes[ tri.n[ 0 ] ].P2 ) ) return i;

                    // Edge 2->0
                    if( Intersects( nodes[ n[ 2 ] ].P2, nodes[ n[ 0 ] ].P2, nodes[ tri.n[ 0 ] ].P2, nodes[ tri.n[ 1 ] ].P2 ) ) return i;
                    if( Intersects( nodes[ n[ 2 ] ].P2, nodes[ n[ 0 ] ].P2, nodes[ tri.n[ 1 ] ].P2, nodes[ tri.n[ 2 ] ].P2 ) ) return i;
                    if( Intersects( nodes[ n[ 2 ] ].P2, nodes[ n[ 0 ] ].P2, nodes[ tri.n[ 2 ] ].P2, nodes[ tri.n[ 0 ] ].P2 ) ) return i;
                }

                return -1;
            }

            public int IntersectsWithAnyEdge()
            {
                var nodes = parent.nodeGroup.Nodes;
                var nCount = parent.nCount;
                for( int i = 0; i < nCount; i++ )
                {
                    int j = ( i + 1 ) % nCount;
                    if( ( n[ 0 ] != i )&&( n[ 0 ] != j )&&
                        ( n[ 1 ] != i )&&( n[ 1 ] != j )&&
                        ( Intersects( nodes[ n[ 0 ] ].P2, nodes[ n[ 1 ] ].P2, nodes[ i ].P2, nodes[ j ].P2 ) ) ) return i;
                    if( ( n[ 1 ] != i )&&( n[ 1 ] != j )&&
                        ( n[ 2 ] != i )&&( n[ 2 ] != j )&&
                        ( Intersects( nodes[ n[ 1 ] ].P2, nodes[ n[ 2 ] ].P2, nodes[ i ].P2, nodes[ j ].P2 ) ) ) return i;
                    if( ( n[ 2 ] != i )&&( n[ 2 ] != j )&&
                        ( n[ 0 ] != i )&&( n[ 0 ] != j )&&
                        ( Intersects( nodes[ n[ 2 ] ].P2, nodes[ n[ 0 ] ].P2, nodes[ i ].P2, nodes[ j ].P2 ) ) ) return i;
                }
                return -1;
            }

            public int AnyNodeInsideTri()
            {
                var nodes = parent.nodeGroup.Nodes;
                var nCount = parent.nCount;
                for( int i = 0; i < nCount; i++ )
                {
                    if( ( n[ 0 ] != i)&&( n[ 1 ] != i )&&( n[ 2 ] != i )&&
                        ( Maths.Geometry.Collision.PointInPoly( nodes[ i ].P2, poly, Geometry.Orientation.CW ) ) ) return i;
                }
                return -1;
            }
            
            public override string ToString()
            {
                return string.Format( "{0} : {1} : {2}", n[ 0 ], n[ 1 ], n[ 2 ] );
            }
            
        }
        
        //string              nifFilePath;
        //public string       nifFile;
        //string              nifPath;

        /// <summary>
        /// Node count
        /// </summary>
        int                 nCount;

        /// <summary>
        /// Vertex count
        /// </summary>
        int                 vCount;
        
        /// <summary>
        /// Triangle count
        /// </summary>
        int                 tCount;

        /// <summary>
        /// Node Group
        /// </summary>
        BorderNodeGroup     nodeGroup;

        /// <summary>
        /// Inside vertex colours top (0) -> bottom (1/2)
        /// </summary>
        uint[]              iColours;

        /// <summary>
        /// Outside vertex colours top (0) -> bottom (1/2)
        /// </summary>
        uint[]              oColours;

        /// <summary>
        /// Gradient Height
        /// </summary>
        float               gHeight;

        /// <summary>
        /// Ground Offset
        /// </summary>
        float               gOffset;
        
        /// <summary>
        /// Ground Sink
        /// </summary>
        float               gSink;
        
        Vector3f[]          rawVerts;
        Vector3f[]          rawNormals;
        Vector3f[]          rawTangents;
        Vector3f[]          rawBiTangents;
        
        MeshVertex[]        vertexes;
        MeshTriangle[]      triangles;
        
        List<MeshBottomReference> bottom;
        
        uint                meshBlocks;

        bool                fullPrecisionVerts = false;

        public Mesh( BorderNodeGroup group, float gradientHeight, float groundOffset, float groundSink, uint[] insideColours, uint[] outsideColours, bool fullPrecisionVertexes )
        {

            //DebugLog.OpenIndentLevel();
            //DebugLog.WriteList( "nodes", group.Nodes, true, true );

            nodeGroup           = group ?? throw new NullReferenceException( "group" );
            
            fullPrecisionVerts = fullPrecisionVertexes;

            /*
            nifFilePath         = nodeGroup.NIFFilePath;
            string workingPath;
            var working         = GenFilePath.FilenameFromPathname( nifFilePath, out workingPath );
            nifFile             = working.EndsWith( ".nif", StringComparison.InvariantCultureIgnoreCase )
                                ? working.Substring( 0, working.Length - 4 )
                                : working;
            */

            nCount              = nodeGroup.Nodes.NullOrEmpty() ? 0 : nodeGroup.Nodes.Count;
            gHeight             = Math.Max( 0.0f, gradientHeight );
            gOffset             = Math.Max( 0.0f, groundOffset );
            gSink               = Math.Max( 0.0f, groundSink );
            
            iColours            = new uint[ 3 ];
            oColours            = new uint[ 3 ];
            for( int i = 0; i < 3; i++ )
            {
                iColours[ i ]   = insideColours[ i ];
                oColours[ i ]   = outsideColours[ i ];
            }

            bool built = false;

            if( ( nCount >= 2 ) && ( gHeight > 0.0f ) )
            {
                bottom          = CalculateBottomTriangles();
                vCount          = CalculateTotalVertexCount();
                tCount          = CalculateTotalTriangleCount();

                DebugLog.WriteStrings( null, new [] {
                    "nCount = " + nCount,
                    "vCount = " + vCount,
                    "tCount = " + tCount,
                    "nifFilePath = \"" + nodeGroup.NIFFilePath( false ) + "\""
                }, false, true, false, false, true, true );

                if( vCount > 0 )
                {
                    rawVerts        = new Vector3f[ vCount ];
                    rawNormals      = new Vector3f[ vCount ];
                    rawTangents     = new Vector3f[ vCount ];
                    rawBiTangents   = new Vector3f[ vCount ];

                    vertexes        = new MeshVertex[ vCount ];
                    triangles       = new MeshTriangle[ tCount ];

                    var bCount      = BuildVertexArray();
                    built           = bCount == vCount;
                    if( !built )
                        DebugLog.WriteError( "BuildVertexArray() = " + bCount.ToString() );
                    else
                    {
                        bCount      = BuildTriangleArray();
                        built       = bCount == tCount;
                        if( !built )
                            DebugLog.WriteError( "BuildTriangleArray() = " + bCount.ToString() );
                    }
                    if( built )
                    {
                        CalculateVertexNormals();
                        CalculateVertexTangents();
                    }
                }
            }

            if( !built )
            {
                iColours        = null;
                oColours        = null;
                    
                bottom          = null;

                vCount          = 0;
                tCount          = 0;

                rawVerts        = null;
                rawNormals      = null;
                rawTangents     = null;
                rawBiTangents   = null;

                vertexes        = null;
                triangles       = null;
            }
            
            meshBlocks      = ( vCount > 0 )&&( tCount > 0 ) ? NIFBlockCount : NIFBlockCountEmpty;
            
            //DebugLog.CloseIndentLevel();
        }

        #region String Builders

        public static List<string> MatchKeys( string name, string neighbour = null, int borderIndex = 1, int subIndex = -1 )
        {
            var keys = new List<string>{ name };
            var sBorderIndex = borderIndex > -1
                ? borderIndex.ToString( "X2" )
                : null;
            if( !string.IsNullOrEmpty( sBorderIndex ) )
                keys.Add( sBorderIndex );
            if( !string.IsNullOrEmpty( neighbour ) )
                keys.Add( neighbour );
            var mssi = subIndex > -1
                ? subIndex.ToString( "X2" )
                : null;
                //= MeshSubSetIndex( neighbour, subIndex );
            if( !string.IsNullOrEmpty( mssi ) )
                keys.Add( mssi );
            return keys;
        }
        
        /*
        public static string BuildTargetPath( string target, string targetSubPath )
        {
            if( ( !string.IsNullOrEmpty( target ) )&&( target[ target.Length - 1 ] != '\\' ) )
                target += @"\";
            if( ( !string.IsNullOrEmpty( targetSubPath ) )&&( targetSubPath[ targetSubPath.Length - 1 ] != '\\' ) )
                targetSubPath += @"\";
            return string.Format(
                @"{0}{1}",
                target,
                targetSubPath
            );
        }
        
        public static string BuildFilePath( string targetSubPath, string meshSubPath, string statEditorIDFormat, string modPrefix, string name, int index = 1, string neighbour = null, int subIndex = -1 )
        {
            if( ( !string.IsNullOrEmpty( targetSubPath ) )&&( !targetSubPath.EndsWith( @"\" ) ) )
                targetSubPath += @"\";
            if( ( !string.IsNullOrEmpty( meshSubPath ) )&&( meshSubPath.EndsWith( @"\" ) ) )
                meshSubPath += @"\";
            var sBorderIndex = index > -1
                ? index.ToString( "X2" )
                : null;
            return EditorIDFormatter.FormatEditorID(
                string.Format(
                    @"{0}Meshes\{1}{2}.nif",
                    targetSubPath,
                    meshSubPath,
                    statEditorIDFormat
                ),
                modPrefix,
                "STAT",
                name,
                index,
                neighbour,
                subIndex
                );
        }
        */

        #endregion

        #region Build Mesh

        #region Vertex & Triangle Array Assignment

        void AssignRawVert( ref int i, Vector3f p, Vector2f uv, uint c )
        {
            try
            {
                rawVerts[ i ] = new Vector3f( p );
                vertexes[ i ] = new MeshVertex( this, p, uv, c );
                //DebugLog.WriteLine( string.Format( "rawVerts[ {0} ] = {1} :: vertexes[ {0} ] = {2}", i, rawVerts[ i ].ToString(), vertexes[ i ].ToStringNullSafe() ) );
                i++;
            }
            catch( Exception e )
            {
                DebugLog.WriteError( string.Format( "Exception assigning raw vert\n\tindex = {0}\n\tvertexes = {1}\n\trawVerts = {2}\n\nException:\n{3}", i, vertexes.Length, rawVerts.Length, e.ToString() ) );
            }
        }
        
        void AssignRawTri( ref int i, int p1, int p2, int p3 )
        {
            try
            {
                triangles[ i ] = new MeshTriangle( this, p1, p2, p3 );
                //DebugLog.WriteLine( string.Format( "triangle[ {0} ] = {1}", i, triangles[ i ].ToStringNullSafe() ) );
                i++;
            }
            catch( Exception e )
            {
                DebugLog.WriteException( e, string.Format( "index = {0}\ntriangles = {1}", i, triangles.Length ) );
            }
        }

        #endregion

        #region Calculate Vertex Array

        int CalculateTotalVertexCount()
        {
            if( nCount < 2 ) return 0;

            var nodes = nodeGroup.Nodes;
            int result = 0;

            for( int i = 0; i < nCount; i++ )
            {
                var currentNode = nodes[ i ];
                var verts = currentNode.FloorVertexMatchesGroundVertex( gOffset, gSink )
                    ? 2
                    : 3;

                currentNode.iVertex = new int[ verts ];
                for( int k = 0; k < verts; k++ )
                    currentNode.iVertex[ k ] = result + k;
                result += verts;

                currentNode.oVertex = new int[ verts ];
                for( int k = 0; k < verts; k++ )
                    currentNode.oVertex[ k ] = result + k;
                result += verts;

                currentNode.vCount = verts;

                //DebugLog.Write( string.Format( "CalculateValidMeshVertexOffsets() :: {0}/{1} :: {2} :: {3} :: {4} :: {5}", i, j, verts, result, gOffset, gSink ) );
            }
            return result;
        }

        int BuildVertexArray()
        {
            if( vCount < 1 ) return 0;

            var uv = new Vector2f[]{
                new Vector2f( -6.632813f, 0.0f ),
                new Vector2f( -6.632813f, 1.0f ),
                new Vector2f( -6.632813f, 1.0f ) };
            int vIndex = 0;
            for( int i = 0; i < nCount; i++ )
            {
                var node = nodeGroup.Nodes[ i ];
                var nPos = node.P;
                
                //DebugLog.Write( string.Format( "BuildVertexArray() :: {0}/{1} :: {2}/{3}", i, nCount, vIndex, vCount ) );
                
                var vT = new Vector3f( nPos.X, nPos.Y, nPos.Z     + gOffset + gHeight );
                var vG = new Vector3f( nPos.X, nPos.Y, nPos.Z     + gOffset           );
                var vF = new Vector3f( nPos.X, nPos.Y, node.Floor - gSink             );
                
                // Vertexes and raw vertex coords for later normal and tangent calculations
                AssignRawVert( ref vIndex, vT, uv[ 0 ], iColours[ 0 ] );        // Inside Top
                AssignRawVert( ref vIndex, vG, uv[ 1 ], iColours[ 1 ] );        // Inside Ground
                if( node.vCount == 3 )
                    AssignRawVert( ref vIndex, vF, uv[ 2 ], iColours[ 2 ] );    // Inside Floor
                
                AssignRawVert( ref vIndex, vT, uv[ 0 ], oColours[ 0 ] );        // Outside Top
                AssignRawVert( ref vIndex, vG, uv[ 1 ], oColours[ 1 ] );        // Outside Ground
                if( node.vCount == 3 )
                    AssignRawVert( ref vIndex, vF, uv[ 2 ], oColours[ 2 ] );    // Outside Floor
                
                /*
                // Raw vertex coords for now and later normal and tangent calculations
                rawVerts[ vIndex + 0 ] = new Vector3f( nPos.X, nPos.Y, nPos.Z     + gOffset + gHeight );
                rawVerts[ vIndex + 1 ] = new Vector3f( nPos.X, nPos.Y, nPos.Z     + gOffset           );
                rawVerts[ vIndex + 2 ] = new Vector3f( nPos.X, nPos.Y, node.Floor - gSink             );
                rawVerts[ vIndex + 3 ] = new Vector3f( rawVerts[ vIndex + 0 ] );
                rawVerts[ vIndex + 4 ] = new Vector3f( rawVerts[ vIndex + 1 ] );
                rawVerts[ vIndex + 5 ] = new Vector3f( rawVerts[ vIndex + 2 ] );
                
                // Inside
                vertexes[ vIndex + 0 ] = new MeshVertex( rawVerts[ vIndex + 0 ], uv[ 0 ], iColours[ 0 ] );
                vertexes[ vIndex + 1 ] = new MeshVertex( rawVerts[ vIndex + 1 ], uv[ 1 ], iColours[ 1 ] );
                vertexes[ vIndex + 2 ] = new MeshVertex( rawVerts[ vIndex + 2 ], uv[ 2 ], iColours[ 2 ] );
                
                // Outside
                vertexes[ vIndex + 3 ] = new MeshVertex( rawVerts[ vIndex + 3 ], uv[ 0 ], oColours[ 0 ] );
                vertexes[ vIndex + 4 ] = new MeshVertex( rawVerts[ vIndex + 4 ], uv[ 1 ], oColours[ 1 ] );
                vertexes[ vIndex + 5 ] = new MeshVertex( rawVerts[ vIndex + 5 ], uv[ 2 ], oColours[ 2 ] );
                
                vIndex += 6;
                */
            }

            return vIndex;
        }

        #endregion

        #region Calculate Triangle Array

        int TrianglesCountBetween( BorderNode cNode, BorderNode nNode )
        {
            if( ( cNode == null ) || ( nNode == null ) ) return 0;
            if( cNode.Type == BorderNode.NodeType.EndPoint ) return 0;

            var cnHasFloor = cNode.vCount == 3;
            var nnHasFloor = nNode.vCount == 3;
            var tris = cnHasFloor
                    ? nnHasFloor
                        ? 4
                        : 3
                    : nnHasFloor
                        ? 3
                        : 2;

            var result = tris * 2; // for inside + outside
            //DebugLog.WriteLine( string.Format( "{0} -> {1} :: {2} :: {3}", cNode.ToStringNullSafe(), nNode.ToStringNullSafe(), tris, result ), true );
            return result;
        }

        int CalculateTotalTriangleCount()
        {
            if( nCount < 2 ) return 0;

            var nodes = nodeGroup.Nodes;
            int result = bottom.NullOrEmpty() ? 0 : bottom.Count * 2;

            for( int i = 0; i < nCount; i++ )
                result += TrianglesCountBetween(
                    nodes[ i ],
                    nodes[ ( i + 1 ) % nCount ] );

            return result;
        }

        int BuildTriangleArray()
        {
            if( tCount < 1 ) return 0;

            int tIndex = 0;
            
            var nodes = nodeGroup.Nodes;
            
            // Build vertical edges first
            for( int i = 0; i < nCount; i++ )
            {
                var cNode = nodes[ i ];
                if( cNode.Type != BorderNode.NodeType.EndPoint )
                {
                    //var n = i >= nCount - 1 ? i - nCount: i; n++;
                    var n = ( i + 1 ) % nCount;
                    var nNode = nodes[ n ];
                    
                    var cnHasFloor = cNode.vCount == 3;
                    var nnHasFloor = nNode.vCount == 3;

                    //DebugLog.Write( string.Format( "BuildTriangleArray() :: {0}/{1} :: {2}/{3}", i, nCount, tIndex, tCount ) );

                    AssignRawTri( ref tIndex, cNode.iVertex[ 0 ], cNode.iVertex[ 1 ], nNode.iVertex[ 0 ] );           // Top Inside
                    AssignRawTri( ref tIndex, cNode.iVertex[ 1 ], nNode.iVertex[ 1 ], nNode.iVertex[ 0 ] );           // Top Inside
                    if( cnHasFloor )
                    {
                        AssignRawTri( ref tIndex, cNode.iVertex[ 1 ], cNode.iVertex[ 2 ], nNode.iVertex[ 1 ] );       // Bottom Inside
                        if( nnHasFloor )
                            AssignRawTri( ref tIndex, cNode.iVertex[ 2 ], nNode.iVertex[ 2 ], nNode.iVertex[ 1 ] );   // Bottom Inside
                    }
                    else if( nnHasFloor )
                        AssignRawTri( ref tIndex, cNode.iVertex[ 1 ], nNode.iVertex[ 2 ], nNode.iVertex[ 1 ] );       // Bottom Inside
                    
                    AssignRawTri( ref tIndex, cNode.oVertex[ 1 ], cNode.oVertex[ 0 ], nNode.oVertex[ 0 ] );           // Top Outside
                    AssignRawTri( ref tIndex, cNode.oVertex[ 1 ], nNode.oVertex[ 0 ], nNode.oVertex[ 1 ] );           // Top Outside
                    if( cnHasFloor )
                    {
                        AssignRawTri( ref tIndex, cNode.oVertex[ 2 ], cNode.oVertex[ 1 ], nNode.oVertex[ 1 ] );       // Bottom Outside
                        if( nnHasFloor )
                            AssignRawTri( ref tIndex, cNode.oVertex[ 2 ], nNode.oVertex[ 1 ], nNode.oVertex[ 2 ] );   // Bottom Outside
                    }
                    else if( nnHasFloor )
                        AssignRawTri( ref tIndex, cNode.oVertex[ 1 ], nNode.oVertex[ 1 ], nNode.oVertex[ 2 ] );       // Bottom Outside
                }
            }
            
            // Build bottom edges
            if( nodeGroup.HasBottom )
            {
                var bCount = bottom.Count;
                for( int i = 0; i < bCount; i++ )
                {
                    var n0 = nodes[ bottom[ i ].n[ 1 ] ];               // Swap 0 and 1 to go from CW -> CCW
                    var n1 = nodes[ bottom[ i ].n[ 0 ] ];
                    var n2 = nodes[ bottom[ i ].n[ 2 ] ];

                    var v0 = n0.iVertex[ n0.vCount - 1 ]; // Bottom vertexes for all
                    var v1 = n1.iVertex[ n1.vCount - 1 ];
                    var v2 = n2.iVertex[ n2.vCount - 1 ];
                    AssignRawTri( ref tIndex, v0, v1, v2 );             // Inside
                    
                    v0 = n0.oVertex[ n0.vCount - 1 ];
                    v1 = n1.oVertex[ n1.vCount - 1 ];
                    v2 = n2.oVertex[ n2.vCount - 1 ];
                    AssignRawTri( ref tIndex, v2, v1, v0 );             // Outside
                }
            }

            return tIndex;
        }

        List<MeshBottomReference> CalculateBottomTriangles()
        {
            if( !nodeGroup.HasBottom ) return null;
            if( nCount < 3 ) return null;

            //DebugLog.OpenIndentLevel();

            int i, j, k;
            var rejected = new List<MeshBottomReference>();
            var accepted = new List<MeshBottomReference>();
            MeshBottomReference tri;

            i = 0;
            while( i < nCount )
            {
                j = ( i + 1 ) % nCount;
                while( j != i )
                {
                    k = ( j + 1 ) % nCount;
                    while( k != j )
                    {
                        if( k == i ) goto continueK;

                        tri = new MeshBottomReference( this, i, j, k );

                        if( ( rejected.Contains( tri ) ) ||
                            ( accepted.Contains( tri ) ) ) goto continueK;

                        var largestAngle = tri.LargestAngle;
                        if( largestAngle >= 180.0f )
                        {
                            //DebugLog.WriteLine( string.Format( "{0} :: Rejected :: Angle {1} > 180.0f", tri.ToString(), largestAngle ) );
                            rejected.Add( tri );
                            goto continueK;
                        }

                        var triIntersect = tri.IntersectsWithAnyTri( accepted );
                        if( triIntersect >= 0 )
                        {
                            //DebugLog.WriteLine( string.Format( "{0} :: Rejected :: Tri Intersect {1}", tri.ToString(), accepted[ triIntersect ].ToString() ) );
                            rejected.Add( tri );
                            goto continueK;
                        }

                        var edgeIntersect = tri.IntersectsWithAnyEdge();
                        if( edgeIntersect >= 0 )
                        {
                            //DebugLog.WriteLine( string.Format( "{0} :: Rejected :: Edge Intersect {1} : {2}", tri.ToString(), edgeIntersect, ( ( edgeIntersect + 1 ) % nCount ) ) );
                            rejected.Add( tri );
                            goto continueK;
                        }

                        var pointInPoly = tri.AnyNodeInsideTri();
                        if( pointInPoly >= 0 )
                        {
                            //DebugLog.WriteLine( string.Format( "{0} :: Rejected :: Node in tri {1}", tri.ToString(), pointInPoly ) );
                            rejected.Add( tri );
                            goto continueK;
                        }

                        accepted.Add( tri );
                        break;

                    continueK:
                        k = ( k + 1 ) % nCount;
                    }

                    j = ( j + 1 ) % nCount;
                }

                i++;
            }

            //DebugLog.WriteList( "rejected", rejected, false, true );
            //DebugLog.WriteList( "accepted", accepted, false, true );

            //DebugLog.CloseIndentLevel();

            return accepted.NullOrEmpty() ? null : accepted;
        }

        #endregion

        #region Calculate Normals

        void CalculateVertexNormals()
        {
            if( ( tCount < 1 ) || ( vCount < 3 ) ) return;

            var norms = new Vector3f[ vCount ];
            
            // Compute triangle norms
            for( int i = 0; i < tCount; i++ )
            {
                MeshTriangle tri = null;
                try
                {
                    tri = triangles[ i ];
                    var triNorm = tri.Normal();

                    norms[ tri.p1 ] += triNorm;
                    norms[ tri.p2 ] += triNorm;
                    norms[ tri.p3 ] += triNorm;
                }
                catch( Exception e )
                {
                    DebugLog.WriteException( e, string.Format( "index = {0}\ntriangles = {1}\ntriangle = {2}", i, triangles.Length, tri.ToStringNullSafe() ) );
                }
            }
            
            // Compute vertex norms from triangle norms
            for( int i = 0; i < vCount; i++ )
            {
                norms[ i ].Normalize();
                rawNormals[ i ] = new Vector3f(
                    -norms[ i ].X,
                    norms[ i ].Y,
                    norms[ i ].Z );
                vertexes[ i ].Normal = new ByteVector3(
                    rawNormals[ i ].X,
                    rawNormals[ i ].Y,
                    rawNormals[ i ].Z );
            }
        }
        
        void CalculateVertexTangents()
        {
            if( ( tCount < 1 ) || ( vCount < 3 ) ) return;

            rawTangents     = new Vector3f[ vCount ];
            rawBiTangents   = new Vector3f[ vCount ];
            
            // Compute triangle tangents
            for( int i = 0; i < tCount; i++ )
            {
                var i1 = triangles[ i ].p1;
                var i2 = triangles[ i ].p2;
                var i3 = triangles[ i ].p3;
                
                var v1 = vertexes[ i1 ].Vertex;
                var v2 = vertexes[ i2 ].Vertex;
                var v3 = vertexes[ i3 ].Vertex;
                
                var w1 = new Vector2f( vertexes[ i1 ].UV );
                var w2 = new Vector2f( vertexes[ i2 ].UV );
                var w3 = new Vector2f( vertexes[ i3 ].UV );
                
                var x1 = v2.X - v1.X;
                var x2 = v3.X - v1.X;
                
                var y1 = v2.Y - v1.Y;
                var y2 = v3.Y - v1.Y;
                
                var z1 = v2.Z - v1.Z;
                var z2 = v3.Z - v1.Z;
                
                var s1 = w2.X - w1.X;
                var s2 = w3.X - w1.X;
                
                var t1 = w2.Y - w1.Y;
                var t2 = w3.Y - w1.Y;
                
                var r = ( ( s1 * t2 ) - ( s2 * t1 ) ) >= 0.0f ? 1.0f : -1.0f;
                
                var sdir = new Vector3f(
                    ( t2 * x1 - t1 * x2 ) * r,
                    ( t2 * y1 - t1 * y2 ) * r,
                    ( t2 * z1 - t1 * z2 ) * r );
                
                var tdir = new Vector3f(
                    ( s1 * x2 - s2 * x1 ) * r,
                    ( s1 * y2 - s2 * y1 ) * r,
                    ( s1 * z2 - s2 * z1 ) * r );
                
                sdir.Normalize();
                tdir.Normalize();
                
                rawTangents  [ i1 ] += tdir; rawTangents  [ i2 ] += tdir; rawTangents  [ i3 ] += tdir;
                rawBiTangents[ i1 ] += sdir; rawBiTangents[ i2 ] += sdir; rawBiTangents[ i3 ] += sdir;
            }
            
            // Compute vertex tangents from triangle tangents
            for( int i = 0; i < vCount; i++ )
            {
                if( ( rawTangents[ i ].IsZero() )||( rawBiTangents[ i ].IsZero() ) )
                {
                    rawTangents  [ i ] = new Vector3f( rawNormals[ i ] );
                    rawBiTangents[ i ] = Vector3f.Cross( rawNormals[ i ], rawTangents[ i ] );
                }
                else
                {
                    rawTangents  [ i ].Normalize();
                    rawTangents  [ i ] = new Vector3f( rawTangents  [ i ] - rawNormals [ i ] * Vector3f.Dot( rawNormals [ i ], rawTangents  [ i ] ) );
                    rawTangents  [ i ].Normalize();
                    
                    rawBiTangents[ i ].Normalize();
                    rawBiTangents[ i ] = new Vector3f( rawBiTangents[ i ] - rawTangents[ i ] * Vector3f.Dot( rawTangents[ i ], rawBiTangents[ i ] ) );
                    rawBiTangents[ i ].Normalize();
                }
                
                vertexes[ i ].Tangent    = new ByteVector3( rawTangents[ i ] );
                
                vertexes[ i ].BiTangentX = rawBiTangents[ i ].X;
                vertexes[ i ].BiTangentY = ByteVector3.ComponentFromFloat( rawBiTangents[ i ].Y );
                vertexes[ i ].BiTangentZ = ByteVector3.ComponentFromFloat( rawBiTangents[ i ].Z );
            }
            
        }

        #endregion

        #endregion

        float LargestVertexMagnitude
        {
            get
            {
                var largest = 0.0f;
                for( int i = 0; i < vCount; i++ )
                {
                    var magnitude = vertexes[ i ].Vertex.Length;
                    if( magnitude > largest ) largest = magnitude;
                }
                return largest;
            }
        }

        #region NIF Writer

        //public bool Write( string targetPath, string targetSuffix, string[] exportInfo )
        public bool Write( string targetPath, string[] exportInfo )
        {
            DebugLog.WriteStrings( null, new[] {
                "targetPath = \"" + targetPath + "\"",
                "nodeGroup.NIFFilePath = \"" + nodeGroup.NIFFilePath( true ) + "\"",
            }, false, true, false, false, true, true );

            if( ( !string.IsNullOrEmpty( targetPath ) )&&( !targetPath.EndsWith( @"\" ) ) )
                targetPath += @"\";

            var fullPath = string.Format( @"{0}{1}", targetPath, nodeGroup.NIFFilePath( true ) );
            GenFilePath.FilenameFromPathname( fullPath, out string nifPath );

            if( string.IsNullOrEmpty( nifPath ) )
            {
                DebugLog.WriteError( "nifPath is null or empty!" );
                return false;
            }
            if( !nifPath.CreatePath() )
            {
                DebugLog.WriteError( string.Format( "Could not build nifPath \"{0}\"", nifPath ) );
                return false;
            }

            var fileStream = new System.IO.FileStream( fullPath, System.IO.FileMode.Create );
            if( fileStream == null )
            {
                DebugLog.WriteError( string.Format( "Could not create FileStream for \"{0}\"", nifPath ) );
                return false;
            }

            var binaryStream = new System.IO.BinaryWriter( fileStream );
            if( binaryStream == null )
            {
                DebugLog.WriteError( string.Format( "Could not create BinaryWriter for \"{0}\"", nifPath ) );
                fileStream.Close();
                return false;
            }

            WriteToStream( binaryStream, exportInfo );

            binaryStream.Flush();
            binaryStream.Close();

            return true;
        }

        void WriteToStream( System.IO.BinaryWriter stream, string[] exportInfo )
        {
            WriteHeader( stream, exportInfo );
            
            WriteNiNode( stream );
            if( meshBlocks > NIFBlockCountEmpty )
            {
                WriteBSXFlags( stream );
                WriteBSTriShape( stream );
                WriteBSEffectShaderProperty( stream );
                WriteBSEffectShaderPropertyFloatController( stream );
                WriteNiFloatInterpolator( stream );
                WriteNiFloatData( stream );
                WriteNiAlphaProperty( stream );
            }
            
            WriteFooter( stream );
        }
        
        static class Nodes
        {
            
            public const string     NiNode                                  = "NiNode";
            public const string     BSXFlags                                = "BSXFlags";
            public const string     BSTriShape                              = "BSTriShape";
            public const string     BSEffectShaderProperty                  = "BSEffectShaderProperty";
            public const string     BSEffectShaderPropertyFloatController   = "BSEffectShaderPropertyFloatController";
            public const string     NiFloatInterpolator                     = "NiFloatInterpolator";
            public const string     NiFloatData                             = "NiFloatData";
            public const string     NiAlphaProperty                         = "NiAlphaProperty";
            
            public const string     UNUSED                                  = "UNUSED";
            
            public static string[]  Blocks = {
                NiNode,
                BSXFlags,
                BSTriShape,
                BSEffectShaderProperty,
                BSEffectShaderPropertyFloatController,
                NiFloatInterpolator,
                NiFloatData,
                NiAlphaProperty };
            
            public static uint[]    Size                                    = { 76, 8, 118, 198, 34, 8, 40, 15 };
            
            public static uint      IndexOf( string name )
            {
                for( uint i = 0; i < Blocks.Length; i++ )
                    if( name == Blocks[ i ] ) return i;
                return 0xFFFFFFFF;
            }
        }
        
        //const uint    NIFVertexSize   = 32;
        uint NIFVertexSize
        {
            get
            {
                return 16 +
                    4 * (
                        fullPrecisionVerts
                        ? sizeof( float )
                        : (uint)Half.SizeOf()
                    );
            }
        }
        const uint    NIFTriangleSize = 6;
        
        #region Header / Footer
        
        const string  NIFIdent        = "Gamebryo File Format, Version 20.2.0.7\n";
        const uint    NIFVersion      = 0x14020007;
        const byte    NIFEndian       = 1;
        const uint    NIFUserVersion  = 12;
        const uint    NIFBlockCountEmpty = 1;
        const uint    NIFBlockCount   = 8;
        const uint    NIFUserVersion2 = 130;

        void WriteHeader( System.IO.BinaryWriter stream, string[] exportInfo )
        {
            string[] NIFStrings = {
                nodeGroup.EditorID,
                "BSX",
                string.Format( "{0}:0", nodeGroup.EditorID ),
                ""
            };
            
            // Base NIF Header
            stream.WriteStringRaw( NIFIdent );
            stream.Write( NIFVersion );
            stream.Write( NIFEndian );
            stream.Write( NIFUserVersion );
            stream.Write( meshBlocks );
            stream.Write( NIFUserVersion2 );
            
            // Write "ExportInfo" strings
            for( int i = 0; i < 4; i++ )
                stream.WriteByteSizedZString( exportInfo[ i ] );
            
            // Block strings and indices
            stream.Write( (ushort)meshBlocks );
            if( meshBlocks > 0 )
            {
                for( int i = 0; i < meshBlocks; i++ )
                    stream.WriteUIntSizedString( Nodes.Blocks[ i ] );
                for( ushort i = 0; i < meshBlocks; i++ )
                    stream.Write( i );
                
                // Block sizes
                var niNodeIndex = Nodes.IndexOf( Nodes.NiNode );
                var triShapeIndex = Nodes.IndexOf( Nodes.BSTriShape );
                for( uint i = 0; i < meshBlocks; i++ )
                {
                    var size = Nodes.Size[ i ];
                    if( i == niNodeIndex )
                        size += (uint)( meshBlocks > NIFBlockCountEmpty ? 8 : 0 );
                    if( i == triShapeIndex )
                        size += (uint)( ( vCount * NIFVertexSize ) + ( tCount * NIFTriangleSize ) );
                    stream.Write( size );
                }
            }
            
            // Strings in file
            stream.Write( (uint)4 );
            stream.Write( NIFStrings.LongestStringLength() );
            for( int i = 0; i < 4; i++ )
                stream.WriteUIntSizedString( NIFStrings[ i ] );
            
            // Zero terminate header
            stream.Write( (uint)0 );
        }
        
        void WriteFooter( System.IO.BinaryWriter stream )
        {
            // Num Roots
            stream.Write( (uint)1 );
            // Root block[s]
            stream.Write( Nodes.IndexOf( Nodes.NiNode ) );
        }
        
        #endregion
        
        #region NiNode
        
        void WriteNiNode( System.IO.BinaryWriter stream )
        {
            var translation = new float[ 3 ];
            var rotation = new float[ 3, 3 ];
            for( int i = 0; i < 3; i++ )
            {
                translation[ i ] = 0.0f;
                for( int j = 0; j < 3; j++ )
                    rotation[ i, j ] = i == j ? 1.0f : 0.0f;
            }
            
            // NiNode Name
            stream.Write( (uint)0 );
            
            // Num Extra Data List
            stream.Write( (uint)( meshBlocks > NIFBlockCountEmpty ? 1 : 0 ) );
            
            // Extra Data List - BSX
            if( meshBlocks > NIFBlockCountEmpty )
                stream.Write( Nodes.IndexOf( Nodes.BSXFlags ) );
            
            // Controller
            stream.Write( Nodes.IndexOf( Nodes.UNUSED ) );
            
            // Flags
            stream.Write( (uint)( meshBlocks > NIFBlockCountEmpty ? 14 : 0 ) );
            
            // Translation
            for( int i = 0; i < 3; i++ )
                stream.Write( translation[ i ] );
            
            // Rotation
            for( int i = 0; i < 3; i++ )
                for( int j = 0; j < 3; j++ )
                    stream.Write( rotation[ i, j ] );
            
            // Scale
            stream.Write( (float)1.0f );
            
            // Collision Object
            stream.Write( Nodes.IndexOf( Nodes.UNUSED ) );
            
            // Num Children
            stream.Write( (uint)( meshBlocks > NIFBlockCountEmpty ? 1 : 0 ) );
            
            // Children - BSTriShape
            if( meshBlocks > NIFBlockCountEmpty )
                stream.Write( Nodes.IndexOf( Nodes.BSTriShape ) );
        }
        
        #endregion
        
        #region BSXFlags
        
        void WriteBSXFlags( System.IO.BinaryWriter stream )
        {
            // BSXFlags Name
            stream.Write( (uint)1 );
            
            // Integer Data
            stream.Write( (uint)1 );
        }
        
        #endregion
        
        #region BSTriShape
        
        void WriteBSTriShape( System.IO.BinaryWriter stream )
        {
            var centre = new float[ 3 ];
            var translation = new float[ 3 ];
            var rotation = new float[ 3, 3 ];
            for( int i = 0; i < 3; i++ )
            {
                centre[ i ] = 0.0f;
                translation[ i ] = 0.0f;
                for( int j = 0; j < 3; j++ )
                    rotation[ i, j ] = i == j ? 1.0f : 0.0f;
            }
            
            // BSTriShape Name
            stream.Write( (uint)2 );
            
            // Num Extra Data List
            stream.Write( (uint)0 );
            
            // Controller
            stream.Write( Nodes.IndexOf( Nodes.UNUSED ) );
            
            // Flags
            stream.Write( (uint)14 );
            
            // Translation
            for( int i = 0; i < 3; i++ )
                stream.Write( translation[ i ] );
            
            // Rotation
            for( int i = 0; i < 3; i++ )
                for( int j = 0; j < 3; j++ )
                    stream.Write( rotation[ i, j ] );
            
            // Scale
            stream.Write( (float)1.0f );
            
            // Collision Object
            stream.Write( Nodes.IndexOf( Nodes.UNUSED ) );
            
            // Bounding Sphere

                // Centre
                for( int i = 0; i < 3; i++ )
                    stream.Write( centre[ i ] );
            
                // Radius
                stream.Write( LargestVertexMagnitude );
            
            // Skin
            stream.Write( Nodes.IndexOf( Nodes.UNUSED ) );
            
            // BS Properties (BSEffectShaderProperty)
            stream.Write( Nodes.IndexOf( Nodes.BSEffectShaderProperty ) );
            
            // BS Properties (NiAlphaProperty)
            stream.Write( Nodes.IndexOf( Nodes.NiAlphaProperty ) );
            
            // Vertex Size (dwords)
            stream.Write( (byte)( NIFVertexSize / sizeof( uint ) ) );
            
            // Float Size
            if( fullPrecisionVerts )
                stream.Write( (byte)( sizeof( float ) ) );
            else
                stream.Write( (byte)( Half.SizeOf() ) );

            // VF3
            stream.Write( (byte)101 );
            
            // VF4
            stream.Write( (byte)7 );
            
            // VF5
            stream.Write( (byte)0 );

            // VF
            if( fullPrecisionVerts )
                stream.Write( (ushort)0x43B0 );
            else
                stream.Write( (ushort)0x03B0 );
            
            // VF8
            stream.Write( (byte)0 );
            
            // Num Triangles
            stream.Write( tCount );
            
            // Num Vertexes
            stream.Write( (ushort)vCount );
            
            // Data size
            stream.Write( (uint)( ( NIFVertexSize * vCount ) + ( NIFTriangleSize * tCount ) ) );
            
            // Vertexes
            for( int i = 0; i < vCount; i++ )
                vertexes[ i ].WriteToStream( stream );
            
            // Triangles
            for( int i = 0; i < tCount; i++ )
                triangles[ i ].WriteToStream( stream );
            
        }
        
        #endregion
        
        #region BSEffectShaderProperty
        
        const string    WorkshopBoundaryTexture     = @"textures\Effects\WorkshopBoundary01_d.dds";
        const string    WorkshopBoundaryGradient    = @"textures\Effects\Gradients\WorkshopBoundaryGrad01.dds";
        
        void WriteBSEffectShaderProperty( System.IO.BinaryWriter stream )
        {
            // BSEffectShaderProperty Name
            stream.Write( (uint)3 );
            
            // Num Extra Data
            stream.Write( (uint)0 );
            
            // Controller
            stream.Write( Nodes.IndexOf( Nodes.BSEffectShaderPropertyFloatController ) );
            
            // Shader Flags
            stream.Write( (uint)0xC0000020 );
            stream.Write( (uint)0x00000020 );
            
            // UV Offset
            stream.Write( 0.0f );
            stream.Write( 0.0f );
            
            // UV Scale
            stream.Write( 20.0f );
            stream.Write( 20.0f );
            
            // Source Texture
            stream.WriteUIntSizedString( WorkshopBoundaryTexture );
            
            // Texture Clamp Mode
            stream.Write( (byte)3 );
            
            // Lighting Influence
            stream.Write( (byte)255 );
            
            // Environment Map Min LOD
            stream.Write( (byte)0 );
            
            // Unk Byte 1
            stream.Write( (byte)3 );
            
            // Falloff Start Angle
            stream.Write( 1.0f );
            
            // Falloff Stop Angle
            stream.Write( 1.0f );
            
            // Falloff Start Opacity
            stream.Write( 0.0f );
            
            // Falloff Stop Opacity
            stream.Write( 0.0f );
            
            // Emmissive Colour
            stream.Write( 1.0f );
            stream.Write( 1.0f );
            stream.Write( 1.0f );
            stream.Write( 1.0f );
            
            // Emmissive Multiple
            stream.Write( 1.0f );
            
            // Soft Falloff Depth
            stream.Write( 128.0f );
            
            // Greyscale Texture
            stream.WriteUIntSizedString( WorkshopBoundaryGradient );
            
            // Environment Map Texture
            stream.WriteUIntSizedString( "" );
            
            // Normal Map Texture
            stream.WriteUIntSizedString( "" );
            
            // Environment Mask Texture
            stream.WriteUIntSizedString( "" );
            
            // Environment Map Scale
            stream.Write( 1.0f );
        }
        
        #endregion
        
        #region BSEffectShaderPropertyFloatController
        
        void WriteBSEffectShaderPropertyFloatController( System.IO.BinaryWriter stream )
        {
            // Next Controller
            stream.Write( Nodes.IndexOf( Nodes.UNUSED ) );
            
            // Flags
            stream.Write( (ushort)72 );
            
            // Frequency
            stream.Write( 1.0f );
            
            // Phase
            stream.Write( 0.0f );
            
            // Start Time
            stream.Write( 0.0f );
            
            // Stop Time
            stream.Write( 1.966667f );
            
            // Target
            stream.Write( Nodes.IndexOf( Nodes.BSEffectShaderProperty ) );
            
            // Interpolator
            stream.Write( Nodes.IndexOf( Nodes.NiFloatInterpolator ) );
            
            // Type of Controlled Variable (U Offset)
            stream.Write( (uint)6 );
        }
        
        #endregion
        
        #region NiFloatInterpolator
        
        void WriteNiFloatInterpolator( System.IO.BinaryWriter stream )
        {
            // Float Value
            stream.Write( float.MinValue );
            
            // Data
            stream.Write( Nodes.IndexOf( Nodes.NiFloatData ) );
        }
        
        #endregion
        
        #region NiFloatData
        
        void WriteNiFloatData( System.IO.BinaryWriter stream )
        {
            // Num Keys
            stream.Write( (uint)2 );
            
            // Interpolation (QUADRATIC_KEY)
            stream.Write( (uint)2 );
            
            // Keys
            
            stream.Write(  0.0f      ); // Key 1 - Time
            stream.Write(  0.0f      ); // Key 1 - Value
            stream.Write(  0.0f      ); // Key 1 - Forward
            stream.Write( -1.0f      ); // Key 1 - Backward
            
            stream.Write(  1.966667f ); // Key 2 - Time
            stream.Write( -1.0f      ); // Key 2 - Value
            stream.Write( -1.0f      ); // Key 2 - Forward
            stream.Write(  0.0f      ); // Key 2 - Backward
        }
        
        #endregion
        
        #region NiAlphaProperty
        
        void WriteNiAlphaProperty( System.IO.BinaryWriter stream )
        {
            // Node Name
            stream.Write( (uint)3 );
            
            // Num Extra Data List
            stream.Write( (uint)0 );
            
            // Controller
            stream.Write( Nodes.IndexOf( Nodes.UNUSED ) );
            
            // Flags
            stream.Write( (ushort)4109 );
            
            // Threshold
            stream.Write( (byte)64 );
        }
        
        #endregion
        
        #endregion
        
    }
    
}
