
# Voximplant UnitySDK (Build for Android)

SDK состоит из двух частей
  - Unity скрипты
  - Android java классы (папка androidPartSDK)

### Unity часть

Папку UnityPartSDK необходимо добавить к Assets Вашего Unity проекта.
На главной сцене проекта, на один из объектов (это может быть пустой объект) добавить скрипт InvSDK.cs.

Для использования, Вам нужно получить объект этого класса в коде и настроить его.
В файле main.cs есть пример  как это делается.
Методы доступные для вызова:

        public void closeConnection()
        public void connect()
        public void login(LoginClassParam pLogin)
        public void call(CallClassParam pCall)
        public void answer()
        public void declineCall()
        public void hangup()
        public void setMute(Boolean p)
        public void sendVideo(Boolean p)
        public void setCamera(CameraSet p)
        public void disableTls()
        public void disconnectCall(string p)
        public void enableDebugLogging()
        public void loginUsingOneTimeKey(LoginOneTimeKeyClassParam pLogin)
        public void requestOneTimeKey(string pName)
        public void sendDTMF(DTFMClassParam pParam)
        public void sendInfo(InfoClassParam pParam)
        public void sendMessage(SendMessageClassParam pParam)
        public void setCameraResolution(CameraResolutionClassParam pParam)
        public void setUseLoudspeaker(bool pUseLoudSpeaker)

Для обратной связи доступна подписка на следующие события:

        void OnLoginSuccessful(string p1)
        void OnLoginFailed(string p1)
        void OnOneTimeKeyGenerated(string p1)
        void OnConnectionSuccessful()
        void OnConnectionClosed()
        void OnConnectionFailedWithError(string p1)
        void OnCallConnected(string p1, Dictionary<string, string> p2)
        void OnCallDisconnected(string p1, Dictionary<string, string> p2)
        void OnCallRinging(string p1, Dictionary<string, string> p2)
        void OnCallFailed(string p1, int p2, string p3, Dictionary<string, string> p4)
        void OnCallAudioStarted(string p1)
        void OnIncomingCall(String p1, String p2, String p3, Boolean p4, Dictionary<String, String> p5)
        void OnSIPInfoReceivedInCall(string p1, string p2, string p3, Dictionary<string, string> p4)
        void OnMessageReceivedInCall(string p1, string p2, Dictionary<string, string> p3)
        void OnNetStatsReceived(string p1, int p2)

### Android часть

Для использования Android части необходимо экспортировать Unity проект в Android Project(в Build settings для Android проекта, поставить галочку Google Android Project).

После этого, этот проект нужно импортировать в Android IDE, напримре Android Studio.
Выполнить действия по покдлючению voximplant:

  - На том же уровне что и файл build.gradle создать папку libs и скопировать туда файл voximplantsdk.jar.
  - На том же уровне что и AndroidManifest.xml в папку jniLibs скопировать файлы armeabi-v71/libjingle_peerconnection_so.so и x86/libjingle_peerconnection_so.so в соответствующие папки.
  - Отредактировать файл build.gradle и добавить код для компиляции файлов библиотек и загрузки зависимых библиотек

  ```
  dependencies {
    compile fileTree(dir: 'libs', include: '*.jar')
    compile 'com.google.code.gson:gson:2.6.1'
    compile 'com.android.support:appcompat-v7:24.2.0'
  }
  ```  

  - В проекте создать пакет в который добавить файлы Android части (папка androidPartSDK)
  - В главном классе UnityPlayerActivity.java, объявить переменную

  ```
  AVoImClient mVoxClient;
  ```

  - В методе onCreate, последней строчкой добавить код создания переменной

  ```
  mVoxClient = new AVoImClient(this);
  ```

  - Добавить метод отрабатывающи при принятии прав доступа для приложений(обязательно если версия API больше 22)

  ```
  @Override
  public void onRequestPermissionsResult(int requestCode,
                     String permissions[], int[] grantResults) {
    mVoxClient.Init();
  }
  ```

  - В файл AndroidManifest.xml добавить права для приложения.

  ```  
  <uses-permission android:name="android.permission.RECORD_AUDIO"/>
  <uses-permission android:name="android.permission.MODIFY_AUDIO_SETTINGS"/>
  <uses-permission android:name="android.permission.INTERNET"/>
  <uses-permission android:name="android.permission.CAMERA"/>
  <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE"/>
  ```



# Voximplant UnitySDK (Build for iOS)

Скопировать папку Assets в папку с Unity проектом.

  - Создать пустой объект на сцене, к которому привязать скрипт InvSDKios (в примере он называется SDKIOS).
  - В main скрипте объявить переменную для работы в SDK
```
        InvSDKios invios;
```
  - Инициализировать переменную нужно получив объекта с объекта сцены и вызвав метод с передачей имени объекта сцены для обратной связи и размеров вьюх для отображения видео потоков.

  ```
        invios = GameObject.FindObjectOfType<InvSDKios>();
        invios.init("SDKIOS", new SizeView(0,0, 100, 100), new SizeView(0, 150, 100, 100));
  ```
  - Привязать нужные события для обработки

```
        invios.onConnectionSuccessful += Invios_onConnectionSuccessful;
```

 При сборке проекта выбрать платформу iOS.
 Run in Xcode - Debug. Все остальные галки снять.
  - В настройках плеера задать Bundle Identifier.
  - Scripting Backend - IL2CPP.
  - Target Device - iPhone+iPad.
  - Target SDK - Device SDK.
  - Target minimum iOS version - 8.1.
  - API Compatibility Level - .NET 2.0.

  - Собрать проект. Открыть его в XCode.
  - В TARGETS выбрать Unity-iPhone.
  - На вкладке Generals задать валидную подпись разработчика. В секции Embedded Binaries добавить и выбрать VoxImplantFWK.framework
  - На вкладке Build Settings в секци Build Options свойство Enable Bitcode установить в No.  
  - В секции Architectures в свойстве Architectures выбрать Standart Architectures (armv7, arm64) - $(ARCHS_STANDART)
  - В Info.plist добавить разрешения со значением &
  Privacy - Microphone Usage Description
  Privacy - Camera Usage Description
