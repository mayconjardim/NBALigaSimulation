using AutoMapper;

namespace NBALigaSimulation.Server.Services.DraftService
{
	public class DraftService : IDraftService
	{

		private readonly DataContext _context;
		private readonly IMapper _mapper;
		private readonly IDraftService _draftService;

		public DraftService(DataContext context, IMapper mapper, IDraftService draftService)
		{
			_context = context;
			_mapper = mapper;
			_draftService = draftService;
		}




	}
}
