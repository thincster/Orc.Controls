﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlConnectionString.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Controls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data.Common;
    using System.Linq;
    using Catel;
    using Catel.Collections;
    using Catel.Data;

    public class SqlConnectionString : ModelBase
    {
        private readonly DbConnectionStringBuilder _connectionStringBuilder;

        public SqlConnectionString(DbConnectionStringBuilder connectionStringBuilder, DbProvider dbProvider)
        {
            Argument.IsNotNull(() => connectionStringBuilder);
            Argument.IsNotNull(() => dbProvider);

            _connectionStringBuilder = connectionStringBuilder;
            DbProvider = dbProvider;

            UpdateProperties();
        }

        public Dictionary<string, ConnectionStringProperty> Properties { get; private set; }

        public DbProvider DbProvider { get; }

        private void UpdateProperties()
        {
            if (_connectionStringBuilder == null)
            {
                Properties = null;
                return;
            }

            var sensitiveProperties = TypeDescriptor.GetProperties(_connectionStringBuilder, new Attribute[] {PasswordPropertyTextAttribute.Yes})
                .OfType<PropertyDescriptor>()
                .Select(x => x.DisplayName);

            var sensitivePropertiesHashSet = new HashSet<string>();
            sensitivePropertiesHashSet.AddRange(sensitiveProperties);

            Properties = _connectionStringBuilder.Keys?.OfType<string>()
                .ToDictionary(x => x, x =>
                {
                    var isSensitive = sensitivePropertiesHashSet.Contains(x);
                    return new ConnectionStringProperty(x, isSensitive, _connectionStringBuilder);
                });
        }

        public virtual string ToDisplayString()
        {
            var sensitiveProperties = Properties.Values.Where(x => x.IsSensitive);

            var removedProperties = new List<Tuple<string, object>>();
            foreach (var sensitiveProperty in sensitiveProperties)
            {
                var propertyName = sensitiveProperty.Name;
                if (!_connectionStringBuilder.ShouldSerialize(propertyName))
                {
                    continue;
                }

                removedProperties.Add(new Tuple<string, object>(propertyName, _connectionStringBuilder[propertyName]));
                _connectionStringBuilder.Remove(propertyName);
            }
            var displayConnectionString = _connectionStringBuilder.ConnectionString;

            foreach (var prop in removedProperties.Where(prop => prop.Item2 != null))
            {
                _connectionStringBuilder[prop.Item1] = prop.Item2;
            }

            return displayConnectionString;
        }

        public override string ToString()
        {
            return _connectionStringBuilder.ConnectionString;
        }
    }
}