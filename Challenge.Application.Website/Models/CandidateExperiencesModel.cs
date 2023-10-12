using System.ComponentModel.DataAnnotations;

namespace Challenge.Application.Website.Models
{
    public class CandidateExperiencesModel
    {
        public int IdCandidateExperience { get; set; }
        public int IdCandidate { get; set; }

        [Display(Name = "Empresa")]
        public string Company { get; set; }

        [Display(Name = "Cargo")]
        public string Job { get; set; }

        [Display(Name = "Descripción")]
        public string Description { get; set; }

        [Display(Name = "Salario")]
        public decimal Salary { get; set; }

        [Display(Name = "Fecha Ingreso")]
        public DateTime? BeginDate { get; set; }

        [Display(Name = "Fecha Retiro")]
        public DateTime? EndDate { get; set; }

        public DateTime InsertDate { get; set; }
        public DateTime? ModifyDate { get; set; }

        public CandidatesModel Candidate { get; set; }

    }
}