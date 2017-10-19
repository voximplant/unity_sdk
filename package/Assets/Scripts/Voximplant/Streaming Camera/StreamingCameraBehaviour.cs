using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

namespace Voximplant
{
    internal class StreamingCameraBehaviour : MonoBehaviour
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

        public bool MainCameraMode = true;
        public int desiredWidth = 0;
        public int desiredHeight = 0;

        private int GetPipelineLength() {
            switch (SystemInfo.graphicsDeviceType)
            {
                case GraphicsDeviceType.Metal: return 3;
                default: return 5;
            }
        }

        private int RenderWidth() {
            int width;
            if (MainCameraMode)
            {
                width = _myCamera.pixelWidth;
            }
            else
            {
                width = desiredWidth;
            }
            Assert.IsTrue(width > 0);
            return ((width / 8) + 1) * 8;
        }

        private int RenderHeight() {
            if (MainCameraMode)
            {
                return _myCamera.pixelHeight;
            }
            
            return desiredHeight;
        }

        public void Reset() {
            _texture2Ds = null;
            OnResolutionWillChange = null;
            OnResolutionDidChange = null;
            OnTextureRendered = null;
        }
        
        public void EnsureTextures()
        {
            if (_texture2Ds != null) {
                return;
            }
            
            Debug.Log("EnsureTextures");

            var newWidth = RenderWidth();
            var newHeight = RenderHeight();

            _renderTextures = new RenderTexture[_pipelineLength];
            for (int i = 0; i < _pipelineLength; i++) {
                _renderTextures[i] = new RenderTexture(newWidth, newHeight, 0,
                    RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
            }

            if (_texture2Ds != null) {
                OnResolutionWillChange();
            }

            _texture2Ds = new Texture2D[_pipelineLength];
            for (int i = 0; i < _pipelineLength; i++) {
                var shouldBypasssRgbConversion = shouldBypasssRGBConversion();
                _texture2Ds[i] =
                    new Texture2D(newWidth, newHeight, TextureFormat.RGBA32, false, shouldBypasssRgbConversion){
                        filterMode = FilterMode.Point
                    };

                _texture2Ds[i].Apply();
            }

            OnResolutionDidChange(newWidth, newHeight);
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
            if (MainCameraMode)
            {
                RenderTexture.active = null;
            }
            else
            {
                EnsureTextures();
            
                _myCamera.targetTexture = _renderTextures[_pipelineStage];
            }
        }

        private void OnPostRender() {
            if (MainCameraMode)
            {
                return;
            }
            
            _pipelineStage = (_pipelineStage + 1) % _pipelineLength;
            
            NativeRender();
        }

        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            Graphics.Blit(src, dest);

            if (!MainCameraMode)
            {
                return;
            }

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
                var oldTexture = RenderTexture.active;
                
                RenderTexture.active = _renderTextures[stage];
                currentTexture.ReadPixels(new Rect(0, 0, RenderTexture.active.width, RenderTexture.active.height), 0, 0, false);
                currentTexture.Apply();
                
                RenderTexture.active = oldTexture;
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

        internal static GameObject StreamingCameraObject(int width, int height) {
            var go = new GameObject("Streaming Camera");
            var camera = go.AddComponent<UnityEngine.Camera>();

            camera.fieldOfView = 120;
            camera.aspect = 4 / 3f;
            camera.stereoTargetEye = StereoTargetEyeMask.None;
            
            var streaming = go.AddComponent<StreamingCameraBehaviour>();
            streaming.MainCameraMode = false;
            streaming.desiredWidth = width;
            streaming.desiredHeight = height;
            return go;
        }
    }
}