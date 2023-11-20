using DMS.Models;

namespace NAxiomindexings.ViewModel
{
	public class JournalsViewModel
	{
		public Author GetAuthors { get; set; }
		public Journals GetJournals { get; set; }
		public Category GetCategories { get; set; }

		public IEnumerable<Author> ListAuthors { get; set; }
		public IEnumerable<Journals> ListJournals { get; set; }
		public IEnumerable<Category> ListCategories { get; set; }
	}
}