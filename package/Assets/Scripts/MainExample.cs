using UnityEngine;
using Voximplant;
using Camera = UnityEngine.Camera;

public class MainExample : MonoBehaviour
{
    VoximplantSDK vox;
    string ACC = "your-acc-name-here";

    public Camera localVirtualCamera;

    public GameObject localQuad;
    public GameObject remoteQuad;

    public void OnConnectClick()
    {
        if (vox == null) {
            vox = gameObject.AddVoximplantSDK(); // extension method helper
            vox.LogMethod += Debug.Log;
            vox.init(granted => {
                if (granted) vox.connect(); // check audio and video permissions
            });
            vox.ConnectionSuccessful += () => {
//            vox.login("user@conference-app." + ACC + ".voximplant.com", "unitydemo");
                vox.login("test11@videochat.yulia.voximplant.com", "testpass");
            };
            vox.CallConnected += (callid, headers) => {
                vox.beginUpdatingTextureWithVideoStream(callid, VoximplantSDK.VideoStream.Remote, texture => {
                    // Assign texture to same object each frame, ex
                    // something.GetComponent<MeshRenderer>().material.mainTexture = texture;
                    remoteQuad.GetComponent<MeshRenderer>().material.mainTexture = texture;
                });
                vox.beginUpdatingTextureWithVideoStream(callid, VoximplantSDK.VideoStream.Local, texture => {
                    localQuad.GetComponent<MeshRenderer>().material.mainTexture = texture;
                });
            };
            vox.ConnectionFailedWithError += (reason) => { };
            vox.LoginFailed += (reason) => { };
            vox.CallDisconnected += (callid, headers) => { };
            vox.CallFailed += (callid, code, reason, heasers) => { };
            vox.CallRinging += (callId, headers) => { };
            vox.IncomingCall += (callId, from, displayName, call, headers) => {
                vox.answer(callId);
            };
        } else {
            var callId = vox.createCall("test10", true, "");
            vox.useCameraVideoStreamForCall(callId, localVirtualCamera);
            vox.startCall(callId);
        }
    }
}