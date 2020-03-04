/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */
#define UPDATE_LOOP
//#define PRE_POST_RENDER_LOOP

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Voximplant.Unity.@internal
{
    internal class ThreadUtils : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod]
        static void createInstance()
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("{ThreadUtils}");
                _instance = go.AddComponent<ThreadUtils>();
                DontDestroyOnLoad(go);
            }
        }

        private static ThreadUtils _instance;

        private static ThreadUtils Instance
        {
            get
            {
                createInstance();
                return _instance;
            }
        }

        #region "Handle Update"

#if UPDATE_LOOP
        public static Queue<Action> updateQueue = new Queue<Action>();
        private static object updateQueueLock = new object();

        public static void RunOnUpdate(Action action)
        {
            lock (updateQueueLock)
            {
                updateQueue.Enqueue(action);
            }
        }

        void Update()
        {
            onUpdateLoop();
        }

        void onUpdateLoop()
        {
            lock (updateQueueLock)
            {
                DrainQueue(updateQueue);
            }
        }

#endif

        #endregion

#if PRE_POST_RENDER_LOOP
		private int camerasLeft = 0;

		void Awake ()
		{
			Camera.onPreRender += onPreRender;
			Camera.onPostRender += onPostRender;
		}

		void OnDestroy ()
		{
			Camera.onPreRender -= onPreRender;
			Camera.onPostRender -= onPostRender;
		}

		#region "Handle PreRender"

		public static Queue<Action> preRenderQueue = new Queue<Action> ();
		private static object preRenderQueueLock = new object ();

		public static void RunOnPreRender (Action action)
		{
			lock (preRenderQueueLock) {  
				preRenderQueue.Enqueue (action);
			}  
		}

		void onPreRender (Camera cam)
		{
			if (camerasLeft <= 0) {
				camerasLeft = Camera.allCamerasCount;
				onPreRenderLoop ();
			}
		}

		void onPreRenderLoop ()
		{
			lock (preRenderQueueLock) {  
				DrainQueue (preRenderQueue);
			}  
		}

		#endregion

		#region "Handle PostRender"

		public static Queue<Action> postRenderQueue = new Queue<Action> ();
		private static object postRenderQueueLock = new object ();

		public static void RunOnPostRender (Action action)
		{
			lock (postRenderQueueLock) {  
				postRenderQueue.Enqueue (action);
			}  
		}

		void onPostRender (Camera cam)
		{
			camerasLeft--;
			if (camerasLeft <= 0) {
				onPostRenderLoop ();
			}
		}

		void onPostRenderLoop ()
		{
			lock (postRenderQueueLock) {  
				DrainQueue (postRenderQueue);
			}  
		}

		#endregion

#endif

        static void DrainQueue(Queue<Action> queue)
        {
            while (queue.Count > 0)
            {
                Action action = queue.Dequeue();
                if (action != null)
                {
                    action.Invoke();
                }
            }
        }
    }
}
