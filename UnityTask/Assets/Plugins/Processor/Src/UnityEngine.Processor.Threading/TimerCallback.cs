using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityEngine.Processor.Threading
{
    /// <summary>
    /// a TimerCallback delegate to specify the method you want the Timer to execute.
    /// </summary>
    /// <param name="p_state">An object containing application-specific information relevant to the method invoked by this delegate, or null.</param>
    public delegate void TimerCallback(object p_state);
}
