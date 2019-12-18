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

public static partial class NIFBuilder
{
    /// <summary>
    /// Description of Mesh.
    /// </summary>
    public class Mesh
    {
        
        public class MeshVertex
        {
            public Vector3f         Vertex;
            public float            BiTangentX;
            public HalfVector2f     UV;
            public ByteVector3      Normal;
            public byte             BiTangentY;
            public ByteVector3      Tangent;
            public byte             BiTangentZ;
            public uint           Colour;
            
            public MeshVertex( Vector3f vertex, Vector2f uv, uint colour )
            {
                Vertex  = new Vector3f( vertex );
                UV      = new HalfVector2f( uv );
                Colour  = colour;
            }
            
            public void WriteToStream( System.IO.BinaryWriter stream )
            {
                Vertex.WriteToStream( stream );
                stream.Write( BiTangentX );
                UV.WriteToStream( stream );
                Normal.WriteToStream( stream );
                stream.Write( BiTangentY );
                Tangent.WriteToStream( stream );
                stream.Write( BiTangentZ );
                stream.Write( Colour );
            }
            
        }
        
        public class MeshTriangle
        {
            public ushort           p1, p2, p3;
            
            Mesh            parent;
            
            public                  MeshTriangle( Mesh mesh, int v1, int v2, int v3 )
            {
                parent = mesh;
                p1 = (ushort)v1;
                p2 = (ushort)v2;
                p3 = (ushort)v3;
            }
            
            Vector3f        RawVertNorm( ushort index )
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
            
            public void WriteToStream( System.IO.BinaryWriter stream )
            {
                stream.Write( p1 );
                stream.Write( p2 );
                stream.Write( p3 );
            }
            
        }
        
        string              nifFilePath;
        public string               nifFile;
        //string              nifPath;
        
        int                 nCount, vCount, tCount;
        BorderNodeGroup     nodeGroup;
        uint[]            iColours;
        uint[]            oColours;
        
        float               gHeight, gOffset, gSink;
        
        Vector3f[]          rawVerts;
        Vector3f[]          rawNormals;
        Vector3f[]          rawTangents;
        Vector3f[]          rawBiTangents;
        
        MeshVertex[]        vertexes;
        MeshTriangle[]      triangles;
        
        uint              meshBlocks;
        
        static int CalculateValidMeshVertexOffsets( List<BorderNode> nodes, float gOffset, float gSink )
        {
            if( nodes == null ) return 0;
            int result = 0;
            var j = nodes.Count;
            if( j < 2 ) return 0;
            for( int i = 0; i < j; i++ )
            {
                var cNode = nodes[ i ];
                var verts = cNode.FloorVertexMatchesGroundVertex( gOffset, gSink )
                    ? 2
                    : 3;
                
                cNode.iVerts = new int[ verts ];
                for( int k = 0; k < verts; k++ )
                    cNode.iVerts[ k ] = result + k;
                result += verts;
                
                cNode.oVerts = new int[ verts ];
                for( int k = 0; k < verts; k++ )
                    cNode.oVerts[ k ] = result + k;
                result += verts;
                
                cNode.verts = verts;
                
                //DebugLog.Write( string.Format( "CalculateValidMeshVertexOffsets() :: {0}/{1} :: {2} :: {3} :: {4} :: {5}", i, j, verts, result, gOffset, gSink ) );
            }
            return result;
        }
        
        static int CalculateValidMeshTriangleOffsets( List<BorderNode> nodes )
        {
            if( nodes == null ) return 0;
            int result = 0;
            var j = nodes.Count;
            if( j < 2 ) return 0;
            for( int i = 0; i < j; i++ )
            {
                var cNode = nodes[ i ];
                if( cNode.Type != BorderNode.NodeType.EndPoint )
                {
                    var n = i >= j - 1 ? i - j: i; n++;
                    var nNode = nodes[ n ];
                    var tnHasFloor = cNode.verts == 3;
                    var nnHasFloor = nNode.verts == 3;
                    var tris = tnHasFloor
                        ? nnHasFloor
                            ? 4
                            : 3
                        : nnHasFloor
                            ? 3
                            : 2;
                    
                    result += tris << 1; // * 2 for inside + outside
                    //DebugLog.Write( string.Format( "CalculateValidMeshTriangleOffsets() :: {0}/{1} :: {2} :: {3}", i, j, tris, result ) );
                }
            }
            return result;
        }
        
        public Mesh( BorderNodeGroup group, float gradientHeight, float groundOffset, float groundSink, uint[] insideColours, uint[] outsideColours )
        {
            //DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "cTor()", "Nodes:" } );
            //for( int i = 0; i < group.Nodes.Count; i++ )
            //    DebugLog.WriteLine( "node[ " + i.ToString() + " ] = " + group.Nodes[ i ].ToString() );
            //DebugLog.CloseIndentLevel();
            
            nodeGroup         = group;
            
            nifFilePath       = nodeGroup.NIFFilePath;
            string workingPath;
            var working = GenFilePath.FilenameFromPathname( nifFilePath, out workingPath );//, out nifPath );
            if( working.EndsWith( ".nif", StringComparison.InvariantCultureIgnoreCase ) )
                nifFile = working.Substring( 0, working.Length - 4 );
            else
                nifFile = working;
            
            nCount            = nodeGroup == null ? 0 : nodeGroup.Nodes.NullOrEmpty() ? 0 : nodeGroup.Nodes.Count;
            gHeight           = Math.Max( 0.0f, gradientHeight );
            gOffset           = Math.Max( 0.0f, groundOffset );
            gSink             = Math.Max( 0.0f, groundSink );
            
            iColours          = new uint[ 3 ];
            oColours          = new uint[ 3 ];
            for( int i = 0; i < 3; i++ )
            {
                iColours[ i ] = insideColours[ i ];
                oColours[ i ] = outsideColours[ i ];
            }
            
            if( ( nCount >= 2 )&&( gHeight > 0.0f ) )
            {
                vCount            = CalculateValidMeshVertexOffsets( nodeGroup.Nodes, groundOffset, groundSink );
                tCount            = CalculateValidMeshTriangleOffsets( nodeGroup.Nodes );
                
                //DebugLog.Write( string.Format( "NIFBuilder.Mesh.cTor() :: {0}:{1}:{2} :: \"{3}\" :: \"{4}\"", nCount, vCount, tCount, nifPath, nifFile ) );
                
                rawVerts          = new Vector3f[ vCount ];
                rawNormals        = new Vector3f[ vCount ];
                rawTangents       = new Vector3f[ vCount ];
                rawBiTangents     = new Vector3f[ vCount ];
                
                vertexes          = new MeshVertex[ vCount ];
                triangles         = new MeshTriangle[ tCount ];
                
                BuildVertexArray();
                BuildTriangleArray();
                CalculateVertexNormals();
                CalculateVertexTangents();
                
            }
            
            meshBlocks      = ( vCount > 0 )&&( tCount > 0 ) ? NIFBlockCount : NIFBlockCountEmpty;
            
            //DebugLog.Write( string.Format( "{0} :: {1} :: {2}", this.GetType().ToString(), nifPath, nifFile ) );
        }
        
        public static string MeshSubSetIndex( string neighbour = null, int subIndex = -1 )
        {
            var sSubIndex = subIndex > -1
                ? string.Format( "_{0}", subIndex.ToString( "X2" ) )
                : null;
            return
                !string.IsNullOrEmpty( neighbour )
                    ? string.Format( "{0}{1}", neighbour, sSubIndex )
                    : !string.IsNullOrEmpty( sSubIndex )
                        ? sSubIndex
                        : null;
        }
        
        public static List<string> MatchKeys( string location, string neighbour = null, int borderIndex = 1, int subIndex = -1 )
        {
            var keys = new List<string>{ location };
            var sBorderIndex = borderIndex > -1
                ? borderIndex.ToString( "X2" )
                : null;
            if( !string.IsNullOrEmpty( sBorderIndex ) )
                keys.Add( sBorderIndex );
            var mssi = MeshSubSetIndex( neighbour, subIndex );
            if( !string.IsNullOrEmpty( mssi ) )
                keys.Add( mssi );
            return keys;
        }
        
        public static string BuildFullFilePath( string target, string targetSuffix, string meshPathSuffix, string meshSubPath, string filePrefix, string location, string fileSuffix = "", int borderIndex = 1, string neighbour = "", int subIndex = -1 )
        {
            if( ( !string.IsNullOrEmpty( target ) )&&( target[ target.Length - 1 ] != '\\' ) )
                target += @"\";
            if( ( !string.IsNullOrEmpty( targetSuffix ) )&&( targetSuffix[ targetSuffix.Length - 1 ] != '\\' ) )
                targetSuffix += @"\";
            return string.Format(
                @"{0}{1}",
                BuildTargetPath(
                    target,
                    targetSuffix ),
                BuildFilePath(
                    meshPathSuffix,
                    meshSubPath,
                    filePrefix,
                    location,
                    fileSuffix,
                    borderIndex,
                    neighbour,
                    subIndex )
            );
        }
        public static string BuildTargetPath( string target, string targetSuffix )
        {
            if( ( !string.IsNullOrEmpty( target ) )&&( target[ target.Length - 1 ] != '\\' ) )
                target += @"\";
            if( ( !string.IsNullOrEmpty( targetSuffix ) )&&( targetSuffix[ targetSuffix.Length - 1 ] != '\\' ) )
                targetSuffix += @"\";
            return string.Format(
                @"{0}{1}",
                target,
                targetSuffix
            );
        }
        public static string BuildFilePath( string meshPathSuffix, string meshSubPath, string filePrefix, string location, string fileSuffix = "", int borderIndex = 1, string neighbour = "", int subIndex = -1 )
        {
            if( ( !string.IsNullOrEmpty( meshPathSuffix ) )&&( meshPathSuffix[ meshPathSuffix.Length - 1 ] != '\\' ) )
                meshPathSuffix += @"\";
            if( ( !string.IsNullOrEmpty( meshSubPath ) )&&( meshSubPath[ meshSubPath.Length - 1 ] != '\\' ) )
                meshSubPath += @"\";
            var sBorderIndex = borderIndex > -1
                ? borderIndex.ToString( "X2" )
                : null;
            var mssi = MeshSubSetIndex( neighbour, subIndex );
            return string.Format(
                @"Meshes\{0}{1}{2}{3}{4}{5}{6}.nif",
                meshPathSuffix,
                meshSubPath,
                filePrefix,
                location,
                fileSuffix,
                sBorderIndex,
                mssi );
        }
        
        public bool Write( string targetPath, string targetSuffix )
        {
            //DebugLog.Write( string.Format( "NIFBuilder.Mesh.Write()\n\tNIFFile = \"{0}\"\n\tCell = {1}\n\tPosition = {2}\n", nifFile, nodeGroup.Cell.ToString(), nodeGroup.Placement.ToString() ) );
            
            var fullTarget = BuildTargetPath( targetPath, targetSuffix );
            if( ( !string.IsNullOrEmpty( fullTarget ) )&&( fullTarget[ fullTarget.Length - 1 ] != '\\' ) )
                fullTarget += @"\";
            var fullPath = string.Format( "{0}{1}", fullTarget, nifFilePath );
            string nifPath;
            GenFilePath.FilenameFromPathname( fullPath, out nifPath );
            
            if( string.IsNullOrEmpty( nifPath ) )
            {
                DebugLog.WriteError( "NIFBuilder.Mesh", "Write()", "nifPath is null or empty!" );
                return false;
            }
            if( !nifPath.CreatePath() )
            {
                DebugLog.WriteError( "NIFBuilder.Mesh", "Write()", string.Format( "Could not build nifPath \"{0}\"", nifPath ) );
                return false;
            }

            var fileStream = new System.IO.FileStream( fullPath, System.IO.FileMode.Create );
            if( fileStream == null )
            {
                DebugLog.WriteError( "NIFBuilder.Mesh", "Write()", string.Format( "Could not create FileStream for \"{0}\"", nifPath ) );
                return false;
            }
            
            var binaryStream = new System.IO.BinaryWriter( fileStream );
            if( binaryStream == null )
            {
                DebugLog.WriteError( "NIFBuilder.Mesh", "Write()", string.Format( "Could not create BinaryWriter for \"{0}\"", nifPath ) );
                fileStream.Close();
                return false;
            }
            
            WriteToStream( binaryStream );
            
            binaryStream.Flush();
            binaryStream.Close();
            
            return true;
        }
        
        #region Build Mesh
        
        void AssignRawVert( ref int i, Vector3f p, Vector2f uv, uint c )
        {
            try
            {
                rawVerts[ i ] = new Vector3f( p );
                vertexes[ i ] = new MeshVertex( p, uv, c );
                i++;
            }
            catch( Exception e )
            {
                DebugLog.WriteError( this.GetType().ToString(), "AssignRawVert()", string.Format( "Exception assigning raw vert\n\tindex = {0}\n\tvertexes = {1}\n\trawVerts = {2}\n\nException:\n{3}", i, vertexes.Length, rawVerts.Length, e.ToString() ) );
            }
        }
        
        void AssignRawTri( ref int i, int p1, int p2, int p3 )
        {
            try
            {
                triangles[ i ] = new MeshTriangle( this, p1, p2, p3 );
                i++;
            }
            catch( Exception e )
            {
                DebugLog.WriteError( this.GetType().ToString(), "AssignRawTri()", string.Format( "Exception assigning raw tri\n\tindex = {0}\n\ttriangles = {1}\n\nException:\n{2}", i, triangles.Length, e.ToString() ) );
            }
        }
        
        void BuildVertexArray()
        {
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
                if( node.verts == 3 )
                    AssignRawVert( ref vIndex, vF, uv[ 2 ], iColours[ 2 ] );    // Inside Floor
                
                AssignRawVert( ref vIndex, vT, uv[ 0 ], oColours[ 0 ] );        // Outside Top
                AssignRawVert( ref vIndex, vG, uv[ 1 ], oColours[ 1 ] );        // Outside Ground
                if( node.verts == 3 )
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
        }
        
        void BuildTriangleArray()
        {
            int tIndex = 0;
            for( int i = 0; i < nCount; i++ )
            {
                var cNode = nodeGroup.Nodes[ i ];
                if( cNode.Type != BorderNode.NodeType.EndPoint )
                {
                    var n = i >= nCount - 1 ? i - nCount: i; n++;
                    var nNode = nodeGroup.Nodes[ n ];
                    
                    //DebugLog.Write( string.Format( "BuildTriangleArray() :: {0}/{1} :: {2}/{3}", i, nCount, tIndex, tCount ) );
                    
                    AssignRawTri( ref tIndex, cNode.iVerts[ 0 ], cNode.iVerts[ 1 ], nNode.iVerts[ 0 ] );           // Top Inside
                    AssignRawTri( ref tIndex, cNode.iVerts[ 1 ], nNode.iVerts[ 1 ], nNode.iVerts[ 0 ] );           // Top Inside
                    if( cNode.verts == 3 )
                    {
                        AssignRawTri( ref tIndex, cNode.iVerts[ 1 ], cNode.iVerts[ 2 ], nNode.iVerts[ 1 ] );       // Bottom Inside
                        if( nNode.verts == 3 )
                            AssignRawTri( ref tIndex, cNode.iVerts[ 2 ], nNode.iVerts[ 2 ], nNode.iVerts[ 1 ] );   // Bottom Inside
                    }
                    else if( nNode.verts == 3 )
                        AssignRawTri( ref tIndex, cNode.iVerts[ 1 ], nNode.iVerts[ 2 ], nNode.iVerts[ 1 ] );       // Bottom Inside
                    
                    AssignRawTri( ref tIndex, cNode.oVerts[ 1 ], cNode.oVerts[ 0 ], nNode.oVerts[ 0 ] );           // Top Outside
                    AssignRawTri( ref tIndex, cNode.oVerts[ 1 ], nNode.oVerts[ 0 ], nNode.oVerts[ 1 ] );           // Top Outside
                    if( cNode.verts == 3 )
                    {
                        AssignRawTri( ref tIndex, cNode.oVerts[ 2 ], cNode.oVerts[ 1 ], nNode.oVerts[ 1 ] );       // Bottom Outside
                        if( nNode.verts == 3 )
                            AssignRawTri( ref tIndex, cNode.oVerts[ 2 ], nNode.oVerts[ 1 ], nNode.oVerts[ 2 ] );   // Bottom Outside
                    }
                    else if( nNode.verts == 3 )
                        AssignRawTri( ref tIndex, cNode.oVerts[ 1 ], nNode.oVerts[ 1 ], nNode.oVerts[ 2 ] );       // Bottom Outside
                }
            }
            /*
            int vIndex = 0;
            for( int tIndex = 0; tIndex < tCount; tIndex += 8 )
            {
                // Inside triangle vertex indicies
                triangles[ tIndex + 0 ] = new MeshTriangle( this, vIndex     + 6, vIndex        , vIndex + 1     );
                triangles[ tIndex + 1 ] = new MeshTriangle( this, vIndex     + 6, vIndex + 1    , vIndex + 1 + 6 );
                triangles[ tIndex + 2 ] = new MeshTriangle( this, vIndex + 1 + 6, vIndex + 1    , vIndex + 2     );
                triangles[ tIndex + 3 ] = new MeshTriangle( this, vIndex + 1 + 6, vIndex + 2    , vIndex + 2 + 6 );
                
                // Outside triangle vertex indicies
                triangles[ tIndex + 4 ] = new MeshTriangle( this, vIndex + 3    , vIndex + 3 + 6, vIndex + 4     );
                triangles[ tIndex + 5 ] = new MeshTriangle( this, vIndex + 3 + 6, vIndex + 4 + 6, vIndex + 4     );
                triangles[ tIndex + 6 ] = new MeshTriangle( this, vIndex + 4    , vIndex + 4 + 6, vIndex + 5     );
                triangles[ tIndex + 7 ] = new MeshTriangle( this, vIndex + 4 + 6, vIndex + 5 + 6, vIndex + 5     );
                
                vIndex += 6;
            }
            */
        }
        
        void CalculateVertexNormals()
        {
            var norms = new Vector3f[ vCount ];
            
            // Compute triangle norms
            for( int i = 0; i < tCount; i++ )
            {
                var tri = triangles[ i ];
                var triNorm = tri.Normal();
                
                norms[ tri.p1 ] += triNorm;
                norms[ tri.p2 ] += triNorm;
                norms[ tri.p3 ] += triNorm;
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
        
        void WriteToStream( System.IO.BinaryWriter stream )
        {
            WriteHeader( stream );
            
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
            
            public static uint[]  Size                                    = { 76, 8, 118, 198, 34, 8, 40, 15 };
            
            public static uint IndexOf( string name )
            {
                for( uint i = 0; i < Blocks.Length; i++ )
                    if( name == Blocks[ i ] ) return i;
                return 0xFFFFFFFF;
            }
        }
        
        const uint    NIFVertexSize   = 32;
        const uint    NIFTriangleSize = 6;
        
        #region Header / Footer
        
        const string    NIFIdent        = "Gamebryo File Format, Version 20.2.0.7\n";
        const uint    NIFVersion      = 0x14020007;
        const byte      NIFEndian       = 1;
        const uint    NIFUserVersion  = 12;
        const uint    NIFBlockCountEmpty = 1;
        const uint    NIFBlockCount   = 8;
        const uint    NIFUserVersion2 = 130;
        
        void WriteHeader( System.IO.BinaryWriter stream )
        {
            string[] NIFStrings = {
                nifFile,
                "BSX",
                string.Format( "{0}:0", nifFile ),
                ""
            };
            
            string[] ExportInfo = {
                string.Format( "GUIBuilder {0} by 1000101", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() ),
                "https://www.nexusmods.com/users/106891",
                " PE Static Art",
                "STAHP LOOKING AT ME!"
            };
            
            // Base NIF Header
            stream.WriteStringRaw( NIFIdent );
            stream.Write( NIFVersion );
            stream.Write( NIFEndian );
            stream.Write( NIFUserVersion );
            stream.Write( meshBlocks );
            stream.Write( NIFUserVersion2 );
            
            // User export strings
            for( int i = 0; i < 4; i++ )
                stream.WriteByteSizedZString( ExportInfo[ i ] );
            
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
            stream.Write( NIFStrings.LongestStringSizeInArray() );
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
            stream.Write( (byte)( sizeof( float ) ) );
            
            // VF3
            stream.Write( (byte)101 );
            
            // VF4
            stream.Write( (byte)7 );
            
            // VF5
            stream.Write( (byte)0 );
            
            // VF
            stream.Write( (ushort)0x43B0 );
            
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
