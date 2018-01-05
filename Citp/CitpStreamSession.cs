using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;

namespace Citp
{
    public delegate void StreamFrameHandler(CitpStreamSession session);

    public class CitpStreamSession:IDisposable
    {
        private DateTime expiryTime = DateTime.Now;
        private StreamFrameHandler frameCallback = null;
        private Timer streamTimer;

        public CitpStreamSession()
        {
            streamTimer = new Timer(new TimerCallback(StreamPulse));
        }

        public bool Expired
        {
            get { return expiryTime < DateTime.Now; }
        }

        private int sourceIdentifier = 0;

        public int SourceIdentifier
        {
            get { return sourceIdentifier; }
            set { sourceIdentifier = value; }
        }


        public string FrameFormat { get; set; }

        public UInt16 FrameWidth { get; set; }

        public UInt16 FrameHeight { get; set; }

        public int FramesPerSecond { get; set; }

        private Bitmap streamCanvas = null;

        public Bitmap StreamCanvas
        {
            get { return streamCanvas; }
            set { streamCanvas = value; }
        }

        public int Interval
        {
            get
            {
                if (FramesPerSecond == 0) 
                    return 0;
                return 1000 / FramesPerSecond;
            }
        }
        
        public void Renew(int timeout, StreamFrameHandler frameCallback)
        {
            this.frameCallback = frameCallback;
            expiryTime = DateTime.Now.AddSeconds(timeout);

            streamTimer.Change(Interval, Timeout.Infinite);
        }

        public void Stop()
        {
            streamTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void StreamPulse(object state)
        {
             frameCallback(this);

            if (!Expired)
            {
                streamTimer.Change(Interval, Timeout.Infinite);
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
