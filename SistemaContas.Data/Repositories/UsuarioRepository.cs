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
    public class UsuarioRepository :IRepository<Usuario>
    {
        public void Add(Usuario entity)
        {
            var query = @"insert into USUARIO
                          (ID, NOME, EMAIL, SENHA)
                          values 
                          (@ID, @NOME, @EMAIL, @SENHA)";

            using (var connection = new SqlConnection(SqlServerConfiguration.ConnectionString))
            {
                connection.Execute(query, entity);
            }
        }

        public void Update(Usuario entity)
        {
            var query = @"update USUARIO
                          set
                            NOME = @NOME, 
                            EMAIL = @EMAIL,
                            SENHA = @SENHA
                          where ID = @Id";

            using (var connection = new SqlConnection(SqlServerConfiguration.ConnectionString))
            {
                connection.Execute(query, entity);
            }
        }

        public void Update(Guid id, string novaSenha)
        {
            var query = @"update USUARIO
                          set                            
                            SENHA = @novaSenha
                          where ID = @Id";

            using (var connection = new SqlConnection(SqlServerConfiguration.ConnectionString))
            {
                connection.Execute(query, new { id, novaSenha });
            }
        }

        public void Delete(Usuario entity)
        {
            var query = @"delete from USUARIO
                          where ID = @Id";

            using (var connection = new SqlConnection(SqlServerConfiguration.ConnectionString))
            {
                connection.Execute(query, entity);
            }
        }

        public List<Usuario> GetAll()
        {
            var query = @"select * from USUARIO
                          order by nome";

            using (var connection = new SqlConnection(SqlServerConfiguration.ConnectionString))
            {
                return connection.Query<Usuario>(query).ToList();
            }
        }

        public Usuario? GetById(Guid id)
        {
            var query = @"select * from USUARIO
                          where ID = @id";

            using (var connection = new SqlConnection(SqlServerConfiguration.ConnectionString))
            {
                return connection.Query<Usuario>(query, new { id }).FirstOrDefault();
            }
        }

        public Usuario? GetByEmail(string email)
        {
            var query = @"select * from USUARIO
                          where EMAIL = @email";

            using (var connection = new SqlConnection(SqlServerConfiguration.ConnectionString))
            {
                return connection.Query<Usuario>(query, new { email }).FirstOrDefault();
            }
        }

        public Usuario? GetByEmailAndSenha(string email, string senha)
        {
            var query = @"select * from USUARIO
                          where EMAIL = @email
                          and SENHA = @senha";

            using (var connection = new SqlConnection(SqlServerConfiguration.ConnectionString))
            {
                return connection.Query<Usuario>(query, new { email, senha }).FirstOrDefault();
            }
        }
    }
}
