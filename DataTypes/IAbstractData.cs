namespace DataTypes.Data
{
    public interface IAbstractData
    {
        (ulong, double) DataTuple { get; }
        string Uuid { get; }
    }
}