namespace Rdm.Api.Application.Interfaces;

using Rdm.Api.Application.Model;
using Rdm.Api.Inrastructure.DTOs;

public interface IOptimiserService
{
     Task<OptimisationWrapperDto> RequestOptimisation(OptimisationRequestDto optimisationRequestDto);
}