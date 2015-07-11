using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ilaro.Admin.Core;
using Ilaro.Admin.Core.Data;
using Ilaro.Admin.Models;
using Ilaro.Admin.Validation;
using Resources;

namespace Ilaro.Admin.Services
{
    public class EntityService : IEntityService
    {
        private readonly Notificator _notificator;
        private readonly IFetchingRecords _source;
        private readonly ICreatingRecords _creator;
        private readonly IUpdatingRecords _updater;
        private readonly IDeletingRecords _deleter;
        private readonly IValidateEntity _validator;

        public EntityService(
            Notificator notificator,
            IFetchingRecords source,
            ICreatingRecords creator,
            IUpdatingRecords updater,
            IDeletingRecords deleter,
            IValidateEntity validator)
        {
            if (notificator == null)
                throw new ArgumentNullException("notificator");
            if (source == null)
                throw new ArgumentNullException("source");
            if (creator == null)
                throw new ArgumentNullException("creator");
            if (updater == null)
                throw new ArgumentNullException("updater");
            if (deleter == null)
                throw new ArgumentNullException("deleter");
            if (validator == null)
                throw new ArgumentNullException("validator");

            _notificator = notificator;
            _source = source;
            _creator = creator;
            _updater = updater;
            _deleter = deleter;
            _validator = validator;
        }

        public string Create(
            Entity entity,
            FormCollection collection,
            HttpFileCollectionBase files)
        {
            entity.Fill(collection, files);
            if (_validator.Validate(entity) == false)
            {
                _notificator.Error("Not valid");
                return null;
            }
            var existingItem = _source.GetRecord(entity, entity.Key.Value.AsObject);
            if (existingItem != null)
            {
                _notificator.Error(IlaroAdminResources.EntityAlreadyExist);
                return null;
            }

            var id = _creator.Create(entity);

            return id;
        }

        public bool Edit(
            Entity entity,
            string key,
            FormCollection collection,
            HttpFileCollectionBase files)
        {
            if (IsRecordExists(entity, key) == false)
            {
                return false;
            }

            entity.Fill(collection, files);
            if (_validator.Validate(entity) == false)
            {
                _notificator.Error("Not valid");
                return false;
            }

            var result = _updater.Update(entity);

            return result;
        }

        public bool Delete(Entity entity, string key, IEnumerable<PropertyDeleteOption> options)
        {
            if (IsRecordExists(entity, key) == false)
            {
                return false;
            }

            options = options ?? new List<PropertyDeleteOption>();
            var deleteOptions = options.ToDictionary(x => x.PropertyName, x => x.DeleteOption);
            foreach (var property in entity.GetForeignsForUpdate())
            {
                if (!deleteOptions.ContainsKey(property.ForeignEntity.Name))
                {
                    deleteOptions[property.ForeignEntity.Name] = property.DeleteOption;
                }
            }

            var result = _deleter.Delete(entity, deleteOptions);

            return result;
        }

        //private void FileHandle(Entity entity)
        //{
        //    foreach (var property in entity
        //        .CreateProperties(getForeignCollection: false)
        //        .Where(x => x.TypeInfo.DataType == DataType.File))
        //    {
        //        if (property.TypeInfo.Type == typeof(string))
        //        {
        //            // we must save file to disk and save file path in db
        //            var file = (HttpPostedFile)property.Value.Raw;
        //            var fileName = String.Empty;
        //            if (property.ImageOptions.NameCreation == NameCreation.UserInput)
        //            {
        //                fileName = "test.jpg";
        //            }
        //            fileName = FileUpload.SaveImage(file, fileName, property.ImageOptions.NameCreation, property.ImageOptions.Settings.ToArray());

        //            property.Value.Raw = fileName;
        //        }
        //        else
        //        {
        //            // we must save file in db as byte array

        //            var file = (HttpPostedFile)property.Value.Raw;
        //            var bytes = FileUpload.GetImageByte(file, property.ImageOptions.Settings.ToArray());
        //            property.Value.Raw = bytes;
        //        }
        //    }
        //}

        public IList<GroupProperties> PrepareGroups(Entity entity, bool getKey = true, string key = null)
        {
            var properties = entity.CreateProperties(getKey);
            foreach (var foreign in properties.Where(x => x.IsForeignKey))
            {
                var records = _source.GetRecords(foreign.ForeignEntity, determineDisplayValue: true).Records;
                foreign.Value.PossibleValues = records.ToDictionary(x => x.KeyValue, x => x.DisplayName);
                if (foreign.TypeInfo.IsCollection)
                {
                    foreign.Value.Values = records.Where(x => x.Values.Any(y => y.Property.ForeignEntity == entity && y.AsString == key)).Select(x => x.KeyValue).OfType<object>().ToList();
                }
            }

            return entity.Groups;
        }

        public bool IsRecordExists(Entity entity, string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                _notificator.Error(IlaroAdminResources.EntityKeyIsNull);
                return false;
            }
            entity.Key.Value.ToObject(key);
            var existingItem = _source.GetRecord(entity, entity.Key.Value.AsObject);
            if (existingItem == null)
            {
                _notificator.Error(IlaroAdminResources.EntityNotExist);
                return false;
            }

            return true;
        }
    }
}