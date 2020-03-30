/*
 * Settlement.cs
 *
 * This groups all related data to settlements.
 *
 */


namespace AnnexTheCommonwealth
{
    
    /// <summary>
    /// Description of Settlement.
    /// </summary>
    [Engine.Plugin.Attributes.ScriptAssociation( "ESM:ATC:Settlement" )]
    public class Settlement : Engine.Plugin.PapyrusScript
    {
        
        // Pulled from handle for quick access
        
        public string LocationName = null;
        
        #region Constructor
        
        public Settlement( Engine.Plugin.Forms.ObjectReference reference ) : base( reference )
        {
            // TODO: DO THIS PROPERLY
            LocationName = string.Format( "{0} - No location", reference.IDString );
        }
        
        #endregion
        
        #region Public Properties
        
        #endregion
        
    }
    
}
