### Unity SDK integration
* Download `VoximplantSDK.unitypackage`
* Assets -> Import Package -> Custom Package, import downloaded package
* File -> Build Settings

#### Android
Required Player Settings:
* bundle identifier
* minimum API level: 16

#### iOS
* Update description for user on why app needs access to microphone and camera by modifying `Assets > Plugins > Editor > Voximplant > VoxiOSExport.cs`. Relevant lines are:
```csharp
    plist.root.SetString("NSMicrophoneUsageDescription", "&");
    plist.root.SetString("NSCameraUsageDescription", "&");
```

You are all set, however app behaviour when user refuse to grant permissions is undefined. Refer to `[AVAudioSession sharedInstance].recordPermission` and `[AVCaptureDevice authorizationStatusForMediaType:AVMediaTypeVideo]` to check current permissions and decide what to do.

Required Player Settings:
* minimal supported SDK: 8.0