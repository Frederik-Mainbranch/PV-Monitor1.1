using System;
using DevExpress.ExpressApp.Xpo;

namespace PV_Monitor.Blazor.Server.Services {
    public class XpoDataStoreProviderAccessor {
        public IXpoDataStoreProvider DataStoreProvider { get; set; }
    }
}
