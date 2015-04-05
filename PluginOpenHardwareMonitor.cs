// Uncomment these only if you want to export GetString() or ExecuteBang().
//#define DLLEXPORT_GETSTRING
//#define DLLEXPORT_EXECUTEBANG

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Rainmeter;

// Overview: This is a blank canvas on which to build your plugin.

// Note: Measure.GetString, Plugin.GetString, Measure.ExecuteBang, and
// Plugin.ExecuteBang have been commented out. If you need GetString
// and/or ExecuteBang and you have read what they are used for from the
// SDK docs, uncomment the function(s). Otherwise leave them commented out
// (or get rid of them)!

namespace PluginOpenHardwareMonitor
{
    internal class Measure
    {
        string sensorName, hardwareName, sensorType;
        internal Measure()
        {           
        }

        internal void Reload(Rainmeter.API api, ref double maxValue)
        {
            //Read query parameter from Rainmeter
            sensorName = api.ReadString("SensorName", "");
            hardwareName = api.ReadString("HardwareName", "");
            sensorType = api.ReadString("SensorType", "");            
            //Rainmeter.API.Log(Rainmeter.API.LogType.Debug, sensorType);
            


        }

        internal double Update()
        {
            OpenHardwareMonitorQuery q = new OpenHardwareMonitorQuery();
            double result = q.query(hardwareName, sensorName, sensorType);
            Rainmeter.API.Log(Rainmeter.API.LogType.Debug, "UPDATE:" + sensorType + " Result:" + result);
            return result;            
        }
        
#if DLLEXPORT_GETSTRING
        internal string GetString()
        {            
        }
#endif
        
#if DLLEXPORT_EXECUTEBANG
        internal void ExecuteBang(string args)
        {
        }
#endif
    }

    public static class Plugin
    {
#if DLLEXPORT_GETSTRING
        static IntPtr StringBuffer = IntPtr.Zero;
#endif

        [DllExport]
        public static void Initialize(ref IntPtr data, IntPtr rm)
        {
            data = GCHandle.ToIntPtr(GCHandle.Alloc(new Measure()));
        }

        [DllExport]
        public static void Finalize(IntPtr data)
        {
            GCHandle.FromIntPtr(data).Free();
            
#if DLLEXPORT_GETSTRING
            if (StringBuffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(StringBuffer);
                StringBuffer = IntPtr.Zero;
            }
#endif
        }

        [DllExport]
        public static void Reload(IntPtr data, IntPtr rm, ref double maxValue)
        {
            Measure measure = (Measure)GCHandle.FromIntPtr(data).Target;
            measure.Reload(new Rainmeter.API(rm), ref maxValue);
        }

        [DllExport]
        public static double Update(IntPtr data)
        {
            Measure measure = (Measure)GCHandle.FromIntPtr(data).Target;
            return measure.Update();
        }
        
#if DLLEXPORT_GETSTRING
        [DllExport]
        public static IntPtr GetString(IntPtr data)
        {
            Measure measure = (Measure)GCHandle.FromIntPtr(data).Target;
            if (StringBuffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(StringBuffer);
                StringBuffer = IntPtr.Zero;
            }

            string stringValue = measure.GetString();
            if (stringValue != null)
            {
                StringBuffer = Marshal.StringToHGlobalUni(stringValue);
            }

            return StringBuffer;
        }
#endif

#if DLLEXPORT_EXECUTEBANG
        [DllExport]
        public static void ExecuteBang(IntPtr data, IntPtr args)
        {
            Measure measure = (Measure)GCHandle.FromIntPtr(data).Target;
            measure.ExecuteBang(Marshal.PtrToStringUni(args));
        }
#endif
    }
}
