using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using LXProtocols.Citp.Packets.Msex;
using LXProtocols.Citp.Packets.CaEx;
using LXProtocols.Citp.Sockets;
using LXProtocols.Citp.Packets.PInf;
using LXProtocols.Citp.Packets.SDmx;
using LXProtocols.Citp.Packets.FPtc;
using LXProtocols.Citp.Packets.FSel;
using System.Runtime.CompilerServices;
using LXProtocols.Citp.Packets.FInf;
[assembly: InternalsVisibleTo("LXProtocols.Citp.Test")]

namespace LXProtocols.Citp.Packets
{
    public class CitpPacketBuilder
    {
        internal static bool TryBuild(CitpRecieveData data,out CitpPacket packet)
        {
            CitpHeader header = new CitpHeader(string.Empty);
            packet = header;

            try
            {
                //We have read all the data.
                if (data.EndOfData())
                {
                    data.Reset();
                    return false;
                }

                //Check we have enough data to construct the header.
                if (data.Length - data.ReadPosition < CitpHeader.PacketSize)
                    return false;

                //Read the packet header.
                header.ReadData(data.GetReader());

                //Ensure the header packet is valid
                if (!header.IsValid())
                {
                    //Purge data as it is probably corrupted.
                    data.Reset();        //Reset position to start so we dump the data.
                    return false;
                }

                //Read the sub packet
                switch (header.ContentType)
                {
                    case CitpPInfHeader.PacketType:
                        packet = BuildPInf(data);
                        break;
                    case SDmxHeader.PacketType:
                        packet = BuildSDmx(data);
                        break;
                    case FPtcHeader.PacketType:
                        packet = BuildFPtc(data);
                        break;
                    case FSelHeader.PacketType:
                        packet = BuildFSel(data);
                        break;
                    case CitpMsexHeader.PacketType:
                        packet = BuildMsex(data);
                        break;                    
                    case CaExHeader.PacketType:
                        packet = BuildCaEx(data);
                        break;
                    case FInfHeader.PacketType:
                        packet = BuildFInf(data);
                        break;
                    default:
                        packet = null;
                        break;
                }

                //Advance the read and write pointers past the successfully read packet.
                data.ReadPosition += header.MessageSize;
            }
            catch (EndOfStreamException)
            {
                return false;
            }

            //We have managed to read the packet even if it was unknown and set to null so lets return true
            return true;
        }

        private static CitpPacket BuildPInf(CitpRecieveData data)
        {
            CitpPInfHeader header = new CitpPInfHeader();
            header.ReadData(data.GetReader());

            switch (header.LayerContentType)
            {
                case CitpPInfPeerLocation.PacketType:
                    return new CitpPInfPeerLocation(data.GetReader());
                case CitpPInfPeerName.PacketType:
                    return new CitpPInfPeerName(data.GetReader());
            }

            return null;

        }

        private static CitpPacket BuildFPtc(CitpRecieveData data)
        {
            FPtcHeader header = new FPtcHeader(string.Empty);
            header.ReadData(data.GetReader());

            CitpPacket packet = null;
            switch (header.FPtcContentType)
            {
                case FPtcPatch.PacketType:
                    packet = new FPtcPatch();
                    break;
                case FPtcUnpatch.PacketType:
                    packet = new FPtcUnpatch();
                    break;
                case FPtcSendPatch.PacketType:
                    packet = new FPtcSendPatch();
                    break;
                default:
                    return null;
            }

            packet.ReadData(data.GetReader());
            return packet;
        }

        private static CitpPacket BuildFSel(CitpRecieveData data)
        {
            FSelHeader header = new FSelHeader(string.Empty);
            header.ReadData(data.GetReader());

            CitpPacket packet = null;
            switch (header.FSelContentType)
            {
                case FSelSelect.PacketType:
                    packet = new FSelSelect();
                    break;
                case FSelDeselect.PacketType:
                    packet = new FSelDeselect();
                    break;
                default:
                    return null;
            }

            packet.ReadData(data.GetReader());
            return packet;
        }

        private static CitpPacket BuildSDmx(CitpRecieveData data)
        {
            SDmxHeader header = new SDmxHeader(string.Empty);
            header.ReadData(data.GetReader());

            CitpPacket packet = null;
            switch (header.SDmxContentType)
            {
                case SDmxCapabilities.PacketType:
                    packet = new SDmxCapabilities();
                    break;
                
                case SDmxEncryptionIdentifier.PacketType:
                    packet = new SDmxEncryptionIdentifier();
                    break;
                case SDmxUniverseName.PacketType:
                    packet = new SDmxUniverseName();
                    break;
                case SDmxChannelBlock.PacketType:
                    packet = new SDmxChannelBlock();
                    break;
                case SDmxSetExternalSource.PacketType:
                    packet = new SDmxSetExternalSource();
                    break;
                case SDmxChannelList.PacketType:
                    packet = new SDmxChannelList();
                    break;
                default:
                    return null;
            }

            packet.ReadData(data.GetReader());
            return packet;
        }

        private static CitpPacket BuildMsex(CitpRecieveData data)
        {
            CitpMsexHeader header = new CitpMsexHeader();
            header.ReadData(data.GetReader());

            switch (header.LayerContentType)
            {
                case CitpMsexClientInformation.PacketType:
                    return new CitpMsexClientInformation(data.GetReader());
                case CitpMsexServerInformation.PacketType:
                    return new CitpMsexServerInformation(data.GetReader());
                case CitpMsexLayerStatus.PacketType:
                    return new CitpMsexLayerStatus(data.GetReader());
                case CitpMsexNack.PacketType:
                    return new CitpMsexNack(data.GetReader());
                case CitpMsexGetElementLibraryInformation.PacketType:
                    return new CitpMsexGetElementLibraryInformation(data.GetReader());
                case CitpMsexElementLibraryInformation.PacketType:
                    return new CitpMsexElementLibraryInformation(data.GetReader());
                case CitpMsexElementLibraryUpdated.PacketType:
                    return new CitpMsexElementLibraryUpdated(data.GetReader());
                case CitpMsexGetElementInformation.PacketType:
                    return new CitpMsexGetElementInformation(data.GetReader());
                case CitpMsexMediaElementInformation.PacketType:
                    return new CitpMsexMediaElementInformation(data.GetReader());
                case CitpMsexEffectElementInformation.PacketType:
                    return new CitpMsexEffectElementInformation(data.GetReader());
                case CitpMsexGenericElementInformation.PacketType:
                    return new CitpMsexGenericElementInformation(data.GetReader());
                case CitpMsexGetElementLibraryThumbnail.PacketType:
                    return new CitpMsexGetElementLibraryThumbnail(data.GetReader());
                case CitpMsexElementLibraryThumbnail.PacketType:
                    return new CitpMsexElementLibraryThumbnail(data.GetReader());
                case CitpMsexGetElementThumbnail.PacketType:
                    return new CitpMsexGetElementThumbnail(data.GetReader());
                case CitpMsexElementThumbnail.PacketType:
                    return new CitpMsexElementThumbnail(data.GetReader());
                case CitpMsexGetVideoSources.PacketType:
                    return new CitpMsexGetVideoSources(data.GetReader());
                case CitpMsexVideoSources.PacketType:
                    return new CitpMsexVideoSources(data.GetReader());
                case CitpMsexRequestStream.PacketType:
                    return new CitpMsexRequestStream(data.GetReader());
                case CitpMsexStreamFrame.PacketType:
                    return new CitpMsexStreamFrame(data.GetReader());

            }

            return null;

        }

        private static CitpPacket BuildCaEx(CitpRecieveData data)
        {
            CaExHeader header = new CaExHeader(0x0);
            header.ReadData(data.GetReader());

            CitpPacket packet = null;
            switch (header.ContentCode)
            {
                case CaExContentCodes.Nack:
                    packet = new CaExNack();
                    break;
                case CaExContentCodes.GetLiveViewStatus:
                    packet = new CaExGetLiveViewStatus();
                    break;
                case CaExContentCodes.LiveViewStatus:
                    packet = new CaExLiveViewStatus();
                    break;
                case CaExContentCodes.GetLiveViewImage:
                    packet = new CaExGetLiveViewImage();
                    break;
                case CaExContentCodes.LiveViewImage:
                    packet = new CaExLiveViewImage();
                    break;
                case CaExContentCodes.SetCueRecordingCapabilities:
                    packet = new CaExSetCueRecordingCapabilities();
                    break;
                case CaExContentCodes.RecordCue:
                    packet = new CaExRecordCue();
                    break;
                case CaExContentCodes.SetRecorderClearingCapabilities:
                    packet = new CaExSetRecorderClearingCapabilities();
                    break;
                case CaExContentCodes.ClearRecorder:
                    packet = new CaExClearRecorder();
                    break;
                case CaExContentCodes.EnterShow:
                    packet = new CaExEnterShow();
                    break;
                case CaExContentCodes.LeaveShow:
                    packet = new CaExLeaveShow();
                    break;
                case CaExContentCodes.SetFixtureTransformationSpace:
                    packet = new CaExSetFixtureTransformationSpace();
                    break;
                case CaExContentCodes.FixtureConsoleStatus:
                    packet = new CaExFixtureConsoleStatus();
                    break;
                case CaExContentCodes.FixtureListRequest:
                    packet = new CaExFixtureListRequest();
                    break;
                case CaExContentCodes.FixtureList:
                    packet = new CaExFixtureList();
                    break;
                case CaExContentCodes.FixtureModify:
                    packet = new CaExFixtureModify();
                    break;
                case CaExContentCodes.FixtureRemove:
                    packet = new CaExFixtureRemove();
                    break;
                case CaExContentCodes.FixtureSelection:
                    packet = new CaExFixtureSelection();
                    break;
                case CaExContentCodes.FixtureIdentify:
                    packet = new CaExFixtureIdentify();
                    break;
                case CaExContentCodes.GetLaserFeedList:
                    //Not implemented yet, left here as placeholder.
                    return null;
                default:
                    return null;
            }

            packet.ReadData(data.GetReader());
            return packet;

        }

        /// <summary>
        /// Builds the f inf.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        private static CitpPacket BuildFInf(CitpRecieveData data)
        {
            FInfHeader header = new FInfHeader();
            header.ReadData(data.GetReader());

            switch (header.FInfContentType)
            {
                case FInfFrames.PacketType:
                    return new FInfFrames(data.GetReader());
                case FInfSendFrames.PacketType:
                    return new FInfSendFrames(data.GetReader());
            }

            return null;

        }

    }
}
