using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace LumsExtras
{
    public class Class1 : VNyanInterface.ITriggerHandler {
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
            // System.IO.File.WriteAllText(ErrorFile, e.ToString());
            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterString("_lum_ext_err", e.ToString());
            Log("DBG:" + e.ToString());
        }
        public void Awake() {
            try {
                VNyanInterface.VNyanInterface.VNyanTrigger.registerTriggerListener(this);
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
                                Process VNyanProcess = Process.GetCurrentProcess();
                                Log("Process ID: " + VNyanProcess.Id);
                                IntPtr[] WindowHandles = GetProcessWindows(VNyanProcess.Id);
                                long WindowStyle;
                                long ExtWindowStyle;
                                foreach (IntPtr WindowHandle in WindowHandles) {
                                    WindowStyle = GetWindowLongPtr64(WindowHandle, -16);
                                    ExtWindowStyle = GetWindowLongPtr64(WindowHandle, -20);
                                    if (ExtWindowStyle > 0) {
                                        Log("Window handle: " + WindowHandle.ToString());
                                        Log("Window style: " + WindowStyle.ToString("X8"));
                                        Log("Window extstyle: " + ExtWindowStyle.ToString("X8"));
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
                                            SetWindowLongPtr64(WindowHandle, -16, WindowStyle);
                                        }
                                        if (int1 != 0 || int2 != 0) {
                                            if (int1 == -99999999 && int2 == -99999999) { int1 = int2 = 0; } // Workaround for if user needs 0,0
                                            Log("Moving window to: " + int1.ToString() + ", " + int2.ToString());
                                            SetWindowPos(WindowHandle, 0, int1, int2, 0, 0, 1);
                                        }
                                    }
                                }
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
                        }
                    }
                }
            } catch (Exception e) {
                ErrorHandler(e);
            }
        }
    }
}
