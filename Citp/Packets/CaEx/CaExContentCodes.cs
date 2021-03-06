﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LXProtocols.Citp.Packets.CaEx
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
        LeaveShow = 0x20101,
        /// <summary>
        /// This message is sent unsolicited by the peer to Capture in order to choose which transformation space to use for fixtures. (0x20150)
        /// </summary>
        SetFixtureTransformationSpace = 0x20150,
        FixtureListRequest = 0x20200,
        FixtureList = 0x20201,
        FixtureModify = 0x20202,
        FixtureRemove = 0x20203,
        FixtureIdentify = 0x20204,
        FixtureSelection = 0x20300,
        FixtureConsoleStatus = 0x20400,
        GetLaserFeedList = 0x30100
    }

    public struct Coordinate
    {
        public float X;
        public float Y;
        public float Z;
    }
}
