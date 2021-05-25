using AutoMapper;
using Contracts.Models;
using Contracts.Repositories;
using Dapper;
using DataAccessContracts.Entities;
using Microsoft.Extensions.Options;
using Services.Repositories.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Repositories {
    public class ShopRepository : IShopRepository {
        private readonly IMapper mapper;
        private readonly string connectionString;

        public ShopRepository(IMapper mapper, IOptions<RepositoryOptions> options) {
            this.mapper = mapper;
            connectionString = options.Value.ConnectionString;
        }

        public IEnumerable<ShopDTO> GetAll() {
            using IDbConnection db = new SqlConnection(connectionString);

            const string query = "SELECT * FROM Shops";
            IEnumerable<ShopDTO> entities = mapper.Map<IEnumerable<ShopDTO>>(db.Query<Shop>(query));

            foreach (ShopDTO entity in entities)
                LoadReferences(db, entity);

            return entities;
        }

        public ShopDTO Get(int id) {
            using IDbConnection db = new SqlConnection(connectionString);

            const string query = "SELECT * FROM Shops WHERE Id = @Id";
            ShopDTO entity = mapper.Map<ShopDTO>(db.QueryFirstOrDefault<Shop>(query, new { Id = id }));

            LoadReferences(db, entity);

            return entity;
        }

        private void LoadReferences(IDbConnection db, ShopDTO entity) {
            const string productQuery = "SELECT p.* FROM Products as p INNER JOIN ProductShops as ps ON p.Id = ps.ProductId WHERE ps.ShopId = @Id";
            entity.Products = mapper.Map<List<ProductDTO>>(db.Query<Product>(productQuery, entity));

            const string requestDeliveriesQuery = "SELECT * FROM RequestDeliverys WHERE ShopId = @Id";
            entity.RequestDeliveries = mapper.Map<List<RequestDeliveryDTO>>(db.Query<RequestDelivery>(requestDeliveriesQuery, entity));
        }

        public int Add(ShopInputDTO inputEntity) {

            using IDbConnection connection = new SqlConnection(connectionString);
            connection.Open();
            using IDbTransaction transaction = connection.BeginTransaction();

            try {
                var insertQuery = "INSERT INTO Shops (Address) VALUES (@Address); SELECT SCOPE_IDENTITY()";
                var createdEntityId = connection.ExecuteScalar<int>(insertQuery, inputEntity, transaction);

                AddShopProducts(connection, transaction, createdEntityId, inputEntity.ProductIds);

                transaction.Commit();

                return createdEntityId;
            } catch {
                transaction.Rollback();
                throw;
            }
        }

        public bool Update(int id, ShopInputDTO inputEntity) {
            using IDbConnection connection = new SqlConnection(connectionString);
            connection.Open();
            using IDbTransaction transaction = connection.BeginTransaction();

            try {
                var updateQuery = "Update Shops SET Address = @Address WHERE Id = @Id";
                Shop entity = mapper.Map<Shop>(inputEntity);
                entity.Id = id;
                var result = connection.Execute(updateQuery, entity, transaction) == 1;

                var deleteShopProducts = "DELETE FROM ProductShops WHERE ShopId = @Id";
                connection.Execute(deleteShopProducts, entity, transaction);
                AddShopProducts(connection, transaction, id, inputEntity.ProductIds);

                transaction.Commit();

                return result;
            } catch {
                transaction.Rollback();
                throw;
            }
        }

        private void AddShopProducts(IDbConnection connection, IDbTransaction transaction, int entityId, List<int> referencedIds) {
            var insertProductShopsQuery = "INSERT INTO ProductShops (ShopId, ProductId) VALUES (@ShopId, @ProductId);";

            var newEntities = new List<object>();
            foreach (var referencedId in referencedIds)
                newEntities.Add(new { ShopId = entityId, ProductId = referencedId });

            connection.Execute(insertProductShopsQuery, newEntities, transaction);
        }

        public bool Remove(int id) {
            using IDbConnection db = new SqlConnection(connectionString);
            var query = "DELETE FROM Shops WHERE Id = @Id";
            return db.Execute(query, new { Id = id }) == 1;
        }
    }
}
