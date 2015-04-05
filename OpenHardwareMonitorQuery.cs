using System;
using System.Collections.Generic;
using System.Text;
using System.Management;

/* A simple class for making query of Open Hardware Monitor WMI Service */
namespace PluginOpenHardwareMonitor
{
    class OpenHardwareMonitorQuery
    {
        public double query(string hwName, string sensorName, string sensorType)
        {
            //Need to first specify the Hardware name and the name of the sensor then sensor type
            //Error checking SensorType or Name is not in specific HardwareName
            string queryStr;
            ManagementObject result = new ManagementObject();

            //First get HardwareIdentifier by name
            string strHwID = getHardwareIdentifierByName(hwName);
            queryStr = "SELECT value FROM sensor WHERE " + "Parent=\"" + strHwID + "\" AND Name=\"" + sensorName + "\" AND SensorType=\"" + sensorType + "\"";
            ManagementObjectCollection mCollection = makeQuery(queryStr);

            if (mCollection.Count == 1) //Expecting only one return value
            {
                foreach (ManagementObject m in makeQuery(queryStr))
                {
                    result = m;
                    break;
                }
                return Convert.ToDouble(result["Value"]);
            }
            else
            {
                return -1.0;
            }            
        }

        private ManagementObjectCollection makeQuery(string queryStr)
        {
            ManagementScope scope = new ManagementScope("root\\OpenHardwareMonitor");
            ObjectQuery query = new ObjectQuery(queryStr);
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
            return searcher.Get();
        }

        private string getHardwareIdentifierByName(string hwName)
        {
            ManagementObject result = null;
            var queryStr = "SELECT identifier FROM hardware WHERE " + "Name=\"" + hwName + "\"";
            foreach (ManagementObject mgtObj in makeQuery(queryStr))
            {
                result = mgtObj;
                break;
            }
            return result["Identifier"].ToString();
        }

    }
}
