namespace NBALigaSimulation.Client.Shared.Services
{
    public class DataService : IDataService
    {
        public event EventHandler<int> OnDataChanged;

        public void SetData(int data)
        {
            OnDataChanged?.Invoke(this, data);
        }
    }
}
