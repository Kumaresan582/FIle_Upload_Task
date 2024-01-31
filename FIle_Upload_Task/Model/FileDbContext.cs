
using Microsoft.EntityFrameworkCore;

namespace FIle_Upload_Task.Model
{
    public class FileDbContext : DbContext
    {
        public FileDbContext(DbContextOptions<FileDbContext> options) : base(options)
        {

        }
        public DbSet<DetailsModel> File_Details { get; set; }
    }
}
