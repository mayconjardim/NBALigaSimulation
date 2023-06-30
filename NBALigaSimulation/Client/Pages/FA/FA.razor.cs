using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace NBALigaSimulation.Client.Pages.FA
{
    partial class FA
    {

        private List<PlayerCompleteDto> players = new List<PlayerCompleteDto>();
        private List<FAOfferDto> faOffers = new List<FAOfferDto>();

        private string message = string.Empty;
        private string messageCssClass = string.Empty;
        private string selectedPosition { get; set; } = string.Empty;
        private int season { get; set; }

        string[] headings = { "NAME", "OFFER", "REMOVE" };
        protected override async Task OnInitializedAsync()
        {
            message = "Carregando Trocas...";

            var result = await PlayerService.GetAllFAPlayers();
            var offersResult = await FAService.GetOffersByTeamId();
            if (!result.Success)
            {
                message = result.Message;
            }
            else
            {
                players = result.Data;
                season = int.Parse(await LocalStorage.GetItemAsync<string>("season"));
            }

            if (!offersResult.Success)
            {
                message = offersResult.Message;
            }
            else
            {
                faOffers = offersResult.Data;
            }

        }

        private void HandlePositionChanged(string value)
        {

            if (value == "ALL")
            {
                value = "";
            }

            selectedPosition = value;
        }

        private List<PlayerCompleteDto> filteredPlayers
        {
            get
            {
                if (string.IsNullOrEmpty(selectedPosition))
                    return players;

                return players.Where(p => p.Pos == selectedPosition).ToList();
            }
        }

        private async Task DeleteOffer(int offerId)
        {

            var tradeResponse = await FAService.DeleteOffer(offerId);

            if (tradeResponse.Success)
            {
                messageCssClass = "success";
                Snackbar.Add("Proposta deletade com sucesso!", Severity.Success);

                faOffers.RemoveAll(t => t.Id == offerId);
                StateHasChanged();
            }
            else
            {
                messageCssClass = "error";
                Snackbar.Add("Proposta não foi deleta!", Severity.Error);

            }
        }

    }
}
