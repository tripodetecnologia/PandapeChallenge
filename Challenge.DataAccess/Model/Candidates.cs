using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Challenge.DataAccess.Model
{
    [Table("candidates")]
    public class Candidates
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdCandidate { get; set; }

        [MaxLength(50)]
        [Column(TypeName = "varchar")]
        public string Name { get; set; }

        [MaxLength(150)]
        [Column(TypeName = "varchar")]
        public string Surname { get; set; }
                
        [Column(TypeName = "datetime")]
        public DateTime Birthdate { get; set; }

        [MaxLength(250)]
        [Column(TypeName = "varchar")]
        public string Email { get; set; }
                
        [Column(TypeName = "datetime")]
        public DateTime InsertDate { get; set; }
                
        [Column(TypeName = "datetime")]
        public DateTime? ModifyDate { get; set; }  
                
    }
}
