namespace DataTypes.Data
{
    public interface IData
    {
        string uuid { get; set; }
        long last { get; set; }
        int interval { get; set; }
        string protocol { get; set; }
        List<List<double>> tuples { get; set; }
    }
}

