/*
 * ConflictStatusHelpers.cs
 *
 * Helper functions for ConflictStatus.
 *
 */
using System;
using System.Drawing;

using Engine.Plugin;

/// <summary>
/// Description of ConflictStatusHelpers.
/// </summary>
public static class ConflictStatusHelpers
{
    
    // TODO: Make this user configurable
    public static Color GetConflictStatusBackColor( this ConflictStatus conflictStatus )
    {
        switch( conflictStatus  )
        {
            case ConflictStatus.Uneditable:
                return Color.Gray;
            case ConflictStatus.NewForm:
                return Color.White;
            case ConflictStatus.NoConflict:
                return Color.Lime;
            case ConflictStatus.OverrideInAncestor:
                return Color.YellowGreen;
            case ConflictStatus.OverrideInWorkingFile:
                return Color.Yellow;
            case ConflictStatus.OverrideInPostLoad:
                return Color.DeepPink;
            case ConflictStatus.RequiresOverride:
                return Color.Cyan;
        }
        // Invalid or otherwise unknown/unhandled
        return Color.Red;
    }
    
}
