using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Flash.SensitiveWords.Application.Commands;
using Flash.SensitiveWords.Application.DTOs;
using Flash.SensitiveWords.Application.Handlers;
using Flash.SensitiveWords.Application.Queries;

namespace Flash.SensitiveWords.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class SensitiveWordsController : ControllerBase
    {
        private readonly GetAllSensitiveWordsHandler _getAllHandler;
        private readonly GetSensitiveWordByIdHandler _getByIdHandler;
        private readonly CreateSensitiveWordHandler _createHandler;
        private readonly UpdateSensitiveWordHandler _updateHandler;
        private readonly DeleteSensitiveWordHandler _deleteHandler;

        public SensitiveWordsController(
            GetAllSensitiveWordsHandler getAllHandler,
            GetSensitiveWordByIdHandler getByIdHandler,
            CreateSensitiveWordHandler createHandler,
            UpdateSensitiveWordHandler updateHandler,
            DeleteSensitiveWordHandler deleteHandler)
        {
            _getAllHandler = getAllHandler ?? throw new ArgumentNullException(nameof(getAllHandler));
            _getByIdHandler = getByIdHandler ?? throw new ArgumentNullException(nameof(getByIdHandler));
            _createHandler = createHandler ?? throw new ArgumentNullException(nameof(createHandler));
            _updateHandler = updateHandler ?? throw new ArgumentNullException(nameof(updateHandler));
            _deleteHandler = deleteHandler ?? throw new ArgumentNullException(nameof(deleteHandler));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SensitiveWordDto>), 200)]
        public async Task<IActionResult> GetAll([FromQuery] bool? activeOnly = null)
        {
            try
            {
                var query = new GetAllSensitiveWordsQuery(activeOnly);
                var result = await _getAllHandler.HandleAsync(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SensitiveWordDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var query = new GetSensitiveWordByIdQuery(id);
                var result = await _getByIdHandler.HandleAsync(query);

                if (result == null)
                    return NotFound(new { error = $"Word with ID '{id}' not found" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(Guid), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] CreateSensitiveWordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var command = new CreateSensitiveWordCommand(request.Word);
                var id = await _createHandler.HandleAsync(command);
                return CreatedAtAction(nameof(GetById), new { id }, new { id });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSensitiveWordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var command = new UpdateSensitiveWordCommand(id, request.Word, request.IsActive);
                var success = await _updateHandler.HandleAsync(command);

                if (!success)
                    return NotFound(new { error = $"Word with ID '{id}' not found" });

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var command = new DeleteSensitiveWordCommand(id);
                var success = await _deleteHandler.HandleAsync(command);

                if (!success)
                    return NotFound(new { error = $"Word with ID '{id}' not found" });

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
