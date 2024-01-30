using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.NewsAndAlert
{
    public class NewsAndAlerRequestDTO
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Header { get; set; } = null!;

        public string Description { get; set; } = null!;

        public DateTime ValidTill { get; set; }
    }
}
