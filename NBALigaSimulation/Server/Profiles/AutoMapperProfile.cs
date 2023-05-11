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

            CreateMap<Game, GameCompleteDto>().ReverseMap();
            CreateMap<CreateGameDto, GameCompleteDto>().ReverseMap();
            CreateMap<CreateGameDto, Game>().ReverseMap();
        }
    }
}
