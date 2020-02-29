using System;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace DatingApp.API.Helpers
{
    public static class Extensions
    {
        public static void AddApplicationError(this HttpResponse response, string message)
        {
            response.Headers.Add("Application-Error", message);
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            response.Headers.Add("Access-Control-Allow-OriGin", "*");
        }

        public static void AddPagination(this HttpResponse response,
            int currentPage, int itemsPerPage, int totalItems, int totalPages)
            {
                var paginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);
                response.Headers.Add("Pagination", JsonConvert.SerializeObject(paginationHeader));
                response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
            }

        public static int CalculateAge(this DateTime theDateTime)
        {
            var age = DateTime.Today.Year - theDateTime.Year;
            if(theDateTime.AddYears(age) > DateTime.Today)
            age--;

            return age;
        }
    }
}