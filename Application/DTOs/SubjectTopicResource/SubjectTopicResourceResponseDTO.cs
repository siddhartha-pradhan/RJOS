using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.SubjectTopicResource
{
    public class SubjectTopicResourceResponseDTO
    {
        public int Id { get; set; } 
        public int SubjectId { get; set; }

        public int ClassId { get; set; }

        public int TopicId { get; set; }

        public string Title { get; set; } = default!;

        public int ResourceType { get; set; }

        public string ResourceTypeAttachment { get; set; } = default!;
    }
}
