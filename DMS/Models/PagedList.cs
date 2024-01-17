using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.Models
{
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