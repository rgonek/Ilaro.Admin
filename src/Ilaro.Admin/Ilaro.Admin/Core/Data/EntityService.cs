using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ilaro.Admin.Core.File;
using Ilaro.Admin.Extensions;
using Ilaro.Admin.Models;
using Ilaro.Admin.Validation;
using Resources;

namespace Ilaro.Admin.Core.Data
{
    public class EntityService : IEntityService
    {
        private static readonly IInternalLogger _log = LoggerProvider.LoggerFor(typeof(EntityService));

        private readonly Notificator _notificator;
        private readonly IFetchingRecords _source;
        private readonly ICreatingRecords _creator;
        private readonly IUpdatingRecords _updater;
        private readonly IDeletingRecords _deleter;
        private readonly IComparingRecords _comparer;
        private readonly IDescribingChanges _changeDescriber;
        private readonly IValidatingEntities _validator;
        private readonly IHandlingFiles _filesHandler;

        public EntityService(
            Notificator notificator,
            IFetchingRecords source,
            ICreatingRecords creator,
            IUpdatingRecords updater,
            IDeletingRecords deleter,
            IComparingRecords comparer,
            IDescribingChanges changeDescriber,
            IHandlingFiles filesHandler,
            IValidatingEntities validator)
        {
            if (notificator == null)
                throw new ArgumentNullException(nameof(notificator));
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (creator == null)
                throw new ArgumentNullException(nameof(creator));
            if (updater == null)
                throw new ArgumentNullException(nameof(updater));
            if (deleter == null)
                throw new ArgumentNullException(nameof(deleter));
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));
            if (changeDescriber == null)
                throw new ArgumentNullException(nameof(changeDescriber));
            if (filesHandler == null)
                throw new ArgumentNullException(nameof(filesHandler));
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));

            _notificator = notificator;
            _source = source;
            _creator = creator;
            _updater = updater;
            _deleter = deleter;
            _comparer = comparer;
            _changeDescriber = changeDescriber;
            _filesHandler = filesHandler;
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
            var existingRecord = _source.GetRecord(entity, entity.Key.Select(x => x.Value.AsObject));
            if (existingRecord != null)
            {
                _notificator.Error(IlaroAdminResources.EntityAlreadyExist);
                return null;
            }

            var propertiesWithUploadedFiles = _filesHandler.Upload(entity);

            var id = _creator.Create(entity, () => _changeDescriber.CreateChanges(entity));

            if (id.IsNullOrWhiteSpace() == false)
                _filesHandler.ProcessUploaded(propertiesWithUploadedFiles);
            else
                _filesHandler.DeleteUploaded(propertiesWithUploadedFiles);

            return id;
        }

        public bool Edit(
            Entity entity,
            string key,
            FormCollection collection,
            HttpFileCollectionBase files)
        {
            var existingRecord = _source.GetRecord(entity, key);
            if (existingRecord == null)
            {
                _notificator.Error(IlaroAdminResources.EntityNotExist);
                return false;
            }

            entity.Fill(key, collection, files);
            if (_validator.Validate(entity) == false)
            {
                _notificator.Error("Not valid");
                return false;
            }

            var propertiesWithUploadedFiles = _filesHandler.Upload(entity);

            _comparer.SkipNotChangedProperties(entity, existingRecord);

            var result = _updater.Update(entity, () => _changeDescriber.UpdateChanges(entity, existingRecord));

            if (result)
                _filesHandler.ProcessUploaded(propertiesWithUploadedFiles, existingRecord);
            else
                _filesHandler.DeleteUploaded(propertiesWithUploadedFiles);

            return result;
        }

        public bool Delete(Entity entity, string key, IEnumerable<PropertyDeleteOption> options)
        {
            var existingRecord = _source.GetRecord(entity, key);
            if (existingRecord == null)
            {
                _notificator.Error(IlaroAdminResources.EntityNotExist);
                return false;
            }
            entity.SetKeyValue(key);

            options = options ?? new List<PropertyDeleteOption>();
            var deleteOptions = options.ToDictionary(x => x.PropertyName, x => x.DeleteOption);
            foreach (var property in entity.GetForeignsForUpdate())
            {
                if (!deleteOptions.ContainsKey(property.ForeignEntity.Name))
                {
                    deleteOptions[property.ForeignEntity.Name] = property.DeleteOption;
                }
            }

            var result = _deleter.Delete(entity, deleteOptions, () => _changeDescriber.DeleteChanges(entity, existingRecord));

            if (result)
            {
                var propertiesWithFilesToDelete = entity
                    .GetDefaultCreateProperties(getForeignCollection: false)
                    .Where(x => x.TypeInfo.IsFile && x.TypeInfo.IsFileStoredInDb == false);
                _filesHandler.Delete(propertiesWithFilesToDelete);
            }

            return result;
        }

        public IList<GroupProperties> PrepareGroups(Entity entity, bool getKey = true, string key = null)
        {
            var properties = entity.GetDefaultCreateProperties(getKey);
            foreach (var foreign in properties.Where(x => x.IsForeignKey))
            {
                var records = _source.GetRecords(foreign.ForeignEntity, determineDisplayValue: true).Records;
                foreign.Value.PossibleValues = records.ToDictionary(x => x.JoinedKeyValue, x => x.DisplayName);
                if (foreign.TypeInfo.IsCollection)
                {
                    foreign.Value.Values = records
                        .Where(x => x.Values.Any(y => y.Property.ForeignEntity == entity && y.AsString == key))
                        .Select(x => x.JoinedKeyValue)
                        .OfType<object>()
                        .ToList();
                }
            }

            return entity.Groups;
        }

        public bool IsRecordExists(Entity entity, string key)
        {
            var keys = key.Split(Const.KeyColSeparator).Select(x => x.Trim()).ToArray();

            return IsRecordExists(entity, keys);
        }

        private bool IsRecordExists(Entity entity, params string[] key)
        {
            if (key == null || key.Length == 0 || key.All(x => string.IsNullOrWhiteSpace(x)))
            {
                _notificator.Error(IlaroAdminResources.EntityKeyIsNull);
                return false;
            }

            var keys = new object[key.Length];
            for (int i = 0; i < key.Length; i++)
            {
                keys[i] = entity.Key[i].Value.ToObject(key[i]);
            }
            var existingRecord = _source.GetRecord(entity, keys);
            if (existingRecord == null)
            {
                _notificator.Error(IlaroAdminResources.EntityNotExist);
                return false;
            }

            return true;
        }
    }
}