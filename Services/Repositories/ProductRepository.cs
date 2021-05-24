using AutoMapper;
using Contracts.Models;
using Contracts.Repositories;
using Dapper;
using DataAccessContracts.Entities;
using Microsoft.Extensions.Options;
using Services.Repositories.Options;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Services.Repositories {
    public class ProductRepository : IProductRepository {

        private readonly IMapper mapper;
        private readonly string connectionString;

        public ProductRepository(IMapper mapper, IOptions<RepositoryOptions> options) {
            this.mapper = mapper;
            connectionString = options.Value.ConnectionString;
        }

        public IEnumerable<ProductDTO> GetAll() {
            using IDbConnection db = new SqlConnection(connectionString);
            const string query = "SELECT * FROM Products";
            return mapper.Map<IEnumerable<ProductDTO>>(db.Query<Product>(query));
        }

        public ProductDTO Get(int id) {
            using IDbConnection db = new SqlConnection(connectionString);
            const string query = "SELECT * FROM Products WHERE Id = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Int32);
            return mapper.Map<ProductDTO>(db.QueryFirstOrDefault<Product>(query, parameters));
        }

        public int Add(ProductInputDTO entity) {

            using IDbConnection connection = new SqlConnection(connectionString);
            connection.Open();
            using IDbTransaction transaction = connection.BeginTransaction();

            try {
                var insertProductQuery = "INSERT INTO Products (Number, Name, Category, Price) VALUES (@Number, @Name, @Category, @Price); SELECT SCOPE_IDENTITY()";
                Product product = mapper.Map<Product>(entity);
                var createdProductId = connection.ExecuteScalar<int>(insertProductQuery, product, transaction);

                AddProductShops(connection, transaction, createdProductId, entity.ShopIds);
                AddProductWarehouses(connection, transaction, createdProductId, entity.WarehouseIds);

                transaction.Commit();

                return createdProductId;
            } catch {
                transaction.Rollback();
                throw;
            }
        }

        public bool Update(int id, ProductInputDTO entity) {
            using IDbConnection connection = new SqlConnection(connectionString);
            connection.Open();
            using IDbTransaction transaction = connection.BeginTransaction();

            try {
                var updateProductQuery = "Update Products SET Number = @Number, Name = @Name, Category = @Category, Price = @Price WHERE Id = @Id";
                Product product = mapper.Map<Product>(entity);
                product.Id = id;
                var result = connection.Execute(updateProductQuery, product, transaction) == 1;

                var deleteProductShopsQuery = "DELETE FROM ProductShops WHERE ProductId = @Id";
                connection.Execute(deleteProductShopsQuery, product, transaction);
                AddProductShops(connection, transaction, id, entity.ShopIds);

                var deleteProductWarehousesQuery = "DELETE FROM ProductWarehouses WHERE ProductId = @Id";
                connection.Execute(deleteProductWarehousesQuery, product, transaction);
                AddProductWarehouses(connection, transaction, id, entity.WarehouseIds);

                transaction.Commit();

                return result;
            } catch {
                transaction.Rollback();
                throw;
            }
        }

        private void AddProductShops(IDbConnection connection, IDbTransaction transaction, int productId, List<int> shopIds) {
            var insertProductShopsQuery = "INSERT INTO ProductShops (ProductId, ShopId) VALUES (@ProductId, @ShopId);";

            var newProductShops = new List<object>();
            foreach (var shopId in shopIds)
                newProductShops.Add(new { ProductId = productId, ShopId = shopId });

            connection.Execute(insertProductShopsQuery, newProductShops, transaction);
        }

        private void AddProductWarehouses(IDbConnection connection, IDbTransaction transaction, int productId, List<int> warehouseIds) {
            var insertProductShopsQuery = "INSERT INTO ProductWarehouses (ProductId, WarehouseId) VALUES (@ProductId, @WarehouseId);";

            var newProductShops = new List<object>();
            foreach (var warehouseId in warehouseIds)
                newProductShops.Add(new { ProductId = productId, WarehouseId = warehouseId });

            connection.Execute(insertProductShopsQuery, newProductShops, transaction);
        }

        public bool Remove(int id) {
            using IDbConnection db = new SqlConnection(connectionString);
            var query = "DELETE FROM Products WHERE Id = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Int32);
            return db.Execute(query, parameters) == 1;
        }
    }
}
