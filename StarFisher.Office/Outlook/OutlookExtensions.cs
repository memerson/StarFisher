using System;
using StarFisher.Office.Utilities;
using OutlookApplication = Microsoft.Office.Interop.Outlook.Application;

namespace StarFisher.Office.Outlook
{
    public static class OutlookExtensions
    {
        public static void ActivateWorkOffline(this OutlookApplication outlook, ComObjectManager com)
        {
            if (outlook == null)
                throw new ArgumentNullException(nameof(outlook));
            if (com == null)
                throw new ArgumentNullException(nameof(com));

            var session = com.Get(() => outlook.Session);
            if (session.Offline)
                return;

            ToggleWorkOffline(outlook, com);
        }

        private static void ToggleWorkOffline(OutlookApplication outlook, ComObjectManager com)
        {
            var explorer = com.Get(outlook.ActiveExplorer);
            var commandBars = com.Get(() => explorer.CommandBars);
            commandBars.ExecuteMso(@"ToggleOnline");
        }
    }
}