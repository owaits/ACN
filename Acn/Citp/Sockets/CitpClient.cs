using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using Citp.Packets;
using System.IO;
using Citp.Packets.Msex;
using Citp.Sockets;
using System.Net;
using Citp.IO;

namespace Citp.Sockets
{
    public class CitpClient:IDisposable
    {
        private TcpClient client = null;

        public event UnhandledExceptionEventHandler UnhandledException;
        public event EventHandler<CitpNewPacketEventArgs> NewPacket;
        public event EventHandler Disconnected;

        public CitpClient(TcpClient client)
        {
            this.client = client;            
        }

        public virtual void Start()
        {
            StartRecieve(new CitpRecieveData());
        }

        protected void RaiseDisconnected()
        {
            if (Disconnected != null)
                Disconnected(this, EventArgs.Empty);
        }

        protected void StartRecieve(CitpRecieveData recieveState)
        {
            try
            {
                recieveState.SetLength(recieveState.ReadPosition + recieveState.ReadNibble);
                client.Client.BeginReceive(recieveState.GetBuffer(), recieveState.ReadPosition, recieveState.ReadNibble, SocketFlags.None, new AsyncCallback(OnRecieve), recieveState);
            }
            catch (Exception ex)
            {
                OnUnhandledException(new ApplicationException("An error ocurred while trying to start recieving CITP.", ex));
            }
        }

        private void OnRecieve(IAsyncResult state)
        {
            CitpPacket newPacket;
            bool restartRecieve = false;

            CitpRecieveData recieveState = (CitpRecieveData)(state.AsyncState);

            try
            {
                if (recieveState != null && client != null && client.Connected)
                {
                    recieveState.SetLength((recieveState.Length - recieveState.ReadNibble) + client.Client.EndReceive(state));

                    if (recieveState.Length > 0 && !IsDisposed())
                    {
                        //We want to start the recieve again to listen for more data.
                        //Only do this when the client is in a position to do so.
                        restartRecieve = true;

                        if (NewPacket != null)
                        {
                            while (CitpPacketBuilder.TryBuild(recieveState, out newPacket))
                            {
                                recieveState.ReadPosition += (int)((CitpHeader)newPacket).MessageSize;

                                //Packet has been read successfully.
                                NewPacket(this, new CitpNewPacketEventArgs((IPEndPoint)client.Client.LocalEndPoint, (IPEndPoint) client.Client.RemoteEndPoint, newPacket));
                            }
                        }
                    }
                }
            }
            catch (SocketException)
            {
                //Connect has been closed.
            }
            catch (Exception ex)
            {
                OnUnhandledException(ex);
            }
            finally
            {
                //Attempt to recieve another packet.
                if (restartRecieve)
                    StartRecieve(recieveState);
                else
                    RaiseDisconnected();                   
            }
        }

        public void BeginSend(CitpPacket citpMessage)
        {
            MemoryStream data = new MemoryStream();
            CitpBinaryWriter writer = new CitpBinaryWriter(data);

            citpMessage.WriteData(writer);
            ((CitpHeader)citpMessage).WriteMessageSize(writer);

            client.Client.BeginSend(data.GetBuffer(), 0, (int) data.Length, SocketFlags.None, new AsyncCallback(OnSendCompleted), null);
        }

        private void OnSendCompleted(IAsyncResult result)
        {
        }
           
        protected void OnUnhandledException(Exception ex)
        {
            if (UnhandledException != null) UnhandledException(this, new UnhandledExceptionEventArgs((object)ex, false));
        }

        private bool disposed = false;

        public bool IsDisposed()
        {
            return disposed;
        }

        public virtual void Dispose()
        {
            disposed = true;
            if (client != null)
            {
                client.Close();
                client = null;
            }            
        }
    }
}
