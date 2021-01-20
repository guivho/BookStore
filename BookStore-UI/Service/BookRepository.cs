using Blazored.LocalStorage;
using BookStore_UI.Contracts;
using BookStore_UI.Models;
using System.Net.Http;

namespace BookStore_UI.Service
{
    public class BookRepository : BaseRepository<Book>, IBookRepository
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILocalStorageService _localStorageService;

        public BookRepository(
            IHttpClientFactory httpClientFactory, 
            ILocalStorageService localStorageService)
            : base(httpClientFactory, localStorageService)
        {
            _httpClientFactory = httpClientFactory;
            _localStorageService = localStorageService;
        }
    }
}
