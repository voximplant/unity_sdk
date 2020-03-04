/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using Voximplant.Unity.Call;

namespace Voximplant.Unity.@internal.Android
{
    internal class EndpointAndroid : Endpoint
    {
        private readonly AndroidJavaObject _nativeEndpoint;

        public EndpointAndroid(AndroidJavaObject nativeEndpoint)
        {
            _nativeEndpoint = nativeEndpoint;
            EndpointId = _nativeEndpoint.Call<string>("getEndpointId");
        }

        public override string EndpointId { get; }
        public override string UserName => _nativeEndpoint.Call<string>("getUserName");
        public override string UserDisplayName => _nativeEndpoint.Call<string>("getUserDisplayName");
        public override string SipUri => _nativeEndpoint.Call<string>("getSipUri");

        protected override VideoStream CreateVideoStream(string streamId)
        {
            var nativeStream = _nativeEndpoint.Call<AndroidJavaObject>("getVideoStream", streamId);
            if (nativeStream == null) return null;

            var videoStream = VoximplantSdk.CreateVideoStream<VideoStreamAndroid>();
            videoStream.SetNativeStream(nativeStream);
            return videoStream;
        }

        protected override void DisposeImpl()
        {
            _nativeEndpoint.Dispose();
        }
    }
}
