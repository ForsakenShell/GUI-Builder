/*
 * WinAPI.cs
 *
 * Native Windows API calls
 *
 * Could reference Microsoft.Crm.UnifiedServiceDesk.Dynamics.dll and get all this from that, but
 * who needs another dependency in the chain, especially such a stupid one?
 *
 * User: 1000101
 * Date: 27/01/2018
 * Time: 4:18 AM
 * 
 */

using System;
using System.Runtime.InteropServices;

/// <summary>
/// Description of WinAPI.
/// </summary>
public static class WinAPI
{
    
    public enum WindowLongIndex : int
    {
        GWL_EXSTYLE     = -20,
        GWLP_HINSTANCE  =  -6,
        GWLP_HWNDPARENT =  -8,
        GWL_ID          = -12,
        GWL_STYLE       = -16,
        GWL_USERDATA    = -21,
        GWL_WNDPROC     =  -4,
        DWLP_USER       = 0x8,
        DWLP_MSGRESULT  = 0x0,
        DWLP_DLGPROC    = 0x4
    }
    
    [Flags]
    public enum WindowStyleFlags : uint
    {
        WS_BORDER           = 0x00800000,
        WS_CAPTION          = 0x00C00000,
        WS_CHILD            = 0x40000000,
        WS_CHILDWINDOW      = WS_CHILD,
        WS_CLIPCHILDREN     = 0x02000000,
        WS_CLIPSIBLINGS     = 0x04000000,
        WS_DISABLED         = 0x08000000,
        WS_DLGFRAME         = 0x00400000,
        WS_GROUP            = 0x00020000,
        WS_HSCROLL          = 0x00100000,
        WS_ICONIC           = 0x20000000,
        WS_MAXIMIZE         = 0x01000000,
        WS_MAXIMIZEBOX      = 0x00010000,
        WS_MINIMIZE         = 0x20000000,
        WS_MINIMIZEBOX      = 0x00020000,
        WS_OVERLAPPED       = 0x00000000,
        WS_OVERLAPPEDWINDOW = ( WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX ),
        WS_POPUP            = 0x80000000,
        WS_POPUPWINDOW      = ( WS_POPUP | WS_BORDER | WS_SYSMENU ),
        WS_SIZEBOX          = 0x00040000,
        WS_SYSMENU          = 0x00080000,
        WS_TABSTOP          = 0x00010000,
        WS_THICKFRAME       = 0x00040000,
        WS_TILED            = 0x00000000,
        WS_TILEDWINDOW      = ( WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX ),
        WS_VISIBLE          = 0x10000000,
        WS_VSCROLL          = 0x00200000,
        
        // See: https://msdn.microsoft.com/en-us/library/windows/desktop/ff700543(v=vs.85).aspx
        WS_EX_ACCEPTFILES   = 0x00000010,
        WS_EX_APPWINDOW     = 0x00040000,
        WS_EX_CLIENTEDGE    = 0x00000200,
        WS_EX_COMPOSITED    = 0x02000000,
        WS_EX_CONTEXTHELP   = 0x00000400,
        WS_EX_CONTROLPARENT = 0x00010000,
        WS_EX_DLGMODALFRAME = 0x00000001,
        WS_EX_LAYERED       = 0x00080000,
        WS_EX_LAYOUTRTL     = 0x00400000,
        WS_EX_LEFT          = 0x00000000,
        WS_EX_LEFTSCROLLBAR = 0x00004000,
        WS_EX_LTRREADING    = 0x00000000,
        WS_EX_MDICHILD      = 0x00000040,
        WS_EX_NOACTIVATE    = 0x08000000,
        WS_EX_NOINHERITLAYOUT = 0x00100000,
        WS_EX_NOPARENTNOTIFY = 0x00000004,
        WS_EX_NOREDIRECTIONBITMAP = 0x00200000,
        WS_EX_OVERLAPPEDWINDOW = ( WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE ),
        WS_EX_PALETTEWINDOW = ( WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST ),
        WS_EX_RIGHT         = 0x00001000,
        WS_EX_RIGHTSCROLLBAR = 0x00000000,
        WS_EX_RTLREADING    = 0x00002000,
        WS_EX_STATICEDGE    = 0x00020000,
        WS_EX_TOOLWINDOW    = 0x00000080,
        WS_EX_TOPMOST       = 0x00000008,
        WS_EX_TRANSPARENT   = 0x00000020,
        WS_EX_WINDOWEDGE    = 0x00000100
    }
    
    [Flags]
    public enum WindowSWPFlags : uint
    {
        SWP_ASYNCWINDOWPOS  = 0x4000,
        SWP_DEFERERASE      = 0x2000,
        SWP_DRAWFRAME       = 0x0020,
        SWP_FRAMECHANGED    = 0x0020,
        SWP_HIDEWINDOW      = 0x0080,
        SWP_NOACTIVATE      = 0x0010,
        SWP_NOCOPYBITS      = 0x0100,
        SWP_NOMOVE          = 0x0002,
        SWP_NOOWNERZORDER   = 0x0200,
        SWP_NOREDRAW        = 0x0008,
        SWP_NOREPOSITION    = 0x0200,
        SWP_NOSENDCHANGING  = 0x0400,
        SWP_NOSIZE          = 0x0001,
        SWP_NOZORDER        = 0x0004,
        SWP_SHOWWINDOW      = 0x0040
    }
    
    public enum ShowWindowFlags : int
    {
        SW_FORCEMINIMIZE    = 11,
        SW_HIDE             = 0,
        SW_MAXIMIZE         = 3,
        SW_MINIMIZE         = 6,
        SW_RESTORE          = 9,
        SW_SHOW             = 5,
        SW_SHOWDEFAULT      = 10,
        SW_SHOWMAXIMIZED    = 3,
        SW_SHOWMINIMIZED    = 2,
        SW_SHOWMINNOACTIVE  = 7,
        SW_SHOWNA           = 8,
        SW_SHOWNOACTIVATE   = 4,
        SW_SHOWNORMAL       = 1
    }
    
    #region Internal 32/64 bit WinAPI entry points
    
    // 32-bit platform calls
    [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
    static extern uint GetWindowLongPtr32( IntPtr handle, WindowLongIndex nIndex );
    [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
    static extern uint SetWindowLongPtr32( IntPtr handle, WindowLongIndex nIndex, uint newLong );
    
    // 64-bit platform calls
    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
    static extern IntPtr GetWindowLongPtr64( IntPtr handle, WindowLongIndex nIndex );
    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
    static extern IntPtr SetWindowLongPtr64( IntPtr handle, WindowLongIndex nIndex, IntPtr newLong );
    
    #endregion
    
    #region Public 32/64-bit Platform Abstraction
    
    public static uint GetWindowLongPtr( IntPtr handle, WindowLongIndex nIndex )
    {
        //var rVal = (uint)( Platform.Is64Bit ? (uint)GetWindowLongPtr64( handle, nIndex ).ToInt32() : GetWindowLongPtr32( handle, nIndex ) );
        //return rVal;
        return (uint)GetWindowLongPtr32( handle, nIndex );
    }
    
    public unsafe static uint SetWindowLongPtr( IntPtr handle, WindowLongIndex nIndex, uint newLong )
    {
        //var rVal = (uint)( Platform.Is64Bit ? (uint)SetWindowLongPtr64( handle, nIndex, new IntPtr( &newLong ) ).ToInt32() : SetWindowLongPtr32( handle, nIndex, newLong ) );
        //return rVal;
        return (uint)SetWindowLongPtr32( handle, nIndex, newLong );
    }
    
    [DllImport("user32.dll")]
    public static extern IntPtr SetWindowPos(
        IntPtr handle,
        IntPtr handleAfter,
        int x,
        int y,
        int cx,
        int cy,
        WindowSWPFlags flags
    );
    
    [DllImport("user32.dll")]
    public static extern IntPtr SetParent( IntPtr child, IntPtr newParent );
    
    [DllImport("user32.dll")]
    public static extern IntPtr ShowWindow( IntPtr handle, ShowWindowFlags command );
    
    #endregion
    
}
