using AutoMapper;

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

            CreateMap<PlayerCompleteDto, Player>();

            CreateMap<PlayerContract, PlayerContractDto>().ReverseMap();

            CreateMap<PlayerAwards, PlayerAwardsDto>().ReverseMap();

            CreateMap<Team, TeamSimpleDto>();
            CreateMap<Team, TeamSimpleWithPlayersDto>().ReverseMap();
            CreateMap<Team, TeamCompleteDto>().ReverseMap();
            CreateMap<TeamGameplan, TeamGameplanDto>().ReverseMap();


            CreateMap<Game, GameCompleteDto>()
              .ForMember(dest => dest.HomeTeam, opt => opt.MapFrom(src => src.HomeTeam.Abrv))
              .ForMember(dest => dest.AwayTeam, opt => opt.MapFrom(src => src.AwayTeam.Abrv))
              .ForMember(dest => dest.Season, opt => opt.MapFrom(src => src.Season.Year))
              .ForMember(dest => dest.HomeTeamScore, opt => opt.MapFrom(src => src.HomeTeam.Stats.FirstOrDefault(p => p.GameId == src.Id).Pts))
              .ForMember(dest => dest.AwayTeamScore, opt => opt.MapFrom(src => src.AwayTeam.Stats.FirstOrDefault(p => p.GameId == src.Id).Pts))
              .ForMember(dest => dest.HomePlayerGameStats, opt => opt.MapFrom(src => src.PlayerGameStats.Where(p => p.TeamId == src.HomeTeam.Id).ToList()))
              .ForMember(dest => dest.AwayPlayerGameStats, opt => opt.MapFrom(src => src.PlayerGameStats.Where(p => p.TeamId == src.AwayTeamId).ToList()));

            CreateMap<PlayerGameStats, PlayerGameStatsDto>();


            CreateMap<GameCompleteDto, Game>();
            CreateMap<CreateGameDto, GameCompleteDto>().ReverseMap();
            CreateMap<CreateGameDto, Game>().ReverseMap();

            CreateMap<CreatePlayerDto, Player>()
            .ForMember(dest => dest.Ratings, opt => opt.MapFrom(src => src.Ratings));

            CreateMap<PlayerRatingDto, PlayerRatings>()
                .ForMember(dest => dest.PlayerId, opt => opt.MapFrom(src => src.PlayerId));

            CreateMap<PlayerRatings, PlayerRatings>();
            CreateMap<PlayerRatings, PlayerRatingDto>();

            CreateMap<PlayerRegularStats, PlayerRegularStatsDto>().ReverseMap();
            CreateMap<PlayerPlayoffsStats, PlayerPlayoffsStatsDto>().ReverseMap();

            CreateMap<TeamGameStats, TeamGameStatsDto>();
            CreateMap<TeamPlayoffsStats, TeamPlayoffsStatsDto>().ReverseMap();


            CreateMap<TeamRegularStats, TeamRegularStatsDto>()
              .ForMember(dest => dest.TeamName, opt => opt.MapFrom(src => src.Team.Name))
              .ForMember(dest => dest.TeamRegion, opt => opt.MapFrom(src => src.Team.Region))
              .ForMember(dest => dest.TeamAbrv, opt => opt.MapFrom(src => src.Team.Abrv))
              .ForMember(dest => dest.TeamConference, opt => opt.MapFrom(src => src.Team.Conference))
;

            CreateMap<TeamRegularStatsDto, TeamRegularStats>().ReverseMap();
            CreateMap<TeamRegularStatsRankDto, TeamRegularStats>().ReverseMap();



            CreateMap<CreateSeasonDto, Season>().ReverseMap();
            CreateMap<Season, CompleteSeasonDto>().ReverseMap();


            CreateMap<Trade, TradeDto>()
               .ForMember(dest => dest.TeamOneName, opt => opt.MapFrom(src => src.TeamOne.Abrv))
               .ForMember(dest => dest.TeamTwoName, opt => opt.MapFrom(src => src.TeamTwo.Abrv));

            CreateMap<TradePlayer, TradePlayerDto>().ReverseMap();
            CreateMap<TradeCreateDto, Trade>().ReverseMap();


            CreateMap<TeamDraftPicks, TeamDraftPickDto>()
                  .ForMember(dest => dest.TeamName, opt => opt.MapFrom(src => src.Team.Abrv));
            CreateMap<TeamDraftPickDto, TeamDraftPicks>();

            CreateMap<TradePicks, TradePicksDto>().ReverseMap();

            CreateMap<FAOffer, FAOfferDto>()
                .ForMember(dest => dest.PlayerName, opt => opt.MapFrom(src => src.Player.Name));

            CreateMap<FAOfferDto, FAOffer>();

            CreateMap<PlayoffsDto, Playoffs>();
            CreateMap<Playoffs, PlayoffsDto>()
                .ForMember(dest => dest.teamOneAbrv, opt => opt.MapFrom(src => src.TeamOne.Abrv))
                .ForMember(dest => dest.teamOneName, opt => opt.MapFrom(src => src.TeamOne.Name))
                .ForMember(dest => dest.teamOneRegion, opt => opt.MapFrom(src => src.TeamOne.Region))
                .ForMember(dest => dest.teamTwoAbrv, opt => opt.MapFrom(src => src.TeamTwo.Abrv))
                .ForMember(dest => dest.teamTwoName, opt => opt.MapFrom(src => src.TeamTwo.Name))
                .ForMember(dest => dest.teamTwoRegion, opt => opt.MapFrom(src => src.TeamTwo.Region))
                .ForMember(dest => dest.GameCompletes, opt => opt.MapFrom(src => src.PlayoffGames.Select(pg => pg.Game)));

            CreateMap<DraftLottery, DraftLotteryDto>().ReverseMap();
            CreateMap<Draft, DraftDto>().ReverseMap();


        }
    }
}
