/*
 * SetStaticObjectModels.cs
 *
 * Sets NIF Model and LODs on a StatcObject.
 *
 */
using System.Collections.Generic;
using Engine.Plugin;
using Engine.Plugin.Forms;


namespace GUIBuilder.FormImport.Operations
{

    public class SetStaticObjectModels : ImportOperation
    {
        
        const string                                    DN_Model = "Static.Model";
        const string                                    DN_LOD = "Static.LOD";

        readonly string                                 Model;
        readonly string[]                               LODs;
        
        public override string[]                        OperationalInformation()
        {
            var lodCount = LODs.NullOrEmpty() ? 0 : LODs.Length;
            var result = new string[ 1 + lodCount ];
            result[ 0 ] = string.Format( "{0}: \"{1}\"", DN_Model.Translate(), Model );
            if( lodCount > 0 )
                for( int i = 0; i < lodCount; i++ )
                    result[ 1 + i ] =string.Format(
                        "{0} #{1}: \"{2}\"",
                        DN_LOD.Translate(),
                        i.ToString(),
                        LODs[ i ]
                    );
            return result;
        }
        
        public                                          SetStaticObjectModels( ImportBase parent, string model, string[] lods )
        : base( parent )
        {
            Model = model;
            LODs  = lods;
        }

        public override bool                            Apply()
        {
            var stat = Target.Value as Static;
            var result = stat != null;
            if( !result )
                Parent.AddErrorMessage( ErrorTypes.Import, "ImportTarget did not resolve to " + typeof( Static ).FullName() );
            else
            {
                stat.SetModel( TargetHandle.Working, Model );

                if( !LODs.NullOrEmpty() )
                    stat.DistantLOD.SetValue( TargetHandle.Working, LODs );
                else
                    stat.DistantLOD.DeleteRootElement( false, false );
                result = TargetMatchesImport();
            }
            return result;
        }

        public override bool                            TargetMatchesImport()
        {
            var stat = Target.Value as Static;
            if( stat == null ) return false;

            var lods = stat.DistantLOD.GetValue( TargetHandle.WorkingOrLastFullRequired );
            var elStat = lods.NullOrEmpty();
            var elImp = LODs.NullOrEmpty();
            if(
                ( (  elStat )&&( !elImp ) )||
                ( ( !elStat )&&(  elImp ) )
            )   return false;

            if( ( !elStat )&&( !elImp ) )
            {
                var clStat = lods.Length;
                var clImp = LODs.Length;
                if( clStat != clImp ) return false;
                for( int i = 0; i < clImp; i++ )
                    if( !lods[ i ].InsensitiveInvariantMatch( LODs[ i ] ) )
                        return false;
            }
            
            var statModel = stat.GetModel( TargetHandle.WorkingOrLastFullRequired );
            return Model.InsensitiveInvariantMatch( statModel );
        }
    }

}
