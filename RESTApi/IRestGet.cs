using DataTypes.Data;

namespace Rest.Api
{
    public interface IRestGet
    {
        List<IAbstractData> LastRestResults { get; }
        string URL { get; }
        IDictionary<string, string> Channels { get; }

        void Dispose();
        Task<List<IAbstractData>> UpdateRestReults();

        Task<List<IAbstractData>> UpdateRestReults(IDictionary<string, string> channels);
    }
}