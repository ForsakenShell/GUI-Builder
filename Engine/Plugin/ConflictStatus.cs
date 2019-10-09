/*
 * ConflictStatus.cs
 *
 * GUIBuilder Form conflict status, not to be confused with the XeLib record conflicts.
 *
 */


namespace Engine.Plugin
{
    
    public enum ConflictStatus
    {
        Invalid,
        Uneditable,
        NewForm,
        NoConflict,
        OverrideInAncestor,
        OverrideInWorkingFile,
        OverrideInPostLoad,
        RequiresOverride
    }
    
}
