namespace NBALigaSimulation.Client.Services.DataService;

public interface IDataService
{
    event EventHandler<int> OnDataChanged;
    void SetData(int data);
}