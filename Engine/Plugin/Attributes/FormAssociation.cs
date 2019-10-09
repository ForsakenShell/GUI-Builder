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
    
    public static partial class Reflection
    {
        
        #region Form Attributes from Signature/Type
        
        public static FormAssociation   FormAssociationFrom( string signature )
        {
            var associations = AllAssociations;
            if( associations == null ) return null;
            foreach( var association in associations )
                if( association.Signature == signature ) return association as FormAssociation;
            return null;
        }
        
        public static FormAssociation   FormAssociationFrom( Type type )
        {
            var associations = AllAssociations;
            if( associations == null ) return null;
            foreach( var association in associations )
                if( association.ClassType == type ) return association as FormAssociation;
            return null;
        }
        
        #endregion
        
    }
    
}
