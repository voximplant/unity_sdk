/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */
using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Voximplant.Unity.@internal
{
    internal class VideoSource : MonoBehaviour, IDisposable
    {
        private Action<Texture2D, int, int> _callback;
        private Camera _camera;
        private bool _cameraIsSet;
        
        public string SourceId { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public float FPS { get; set; }

        private RenderTexture _renderTexture;
        private Texture2D _texture2D;


        public void Dispose()
        {
            CancelInvoke();

            Destroy(this);
        }

        public void Initialize([NotNull] Camera unityCamera, int width, int height, float fps)
        {
            Width = width;
            Height = height;
            FPS = fps;

            _camera = gameObject.AddComponent<Camera>();
            _camera.CopyFrom(unityCamera);
            _camera.enabled = false;

            _renderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
            _texture2D = new Texture2D(width, height, TextureFormat.ARGB32, false);

            _camera.targetTexture = _renderTexture;

            InvokeRepeating(nameof(Render), 0f, 1f / fps);
        }

        private void Render()
        {
            _camera.enabled = true;
        }

        private void OnPostRender()
        {
            _camera.enabled = false;

            _texture2D.ReadPixels(new Rect(0, 0, Width, Height), 0, 0);
            _texture2D.Apply();

            _callback?.Invoke(_texture2D, Width, Height);
        }

        public void SetTextureCallback(Action<Texture2D, int, int> textureCallback)
        {
            _callback = textureCallback;
        }
    }
}
