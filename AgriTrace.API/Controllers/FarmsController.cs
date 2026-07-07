using System;
using System.Threading.Tasks;
using AgriTrace.API.Models;
using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Application.Contracts;
using AgriTrace.Application.Features.Farms.Commands;
using AgriTrace.Application.Features.Farms.Queries;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AgriTrace.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FarmsController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public FarmsController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }

        // Tạo mới một nông trại (phía "1").
        [HttpPost]
        public async Task<ActionResult<FarmResponse>> CreateFarm([FromBody] FarmRequest request)
        {
            var command = _mapper.Map<CreateFarmCommand>(request);
            var resultDto = await _sender.Send(command);
            var response = _mapper.Map<FarmResponse>(resultDto);

            return CreatedAtAction(nameof(GetFarmById), new { id = response.Id }, response);
        }

        // Lấy danh sách nông trại theo trang (phân trang).
        [HttpGet]
        public async Task<ActionResult<PaginationResponse<FarmDto>>> GetFarms([FromQuery] PaginationRequest request)
        {
            var query = _mapper.Map<GetFarmsQuery>(request);
            return Ok(await _sender.Send(query));
        }

        // Lấy một nông trại kèm toàn bộ mùa vụ (quan hệ 1 - N).
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<FarmResponse>> GetFarmById(Guid id)
        {
            var query = _mapper.Map<GetFarmByIdQuery>(id);
            var resultDto = await _sender.Send(query);

            if (resultDto == null)
            {
                throw new NotFoundException($"Farm '{id}' not found.");
            }

            return Ok(_mapper.Map<FarmResponse>(resultDto));
        }
    }
}
