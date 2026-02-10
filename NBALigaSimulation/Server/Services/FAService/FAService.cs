using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NBALigaSimulation.Shared.Dtos.FA;
using NBALigaSimulation.Shared.Models.FA;
using NBALigaSimulation.Shared.Models.Users;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Server.Services.FAService
{
    public class FAService : IFAService
    {

        private readonly IGenericRepository<FAOffer> _offerRepository;
        private readonly IGenericRepository<User> _userRepository;
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;

        public FAService(
            IGenericRepository<FAOffer> offerRepository,
            IGenericRepository<User> userRepository,
            IMapper mapper,
            IAuthService authService)
        {
            _offerRepository = offerRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _authService = authService;
        }

        public async Task<ServiceResponse<FAOfferDto>> CreateOffer(FAOfferDto offerDto)
        {
            var response = new ServiceResponse<FAOfferDto>();

            try
            {

                FAOffer offer = _mapper.Map<FAOffer>(offerDto);

                await _offerRepository.AddAsync(offer);
                await _offerRepository.SaveChangesAsync();
                response.Success = true;
                response.Data = _mapper.Map<FAOfferDto>(offer);

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Ocorreu um erro ao criar uma oferta: " + ex.Message;
            }

            return response;
        }

        public async Task<ServiceResponse<List<FAOfferDto>>> GetOffersByTeamId()
        {
            var response = new ServiceResponse<List<FAOfferDto>>();

            var userId = _authService.GetUserId();
            var user = await _userRepository.Query()
                .FirstOrDefaultAsync(u => u.Id == userId);
            int? teamId = null;

            if (user != null)
            {
                teamId = user.TeamId;
            }

            List<FAOffer> offers = await _offerRepository.Query()
                .Include(p => p.Player)
                .OrderByDescending(o => o.DateCreated)
                .Where(o => o.TeamId == teamId)
                .ToListAsync();

            if (offers == null)
            {
                response.Success = false;
                response.Message = $"As ofertas com time de Id {teamId} não existem!";
            }
            else
            {
                response.Data = _mapper.Map<List<FAOfferDto>>(offers);
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteOffer(int offerId)
        {
            var response = new ServiceResponse<bool>();

            try
            {
                var offer = await _offerRepository.GetByIdAsync(offerId);

                if (offer == null)
                {
                    response.Success = false;
                    response.Message = "A oferta não foi encontrada.";
                    return response;
                }

                _offerRepository.Remove(offer);
                await _offerRepository.SaveChangesAsync();

                response.Success = true;
                response.Data = true;
                response.Message = "A oferta foi excluída com sucesso.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Ocorreu um erro ao excluir a oferta: " + ex.Message;
            }

            return response;
        }

    }
}
