using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.IO;

namespace AcnDataMatrix
{
    class MatrixWindow : GameWindow
    {        
        bool viewport_changed = true;
        int viewport_width, viewport_height;
        bool exit = false;
        Thread rendering_thread;
        object data_lock = new object();
        object update_lock = new object();
        int universeCount;
        int fixtureCount=170;
        private static AutoResetEvent waitData = new AutoResetEvent(false);
        
        private const int windowSize = 5;
        List<UniverseData> dataWindow;
        class UniverseData
        {
            public UniverseData(int universes)
            {
                Data = new List<ChannelData>();
                for (int i = 0; i < universes; i++)
                {
                    Data.Add(new ChannelData());
                }
            }

            public void ResetData()
            {
                foreach (ChannelData item in Data)
                {
                    item.Set = false;
                }
            }

            public long ArrivalTime{get;set;}
            public bool Ready { get; set; }
            public List<ChannelData> Data { get; set; }

            public class ChannelData
            {
                public bool Set { get; set; }

                private byte[] data = new byte[513];

                public byte[] Data
                {
                    get { return data; }
                    set { data = value; }
                }
            }
        }

        //a read write lock. We're going to use this backwards.
        //as we want to allow for synchronous reads.
        private ReaderWriterLockSlim dataLock = new ReaderWriterLockSlim();
        private List<object> singleUniverseLock;

        private long updateTime = 0;

        public long UpdateTime1
        {
            get { return updateTime; }
            set { updateTime = value; }
        }

        private byte[] cacheData;
        private byte[] channelData; 

        /// <summary>
        /// Initializes a new instance of the <see cref="MatrixWindow"/> class.
        /// Create handlers for Esc, F11, Resize and Mouse Operations.
        /// </summary>
        public MatrixWindow(int universeCount, bool rgbFixture, string winName)
            : base(800, 600, GraphicsMode.Default, winName)
        {
            this.universeCount = universeCount;
            channelData = new byte[universeCount*fixtureCount*3];
            cacheData = new byte[universeCount * fixtureCount * 3];
            singleUniverseLock = new List<object>();
            dataWindow = new List<UniverseData>(windowSize);
            for(int j =0; j < windowSize; j++)
            {
                dataWindow.Add(new UniverseData(universeCount));
            }

                Keyboard.KeyDown += delegate(object sender, KeyboardKeyEventArgs e)
                {
                    if (e.Key == Key.Escape)
                        this.Exit();
                };

            Keyboard.KeyUp += delegate(object sender, KeyboardKeyEventArgs e)
            {
                if (e.Key == Key.F11)
                    if (this.WindowState == WindowState.Fullscreen)
                        this.WindowState = WindowState.Normal;
                    else
                        this.WindowState = WindowState.Fullscreen;
            };

            Resize += delegate(object sender, EventArgs e)
            {
                // Note that we cannot call any OpenGL methods directly. What we can do is set
                // a flag and respond to it from the rendering thread.
                lock (update_lock)
                {
                    viewport_changed = true;
                    viewport_width = Width;
                    
                    viewport_height = Height;
                    
                }
            };
        }

        #region OnLoad

        /// <summary>
        /// Setup OpenGL and load resources here.
        /// </summary>
        /// <param name="e">Not used.</param>
        protected override void OnLoad(EventArgs e)
        {
            Context.MakeCurrent(null); // Release the OpenGL context so it can be used on the new thread.


            GL.GenTextures(1, out _texture);

            rendering_thread = new Thread(RenderLoop);
            rendering_thread.IsBackground = true;
            rendering_thread.Start();
        }

        #endregion

        #region OnUnload

        /// <summary>
        /// Release resources here.
        /// </summary>
        /// <param name="e">Not used.</param>
        protected override void OnUnload(EventArgs e)
        {
            exit = true; // Set a flag that the rendering thread should stop running.
            rendering_thread.Join();

            base.OnUnload(e);

            GL.DeleteTextures(1, ref _texture);
            //debugInput.Close();
        }

        #endregion

        #region OnUpdateFrame

        /// <summary>
        /// Add your game logic here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        /// <remarks>There is no need to call the base implementation.</remarks>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            // Nothing to do!
        }

        #endregion

        #region OnRenderFrame

        /// <summary>
        /// Ignored. All rendering is performed on our own rendering function.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        /// <remarks>There is no need to call the base implementation.</remarks>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            // Nothing to do. Release the CPU to other threads.
            Thread.Sleep(1);
        }

        #endregion

        #region RenderLoop

        /// <summary>
        /// The length of time the render loop should pause for on each pass.
        /// </summary>
        private TimeSpan renderPause = TimeSpan.Zero;

        void RenderLoop()
        {
            MakeCurrent(); // The context now belongs to this thread. No other thread may use it!

            VSync = VSyncMode.On;
                       
            GL.ClearColor(Color.Black);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.PointSmooth);

            Stopwatch update_watch = new Stopwatch();
                                    
            while (!exit)
            {
                update_watch.Start();

                //redraw the screen every 1/2 second when there is no data
                waitData.WaitOne(500);
                //give some cpu time back. 
                //Also wait here rather than the end of the loop to give any other universes time to arrive
                Thread.Sleep(renderPause);                

                Render();
                SwapBuffers();
                                
                UpdateTime1 = update_watch.ElapsedMilliseconds;
                update_watch.Reset();
                waitData.Reset();                
            }

            Context.MakeCurrent(null);
        }

        #endregion


        /// <summary>
        /// Refernce to the texture used in the render loop.
        /// </summary>
        private int _texture;

        #region Render
        //private Matrix4 perspective = Matrix4.CreateOrthographic(2, 2, -1, 1);
        /// <summary>
        /// This is our main rendering function, which executes on the rendering thread.
        /// </summary>
        public void Render()
        {
            lock (update_lock)
            {
                if (viewport_changed)
                {
                    GL.Viewport(0, 0, viewport_width, viewport_height);
                    viewport_changed = false;
                }
            }                        

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, _texture);
            
            //next 2 lines seem to unintentionally make the texture brighter.
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

            lock(channelData)
            {
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, fixtureCount, universeCount, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgb, PixelType.UnsignedByte, channelData);
            }
      
            GL.Begin(PrimitiveType.Quads);

            GL.TexCoord2(0,0);
            GL.Vertex2((float)-1, (float)1);
            GL.TexCoord2(1, 0);
            GL.Vertex2((float)1, (float)1);
            GL.TexCoord2(1, 1);
            GL.Vertex2((float)1, (float)-1);
            GL.TexCoord2(0, 1);
            GL.Vertex2((float)-1, (float)-1);

            GL.End();
            GL.Disable(EnableCap.Texture2D);
        }

        #endregion

        /// <summary>
        /// The recieved universes
        /// </summary>
        int recievedUniverses = 0;


        /// <summary>
        /// Updates the data.
        /// </summary>
        /// <param name="universe">The universe.</param>
        /// <param name="data">The data.</param>
        public void UpdateData(int universe, byte[] data)
        {          
            lock(cacheData)
            {
                Array.Copy(data, 1, cacheData, universe * fixtureCount * 3, fixtureCount * 3);
                recievedUniverses++;
            }

        }

        /// <summary>
        /// Swaps the buffers over. 
        /// </summary>
        /// <param name="pauseBeforeRender">The pause before render.</param>
        public void Draw(TimeSpan pauseBeforeRender)
        {
            if (recievedUniverses == 0)
                return;

            lock (cacheData)
            {
                lock(channelData)
                {
                    //Flip the buffers
                    Array.Copy(cacheData, channelData, cacheData.Length);
                    Console.WriteLine(recievedUniverses);
                    recievedUniverses = 0;
                }
            }

            renderPause = pauseBeforeRender;
            waitData.Set();  
        }
    }
}
