using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ITR
{
    /// <summary>
    /// Class for storage of original image<br/>
    /// and generating events when output image is rendered.<br/><br/>
    /// Management is done by <seealso cref="RenderController"/>
    /// </summary>
    [Obsolete(message:"This is slow trash. You been warned")]
    [System.Runtime.Versioning.SupportedOSPlatform(platformName: "windows")]
    public sealed class ItemRenderer
    {
        /// <summary>
        /// Delegate for TextureUpdated events
        /// </summary>
        /// <param name="texture">newly generated texture</param>
        public delegate void EventHandler_TextureUpdated(Bitmap texture);
        /// <summary>
        /// Event generated when new texture is rendered and ready for use
        /// </summary>
        public event EventHandler_TextureUpdated TextureUpdated;

        /// <summary>
        /// Image used for rendering output texture
        /// </summary>
        public Image texture_Original;
        /// <summary>
        /// Last rendered texture
        /// </summary>
        public Bitmap texture_Output;
        int uid;

        WeakReference _controller;
        /// <summary>
        /// Owner of this ItemRenderer
        /// </summary>
        public RenderController Controller { get => _controller.Target as RenderController; }

        int _rendered;
        /// <summary>
        /// Is item enabled for rendering
        /// </summary>
        public bool Rendered
        {
            get => Interlocked.Exchange(ref _rendered, _rendered) != 0;
            set => Interlocked.Exchange(ref _rendered, (value?1:0));
        }

        int _glow;
        /// <summary>
        /// Is item glowing(enchanted)
        /// </summary>
        public bool Glow
        {
            get => Interlocked.Exchange(ref _glow, _glow) != 0;
            set => Interlocked.Exchange(ref _glow, (value?1:0));
        }

        ItemRenderer(RenderController renderController, Image texture, int uid)
        {
            _controller = new(renderController);
            texture_Original = texture;
            Rendered = true;
            Glow = false;
            this.uid = uid;
        }

        ItemRenderer(RenderController renderController, Item item, int uid)
        {
            _controller = new(renderController);
            texture_Original = item.Texture;
            Rendered = true;
            Glow = item.Glow;
            this.uid = uid;
        }

        /// <summary>
        /// Change the image that is used for rendering
        /// </summary>
        /// <param name="img">Image to render</param>
        public void Update(Image img)
        {
            Interlocked.Exchange(ref texture_Original, img);
        }

        /// <summary>
        /// Change the image that is used for rendering from <see cref="Item"/><br/>
        /// also Glow is updated from <see cref="Item"/>
        /// </summary>
        /// <param name="item">Item to render</param>
        public void Update(Item item)
        {
            Interlocked.Exchange(ref texture_Original, item.Texture);
            Glow = item.Glow;
        }

        void OnTextureUpdated()
        {
            EventHandler_TextureUpdated handler = TextureUpdated;
            handler?.Invoke(new(Interlocked.Exchange(ref texture_Output, texture_Output)));
        }

        /// <summary>
        /// Class responsible for:<br/>
        /// Controlling FPS used to render <see cref="ItemRenderer"/><br/>
        /// Rendering <see cref="ItemRenderer"/>s<br/>
        /// Creating and removing <see cref="ItemRenderer"/>s to render<br/>
        /// </summary>
        public sealed class RenderController
        {
            /// <summary>
            /// Color of Glow Tint<br/>
            /// IMPORTANT: when changed use <see cref="RerenderGlowTexture"/> to see result!
            /// </summary>
            public Color Glow_Tint { get; set; }
            /// <summary>
            /// Rotation used to create Glow TextureBrush<br/>
            /// IMPORTANT: when changed use <see cref="RerenderGlowTexture"/> to see result!
            /// </summary>
            public float Glow_TextureRotation { get; set; }
            /// <summary>
            /// Scale used to create Glow TextureBrush <br/>
            /// IMPORTANT: when changed use <see cref="RerenderGlowTexture"/> to see result!
            /// </summary>
            public SizeF Glow_TextureScale { get; set; }
            int _outputImage_Resolution_W;
            int _outputImage_Resolution_H;
            /// <summary>
            /// Resolution used to render <see cref="ItemRenderer"/> Textures<br/>
            /// Note that base Image used for rendering is just scaled to requested resolution, Glow Texture also
            /// </summary>
            public Size OutputImage_Resolution
            {
                get
                {
                    return new(Interlocked.Exchange(ref _outputImage_Resolution_W, _outputImage_Resolution_W),
                       Interlocked.Exchange(ref _outputImage_Resolution_H, _outputImage_Resolution_H));
                }
                set
                {
                    Interlocked.Exchange(ref _outputImage_Resolution_W, value.Width);
                    Interlocked.Exchange(ref _outputImage_Resolution_H, value.Height);
                }
            }

            int _glow_Resolution_W;
            int _glow_Resolution_H;
            /// <summary>
            /// Resolution used to render Glow Texture
            /// </summary>
            public Size Glow_Resolution
            {
                get
                {
                    return new(Interlocked.Exchange(ref _glow_Resolution_W, _glow_Resolution_W),
                       Interlocked.Exchange(ref _glow_Resolution_H, _glow_Resolution_H));
                }
                set
                {
                    Interlocked.Exchange(ref _glow_Resolution_W, value.Width);
                    Interlocked.Exchange(ref _glow_Resolution_H, value.Height);
                }
            }

            float _glow_Translation_X;
            float _glow_Translation_Y;
            /// <summary>
            /// Value controlling move per second of Glow TextureBrush
            /// </summary>
            public PointF Glow_TranslationPerSecond
            {
                get
                {
                    return new(Interlocked.Exchange(ref _glow_Translation_X, _glow_Translation_X),
                       Interlocked.Exchange(ref _glow_Translation_Y, _glow_Translation_Y));
                }
                set
                {
                    Interlocked.Exchange(ref _glow_Translation_X, value.X);
                    Interlocked.Exchange(ref _glow_Translation_Y, value.Y);
                }
            }

            int frame;
            int _glow_TargetFPS;
            /// <summary>
            /// Glow rerender rate. Best same as <see cref="Render_TargetFPS"/>
            /// </summary>
            public int Glow_TargetFPS
            {
                get => Interlocked.Exchange(ref _glow_TargetFPS, _glow_TargetFPS);
                set => Interlocked.Exchange(ref _glow_TargetFPS, value);
            }

            int _gif_TargetFPS;
            /// <summary>
            /// Gif frames per second
            /// </summary>
            public int Gif_TargetFPS
            {
                get => Interlocked.Exchange(ref _gif_TargetFPS, _gif_TargetFPS);
                set => Interlocked.Exchange(ref _gif_TargetFPS, value);
            }

            int _render_TargetFPS;
            /// <summary>
            /// <see cref="Item"/> rerendering rate. Best same as <see cref="Glow_TargetFPS"/>
            /// </summary>
            public int Render_TargetFPS
            {
                get => Interlocked.Exchange(ref _render_TargetFPS, _render_TargetFPS);
                set => Interlocked.Exchange(ref _render_TargetFPS, value);
            }

            int uid = 0;

            Bitmap _glow_Texture;
            /// <summary>
            /// Glow Texture.<br/>
            /// If u want Glow Texture, here have one
            /// </summary>
            public Bitmap Glow_Texture { get => Interlocked.Exchange(ref _glow_Texture, _glow_Texture); }
            TextureBrush _glow_TextureBrush;

            Thread glowUpdate_Thread;
            int glowUpdate_Thread_work = 0;

            void UpdateGlow()
            {
                Bitmap glow_Buffer;
                DateTime lastBrushUpdate = DateTime.Now;
                DateTime codeTime = DateTime.Now;
                while (Interlocked.Exchange(ref glowUpdate_Thread_work, glowUpdate_Thread_work) != 0)
                {
                    int w = Interlocked.Exchange(ref _glow_Resolution_W, _glow_Resolution_W);
                    int h = Interlocked.Exchange(ref _glow_Resolution_H, _glow_Resolution_H);

                    glow_Buffer = new(w, h);
                    Graphics glow_Draw = Graphics.FromImage(glow_Buffer);
                    glow_Draw.FillRectangle(
                        _glow_TextureBrush,
                        new Rectangle(0, 0, w, h)
                        );
                    glow_Draw.Flush();
                    glow_Draw.Save();
                    glow_Draw.Dispose();

                    Interlocked.Exchange(ref _glow_Texture, glow_Buffer);
                    TimeSpan elapsed = DateTime.Now - lastBrushUpdate; // elapsed from last Brush update
                    _glow_TextureBrush.TranslateTransform(
                        Interlocked.Exchange(ref _glow_Translation_X, _glow_Translation_X) * (float)elapsed.TotalSeconds,
                        Interlocked.Exchange(ref _glow_Translation_Y, _glow_Translation_Y) * (float)elapsed.TotalSeconds
                        );
                    lastBrushUpdate = DateTime.Now;
                    elapsed = DateTime.Now - codeTime; // elapsed, whole code
                    Thread.Sleep(Math.Max((1000 / Interlocked.Exchange(ref _glow_TargetFPS, _glow_TargetFPS)) - (int)elapsed.TotalMilliseconds, 0));
                    codeTime = DateTime.Now;
                }
            }

            Thread gifUpdate_Thread;
            int gifUpdate_Thread_work = 1;

            void UpdateGif()
            {
                while (Interlocked.Exchange(ref gifUpdate_Thread_work, gifUpdate_Thread_work) != 0)
                {
                    Interlocked.Increment(ref frame);
                    Thread.Sleep(Math.Max(1000 / Interlocked.Exchange(ref _gif_TargetFPS, _gif_TargetFPS),0));
                }
            }

            ConcurrentDictionary<int, ItemRenderer> renderList;
            Thread render_Thread;
            int render_Thread_work = 1;

            void Render()
            {
                DateTime codeTime = DateTime.Now;
                while (Interlocked.Exchange(ref render_Thread_work, render_Thread_work) != 0)
                {
                    Rectangle render_Rect = new(
                        0,
                        0,
                        Interlocked.Exchange(ref _outputImage_Resolution_W, _outputImage_Resolution_W),
                        Interlocked.Exchange(ref _outputImage_Resolution_H, _outputImage_Resolution_H));

                    Bitmap glow_Scaled = new(
                            Interlocked.Exchange(ref _glow_Texture, _glow_Texture),
                            render_Rect.Width,
                            render_Rect.Height);

                    BitmapData glow_Data = glow_Scaled.LockBits(render_Rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                    int renderCalcSize = Math.Abs(glow_Data.Stride) * glow_Data.Height;
                    byte[] glow_BitmapPtr = new byte[renderCalcSize];
                    System.Runtime.InteropServices.Marshal.Copy(glow_Data.Scan0, glow_BitmapPtr, 0, renderCalcSize);

                    foreach (var item in renderList.Values)
                    {
                        if (!item.Rendered) continue;
                        Image sourceImage = Interlocked.Exchange(ref item.texture_Original, item.texture_Original);
                        FrameDimension itemFrameDim = new(sourceImage.FrameDimensionsList[0]);
                        sourceImage.SelectActiveFrame(itemFrameDim, Interlocked.Exchange(ref frame,frame) % sourceImage.GetFrameCount(itemFrameDim));

                        if (Interlocked.Exchange(ref item._glow, item._glow) != 0)
                        {
                            Bitmap outputBitmap = new(render_Rect.Width, render_Rect.Height, PixelFormat.Format32bppArgb);
                            Bitmap sourceBitmap = new(sourceImage, render_Rect.Width, render_Rect.Height);

                            BitmapData sourceData = sourceBitmap.LockBits(render_Rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                            BitmapData outputData = outputBitmap.LockBits(render_Rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

                            byte[] sourcePtr = new byte[renderCalcSize];
                            byte[] outputPtr = new byte[renderCalcSize];

                            Marshal.Copy(sourceData.Scan0, sourcePtr, 0, renderCalcSize);
                            Marshal.Copy(outputData.Scan0, outputPtr, 0, renderCalcSize);

                            for (int i = 3; i < renderCalcSize; i += 4)
                            {
                                if (sourcePtr[i] == 0)//A 0 = szyba
                                {
                                    outputPtr[i] = 69;//A
                                    outputPtr[i - 3] = (byte)(glow_BitmapPtr[i - 3]);//R
                                    outputPtr[i - 2] = (byte)(glow_BitmapPtr[i - 2]);//G
                                    outputPtr[i - 1] = (byte)(glow_BitmapPtr[i - 1]);//B
                                }
                                else
                                {
                                    outputPtr[i] = sourcePtr[i];//A
                                    outputPtr[i - 3] = (byte)((sourcePtr[i - 3] / 4) * 3 + (glow_BitmapPtr[i - 3] / 4));//R
                                    outputPtr[i - 2] = (byte)((sourcePtr[i - 2] / 4) * 3 + (glow_BitmapPtr[i - 2] / 4));//G
                                    outputPtr[i - 1] = (byte)((sourcePtr[i - 1] / 4) * 3 + (glow_BitmapPtr[i - 1] / 4));//B
                                }
                            }

                            Marshal.Copy(outputPtr, 0, outputData.Scan0, renderCalcSize);

                            sourceBitmap.UnlockBits(sourceData);
                            outputBitmap.UnlockBits(outputData);
                            Interlocked.Exchange(ref item.texture_Output, outputBitmap);
                            item.OnTextureUpdated();
                        }
                        else
                        {
                            Interlocked.Exchange(ref item.texture_Output, new(sourceImage, render_Rect.Width, render_Rect.Height));
                            item.OnTextureUpdated();
                        }
                    }

                    glow_Scaled.UnlockBits(glow_Data);

                    TimeSpan elapsed = DateTime.Now - codeTime; // elapsed, whole code
                    Thread.Sleep(Math.Max((1000 / Interlocked.Exchange(ref _render_TargetFPS, _render_TargetFPS)) - (int)elapsed.TotalMilliseconds, 0));
                    codeTime = DateTime.Now;
                }
            }

            /// <summary>
            /// Creates new instance and sets up default values
            /// </summary>
            public RenderController()
            {
                glowUpdate_Thread = new(UpdateGlow);
                glowUpdate_Thread.Name = "RenderController Glow update";
                glowUpdate_Thread.IsBackground = true;
                gifUpdate_Thread = new(UpdateGif);
                gifUpdate_Thread.Name = "RenderController Gif(frame) update";
                gifUpdate_Thread.IsBackground = true;
                render_Thread = new(Render);
                render_Thread.Name = "RenderController ItemRenderer";
                render_Thread.IsBackground = true;

                Glow_TargetFPS = 60;
                Gif_TargetFPS = 10;
                Render_TargetFPS = 60;


                renderList = new();

                Glow_Resolution = new(64, 64);
                OutputImage_Resolution = new(64, 64);
                Glow_TranslationPerSecond = new(120f, -180f);


                Glow_Tint = Color.FromArgb(0xf6, 0x43, 0xf6);
                Glow_TextureRotation = 30f;
                Glow_TextureScale = new SizeF(1f, 1f);
                RerenderGlowTexture();
                gifUpdate_Thread.Start();
                render_Thread.Start();
            }

            /// <summary>
            /// Creates new TextureBrush for Glow using
            /// <see cref="Glow_Tint"/>, 
            /// <see cref="Glow_TextureRotation"/> and 
            /// <see cref="Glow_TextureScale"/>
            /// </summary>
            public void RerenderGlowTexture()
            {
                Interlocked.Exchange(ref glowUpdate_Thread_work, 0);

                _glow_Texture = new(Properties.Resources.enchanted_item_glint);
                BitmapData glow_BData = _glow_Texture.LockBits(
                    new Rectangle(
                        0,
                        0,
                        Properties.Resources.enchanted_item_glint.Width,
                        Properties.Resources.enchanted_item_glint.Height
                        ),
                    ImageLockMode.ReadWrite,
                    PixelFormat.Format32bppArgb
                    );

                int glow_bytes = Math.Abs(glow_BData.Stride) * glow_BData.Height;
                byte[] glow_Ptr = new byte[glow_bytes];

                Marshal.Copy(glow_BData.Scan0, glow_Ptr, 0, glow_bytes);

                for (int counter = 3; counter < glow_Ptr.Length; counter += 4)
                {
                    glow_Ptr[counter - 3] = (byte)((glow_Ptr[counter - 3] / 255.0f) * Glow_Tint.R);
                    glow_Ptr[counter - 2] = (byte)((glow_Ptr[counter - 2] / 255.0f) * Glow_Tint.G);
                    glow_Ptr[counter - 1] = (byte)((glow_Ptr[counter - 1] / 255.0f) * Glow_Tint.B);
                }

                Marshal.Copy(glow_Ptr, 0, glow_BData.Scan0, glow_bytes);

                _glow_Texture.UnlockBits(glow_BData);

                _glow_TextureBrush = new(_glow_Texture);

                _glow_TextureBrush.RotateTransform(Glow_TextureRotation);
                _glow_TextureBrush.ScaleTransform(Glow_TextureScale.Width, Glow_TextureScale.Height);

                Interlocked.Exchange(ref glowUpdate_Thread_work, 1);
                glowUpdate_Thread.Start();
            }
        
            int GetNewUID()
            {
                return uid++;
            }

            /// <summary>
            /// Creates new <see cref= "ItemRenderer"/> with blank texture and starts rendering it
            /// </summary>
            /// <returns><see cref= "ItemRenderer"/> that was created</returns>
            public ItemRenderer NewRenderer()
            {
                int uid = GetNewUID();
                ItemRenderer itemR = new(this,new Bitmap(1,1), uid);
                renderList.TryAdd(uid, itemR);
                return itemR;
            }

            /// <summary>
            /// Creates new <see cref= "ItemRenderer"/> with specified texture and starts rendering it
            /// </summary>
            /// <param name="source">source Image</param>
            /// <returns><see cref= "ItemRenderer"/> that was created</returns>
            public ItemRenderer NewRenderer(Image source)
            {
                int uid = GetNewUID();
                ItemRenderer itemR = new(this, source, uid);
                renderList.TryAdd(uid, itemR);
                return itemR;
            }

            /// <summary>
            /// Creates new <see cref= "ItemRenderer"/> from <see cref= "Item"/> and starts rendering it<br/>
            /// </summary>
            /// <param name="item">source Item</param>
            /// <returns><see cref= "ItemRenderer"/> that was created</returns>
            public ItemRenderer NewRenderer(Item item)
            {
                int uid = GetNewUID();
                ItemRenderer itemR = new(this, item, uid);
                renderList.TryAdd(uid, itemR);
                return itemR;
            }

            /// <summary>
            /// Deletes specified <see cref="ItemRenderer"/>
            /// </summary>
            /// <param name="itemRenderer"><see cref="ItemRenderer"/> to remove</param>
            /// <returns>true if was succesfully removed, false otherwise</returns>
            public bool DeleteRenderer(ItemRenderer itemRenderer)
            {
                return renderList.TryRemove(itemRenderer.uid,out _);
            }
        }

    }

}
