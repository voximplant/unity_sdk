using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

namespace Voximplant
{
    internal class LocalStreamCameraBehaviour : MonoBehaviour
    {
        private bool shouldBypasssRGBConversion()
        {
            switch (SystemInfo.graphicsDeviceType) {
                case GraphicsDeviceType.Metal:
                    return false;
            }

            return true;
        }

        private UnityEngine.Camera _myCamera;

        private int _pipelineStage = 0;

        private int _pipelineLength = 0;
        private RenderTexture[] _renderTextures;
        private Texture2D[] _texture2Ds;

        private bool _hasFastCopy;

        private int GetPipelineLength() {
            switch (SystemInfo.graphicsDeviceType)
            {
                case GraphicsDeviceType.Metal: return 3;
                default: return 5;
            }
        }

        private int CurrentWidth() {
            return ((_myCamera.pixelWidth / 8) + 1) * 8;
        }
        
        public void EnsureTextures()
        {
            if (_texture2Ds != null) {
                return;
            }

            var newWidth = CurrentWidth();

            _renderTextures = new RenderTexture[_pipelineLength];
            for (int i = 0; i < _pipelineLength; i++) {
                _renderTextures[i] = new RenderTexture(newWidth, _myCamera.pixelHeight, 0,
                    RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
            }

            if (_texture2Ds != null) {
                OnResolutionWillChange();
            }

            _texture2Ds = new Texture2D[_pipelineLength];
            for (int i = 0; i < _pipelineLength; i++) {
                var shouldBypasssRgbConversion = shouldBypasssRGBConversion();
                _texture2Ds[i] =
                    new Texture2D(newWidth, _myCamera.pixelHeight, TextureFormat.RGBA32, false, shouldBypasssRgbConversion){
                        filterMode = FilterMode.Point
                    };

                _texture2Ds[i].Apply();
            }

            OnResolutionDidChange(newWidth, _myCamera.pixelHeight);
        }

        private void Awake()
        {
            _hasFastCopy = (SystemInfo.copyTextureSupport & CopyTextureSupport.RTToTexture) != 0;
            _pipelineLength = GetPipelineLength();
            
            Debug.Log(string.Format("copyTextureSupport {0}", SystemInfo.copyTextureSupport));
            Debug.Log(string.Format("Pipeline length {0}", _pipelineLength));
            
            _myCamera = GetComponent<UnityEngine.Camera>();
            Assert.IsNotNull(_myCamera);
        }

        private void OnPreRender()
        {
            RenderTexture.active = null;
        }

        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            Graphics.Blit(src, dest);

            if (_myCamera.stereoActiveEye == UnityEngine.Camera.MonoOrStereoscopicEye.Right)
            {
                return;
            }
            
            EnsureTextures();

            Graphics.Blit(src, _renderTextures[_pipelineStage]);
            _pipelineStage = (_pipelineStage + 1) % _pipelineLength;
            
            NativeRender();
        }

        private void NativeRender() {
            var stage = _pipelineStage;

            Profiler.BeginSample("Read Pixels");
            var currentTexture = _texture2Ds[stage];
            if (_hasFastCopy)
            {
                Graphics.CopyTexture(_renderTextures[stage], currentTexture);
            }
            else
            {
                RenderTexture.active = _renderTextures[stage];
                currentTexture.ReadPixels(new Rect(0, 0, RenderTexture.active.width, RenderTexture.active.height), 0, 0, false);
                currentTexture.Apply();
            }
            Profiler.EndSample();

            stage = (stage + 1) % _pipelineLength;

            stage = (stage + 1) % _pipelineLength;

            Profiler.BeginSample("Native Render");
            OnTextureRendered(_texture2Ds[stage]);
            Profiler.EndSample();
        }

        private void OnDestroy()
        {
            if (_texture2Ds != null) {
                OnResolutionWillChange();
            }

            foreach (var renderTexture in _renderTextures) {
                renderTexture.Release();
            }
        }

        internal Action OnResolutionWillChange = () => { };
        internal Action<int, int> OnResolutionDidChange = (x, y) => { };

        internal Action<Texture> OnTextureRendered = texture => { };
    }
}