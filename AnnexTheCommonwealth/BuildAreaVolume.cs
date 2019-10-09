/*
 * BuildAreaVolume.cs
 *
 * An ATC build area volume primitive.
 *
 */

using System;

using Maths;


namespace AnnexTheCommonwealth
{
    
    /// <summary>
    /// Description of BuildAreaVolume.
    /// </summary>
    [Engine.Plugin.Attributes.ScriptAssociation( "ESM:ATC:BuildAreaVolume" )]
    public class BuildAreaVolume : Volume
    {
        
        #region Constructor
        
        public BuildAreaVolume( Engine.Plugin.Forms.ObjectReference reference )
            : base( reference ) { }
        
        #endregion
        
    }
    
}
