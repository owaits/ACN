using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LXProtocols.Acn.Sockets;
using System.Reflection;
using LXProtocols.Acn.Rdm.Packets.Status;
using LXProtocols.Acn.Rdm.Packets.Management;
using LXProtocols.Acn.Rdm.Packets.Parameters;
using System.ComponentModel;

namespace LXProtocols.Acn.Rdm.Broker
{
    public delegate RdmPacket ProcessPacketHandler(RdmPacket requestPacket);

    /// <summary>
    /// The staqtus of a parameter value.
    /// </summary>
    public enum ParameterStatus
    {
        /// <summary>
        /// The parameter is invalid or has not been set.
        /// </summary>
        Empty,
        /// <summary>
        /// The parameter has been request but not yet recieved.
        /// </summary>
        Pending,
        /// <summary>
        /// The parameter is up to date.
        /// </summary>
        Valid
    }

    public class RdmMessageBroker
    {
        private Dictionary<RdmParameters, RdmPacket> responsePackets = new Dictionary<RdmParameters, RdmPacket>();

        private Dictionary<RdmParameters, ProcessPacketHandler> packetGetHandlers = new Dictionary<RdmParameters, ProcessPacketHandler>();
        private Dictionary<RdmParameters, ProcessPacketHandler> packetGetResponseHandlers = new Dictionary<RdmParameters, ProcessPacketHandler>();

        private Dictionary<RdmParameters, ProcessPacketHandler> packetSetHandlers = new Dictionary<RdmParameters, ProcessPacketHandler>();
        private Dictionary<RdmParameters, ProcessPacketHandler> packetSetResponseHandlers = new Dictionary<RdmParameters, ProcessPacketHandler>();

        public RdmMessageBroker()
        {
            RegisterResponse(RdmParameters.StatusMessage, StatusReply);
            RegisterHandler(RdmCommands.Get, RdmParameters.SupportedParameters, new ProcessPacketHandler(ProcessSupportedParameters));
        }

        private bool autoNack = false;

        [Browsable(false)]
        public bool AutoNack
        {
            get { return autoNack; }
            set { autoNack = value; }
        }

        private StatusMessage.GetReply statusReply = new StatusMessage.GetReply();

        [Browsable(false)]
        public StatusMessage.GetReply StatusReply
        {
            get { return statusReply; }
        }

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
                case RdmCommands.GetResponse:
                    packetGetResponseHandlers[parameterId] = packetHandler;
                    break;
                case RdmCommands.Set:
                    packetSetHandlers[parameterId] = packetHandler;
                    break;
                case RdmCommands.SetResponse:
                    packetSetResponseHandlers[parameterId] = packetHandler;
                    break;
                default:
                    throw new NotSupportedException(string.Format("The packet command type {0} is not supported yet.",command.ToString()));
            }          
        }

        public void RegisterHandlers(object messageProvider)
        {
            foreach(MethodInfo method in messageProvider.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy))
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

        public void RegisterPersonality(RdmPersonality personality)
        {
            RegisterHandlers(personality);
        }

        public virtual RdmPacket ProcessPacket(RdmPacket packet)
        {
            RdmPacket responsePacket = null;
            ProcessPacketHandler handler;

            switch(packet.Header.Command)
            {
                case RdmCommands.Get:
                    responsePacket = DequeueOverflow(packet);
                    if (responsePacket == null && !responsePackets.TryGetValue(packet.Header.ParameterId, out responsePacket))
                    {
                        if (packetGetHandlers.TryGetValue(packet.Header.ParameterId, out handler))
                        {
                            responsePacket = handler(packet);

                            if (responsePacket != null && responsePacket.Header.Command == RdmCommands.GetResponse)
                                responsePacket.Header.TransactionNumber = packet.Header.TransactionNumber;

                        }
                    }
                    break;
                case RdmCommands.GetResponse:
                    if (packetGetResponseHandlers.TryGetValue(packet.Header.ParameterId, out handler))
                    {
                        responsePacket = handler(packet);
                        
                    }
                    break;
                case RdmCommands.Set:
                    responsePacket = DequeueOverflow(packet);
                    if (responsePacket == null && packetSetHandlers.TryGetValue(packet.Header.ParameterId, out handler))
                    {
                        responsePacket = handler(packet);

                        if (responsePacket != null && responsePacket.Header.Command == RdmCommands.SetResponse)
                            responsePacket.Header.TransactionNumber = packet.Header.TransactionNumber;
                    }
                    break;
                case RdmCommands.SetResponse:
                    if (packetSetResponseHandlers.TryGetValue(packet.Header.ParameterId, out handler))
                    {
                        responsePacket = handler(packet);
                    }
                    break;
            }

            //When an overflow is recieved automatically request the rest of the packet.
            if (packet.IsOverflow())
                responsePacket = ProcessOverflow(packet);

            if (AutoNack && responsePacket == null)
            {
                //Automatically generate a Nack message for the unsupported packet.
                responsePacket = new RdmNack((RdmCommands) (packet.Header.Command + 1), packet.Header.ParameterId);
            }

            if (responsePacket != null)
                responsePacket.Header.TransactionNumber = packet.Header.TransactionNumber;


            return responsePacket;
        }

        public RdmPacket ProcessSupportedParameters(RdmPacket packet)
        {
            SupportedParameters.Get parameterRequest = packet as SupportedParameters.Get;
            if (parameterRequest != null)
            {
                SupportedParameters.GetReply parameterReply = new SupportedParameters.GetReply();
                
                //Add the supported parameter Ids
                parameterReply.ParameterIds.AddRange(responsePackets.Keys);
                parameterReply.ParameterIds.AddRange(packetGetHandlers.Keys);
                parameterReply.ParameterIds.AddRange(packetSetHandlers.Keys);

                return parameterReply;
            }

            return null;
        }

        public RdmPacket ProcessOverflow(RdmPacket packet)
        {
            //Create an overflow response header to obtain the rest of the data.
            RdmHeader header = new RdmHeader();
            header.ParameterId = packet.Header.ParameterId;
            header.Command = (packet.Header.Command == RdmCommands.SetResponse ? RdmCommands.Set : RdmCommands.Get); 

            //Create a request packet to obtain the remaining data for the overflow.
            return RdmPacket.Create(header);
        }

        #region Overflow

        private Dictionary<UId, Queue<RdmPacket>> overflowQueues = new Dictionary<UId, Queue<RdmPacket>>();

        protected RdmPacket QueueOverflow(RdmPacket request, Queue<RdmPacket> overflowPackets)
        {
            overflowQueues[request.Header.SourceId] = overflowPackets;
            return DequeueOverflow(request);
        }

        protected RdmPacket DequeueOverflow(RdmPacket request)
        {
            Queue<RdmPacket> messages;
            if (overflowQueues.TryGetValue(request.Header.SourceId, out messages))
            {
                RdmPacket reply = messages.Dequeue();

                //If a different parameter is being requested then dump 
                if (reply.Header.ParameterId != request.Header.ParameterId)
                {
                    overflowQueues.Remove(request.Header.SourceId);
                    return null;
                }

                reply.Header.PortOrResponseType = (byte) (messages.Count == 0 ? RdmResponseTypes.Ack : RdmResponseTypes.AckOverflow);

                if (messages.Count == 0)
                    overflowQueues.Remove(request.Header.SourceId);

                return reply;
            }

            return null;
        }

        #endregion


        #region Parameter Status

        private Dictionary<RdmParameters, ParameterStatus> parameterStatus = new Dictionary<RdmParameters, ParameterStatus>();

        protected void SetParameterStatus(RdmParameters parameter,ParameterStatus status)
        {
            parameterStatus[parameter] = status;
        }

        protected ParameterStatus GetParameterStatus(RdmParameters parameter)
        {
            ParameterStatus status;
            if (parameterStatus.TryGetValue(parameter, out status))
                return status;

            return ParameterStatus.Empty;
        }

        protected bool IsParameterStatus(RdmParameters parameter,ParameterStatus status)
        {
            return GetParameterStatus(parameter) == status;
        }

        #endregion
    }
}
