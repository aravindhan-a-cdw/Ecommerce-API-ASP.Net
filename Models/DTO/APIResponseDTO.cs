using System;
namespace EcommerceAPI.Models.DTO
{
	public class APIResponseDTO
	{

		public APIResponseDTO(int statusCode, string message)
		{
			StatusCode = statusCode;
			Message = message;
		}

		public int StatusCode { get; set; }
		public string Message { get; set; }
	}
}

