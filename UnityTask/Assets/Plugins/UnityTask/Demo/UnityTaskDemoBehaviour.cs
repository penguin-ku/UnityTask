using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.TaskExtension;

namespace UnityEngine.TaskExtension.Demo
{
    public class UnityTaskDemoBehaviour : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
            Task.Run(() =>
            {
                return Task.FromException(new System.Exception());
            }).Unwrap().AsForeground().ContinueWith(t => 
            {
                if (t.IsFaulted)
                {
                    return UnityTask.FromException(t.Exception);
                }
                else
                {
                    return UnityTask.FromResult(0);
                }
            }).Unwrap().AsBackground().ContinueWith(t=> 
            {
                if (t.IsFaulted)
                {
                    Debug.Log(t.Exception);
                }
            });
            //UnityTask.Run(FirstTask).ContinueWith(t =>
            //{
            //    UnityTask.Run(ThirdSecondTask);
            //    return SecondTask();
            //}).ContinueWith(t =>
            //{
            //    Debug.Log("complete");
            //});

            //Task.Run(() =>
            //{
            //    Thread.Sleep(5000);
            //});
            //Task.Run<int>(() =>
            //{
            //    for (int i = 0; i < 10; i++)
            //    {
            //        Debug.Log(i);
            //    }
            //    return 100;
            //}).ContinueWith(t =>
            //{
            //    Task.Run(() =>
            //    {
            //        for (int i = 0; i < 50; i++)
            //        {
            //            Debug.Log(i);
            //        }
            //    });
            //}).ContinueWith(t =>
            //{
            //    Debug.Log("complete");
            //});
        }

        // Update is called once per frame
        void Update()
        {

        }

        IEnumerator FirstTask()
        {
            int i = 10;
            while (i-- > 0)
            {
                Debug.Log(i + "         first");
                yield return i;
            }

            yield return 1000;
        }

        IEnumerator SecondTask()
        {
            int i = 50;
            while (i-- > 0)
            {
                Debug.Log(i + "         second");
                yield return i;
            }

            yield return 1000;
        }

        IEnumerator ThirdSecondTask()
        {
            int i = 100;
            while (i-- > 0)
            {
                Debug.Log(i + "         third");
                yield return i;
            }

            yield return 1000;
        }
    }
}
