using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UnityEngine.Processor
{
    public class ProcessorManager : MonoBehaviour
    {
        private void Awake()
        {
            ForegroundInvoker.Initialize().transform.parent = this.transform;
            ForegroundProcessor.Initialize().transform.parent = this.transform;
            BackgroundProcessor.Initialize();
            DontDestroyOnLoad(this.gameObject);
        }

        private void OnApplicationQuit()
        {
            ForegroundInvoker.Exit();
            ForegroundProcessor.Exit();
            BackgroundProcessor.Exit();
        }
    }
}
