/*
 * NoBSFileAccess.cs
 * 
 * Native API wrapper functions to load directly from file to a buffer without .Nets load-copy-copy-copy-copy-copy-etc-etc bullshit.
 * 
 */
using System;
using System.IO;
using System.Runtime.InteropServices;

/// <summary>
/// Description of NoBSFileAccess.
/// </summary>
unsafe public static class NoBSFileAccess
{
    // Notes:  Microsoft.Win32.SafeHandles.SafeFileHandle must be explicitly referenced as the compiler likes to be retarded and reference the System.IO.FileStream.SafeFileHandle property as a type
    
    [DllImport("kernel32.dll")]
    static extern bool ReadFile( Microsoft.Win32.SafeHandles.SafeFileHandle handle, void* buffer, uint numBytesToRead, out uint numBytesRead, IntPtr overlapped );
    
    [DllImport("kernel32.dll")]
    static extern bool SetFilePointerEx( Microsoft.Win32.SafeHandles.SafeFileHandle hFile, long liDistanceToMove, out long lpNewFilePointer, uint dwMoveMethod );
    
    
    unsafe public static bool Read( FileStream fs, void* buffer, uint bytesToRead )
    {
        long unused;
        long filePos = fs.Position;
        Microsoft.Win32.SafeHandles.SafeFileHandle nativeHandle = fs.SafeFileHandle; // clears Position property
        if( !SetFilePointerEx( nativeHandle, filePos, out unused, 0 ) ) // so reset the native pointer
            throw new Exception( "SetFilePointerEx" );
        uint bytesRead;
        bool rVal = ReadFile( nativeHandle, buffer, bytesToRead, out bytesRead, IntPtr.Zero );
        return rVal;
    }
    
    unsafe public static bool Read( FileStream fs, long offsetInFile, void* buffer, uint bytesToRead )
    {
        long unused;
        Microsoft.Win32.SafeHandles.SafeFileHandle nativeHandle = fs.SafeFileHandle; // clears Position property
        if( !SetFilePointerEx( nativeHandle, offsetInFile, out unused, 0 ) ) // doesn't matter, setting it explicitly anyway
            throw new Exception( "SetFilePointerEx" );
        uint bytesRead;
        bool rVal = ReadFile( nativeHandle, buffer, bytesToRead, out bytesRead, IntPtr.Zero );
        return rVal;
    }
    
    
    // TODO:  Make writer[s]
    
}
