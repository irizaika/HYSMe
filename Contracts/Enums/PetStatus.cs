using System.ComponentModel.DataAnnotations;

namespace Contracts.Enums
{
    public enum PetStatus
    {
        [Display(Name = "Lost")]
        Lost = 0,

        [Display(Name = "Reunited")]
        Reunited = 1,

        [Display(Name = "Deceased")]
        Deceased = 2
    }
}
