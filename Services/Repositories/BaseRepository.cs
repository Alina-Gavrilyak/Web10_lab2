using AutoMapper;
using Contracts.Repositories;
using Dapper;
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
    public abstract class BaseRepository<TEntity, TDTOEntity, TInputEntity> : IRepository<TDTOEntity, TInputEntity, int> {

        private readonly IMapper mapper;
        private readonly string connectionString;
        private readonly string tableName;

        public BaseRepository(IMapper mapper, IOptions<RepositoryOptions> options, string tableName) {
            this.mapper = mapper;
            connectionString = options.Value.ConnectionString;
            this.tableName = tableName;
        }

        public IEnumerable<TDTOEntity> GetAll() {
            using IDbConnection db = new SqlConnection(connectionString);
            string query = $"SELECT * FROM {tableName}";
            return mapper.Map<IEnumerable<TDTOEntity>>(db.Query<TEntity>(query));
        }

        public TDTOEntity Get(int id) {
            using IDbConnection db = new SqlConnection(connectionString);
            string query = $"SELECT * FROM {tableName} WHERE Id = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Int32);
            return mapper.Map<TDTOEntity>(db.QueryFirstOrDefault<TEntity>(query, parameters));
        }

        public int Add(TInputEntity entity) {
            throw new NotImplementedException();
        }

        public bool Update(int id, TInputEntity entity) {
            throw new NotImplementedException();
        }

        public bool Remove(int id) {
            using IDbConnection db = new SqlConnection(connectionString);
            string query = $"DELETE FROM {tableName} WHERE Id = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Int32);
            return db.Execute(query, parameters) == 1;
        }
 
    }
}
