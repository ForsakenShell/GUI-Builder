/*
 * ImportOperation.cs
 *
 * Base import operation class.  Without operations imports do nothing.
 *
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Engine.Plugin;
using Engine.Plugin.Forms;
using Engine.Plugin.Interface;
using Engine.Plugin.Attributes;
using Engine.Plugin.Extensions;

namespace GUIBuilder.FormImport
{

    public abstract class ImportOperation
    {

        protected readonly ImportBase                   Parent;

        protected ImportTarget                          Target
        {
            get
            {
                return Parent.Target;
            }
        }

        protected                                       ImportOperation( ImportBase parent )
        {
            Parent = parent;
        }

        public string                                   Signature
        {
            get
            {
                return this.TypeName();
            }
        }
        
        /// <summary>
        /// This should return the fields and values the operation will make changes to
        /// </summary>
        /// <returns></returns>
        public abstract string[]                        OperationalInformation();

        /// <summary>
        /// Resolve additional ImportTargets for this operation
        /// </summary>
        /// <param name="errorIfUnresolveable"></param>
        /// <returns></returns>
        public virtual bool                             Resolve( bool errorIfUnresolveable )
        {
            return true;
        }

        /// <summary>
        /// Apply the import operation to the ImportTarget
        /// </summary>
        /// <returns></returns>
        public abstract bool                            Apply();

        /// <summary>
        /// Does the ImportTarget match this operations outcome?
        /// </summary>
        /// <returns></returns>
        public abstract bool                            TargetMatchesImport();

    }

}