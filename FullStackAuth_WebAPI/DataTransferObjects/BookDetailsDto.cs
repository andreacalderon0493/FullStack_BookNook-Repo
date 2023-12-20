using System;
namespace FullStackAuth_WebAPI.DataTransferObjects
{
	public class BookDetailsDto
	{
		public ICollection <ReviewWithUserDto> Reviews { get; set; }
		public double Average { get; set; }
		public bool IsFavorited { get; set; }
		public string BookId { get; set; }

		
	}
}

