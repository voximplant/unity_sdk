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

    private void Awake()
    {
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
            if (remoteQuad != null) {
                vox.beginUpdatingTextureWithVideoStream(callid, VoximplantSDK.VideoStream.Remote, texture => {
                    // Assign texture to same object each frame, ex
                    // something.GetComponent<MeshRenderer>().material.mainTexture = texture;
                    remoteQuad.GetComponent<MeshRenderer>().material.mainTexture = texture;
                });
            }
            if (localQuad != null) {
                vox.beginUpdatingTextureWithVideoStream(callid, VoximplantSDK.VideoStream.Local,
                    texture => { localQuad.GetComponent<MeshRenderer>().material.mainTexture = texture; });
            }
        };
        vox.IncomingCall += (callId, from, displayName, call, headers) => { vox.answer(callId); };
        vox.LoginSuccessful += displayName => {
            var callId = vox.createCall("test1", true, "");

            vox.useCameraVideoStreamForCall(callId, localVirtualCamera);
            vox.startCall(callId);
            vox.setMute(true);
        };
        vox.CallDisconnected += (id, headers) => {
            var callId = vox.createCall("test1", true, "");

            vox.useCameraVideoStreamForCall(callId, localVirtualCamera);
            vox.startCall(callId);
            vox.setMute(true);
        };
    }
}