using Dapper;
using SistemaContas.Data.Configurations;
using SistemaContas.Data.Entities;
using SistemaContas.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace SistemaContas.Data.Repositories
{
    public class CategoriaRepository : IRepository<Categoria>
    {
        public void Add(Categoria entity)
        {
            var query = @"insert into categoria (ID, NOME, IDUSUARIO)
                          values (@Id, @Nome, @IdUsuario)";

            using (var connection = new SqlConnection(SqlServerConfiguration.ConnectionString))
            {
                connection.Execute(query, entity);
            }
        }

        public void Delete(Categoria entity)
        {
            var query = @"delete from categoria
                          where ID = @Id";

            using (var connection = new SqlConnection(SqlServerConfiguration.ConnectionString))
            {
                connection.Execute(query, entity);
            }
        }

        public List<Categoria> GetAll()
        {
            var query = @"select * from categoria
                          order by nome";

            using (var connection = new SqlConnection(SqlServerConfiguration.ConnectionString))
            {
                return connection.Query<Categoria>(query).ToList();
            }
        }

        public Categoria? GetById(Guid id)
        {
            var query = @"select * from categoria
                          where ID = @id";

            using (var connection = new SqlConnection(SqlServerConfiguration.ConnectionString))
            {
                return connection.Query<Categoria>(query, new { id }).FirstOrDefault();
            }
        }

        public List<Categoria> GetByUsario(Guid idUsuario)
        {
            var query = @"select * from categoria
                          where IDUSUARIO = @idUsuario
                          order by nome";

            using (var connection = new SqlConnection(SqlServerConfiguration.ConnectionString))
            {
                return connection.Query<Categoria>(query, new { idUsuario }).ToList();
            }
        }

        public void Update(Categoria entity)
        {
            var query = @"update categoria
                          set NOME = @NOME 
                          where ID = @Id";

            using (var connection = new SqlConnection(SqlServerConfiguration.ConnectionString))
            {
                connection.Execute(query, entity);
            }
        }

        public int CountContasByIdCategoria(Guid id)
        {
            var query = @"select count(*) from conta 
                          where IDCATEGORIA = @Id";

            using (var connection = new SqlConnection(SqlServerConfiguration.ConnectionString))
            {
                return connection.Query<int>(query, new { id } ).FirstOrDefault();
            }
        }
    }
}
