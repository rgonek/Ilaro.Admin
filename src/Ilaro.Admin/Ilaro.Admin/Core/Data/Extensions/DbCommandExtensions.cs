using System;
using System.Data.Common;
using System.Globalization;

namespace Ilaro.Admin.Core.Data.Extensions
{
    public static class DbCommandExtensions
    {
        public static string PreviewCommandText(this DbCommand cmd)
        {
            var previewSql = "";
            foreach (DbParameter parameter in cmd.Parameters)
            {
                previewSql += 
                    parameter.ParameterName +
                    ": " + 
                    Convert.ToString(parameter.Value, CultureInfo.InvariantCulture) + 
                    Environment.NewLine;
            }

            previewSql += Environment.NewLine + cmd.CommandText;

            return previewSql;
        }
    }
}