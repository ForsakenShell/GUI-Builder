/*
 * ClassAssociation.cs
 *
 * Base attribute to abstract and associate forms/scripts with the C# class to handle
 * them in a way that does not require extra code in the form/script classes themselves.
 *
 */
#define DUMP_ASSOCIATIONS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace Engine.Plugin.Attributes
{
    // Notes:
    
    // Signature:
    // For forms this will be the actual form signature - "STAT", "WRLD", "CELL", etc
    // For scripts this will be the fully qualified scriptname - "WorkshopScript", "ESM:ATC:SubDivision", etc
    // For files, this will be the filename
    
    /// <summary>
    /// Description of ClassAttributes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ClassAssociation : Attribute, IEquatable<ClassAssociation>
    {
        
        public readonly Type[]          ChildTypes = null;
        
        public readonly string          Signature = null;
        public readonly string          PrettyName = null;
        
        Type                           _ClassType = null;
        bool                           _ClassTypeSetOnce = false;
        
        public Type                     ClassType
        {
            get { return _ClassType; }
            set
            {
                if( _ClassTypeSetOnce ) throw new Exception( string.Format( "{0}\n\tError :: ClassType can only be set once on a ClassAssociation!\nStack:\n{1}", this.GetType().ToString(), Environment.StackTrace ) );
                _ClassTypeSetOnce = true;
                if( value == null ) return;
                if( !value.HasInterface<Interface.IXHandle>() )
                    throw new Exception( string.Format( "{0}\n\tError :: ClassType must implement IXHandle!\n{1}", this.GetType().ToString(), Environment.StackTrace ) );
                _ClassType = value;
            }
        }
        
        public readonly Type            CollectionClassType = null;
        
        public readonly bool            AllowRootCollection = false;
        
        public bool                     HasChildCollections { get { return !ChildTypes.NullOrEmpty(); } }
        
        public bool                     HasChildOrGrandchildAssociationOf( ClassAssociation association )
        {
            if( !HasChildCollections ) return false;
            foreach( var childType in ChildTypes )
            {
                var childAssociation = Reflection.AssociationFrom( childType );
                if( !childAssociation.IsValid() ) continue;
                if(
                    ( childAssociation == association )||
                    ( childAssociation.HasChildOrGrandchildAssociationOf( association ) )
                )   return true;
            }
            return false;
        }
        
        public bool                     HasChildOrGrandchildAssociationOf( Type type )
        {
            if( !HasChildCollections ) return false;
            foreach( var childType in ChildTypes )
            {
                var childAssociation = Reflection.AssociationFrom( childType );
                if( !childAssociation.IsValid() ) continue;
                if(
                    ( childAssociation.ClassType == type )||
                    ( childAssociation.HasChildOrGrandchildAssociationOf( type ) )
                )   return true;
            }
            return false;
        }
        
        public bool                     HasChildOrGrandchildAssociationOf( string signature )
        {
            if( !HasChildCollections ) return false;
            foreach( var childType in ChildTypes )
            {
                var childAssociation = Reflection.AssociationFrom( childType );
                if( !childAssociation.IsValid() ) continue;
                if(
                    ( childAssociation.Signature == signature )||
                    ( childAssociation.HasChildOrGrandchildAssociationOf( signature ) )
                )   return true;
            }
            return false;
        }
        
        #region IEquatable
        
        public override int             GetHashCode()
        {
            return Signature.GetHashCode();
        }
        
        public override bool            Equals( Object obj )
        {
            if( ReferenceEquals( null, obj ) ) return false;
            if( ReferenceEquals( this, obj ) ) return true;
            return
                ( obj.GetType() == GetType() )&&
                ( Equals( (ClassAssociation) obj ) );
        }
        
        public bool                     Equals( ClassAssociation other )
        {
            return
                ( ReferenceEquals( this, other ) )||
                ( this.GetHashCode() == other.GetHashCode() );
        }
        
        #endregion
        
        public                          ClassAssociation( string signature, string prettyName )
        {
            if( string.IsNullOrEmpty( signature ) )
                throw new Exception( string.Format( "{0}\n\tError :: Signature cannot be null or empty!", this.GetType().ToString() ) );
            Signature = signature;
            PrettyName = prettyName;
        }
        
        public                          ClassAssociation( string signature, string prettyName, bool allowRootCollection, Type collectionClassType )
        {
            if( string.IsNullOrEmpty( signature ) )
                throw new Exception( string.Format( "{0}\n\tError :: Signature cannot be null or empty!", this.GetType().ToString() ) );
            if( ( collectionClassType == null )||( !collectionClassType.HasInterface<Interface.ICollection>() ) )
                throw new Exception( string.Format( "{0}\n\tError :: collectionClassType cannot be null and must implement ICollection!", this.GetType().ToString() ) );
            CollectionClassType = collectionClassType;
            Signature = signature;
            AllowRootCollection = allowRootCollection;
            PrettyName = prettyName;
        }
        
        public                          ClassAssociation( string signature, string prettyName, bool allowRootCollection, Type collectionClassType, Type[] children )
        {
            if( string.IsNullOrEmpty( signature ) )
                throw new Exception( string.Format( "{0}\n\tError :: Signature cannot be null or empty!", this.GetType().ToString() ) );
            if( ( collectionClassType == null )||( !collectionClassType.HasInterface<Interface.ICollection>() ) )
                throw new Exception( string.Format( "{0}\n\tError :: collectionClassType cannot be null and must implement ICollection!", this.GetType().ToString() ) );
            CollectionClassType = collectionClassType;
            Signature = signature;
            AllowRootCollection = allowRootCollection;
            ChildTypes = children;
            PrettyName = prettyName;
        }
        
        public override string          ToString()
        {
            return this == null
                ? "[null]"
                : string.Format(
                    "{3}[Signature = \"{0}\" :: ClassType = {1} :: CollectionClassType = {2}]",
                    Signature,
                    ( _ClassType == null ? "[null]" : _ClassType.ToString() ),
                    ( CollectionClassType == null ? "[null]" : CollectionClassType.ToString() ),
                    ( PrettyName == null ? null : string.Format( "{0} = ", PrettyName ) )
                );
        }
        
    }
    
    public static partial class AssociationExtensions
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
                ( association.ClassType == null ? "null" : association.ClassType.ToString() ),
                ( association.CollectionClassType == null ? "null" : association.CollectionClassType.ToString() ),
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
                        ( associations.ClassType == null ? "null" : associations.ClassType.ToString() )
                    ) );
                }
            }
        }
    }
    
    public static partial class Reflection
    {
        
        #region Attributes from Signature/Type
        
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
        
        #region Global enumeration of Associations
        
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
        
        static List<ClassAssociation>   GetAllAssociations()
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
                for( int i = 0; i < result.Count; i++ )
                    result[ i ].Dump( string.Format( "ClassAssociation[ {0} ]", i ) );
            }
            
            #endif
            #endregion
            
            return result;
        }
        
        #endregion
        
    }
    
}
