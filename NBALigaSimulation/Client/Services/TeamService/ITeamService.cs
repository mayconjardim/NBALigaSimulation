﻿namespace NBALigaSimulation.Client.Services.TeamService
{
    public interface ITeamService
    {

        Task<ServiceResponse<TeamCompleteDto>> GetTeamById(int teamId);

    }
}