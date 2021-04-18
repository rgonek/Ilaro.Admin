using SqlKata.Execution;
using System;

namespace Ilaro.Admin.Core
{
    public class IlaroAdminOptions : IIlaroAdminOptions
    {
        public int? ItemsQuantityPerPage { get; }

        public string PageRequestName { get; }

        public string SearchQueryRequestName { get; }

        public string PerPageRequestName { get; }

        public string OrderRequestName { get; }

        public string OrderDirectionRequestName { get; }

        public long? MaxFileSize { get; }

        public string AllowedFileExtensions { get; }

        public string UploadFilesTempFolderSufix { get; }

        public string ConnectionString { get; set; }

        public Func<string, QueryFactory> QueryFactoryFactory { get; set; }

        public string RoutePrefix { get; set; }

        //public IlaroAdminOptions(string routePrefix, string connectionStringName)
        //{
        //    RoutePrefix = RoutePrefix;
        //    ConnectionStringName = connectionStringName;


        //}
    }
}
