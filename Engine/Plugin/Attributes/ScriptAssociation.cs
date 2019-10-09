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
    
    public static partial class Reflection
    {
        
        #region Form Attributes from Signature/Type
        
        public static ScriptAssociation ScriptAssociationFrom( string signature )
        {
            var associations = AllAssociations;
            if( associations == null ) return null;
            foreach( var association in associations )
                if( association.Signature == signature ) return association as ScriptAssociation;
            return null;
        }
        
        public static ScriptAssociation ScriptAssociationFrom( Type type )
        {
            var associations = AllAssociations;
            if( associations == null ) return null;
            foreach( var association in associations )
                if( association.ClassType == type ) return association as ScriptAssociation;
            return null;
        }
        
        #endregion
        
    }
    
}
