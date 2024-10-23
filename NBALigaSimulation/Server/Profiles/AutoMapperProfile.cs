using AutoMapper;
using NBALigaSimulation.Client.Pages.Players.PlayerPage;
using NBALigaSimulation.Shared.Dtos.Drafts;
using NBALigaSimulation.Shared.Dtos.FA;
using NBALigaSimulation.Shared.Dtos.GameNews;
using NBALigaSimulation.Shared.Dtos.Games;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Dtos.Playoffs;
using NBALigaSimulation.Shared.Dtos.Seasons;
using NBALigaSimulation.Shared.Dtos.Teams;
using NBALigaSimulation.Shared.Dtos.Trades;
using NBALigaSimulation.Shared.Models.Drafts;
using NBALigaSimulation.Shared.Models.FA;
using NBALigaSimulation.Shared.Models.GameNews;
using NBALigaSimulation.Shared.Models.Games;
using NBALigaSimulation.Shared.Models.Players;
using NBALigaSimulation.Shared.Models.SeasonPlayoffs;
using NBALigaSimulation.Shared.Models.Seasons;
using NBALigaSimulation.Shared.Models.Teams;
using NBALigaSimulation.Shared.Models.Trades;
using Player = NBALigaSimulation.Shared.Models.Players.Player;

namespace NBALigaSimulation.Server.Profiles
{
    public class AutoMapperProfile : Profile
    {

        public AutoMapperProfile()
        {
            CreateMap<Player, PlayerSimpleDto>();
            CreateMap<Player, PlayerCompleteDto>()
                .ForMember(dest => dest.Ratings, opt => opt.MapFrom(src => src.Ratings))
                .ForMember(dest => dest.TeamName, opt => opt.MapFrom(src => src.Team.Name))
                .ForMember(dest => dest.TeamAbrv, opt => opt.MapFrom(src => src.Team.Abrv));

            CreateMap<PlayerCompleteDto, Player>().ReverseMap();



            CreateMap<CreatePlayersDto, Player>()
                .ForMember(dest => dest.Draft, opt => opt.Ignore()) 
                .ForMember(dest => dest.Stats, opt => opt.Ignore())   
                .ForMember(dest => dest.Contract, opt => opt.Ignore()) 
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName))

                // Se precisar mapear Born e Ratings
                .ForMember(dest => dest.Born, opt => opt.MapFrom(src => src.Born))  
                .ForMember(dest => dest.Ratings, opt => opt.MapFrom(src => src.Ratings));

            CreateMap<BornDto, Born>().ReverseMap();
            CreateMap<PlayerRatingsDto, PlayerRatings>().ReverseMap();
            
            CreateMap<PlayerContract, PlayerContractDto>().ReverseMap();

            CreateMap<PlayerAwards, PlayerAwardsDto>().ReverseMap();

            CreateMap<Team, TeamSimpleDto>();
            CreateMap<Team, TeamSimpleWithPlayersDto>().ReverseMap();
            CreateMap<Team, TeamCompleteDto>().ReverseMap();
            CreateMap<TeamGameplan, TeamGameplanDto>().ReverseMap();

            CreateMap<Game, GameCompleteDto>()
              .ForMember(dest => dest.HomeTeam, opt => 
                  opt.MapFrom(src => src.HomeTeam.Abrv))
              .ForMember(dest => dest.AwayTeam, opt => 
                  opt.MapFrom(src => src.AwayTeam.Abrv))
              .ForMember(dest => dest.Season, opt => 
                  opt.MapFrom(src => src.Season.Year))
              .ForMember(dest => dest.HomeTeamScore, opt => 
                  opt.MapFrom(src => src.HomeTeam.Stats.FirstOrDefault(p => p.GameId == src.Id).Pts))
              .ForMember(dest => dest.AwayTeamScore, opt => 
                  opt.MapFrom(src => src.AwayTeam.Stats.FirstOrDefault(p => p.GameId == src.Id).Pts))
              .ForMember(dest => dest.HomePlayerGameStats, opt => 
                  opt.MapFrom(src => src.PlayerGameStats.Where(p => p.TeamId == src.HomeTeam.Id).ToList()))
              .ForMember(dest => dest.AwayPlayerGameStats, opt =>
                  opt.MapFrom(src => src.PlayerGameStats.Where(p => p.TeamId == src.AwayTeamId).ToList()));

            CreateMap<PlayerGameStats, PlayerGameStatsDto>();
            CreateMap<GameCompleteDto, Game>();
            CreateMap<CreateGameDto, GameCompleteDto>().ReverseMap();
            CreateMap<CreateGameDto, Game>().ReverseMap();

            CreateMap<CreatePlayerDto, Player>()
            .ForMember(dest => dest.Ratings, opt => 
                opt.MapFrom(src => src.Ratings));

            CreateMap<PlayerRatingDto, PlayerRatings>()
                .ForMember(dest => dest.PlayerId, opt =>
                    opt.MapFrom(src => src.PlayerId));

            CreateMap<PlayerRatings, PlayerRatingDto>()
                .ForMember(dest => dest.TeamAbrv, opt =>
                    opt.MapFrom(src => src.Player.Team.Abrv));

            
            CreateMap<PlayerRatings, PlayerRatings>();
            CreateMap<PlayerRatings, PlayerRatingDto>();

            CreateMap<PlayerRegularStats, PlayerRegularStatsDto>().ForMember(dest => dest.ImgUrl,
                opt => opt.MapFrom(p => p.Player.ImgUrl));

            CreateMap<PlayerPlayoffsStats, PlayerPlayoffsStatsDto>();

            CreateMap<TeamGameStats, TeamGameStatsDto>();
            CreateMap<TeamGameStats, TeamGameStatsSimple>();
            CreateMap<TeamPlayoffsStats, TeamPlayoffsStatsDto>().ReverseMap();

            CreateMap<TeamRegularStats, TeamRegularStatsDto>()
              .ForMember(dest => dest.TeamName, 
                  opt => opt.MapFrom(src => src.Team.Name))
              .ForMember(dest => dest.TeamRegion, 
                  opt => opt.MapFrom(src => src.Team.Region))
              .ForMember(dest => dest.TeamAbrv, 
                  opt => opt.MapFrom(src => src.Team.Abrv))
              .ForMember(dest => dest.TeamConference, 
                  opt => opt.MapFrom(src => src.Team.Conference))
;

            CreateMap<TeamRegularStatsDto, TeamRegularStats>().ReverseMap();
            CreateMap<TeamRegularStatsRankDto, TeamRegularStats>().ReverseMap();
            CreateMap<TeamRegularStats, TeamRegularStatsRankDto>()
                .ForMember(dest => dest.Conference,
                    opt =>
                        opt.MapFrom(t => t.Team.Conference));
            
            

            CreateMap<CreateSeasonDto, Season>().ReverseMap();
            CreateMap<Season, CompleteSeasonDto>().ReverseMap();


            CreateMap<Trade, TradeDto>()
               .ForMember(dest => dest.TeamOneName, opt => 
                   opt.MapFrom(src => src.TeamOne.Abrv))
               .ForMember(dest => dest.TeamTwoName, opt => 
                   opt.MapFrom(src => src.TeamTwo.Abrv));

            CreateMap<TradePlayer, TradePlayerDto>().ReverseMap();
            CreateMap<TradeCreateDto, Trade>().ReverseMap();


            CreateMap<TeamDraftPicks, TeamDraftPickDto>()
                  .ForMember(dest => dest.TeamName, opt =>
                      opt.MapFrom(src => src.Team.Abrv));
            CreateMap<TeamDraftPickDto, TeamDraftPicks>();

            CreateMap<TradePicks, TradePicksDto>().ReverseMap();

            CreateMap<FAOffer, FAOfferDto>()
                .ForMember(dest => dest.PlayerName, opt =>
                    opt.MapFrom(src => src.Player.Name));

            CreateMap<FAOfferDto, FAOffer>();

            CreateMap<PlayoffsDto, Playoffs>();
            CreateMap<Playoffs, PlayoffsDto>()
                .ForMember(dest => dest.teamOneAbrv, opt => 
                    opt.MapFrom(src => src.TeamOne.Abrv))
                .ForMember(dest => dest.teamOneName, opt =>
                    opt.MapFrom(src => src.TeamOne.Name))
                .ForMember(dest => dest.teamOneRegion, opt => 
                    opt.MapFrom(src => src.TeamOne.Region))
                .ForMember(dest => dest.teamTwoAbrv, opt =>
                    opt.MapFrom(src => src.TeamTwo.Abrv))
                .ForMember(dest => dest.teamTwoName, opt => 
                    opt.MapFrom(src => src.TeamTwo.Name))
                .ForMember(dest => dest.teamTwoRegion, opt => 
                    opt.MapFrom(src => src.TeamTwo.Region))
                .ForMember(dest => dest.GameCompletes, opt => 
                    opt.MapFrom(src => src.PlayoffGames.Select(pg => pg.Game)));

            CreateMap<DraftLottery, DraftLotteryDto>().ReverseMap();
            CreateMap<Draft, DraftDto>()
                .ForMember(dest => dest.PlayerName, opt => 
                    opt.MapFrom(src => src.Player.Name))
                .ForMember(dest => dest.TeamAbrv, opt => 
                    opt.MapFrom(src => src.Team.Abrv))
                .ForMember(dest => dest.TeamName, opt => 
                    opt.MapFrom(src => src.Team.Name));

            CreateMap<DraftDto, Draft>();


            CreateMap<News, NewsDto>().ReverseMap();


        }
    }
}
