using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Subject
{
    public class SubjectResponseDTO
    {
        public int Id { get; set; }
        
        public int ClassId { get; set; }

        public int LanguageId { get; set; }

        public string Name { get; set; } = default!;

        public int Sequence { get; set; }
    }
}
