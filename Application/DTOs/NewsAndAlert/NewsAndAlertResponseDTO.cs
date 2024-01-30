using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.NewsAndAlert
{
    public class NewsAndAlertResponseDTO
    {
        public int Id { get; set; }

        public string Header { get; set; } = null!;

        public string Description { get; set; } = null!;

        public DateTime ValidTill { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedOn { get; set; }

    }
}
