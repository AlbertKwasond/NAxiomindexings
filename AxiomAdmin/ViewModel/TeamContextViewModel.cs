using DMS.Models;

namespace AxiomAdmin.ViewModel
{
    public class TeamContextViewModel
    {
        public TeamContext TeamContexts { get; set; }
        public IEnumerable<TeamContext> ListTeamContexts { get; set; }
    }
}