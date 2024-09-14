using MewingPad.Services.TagService;
using MewingPad.DTOs;
using MewingPad.DTOs.Converters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace MewingPad.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagsController(ITagService tagService) : ControllerBase
    {
        private readonly ITagService _tagService = tagService;
        private readonly Serilog.ILogger _logger = Log.ForContext<TagsController>();

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var tags = from tag in await _tagService.GetAllTags()
                           select TagConverter.CoreModelToDto(tag);
                return Ok(tags);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception thrown {Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddTag([FromBody] TagDto tagDto)
        {
            try
            {   
                _logger.Information("Received {@Tag}", tagDto);
                var tag = TagConverter.DtoToCoreModel(tagDto);
                tag.Id = Guid.NewGuid();
                await _tagService.CreateTag(tag);
                return Ok("Tag added successfully");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception thrown {Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateTag([FromBody] TagDto tagDto)
        {
            try
            {   
                _logger.Information("Received {@Tag}", tagDto);
                var tag = TagConverter.DtoToCoreModel(tagDto);
                await _tagService.UpdateTagName(tag.Id, tag.Name);
                return Ok("Tag updated successfully");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception thrown {Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    
        [Authorize]
        [HttpDelete("{tagId:guid}")]
        public async Task<IActionResult> DeleteTag(Guid tagId)
        {
            try
            {
                _logger.Information("Received {@TagId}", tagId);
                await _tagService.DeleteTag(tagId);
                return Ok("Tag deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception thrown {Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}