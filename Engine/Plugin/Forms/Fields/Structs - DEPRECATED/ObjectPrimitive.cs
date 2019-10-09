/*
 * [Insert File Name Here]
 *
 * Insert description here.
 *
 * User: 1000101
 * Date: 01/10/2018
 * Time: 1:20 PM
 * 
 */
using System;

using Maths;

using sdColor = System.Drawing.Color;

namespace Engine.Plugin.Forms.Fields.Structs
{
    
    public struct ObjectPrimitive
    {
        
        public Vector3f Bounds;
        public sdColor Color;
        public UInt32 Type;
        
        public ObjectPrimitive( Vector3f bounds, sdColor color, UInt32 type )
        {
            Bounds = bounds;
            Color = color;
            Type = type;
        }
    }
    
}
