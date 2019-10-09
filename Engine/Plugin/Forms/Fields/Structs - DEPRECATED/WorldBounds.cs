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
    
    public struct WorldBounds
    {
        
        public Vector2i Min;
        public Vector2i Max;
        
        public WorldBounds( Vector2i min, Vector2i max )
        {
            Min = min;
            Max = max;
        }
    }
    
}
