using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ShutdownVR
{
    class CloseSteamVR
    {
        public static void CloseSteam()
        {
            try
            {
                var p = Process.GetProcessesByName("vrmonitor").First();
                foreach (var handle in EnumerateProcessWindowHandles(Process.GetProcessesByName("vrmonitor").First().Id))
                {
                    StringBuilder message = new StringBuilder(1000);
                    SendMessage(handle, WM_CLOSE, message.Capacity, message);
                    p.WaitForExit();
                    Debug.WriteLine(message);
                    Shutdown();
                }
                //Console.WriteLine("Hello World!");
            }
            catch (System.Exception) { }
        }
        private const uint WM_CLOSE = 0x10;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam,
            StringBuilder lParam);
        delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern bool EnumThreadWindows(int dwThreadId, EnumThreadDelegate lpfn,
            IntPtr lParam);

        static IEnumerable<IntPtr> EnumerateProcessWindowHandles(int processId)
        {
            try
            {
                var handles = new List<IntPtr>();

                foreach (ProcessThread thread in Process.GetProcessById(processId).Threads)
                    EnumThreadWindows(thread.Id,
                        (hWnd, lParam) => { handles.Add(hWnd); return true; }, IntPtr.Zero);

                return handles;
            }
            catch (System.Exception) { return null; }
        }

        static void Shutdown()
        {
            Process.Start("shutdown.exe", "-s -t 00");
        }
    }
}