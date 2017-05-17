using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Inventor;

namespace samplePlugin
{
    [Guid("FAE1868B-18EE-4090-A676-535335C4D41A")]
    public class PluginServer : Inventor.ApplicationAddInServer
    {
        public PluginServer()
        {
        }

        // Inventor application object.
        Inventor.InventorServer m_inventorServer;
        SampleAutomation m_automation;

        public dynamic Automation
        {
            get
            {
                return m_automation;
            }
        }

        public void Activate(ApplicationAddInSite AddInSiteObject, bool FirstTime)
        {
            Trace.TraceInformation(": samplePlugin: initializing... ");

            // Initialize AddIn members.
            m_inventorServer = AddInSiteObject.InventorServer;
            m_automation = new SampleAutomation(m_inventorServer);
        }

        public void Deactivate()
        {
            Trace.TraceInformation(": samplePlugin: deactivating... ");

            // Release objects.
            Marshal.ReleaseComObject(m_inventorServer);
            m_inventorServer = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void ExecuteCommand(int CommandID)
        {
            // obsolete
        }
    }
}
