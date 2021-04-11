﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ilaro.Admin.Commons.File;
using Ilaro.Admin.Commons.Extensions;
using Ilaro.Admin.Commons.Models;
using Ilaro.Admin.Commons.Validation;
using Resources;

namespace Ilaro.Admin.Commons.Data
{
    public class EntityService : IEntityService
    {
        private static readonly IInternalLogger _log = LoggerProvider.LoggerFor(typeof(EntityService));

        private readonly Notificator _notificator;
        private readonly IFetchingRecords _source;
        private readonly IRecordCreator _creator;
        private readonly IRecordUpdater _updater;
        private readonly IRecordDeleter _deleter;
        private readonly IComparingRecords _comparer;
        private readonly IChangeDescriber _changeDescriber;
        private readonly IValidatingEntities _validator;
        private readonly IHandlingFiles _filesHandler;

        public EntityService(
            Notificator notificator,
            IFetchingRecords source,
            IRecordCreator creator,
            IRecordUpdater updater,
            IRecordDeleter deleter,
            IComparingRecords comparer,
            IChangeDescriber changeDescriber,
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
            var entityRecord = entity.CreateRecord(collection, files, x => x.OnCreateDefaultValue);
            if (_validator.Validate(entityRecord) == false)
            {
                _notificator.Error(IlaroAdminResources.RecordNotValid);
                return null;
            }
            var existingRecord = _source.GetRecord(
                entity,
                entityRecord.Keys.Select(value => value.AsObject).ToArray());
            if (existingRecord != null)
            {
                _notificator.Error(IlaroAdminResources.EntityAlreadyExist);
                return null;
            }

            var propertiesWithUploadedFiles = _filesHandler.Upload(
                entityRecord,
                x => x.OnCreateDefaultValue);

            var id = _creator.Create(
                entityRecord,
                () => _changeDescriber.CreateChanges(entityRecord));

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
            HttpFileCollectionBase files,
            object concurrencyCheckValue = null)
        {
            try
            {
                var existingRecord = _source.GetRecord(entity, key);
                if (existingRecord == null)
                {
                    _notificator.Error(IlaroAdminResources.EntityNotExist);
                    return false;
                }

                var entityRecord = entity.CreateRecord(key, collection, files, x => x.OnUpdateDefaultValue);
                if (_validator.Validate(entityRecord) == false)
                {
                    _notificator.Error(IlaroAdminResources.RecordNotValid);
                    return false;
                }

                var propertiesWithUploadedFiles = _filesHandler.Upload(
                    entityRecord,
                    x => x.OnUpdateDefaultValue);

                _comparer.SkipNotChangedProperties(entityRecord, existingRecord);

                var result = false;

                try
                {
                    result = _updater.Update(
                        entityRecord,
                        concurrencyCheckValue,
                        () => _changeDescriber.UpdateChanges(entityRecord, existingRecord));
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message);
                    _notificator.Error(ex.Message);
                }

                if (result)
                    _filesHandler.ProcessUploaded(propertiesWithUploadedFiles, existingRecord);
                else
                    _filesHandler.DeleteUploaded(propertiesWithUploadedFiles);

                return result;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                _notificator.Error(ex.Message);

                return false;
            }
        }

        public bool Delete(Entity entity, string key, IEnumerable<PropertyDeleteOption> options)
        {
            var existingRecord = _source.GetRecord(entity, key);
            if (existingRecord == null)
            {
                _notificator.Error(IlaroAdminResources.EntityNotExist);
                return false;
            }
            var entityRecord = entity.CreateRecord(existingRecord);

            options = options ?? new List<PropertyDeleteOption>();
            var deleteOptions = options.ToDictionary(x => x.HierarchyName);

            var result = _deleter.Delete(
                entityRecord,
                deleteOptions,
                () => _changeDescriber.DeleteChanges(entityRecord, existingRecord));

            if (result)
            {
                var propertiesWithFilesToDelete = entityRecord.Values
                    .Where(value => value.Property.TypeInfo.IsFile && value.Property.TypeInfo.IsFileStoredInDb == false);
                _filesHandler.Delete(propertiesWithFilesToDelete);
            }

            return result;
        }

        public IList<GroupProperties> PrepareGroups(
            EntityRecord entityRecord,
            bool getKey = true,
            string key = null)
        {
            var creatableProperties = entityRecord.Entity
                .GetDefaultCreateProperties(getKey);

            var groups = entityRecord.Values
                .Where(value => creatableProperties.Contains(value.Property))
                .GroupBy(x => x.Property.Group)
                .Select(x => new GroupProperties
                {
                    GroupName = x.FirstOrDefault().Property.Group,
                    IsCollapsed = entityRecord.Entity.Groups
                        .FirstOrDefault(y => y.GroupName == x.FirstOrDefault().Property.Group).IsCollapsed,
                    PropertiesValues = x.ToList()
                });

            foreach (var foreignValue in groups.SelectMany(x => x.PropertiesValues)
                .Where(x => x.Property.IsForeignKey && x.Property.ForeignEntity != null))
            {
                var entityToLoad = GetEntityToLoad(foreignValue.Property);
                var records = _source.GetRecords(entityToLoad, determineDisplayValue: true, loadForeignKeys: foreignValue.Property.IsManyToMany).Records;
                foreignValue.PossibleValues = records.ToDictionary(x => x.JoinedKeysValues, x => x.ToString());
                if (foreignValue.Property.TypeInfo.IsCollection)
                {
                    foreignValue.Values = GetRecordValues(foreignValue.Property, key, records).ToList();
                }
            }

            return groups.ToList();
        }

        private static IEnumerable<object> GetRecordValues(Property foreignProperty, string key, IList<EntityRecord> records)
        {
            if (foreignProperty.IsManyToMany)
            {
                return records
                    .Where(x => x.Values.Any(y => y.Property.ForeignEntity == foreignProperty.ForeignEntity && y.AsString == key))
                    .Select(x => x.JoinedKeysValues);
            }

            return records
                .Where(x => x.Values.Any(y => y.Property.ForeignEntity == foreignProperty.Entity && y.AsString == key))
                .Select(x => x.JoinedKeysValues);
        }

        private static Entity GetEntityToLoad(Property foreignProperty)
        {
            if (foreignProperty.IsManyToMany)
            {
                return foreignProperty.ForeignEntity.ForeignKeys
                    .First(x => x.ForeignEntity != foreignProperty.Entity)
                    .ForeignEntity;
            }

            return foreignProperty.ForeignEntity;
        }
    }
}