using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using static LumsExtras.LumsExtras;
using UnityEngine.XR;
using VNyanInterface;

namespace LumsExtras {

        public class LumsExtras : IVNyanPluginManifest, VNyanInterface.ITriggerHandler {

        public string PluginName { get; } = "LumsExtras";
        public string Version    { get; } = "v1.2";
        public string Title      { get; } = "Lum's Extras and Tweaks";
        public string Author     { get; } = "LumKitty";
        public string Website    { get; } = "https://lum.uk/";

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT rect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT {
            public int xMin;        // x position of upper-left corner
            public int yMin;         // y position of upper-left corner
            public int xMax;       // x position of lower-right corner
            public int yMax;      // y position of lower-right corner
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        private static extern bool SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);
        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
        public static extern IntPtr FindWindowEx(IntPtr parentWindow, IntPtr previousChildWindow, string windowClass, string windowTitle);
        [DllImport("user32.dll", EntryPoint = "GetWindowThreadProcessId")]
        private static extern IntPtr GetWindowThreadProcessId(IntPtr window, out int process);
        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern long SetWindowLongPtr64(IntPtr hWnd, int nIndex, long dwNewLong);
        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        private static extern long GetWindowLongPtr64(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
        private static extern int GetSystemMetrics(int Index);

        Process VNyanProcess = Process.GetCurrentProcess();
        static IntPtr VNyanWindowHandle = IntPtr.Zero;
        static int WindowsBorderPadding = GetSystemMetrics(92); // SM_CXPADDEDBORDER
        static int WindowsBorderWidth = GetSystemMetrics(32);   // SM_CXFRAME
        static int WindowsTitleBarHeight = GetSystemMetrics(4); // SM_CYCAPTION

        private IntPtr[] GetProcessWindows(int process) {
            IntPtr[] apRet = (new IntPtr[256]);
            int iCount = 0;
            IntPtr pLast = IntPtr.Zero;
            do {
                pLast = FindWindowEx(IntPtr.Zero, pLast, null, null);
                int iProcess_;
                GetWindowThreadProcessId(pLast, out iProcess_);
                if (iProcess_ == process) apRet[iCount++] = pLast;
            } while (pLast != IntPtr.Zero);
            System.Array.Resize(ref apRet, iCount);
            return apRet;
        }
        void CallVNyan(string TriggerName, int int1, int int2, int int3, string Text1, string Text2, string Text3) {
            if (TriggerName.Length > 0) {
                VNyanInterface.VNyanInterface.VNyanTrigger.callTrigger(TriggerName, int1, int2, int3, Text1, Text2, Text3);
            }
        }
        void Log(string Message) {
            CallVNyan("_lum_dbg_raw", 0, 0, 0, Message, "", "");
        }
        void ErrorHandler(Exception e) {
            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterString("_lum_ext_err", e.ToString());
            Log("DBG:" + e.ToString());
        }
        public void InitializePlugin() {
            try {
                VNyanInterface.VNyanInterface.VNyanTrigger.registerTriggerListener(this);
                
                UnityEngine.Debug.Log("Lum's Extras starting. VNyan Process ID: " + VNyanProcess.Id);
                IntPtr[] WindowHandles = GetProcessWindows(VNyanProcess.Id);
                long ExtWindowStyle;
                UnityEngine.Debug.Log("Windows border width: " + WindowsBorderWidth);
                UnityEngine.Debug.Log("Windows border padding: " + WindowsBorderPadding);
                UnityEngine.Debug.Log("Windows titlebar height: " + WindowsTitleBarHeight);
                foreach (IntPtr WindowHandle in WindowHandles) {
                    ExtWindowStyle = GetWindowLongPtr64(WindowHandle, -20);
                    if (ExtWindowStyle > 0) {
                        VNyanWindowHandle = WindowHandle;
                        UnityEngine.Debug.Log("Found VNyan WindowHandle: " + VNyanWindowHandle.ToString());
                        break;
                    }
                }
                
            } catch (Exception e) {
                ErrorHandler(e);
            }
        }


        public void triggerCalled(string name, int int1, int int2, int int3, string text1, string text2, string text3) {
            try {
                if (name.Length > 10) {
                    name = name.ToLower();
                    if (name.Substring(0, 9) == "_lum_ext_") {
                        //Log("Detected trigger: " + name + " with " + int1.ToString() + ", " + SessionID.ToString() + ", " + PlatformID.ToString() + ", " + text1 + ", " + text2 + ", " + Callback);
                        switch (name.Substring(8)) {
                            case "_setwindow":
                                long WindowStyle = GetWindowLongPtr64(VNyanWindowHandle, -16);
                                Log("Window style: " + WindowStyle.ToString("X8"));
                                if (int3 > 0) {
                                    switch (int3) {
                                        case 1:
                                            WindowStyle = WindowStyle & 0b_1111_1111_1111_1010_1111_1111_1111_1111;
                                            break;
                                        case 2:
                                            WindowStyle = WindowStyle ^ 0b_0000_0000_0000_0101_0000_0000_0000_0000;
                                            break;
                                        default:
                                            Log("Unknown value: " + int3.ToString());
                                            break;
                                    }
                                    Log("New style: " + WindowStyle.ToString("X8"));
                                    SetWindowLongPtr64(VNyanWindowHandle, -16, WindowStyle);
                                }
                                if (int1 != 0 || int2 != 0) {
                                    if (int1 == -99999999 && int2 == -99999999) { int1 = int2 = 0; } // Workaround for if user needs 0,0
                                    Log("Moving window to: " + int1.ToString() + ", " + int2.ToString());
                                    SetWindowPos(VNyanWindowHandle, 0, int1-WindowsBorderWidth-WindowsBorderPadding, int2, 0, 0, 1);
                                }
                                break;
                            case "_getwindow":
                                RECT Result = new RECT();
                                GetWindowRect(VNyanWindowHandle, ref Result);
                                Log("X1: " + Result.xMin + ", Y1: " + Result.yMin + ", X2: " + Result.xMax + ", Y2: " + Result.yMax);
                                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_ext_winX", Result.xMin + WindowsBorderWidth + WindowsBorderPadding);
                                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_ext_winY", Result.yMin);
                                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_ext_winW", Result.xMax - Result.xMin - (WindowsBorderWidth * 2) - (WindowsBorderPadding * 2));
                                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_ext_winH", Result.yMax - Result.yMin - (WindowsBorderWidth * 2) - (WindowsBorderPadding * 2) - WindowsTitleBarHeight);
                                break;
                            case "_getdesktop":
                                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_ext_desktopX", GetSystemMetrics(78)); // SM_CXVIRTUALSCREEN
                                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_ext_desktopY", GetSystemMetrics(79)); // SM_CYVIRTUALSCREEN
                                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_ext_monitorX", GetSystemMetrics(0));  // SM_CXSCREEN
                                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_ext_monitorY", GetSystemMetrics(1));  // SM_CYSCREEN
                                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("_lum_ext_monitors", GetSystemMetrics(80)); // SM_CMONITORS
                                break;
                            case "_getcam":
                                var camera = Camera.main;
                                float pX = camera.transform.position.x;
                                float pY = camera.transform.position.y;
                                float pZ = camera.transform.position.z;
                                float rX = camera.transform.rotation.eulerAngles.x;
                                float rY = camera.transform.rotation.eulerAngles.y;
                                float rZ = camera.transform.rotation.eulerAngles.z;
                                float FOV = camera.fieldOfView;
                                string CameraData = "Current camera position (for use with props, LIV etc.):\n" +
                                    "fov=" + FOV.ToString("0.0000000000000") + "\n" +
                                    "x=" + pX.ToString("0.0000000000000") + "\n" +
                                    "y=" + pY.ToString("0.0000000000000") + "\n" +
                                    "z=" + pZ.ToString("0.0000000000000") + "\n" +
                                    "rx=" + rX.ToString("0.0000000000000") + "\n" +
                                    "ry=" + rY.ToString("0.0000000000000") + "\n" +
                                    "rz=" + rZ.ToString("0.0000000000000");
                                Log(CameraData);
                                string FileName = Path.GetTempFileName();
                                File.WriteAllText(FileName, CameraData);
                                ProcessStartInfo startInfo = new ProcessStartInfo();
                                startInfo.FileName = "notepad.exe";
                                startInfo.Arguments = FileName;
                                Process.Start(startInfo);
                                break;
                            case "_setcam":
                                if (int1 == 1) {
                                    Camera.main.gateFit = Camera.GateFitMode.Vertical;
                                } else if (int1 == 2) {
                                    Camera.main.gateFit = Camera.GateFitMode.Horizontal;
                                }
                                if (int2 == 1) {
                                    Camera.main.usePhysicalProperties = false;
                                } else if (int2 == 2) {
                                    Camera.main.usePhysicalProperties = true;
                                }
                                break;
                        }
                    }
                }
            } catch (Exception e) {
                ErrorHandler(e);
            }
        }
    }
}
