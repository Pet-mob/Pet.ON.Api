using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;

// DTOs
public record SendEmailRequest(string Telefone);
public record ValidateCodeRequest(string Email, string Codigo);
public record ResetPasswordRequest(string Email, string Codigo, string NovaSenha);

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMemoryCache _cache;
    private readonly IDbConnection _dbConnection;
    private const int CodigoExpiraEmMinutos = 5;

    public AuthController(IMemoryCache cache, IDbConnection dbConnection)
    {
        _cache = cache;
        _dbConnection = dbConnection;
    }

    /// <summary>
    /// Envia um código de verificação para o e-mail associado ao telefone informado.
    /// </summary>
    [HttpPost("enviar-codigo")]
    public async Task<IActionResult> EnviarCodigoPorEmail([FromBody] SendEmailRequest request)
    {
        var sql = @"SELECT email FROM usuario WHERE telefone = @Telefone";
        var emailUsuario = await _dbConnection.QueryFirstOrDefaultAsync<string>(sql, new { Telefone = request.Telefone });

        if (string.IsNullOrEmpty(emailUsuario))
        {
            return Ok(new { sucesso = false, mensagem = "Usuário não encontrado para o telefone informado." });
        }

        var codigo = Random.Shared.Next(100000, 999999).ToString();
        _cache.Set(emailUsuario.ToLower(), codigo, TimeSpan.FromMinutes(CodigoExpiraEmMinutos));

        try
        {
            await EnviarEmailAsync(emailUsuario, codigo);
            return Ok(new { sucesso = true, mensagem = "Código enviado para o e-mail cadastrado.", email = emailUsuario });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { sucesso = false, mensagem = "Erro ao enviar e-mail.", erro = ex.Message });
        }
    }

    /// <summary>
    /// Valida o código recebido no e-mail.
    /// </summary>
    [HttpPost("validar-codigo")]
    public IActionResult ValidarCodigo([FromBody] ValidateCodeRequest request)
    {
        var email = request.Email.Trim().ToLower();

        if (_cache.TryGetValue(email, out string codigoEmCache) && codigoEmCache == request.Codigo)
        {
            return Ok(new { valido = true });
        }

        return Ok(new { valido = false, mensagem = "Código inválido ou expirado." });
    }

    /// <summary>
    /// Redefine a senha do usuário após a validação do código.
    /// </summary>
    [HttpPost("redefinir-senha")]
    public async Task<IActionResult> RedefinirSenha([FromBody] ResetPasswordRequest request)
    {
        var email = request.Email.Trim().ToLower();

        if (!_cache.TryGetValue(email, out string codigoEmCache) || codigoEmCache != request.Codigo)
        {
            return BadRequest(new { sucesso = false, mensagem = "Código inválido ou expirado." });
        }

        var senhaCriptografada = request.NovaSenha;

        var sql = @"UPDATE usuario SET senha = @Senha WHERE email = @Email";
        var linhasAfetadas = await _dbConnection.ExecuteAsync(sql, new { Senha = senhaCriptografada, Email = email });

        if (linhasAfetadas > 0)
        {
            _cache.Remove(email);
            return Ok(new { sucesso = true, mensagem = "Senha redefinida com sucesso." });
        }

        return BadRequest(new { sucesso = false, mensagem = "Usuário não encontrado." });
    }

    // Função de hash de senha (substituir por BCrypt em produção!)
    private static string HashPassword(string senha)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(senha));
        return Convert.ToBase64String(bytes);
    }

    // Envia o e-mail com o código de verificação
    private async Task EnviarEmailAsync(string paraEmail, string codigo)
    {
        var emailUsuario = Environment.GetEnvironmentVariable("EMAIL_USERNAME");
        var emailSenha = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");
        var nomeRemetente = Environment.GetEnvironmentVariable("EMAIL_FROM_NAME") ?? "PetMob";

        var smtp = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential(emailUsuario, emailSenha),
            EnableSsl = true,
        };

        var mensagem = new MailMessage
        {
            From = new MailAddress(emailUsuario, nomeRemetente),
            Subject = "Código de verificação - PetMob",
            Body = $"Olá,\n\nSeu código de verificação é: {codigo}\n\nEste código expira em {CodigoExpiraEmMinutos} minutos.\n\nEquipe PetMob.",
            IsBodyHtml = false,
        };

        mensagem.To.Add(paraEmail);

        await smtp.SendMailAsync(mensagem);
    }
}
