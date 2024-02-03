using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EfectivoWork.Entities;

    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        public int TaskId { get; set; }
        public string Notes { get; set; }
        public DateTime RecordStamp { get; set; }

        public Task Task { get; set; } // Właściwość nawigacyjna
    }

