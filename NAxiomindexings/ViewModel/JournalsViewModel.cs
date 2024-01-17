using DMS.Models;
using X.PagedList;

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
		public int PageNumber { get; internal set; }
		public int TotalPages { get; internal set; }

		public class PagedList<T>
		{
			public List<T> Items { get; }
			public int PageNumber { get; }
			public int PageSize { get; }
			public int TotalItems { get; }
			public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);

			public PagedList(List<T> items, int pageNumber, int pageSize, int totalItems)
			{
				PageNumber = pageNumber;
				PageSize = pageSize;
				TotalItems = totalItems;
				Items = items;
			}

			public static PagedList<T> Create(List<T> source, int pageNumber, int pageSize)
			{
				var totalItems = source.Count;
				var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
				return new PagedList<T>(items, pageNumber, pageSize, totalItems);
			}
		}
	}
}