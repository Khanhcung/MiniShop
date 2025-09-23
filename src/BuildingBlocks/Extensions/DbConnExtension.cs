using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.Extensions
{
	public static class DbConnExt
	{
		public static async Task<IDbConnection> EnsureOpenAsync(this IDbConnection c)
		{
			if (c.State != ConnectionState.Open) await ((DbConnection)c).OpenAsync();
			return c;
		}
	}
}
