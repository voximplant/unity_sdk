/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

using System;
using System.Collections;
using System.Threading;
using JetBrains.Annotations;
using UnityEngine;
using Voximplant.Unity.Call;

namespace Voximplant.Unity.@internal
{
    internal class VideoSource : MonoBehaviour, IDisposable
    {
        private Action<Texture2D, int, int> _callback;
        private Camera _unityCamera;
        private Camera _camera;
        private bool _cameraIsSet;

        public string SourceId { get; set; }
        private int _nativeWidth;
        public int Width { get; set; }
        private int _nativeHeight;
        public int Height { get; set; }
        public float FPS { get; set; }

        private bool _renderingQueued;
        private RenderTexture _nativeRenderTexture;
        private Texture2D _nativeTexture2D;
        private RenderTexture _renderTexture;
        private Texture2D _texture2D;

        public void Dispose()
        {
            CancelInvoke();
            Destroy(_camera);
            Destroy(this);
        }

        public void Initialize([NotNull] Camera unityCamera, int width, int height, float fps)
        {
            _unityCamera = unityCamera;
            _camera = gameObject.AddComponent<Camera>();
            _camera.name = "Voximplant.VideoSource";
            DontDestroyOnLoad(_camera);
            _camera.CopyFrom(_unityCamera);
            _camera.enabled = false;

            Width = width;
            _nativeWidth = _unityCamera.pixelWidth;
            Height = height;
            _nativeHeight = _unityCamera.pixelHeight;
            FPS = fps;

            _nativeRenderTexture = new RenderTexture(_nativeWidth, _nativeHeight, 24, RenderTextureFormat.ARGB32);
            _nativeTexture2D = new Texture2D(_nativeWidth, _nativeHeight, TextureFormat.ARGB32, false);
            _renderTexture = new RenderTexture(Width, Height, 24, RenderTextureFormat.ARGB32);
            _texture2D = new Texture2D(Width, Height, TextureFormat.ARGB32, false);

            _camera.targetTexture = _nativeRenderTexture;

            InvokeRepeating(nameof(Render), 0f, 1f / fps);
        }

        private void LateUpdate()
        {
            var targetTransform = _unityCamera.transform;
            var currentTransform = _camera.transform;
            currentTransform.position = targetTransform.position;
            currentTransform.rotation = targetTransform.rotation;
        }

        private void Render()
        {
            if (_renderingQueued) return;
            _renderingQueued = true;
            _camera.Render();
        }

        private IEnumerator OnPostRender()
        {
            yield return new WaitForEndOfFrame();

            _nativeTexture2D.ReadPixels(new Rect(0, 0, _nativeWidth, _nativeHeight), 0, 0);
            _nativeTexture2D.Apply();
            _gpu_scale(_nativeTexture2D, FilterMode.Bilinear);
            _texture2D.ReadPixels(new Rect(0, 0, Width, Height), 0, 0);
            _texture2D.Apply();
            
            _callback?.Invoke(_texture2D, Width, Height);
            _renderingQueued = false;
        }

        private void _gpu_scale(Texture2D src, FilterMode filterMode)
        {
            src.filterMode = filterMode;
            src.Apply(true);

            Graphics.SetRenderTarget(_renderTexture);

            GL.LoadPixelMatrix(0, 1, 1, 0);

            GL.Clear(true, true, new Color(0, 0, 0, 0));
            Graphics.DrawTexture(new Rect(0, 0, 1, 1), src);
        }

        public void SetTextureCallback(Action<Texture2D, int, int> textureCallback)
        {
            _callback = textureCallback;
        }
    }
}
