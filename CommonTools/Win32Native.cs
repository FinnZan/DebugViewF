namespace FinnZan.Utilities
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    public class Win32Native
    {
        [DllImport("user32")]
        public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

        [DllImport("user32.dll", SetLastError=true, CharSet=CharSet.Auto)]
        public static extern uint RegisterWindowMessage(string lpString);

        [Flags]
        public enum MessageBroadcastFlags : uint
        {
            BSF_QUERY = 0x00000001,
            BSF_IGNORECURRENTTASK = 0x00000002,
            BSF_FLUSHDISK = 0x00000004,
            BSF_NOHANG = 0x00000008,
            BSF_POSTMESSAGE = 0x00000010,
            BSF_FORCEIFHUNG = 0x00000020,
            BSF_NOTIMEOUTIFNOTHUNG = 0x00000040,
            BSF_ALLOWSFW = 0x00000080,
            BSF_SENDNOTIFYMESSAGE = 0x00000100,
            BSF_RETURNHDESK = 0x00000200,
            BSF_LUID = 0x00000400
        }

        [Flags]
        public enum MessageBroadcastRecipients : uint
        {
            BSM_ALLCOMPONENTS = 0x00000000,
            BSM_VXDS = 0x00000001,
            BSM_NETDRIVER = 0x00000002,
            BSM_INSTALLABLEDRIVERS = 0x00000004,
            BSM_APPLICATIONS = 0x00000008,
            BSM_ALLDESKTOPS = 0x00000010
        }

        [DllImport("user32", SetLastError=true)]
        public static extern int BroadcastSystemMessage(MessageBroadcastFlags flags, ref MessageBroadcastRecipients lpInfo, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError=true)]
        public static extern bool ShutdownBlockReasonCreate(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)] string reason);

        [DllImport("user32.dll", SetLastError=true)]
        public static extern bool ShutdownBlockReasonDestroy(IntPtr hWnd);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("shell32.dll", EntryPoint = "#261", CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void GetUserTilePath(string username, 
          UInt32 whatever, // 0x80000000
          StringBuilder picpath, int maxLength);

        [StructLayout(LayoutKind.Sequential)]
        public struct DEV_BROADCAST_HDR
        {
            public uint dbch_Size;
            public uint dbch_DeviceType;
            public uint dbch_Reserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DEV_BROADCAST_VOLUME
        {
            public uint dbch_Size;
            public uint dbch_Devicetype;
            public uint dbch_Reserved;
            public uint dbch_Unitmask;
            public ushort dbch_Flags;
        }

        #region Mouse Hook ============

        public delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        public const int WH_MOUSE_LL = 14;
        public const int WH_KEYBOARD_LL = 13;        

        public enum MouseMessages
        {
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_MOUSEMOVE = 0x0200,
            WM_MOUSEWHEEL = 0x020A,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook,    LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
          IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        #endregion

    }
}
