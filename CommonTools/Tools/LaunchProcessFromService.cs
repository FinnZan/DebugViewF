using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace FinnZan.Utilities
{
    /// <summary>
    /// This class launch a non admin process from the windows service. This is done by getting the active session id WTSGetActiveConsoleSessionId followed by 
    /// using the CreateProcessAsUser to launch the process
    /// </summary>
    internal class LaunchProcessFromService
    {
        #region constants
        const uint MAXIMUM_ALLOWED = 0x02000000;
        #endregion

        #region structures
        [StructLayout(LayoutKind.Sequential)]
        public struct STARTUPINFO 
        { 
            public int cb; 
            public String lpReserved; 
            public String lpDesktop; 
            public String lpTitle; 
            public uint dwX; 
            public uint dwY; 
            public uint dwXSize; 
            public uint dwYSize; 
            public uint dwXCountChars; 
            public uint dwYCountChars;
            public uint dwFillAttribute; 
            public uint dwFlags; 
            public short wShowWindow; 
            public short cbReserved2; 
            public IntPtr lpReserved2; 
            public IntPtr hStdInput; 
            public IntPtr hStdOutput; 
            public IntPtr hStdError; 
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESS_INFORMATION 
        { 
            public IntPtr hProcess; 
            public IntPtr hThread; 
            public uint dwProcessId; 
            public uint dwThreadId; 
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES 
        { 
            public int nLength; 
            public IntPtr lpSecurityDescriptor; 
            public int bInheritHandle; 
        }

        #endregion

        #region enums
        public enum TOKEN_TYPE 
        { 
            TokenPrimary = 1, 
            TokenImpersonation 
        }
        public enum SECURITY_IMPERSONATION_LEVEL 
        { 
            SecurityAnonymous,
            SecurityIdentification,
            SecurityImpersonation,
            SecurityDelegation 
        }

        #endregion

        #region DLLImports

        [DllImport("kernel32.dll")]
        public static extern uint WTSGetActiveConsoleSessionId();

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern bool WTSQueryUserToken(UInt32 sessionId, out IntPtr Token);

        [DllImport("advapi32.dll", EntryPoint = "CreateProcessAsUser", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static bool CreateProcessAsUser(IntPtr hToken, String lpApplicationName, String lpCommandLine, ref SECURITY_ATTRIBUTES lpProcessAttributes,
            ref SECURITY_ATTRIBUTES lpThreadAttributes, bool bInheritHandle, int dwCreationFlags, IntPtr lpEnvironment,
            String lpCurrentDirectory, ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public extern static bool DuplicateTokenEx(
            IntPtr hExistingToken,
            uint dwDesiredAccess,
            ref SECURITY_ATTRIBUTES lpTokenAttributes,
            SECURITY_IMPERSONATION_LEVEL ImpersonationLevel,
            TOKEN_TYPE TokenType,
            out IntPtr phNewToken);

        [DllImport("kernel32.dll", EntryPoint = "CloseHandle", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public extern static bool CloseHandle(IntPtr handle);

        #endregion

        public void LaunchProcess(string appPath,string args)
        {
            bool result;
            SECURITY_ATTRIBUTES securityAttributes = new SECURITY_ATTRIBUTES();
            //get the current active console session id
            uint dwSessionID = WTSGetActiveConsoleSessionId();

            Debug.Write("WTSGetActiveConsoleSessionId: " + dwSessionID);

            IntPtr Token = new IntPtr();
            //Query the user token for the session id
            result = WTSQueryUserToken((UInt32)dwSessionID, out Token);
            if (result == false)
            {
                Debug.Write("WTSQueryUserToken failed with " + Marshal.GetLastWin32Error());
            }

            IntPtr DuplicatedToken = IntPtr.Zero;

            result = DuplicateTokenEx(Token,
                MAXIMUM_ALLOWED,//To request all access rights that are valid for the caller, specify MAXIMUM_ALLOWED. 
                ref securityAttributes,
                SECURITY_IMPERSONATION_LEVEL.SecurityIdentification,
                TOKEN_TYPE.TokenPrimary,
                out DuplicatedToken);

            if (result == false)
            {
                Debug.Write("DuplicateTokenEx failed with " + Marshal.GetLastWin32Error());

            }
            else
            {
                Debug.Write("DuplicateTokenEx SUCCESS");
            }

            STARTUPINFO startupInfo = new STARTUPINFO();
            startupInfo.cb = Marshal.SizeOf(startupInfo);

            string applicationPath;
            //build the entire path using the application path and the arguments
            applicationPath = appPath + " " + args;

            PROCESS_INFORMATION processInfo = new PROCESS_INFORMATION();
            result = CreateProcessAsUser(DuplicatedToken, null, applicationPath, ref securityAttributes, ref securityAttributes, false, 0, (IntPtr)0, null, ref startupInfo, out processInfo);

            if (result == false)
            {
                Debug.Write("CreateProcessAsUser failed with " + Marshal.GetLastWin32Error());
            }
            else
            {
                Debug.Write("CreateProcessAsUser SUCCESS.  The child PID is" + processInfo.dwProcessId);
                CloseHandle(processInfo.hProcess);
                CloseHandle(processInfo.hThread);
            }

            result = CloseHandle(DuplicatedToken);
            if (result == false)
            {
                Debug.Write("CloseHandle LastError: " + Marshal.GetLastWin32Error());
            }
            else
            {
                Debug.Write("CloseHandle SUCCESS");
            }
        }
    }
}
