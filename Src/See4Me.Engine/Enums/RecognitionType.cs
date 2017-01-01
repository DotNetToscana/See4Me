using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace See4Me.Engine
{
    [Flags]
    public enum RecognitionType
    {
        Vision = 1,
        Face = 2,
        Emotion = 4,
        Text = 8,
        All = Vision | Face | Emotion | Text
    }
}