#region

using System;
using System.Collections.Generic;

#endregion

namespace CommandLineUtils.Collections
{
    internal class Map<TKey, TValue> : Dictionary<TKey, TValue>
    {
        internal new bool Add( TKey key, TValue value )
        {
            try
            {
                base.Add( key, value );
                return true;
            }
            catch ( ArgumentException )
            {
                return false;
            }
        }
    }
}