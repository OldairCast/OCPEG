using AutoMapper;
using OCPEG.Application.UseCasesServices.Interfaces;
using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.DataAccess.Interfaces;
using OCPEG.Domain.Dto;
using OCPEG.Framework;
using OCPEG.Framework.Exception;
using OCPEG.Framework.Resource;

namespace OCPEG.Application.UseCasesServices.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
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
                Product product = await _productRepository.GetById(id);
                
                return product;
            }
            catch (BusinessLogicCustomException)
            {
                throw;
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw new OcException(ex.Message);
            }
        }

        /// <summary>
        /// Retorna lista de assunto
        /// </summary>
        /// <returns>Lista de Objeto de negocio Product</returns>
        public async Task<List<Product>> GetProductAll()
        {
            try
            {
                List<Product> products = await _productRepository.GetAll();

                return products;

            }
            catch (BusinessLogicCustomException)
            {
                throw;
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw new OcException(ex.Message);
            }
        }

        /// <summary>
        /// Insere um novo assunto
        /// </summary>
        /// <returns>Id do assunto criado</returns>
        public async Task<int> Create(Product entity)
        {
            try
            {
                Validate(entity);

                int productId = await _productRepository.Create(entity);

                return productId;
            }
            catch (BusinessLogicCustomException)
            {
                throw;
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw new OcException(ex.Message);
            }
        }

        /// <summary>
        /// Atualiza um assunto
        /// </summary>
        /// <param name="customer">Dados do assunto</param> 
        public async Task<bool> Update(Product entity)
        {
            try
            {
                Validate(entity);

                await _productRepository.Update(entity);

                return true;
            }
            catch (BusinessLogicCustomException)
            {
                throw;
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw new OcException(ex.Message);
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
                await _productRepository.Delete(id);

                return true;
            }
            catch (BusinessLogicCustomException)
            {
                throw;
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw new OcException(ex.Message);
            }
        }

        /// <summary>
        /// Valida as informações da requisição
        /// </summary>
        private static void Validate(Product product)
        {
            if (string.IsNullOrEmpty(product.Name))
            {
                throw new BusinessLogicCustomException(string.Format("{0} {1}", OcPegResource.ENTER_THE, OcPegResource.NAME));
            }
        }

        Task<List<ProductDto>> IBaseService<Product, ProductDto>.GetAll()
        {
            throw new NotImplementedException();
        }
    }

}
