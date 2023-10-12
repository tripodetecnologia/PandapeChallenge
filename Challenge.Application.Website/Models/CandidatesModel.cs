using System.ComponentModel.DataAnnotations;

namespace Challenge.Application.Website.Models
{
    public class CandidatesModel
    {
        public int IdCandidate { get; set; }

        [Display(Name = "Nombre")]
        public string Name { get; set; }

        [Display(Name = "Apellido")]
        public string Surname { get; set; }

        [Display(Name = "Fecha Nacimiento")]
        public DateTime Birthdate { get; set; }

        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime? ModifyDate { get; set; }        

    }
}