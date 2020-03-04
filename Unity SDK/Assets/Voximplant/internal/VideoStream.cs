/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

using System;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;
using Voximplant.Unity.Call;
using Voximplant.Unity.Call.EventArgs;
using Object = UnityEngine.Object;

namespace Voximplant.Unity.@internal
{
    internal abstract class VideoStream : MonoBehaviour, IVideoStream
    {
        protected readonly Collection<Object> Renderers = new Collection<Object>();
        private int _height;
        private int _rotation;
        private Texture2D _texture;
        private IntPtr _texturePtr = IntPtr.Zero;
        private int _width;

        protected abstract IntPtr TexturePtr { get; }
        public abstract string StreamId { get; }
        public abstract int Width { get; }
        public abstract int Height { get; }
        public abstract int Rotation { get; }

        internal bool Local { get; set; }

        public event SdkEventHandler<IVideoStream, VideoStreamChangedEventArgs> VideoStreamChanged;

        public virtual void AddRenderer(RawImage target)
        {
            Debug.Log($"AddRenderer({StreamId} => {target})");
            Renderers.Add(target);
        }

        public virtual void AddRenderer(Material target)
        {
            Debug.Log($"AddRenderer({StreamId} => {target})");
            Renderers.Add(target);
        }

        public virtual void RemoveRenderer(Object target)
        {
            Debug.Log($"RemoveRenderer({StreamId} => {target})");
            Renderers.Remove(target);
        }

        public void Dispose()
        {
            Debug.Log($"Dispose called on {StreamId}");
            if (_texturePtr != IntPtr.Zero)
            {
                Destroy(_texture);
            }
            _texturePtr = IntPtr.Zero;
            _texture = null;

            UpdateRenderers();
            DisposeImpl();

            _width = 0;
            _height = 0;
            _rotation = 0;

            Destroy(this);
        }

        public virtual void Start()
        {
        }

        public void Update()
        {
            if (Renderers.Count == 0)
            {
                Debug.Log($"Renderers == 0 ; Stream {StreamId} {_texturePtr} {_texture} {TexturePtr}");
            }
            
            if (Width != _width || Height != _height || Rotation != _rotation || TexturePtr != _texturePtr)
            {
                Debug.Log($"Create TexturePtr: {_texturePtr}={TexturePtr} ({IntPtr.Zero})\n" +
                          $"{Width}={_width},{Height}={_height},{Rotation}={_rotation}\n" +
                          $"{StreamId}");
                CreateTexture();
                UpdateRenderers();

                VideoStreamChanged?.Invoke(this, new VideoStreamChangedEventArgs
                {
                    Width = Width,
                    Height = Height,
                    Rotation = Rotation,
                });
            }

            UpdateImpl();
        }

        protected abstract void UpdateImpl();
        protected abstract void DisposeImpl();

        private void CreateTexture()
        {
            _rotation = Rotation;
            _width = Width;
            _height = Height;

            _texturePtr = TexturePtr;

            if (_texturePtr == IntPtr.Zero)
            {
                Destroy(_texture);
                _texture = null;
            }
            else
            {
                _texture = Texture2D.CreateExternalTexture(_width, _height, TextureFormat.RGBA32,
                    false, false, _texturePtr);
            }
        }

        private void UpdateRenderers()
        {
            foreach (var o in Renderers)
            {
                switch (o)
                {
                    case RawImage rawImage:
                        rawImage.material.mainTexture = _texture;
                        break;
                    case Material material:
                        material.mainTexture = _texture;
                        break;
                }
            }
        }
    }
}