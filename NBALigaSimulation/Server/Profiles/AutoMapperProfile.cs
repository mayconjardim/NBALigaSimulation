using AutoMapper;
using Microsoft.Extensions.Configuration;

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
              .ForMember(dest => dest.Season, opt => opt.MapFrom(src => src.Season.Year))
              .ForMember(dest => dest.HomeTeamScore, opt => opt.MapFrom(src => src.HomeTeam.Stats.FirstOrDefault(p => p.GameId == src.Id).Pts))
              .ForMember(dest => dest.AwayTeamScore, opt => opt.MapFrom(src => src.AwayTeam.Stats.FirstOrDefault(p => p.GameId == src.Id).Pts))
              .ForMember(dest => dest.HomePlayerGameStats, opt => opt.MapFrom(src => src.PlayerGameStats.Where(p => p.TeamId == src.HomeTeam.Id).ToList()))
              .ForMember(dest => dest.AwayPlayerGameStats, opt => opt.MapFrom(src => src.PlayerGameStats.Where(p => p.TeamId == src.AwayTeamId).ToList()));
              


            CreateMap<PlayerGameStats, PlayerGameStatsDto>();

            CreateMap<GameCompleteDto, Game>();
            CreateMap<CreateGameDto, GameCompleteDto>().ReverseMap();
            CreateMap<CreateGameDto, Game>().ReverseMap();

            CreateMap<TeamGameStats, TeamGameStatsDto>();

            CreateMap<CreateSeasonDto, Season>().ReverseMap();
            CreateMap<Season, CompleteSeasonDto>().ReverseMap();

        }
    }
}
