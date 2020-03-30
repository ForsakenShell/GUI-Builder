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
                if( _ClassTypeSetOnce ) throw new Exception( string.Format( "{0}\n\tError :: ClassType can only be set once on a ClassAssociation!\nStack:\n{1}", this.TypeFullName(), Environment.StackTrace ) );
                _ClassTypeSetOnce = true;
                if( value == null ) return;
                if( !value.HasInterface<Interface.IXHandle>() )
                    throw new Exception( string.Format( "{0}\n\tError :: ClassType must implement IXHandle!\n{1}", this.TypeFullName(), Environment.StackTrace ) );
                _ClassType = value;
            }
        }
        
        public readonly Type            CollectionClassType = null;
        
        public readonly bool            AllowRootCollection = false;
        
        public bool                     HasChildCollections { get { return !ChildTypes.NullOrEmpty(); } }
        
        public bool                     HasChildOrGrandchildAssociationOf( ClassAssociation association, bool includeGrandchildren = true )
        {
            if( !HasChildCollections ) return false;
            foreach( var childType in ChildTypes )
            {
                var childAssociation = Reflection.AssociationFrom( childType );
                if( !childAssociation.IsValid() ) continue;
                if(
                    ( childAssociation == association )||
                    ( includeGrandchildren &&( childAssociation.HasChildOrGrandchildAssociationOf( association ) ) )
                )   return true;
            }
            return false;
        }
        
        public bool                     HasChildOrGrandchildAssociationOf( Type type, bool includeGrandchildren = true )
        {
            if( !HasChildCollections ) return false;
            foreach( var childType in ChildTypes )
            {
                var childAssociation = Reflection.AssociationFrom( childType );
                if( !childAssociation.IsValid() ) continue;
                if(
                    ( childAssociation.ClassType == type )||
                    ( includeGrandchildren &&( childAssociation.HasChildOrGrandchildAssociationOf( type ) ) )
                )   return true;
            }
            return false;
        }
        
        public bool                     HasChildOrGrandchildAssociationOf( string signature, bool includeGrandchildren = true )
        {
            if( !HasChildCollections ) return false;
            foreach( var childType in ChildTypes )
            {
                var childAssociation = Reflection.AssociationFrom( childType );
                if( !childAssociation.IsValid() ) continue;
                if(
                    ( childAssociation.Signature == signature )||
                    ( includeGrandchildren &&( childAssociation.HasChildOrGrandchildAssociationOf( signature ) ) )
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
                throw new Exception( string.Format( "{0}\n\tError :: Signature cannot be null or empty!", this.TypeFullName() ) );
            Signature = signature;
            PrettyName = prettyName;
        }
        
        public                          ClassAssociation( string signature, string prettyName, bool allowRootCollection, Type collectionClassType )
        {
            if( string.IsNullOrEmpty( signature ) )
                throw new Exception( string.Format( "{0}\n\tError :: Signature cannot be null or empty!", this.TypeFullName() ) );
            if( ( collectionClassType == null )||( !collectionClassType.IsClassOrSubClassOf( typeof( Collection ) ) ) )
                throw new Exception( string.Format( "{0}\n\tError :: collectionClassType cannot be null and must be [a sub-class of] Collection!", this.TypeFullName() ) );
            CollectionClassType = collectionClassType;
            Signature = signature;
            AllowRootCollection = allowRootCollection;
            PrettyName = prettyName;
        }
        
        public                          ClassAssociation( string signature, string prettyName, bool allowRootCollection, Type collectionClassType, Type[] children )
        {
            if( string.IsNullOrEmpty( signature ) )
                throw new Exception( string.Format( "{0}\n\tError :: Signature cannot be null or empty!", this.TypeFullName() ) );
            if( ( collectionClassType == null )||( !collectionClassType.IsClassOrSubClassOf( typeof( Collection ) ) ) )
                throw new Exception( string.Format( "{0}\n\tError :: collectionClassType cannot be null and must be [a sub-class of] Collection!", this.TypeFullName() ) );
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
                    ( _ClassType.FullName() ),
                    ( CollectionClassType.FullName() ),
                    ( PrettyName == null ? null : string.Format( "{0} = ", PrettyName ) )
                );
        }
        
    }
    
}
