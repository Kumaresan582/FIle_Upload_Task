using FIle_Upload_Task.Model;
using FIle_Upload_Task.Service;
using Microsoft.AspNetCore.Mvc;

namespace FIle_Upload_Task.Controllers
{
    [ApiController]
    public class FileManagerController : ControllerBase
    {
        private readonly IManageImage _iManageImage;

        public FileManagerController(IManageImage iManageImage)
        {
            _iManageImage = iManageImage;
        }

        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> UploadFile(IFormFile Files)
        {
            var result = await _iManageImage.UploadFile(Files);
            return Ok(result);
        }

        [HttpPost]
        [Route("uploadfile")]
        public async Task<IActionResult> UploadFile(IFormFile file, bool addWatermark)
        {
            try
            {
                DetailsModel fileDetails = await _iManageImage.UploadFileWithOptionalWatermark(file, addWatermark);

                return Ok(fileDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}