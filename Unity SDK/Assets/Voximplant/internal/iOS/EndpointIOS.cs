/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Voximplant.Unity.Call;

namespace Voximplant.Unity.@internal.iOS
{
    internal class EndpointIOS : Endpoint
    {
        public EndpointIOS(string endpointId)
        {
            EndpointId = endpointId;
        }

        public override string EndpointId { get; }

        public override string UserName => voximplant_endpoint_username(EndpointId);

        public override string UserDisplayName => voximplant_endpoint_user_display_name(EndpointId);

        public override string SipUri => voximplant_endpoint_sip_uri(EndpointId);

        [DllImport("__Internal")]
        private static extern string voximplant_endpoint_username(string endpointId);

        [DllImport("__Internal")]
        private static extern string voximplant_endpoint_user_display_name(string endpointId);

        [DllImport("__Internal")]
        private static extern string voximplant_endpoint_sip_uri(string endpointId);

        protected override VideoStream CreateVideoStream(string streamId)
        {
            var videoStream = VoximplantSdk.CreateVideoStream<VideoStreamIOS>();
            videoStream.SetStreamId(streamId);
            return videoStream;
        }

        protected override void DisposeImpl()
        {
        }
    }
}
