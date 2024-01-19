using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.SubjectTopic
{
    public class SubjectTopicRequestDTO
    {
        public int SubjectId { get; set; }

        public int ClassId { get; set; }

        public string Name { get; set; } = default!;

        public string Description { get; set; } = default!;
    }
}
