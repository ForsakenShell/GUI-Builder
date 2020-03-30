/*
 * ClassAssociation.cs
 *
 * Base attribute to abstract and associate forms/scripts with the C# class to handle
 * them in a way that does not require extra code in the form/script classes themselves.
 *
 */
#define DUMP_ASSOCIATIONS
//#define DUMP_STACK_WITH_ACCOCIATION_DUMP

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace Engine.Plugin.Attributes
{
    
    public static class Reflection
    {

        #region All Class Associations

        static List<ClassAssociation>  _AllAssociations = null;
        public static List<ClassAssociation> AllAssociations
        {
            get
            {
                if( _AllAssociations == null )
                    _AllAssociations = GetAllAssociations();
                return _AllAssociations;
            }
        }

#if DUMP_ASSOCIATIONS
        static bool                 ____doDebugDumpOnce = false;
#endif

        static List<ClassAssociation> GetAllAssociations()
        {
            var thisAssembly = Assembly.GetExecutingAssembly();
            if( thisAssembly == null ) return null;

            var list = thisAssembly
                .GetTypes()
                // disable once ConvertClosureToMethodGroup
                .Where( type => type.HasAttribute<ClassAssociation>() )
                .ToList();
            if( list.NullOrEmpty() ) return null;

            Type foo = null;
            var bar = foo.HasAttribute<ClassAssociation>();
            var result = new List<ClassAssociation>();
            foreach( var type in list )
            {
                ClassAssociation association = null;
                if( !type.TryGetAttribute<ClassAssociation>( out association ) ) continue;
                association.ClassType = type;
                result.Add( association );
            }

            #region Debug dump
#if DUMP_ASSOCIATIONS

            if( !____doDebugDumpOnce )
            {
                ____doDebugDumpOnce = true;
#if DUMP_STACK_WITH_ACCOCIATION_DUMP
                DebugLog.OpenIndentLevel( new[] { System.Environment.StackTrace }, false, true, false, false, false );
#else
                DebugLog.OpenIndentLevel();
#endif
                for( int i = 0; i < result.Count; i++ )
                    result[ i ].Dump( string.Format( "ClassAssociation[ {0} ]", i ) );
                DebugLog.CloseIndentLevel();
            }

#endif
            #endregion

            return result;
        }

        #endregion

        #region Class Associations from Signature/Type

        public static ClassAssociation  AssociationFrom( string signature )
        {
            var associations = AllAssociations;
            if( associations == null ) return null;
            foreach( var association in associations )
                if( association.Signature == signature ) return association;
            return null;
        }
        
        public static ClassAssociation  AssociationFrom( Type type )
        {
            var associations = AllAssociations;
            if( associations == null ) return null;
            foreach( var association in associations )
                if( association.ClassType == type ) return association;
            return null;
        }
        
        public static List<ClassAssociation> ParentAssociationsOf( string childSignature )
        {
            var childAttributes = AssociationFrom( childSignature );
            return ParentAssociationsOf( childAttributes );
        }
        
        public static List<ClassAssociation> ParentAssociationsOf( Type childType )
        {
            var childAttributes = AssociationFrom( childType );
            return ParentAssociationsOf( childAttributes );
        }
        
        public static List<ClassAssociation> ParentAssociationsOf( ClassAssociation childAssociation )
        {
            if( !childAssociation.IsValid() ) return null;
            var associations = AllAssociations;
            if( associations == null ) return null;
            var list = new List<ClassAssociation>();
            foreach( var association in associations )
                if(
                    ( association.AllowRootCollection )&&
                    ( association.HasChildOrGrandchildAssociationOf( childAssociation ) )
                )   list.AddOnce( association );
            return list.NullOrEmpty()
                ? null
                : list;
        }

        #endregion

        #region Form Associations from Signature/Type

        public static FormAssociation FormAssociationFrom( string signature )
        {
            var associations = AllAssociations;
            if( associations == null ) return null;
            foreach( var association in associations )
                if( association.Signature == signature ) return association as FormAssociation;
            return null;
        }

        public static FormAssociation FormAssociationFrom( Type type )
        {
            var associations = AllAssociations;
            if( associations == null ) return null;
            foreach( var association in associations )
                if( association.ClassType == type ) return association as FormAssociation;
            return null;
        }

        #endregion

        #region Script Associations from Signature/Type

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
