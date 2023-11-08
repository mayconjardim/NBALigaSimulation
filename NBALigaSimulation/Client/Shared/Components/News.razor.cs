namespace NBALigaSimulation.Client.Shared.Components
{
    partial class News
    {

        private List<NewsDto> news = new List<NewsDto>();

        private string message = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            message = "Carregando Stats...";

            var result = await NewsService.GetAllNews();
            if (!result.Success)
            {
                message = result.Message;
            }
            else
            {
                news = result.Data.ToList();
            }

        }

    }
}
