using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acn.Rdm.Packets.Net;
using Acn.Rdm.Packets.DMX;
using Acn.Rdm.Packets.Product;
using Acn.Rdm.Packets.Parameters;
using Acn.Rdm.Packets.Control;

namespace Acn.Rdm
{
    public static class RdmPacketFactory
    {
        static RdmPacketFactory()
        {
            RegisterCoreMessages();
            RegisterRdmNetMessages();
            RegisterProductMessages();
            RegisterDmxMessages();
            RegisterControlMessages();
        }

        private static void RegisterCoreMessages()
        {
            //SupportedParameters
            RegisterPacketType(RdmCommands.Get, RdmParameters.SupportedParameters, typeof(SupportedParameters.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.SupportedParameters, typeof(SupportedParameters.GetReply));

            //ParameterDescription
            RegisterPacketType(RdmCommands.Get, RdmParameters.ParameterDescription, typeof(ParameterDescription.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.ParameterDescription, typeof(ParameterDescription.GetReply));
        }

        private static void RegisterRdmNetMessages()
        {
            //Port List
            RegisterPacketType(RdmCommands.Get, RdmParameters.PortList, typeof(PortList.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.PortList, typeof(PortList.Reply));

            //Identify Port
            RegisterPacketType(RdmCommands.Get, RdmParameters.PortIdentify, typeof(PortList.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.PortIdentify, typeof(PortList.Reply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.PortIdentify, typeof(PortList.Reply));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.PortIdentify, typeof(PortList.Get));
        }

        private static void RegisterProductMessages()
        {
            //DeviceInfo
            RegisterPacketType(RdmCommands.Get, RdmParameters.DeviceInfo, typeof(DeviceInfo.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.DeviceInfo, typeof(DeviceInfo.GetReply));

            //ProductDetailIdList
            RegisterPacketType(RdmCommands.Get, RdmParameters.ProductDetailIdList, typeof(ProductDetailIdList.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.ProductDetailIdList, typeof(ProductDetailIdList.GetReply));

            //DeviceModelDescription
            RegisterPacketType(RdmCommands.Get, RdmParameters.DeviceModelDescription, typeof(DeviceModelDescription.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.DeviceModelDescription, typeof(DeviceModelDescription.GetReply));

            //ManufacturerLabel
            RegisterPacketType(RdmCommands.Get, RdmParameters.ManufacturerLabel, typeof(ManufacturerLabel.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.ManufacturerLabel, typeof(ManufacturerLabel.GetReply));

            //DeviceLabel
            RegisterPacketType(RdmCommands.Get, RdmParameters.DeviceLabel, typeof(DeviceLabel.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.DeviceLabel, typeof(DeviceLabel.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.DeviceLabel, typeof(DeviceLabel.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.DeviceLabel, typeof(DeviceLabel.SetReply));

            //FactoryDefaults
            RegisterPacketType(RdmCommands.Get, RdmParameters.FactoryDefaults, typeof(FactoryDefaults.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.FactoryDefaults, typeof(FactoryDefaults.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.FactoryDefaults, typeof(FactoryDefaults.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.FactoryDefaults, typeof(FactoryDefaults.SetReply));

            //LanguageCapabilities
            RegisterPacketType(RdmCommands.Get, RdmParameters.LanguageCapabilities, typeof(LanguageCapabilities.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.LanguageCapabilities, typeof(LanguageCapabilities.GetReply));

            //Language
            RegisterPacketType(RdmCommands.Get, RdmParameters.Language, typeof(Language.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.Language, typeof(Language.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.Language, typeof(Language.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.Language, typeof(Language.SetReply));

            //SoftwareVersionLabel
            RegisterPacketType(RdmCommands.Get, RdmParameters.SoftwareVersionLabel, typeof(SoftwareVersionLabel.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.SoftwareVersionLabel, typeof(SoftwareVersionLabel.GetReply));

            //BootSoftwareVersionId
            RegisterPacketType(RdmCommands.Get, RdmParameters.BootSoftwareVersionId, typeof(BootSoftwareVersionId.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.BootSoftwareVersionId, typeof(BootSoftwareVersionId.GetReply));

            //SoftwareVersionLabel
            RegisterPacketType(RdmCommands.Get, RdmParameters.SoftwareVersionLabel, typeof(SoftwareVersionLabel.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.SoftwareVersionLabel, typeof(SoftwareVersionLabel.GetReply));
        }

        private static void RegisterDmxMessages()
        {
            //DmxPersonality
            RegisterPacketType(RdmCommands.Get, RdmParameters.DmxPersonality, typeof(DmxPersonality.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.DmxPersonality, typeof(DmxPersonality.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.DmxPersonality, typeof(DmxPersonality.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.DmxPersonality, typeof(DmxPersonality.SetReply));

            //DmxPersonalityDescription
            RegisterPacketType(RdmCommands.Get, RdmParameters.DmxPersonalityDescription, typeof(DmxPersonalityDescription.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.DmxPersonality, typeof(DmxPersonalityDescription.GetReply));

            //DmxStartAddress
            RegisterPacketType(RdmCommands.Get, RdmParameters.DmxStartAddress, typeof(DmxStartAddress.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.DmxStartAddress, typeof(DmxStartAddress.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.DmxStartAddress, typeof(DmxStartAddress.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.DmxStartAddress, typeof(DmxStartAddress.SetReply));

            //SlotInfo
            RegisterPacketType(RdmCommands.Get, RdmParameters.SlotInfo, typeof(SlotInfo.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.SlotInfo, typeof(SlotInfo.GetReply));

            //SlotDescription
            RegisterPacketType(RdmCommands.Get, RdmParameters.SlotDescription, typeof(SlotDescription.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.SlotDescription, typeof(SlotDescription.GetReply));

            //DefaultSlotValue
            RegisterPacketType(RdmCommands.Get, RdmParameters.DefaultSlotValue, typeof(DefaultSlotValue.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.DefaultSlotValue, typeof(DefaultSlotValue.GetReply));
        }

        private static void RegisterControlMessages()
        {
            //IdentifyDevice
            RegisterPacketType(RdmCommands.Get, RdmParameters.IdentifyDevice, typeof(IdentifyDevice.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.IdentifyDevice, typeof(IdentifyDevice.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.IdentifyDevice, typeof(IdentifyDevice.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.IdentifyDevice, typeof(IdentifyDevice.SetReply));

            //ResetDevice
            RegisterPacketType(RdmCommands.Set, RdmParameters.ResetDevice, typeof(ResetDevice.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.ResetDevice, typeof(ResetDevice.SetReply));

            //PowerState
            RegisterPacketType(RdmCommands.Get, RdmParameters.PowerState, typeof(PowerState.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.PowerState, typeof(PowerState.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.PowerState, typeof(PowerState.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.PowerState, typeof(PowerState.SetReply));

            //PerformSelfTest
            RegisterPacketType(RdmCommands.Get, RdmParameters.PerformSelfTest, typeof(PerformSelfTest.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.PerformSelfTest, typeof(PerformSelfTest.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.PerformSelfTest, typeof(PerformSelfTest.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.PerformSelfTest, typeof(PerformSelfTest.SetReply));

            //SelfTestDescription
            RegisterPacketType(RdmCommands.Get, RdmParameters.SelfTestDescription, typeof(SelfTestDescription.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.SelfTestDescription, typeof(SelfTestDescription.GetReply));

            //CapturePreset
            RegisterPacketType(RdmCommands.Set, RdmParameters.CapturePreset, typeof(CapturePreset.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.CapturePreset, typeof(CapturePreset.SetReply));

            //PresetPlayback
            RegisterPacketType(RdmCommands.Get, RdmParameters.PresetPlayback, typeof(PresetPlayback.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.PresetPlayback, typeof(PresetPlayback.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.PresetPlayback, typeof(PresetPlayback.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.PresetPlayback, typeof(PresetPlayback.SetReply));
        }

        private struct PacketKey
        {
            public PacketKey(RdmCommands command, RdmParameters parameter)
            {
                this.Command = command;
                this.Parameter = parameter;
            }

            public RdmCommands Command;
            public RdmParameters Parameter;
        }

        private static Dictionary<PacketKey, Type> packetStore = new Dictionary<PacketKey, Type>();

        public static void RegisterPacketType(RdmCommands command, RdmParameters parameter, Type packetType)
        {
            PacketKey key = new PacketKey();
            key.Command = command;
            key.Parameter = parameter;

            packetStore[key] = packetType;
        }

        public static RdmPacket Build(RdmHeader header)
        {
            Type packetType;
            if (packetStore.TryGetValue(new PacketKey(header.Command, header.ParameterId), out packetType))
            {
                return RdmPacket.Create(header, packetType);
            }

            return null;
        }

    }
}
