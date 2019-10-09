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
    
    public struct MapData
    {
        
        public Vector2i CellNW;
        public Vector2i CellSE;
        
        public MapData( Vector2i cellnw, Vector2i cellse )
        {
            CellNW = cellnw;
            CellSE = cellse;
        }
    }
    
}
