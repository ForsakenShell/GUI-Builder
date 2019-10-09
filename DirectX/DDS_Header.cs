/*
 * DDS_Header.cs
 * 
 * DDS_Header and DDS_PixelFormat structs as they appear in a DDS file. 
 * 
 */
using System;
using System.Runtime.InteropServices;


namespace DirectX
{
    
    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct DDS_PixelFormat
    {
        public uint
            dwSize,
            dwFlags,
            dwFourCC,
            dwRGBBitCount,
            dwRBitMask,
            dwGBitMask,
            dwBBitMask,
            dwABitMask;
    }
    
    /// <summary>
    /// Description of DDS_Header.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack=1)]
    unsafe public struct DDS_Header
    {
        
        const uint DDS_MAGIC_ID = 0x20534444;   // "DDS "
        
        public uint
            fileID,
            dwSize,
            dwFlags,
            dwHeight,
            dwWidth,
            dwPitchOrLinearSize,
            dwDepth,
            dwMipMapCount;
        public fixed uint dwReserved1[11];
        public DDS_PixelFormat ddspf;
        public uint
            dwCaps,
            dwCaps2,
            dwCaps3,
            dwCaps4,
            dwReserved2;
        
        public bool FileIDOk        { get { return fileID == DDS_MAGIC_ID; } }
        
    }
    
}
