using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Voximplant
{
    internal class LocalStreamCameraBehaviour : MonoBehaviour
    {
        private UnityEngine.Camera _myCamera;
        private RenderTexture _renderTexture;

        private Texture2D _texture2D;

        public void EnsureTexture()
        {
            if (_texture2D != null
                && _myCamera.pixelHeight == _texture2D.height
                && _myCamera.pixelWidth == _texture2D.width) {
                return;
            }


            if (_texture2D != null) {
                OnTextureDestroyed(_texture2D);
            }

            _texture2D = new Texture2D(_myCamera.pixelWidth, _myCamera.pixelHeight, TextureFormat.RGBA32, false);

            if (_texture2D != null) {
                _texture2D.Apply();
                OnTextureCreated(_texture2D);
            }
        }

        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            EnsureTexture();

            var prevTexture = RenderTexture.active;
            RenderTexture.active = src;
            _texture2D.ReadPixels(new Rect(0, 0, _myCamera.pixelWidth, _myCamera.pixelHeight), 0, 0, false);
            _texture2D.Apply();
            RenderTexture.active = prevTexture;

            Graphics.Blit(src, dest);

            OnTextureRendered(_texture2D);
        }

        private void Awake()
        {
            _myCamera = GetComponent<UnityEngine.Camera>();
            Assert.IsNotNull(_myCamera);
        }

        private void OnDestroy()
        {
            if (_texture2D != null) {
                OnTextureDestroyed(_texture2D);
            }
        }

        internal Action<Texture2D> OnTextureDestroyed = texture => { };
        internal Action<Texture2D> OnTextureCreated = texture => { };

        internal Action<Texture2D> OnTextureRendered = texture => { };
    }
}