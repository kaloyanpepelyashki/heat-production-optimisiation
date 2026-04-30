using Rdm.Api.Application.Model;
using Rdm.Api.Inrastructure.DTOs;

namespace Rdm.Api.Application.Interfaces;

public interface IOptimiserService
{
     Task<OptimisationWrapperDto> RequestOptimisation(OptimisationRequestDto optimisationRequestDto);
}