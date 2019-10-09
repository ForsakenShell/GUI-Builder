using System;
using System.Collections.Generic;

namespace XeLib
{
    public class HandleGroup : IDisposable
    {
        public HandleGroup()
        {
            _Values = new HashSet<ElementHandle>();
        }

        public HandleGroup( ElementHandle[] array )
        {
            _Values = new HashSet<ElementHandle>(array);
        }

        HashSet<ElementHandle> _Values;
        public HashSet<ElementHandle> Values {
            get{
                return _Values;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if (!disposing) return;
            foreach (var h in Values)
                h.Dispose();
        }

        public ElementHandle AddHandle(ElementHandle h)
        {
            Values.Add(h);
            return h;
        }

        public void RemoveHandle(ElementHandle h)
        {
            Values.Remove(h);
        }

        public ElementHandle[] AddHandles(ElementHandle[] array)
        {
            Values.UnionWith(array);
            return array;
        }

        public void RemoveHandles(ElementHandle[] array)
        {
            Values.ExceptWith(array);
        }

        public override string ToString()
        {
            string resString = "(";
            bool first = true;
            foreach( var value in Values )
            {
                if( !first )
                    resString += "\\";
                resString += value;
                first = false;
            }
            resString += ")";
            return resString;
            //return "HandleGroup({string.Join(", ", Values)})";
        }
    }
}