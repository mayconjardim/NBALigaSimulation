namespace NBALigaSimulation.Client.Shared.Services
{
    public interface IDataService
    {
        event EventHandler<int> OnDataChanged;
        void SetData(int data);
    }
}
