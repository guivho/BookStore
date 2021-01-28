using Blazored.LocalStorage;
using BookStore_UI.WASM.Contracts;
using BookStore_UI.WASM.Models;
using System.Net.Http;

namespace BookStore_UI.WASM.Service
{
    public class BookRepository : BaseRepository<Book>, IBookRepository
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorageService;

        public BookRepository(
            HttpClient httpClient, 
            ILocalStorageService localStorageService)
            : base(httpClient, localStorageService)
        {
            _httpClient = httpClient;
            _localStorageService = localStorageService;
        }
    }
}
