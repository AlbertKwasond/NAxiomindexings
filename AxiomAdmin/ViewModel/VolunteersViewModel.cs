using DMS.Models;

namespace AxiomAdmin.ViewModel
{
    public class VolunteersViewModel
    {
        public Volunteers GetVolunteers { get; set; }

        public IEnumerable<Volunteers> ListVolunteers { get; set; }
    }
}