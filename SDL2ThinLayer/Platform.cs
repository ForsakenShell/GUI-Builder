/*
 * Platform.cs
 *
 * Some simple system non-specific platform enumerations.
 *
 * User: 1000101
 * Date: 27/01/2018
 * Time: 4:21 AM
 * 
 */

using System;

public static class Platform
{
    
    /// <summary>
    /// Currently running on a 64-bit platform?
    /// </summary>
    public static bool Is64Bit { get { return IntPtr.Size == 8; } }
    
}
