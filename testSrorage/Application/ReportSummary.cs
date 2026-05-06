namespace testSrorage.Application
{
    public sealed class ReportSummary
    {
        public ReportSummary(int totalQuantity, int recordCount)
        {
            TotalQuantity = totalQuantity;
            RecordCount = recordCount;
        }

        public int TotalQuantity { get; private set; }
        public int RecordCount { get; private set; }
    }
}
