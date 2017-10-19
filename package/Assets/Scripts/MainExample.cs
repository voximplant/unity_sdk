using UnityEngine;
using Voximplant;
using Camera = UnityEngine.Camera;

public class MainExample : MonoBehaviour
{
    string ACC = "your-acc-name-here";

    
    VoximplantSDK vox;
    public Camera localCamera;
    public GameObject localQuad;
    public GameObject remoteQuad;

    void Start() {
        var streamingCamera = StreamingCameraBehaviour.StreamingCameraObject(960, 720); // It's desired to keep 4/3 aspect ratio
        streamingCamera.transform.parent = localCamera.gameObject.transform;
        localCamera = streamingCamera.GetComponent<Camera>();
        
        vox = gameObject.AddVoximplantSDK(); // extension method helper
        vox.init(granted => {
            if (granted) vox.connect(); // check audio and video permissions
        }, true);
        vox.LogMethod += Debug.Log;
        vox.ConnectionSuccessful += () => {
            vox.login("user@conference-app." + ACC + ".voximplant.com", "unitydemo");
        };
        vox.LoginSuccessful += (name) =>
        {
            var callId = vox.createCall("*", false, "");
            vox.startCall(callId);
        };

        vox.CallConnected += (callid, headers) => {
            vox.beginUpdatingTextureWithVideoStream(callid, VoximplantSDK.VideoStream.Local, texture =>
            {
                localQuad.GetComponent<MeshRenderer>().material.mainTexture = texture;
            });
            vox.beginUpdatingTextureWithVideoStream(callid, VoximplantSDK.VideoStream.Remote, texture =>
            {
                remoteQuad.GetComponent<MeshRenderer>().material.mainTexture = texture;
            });
        };
        vox.ConnectionFailedWithError += (reason) => { };
        vox.LoginFailed += (reason) => { };
        vox.CallConnected += (callid, headers) => { };
        vox.CallDisconnected += (callid, headers) => { };
        vox.CallFailed += (callid, code, reason, heasers) => { };
    }
}

