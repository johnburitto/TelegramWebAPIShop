using FileServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace FileServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IFileService _service;

        public FileController(IFileService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpPost]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> SaveAync(IFormFile file)
        {
            return Ok(await _service.SaveAsync(file) ? "File is saved" : "File doesn't saved");
        }

        [HttpPost("range")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> SaveRangeAync(List<IFormFile> files)
        {
            return Ok(await _service.SaveRangeAsync(files) ? "All files are saved" : "File doesn't saved");
        }
    }
}
