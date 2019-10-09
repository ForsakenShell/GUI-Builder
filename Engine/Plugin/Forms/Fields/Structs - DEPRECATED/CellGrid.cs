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
    
    public struct CellGrid
    {
        
        public Vector2i Cell;
        public UInt32 ForceHideLand;
        
        public CellGrid( Vector2i cell, UInt32 forcehideland )
        {
            Cell = cell;
            ForceHideLand = forcehideland;
        }
        
        public override string ToString()
        {
            return Cell.ToString();
        }
    }
    
}
