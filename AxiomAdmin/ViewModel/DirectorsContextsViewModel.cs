using DMS.Models;

namespace AxiomAdmin.ViewModel
{
    public class DirectorsContextsViewModel
    {
        public BoardOfDirectorsContext BoardOfDirectors { get; set; }
        public IEnumerable<BoardOfDirectorsContext> ListBoardOfDirectors { get; set; }
    }
}