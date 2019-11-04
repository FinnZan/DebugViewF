using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace FinnZan.Utilities
{
    [ServiceContract(Namespace = "FinnZan.Utilities")]
    interface IFinnZanLog
    {
        [OperationContract(Name = "Log")]
        void Log(string log);
    }
}
