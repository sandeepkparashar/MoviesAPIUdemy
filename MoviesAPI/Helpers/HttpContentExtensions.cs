using Microsoft.EntityFrameworkCore;

namespace MoviesAPI.Helpers
{
    public static class HttpContentExtensions
    {
        public async static Task InsertPaginationHeaderInResponse<T>(this HttpContext httpContext, 
            IQueryable<T> queryable, int recordsPerPage)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            double count= await queryable.CountAsync();
            double totalNoOfPages=Math.Ceiling(count/recordsPerPage);
            httpContext.Response.Headers.Add("totalNoOfPages",totalNoOfPages.ToString());
        }
    }
}
