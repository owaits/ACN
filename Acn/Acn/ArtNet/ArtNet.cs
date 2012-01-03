using System;
using System.Collections.Generic;
using System.Text;

namespace Acn.ArtNet
{
    public enum ArtNetOpCodes
    {
        Poll = 0x2000,
        PollReply = 0x2100,
        Dmx = 0x5000,
        TodRequest = 0x8000,
        TodData = 0x8100,
        TodControl = 0x8200,
        Rdm = 0x8300,
        RdmSub = 0x8400,


    }

    public enum ArtNetStyles
    {
        StNode = 0x00,
        StServer = 0x01,
        StMedia = 0x02,
        StRoute = 0x03,
        StBackup = 0x04,
        StConfig = 0x05
    }

    public enum ArtNetOemCodes
    {
        OemOarwSm1 = 0x09d0      //Company Name: Oarw, Product Name: Screen Monkey, Number of DMX Inputs: 0, Number of DMX Outputs: 1, Are dmx ports physical or emulated: Emulated, Is RDM Supported: Not at this time
    }
}
