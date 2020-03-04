# Voximplant Unity SDK

## Demo

<https://github.com/voximplant/unity_sdk_demo>

## Install

1. Download latest Voximplant Unity SDK from [GitHub Releases](https://github.com/voximplant/unity_sdk/releases)
2. In Unity Editor, select **Assets > Import Package > Custom Package...** Navigate to the directory where you downloaded the Voximplant Unity SDK and select `VoximplantSDK-*.unitypackage`.
3. Import all assets in the package.

## Usage

To get started, you'll need to [register](https://voximplant.com) a free Voximplant developer account.

### Initialization

IClient is the main class of the SDK that provides access to Voximplant’s functions, the `VoximplantSdk.GetClient()` method is used to get its instance:

```csharp
VoximplantSdk.Initialize();
_client = VoximplantSdk.GetClient();

_client.Connected += ClientOnConnected;
_client.LoginSuccess += ClientOnLoginSuccess;
```

### Connect and log in to the Voximplant Cloud

The `IClient.State` property is used to get the current state of connection to the Voximplant cloud and perform the actions according to it.

```csharp
private void LoginWithPassword(string login, string password) {
    _login = login;
    _password = _password;
    if (_client.State == ClientState.Disconnected)
    {
        _client.Connect();
    }
    else if (_client.State == ClientState.Connected)
    {
        _client.Login(_login, _password);
    }
}

private void ClientOnConnected(IClient sender)
{
    _client.Login(_login, _password);
}

private void ClientOnLoginSuccess(IClient sender, LoginSuccessEventArgs e)
{
    _displayName = e.DisplayName;
}
```

### Make calls

To initiate a call we need the `IClient.Call()` method. There is a `CallSettings` class which could contain custom data and extra headers (SIP headers).

Since the call can behave in different ways, there is a group of call events. They can be triggered by the `ICall` instance as the class contains all the functionality for call management.

```csharp
private ICall MakeAudioCall(string number)
{
    var call = _client.Call(number, new CallSettings());
    if (call == null) return null;

    call.Disconnected += OnCallDisconnected;
    call.Failed += OnCallFailed;
    call.Connected += OnCallConnected;
    call.Ringing += OnCallRinging;
    call.AudioStarted += OnCallAudioStarted;

    call.Start();
    return call;
}

private void OnCallConnected(ICall sender, CallConnectedEventArgs e)
{
    Debug.Log("Call connected");
}
```

### Receiving calls

`IClient.IncomingCall` event handler is used to get incoming calls.

There are two methods for an incoming call: answer and reject. An audio stream can be sent only after the answer method call.

```csharp
VoximplantSdk.GetClient().IncomingCall += OnIncomingCall;

private void OnIncomingCall(IClient sender, IncomingCallEventArgs e)
{
    sender.Answer(new CallSettings());
}
```

### Mid-call operations

Call can be put on/off hold

```csharp
_call.Hold(true, error =>
{
    if (error != null)
    {
        Debug.LogError(error.Value.Message);
    }
});
```

## Limitations

* Unity 2018
* Android Multithreaded rendeding unsupported
* iOS OpenGL rendering unsupported
