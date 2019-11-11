using System.Collections.Generic;
using System.ServiceModel;

namespace FinnZan.Utilities
{
    [ServiceContract(Namespace = "FinnZan.Utilities")]
    interface IFinnZanLog
    {
        [OperationContract(Name = "Log")]
        void Log(List<string> logs);
    }
}
