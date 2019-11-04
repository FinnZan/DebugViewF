namespace FinnZan.Utilities
{
    using FinnZan.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Interop;

    public class Screen
    {
        public static IEnumerable<Screen> AllScreens()
        {
            foreach (System.Windows.Forms.Screen screen in System.Windows.Forms.Screen.AllScreens)
            {
                yield return new Screen(screen);
            }
        }
        
        public static Screen GetScreenFrom(System.Windows.Point point)
        {
            int x = (int)Math.Round(point.X);
            int y = (int)Math.Round(point.Y);

            // are x,y device-independent-pixels ??
            System.Drawing.Point drawingPoint = new System.Drawing.Point(x, y);
            System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.FromPoint(drawingPoint);
            Screen wpfScreen = new Screen(screen);

            return wpfScreen;
        }

        public static Screen Primary
        {
            get { return new Screen(System.Windows.Forms.Screen.PrimaryScreen); }
        }

        private readonly System.Windows.Forms.Screen screen;

        private Screen(System.Windows.Forms.Screen screen)
        {
            this.screen = screen;
        }

        public Rect Bounds
        {
            get { return this.GetRect(this.screen.Bounds); }
        }

        public double Width
        {
            get
            {
                return (double)this.screen.Bounds.Width;
            }
        }

        public double Height
        {
            get
            {
                return (double)this.screen.Bounds.Height;
            }
        }

        public Rect WorkingArea
        {
            get { return this.GetRect(this.screen.WorkingArea); }
        }

        private Rect GetRect(Rectangle value)
        {
            // should x, y, width, height be device-independent-pixels ??
            return new Rect
            {
                X = value.X,
                Y = value.Y,
                Width = value.Width,
                Height = value.Height
            };
        }

        public bool IsPrimary
        {
            get { return this.screen.Primary; }
        }

        public string Name
        {
            get { return this.screen.DeviceName; }
        }

        public static Rect GetGlobalBounds()
        {
            try
            {
                int left = 10000;
                int top = 10000;
                int bottom = 0;
                int right = 0;

                foreach (System.Windows.Forms.Screen screen in System.Windows.Forms.Screen.AllScreens)
                {
                    if (screen.Bounds.Left < left)
                    {
                        left = screen.Bounds.Left;
                    }

                    if (screen.Bounds.Top < top)
                    {
                        top = screen.Bounds.Top;
                    }

                    if (screen.Bounds.Right > right)
                    {
                        right = screen.Bounds.Right;
                    }

                    if (screen.Bounds.Bottom > bottom)
                    {
                        bottom = screen.Bounds.Bottom;
                    }
                }

                Rect rect = new Rect(left, top, right - left, bottom - top);

                return rect;
            }
            catch (Exception ex)
            {
                CommonTools.HandleException(ex);
                return new Rect(0, 0, 0, 0);
            }
        }

        public static Bitmap Capture(int x, int y, int w, int h)
        {
            try
            {
                Bitmap bmpScreenCapture = new Bitmap(w, h);

                using (Graphics g = Graphics.FromImage(bmpScreenCapture))
                {
                    g.CopyFromScreen(x - w / 2,
                                     y - w / 2,
                                     0, 0,
                                     bmpScreenCapture.Size,
                                     CopyPixelOperation.SourceCopy);
                }

                return bmpScreenCapture;
            }
            catch (Exception ex)
            {
                CommonTools.HandleException(ex);
                return null;
            }
        }
    }
}
