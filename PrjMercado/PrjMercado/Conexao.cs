using System.Configuration;
using System.Data.SqlClient;

namespace PrjMercado
{
    public static class Conexao
    {
        public static bool TentarCriarConexao(out SqlConnection conexao)
        {
            ConnectionStringSettings configuracao = ConfigurationManager.ConnectionStrings["MercadoOnline"];
            if (configuracao == null || string.IsNullOrWhiteSpace(configuracao.ConnectionString))
            {
                conexao = null;
                return false;
            }

            conexao = new SqlConnection(configuracao.ConnectionString);
            return true;
        }
    }
}
