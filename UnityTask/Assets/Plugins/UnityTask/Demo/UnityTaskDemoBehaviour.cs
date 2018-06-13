using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TaskExtension;

public class UnityTaskDemoBehaviour : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        UnityTask.Run<int>(FirstTask).ContinueWith(t =>
        {
            Debug.Log(t.Result);
        });
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator FirstTask()
    {
        int i = 10;
        while (i-->0)
        {
            yield return i;
        }

        yield return 1000;
    }
}
