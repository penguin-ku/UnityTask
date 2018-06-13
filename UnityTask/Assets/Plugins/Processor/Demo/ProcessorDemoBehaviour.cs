using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Processor;
using UnityEngine.Processor.Threading;

public class ProcessorDemoBehaviour : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        Debug.Log(string.Format("{0} start：", System.DateTime.Now));

        Debug.Log(string.Format("ForegroudTimer start：", System.DateTime.Now));
        ForegroudTimer timer = new ForegroudTimer(o => 
        {
            Debug.Log(string.Format("{0} ForegroudTimer trigger：", System.DateTime.Now));
        }, null, 1000, 2000);


        ForegroundInvoker.Invoke(() => 
        {
            Debug.Log(string.Format("ForegroundInvoker invoke：", System.DateTime.Now));
        });

        BackgroundProcessor.AppendAction(() => 
        {
            timer.Dispose();
            Debug.Log(string.Format("{0} ForegroudTimer end：", System.DateTime.Now));
        }, 4000, 1, 8000);

    }

    // Update is called once per frame
    void Update()
    {

    }

    
}
