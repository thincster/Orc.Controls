﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionStringBuilderViewModel.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Controls
{
    using Catel;
    using Catel.IoC;
    using Catel.MVVM;
    using Catel.Services;

    public class ConnectionStringBuilderViewModel : ViewModelBase
    {
        private readonly IUIVisualizerService _uiVisualizerService;
        private readonly ITypeFactory _typeFactory;
        private DbProvider _dbProvider;

        public ConnectionStringBuilderViewModel(IUIVisualizerService uiVisualizerService, ITypeFactory typeFactory)
        {
            Argument.IsNotNull(() => uiVisualizerService);
            Argument.IsNotNull(() => typeFactory);

            _uiVisualizerService = uiVisualizerService;
            _typeFactory = typeFactory;

            Clear = new Command(OnClear, CanClear);
            Edit = new Command(OnEdit);
        }
        public ConnectionState ConnectionState { get; set; } = ConnectionState.Undefined;
        public string ConnectionString { get; private set; }
        public string DisplayConnectionString { get; private set; }

        public Command Edit { get; }
        public Command Test { get; }
        public Command Clear { get; }

        private void OnEdit()
        {
            var connectionStringEditViewModel = _typeFactory.CreateInstanceWithParametersAndAutoCompletion<ConnectionStringEditViewModel>(ConnectionString, _dbProvider);
            if (_uiVisualizerService.ShowDialog(connectionStringEditViewModel) ?? false)
            {
                _dbProvider = connectionStringEditViewModel.DbProvider;
                var connectionString = connectionStringEditViewModel.ConnectionString;
                
                ConnectionString = connectionString?.ToString();
                DisplayConnectionString = connectionString?.ToDisplayString();
                ConnectionState = connectionStringEditViewModel.ConnectionState;
            }
        }

        private bool CanClear()
        {
            return ConnectionString != null;
        }

        private void OnClear()
        {
            ConnectionString = null;
            DisplayConnectionString = null;
            ConnectionState = ConnectionState.Undefined;
            _dbProvider = null;
        }
    }
}