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
            LocationName = string.Format( "0x{0} - No location", reference.GetFormID( Engine.Plugin.TargetHandle.Master ).ToString( "X8" ) );
        }
        
        #endregion
        
        #region Public Properties
        
        #endregion
        
    }
    
}
