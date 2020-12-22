using APIRest_ASPNET5.Configurations;
using APIRest_ASPNET5.Data.VO;
using APIRest_ASPNET5.Repository;
using APIRest_ASPNET5.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace APIRest_ASPNET5.Business.Implementations
{
    public class LoginBusinessImplementation : ILoginBusiness
    {
        private const string DATE_FORMAT = "dd/MM/yyyy hh:mm:ss";
        private TokenConfiguration _configuration;

        private IEmployeeRepository _repository;
        private readonly ITokenService _tokenService;

        public LoginBusinessImplementation(TokenConfiguration configuration, IEmployeeRepository repository, ITokenService tokenService)
        {
            _configuration = configuration;
            _repository = repository;
            _tokenService = tokenService;
        }

        public TokenVO ValidateCredentials(EmployeeVO employeeCredentials)
        {
            var employee = _repository.ValidateCredentials(employeeCredentials);
            if (employee == null) return null;
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new Claim(JwtRegisteredClaimNames.UniqueName, employee.Username)
            };

            var accessToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();

            employee.RefreshToken = refreshToken;
            employee.RefreshTokenExpiryTime = DateTime.Now.AddDays(_configuration.DaysToExpire);

            _repository.RefreshEmployeeInfo(employee);

            DateTime createDate = DateTime.Now;
            DateTime expirationDate = createDate.AddMinutes(_configuration.Minutes);

            return new TokenVO(
                true,
                createDate.ToString(DATE_FORMAT),
                expirationDate.ToString(DATE_FORMAT),
                accessToken,
                refreshToken
            );
        }

        public TokenVO ValidateCredentials(TokenVO token)
        {
            var accessToken = token.AccessToken;
            var refreshToken = token.RefreshToken;

            var principal = _tokenService.GetPrincipalFromExpiredTokens(accessToken);
            var username = principal.Identity.Name;
            var employee = _repository.ValidateCredentials(username);

            if (employee == null || 
                employee.RefreshToken != refreshToken || 
                employee.RefreshTokenExpiryTime <= DateTime.Now) return null;

            accessToken = _tokenService.GenerateAccessToken(principal.Claims);
            refreshToken = _tokenService.GenerateRefreshToken();

            employee.RefreshToken = refreshToken;

            _repository.RefreshEmployeeInfo(employee);

            DateTime createDate = DateTime.Now;
            DateTime expirationDate = createDate.AddMinutes(_configuration.Minutes);

            return new TokenVO(
                true,
                createDate.ToString(DATE_FORMAT),
                expirationDate.ToString(DATE_FORMAT),
                accessToken,
                refreshToken
            );
        }

        public bool RevokeToken(string Username)
        {
            return _repository.RevokeToken(Username);
        }
    }
}