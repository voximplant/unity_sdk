using UnityEngine;
using System.Collections.Generic;
using Voximplant;

public class MainExample : MonoBehaviour
{
    VoximplantSDK vox;
    string ACC = "your-acc-name-here";

    void Start()
    {
        vox = gameObject.AddVoximplantSDK(); // extension method helper
        vox.init(granted => {
            if (granted) vox.connect(); // check audio and video permissions
        });
        vox.LogMethod += Debug.Log;
        vox.ConnectionSuccessful += () => {
            vox.login("user@conference-app." + ACC + ".voximplant.com", "unitydemo");
        };
        vox.LoginSuccessful += (name) => {
            vox.call("*", false, false, "");
        };
        vox.CallConnected += (callid, headers) => {
            vox.beginUpdatingTextureWithVideoStream(VoximplantSDK.VideoStream.Remote, texture => {
                // Assign texture to same object each frame, ex
                // something.GetComponent<MeshRenderer>().material.mainTexture = texture;
            });
        };
        vox.ConnectionFailedWithError += (reason) => { };
        vox.LoginFailed += (reason) => { };
        vox.CallConnected += (callid, headers) => { };
        vox.CallDisconnected += (callid, headers) => { };
        vox.CallFailed += (callid, code, reason, heasers) => { };
    }
}

