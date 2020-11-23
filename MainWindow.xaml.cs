using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Drawing;
using System.Drawing.Imaging;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.WpfExtensions;
using System.Threading;

namespace Puri2
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public double minVal, maxVal;

        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, int dwExtraInfo);

        [DllImport("winio.dll")]
        public static extern bool SetPortVal(uint wPortAddr, IntPtr dwPortVal, byte bSize);

        [DllImport("winio.dll")]
        public static extern bool GetPortVal(IntPtr wPortAddr, out int pdwPortVal, byte bSize);

        [DllImport("user32.dll")]
        public static extern int MapVirtualKey(uint Ucode, uint uMapType);

        public static double VirtualScreenWidth { get; }
        public static double VirtualScreenTop { get; }

        private static Mat picImage;

        private static Mat src;

        private static bool TESX;

        public MainWindow()
        {
            InitializeComponent();
            
        }


        private void screen()
        {
            
            CaptureMyScreen();
            picImage = new Mat(@"G:\相片\新增資料夾\Puri\Capture.jpg", ImreadModes.Grayscale);
            src = new Mat(@"G:\相片\新增資料夾\Puri\src.jpg", ImreadModes.Grayscale);
            //imgSC.Source = picImage.ToBitmapSource();

            using (Mat result = new Mat())
            {

                OpenCvSharp.Point minLoc, maxLoc;
                Cv2.MatchTemplate(src, picImage, result, TemplateMatchModes.CCoeffNormed); //マッチング処理
                Cv2.MinMaxLoc(result, out minVal, out maxVal, out minLoc, out maxLoc); //最大値と座標を取得

                Console.WriteLine(maxVal.ToString() + maxLoc + src.Size() + minVal.ToString() + minLoc);
                if (maxVal >= 0.8)
                {
                    var ssx = src.Size();

                    //SetCursorPos(maxLoc.X + ssx.Width / 2, maxLoc.Y + ssx.Height / 2);
                    //Console.WriteLine(DateTime.Now.ToString("hh:mm:ss.fff ") + "Set Mouse_XY");
                    //listBox1.Items.Add(DateTime.Now.ToString("hh:mm:ss.fff ") + "Set Mouse_XY");
                    //MyMouseDown(MOUSEEVENTF_LEFTDOWN);
                    //MyMouseUp(MOUSEEVENTF_LEFTUP);
                    //Console.WriteLine(DateTime.Now.ToString("hh:mm:ss.fff ") + "Mouse_LeftDown");
                    //listBox1.Items.Add(DateTime.Now.ToString("hh:mm:ss.fff ") + "Mouse_LeftDown");
                    //mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                    //Console.WriteLine(DateTime.Now.ToString("hh:mm:ss.fff ") + "Mouse_LeftUp");
                    //listBox1.Items.Add(DateTime.Now.ToString("hh:mm:ss.fff ") + "Mouse_LeftUp");
                    //mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                    //mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                    //mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                    //矩形と値を描画
                    picImage.Rectangle(new OpenCvSharp.Rect(maxLoc, src.Size()), Scalar.Blue, 2);
                    picImage.PutText(maxVal.ToString(), maxLoc, HersheyFonts.HersheyDuplex, 1, Scalar.Blue);
                }
                Console.WriteLine(maxVal.ToString()+ " err");
            }
            //picImage.Release();
            imgSC.Source = picImage.ToBitmapSource();
            picImage.Release();
            DoEvents();
            GC.Collect();

        }

        private void mouese()
        {
            this.Dispatcher.BeginInvoke((Action)delegate ()
            {
                while (TESX)
                {
                    var transform = PresentationSource.FromVisual(this).CompositionTarget.TransformFromDevice;
                    var mouse = transform.Transform(GetMousePosition());
                    Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "X:" + mouse.X + " Y: " + mouse.Y);
                    //System.Threading.Thread.Sleep(250);
                    System.Threading.SpinWait.SpinUntil(() => false, 450);
                    DoEvents();
                    GC.Collect();
                }
            });

            //while (TESX)
            //{
            //    var transform = PresentationSource.FromVisual(this).CompositionTarget.TransformFromDevice;
            //    var mouse = transform.Transform(GetMousePosition());
            //    Console.WriteLine("X:" + mouse.X + " Y: " + mouse.Y);
            //    //System.Threading.Thread.Sleep(250);
            //    System.Threading.SpinWait.SpinUntil(() => false, 200);
            //    DoEvents();
            //}
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            screen();
            TESX = true;
            this.Dispatcher.Invoke(mouese);
            GC.Collect();
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            TESX = false;
            GC.Collect();
        }

        private static void CaptureMyScreen()
        {
            try
            {
                //Creating a new Bitmap object
                Bitmap captureBitmap = new Bitmap(1920, 1080, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                //Creating a Rectangle object which will 
                //capture our Current Screen
                System.Drawing.Rectangle captureRectangle = Screen.AllScreens[0].Bounds;
                //Creating a New Graphics Object
                Graphics captureGraphics = Graphics.FromImage(captureBitmap);
                //Copying Image from The Screen
                captureGraphics.CopyFromScreen(captureRectangle.Left, captureRectangle.Top, 0, 0, captureRectangle.Size);
                captureBitmap.Save(@"G:\相片\新增資料夾\Puri\Capture.jpg", ImageFormat.Jpeg);
                //await Task.Delay(50);
                //listBox1.Items.Add("Screen Captured");
                //Console.WriteLine("Screen Captured");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //System.Windows.MessageBox.Show(ex.Message);
            }
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new DispatcherOperationCallback(ExitFrame), frame);
            Dispatcher.PushFrame(frame);
        }

        public object ExitFrame(object f)
        {
            ((DispatcherFrame)f).Continue = false;
            return null;
        }

        public System.Windows.Point GetMousePosition()
        {
            var point = System.Windows.Forms.Control.MousePosition;
            return new System.Windows.Point(point.X, point.Y);
        }
    }
}
