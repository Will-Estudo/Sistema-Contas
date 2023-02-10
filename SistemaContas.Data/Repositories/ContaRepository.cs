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

namespace SistemaContas.Data.Repositories
{
    public class ContaRepository : IRepository<Conta>
    {
        public void Add(Conta entity)
        {
            var query = @"insert into conta
                          (ID, NOME, VALOR, DATA, OBSERVACOES, TIPO, IDUSUARIO, IDCATEGORIA)
                          values 
                          (@ID, @NOME, @VALOR, @DATA, @OBSERVACOES, @TIPO, @IDUSUARIO, @IDCATEGORIA)";

            using (var connection = new SqlConnection(SqlServerConfiguration.ConnectionString))
            {
                connection.Execute(query, entity);
            }
        }

        public void Update(Conta entity)
        {
            var query = @"update conta
                          set
                            NOME = @NOME, 
                            VALOR = @VALOR,
                            DATA = @DATA,
                            OBSERVACOES = @OBSERVACOES,
                            TIPO = @TIPO,
                            IDCATEGORIA = @IDCATEGORIA
                          where ID = @Id";

            using (var connection = new SqlConnection(SqlServerConfiguration.ConnectionString))
            {
                connection.Execute(query, entity);
            }
        }

        public void Delete(Conta entity)
        {
            var query = @"delete from CONTA
                          where ID = @Id";

            using (var connection = new SqlConnection(SqlServerConfiguration.ConnectionString))
            {
                connection.Execute(query, entity);
            }
        }

        public List<Conta> GetAll()
        {
            var query = @"select * from CONTA
                          order by nome";

            using (var connection = new SqlConnection(SqlServerConfiguration.ConnectionString))
            {
                return connection.Query<Conta>(query).ToList();
            }
        }

        public Conta? GetById(Guid id)
        {
            var query = @"select * from CONTA
                          where ID = @id";

            using (var connection = new SqlConnection(SqlServerConfiguration.ConnectionString))
            {
                return connection.Query<Conta>(query, new { id }).FirstOrDefault();
            }
        }

        public List<Conta> GetByUsario(Guid idUsuario)
        {
            var query = @"select * from CONTA
                          where IDUSUARIO = @idUsuario
                          order by nome";

            using (var connection = new SqlConnection(SqlServerConfiguration.ConnectionString))
            {
                return connection.Query<Conta>(query, new { idUsuario }).ToList();
            }
        }

        public List<Conta> GetByUsarioAndDatas(Guid idUsuario, DateTime dataIni, DateTime dataFim)
        {
            var query = @"select * from CONTA
                          where IDUSUARIO = @idUsuario
                          and DATA between @dataIni and @dataFim
                          order by DATA desc";

            using (var connection = new SqlConnection(SqlServerConfiguration.ConnectionString))
            {
                return connection.Query<Conta>(query, new { idUsuario, dataIni, dataFim }).ToList();
            }
        }

    }
}
