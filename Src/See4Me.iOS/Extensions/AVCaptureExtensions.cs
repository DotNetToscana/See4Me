using System;
using System.Collections.Generic;
using System.Text;
using AVFoundation;

namespace See4Me.iOS.Extensions
{
    public static class AVCaptureExtensions
    {
        public static AVCaptureDevicePosition GetPosition(this AVCaptureInput avCaptureInput)
            => ((AVCaptureDeviceInput) avCaptureInput).Device.Position;
    }
}
