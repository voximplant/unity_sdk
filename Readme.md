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
You are all set.

Required Player Settings:
    * minimal supported SDK: 8.0+
