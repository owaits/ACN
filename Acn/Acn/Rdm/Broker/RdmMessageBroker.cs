using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acn.Sockets;
using System.Reflection;

namespace Acn.Rdm.Broker
{
    public delegate RdmPacket ProcessPacketHandler(RdmPacket requestPacket);

    public class RdmMessageBroker
    {
        private Dictionary<RdmParameters, RdmPacket> responsePackets = new Dictionary<RdmParameters, RdmPacket>();

        private Dictionary<RdmParameters, ProcessPacketHandler> packetGetHandlers = new Dictionary<RdmParameters, ProcessPacketHandler>();

        private Dictionary<RdmParameters, ProcessPacketHandler> packetSetHandlers = new Dictionary<RdmParameters, ProcessPacketHandler>();

        public void RegisterResponse(RdmParameters parameterId, RdmPacket responsePacket)
        {
            responsePackets[parameterId] = responsePacket;
        }

        public void RegisterHandler(RdmCommands command, RdmParameters parameterId, ProcessPacketHandler packetHandler)
        {
            switch(command)
            {
                case RdmCommands.Get:
                    packetGetHandlers[parameterId] = packetHandler;
                    break;
                case RdmCommands.Set:
                    packetSetHandlers[parameterId] = packetHandler;
                    break;
            }          
        }

        public void RegisterHandlers(object messageProvider)
        {
            foreach(MethodInfo method in messageProvider.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                foreach(object attribute in method.GetCustomAttributes(typeof(RdmMessageAttribute), true))
                {
                    RdmMessageAttribute messageAttribute = attribute as RdmMessageAttribute;
                    if(messageAttribute != null)
                    {
                        try 
                        {
                            ProcessPacketHandler handler = (ProcessPacketHandler) Delegate.CreateDelegate(typeof(ProcessPacketHandler), messageProvider, method.Name);
                            RegisterHandler(messageAttribute.Command,messageAttribute.ParameterId,handler);
                        }
                        catch (ArgumentException exception) 
                        {
                            throw new InvalidOperationException(string.Format("Failed to bind message handler for {1}.{2}. Did you declare it with the wrong delegate type?", method.DeclaringType.Name, method.Name), exception);
                        }
                    }
                }
            }
        }

        public RdmPacket ProcessPacket(RdmPacket packet)
        {
            RdmPacket responsePacket = null;
            ProcessPacketHandler handler;
            
            switch(packet.Header.Command)
            {
                case RdmCommands.Get:
                    if (!responsePackets.TryGetValue(packet.Header.ParameterId, out responsePacket))
                    {
                        if (packetGetHandlers.TryGetValue(packet.Header.ParameterId, out handler))
                        {
                            responsePacket = handler(packet);
                        }
                    }
                    break;
                case RdmCommands.Set:
                    if (packetGetHandlers.TryGetValue(packet.Header.ParameterId, out handler))
                    {
                        responsePacket = handler(packet);
                    }
                    break;
            }

            return responsePacket;
        }
    }
}
