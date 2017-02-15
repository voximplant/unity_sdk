
# Voximplant UnitySDK

### Unity часть

Весь пакет файлов необходимо добавить в папку с Assets Вашего Unity проекта.
Сцена Example создержит в себе пример проекта с использованием SDK.

Для использования SDK необходимо на один из объектов сцены (это может быть пустой объект) добавить скрипт InvSDK.cs.

Для использования, Вам нужно получить объект этого класса в коде и настроить его.
В файле main.cs есть пример как это делается.
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

        void OnLoginSuccessful(string displayName)
        void OnLoginFailed(LoginFailureReason reason)
        void OnOneTimeKeyGenerated(string key)
        void OnConnectionSuccessful()
        void OnConnectionClosed()
        void OnConnectionFailedWithError(string reason)
        void OnCallConnected(string callId, Dictionary<string, string> headers)
        void OnCallDisconnected(string callId, Dictionary<string, string> headers)
        void OnCallRinging(string callId, Dictionary<string, string> headers)
        void OnCallFailed(string callId, int code, string reason, Dictionary<string, string> headers)
        void OnCallAudioStarted(string callId)
        void OnIncomingCall(String callId, String from, String displayName, Boolean videoCall, Dictionary<String, String> headers)
        void OnSIPInfoReceivedInCall(string callId, string type, string content, Dictionary<string, string> headers)
        void OnMessageReceivedInCall(string callId, string text, Dictionary<string, string> headers)
        void OnNetStatsReceived(string callId, int packetLoss)
        void OnStartCall(string callId)

# Сборка проекта

 В основном тулбаре Unity есть дополнительное меню SmartBuild. В нем можно выбрать экспорт проекта в Android или xCode.
Большая часть настроек произойдет автоматически, но некоторые моменты нужно будет изменить руками.

## Android project

После SmartBuild/Export Android project проект нужно импортировать в Android Studio чтобы IDE собрала Gradle сборку под него.
Если Вы используется compileSdkVersion отличный от 24, то добавьте зависимость на библиотеку
com.android.support:appcompat-v7 нужной Вам версии.
 После этого проект готов для сборки в apk файл.

## xCode project

После SmartBuild/Export xCode project проект нужно открыть в xCode, на вкладке Generals задать валидную подпись разработчика и в секции Embedded Binaries добавить и выбрать VoxImplantFWK.framework. Если используется Xcode 8.3 то в Build Settings в значениях Valid Architectures оставть только arm64 (В будущем фреймворк будет сбилжен и под другие архитектуры). Сборщик билдит проект для Device SDK.
