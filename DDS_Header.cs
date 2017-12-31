/*
 * DDS_Header.cs
 * 
 * DDS_Header and DDS_PixelFormat structs as they appear in a DDS file. 
 * 
 * User: 1000101
 * Date: 27/11/2017
 * Time: 4:46 PM
 * 
 */
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Border_Builder
{
    
    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct DDS_PixelFormat
    {
        public UInt32
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
        public UInt32
            fileID,
            dwSize,
            dwFlags,
            dwHeight,
            dwWidth,
            dwPitchOrLinearSize,
            dwDepth,
            dwMipMapCount;
        public fixed UInt32 dwReserved1[11];
        public DDS_PixelFormat ddspf;
        public UInt32
            dwCaps,
            dwCaps2,
            dwCaps3,
            dwCaps4,
            dwReserved2;
        
        public bool FileIDOk
        {
            get
            {
                // fileID = "DDS "?
                return fileID == 0x20534444;
            }
        }
    }
}
