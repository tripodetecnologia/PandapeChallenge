using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Challenge.DataAccess.Model
{
    [Table("candidateexperiences")]
    public class CandidateExperiences
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdCandidateExperience { get; set; }

        public int IdCandidate { get; set; }

        [MaxLength(100)]
        [Column(TypeName = "varchar")]
        public string Company { get; set; }

        [MaxLength(100)]
        [Column(TypeName = "varchar")]
        public string Job { get; set; }

        [MaxLength(4000)]
        [Column(TypeName = "varchar")]
        public string Description { get; set; }

        [Precision(8, 2)]
        [Column(TypeName = "numeric")]
        public decimal Salary { get; set; }
                
        [Column(TypeName = "datetime")]
        public DateTime BeginDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? EndDate { get; set; }
                
        [Column(TypeName = "datetime")]
        public DateTime InsertDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ModifyDate { get; set; }

        [ForeignKey("IdCandidate")]
        public virtual Candidates Candidate { get; set; }
    }
}
