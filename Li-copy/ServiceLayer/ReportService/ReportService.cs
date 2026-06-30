using Li_copy.I_InterfaceLayer.ReportInterface;

namespace Li_copy.ServiceLayer.ReportService
{
    public class ReportService  : IReportService
    {
        private readonly IReportDLL _reportDLL;

        public ReportService(IReportDLL reportDLL)
        {
            _reportDLL = reportDLL;
        }

    }
}
