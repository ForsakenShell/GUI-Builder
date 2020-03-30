/*
 * ClassAssociation.cs
 *
 * Base attribute to abstract and associate forms/scripts with the C# class to handle
 * them in a way that does not require extra code in the form/script classes themselves.
 *
 */
#define DUMP_ASSOCIATIONS
#define DUMP_STACK_WITH_ACCOCIATION_DUMP

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace Engine.Plugin.Attributes
{
    
    public static class AssociationExtensions
    {
        
        public static bool              IsValid( this ClassAssociation association )
        {
            return
                ( association != null )&&
                ( !string.IsNullOrEmpty( association.Signature ) )&&
                ( association.ClassType != null )&&
                ( association.ClassType.GetInterface( typeof( Engine.Plugin.Interface.IXHandle ).Name ) != null );
        }
        
        public static void              Dump( this ClassAssociation association, string prefix = null )
        {
            if( string.IsNullOrEmpty( prefix ) ) prefix = "ClassAssociation";
            DebugLog.WriteLine( string.Format(
                "{0}\n\tSignature = \"{1}\"\n\tClass Type = \"{2}\"\n\tCollection Class Type = \"{3}\"\n\tAllow Root Collection = {4}\n\tHas Child Collections = {5}",
                prefix,
                association.Signature,
                ( association.ClassType.FullName() ),
                ( association.CollectionClassType.FullName() ),
                association.AllowRootCollection,
                association.HasChildCollections
            ) );
            if( association.HasChildCollections )
            {
                foreach( var childType in association.ChildTypes )
                {
                    var associations = Reflection.AssociationFrom( childType );
                    if( !associations.IsValid() ) continue;
                    DebugLog.WriteLine( string.Format(
                        "\t\tChild :: \"{0}\" - {1}",
                        associations.Signature,
                        ( associations.ClassType.FullName() )
                    ) );
                }
            }
        }
        
    }
    
}
