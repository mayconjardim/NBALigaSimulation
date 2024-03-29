﻿using AutoMapper;
using NBALigaSimulation.Shared.Dtos.FA;
using NBALigaSimulation.Shared.Models.FA;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Server.Services.FAService
{
    public class FAService : IFAService
    {

        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;

        public FAService(DataContext context, IMapper mapper, IAuthService authService)
        {
            _context = context;
            _mapper = mapper;
            _authService = authService;
        }

        public async Task<ServiceResponse<FAOfferDto>> CreateOffer(FAOfferDto offerDto)
        {
            var response = new ServiceResponse<FAOfferDto>();

            try
            {

                FAOffer offer = _mapper.Map<FAOffer>(offerDto);

                _context.FAOffers.Add(offer);
                await _context.SaveChangesAsync();
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
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            int? teamId = null;

            if (user != null)
            {
                teamId = user.TeamId;
            }

            List<FAOffer> offers = await _context.FAOffers
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
                var offer = await _context.FAOffers.FindAsync(offerId);

                if (offer == null)
                {
                    response.Success = false;
                    response.Message = "A oferta não foi encontrada.";
                    return response;
                }

                _context.FAOffers.Remove(offer);
                await _context.SaveChangesAsync();

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
