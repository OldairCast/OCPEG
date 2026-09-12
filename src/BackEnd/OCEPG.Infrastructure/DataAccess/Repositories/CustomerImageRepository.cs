using OCEPG.Infrastructure.DataAccess.Base;
using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.DataAccess.Entities;
using OCPEG.Domain.DataAccess.Interfaces;
using OCPEG.Framework;

namespace OCEPG.Infrastructure.DataAccess.Repositories
{
    public class CustomerImageRepository: ICustomerImageRepository
    {

        private readonly AppDbContext _dbContext;

        public CustomerImageRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Insere uma nova imagem
        /// </summary>
        /// <returns>Código da imagem gerada</returns>
        public async Task<long> Upload(CustomerImage image)
        {
            try
            {
                ClienteImagem imagem = new ClienteImagem
                {
                   ClienteId = image.CustomerId,
                   Descricao = image.Nome,
                   ContentType = image.ContentType,
                   Dados = image.Dados,
                   DataUpload = DateTime.Now
                };

                await _dbContext.ClienteImagem.AddAsync(imagem);
                await _dbContext.SaveChangesAsync();

                return imagem.Id;

            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw;
            }
        }

    }
}
