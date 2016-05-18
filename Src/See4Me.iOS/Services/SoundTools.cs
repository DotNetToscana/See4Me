using System;
using System.Collections.Generic;
using System.Text;
using AudioToolbox;

namespace See4Me.Services
{
    public static class SoundTools
    {
        private const string NotificationSoundPath = "/System/Library/Audio/UISounds/photoShutter.caf";

        public static void TriggerSoundAndViber()
        {
            try
            {
                var notificationSound = SystemSound.FromFile(NotificationSoundPath);
                notificationSound.AddSystemSoundCompletion(SystemSound.Vibrate.PlaySystemSound);
                notificationSound.PlaySystemSound();
            }
            catch { }
        }
    }
}
