using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Diagnostics;
using System;
using System.Reflection;
using System.Threading;

namespace MBT
{
    public class TestExecutor : MonoBehaviour
    {
        public MonoBehaviourTree tree;

        // void Awake(){
        //     Profile("standard", Test1);
        //     Profile("reflection", Test2);
        // }

        // public void Test1()
        // {
        //     for (int i = 0; i < 1000000; i++)
        //     {
        //         string description = tree.description;
        //     }
        // }

        // public void Test2()
        // {
        //     for (int i = 0; i < 1000000; i++)
        //     {
        //         string description = (string) tree.GetType().GetField("description").GetValue(tree);
        //     }
        // }

        void Update()
        {
            tree.Tick();
        }
        // static double Profile(string description, Action func) {
        //     //Run at highest priority to minimize fluctuations caused by other processes/threads
        //     Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
        //     Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Highest;

        //     // warm up 
        //     func();

        //     var watch = new Stopwatch(); 

        //     // clean up
        //     GC.Collect();
        //     GC.WaitForPendingFinalizers();
        //     GC.Collect();

        //     watch.Start();
        //     func();
        //     watch.Stop();
        //     UnityEngine.Debug.Log(description);
        //     UnityEngine.Debug.Log(watch.Elapsed.TotalMilliseconds);
        //     return watch.Elapsed.TotalMilliseconds;
        // }
    }
}
