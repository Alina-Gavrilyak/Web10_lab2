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

            const string productQuery = "SELECT * FROM Products";
            IEnumerable<ProductDTO> products = mapper.Map<IEnumerable<ProductDTO>>(db.Query<Product>(productQuery));

            foreach (ProductDTO product in products)
                LoadReferences(db, product);

            return products;
        }

        public ProductDTO Get(int id) {
            using IDbConnection db = new SqlConnection(connectionString);

            const string query = "SELECT * FROM Products WHERE Id = @Id";
            ProductDTO product = mapper.Map<ProductDTO>(db.QueryFirstOrDefault<Product>(query, new { Id = id }));

            LoadReferences(db, product);

            return product;
        }

        private void LoadReferences(IDbConnection db, ProductDTO product) {
            const string shopQuery = "SELECT s.* FROM Shops as s INNER JOIN ProductShops as ps ON s.Id = ps.ShopId WHERE ps.ProductId = @Id";
            product.Shops = mapper.Map<List<ShopDTO>>(db.Query<Shop>(shopQuery, product));

            const string warehouseDTOQuery = "SELECT w.* FROM Warehouses as w INNER JOIN ProductWarehouses as pw ON w.Id = pw.WarehouseId WHERE pw.ProductId = @Id";
            product.Warehouses = mapper.Map<List<WarehouseDTO>>(db.Query<Warehouse>(warehouseDTOQuery, product));

            const string requestDeliveriesQuery = "SELECT rd.* FROM RequestDeliverys as rd INNER JOIN DeliveryItems as di ON rd.Id = di.RequestDeliveryId WHERE di.ProductId = @Id";
            product.RequestDeliveries = mapper.Map<List<RequestDeliveryDTO>>(db.Query<RequestDelivery>(requestDeliveriesQuery, product));
        }

        public int Add(ProductInputDTO inputEntity) {

            using IDbConnection connection = new SqlConnection(connectionString);
            connection.Open();
            using IDbTransaction transaction = connection.BeginTransaction();

            try {
                var insertProductQuery = "INSERT INTO Products (Number, Name, Category, Price) VALUES (@Number, @Name, @Category, @Price); SELECT SCOPE_IDENTITY()";
                var createdProductId = connection.ExecuteScalar<int>(insertProductQuery, inputEntity, transaction);

                AddProductShops(connection, transaction, createdProductId, inputEntity.ShopIds);
                AddProductWarehouses(connection, transaction, createdProductId, inputEntity.WarehouseIds);

                transaction.Commit();

                return createdProductId;
            } catch {
                transaction.Rollback();
                throw;
            }
        }

        public bool Update(int id, ProductInputDTO inputEntity) {
            using IDbConnection connection = new SqlConnection(connectionString);
            connection.Open();
            using IDbTransaction transaction = connection.BeginTransaction();

            try {
                var updateProductQuery = "Update Products SET Number = @Number, Name = @Name, Category = @Category, Price = @Price WHERE Id = @Id";
                Product product = mapper.Map<Product>(inputEntity);
                product.Id = id;
                var result = connection.Execute(updateProductQuery, product, transaction) == 1;

                var deleteProductShopsQuery = "DELETE FROM ProductShops WHERE ProductId = @Id";
                connection.Execute(deleteProductShopsQuery, product, transaction);
                AddProductShops(connection, transaction, id, inputEntity.ShopIds);

                var deleteProductWarehousesQuery = "DELETE FROM ProductWarehouses WHERE ProductId = @Id";
                connection.Execute(deleteProductWarehousesQuery, product, transaction);
                AddProductWarehouses(connection, transaction, id, inputEntity.WarehouseIds);

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
            return db.Execute(query, new { Id = id }) == 1;
        }
    }
}
