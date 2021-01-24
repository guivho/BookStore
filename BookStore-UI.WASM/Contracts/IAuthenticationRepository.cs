using BookStore_UI.WASM.Models;
using System.Threading.Tasks;

namespace BookStore_UI.WASM.Contracts
{
    public interface IAuthenticationRepository
    {
        public Task<bool> Register(RegistrationModel user);
        public Task<bool> Login(LoginModel user);
        public Task Logout();

    }
}
