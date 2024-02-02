using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.StudentVideoTracking
{
    public class StudentVideoTrackingResponseDTO
    {
        public int Id { get; set; }

        public int SubjectId { get; set; }

        public int VideoId { get; set; }

        public int StudentId { get; set; }

        public int Class { get; set; }

        public int? PlayTimeInSeconds { get; set; }

        public decimal? PercentageCompleted { get; set; }

        public int? VideoDurationInSeconds { get; set; }
    }
}
