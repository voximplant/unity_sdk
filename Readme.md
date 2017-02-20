### Unity SDK integration
* Download `VoximplantSDK.unitypackage`
* Assets -> Import Package -> Custom Package, import downloaded package
* File -> Build Settings

#### Android
* Change build system to external
* Export project
* Modify exported `UnityPlayerActivity.java`:
* include `protected AVoImClient mVoxClient;` inside `UnityPlayerActivity` class
* insert `mVoxClient = new AVoImClient();` right after `protected void onCreate(Bundle savedInstanceState) {`
* decide how to handle permission requests, if you target Android M and older. Once user granted permissions listed by `AVoImClient.getRequiredPermissions`, call `mVoxClient.Init();`; call `mVoxClient.Init()` right away, if you decide not to handle Android M and newer targets.

Code checking permissions might look like this:
```java
    // Resume Unity
    @Override
    protected void onResume() {
        super.onResume();
        mUnityPlayer.resume();

        if (Build.VERSION.SDK_INT < Build.VERSION_CODES.M ||
            AVoImClient.areRequiredPermissionsGranted(getApplicationContext())) {
                mVoxClient.Init();
        }
    }

    @Override
    public void onRequestPermissionsResult(int requestCode, String[] permissions, int[] grantResults) {
        if (!AVoImClient.areRequiredPermissionsGranted(getApplicationContext())) {
            return;
        }

        mVoxClient.Init();
    }
```

Required Player Settings:
* bundle identifier
* minimum API level: 16

#### iOS
* Update description for user on why app needs access to microphone and camera by modifying `Assets > Plugins > Editor > Voximplant > VoxiOSExport.cs` file

```csharp
    ...
    plist.root.SetString("NSMicrophoneUsageDescription", "&");
    plist.root.SetString("NSCameraUsageDescription", "&");
    ...
```

You are all set, however app behaviour when user refuse to grant permissions is undefined. Refer to `[AVAudioSession sharedInstance].recordPermission` and `[AVCaptureDevice authorizationStatusForMediaType:AVMediaTypeVideo]` to check current permissions and decide what to do.

Required Player Settings:
* minimal supported SDK: 8.0+
* 