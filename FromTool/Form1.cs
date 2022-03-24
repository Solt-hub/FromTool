using LanguageExt.TypeClasses;
using System.Data;
using System.Runtime.InteropServices;
using System.Text;
using static FromTool.Form1;
using System.Diagnostics;
using Microsoft.Win32;
using System.Drawing.Imaging;

namespace FromTool
{
 
    public partial class Form1 : Form
    {
        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        [DllImport("user32.dll", EntryPoint = "WindowFromPoint")]//指定坐标处窗体句柄
        public static extern IntPtr WindowFromPoint(int xPoint, int yPoint);
        //Windows消息值
        Bitmap img;
        int WM_CLOSE = 0x10;
        int WM_DESTROY = 0x02;
        int WM_QUIT = 0x12;
        public int SW_HIDE = 0;
        public int SW_SHOWNORMAL = 1;
        public int SW_SHOWMINIMIZED = 2;
        public int SW_SHOWMAXIMIZED = 3;
        public int SW_MAXIMIZE = 3;
        public int SW_SHOWNOACTIVATE = 4;
        public int SW_SHOW = 5;
        public int SW_MINIMIZE = 6;
        public int SW_SHOWMINNOACTIVE = 7;
        public int SW_SHOWNA = 8;
        public int SW_RESTORE = 9;
        public uint SWP_NOSIZE = 1;
        public uint SWP_NOMOVE = 2;
        public uint SWP_NOZORDER = 4;
        public uint SWP_NOREDRAW = 8;
        public uint SWP_NOACTIVATE = 0x10;
        public uint SWP_FRAMECHANGED = 0x20;
        public uint SWP_SHOWWINDOW = 0x40;
        public uint SWP_HIDEWINDOW = 0x80;
        public uint SWP_NOCOPYBITS = 0x100;
        public uint SWP_NOOWNERZORDER = 0x200;
        public uint SWP_NOSENDCHANGING = 0x400;
        public uint SWP_DRAWFRAME = 0x20;
        public uint SWP_NOREPOSITION = 0x200;
        public uint SWP_DEFERERASE = 0x2000;
        public uint SWP_ASYNCWINDOWPOS = 0x4000;
        //SendMessage和PostMessage的P/Invoke
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", EntryPoint = "ShowWindow", CharSet = CharSet.Auto)]
        public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        [DllImport("user32.dll")]
        private static extern int GetWindowRect(IntPtr hwnd, ref Rect lpRect);
        public static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        [DllImport("Oleacc.dll")]
        private static extern IntPtr GetProcessHandleFromHwnd(IntPtr hwnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);

        [DllImport("user32.dll",CharSet =CharSet.Auto)]
        private static extern bool SetWindowText(IntPtr hWnd, string text);
        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr Hwnd); 
        [DllImport("user32.dll")]
        static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
        [DllImport("user32.dll", EntryPoint = "SetWindowTextW")]
        public static extern bool SetWindowTextW(IntPtr hwnd, string lpString);
        [DllImport("user32.dll")]
        public static extern int GetWindowTextW(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpString, int nMaxCount);

        //获取窗口类名 
        [DllImport("user32.dll")]
        public static extern int GetClassNameW(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpString, int nMaxCount);
        

        public List<WindowsFinder.WindowInfo> info = new List<WindowsFinder.WindowInfo>();
        public WindowsFinder finder = new WindowsFinder();
        public int cthhdl = 0;
        public bool ismousedown = false;
        public Color RectColor = Color.Blue;

        public int findinfo(string text)
        {
            int tp = -1;
            for(int i = 0; i < info.Count; i++)
            {
                if(info[i].szWindowName==text)
                {
                    i = tp;
                }
            }
            return tp;
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            cthhdl = 0;
            info.Clear();
            info = finder.GetAllDesktopWindows().ToList();
            cthhdl = info.Count;
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            listBox3.Items.Clear();
            listBox4.Items.Clear();
            listBox5.Items.Clear();
            for (int i = 0; i < info.Count; i++)
            {
                listBox1.Items.Add(info[i].szWindowName ?? "<None>");
                listBox2.Items.Add(info[i].szClassName ?? "<None>");
                listBox3.Items.Add(info[i].h.ToString() ?? "<None>");
                listBox4.Items.Add(info[i].w.ToString() ?? "<None>");
                listBox5.Items.Add(string.Format("{0},{0}", info[i].rect.Left, info[i].rect.Top));
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            listBox3.Items.Clear();
            listBox4.Items.Clear();
            listBox5.Items.Clear();
            
        }

        private void listBox1_MouseDown(object sender, MouseEventArgs e)
        {
            _ = listBox1.Focus();
        }

        private void listBox2_MouseDown(object sender, MouseEventArgs e)
        {
            _ = listBox2.Focus();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox4.SelectedIndex = listBox2.SelectedIndex = listBox3.SelectedIndex = listBox5.SelectedIndex = listBox1.SelectedIndex;
            textBox3.Text=listBox1.SelectedIndex.ToString();
            textBox5.Text = listBox3.SelectedItem.ToString();
            textBox6.Text = listBox4.SelectedItem.ToString();
            textBox7.Text = listBox5.SelectedItem.ToString().Substring(0, listBox5.SelectedItem.ToString().IndexOf(','));
            textBox8.Text = listBox5.SelectedItem.ToString().Substring(listBox5.SelectedItem.ToString().IndexOf(',') + 1);
            int proid;
            GetWindowThreadProcessId(info[listBox1.SelectedIndex].hWnd, out proid);
            Process p = Process.GetProcessById(proid);
            textBox10.Text = p.MainModule.FileName;
            textBox11.Text = info[listBox1.SelectedIndex].szWindowName;
            IntPtr ptr = GetDC(IntPtr.Zero);
            Graphics graphics = Graphics.FromHdc(ptr);
            graphics.DrawRectangle(new Pen(RectColor), new Rectangle(info[listBox1.SelectedIndex].rect.Left, info[listBox1.SelectedIndex].rect.Top, info[listBox1.SelectedIndex].w, info[listBox1.SelectedIndex].h));
            ReleaseDC(IntPtr.Zero, ptr); 
            graphics.Dispose();
            Icon icon = Icon.ExtractAssociatedIcon(textBox10.Text);
            MemoryStream ms = new MemoryStream();
            icon.Save(ms);
            Image image = Image.FromStream(ms);
            pictureBox1.Image = image;
        }
        private void ListBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox1.SelectedIndex =listBox4.SelectedIndex  = listBox3.SelectedIndex = listBox5.SelectedIndex = listBox2.SelectedIndex;
            textBox3.Text = listBox1.SelectedIndex.ToString();
            textBox5.Text = listBox3.SelectedItem.ToString();
            textBox6.Text = listBox4.SelectedItem.ToString();
            textBox7.Text = listBox5.SelectedItem.ToString().Substring(0, listBox5.SelectedItem.ToString().IndexOf(','));
            textBox8.Text = listBox5.SelectedItem.ToString().Substring(listBox5.SelectedItem.ToString().IndexOf(',') + 1);
            int proid;
            GetWindowThreadProcessId(info[listBox1.SelectedIndex].hWnd, out proid);
            Process p = Process.GetProcessById(proid);
            textBox10.Text = p.MainModule.FileName;
            textBox11.Text = info[listBox1.SelectedIndex].szWindowName;
            IntPtr ptr = GetDC(IntPtr.Zero);
            Graphics graphics = Graphics.FromHdc(ptr);
            graphics.DrawRectangle(new Pen(RectColor), new Rectangle(info[listBox1.SelectedIndex].rect.Left, info[listBox1.SelectedIndex].rect.Top, info[listBox1.SelectedIndex].w, info[listBox1.SelectedIndex].h));
            ReleaseDC(IntPtr.Zero, ptr);
            graphics.Dispose();
            Icon icon = Icon.ExtractAssociatedIcon(textBox10.Text);
            MemoryStream ms = new MemoryStream();
            icon.Save(ms);
            Image image = Image.FromStream(ms);
            pictureBox1.Image = image;
        }

        private void 结束进程ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(info.Count > 0)
            {
                PostMessage(info[listBox1.SelectedIndex].hWnd,(uint)WM_DESTROY,IntPtr.Zero,IntPtr.Zero);
            }
            else
            {
                MessageBox.Show("清先查找窗体！", "提示", icon: MessageBoxIcon.Information,buttons:MessageBoxButtons.OK);
            }
        }

        private void 关闭窗口ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (info.Count > 0)
            {
                PostMessage(info[listBox1.SelectedIndex].hWnd, (uint)WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            }
            else
            {
                MessageBox.Show("清先查找窗体！", "提示", icon: MessageBoxIcon.Information, buttons: MessageBoxButtons.OK);
            }
        }

        private void 最大化ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (info.Count > 0)
            {
                ShowWindow(info[listBox1.SelectedIndex].hWnd, SW_MAXIMIZE);
            }
            else
            {
                MessageBox.Show("清先查找窗体！", "提示", icon: MessageBoxIcon.Information, buttons: MessageBoxButtons.OK);
            }
        }

        private void 最小化ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (info.Count > 0)
            {
                ShowWindow(info[listBox1.SelectedIndex].hWnd, SW_SHOWMINIMIZED);
            }
            else
            {
                MessageBox.Show("清先查找窗体！", "提示", icon: MessageBoxIcon.Information, buttons: MessageBoxButtons.OK);
            }
        }

        private void 置顶ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(info.Count > 0)
            {
                SetForegroundWindow(info[listBox1.SelectedIndex].hWnd);
            }
            else
            {
                MessageBox.Show("清先查找窗体！", "提示", icon: MessageBoxIcon.Information, buttons: MessageBoxButtons.OK);
            }
        }

        private void 置底ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (info.Count > 0)
            {
                SetWindowPos(info[listBox1.SelectedIndex].hWnd, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOREPOSITION);
            }
            else
            {
                MessageBox.Show("清先查找窗体！", "提示", icon: MessageBoxIcon.Information, buttons: MessageBoxButtons.OK);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string title = textBox1.Text;
            if (!string.IsNullOrEmpty(title))
            {
                listBox1.Items.Clear();
                listBox2.Items.Clear();
                listBox3.Items.Clear();
                listBox4.Items.Clear();
                listBox5.Items.Clear();
                info.Clear();
                List<WindowsFinder.WindowInfo> info1 = finder.GetAllDesktopWindows().ToList();
                foreach (WindowsFinder.WindowInfo desktopWindow in info1)
                {
                   if(title.Contains(desktopWindow.szWindowName))
                   {
                        listBox1.Items.Add(desktopWindow.szWindowName);
                        listBox2.Items.Add(desktopWindow.szClassName);
                        listBox3.Items.Add(desktopWindow.h);
                        listBox4.Items.Add(desktopWindow.w);
                        listBox5.Items.Add(string.Format("{0},{0}", desktopWindow.rect.Right - desktopWindow.rect.Left, desktopWindow.rect.Bottom - desktopWindow.rect.Top));
                        info.Add(desktopWindow);
                   }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string classname = textBox2.Text;
            if (!string.IsNullOrEmpty(classname))
            {
                listBox1.Items.Clear();
                listBox2.Items.Clear();
                listBox3.Items.Clear();
                listBox4.Items.Clear();
                listBox5.Items.Clear();
                info.Clear();
                List<WindowsFinder.WindowInfo> info1 = finder.GetAllDesktopWindows().ToList();
                foreach (WindowsFinder.WindowInfo desktopWindow in info1)
                {
                    if (classname.Contains(desktopWindow.szClassName))
                    {
                        listBox1.Items.Add(desktopWindow.szWindowName);
                        listBox2.Items.Add(desktopWindow.szClassName);
                        info.Add(desktopWindow);
                    }
                }
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            listBox1.SelectedIndex = listBox2.SelectedIndex = Convert.ToInt32(textBox3.Text);
            textBox9.Text = info[listBox1.SelectedIndex].hWnd.ToInt64().ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(textBox3.Text) && !string.IsNullOrEmpty(textBox4.Text))
            {
                if (Convert.ToInt32(textBox3.Text) < info.Length())
                {
                    SendMessage(info[Convert.ToInt32(textBox3.Text)].hWnd,Convert.ToUInt32(textBox4.Text),IntPtr.Zero,IntPtr.Zero);
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox3.Text) && !string.IsNullOrEmpty(textBox4.Text))
            {
                if (Convert.ToInt32(textBox3.Text) < info.Length())
                {
                    PostMessage(info[Convert.ToInt32(textBox3.Text)].hWnd, Convert.ToUInt32(textBox4.Text), IntPtr.Zero, IntPtr.Zero);
                }
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox1.SelectedIndex = listBox2.SelectedIndex = listBox4.SelectedIndex = listBox5.SelectedIndex = listBox3.SelectedIndex;
            textBox3.Text = listBox1.SelectedIndex.ToString();
            textBox5.Text = listBox3.SelectedItem.ToString();
            textBox6.Text = listBox4.SelectedItem.ToString();
            textBox7.Text = listBox5.SelectedItem.ToString().Substring(0, listBox5.SelectedItem.ToString().IndexOf(','));
            textBox8.Text = listBox5.SelectedItem.ToString().Substring(listBox5.SelectedItem.ToString().IndexOf(',') + 1);
            int proid;
            GetWindowThreadProcessId(info[listBox1.SelectedIndex].hWnd, out proid);
            Process p = Process.GetProcessById(proid);
            textBox10.Text = p.MainModule.FileName;
            textBox11.Text = info[listBox1.SelectedIndex].szWindowName;
            IntPtr ptr = GetDC(IntPtr.Zero);
            Graphics graphics = Graphics.FromHdc(ptr);
            graphics.DrawRectangle(new Pen(RectColor), new Rectangle(info[listBox1.SelectedIndex].rect.Left, info[listBox1.SelectedIndex].rect.Top, info[listBox1.SelectedIndex].w, info[listBox1.SelectedIndex].h));
            ReleaseDC(IntPtr.Zero, ptr);
            graphics.Dispose();
            Icon icon = Icon.ExtractAssociatedIcon(textBox10.Text);
            MemoryStream ms = new MemoryStream();
            icon.Save(ms);
            Image image = Image.FromStream(ms);
            pictureBox1.Image = image;
        }

        private void listBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox1.SelectedIndex = listBox2.SelectedIndex =listBox3.SelectedIndex = listBox5.SelectedIndex = listBox4.SelectedIndex;
            textBox3.Text = listBox1.SelectedIndex.ToString();
            textBox5.Text = listBox3.SelectedItem.ToString();
            textBox6.Text = listBox4.SelectedItem.ToString();
            textBox7.Text = listBox5.SelectedItem.ToString().Substring(0, listBox5.SelectedItem.ToString().IndexOf(','));
            textBox8.Text = listBox5.SelectedItem.ToString().Substring(listBox5.SelectedItem.ToString().IndexOf(',') + 1);
            int proid;
            GetWindowThreadProcessId(info[listBox1.SelectedIndex].hWnd, out proid);
            Process p = Process.GetProcessById(proid);
            textBox10.Text = p.MainModule.FileName;
            textBox11.Text = info[listBox1.SelectedIndex].szWindowName;
            IntPtr ptr = GetDC(IntPtr.Zero);
            Graphics graphics = Graphics.FromHdc(ptr);
            graphics.DrawRectangle(new Pen(RectColor), new Rectangle(info[listBox1.SelectedIndex].rect.Left, info[listBox1.SelectedIndex].rect.Top, info[listBox1.SelectedIndex].w, info[listBox1.SelectedIndex].h));
            ReleaseDC(IntPtr.Zero, ptr);
            graphics.Dispose();
            Icon icon = Icon.ExtractAssociatedIcon(textBox10.Text);
            MemoryStream ms = new MemoryStream();
            icon.Save(ms);
            Image image = Image.FromStream(ms);
            pictureBox1.Image = image;
        }

        private void listBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox1.SelectedIndex = listBox2.SelectedIndex = listBox3.SelectedIndex = listBox4.SelectedIndex = listBox5.SelectedIndex;
            textBox3.Text = listBox1.SelectedIndex.ToString();
            textBox5.Text = listBox3.SelectedItem.ToString();
            textBox6.Text = listBox4.SelectedItem.ToString();
            textBox7.Text = listBox5.SelectedItem.ToString().Substring(0, listBox5.SelectedItem.ToString().IndexOf(','));
            textBox8.Text = listBox5.SelectedItem.ToString().Substring(listBox5.SelectedItem.ToString().IndexOf(',') + 1);
            int proid;
            GetWindowThreadProcessId(info[listBox1.SelectedIndex].hWnd, out proid);
            Process p = Process.GetProcessById(proid);
            textBox10.Text = p.MainModule.FileName;
            textBox11.Text = info[listBox1.SelectedIndex].szWindowName;
            IntPtr ptr = GetDC(IntPtr.Zero);
            Graphics graphics = Graphics.FromHdc(ptr);
            graphics.DrawRectangle(new Pen(RectColor), new Rectangle(info[listBox1.SelectedIndex].rect.Left, info[listBox1.SelectedIndex].rect.Top, info[listBox1.SelectedIndex].w, info[listBox1.SelectedIndex].h));
            ReleaseDC(IntPtr.Zero, ptr);
            graphics.Dispose();
            Icon icon = Icon.ExtractAssociatedIcon(textBox10.Text);
            MemoryStream ms = new MemoryStream();
            icon.Save(ms);
            Image image = Image.FromStream(ms);
            pictureBox1.Image = image;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Rect rect = new Rect();
            rect = info[Convert.ToInt32(textBox3.Text)].rect;
            rect.Left=Convert.ToInt32(textBox7.Text);
            rect.Top=Convert.ToInt32(textBox8.Text);
            rect.Right = rect.Left + info[Convert.ToInt32(textBox3.Text)].w;
            rect.Bottom = rect.Top + info[Convert.ToInt32(textBox3.Text)].h;
            _ = SetWindowPos(info[Convert.ToInt32(textBox3.Text)].hWnd, IntPtr.Zero, rect.Left, rect.Top, rect.Right - Left, rect.Bottom - rect.Top, SWP_NOZORDER | SWP_SHOWWINDOW);
        }

        private void 默认大小ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowWindow(info[listBox1.SelectedIndex].hWnd, SW_SHOWNORMAL);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(textBox10.Text))
            {
                Process.Start("Explorer", "/select," + textBox10.Text);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            SetWindowText(info[listBox1.SelectedIndex].hWnd, textBox11.Text);
        }

        private void 复制ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();
            Clipboard.SetText(listBox1.SelectedItem.ToString() + " " + listBox2.SelectedItem.ToString() + " " + listBox3.SelectedItem.ToString() + listBox4.SelectedItem.ToString() + " " + listBox5.SelectedItem.ToString());
        }

        private void button9_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "图标文件|*.ico";
            if(saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if(pictureBox1.Image != null)
                {
                    pictureBox1.Image.Save(saveFileDialog1.FileName + ".ico");
                }
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap(info[listBox1.SelectedIndex].w, info[listBox1.SelectedIndex].h);
            Graphics graphics = Graphics.FromImage(bmp);
            graphics.CopyFromScreen(new Point(info[listBox1.SelectedIndex].rect.Left, info[listBox1.SelectedIndex].rect.Top), new Point(0, 0), bmp.Size);
            this.img = bmp;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if(this.img != null)
            {
                saveFileDialog1.Filter = "jpeg|*.jpg|bmp|*.bmp|gif|*.gif|png|*.png";
                if(saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    System.Drawing.Imaging.ImageFormat format;
                    switch(saveFileDialog1.FilterIndex)
                    {
                        case 0:
                            {
                                format = ImageFormat.Jpeg;
                                break;
                            }
                        case 1:
                            {
                                format = ImageFormat.Bmp;
                                break;
                            }
                        case 2:
                            {
                                format= ImageFormat.Gif;
                                break;
                            }
                        case 3:
                            {
                                format = ImageFormat.Png;
                                break;
                            }
                        default:
                            {
                                format=ImageFormat.Jpeg;
                                break;
                            }
                    }
                    img.Save(saveFileDialog1.FileName, format);
                }
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            ismousedown = true;
            Cursor = Cursors.Cross;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if(ismousedown)
            {
                listBox1.Items.Clear();
                listBox2.Items.Clear();
                listBox3.Items.Clear();
                listBox4.Items.Clear();
                listBox5.Items.Clear();
                info.Clear();
                IntPtr hwnd = WindowFromPoint(Cursor.Position.X, Cursor.Position.Y);
                if(hwnd != IntPtr.Zero)
                {
                    StringBuilder sb = new StringBuilder(256);
                    GetWindowTextW(hwnd, sb, sb.Capacity);
                    listBox1.Items.Add(sb.ToString());
                    GetClassNameW(hwnd, sb, sb.Capacity);
                    listBox2.Items.Add(sb.ToString());
                    Rect rect = new Rect();
                    GetWindowRect(hwnd, ref rect);
                    listBox3.Items.Add((rect.Bottom - rect.Top).ToString());
                    listBox4.Items.Add((rect.Left - rect.Right).ToString());
                    listBox5.Items.Add(string.Format("{0},{1}", rect.Left, rect.Top));
                    info.Add(new WindowsFinder.WindowInfo(hwnd, listBox1.Items[0].ToString(), listBox2.Items[0].ToString(), rect));
                }
                listBox1.SelectedIndex = 0;
                ismousedown = false;
                Cursor = Cursors.Default;
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            if(colorDialog1.ShowDialog() == DialogResult.OK)
            {
                RectColor=colorDialog1.Color;
            }
        }
    }
    public class WindowsFinder
    {
        public delegate bool WNDENUMPROC(IntPtr hWnd, int lParam);

        //用来遍历所有窗口 
        [DllImport("user32.dll")]
        public static extern bool EnumWindows(WNDENUMPROC lpEnumFunc, int lParam);

        //获取窗口Text 
        [DllImport("user32.dll")]
        public static extern int GetWindowTextW(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpString, int nMaxCount);

        //获取窗口类名 
        [DllImport("user32.dll")]
        public static extern int GetClassNameW(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll")]
        public static extern int GetWindowRect(IntPtr hwnd, ref Rect lpRect);
        
        public struct WindowInfo
        {
            public IntPtr hWnd;
            public string szWindowName;
            public string szClassName;
            public Rect rect;
            public int h;
            public int w;
            public WindowInfo(IntPtr hwnd,string szWindowName, string szClassName, Rect rect)
            {
                this.hWnd = hwnd;
                this.szWindowName = szWindowName;
                this.szClassName = szClassName;
                this.rect = rect;
                this.h = rect.Bottom - rect.Top;
                this.w = rect.Right - rect.Left;
            }
        }

        public WindowInfo[] GetAllDesktopWindows()
        {
            //用来保存窗口对象 列表
            List<WindowInfo> wndList = new List<WindowInfo>();

            //enum all desktop windows 
            EnumWindows(delegate (IntPtr hWnd, int lParam)
            {
                WindowInfo wnd = new WindowInfo();
                StringBuilder sb = new StringBuilder(256);

                //get hwnd 
                wnd.hWnd = hWnd;

                //get window name  
                GetWindowTextW(hWnd, sb, sb.Capacity);
                wnd.szWindowName = sb.ToString();

                //get window class 
                GetClassNameW(hWnd, sb, sb.Capacity);
                wnd.szClassName = sb.ToString();

                Rect rect2 = new Rect();
                GetWindowRect(hWnd, ref rect2);
                wnd.rect = rect2;

                wnd.h = rect2.Bottom - rect2.Top;
                wnd.w = rect2.Right - rect2.Left;
                //add it into list 
                wndList.Add(wnd);
                return true;
            }, 0);

            return wndList.ToArray();
        }
    }
}