using AutoMapper;

namespace NBALigaSimulation.Server.Profiles
{
    public class AutoMapperProfile : Profile
    {

        public AutoMapperProfile()
        {
            CreateMap<Player, PlayerSimpleDto>();
        }
    }
}
