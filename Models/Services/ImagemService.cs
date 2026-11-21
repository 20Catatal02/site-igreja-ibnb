using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace IBNB.Services
{
    public class ImagemService
    {
        public async Task<string> SalvarImagemLocalAsync(IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
                return string.Empty;

            // Validação básica de extensão (segurança)
            var extensoesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extensao = Path.GetExtension(arquivo.FileName).ToLowerInvariant();
            
            if (!extensoesPermitidas.Contains(extensao))
                throw new Exception("Formato de imagem não suportado.");

            // Define a pasta e cria se não existir
            var pastaDestino = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "eventos");
            if (!Directory.Exists(pastaDestino))
                Directory.CreateDirectory(pastaDestino);

            // Gera um nome único para evitar conflitos de arquivos com mesmo nome
            var nomeArquivo = Guid.NewGuid().ToString() + extensao;
            var caminhoCompleto = Path.Combine(pastaDestino, nomeArquivo);

            using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            // Retorna o caminho relativo para salvarmos no banco
            return $"/uploads/eventos/{nomeArquivo}";
        }
    }
}