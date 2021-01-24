using Blazored.LocalStorage;
using BookStore_UI.WASM.Contracts;
using BookStore_UI.WASM.Models;
using System.Net.Http;

namespace BookStore_UI.WASM.Service
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
