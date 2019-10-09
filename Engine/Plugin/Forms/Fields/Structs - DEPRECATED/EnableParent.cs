/*
 * [Insert File Name Here]
 *
 * Insert description here.
 *
 * User: 1000101
 * Date: 10/07/2018
 * Time: 11:52 AM
 * 
 */
using System;
using System.Collections;
using System.Collections.Generic;

using Maths;

using XeLib;
using XeLib.API;

namespace Engine.Plugin.Forms.Fields.Structs
{
    
    public struct EnableParent
    {
        
        public UInt32 Reference;
        public UInt32 Flags;
        
        public EnableParent( UInt32 reference, UInt32 flags )
        {
            Reference = reference;
            Flags = flags;
        }
        
    }
    
}
