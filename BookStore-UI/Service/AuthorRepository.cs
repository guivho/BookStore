using BookStore_UI.Contracts;
using BookStore_UI.Models;
using System.Net.Http;

namespace BookStore_UI.Service
{
    public class AuthorRepository : BaseRepository<Author>, IAuthorRepository
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthorRepository(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
    }
}
