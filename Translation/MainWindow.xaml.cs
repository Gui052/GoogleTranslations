using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Translation
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private dotranslate ts;
        public MainWindow()
        {
            InitializeComponent();
            ts = new dotranslate();

            this.SourceInitialized += delegate (object sender, EventArgs e)//执行拖拽
            {
                this._HwndSource = PresentationSource.FromVisual((Visual)sender) as HwndSource;
            };
            this.MouseMove += new MouseEventHandler(Window_MouseMove);//鼠标移入到边缘收缩
            this.Loaded += new RoutedEventHandler(Window_Loaded);
        }
        #region 窗体相关
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnMin_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnTopMost_Click(object sender, RoutedEventArgs e)
        {
            if (Topmost == false)
            {
                Topmost = true;
                btnTopMost.ToolTip = "取消顶置";
            }

            else
            {
                Topmost = false;
                btnTopMost.ToolTip = "顶置";
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tbxWord.Focus();
        }

        private void btnTranslate_Click(object sender, RoutedEventArgs e)
        {
            DoTranslate();
        }
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //翻译快捷键
            if ((e.KeyboardDevice.IsKeyDown(Key.Enter) && Keyboard.Modifiers == ModifierKeys.Alt))
            {
                DoTranslate();
            }
            //中英切换快捷键
            if ((e.KeyboardDevice.IsKeyDown(Key.X) && Keyboard.Modifiers == ModifierKeys.Alt))
            {
                btnLang_Click(sender, e);
            }
            //最小化快捷键
            if ((e.KeyboardDevice.IsKeyDown(Key.M) && Keyboard.Modifiers == ModifierKeys.Alt))
            {
                this.WindowState = WindowState.Minimized;
            }

        }
        #endregion
        private void DoTranslate()
        {
            string type="zh-CN";
            if (tbLang.Text == "中")
            {
                type = "zh-CN";
            }
            else
            {
                type = "en";
            }
            tbxResult.Text = string.Empty;
            string[] texts = tbxWord.Text.Split('\n');
            foreach(string i in texts)
            {
                tbxResult.Text= tbxResult.Text+ ts.GoogleTranslate(i.Replace("\t", "").Replace("\r", ""), "auto", type)+'\n';
            }
        }

        private void btnLang_Click(object sender, RoutedEventArgs e)
        {
            if(tbLang.Text=="中")
            {
                tbLang.Text = "英";
                btnLang.ToolTip = "英";
            }
            else
            {
                tbLang.Text = "中";
                btnLang.ToolTip = "中";
            }
        }

        #region 初始化窗体可以缩放大小
        private const int WM_SYSCOMMAND = 0x112;
        private HwndSource _HwndSource;
        private Dictionary<ResizeDirection, Cursor> cursors = new Dictionary<ResizeDirection, Cursor>
        {
            {ResizeDirection.BottomRight, Cursors.SizeNWSE},
        };
        private enum ResizeDirection
        {
            BottomRight = 8,
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        #endregion
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton != MouseButtonState.Pressed)
            {
                FrameworkElement element = e.OriginalSource as FrameworkElement;
                if (element != null && !element.Name.Contains("Resize"))
                    this.Cursor = Cursors.Arrow;
            }

        }
        private void ResizePressed(object sender, MouseEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            ResizeDirection direction = (ResizeDirection)Enum.Parse(typeof(ResizeDirection), element.Name.Replace("Resize", ""));
            this.Cursor = cursors[direction];
            if (e.LeftButton == MouseButtonState.Pressed)
                ResizeWindow(direction);
        }
        private void ResizeWindow(ResizeDirection direction)
        {
            SendMessage(_HwndSource.Handle, WM_SYSCOMMAND, (IntPtr)(61440 + direction), IntPtr.Zero);
        }

    }
}
