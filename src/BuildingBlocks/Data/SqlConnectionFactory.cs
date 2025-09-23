using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.Data
{
	public class SqlConnectionFactory
	{
		private readonly string _cs;

		public SqlConnectionFactory(IConfiguration cfg)
		{
			_cs = cfg.GetConnectionString("Default")!;
		}

		public IDbConnection Create() => new SqlConnection(_cs);



	}
}
