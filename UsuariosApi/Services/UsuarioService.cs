using AutoMapper;
using Microsoft.AspNetCore.Identity;
using UsuariosApi.Data.Dtos;
using UsuariosApi.Models;
using UsuariosAPI.Data.Dtos;

namespace UsuariosApi.Services
{
    public class UsuarioService
    {
        private IMapper _mapper;
        private UserManager<Usuario> _userManager;
        private SignInManager<Usuario> _singImManager;
        private TokenService _tokenService;

        public UsuarioService(UserManager<Usuario> userManager, IMapper mapper, SignInManager<Usuario> singImManager, TokenService tokenService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _singImManager = singImManager;
            _tokenService = tokenService;
        }

        public async Task CadastraUsuario(CreateUsuarioDto dto)
        {
            Usuario usuario = _mapper.Map<Usuario>(dto);

            IdentityResult resultado = await _userManager.CreateAsync(usuario, dto.Password);

            if (!resultado.Succeeded)
            {
                throw new ApplicationException("Falha ao cadastrar usuário!");
            }
            
        }

        public async Task<string> Login(LoginUsuarioDto dto)
        {
           var resultado = await _singImManager.PasswordSignInAsync(dto.Username, dto.Password, false, false);
        
           if (!resultado.Succeeded)
            {
                throw new ApplicationException("Usuário não autenticado!");
            }

            var usuario = _singImManager
                 .UserManager
                 .Users.
                 FirstOrDefault(user => user.NormalizedUserName == dto.Username.ToUpper());

            var token = _tokenService.GenerateToken(usuario);


            return token;
        }
    }
}
