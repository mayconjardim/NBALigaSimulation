using AutoMapper;
using NBALigaSimulation.Shared.Engine.Utils;
using System.Collections.Generic;

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


        public async Task<ServiceResponse<List<DraftDto>>> GetLastDraft()
        {
            var response = new ServiceResponse<List<DraftDto>>();
            var season = await _context.Seasons.OrderBy(s => s.Year).LastOrDefaultAsync();
            var draft = await _context.Drafts.OrderBy(s => s.Pick).Where(d => d.Season == season.Year).Include(d => d.Team).ToListAsync();

            if (draft == null)
            {
                response.Success = false;
                response.Message = $"Draft não econtrado!";
            }
            else
            {
                response.Data = _mapper.Map<List<DraftDto>>(draft);
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

        public async Task<ServiceResponse<bool>> GenerateDraft()
        {

            var response = new ServiceResponse<bool>();
            var season = await _context.Seasons.OrderBy(s => s.Year).LastOrDefaultAsync();

            var lottery = await _context.DraftLotteries
               .Where(l => l.Season == season.Year)
               .OrderByDescending(l => l.Id)
               .FirstOrDefaultAsync();

            var newDraft = await _context.Drafts
            .Where(l => l.Season == season.Year)
            .OrderByDescending(l => l.Id).ToListAsync();

            if (newDraft.Count > 0)
            {
                response.Success = false;
                response.Message = "Draft já criado!";
                return response;
            }

            var teams = await _context.Teams
                .Where(t => t.IsHuman == true)
                .ToListAsync();

            var regularStats = await _context.TeamRegularStats.Where(s => s.Season == season.Year).Include(t => t.Team).ToListAsync();

            var picks = await _context.TeamDraftPicks.Where(s => s.Year == season.Year).Include(t => t.Team).ToListAsync();

            newDraft = DraftUtils.GenerateDraft(lottery, season.Year, regularStats, picks, teams);

            _context.AddRange(newDraft);

            await _context.SaveChangesAsync();
            response.Success = true;
            return response;
        }

        public async Task<ServiceResponse<bool>> SelectPlayer(Draft Player)
        {

            var response = new ServiceResponse<bool>();
            var season = await _context.Seasons.OrderBy(s => s.Year).LastOrDefaultAsync();

            var selectedPlayer = await _context.Players.Where()



            _context.Add();

            await _context.SaveChangesAsync();
            response.Success = true;
            return response;
        }

    }
}
