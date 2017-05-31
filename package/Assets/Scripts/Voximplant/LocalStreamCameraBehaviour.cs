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
        private UnityEngine.Camera _myCamera;

        private int _pipelineStage = 0;

        private const int _pipelineLength = 3;
        private RenderTexture[] _renderTextures;
        private Texture2D[] _texture2Ds;

        private bool _hasFastCopy;

        public void EnsureTextures()
        {
            if (_texture2Ds != null) {
                return;
            }

            _renderTextures = new RenderTexture[_pipelineLength];
            for (int i = 0; i < _pipelineLength; i++) {
                _renderTextures[i] = new RenderTexture(_myCamera.pixelWidth, _myCamera.pixelHeight, 0);
            }

            if (_texture2Ds != null) {
                OnResolutionWillChange();
            }

            _texture2Ds = new Texture2D[_pipelineLength];
            for (int i = 0; i < _pipelineLength; i++) {
                _texture2Ds[i] =
                    new Texture2D(_myCamera.pixelWidth, _myCamera.pixelHeight, TextureFormat.RGBA32, false){
                        filterMode = FilterMode.Point
                    };

                _texture2Ds[i].Apply();
            }

            OnResolutionDidChange(_myCamera.pixelWidth, _myCamera.pixelHeight);
        }

        private void Awake()
        {
            _hasFastCopy = (SystemInfo.copyTextureSupport & CopyTextureSupport.RTToTexture) != 0;
            
            Debug.Log(string.Format("copyTextureSupport {0}", SystemInfo.copyTextureSupport));
            
            _myCamera = GetComponent<UnityEngine.Camera>();
            Assert.IsNotNull(_myCamera);
            
            _isRunning = true;
            StartCoroutine(NativeRender());
        }

        private void OnPreRender()
        {
            RenderTexture.active = null;
        }

        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            Graphics.Blit(src, dest);

            EnsureTextures();

            Graphics.Blit(src, _renderTextures[_pipelineStage]);
            _pipelineStage = (_pipelineStage + 1) % _pipelineLength;
        }

        private bool _isRunning;

        private IEnumerator NativeRender()
        {
            while (_isRunning) {
                yield return new WaitForEndOfFrame();
                
                if (Input.touchCount > 0) {
                    continue;
                }

                var stage = _pipelineStage;
                
                Profiler.BeginSample("Read Pixels");
                var currentTexture = _texture2Ds[stage];
                if (_hasFastCopy) {
                    Graphics.CopyTexture(_renderTextures[stage], currentTexture);
                } else {
                    RenderTexture.active = _renderTextures[stage];
                    _texture2Ds[stage].ReadPixels(new Rect(0, 0, _myCamera.pixelWidth, _myCamera.pixelHeight), 0, 0, false);
                    _texture2Ds[stage].Apply();
                }
                Profiler.EndSample();
                
                stage = (stage + 1) % _pipelineLength;

                stage = (stage + 1) % _pipelineLength;

                Profiler.BeginSample("Native Render");
                OnTextureRendered(_texture2Ds[stage]);
                Profiler.EndSample();
            }
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