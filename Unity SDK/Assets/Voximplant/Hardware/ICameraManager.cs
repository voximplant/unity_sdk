/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

namespace Voximplant.Unity.Hardware
{
    /// <summary>
    /// Interface that may be used to manage cameras on a device.
    /// </summary>
    public interface ICameraManager
    {
        /// <summary>
        /// Current active camera.
        /// </summary>
        CameraType Camera { get; set; }

        /// <summary>
        /// Camera will capture frames in a format that is as close as possible to width and height.
        /// </summary>
        /// <param name="width">Camera resolution width.</param>
        /// <param name="height">Camera resolution height.</param>
        void SetCameraResolution(int width, int height);
    }
}