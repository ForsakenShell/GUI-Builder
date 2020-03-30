/*
 * ScriptAssociation.cs
 *
 * Attribute to abstract and associate papyrus scripts with the C# class to handle them.
 *
 */

using System;


namespace Engine.Plugin.Attributes
{
    
    [AttributeUsage(AttributeTargets.Class)]
    public class ScriptAssociation : ClassAssociation
    {
        
        public                          ScriptAssociation( string signature )
            : base( signature, signature ) { }
        
    }
    
}
