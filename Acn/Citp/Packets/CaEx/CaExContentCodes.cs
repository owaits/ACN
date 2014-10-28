using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Citp.Packets.CaEx
{
    public enum CaExContentCodes:uint
    {
        Nack = 0xFFFFFFFF,
        GetLiveViewStatus = 0x100,
        LiveViewStatus = 0x101,
        GetLiveViewImage = 0x200,
        LiveViewImage = 0x201,
        SetCueRecordingCapabilities = 0x10100,
        RecordCue = 0x10200,
        SetRecorderClearingCapabilities = 0x10300,
        ClearRecorder = 0x10400,
        EnterShow = 0x20100,
        LeaveShow = 0x20101
    }
}
