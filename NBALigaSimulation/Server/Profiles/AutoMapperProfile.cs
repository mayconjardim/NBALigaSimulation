using AutoMapper;

namespace NBALigaSimulation.Server.Profiles
{
    public class AutoMapperProfile : Profile
    {

        public AutoMapperProfile()
        {
            CreateMap<Player, PlayerSimpleDto>();
            CreateMap<Player, PlayerCompleteDto>()
                .ForMember(dest => dest.TeamName, opt => opt.MapFrom(src => src.Team.Name))
                .ForMember(dest => dest.TeamAbrv, opt => opt.MapFrom(src => src.Team.Abrv));

            CreateMap<PlayerCompleteDto, Player>();

            CreateMap<PlayerRatings, PlayerRatingDto>();

            CreateMap<Team, TeamSimpleDto>();
            CreateMap<Team, TeamCompleteDto>().ReverseMap();

            CreateMap<Game, GameCompleteDto>()
                 .ForMember(dest => dest.HomeTeam, opt => opt.MapFrom(src => src.HomeTeam.Abrv))
                 .ForMember(dest => dest.AwayTeam, opt => opt.MapFrom(src => src.AwayTeam.Abrv))
                 .ForMember(dest => dest.HomeTeamScore, opt => opt.MapFrom(src => src.HomeTeam.Stats.FirstOrDefault(p => p.GameId == src.Id).Pts))
                 .ForMember(dest => dest.AwayTeamScore, opt => opt.MapFrom(src => src.AwayTeam.Stats.FirstOrDefault(p => p.GameId == src.Id).Pts));

            CreateMap<GameCompleteDto, Game>();
            CreateMap<CreateGameDto, GameCompleteDto>().ReverseMap();
            CreateMap<CreateGameDto, Game>().ReverseMap();

            CreateMap<TeamGameStats, TeamGameStatsDto>();

        }
    }
}
