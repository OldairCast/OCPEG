using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OCEPG.Infrastructure.DataAccess.Base;
using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.DataAccess.Entities;
using OCPEG.Domain.DataAccess.Interfaces;
using OCPEG.Framework;

namespace OCEPG.Infrastructure.DataAccess.RepositoriesMySql
{
    public class ProductRepositoryMySql : BaseRepository, IProductRepository
    {
        private readonly AppDbContext _dbContext;

        public ProductRepositoryMySql(AppDbContext dbContext, IMapper mapper) : base(mapper)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Obtém dados do assunto por id
        /// </summary>
        /// <param name="id">Id do assunto</param>
        /// <returns>Objeto de negocio Product</returns>
        public async Task<Product> GetById(int id)
        {
            try
            {
                Product product = new Product();

                Assunto? assunto = await _dbContext.Assunto.FindAsync(id);
                if (assunto != null)
                {
                    product.Id = assunto.Id;
                    product.Name = string.IsNullOrEmpty(assunto.Descricao) ? string.Empty : assunto.Descricao;
                }

                return product;

            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw;
            }
        }

        /// <summary>
        /// Retorna lista de assunto
        /// </summary>
        /// <returns>Lista de Objeto de negocio Product</returns>
        public async Task<List<Product>> GetAll()
        {
            try
            {
                List<Product> productlst = new List<Product>();

                List<Assunto>? assuntolst = await _dbContext.Assunto.ToListAsync();

                foreach (var item in assuntolst)
                {
                    Product product = new Product
                    {
                        Id = item.Id,
                        Name = item.Descricao != null ? item.Descricao : string.Empty,
                    };

                    productlst.Add(product);
                }

                return productlst;

            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw;
            }
        }

        /// <summary>
        /// Insere um novo assunto
        /// </summary>
        /// <returns>Id do assunto criado</returns>
        public async Task<int> Create(Product product)
        {
            try
            {
                Assunto assunto = new Assunto
                {
                    Id = product.Id,
                    Descricao = product.Name,
                };

                await _dbContext.Assunto.AddAsync(assunto);
                await _dbContext.SaveChangesAsync();

                return assunto.Id;

            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw;
            }
        }

        /// <summary>
        /// Atualiza um assunto
        /// </summary>
        /// <param name="customer">Dados do assunto</param> 
        public async Task<bool> Update(Product product)
        {
            try
            {
                Assunto? assunto = await _dbContext.Assunto.FindAsync(product.Id);
                if (assunto != null)
                {
                    assunto.Descricao = product.Name;
                    _dbContext.Assunto.Update(assunto);
                }

                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw;
            }
        }

        /// <summary>
        /// Exclui um assunto
        /// </summary>
        /// <param name="id">Id do assunto</param>
        public async Task<bool> Delete(int id)
        {
            try
            {
                Assunto? product = await _dbContext.Assunto.FindAsync(id);

                if (product != null)
                {
                    _dbContext.Assunto.Remove(product);
                    await _dbContext.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw;
            }
        }

    }
}
