using System.Globalization;
using System.Text;

namespace NBALigaSimulation.Client.Pages.Stats
{
    partial class PlayerStats
    {

        private List<PlayerRegularStatsDto> playerStats = new List<PlayerRegularStatsDto>();
        private string message = string.Empty;

        private string sortedColumn = "PtsPG";
        private bool isAscending = true;
        private int pageSize = 20;
        private int currentPage = 1;
        private int totalItems;


        protected override async Task OnInitializedAsync()
        {
            var result = await StatsService.GetAllPlayerRegularStats();

            if (!result.Success)
            {
                message = "Não existem Stats";
            }
            else
            {
                playerStats = result.Data.OrderByDescending(player => Convert.ToDouble(player.PtsPG, CultureInfo.InvariantCulture)).ToList();
                totalItems = result.Data.Count;
            }

        }

        private List<PlayerRegularStatsDto> GetCurrentPage()
        {
            int startIndex = (currentPage - 1) * pageSize;
            int endIndex = startIndex + pageSize;

            if (startIndex < 0)
            {
                startIndex = 0;
            }

            if (endIndex > playerStats.Count)
            {
                endIndex = playerStats.Count;
            }

            return playerStats.Skip(startIndex).Take(endIndex - startIndex).ToList();
        }

        private async Task OnPageChanged(int newPage)
        {
            currentPage = newPage;
        }

        private void NextPage()
        {
            if (currentPage < TotalPages)
            {
                OnPageChanged(currentPage + 1);
            }
        }

        private void ReturnPage()
        {
            if (currentPage > 1)
            {
                OnPageChanged(currentPage - 1);
            }
        }

        private int TotalPages
        {
            get
            {
                return (int)Math.Ceiling((double)totalItems / pageSize);
            }
        }

        private string GetSortIcon(string columnName)
        {
            if (columnName == sortedColumn)
            {
                return isAscending ? "asc" : "desc";
            }
            return string.Empty;
        }

        private void SortTable(string columnName)
        {
            if (columnName == sortedColumn)
            {
                isAscending = !isAscending;
            }
            else
            {
                sortedColumn = columnName;
                isAscending = true;
            }


            switch (columnName)
            {
                case "PtsPG":
                    playerStats = isAscending
                        ? playerStats.OrderBy(player => Convert.ToDouble(player.PtsPG, CultureInfo.InvariantCulture)).ToList()
                        : playerStats.OrderByDescending(player => Convert.ToDouble(player.PtsPG, CultureInfo.InvariantCulture)).ToList();
                    break;

                case "PfPG":
                    playerStats = isAscending
                        ? playerStats.OrderBy(player => Convert.ToDouble(player.PfPG, CultureInfo.InvariantCulture)).ToList()
                        : playerStats.OrderByDescending(player => Convert.ToDouble(player.PfPG, CultureInfo.InvariantCulture)).ToList();
                    break;

                case "BlkPG":
                    playerStats = isAscending
                        ? playerStats.OrderBy(player => Convert.ToDouble(player.BlkPG, CultureInfo.InvariantCulture)).ToList()
                        : playerStats.OrderByDescending(player => Convert.ToDouble(player.BlkPG, CultureInfo.InvariantCulture)).ToList();
                    break;

                case "StlPG":
                    playerStats = isAscending
                        ? playerStats.OrderBy(player => Convert.ToDouble(player.StlPG, CultureInfo.InvariantCulture)).ToList()
                        : playerStats.OrderByDescending(player => Convert.ToDouble(player.StlPG, CultureInfo.InvariantCulture)).ToList();
                    break;
                case "TovPG":
                    playerStats = isAscending
                        ? playerStats.OrderBy(player => Convert.ToDouble(player.TovPG, CultureInfo.InvariantCulture)).ToList()
                        : playerStats.OrderByDescending(player => Convert.ToDouble(player.TovPG, CultureInfo.InvariantCulture)).ToList();
                    break;

                case "AstPG":
                    playerStats = isAscending
                        ? playerStats.OrderBy(player => Convert.ToDouble(player.AstPG, CultureInfo.InvariantCulture)).ToList()
                        : playerStats.OrderByDescending(player => Convert.ToDouble(player.AstPG, CultureInfo.InvariantCulture)).ToList();
                    break;

                case "TRebPG":
                    playerStats = isAscending
                        ? playerStats.OrderBy(player => Convert.ToDouble(player.TRebPG, CultureInfo.InvariantCulture)).ToList()
                        : playerStats.OrderByDescending(player => Convert.ToDouble(player.TRebPG, CultureInfo.InvariantCulture)).ToList();
                    break;

                case "DRebPG":
                    playerStats = isAscending
                        ? playerStats.OrderBy(player => Convert.ToDouble(player.DRebPG, CultureInfo.InvariantCulture)).ToList()
                        : playerStats.OrderByDescending(player => Convert.ToDouble(player.DRebPG, CultureInfo.InvariantCulture)).ToList();
                    break;
                case "ORebPG":
                    playerStats = isAscending
                        ? playerStats.OrderBy(player => Convert.ToDouble(player.ORebPG, CultureInfo.InvariantCulture)).ToList()
                        : playerStats.OrderByDescending(player => Convert.ToDouble(player.ORebPG, CultureInfo.InvariantCulture)).ToList();
                    break;

                case "FtPct":
                    playerStats = isAscending
                        ? playerStats.OrderBy(player => Convert.ToDouble(player.FtPct, CultureInfo.InvariantCulture)).ToList()
                        : playerStats.OrderByDescending(player => Convert.ToDouble(player.FtPct, CultureInfo.InvariantCulture)).ToList();
                    break;

                case "FtaPG":
                    playerStats = isAscending
                        ? playerStats.OrderBy(player => Convert.ToDouble(player.FtaPG, CultureInfo.InvariantCulture)).ToList()
                        : playerStats.OrderByDescending(player => Convert.ToDouble(player.FtaPG, CultureInfo.InvariantCulture)).ToList();
                    break;

                case "FtPG":
                    playerStats = isAscending
                        ? playerStats.OrderBy(player => Convert.ToDouble(player.FtPG, CultureInfo.InvariantCulture)).ToList()
                        : playerStats.OrderByDescending(player => Convert.ToDouble(player.FtPG, CultureInfo.InvariantCulture)).ToList();
                    break;
                case "TpPct":
                    playerStats = isAscending
                        ? playerStats.OrderBy(player => Convert.ToDouble(player.TpPct, CultureInfo.InvariantCulture)).ToList()
                        : playerStats.OrderByDescending(player => Convert.ToDouble(player.TpPct, CultureInfo.InvariantCulture)).ToList();
                    break;
                case "TpaPG":
                    playerStats = isAscending
                        ? playerStats.OrderBy(player => Convert.ToDouble(player.TpaPG, CultureInfo.InvariantCulture)).ToList()
                        : playerStats.OrderByDescending(player => Convert.ToDouble(player.TpaPG, CultureInfo.InvariantCulture)).ToList();
                    break;
                case "TpPG":
                    playerStats = isAscending
                        ? playerStats.OrderBy(player => Convert.ToDouble(player.TpPG, CultureInfo.InvariantCulture)).ToList()
                        : playerStats.OrderByDescending(player => Convert.ToDouble(player.TpPG, CultureInfo.InvariantCulture)).ToList();
                    break;
                case "FgPct":
                    playerStats = isAscending
                        ? playerStats.OrderBy(player => Convert.ToDouble(player.FgPct, CultureInfo.InvariantCulture)).ToList()
                        : playerStats.OrderByDescending(player => Convert.ToDouble(player.FgPct, CultureInfo.InvariantCulture)).ToList();
                    break;
                case "FgaPG":
                    playerStats = isAscending
                        ? playerStats.OrderBy(player => Convert.ToDouble(player.FgaPG, CultureInfo.InvariantCulture)).ToList()
                        : playerStats.OrderByDescending(player => Convert.ToDouble(player.FgaPG, CultureInfo.InvariantCulture)).ToList();
                    break;
                case "FgPG":
                    playerStats = isAscending
                        ? playerStats.OrderBy(player => Convert.ToDouble(player.FgPG, CultureInfo.InvariantCulture)).ToList()
                        : playerStats.OrderByDescending(player => Convert.ToDouble(player.FgPG, CultureInfo.InvariantCulture)).ToList();
                    break;
                case "MinPG":
                    playerStats = isAscending
                        ? playerStats.OrderBy(player => Convert.ToDouble(player.MinPG, CultureInfo.InvariantCulture)).ToList()
                        : playerStats.OrderByDescending(player => Convert.ToDouble(player.MinPG, CultureInfo.InvariantCulture)).ToList();
                    break;
                case "Games":
                    playerStats = isAscending
                        ? playerStats.OrderBy(player => Convert.ToDouble(player.Games, CultureInfo.InvariantCulture)).ToList()
                        : playerStats.OrderByDescending(player => Convert.ToDouble(player.Games, CultureInfo.InvariantCulture)).ToList();
                    break;
            }
        }

        public string AbbreviateLastName(string fullName)
        {
            string[] nameParts = fullName.Split(' ');

            if (fullName.Length <= 20)
            {
                return fullName;
            }

            StringBuilder abbreviatedName = new StringBuilder();
            for (int i = 0; i < nameParts.Length - 1; i++)
            {
                abbreviatedName.Append(nameParts[i]);
                abbreviatedName.Append(' ');
            }

            string lastName = nameParts[nameParts.Length - 1];
            abbreviatedName.Append(lastName[0]);
            abbreviatedName.Append(". ");

            return abbreviatedName.ToString().Trim();
        }



    }
}
