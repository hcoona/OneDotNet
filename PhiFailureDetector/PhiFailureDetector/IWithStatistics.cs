namespace PhiFailureDetector
{
    public interface IWithStatistics
    {
        double Avg { get; }

        int Count { get; }

        double StdDeviation { get; }

        long Sum { get; }

        double Variance { get; }
    }
}
