using System;
using System.Collections.Generic;

public abstract class Factor
{
    /// <summary>
    /// Virtual function that every Factor has. It's used to get the utility.
    /// </summary>
    public virtual float getValue()
    {
        return 0;
    }
}

