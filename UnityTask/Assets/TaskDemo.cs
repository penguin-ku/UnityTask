using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.TaskExtension;
public class TaskDemo : MonoBehaviour {

    public GameObject Object;

	// Use this for initialization
	void Start () {
        Task.Run(() =>
        {
            Thread.Sleep(200);
            Debug.Log("休眠好了");
        }).ContinueWith(t=> 
        {
            Thread.Sleep(200);
            Debug.Log("又休眠好了");
        }).ContinueToForeground(t=> 
        {
            UnityEngine.Object.DestroyImmediate(Object);
            Debug.Log("强势插入主线程，干掉了目标");
        });
        Debug.Log("开始");
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
