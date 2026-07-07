using System.Threading.Tasks;
using AgriTrace.API.Models;
using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Application.Features.Crops.Commands;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AgriTrace.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CropsController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public CropsController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }

        // Tạo mới một mùa vụ (phía "N"). Nông trại đích được chỉ định qua FarmId trong body.
        [HttpPost]
        public async Task<ActionResult<CropResponse>> CreateCrop([FromBody] CropRequest request)
        {
            var command = _mapper.Map<CreateCropCommand>(request);
            var resultDto = await _sender.Send(command);

            if (resultDto == null)
            {
                throw new NotFoundException($"Farm '{request.FarmId}' not found.");
            }

            return CreatedAtAction(nameof(CreateCrop), _mapper.Map<CropResponse>(resultDto));
        }
    }
}
