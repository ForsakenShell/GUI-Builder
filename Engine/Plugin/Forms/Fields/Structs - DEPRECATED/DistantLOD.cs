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
    
    public struct DistantLOD
    {
        
        public string[] Level;
        
        public DistantLOD( string[] level )
        {
            Level = new string[4];
            if( level == null )
                return;
            for( var i = 0; i <= Math.Min( 3, level.Length ); i++ )
                Level[ i ] = level[ i ];
        }
        
    }
    
}
