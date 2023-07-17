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

            var teams = await _context.Teams
                .Where(t => t.IsHuman == true)
                .ToListAsync();

            var regularStats = await _context.TeamRegularStats.Where(s => s.Season == season.Year).Include(t => t.Team).ToListAsync();

            var picks = await _context.TeamDraftPicks.Where(s => s.Year == season.Year).Include(t => t.Team).ToListAsync();

            regularStats.RemoveAll(stats => stats.TeamId == lottery.FirstTeamId);
            regularStats.RemoveAll(stats => stats.TeamId == lottery.SecondTeamId);
            regularStats.RemoveAll(stats => stats.TeamId == lottery.ThirdTeamId);
            regularStats.RemoveAll(stats => stats.TeamId == lottery.FourthTeamId);
            regularStats.RemoveAll(stats => stats.TeamId == lottery.FifthTeamId);
            regularStats.RemoveAll(stats => stats.TeamId == lottery.SixthTeamId);

            var firstTeamPick = picks.FirstOrDefault(p => p.Original == lottery.FirstTeam);
            var SecondTeamPick = picks.FirstOrDefault(p => p.Original == lottery.SecondTeam);
            var ThirdTeamPick = picks.FirstOrDefault(p => p.Original == lottery.ThirdTeam);
            var FourthTeamPick = picks.FirstOrDefault(p => p.Original == lottery.FourthTeam);
            var FifthTeamPick = picks.FirstOrDefault(p => p.Original == lottery.FifthTeam);
            var SixthTeamPick = picks.FirstOrDefault(p => p.Original == lottery.SixthTeam);

            var sortedStats = regularStats
                .OrderBy(stats => stats.WinPct)
                .ToList();

            for (int i = 0; i < sortedStats.Count; i++)
            {
                await Console.Out.WriteLineAsync(i + " - " + sortedStats[i].Team.Abrv);
            }




            List<Draft> newDraft = new List<Draft>
            {
                new Draft
                {
                    Original = lottery.FirstTeam,
                    Pick = 1,
                    Season = season.Year,
                    TeamId = firstTeamPick.TeamId,
                    Round = 1,
                    DateTime = null,
                    Player = null,
                    PlayerId = null,
                },

                new Draft
                {
                    Original = lottery.SecondTeam,
                    Pick = 2,
                    Season = season.Year,
                    TeamId = SecondTeamPick.TeamId,
                    Round = 1,
                    DateTime = null,
                    Player = null,
                    PlayerId = null,
                },

                new Draft
                {
                    Original = lottery.ThirdTeam,
                    Pick = 3,
                    Season = season.Year,
                    TeamId = ThirdTeamPick.TeamId,
                    Round = 1,
                    DateTime = null,
                    Player = null,
                    PlayerId = null,
                },

                new Draft
                {
                    Original = lottery.FourthTeam,
                    Pick = 4,
                    Season = season.Year,
                    TeamId = FourthTeamPick.TeamId,
                    Round = 1,
                    DateTime = null,
                    Player = null,
                    PlayerId = null,

                },

                new Draft
                {
                    Original = lottery.FifthTeam,
                    Pick = 5,
                    Season = season.Year,
                    TeamId = FifthTeamPick.TeamId,
                    Round = 1,
                    DateTime = null,
                    Player = null,
                    PlayerId = null,
                },

                new Draft
                {
                    Original = lottery.SixthTeam,
                    Pick = 6,
                    Season = season.Year,
                    TeamId = SixthTeamPick.TeamId,
                    Round = 1,
                    DateTime = null,
                    Player = null,
                    PlayerId = null,
                },

            };

            int pickNumber = 7;

            for (int i = 0; i < 16; i++)
            {
                string teamAbrv = sortedStats[i].Team.Abrv;
                int? teamId = picks
                    .Where(pick => pick.Original == teamAbrv && pick.Round == 1)
                    .Select(pick => pick.TeamId)
                    .FirstOrDefault();

                newDraft.Add(new Draft
                {
                    Original = teamAbrv,
                    Pick = pickNumber++,
                    Season = season.Year,
                    TeamId = (int)teamId,
                    Round = 1,
                    DateTime = null,
                    Player = null,
                    PlayerId = null,
                });
            }



            _context.AddRange(newDraft);

            await _context.SaveChangesAsync();
            response.Success = true;
            return response;
        }
    }
}
