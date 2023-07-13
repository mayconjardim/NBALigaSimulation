using AutoMapper;
using NBALigaSimulation.Shared.Engine.Utils;

namespace NBALigaSimulation.Server.Services.PlayoffsService
{
	public class PlayoffsService : IPlayoffsService
	{

		private readonly DataContext _context;
		private readonly IMapper _mapper;

		public PlayoffsService(DataContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		public async Task<ServiceResponse<List<PlayoffsDto>>> GetPlayoffs()
		{
			var response = new ServiceResponse<List<PlayoffsDto>>();
			var season = await _context.Seasons.OrderBy(s => s.Year).LastOrDefaultAsync();

			var playoffs = await _context.Playoffs.Where(p => p.Season == season.Year).Include(p => p.TeamOne).Include(p => p.TeamTwo).ToListAsync();

			if (playoffs == null)
			{
				response.Success = false;
				response.Message = $"Playoffs não econtrado!";
			}
			else
			{

				response.Data = _mapper.Map<List<PlayoffsDto>>(playoffs);
			}

			return response;
		}

		public async Task<ServiceResponse<bool>> GeneratePlayoffs()
		{
			var response = new ServiceResponse<bool>();
			var season = await _context.Seasons.OrderBy(s => s.Year).LastOrDefaultAsync();

			List<Team> teamsEast = await _context.Teams
				.Include(t => t.TeamRegularStats)
				.Where(t => t.Conference == "East" && t.TeamRegularStats.Any(trs => trs.Season == season.Year))
				.ToListAsync();

			List<Team> teamsWest = await _context.Teams
				.Include(t => t.TeamRegularStats)
				.Where(t => t.Conference == "West" && t.TeamRegularStats.Any(trs => trs.Season == season.Year))
				.ToListAsync();

			var playoffs = PlayoffsUtils.Generate1stRound(teamsEast, teamsWest, season.Year);
			var games = PlayoffsUtils.GenerateRoundGames(playoffs, season);

			_context.AddRange(games);
			_context.AddRange(playoffs);
			await _context.SaveChangesAsync();

			response.Success = true;
			return response;
		}

		public async Task<ServiceResponse<bool>> Generate2Round()
		{
			var response = new ServiceResponse<bool>();
			var season = await _context.Seasons.OrderBy(s => s.Year).LastOrDefaultAsync();

			List<Playoffs> playoffs = await _context.Playoffs
			.Where(p => p.Season == season.Year)
			.Include(t => t.TeamOne).ThenInclude(t => t.TeamRegularStats)
			.Include(t => t.TeamTwo).ThenInclude(t => t.TeamRegularStats)
			.ToListAsync();

			var newPlayoffs = PlayoffsUtils.Generate2ndRound(playoffs, season.Year);
			var games = PlayoffsUtils.GenerateRoundGames(newPlayoffs, season);

			_context.AddRange(games);
			_context.AddRange(newPlayoffs);
			await _context.SaveChangesAsync();

			response.Success = true;
			return response;
		}

		public async Task<ServiceResponse<bool>> Generate3Round()
		{
			var response = new ServiceResponse<bool>();
			var season = await _context.Seasons.OrderBy(s => s.Year).LastOrDefaultAsync();

			List<Playoffs> playoffs = await _context.Playoffs
			.Where(p => p.Season == season.Year)
			.Include(t => t.TeamOne).ThenInclude(t => t.TeamRegularStats)
			.Include(t => t.TeamTwo).ThenInclude(t => t.TeamRegularStats)
			.ToListAsync();

			var newPlayoffs = PlayoffsUtils.Generate3ndRound(playoffs, season.Year);
			var games = PlayoffsUtils.GenerateRoundGames(newPlayoffs, season);

			_context.AddRange(games);
			_context.AddRange(newPlayoffs);
			await _context.SaveChangesAsync();

			response.Success = true;
			return response;
		}


	}
}
