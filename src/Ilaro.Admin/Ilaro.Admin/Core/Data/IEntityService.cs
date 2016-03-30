using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Ilaro.Admin.Models;

namespace Ilaro.Admin.Core.Data
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

        IList<GroupProperties> PrepareGroups(
            EntityRecord entityRecord,
            bool getKey = true,
            string key = null);
    }
}