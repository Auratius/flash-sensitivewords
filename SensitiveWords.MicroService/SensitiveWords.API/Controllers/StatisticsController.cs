using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Flash.SensitiveWords.Domain.Interfaces;

namespace SensitiveWords.API.Controllers;

/// <summary>
/// Controller for operation statistics
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Tags("Statistics")]
public class StatisticsController : ControllerBase
{
    private readonly IOperationStatsRepository _statsRepository;
    private readonly ILogger<StatisticsController> _logger;

    public StatisticsController(
        IOperationStatsRepository statsRepository,
        ILogger<StatisticsController> logger)
    {
        _statsRepository = statsRepository ?? throw new ArgumentNullException(nameof(statsRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets all operation statistics
    /// </summary>
    /// <returns>List of operation statistics</returns>
    /// <response code="200">Returns the operation statistics</response>
    /// <response code="500">If an error occurs while retrieving statistics</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllStatistics(CancellationToken cancellationToken)
    {
        try
        {
            var stats = await _statsRepository.GetAllAsync(cancellationToken);
            _logger.LogInformation("Retrieved {Count} operation statistics", stats.Count());
            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving operation statistics");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "An error occurred while retrieving statistics" });
        }
    }

    /// <summary>
    /// Gets statistics for a specific operation type
    /// </summary>
    /// <param name="operationType">The operation type (CREATE, READ, UPDATE, DELETE, SANITIZE)</param>
    /// <returns>Statistics for the specified operation type</returns>
    /// <response code="200">Returns the operation statistics</response>
    /// <response code="400">If operation type is invalid</response>
    /// <response code="500">If an error occurs while retrieving statistics</response>
    [HttpGet("{operationType}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetStatisticsByType(
        string operationType,
        CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(operationType))
            {
                return BadRequest(new { message = "Operation type cannot be empty" });
            }

            var stats = await _statsRepository.GetByTypeAsync(operationType.ToUpperInvariant(), cancellationToken);
            _logger.LogInformation("Retrieved statistics for operation type: {OperationType}", operationType);
            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving statistics for operation type: {OperationType}", operationType);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = $"An error occurred while retrieving statistics for operation type '{operationType}'" });
        }
    }

    /// <summary>
    /// Resets all operation statistics to zero
    /// </summary>
    /// <returns>Success message</returns>
    /// <response code="200">Statistics reset successfully</response>
    /// <response code="500">If an error occurs while resetting statistics</response>
    [HttpPost("reset")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ResetStatistics(CancellationToken cancellationToken)
    {
        try
        {
            await _statsRepository.ResetAsync(cancellationToken);
            _logger.LogWarning("Operation statistics have been reset");
            return Ok(new { message = "All operation statistics have been reset to zero" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting operation statistics");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "An error occurred while resetting statistics" });
        }
    }
}
