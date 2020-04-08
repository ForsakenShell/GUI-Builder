/*
 * ObjectReferenceTarget.cs
 *
 * Base object reference import class.
 *
 */
using System;

using Engine.Plugin;
using Engine.Plugin.Forms;
using Engine.Plugin.Interface;
using Engine.Plugin.Attributes;
using Engine.Plugin.Extensions;


namespace GUIBuilder.FormImport
{
    
    public class ObjectReferenceTarget : ImportTarget
    {

        ImportTarget                                    _Cell = null;
        public ImportTarget                             Cell
        {
            get
            {
                return _Cell;
            }
        }

        #region Constructors

        private void                                    INTERNAL_Constructor(
            ImportBase parent,
            Engine.Plugin.Forms.Cell cell )
        {
            _Cell           = new ImportTarget( parent, "Form.CellU"      .Translate(), typeof( Cell       ), cell       );
        }

        public                                          ObjectReferenceTarget(
            ImportBase parent, 
            string displayName,
            string editorID,
            Cell cell )
            : base(
                parent,
                displayName,
                typeof( ObjectReference ),
                editorID )
        {
            INTERNAL_Constructor( parent, cell );
        }

        public                                          ObjectReferenceTarget(
            ImportBase parent,
            string displayName,
            ObjectReference reference,
            Cell cell )
            : base(
                parent,
                displayName,
                typeof( ObjectReference ),
                reference )
        {
            INTERNAL_Constructor( parent, cell );
        }

        #endregion

        public override bool                            Resolve( bool errorIfUnresolveable = true )
        {
            if( !Cell.Resolve( errorIfUnresolveable ) )
                return false;
            
            return base.Resolve( errorIfUnresolveable );
        }

        public override bool                            CreateNewFormInWorkingFile()
        {
            var cell = _Cell.Value as Cell;
            if( cell == null )
            {
                Parent.AddErrorMessage( ErrorTypes.Import, "Cell is unresolved" );
                return false;
            }
            try
            {
                var refr = cell.ObjectReferences.CreateNew<ObjectReference>();
                if( refr == null )
                {
                    Parent.AddErrorMessage( ErrorTypes.Import, string.Format(
                        "Unable to create a new ObjectReference in cell {0}",
                        cell.IDString ) );
                    return false;
                }
                return true;
            }
            catch( Exception e )
            {
                Parent.AddErrorMessage( ErrorTypes.Import, string.Format(
                    "An exception occured when trying to create a new ObjectReference in cell {0}\nInner Exception:\n{1}",
                    cell.IDString,
                    e.ToString()) );
            }
            return false;
        }

    }
    
}