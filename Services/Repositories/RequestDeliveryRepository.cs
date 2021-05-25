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
    public class RequestDeliveryRepository : IRequestDeliveryRepository {
        private readonly IMapper mapper;
        private readonly string connectionString;

        public RequestDeliveryRepository(IMapper mapper, IOptions<RepositoryOptions> options) {
            this.mapper = mapper;
            connectionString = options.Value.ConnectionString;
        }

        public IEnumerable<RequestDeliveryDTO> GetAll() {
            using IDbConnection db = new SqlConnection(connectionString);

            const string query = "SELECT * FROM RequestDeliverys";
            IEnumerable<RequestDeliveryDTO> entities = mapper.Map<IEnumerable<RequestDeliveryDTO>>(db.Query<RequestDelivery>(query));

            foreach (RequestDeliveryDTO entity in entities)
                LoadReferences(db, entity);

            return entities;
        }

        public RequestDeliveryDTO Get(int id) {
            using IDbConnection db = new SqlConnection(connectionString);

            const string query = "SELECT * FROM RequestDeliverys WHERE Id = @Id";
            RequestDeliveryDTO entity = mapper.Map<RequestDeliveryDTO>(db.QueryFirstOrDefault<RequestDelivery>(query, new { Id = id }));

            LoadReferences(db, entity);

            return entity;
        }

        private void LoadReferences(IDbConnection db, RequestDeliveryDTO entity) {
            const string shopQuery = "SELECT * FROM Shops WHERE Id = @ShopId";
            entity.Shop = mapper.Map<ShopDTO>(db.QueryFirst<Shop>(shopQuery, entity));

            const string warehouseQuery = "SELECT * FROM WHERE Id = @WarehouseId";
            entity.Warehouse = mapper.Map<WarehouseDTO>(db.QueryFirst<Warehouse>(warehouseQuery, entity));

            const string productsQuery = "SELECT p.* FROM Products as p INNER JOIN DeliveryItems as di ON p.Id = di.ProductId WHERE di.RequestDeliveryId = @Id";
            entity.Products = mapper.Map<List<ProductDTO>>(db.Query<Product>(productsQuery, entity));
        }

        public int Add(RequestDeliveryInputDTO inputEntity) {

            using IDbConnection connection = new SqlConnection(connectionString);
            connection.Open();
            using IDbTransaction transaction = connection.BeginTransaction();

            try {
                var insertQuery = "INSERT INTO RequestDeliverys (Number, WarehouseId, ShopId) VALUES (@Number, @WarehouseId, @ShopId); SELECT SCOPE_IDENTITY()";
                var createdEntityId = connection.ExecuteScalar<int>(insertQuery, inputEntity, transaction);

                AddDeliveryItems(connection, transaction, createdEntityId, inputEntity.ProductIds);

                transaction.Commit();

                return createdEntityId;
            } catch {
                transaction.Rollback();
                throw;
            }
        }

        public bool Update(int id, RequestDeliveryInputDTO inputEntity) {
            using IDbConnection connection = new SqlConnection(connectionString);
            connection.Open();
            using IDbTransaction transaction = connection.BeginTransaction();

            try {
                var updateQuery = "Update RequestDeliverys SET Number = @Number, WarehouseId = @WarehouseId, ShopId = @ShopId WHERE Id = @Id";
                RequestDelivery entity = mapper.Map<RequestDelivery>(inputEntity);
                entity.Id = id;
                var result = connection.Execute(updateQuery, entity, transaction) == 1;

                var deleteDeliveryItems = "DELETE FROM DeliveryItems WHERE RequestDeliveryId = @Id";
                connection.Execute(deleteDeliveryItems, entity, transaction);
                AddDeliveryItems(connection, transaction, id, inputEntity.ProductIds);

                transaction.Commit();

                return result;
            } catch {
                transaction.Rollback();
                throw;
            }
        }

        private void AddDeliveryItems(IDbConnection connection, IDbTransaction transaction, int entityId, List<int> referencedIds) {
            var insertProductShopsQuery = "INSERT INTO DeliveryItems (RequestDeliveryId, ProductId) VALUES (@RequestDeliveryId, @ProductId);";

            var newEntities = new List<object>();
            foreach (var referencedId in referencedIds)
                newEntities.Add(new { RequestDeliveryId = entityId, ProductId = referencedId });

            connection.Execute(insertProductShopsQuery, newEntities, transaction);
        }

        public bool Remove(int id) {
            using IDbConnection db = new SqlConnection(connectionString);
            var query = "DELETE FROM RequestDeliverys WHERE Id = @Id";
            return db.Execute(query, new { Id = id }) == 1;
        }
    }
}
