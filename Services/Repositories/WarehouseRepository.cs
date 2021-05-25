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
    public class WarehouseRepository : IWarehouseRepository {
        private readonly IMapper mapper;
        private readonly string connectionString;

        public WarehouseRepository(IMapper mapper, IOptions<RepositoryOptions> options) {
            this.mapper = mapper;
            connectionString = options.Value.ConnectionString;
        }

        public IEnumerable<WarehouseDTO> GetAll() {
            using IDbConnection db = new SqlConnection(connectionString);

            const string query = "SELECT * FROM Warehouses";
            IEnumerable<WarehouseDTO> entities = mapper.Map<IEnumerable<WarehouseDTO>>(db.Query<Warehouse>(query));

            foreach (WarehouseDTO entity in entities)
                LoadReferences(db, entity);

            return entities;
        }

        public WarehouseDTO Get(int id) {
            using IDbConnection db = new SqlConnection(connectionString);

            const string query = "SELECT * FROM Warehouses WHERE Id = @Id";
            WarehouseDTO entity = mapper.Map<WarehouseDTO>(db.QueryFirstOrDefault<Warehouse>(query, new { Id = id }));

            LoadReferences(db, entity);

            return entity;
        }

        private void LoadReferences(IDbConnection db, WarehouseDTO entity) {
            const string productQuery = "SELECT p.* FROM Products as p INNER JOIN ProductWarehouses as ps ON p.Id = ps.ProductId WHERE ps.WarehouseId = @Id";
            entity.Products = mapper.Map<List<ProductDTO>>(db.Query<Product>(productQuery, entity));

            const string requestDeliveriesQuery = "SELECT * FROM RequestDeliverys WHERE WarehouseId = @Id";
            entity.RequestDeliveries = mapper.Map<List<RequestDeliveryDTO>>(db.Query<RequestDelivery>(requestDeliveriesQuery, entity));
        }

        public int Add(WarehouseInputDTO inputEntity) {

            using IDbConnection connection = new SqlConnection(connectionString);
            connection.Open();
            using IDbTransaction transaction = connection.BeginTransaction();

            try {
                var insertQuery = "INSERT INTO Warehouses (Number, Address) VALUES (@Number, @Address); SELECT SCOPE_IDENTITY()";
                var createdEntityId = connection.ExecuteScalar<int>(insertQuery, inputEntity, transaction);

                AddWarehouseProducts(connection, transaction, createdEntityId, inputEntity.ProductIds);

                transaction.Commit();

                return createdEntityId;
            } catch {
                transaction.Rollback();
                throw;
            }
        }

        public bool Update(int id, WarehouseInputDTO inputEntity) {
            using IDbConnection connection = new SqlConnection(connectionString);
            connection.Open();
            using IDbTransaction transaction = connection.BeginTransaction();

            try {
                var updateQuery = "Update Warehouses SET Number = @Number, Address = @Address WHERE Id = @Id";
                Warehouse entity = mapper.Map<Warehouse>(inputEntity);
                entity.Id = id;
                var result = connection.Execute(updateQuery, entity, transaction) == 1;

                var deleteWarehouseProducts = "DELETE FROM ProductWarehouses WHERE WarehouseId = @Id";
                connection.Execute(deleteWarehouseProducts, entity, transaction);
                AddWarehouseProducts(connection, transaction, id, inputEntity.ProductIds);

                transaction.Commit();

                return result;
            } catch {
                transaction.Rollback();
                throw;
            }
        }

        private void AddWarehouseProducts(IDbConnection connection, IDbTransaction transaction, int entityId, List<int> referencedIds) {
            var insertProductWarehousesQuery = "INSERT INTO ProductWarehouses (WarehouseId, ProductId) VALUES (@WarehouseId, @ProductId);";

            var newEntities = new List<object>();
            foreach (var referencedId in referencedIds)
                newEntities.Add(new { WarehouseId = entityId, ProductId = referencedId });

            connection.Execute(insertProductWarehousesQuery, newEntities, transaction);
        }

        public bool Remove(int id) {
            using IDbConnection db = new SqlConnection(connectionString);
            var query = "DELETE FROM Products WHERE Id = @Id";
            return db.Execute(query, new { Id = id }) == 1;
        }
    }
}
