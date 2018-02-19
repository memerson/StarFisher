using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace StarFisher.Office.Utilities
{
    public class ComObjectManager : IDisposable
    {
        private readonly Stack<object> _comObjects = new Stack<object>();

        public void Dispose()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            while (_comObjects.Count > 0)
                Marshal.ReleaseComObject(_comObjects.Pop());
        }

        public TComObject Get<TComObject>(Func<TComObject> getter)
        {
            var comObject = getter();

            if (comObject != null)
                _comObjects.Push(comObject);

            return comObject;
        }
    }
}