/*
 * IMouseOver.cs
 *
 * Interface for the render window to retrieve some basic information about the object the mouse is over.
 *
 */
using System;
using System.Collections.Generic;

namespace GUIBuilder
{
    /// <summary>
    /// Description of IMouseOver.
    /// </summary>
    public interface IMouseOver
    {
        
        bool IsMouseOver( Maths.Vector2f mouse, float maxDistance );
        
        List<string> MouseOverInfo
        {
            get;
        }
        
    }
}
