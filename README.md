
# Voximplant UnitySDK

## Android build

SDK countains two parts:
- Unity code
- Android java code (androidPartSDK dir)

### Unity part

Add UnityPartSDK to Unity Project Assets.
Select project main scene, select any object on it (can be an empty object) and connect "InvSDK.cs" script to that object.

In order to use SDK get object instance and access it's methods. Refer to `main.cs` example for details. Available methods are:
- public void closeConnection()
- public void connect()
- public void login(LoginClassParam pLogin)
- public void call(CallClassParam pCall)
- public void answer()
- public void declineCall()
- public void hangup()
- public void setMute(Boolean p)
- public void sendVideo(Boolean p)
- public void setCamera(CameraSet p)
- public void disableTls()
- public void disconnectCall(string p)
- public void enableDebugLogging()
- public void loginUsingOneTimeKey(LoginOneTimeKeyClassParam pLogin)
- public void requestOneTimeKey(string pName)
- public void sendDTMF(DTFMClassParam pParam)
- public void sendInfo(InfoClassParam pParam)
- public void sendMessage(SendMessageClassParam pParam)
- public void setCameraResolution(CameraResolutionClassParam pParam)
- public void setUseLoudspeaker(bool pUseLoudSpeaker)

Use following event handlers to receive notifications:

- void OnLoginSuccessful(string p1)
- void OnLoginFailed(string p1)
- void OnOneTimeKeyGenerated(string p1)
- void OnConnectionSuccessful()
- void OnConnectionClosed()
- void OnConnectionFailedWithError(string p1)
- void OnCallConnected(string p1, Dictionary<string, string> p2)
- void OnCallDisconnected(string p1, Dictionary<string, string> p2)
- void OnCallRinging(string p1, Dictionary<string, string> p2)
- void OnCallFailed(string p1, int p2, string p3, Dictionary<string, string> p4)
- void OnCallAudioStarted(string p1)
- void OnIncomingCall(String p1, String p2, String p3, Boolean p4, Dictionary<String, String> p5)
- void OnSIPInfoReceivedInCall(string p1, string p2, string p3, Dictionary<string, string> p4)
- void OnMessageReceivedInCall(string p1, string p2, Dictionary<string, string> p3)
- void OnNetStatsReceived(string p1, int p2)

### Android part

In order to enable Android part open a Build Settings and check the "Google Android Project" checkbox. After that, open project via Android IDE like "Android Studio". Follow these configuration steps:
- Create 'libs' dir at same level as the 'build.gradle' file and copy 'voximplant.jar' file into that newly created dir.
- Find a 'jniLibs' dir at same level as the 'AndroidManifest.xml' and copy into it the 'armeabi-v71/libjingle_peerconnection_so.so' file and the 'x86/libjingle_peerconnection_so.so' file.
- Add build and load lib configuration into 'build.gradle' file:

```json
dependencies {
    compile fileTree(dir: 'libs', include: '*.jar')
    compile 'com.google.code.gson:gson:2.6.1'
    compile 'com.android.support:appcompat-v7:24.2.0'
}
```  

- Create a Unity package within a project and add 'androidPartSDK' dir to it.
- Add following field into 'UnityPlayerActivity.java' file:

```cs
AVoImClient mVoxClient;
```

- Add following line as a last one in the 'onCreate' method:

```cs
mVoxClient = new AVoImClient(this);
```

- If Android API version is greater then 22, add permission change hadler:

```cs
@Override
public void onRequestPermissionsResult(int requestCode, String permissions[], int[] grantResults) {
  mVoxClient.Init();
}
```

- Add permission info into 'AndroidManifest.xml' file:

```xml
<uses-permission android:name="android.permission.RECORD_AUDIO"/>
<uses-permission android:name="android.permission.MODIFY_AUDIO_SETTINGS"/>
<uses-permission android:name="android.permission.INTERNET"/>
<uses-permission android:name="android.permission.CAMERA"/>
<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE"/>
```

## iOS build

Copy 'Assets' dir into the Unity project dir.
Select project main scene, select any object on it (can be an empty object) and connect "InvSDKios" script to that object. You can check 'SDKIOS' example for reference.

- Add SDK instance field into main script:
```objective-c
InvSDKios invios;
```

- Init instance field by quering object from the scene and initializing it with callback object name and video areas size.
```objective-c
invios = GameObject.FindObjectOfType<InvSDKios>();
invios.init("SDKIOS", new SizeView(0,0, 100, 100), new SizeView(0, 150, 100, 100));
```

- Connect event handlers:
```objective-c
invios.onConnectionSuccessful += Invios_onConnectionSuccessful;
```

- Select 'iOS' platform while building project.
- Set 'Bundle Identifier' in the player options.
- Set the 'Scripting Backend' as 'IL2CPP'.
- Set the 'Target Device' as 'iPhone+iPad'.
- Set the 'Target SDK' as 'Device SDK'.
- Target minimum iOS version as '8.1'.
- Set API Compatibility Level as '.NET 2.0'.
- Build a project and open it with 'XCode'.
- Set the 'TARGETS' as 'Unity-iPhone'.
- Add a valid developer signature in the 'General' tab.
- Add 'VoxImplantFWK.framework' at 'Embedded Binaries' configuration section.
- Set the 'Enable Bitcode' to 'No' in the 'Build Settings'.
- Set the 'Architectures' as 'Standart Architectures (armv7, arm64) - $(ARCHS_STANDART)'
- Add the 'Privacy - Microphone Usage Description' and the 'Privacy - Camera Usage Description' permissions to the 'Info.plist' file.
- Run via 'XCode - Debug' with all checkbox disabled.
