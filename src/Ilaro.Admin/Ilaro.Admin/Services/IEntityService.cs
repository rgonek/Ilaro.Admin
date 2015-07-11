using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Ilaro.Admin.Core;
using Ilaro.Admin.Models;

namespace Ilaro.Admin.Services
{
    public interface IEntityService
    {
        string Create(
            Entity entity, 
            FormCollection collection, 
            HttpFileCollectionBase files);

        bool Edit(
            Entity entity,
            string key,
            FormCollection collection,
            HttpFileCollectionBase files);

        bool Delete(
            Entity entity,
            string key,
            IEnumerable<PropertyDeleteOption> options);


        bool IsRecordExists(Entity entity, string key);

        IList<GroupProperties> PrepareGroups(
            Entity entity,
            bool getKey = true,
            string key = null);
    }
}