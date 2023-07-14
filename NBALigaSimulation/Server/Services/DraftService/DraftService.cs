using AutoMapper;
using NBALigaSimulation.Shared.Engine.Utils;

namespace NBALigaSimulation.Server.Services.DraftService
{
	public class DraftService : IDraftService
	{

		private readonly DataContext _context;
		private readonly IMapper _mapper;

		public DraftService(DataContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		public async Task<ServiceResponse<DraftLotteryDto>> GetLastLottery()
		{
			var response = new ServiceResponse<DraftLotteryDto>();
			var lottery = await _context.DraftLotteries.OrderBy(s => s.Season).LastOrDefaultAsync();

			if (lottery == null)
			{
				response.Success = false;
				response.Message = $"Loteria não econtrada!";
			}
			else
			{
				response.Data = _mapper.Map<DraftLotteryDto>(lottery);
			}

			return response;
		}

		public async Task<ServiceResponse<bool>> GenerateLottery()
		{
			var response = new ServiceResponse<bool>();
			var season = await _context.Seasons.OrderBy(s => s.Year).LastOrDefaultAsync();

			var lottery = await _context.DraftLotteries
				.Where(l => l.Season == season.Year)
				.OrderByDescending(l => l.Id)
				.FirstOrDefaultAsync();

			if (lottery != null)
			{
				response.Success = false;
				response.Message = "Loteria já criada!";
				return response;
			}

			var teams = await _context.TeamRegularStats
				.Where(t => t.Season == season.Year)
				.Include(t => t.Team)
				.ToListAsync();

			teams = teams.AsEnumerable().OrderByDescending(t => t.ConfRank).Take(6).ToList();
			teams = teams.OrderByDescending(t => t.WinPct).ToList();

			var order = DraftUtils.RunLottery(teams);

			var newLottery = new DraftLottery
			{
				Id = 1,
				Season = season.Year,
				FirstTeam = order[0].Team.Abrv,
				FirstTeamId = order[0].TeamId,
				SecondTeam = order[1].Team.Abrv,
				SecondTeamId = order[1].TeamId,
				ThirdTeam = order[2].Team.Abrv,
				ThirdTeamId = order[2].TeamId,
				FourthTeam = order[3].Team.Abrv,
				FourthTeamId = order[3].TeamId,
				FifthTeam = order[4].Team.Abrv,
				FifthTeamId = order[4].TeamId,
				SixthTeam = order[5].Team.Abrv,
				SixthTeamId = order[5].TeamId,
			};

			_context.Add(newLottery);
			await _context.SaveChangesAsync();
			response.Success = true;
			return response;
		}


	}
}
