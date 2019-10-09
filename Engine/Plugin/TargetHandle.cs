/*
 * TargetHandle.cs
 *
 * Target for field getters and setters.  Not an actual handle but an enum of valid targets which are resolved
 * in the getter and setter from the base file/form/script.
 *
 */

using System;

namespace Engine.Plugin
{
    
    public enum TargetHandle
    {
        Master,
        LastFullRequired,
        Working,
        WorkingOrLastFullRequired, // Will probably need to change many functions to refer to this as a safety catch
        LastFullOptional
    }
    
}
