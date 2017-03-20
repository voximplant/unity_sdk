### Unity SDK integration
* Download `VoximplantSDK.unitypackage`
* Assets -> Import Package -> Custom Package, import downloaded package
* File -> Build Settings

#### Android
Required Player Settings:
* bundle identifier
* minimum API level: 16

#### iOS
You are all set, however app behaviour when user refuse to grant permissions is undefined. Refer to `[AVAudioSession sharedInstance].recordPermission` and `[AVCaptureDevice authorizationStatusForMediaType:AVMediaTypeVideo]` to check current permissions and decide what to do.

Required Player Settings:
* minimal supported SDK: 8.0
* camera usage description
* microphone usage description


### Voximplant package structure
* `Plugins/Android` — Android SDK with all dependencies, packaged as .aar archive
* `Plugins/iOS` — iOS SDK with dependencies as dynamic libraries
* `Plugins/Editor/Voximplant` — Unity build step for iOS setting up static and dynamic libraries integration
* `Scripts/Voximplant` — Unity-land SDK code