using FIle_Upload_Task.Model;

namespace FIle_Upload_Task.Service
{
    public interface IManageImage
    {
        Task<DetailsModel> UploadFile(IFormFile Files);
        public  Task<DetailsModel> UploadFileWithOptionalWatermark(IFormFile Files, bool addWatermark);
    }
}
