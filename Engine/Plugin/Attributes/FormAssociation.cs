/*
 * FormAssociation.cs
 *
 * Attribute to abstract and associate forms with the C# class to handle them.
 *
 */

using System;


namespace Engine.Plugin.Attributes
{
    
    [AttributeUsage(AttributeTargets.Class)]
    public class FormAssociation : ClassAssociation
    {
        
        public                          FormAssociation( string signature, string prettyName, bool allowRootCollection )
            : base( signature, prettyName, allowRootCollection, typeof( Engine.Plugin.Collection ) ) { }
        
        public                          FormAssociation( string signature, string prettyName, bool allowRootCollection, Type[] children )
            : base( signature, prettyName, allowRootCollection, typeof( Engine.Plugin.Collection ), children ) { }
        
        public                          FormAssociation( string signature, string prettyName, bool allowRootCollection, Type containerClassType )
            : base( signature, prettyName, allowRootCollection, containerClassType ) { }
        
        public FormAssociation( string signature, string prettyName, bool allowRootCollection, Type containerClassType, Type[] children )
            : base( signature, prettyName, allowRootCollection, containerClassType, children ) { }
        
    }
    
}
