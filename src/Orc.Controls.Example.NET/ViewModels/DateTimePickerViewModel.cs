﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimePickerViewModel.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Controls.Examples.ViewModels
{
    using System;
    using Catel.Collections;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Threading.Tasks;
    using Catel.MVVM;
    using Catel.Threading;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Catel.Data;
    using Models;

    public class DateTimePickerViewModel : ViewModelBase
    {
        #region Constructors
        public DateTimePickerViewModel()
        {
            AvailableFormats = new FastObservableCollection<CultureFormat>();
            DateTimeValue = DateTime.Now;
            DateTimeValueString = string.Empty;
            SetNull = new Command(OnSetNullExecute);
        }
        #endregion

        #region Properties
        public DateTime? DateTimeValue { get; set; }
        public string DateTimeValueString { get; set; }
        public FastObservableCollection<CultureFormat> AvailableFormats { get; private set; }
        public CultureFormat SelectedFormat { get; set; }

        public Command SetNull { get; private set; }
        #endregion

        #region Methods
        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            using (AvailableFormats.SuspendChangeNotifications())
            {
                foreach (var cultureInfo in CultureInfo.GetCultures(CultureTypes.AllCultures))
                {
                    var format = new CultureFormat
                    {
                        CultureCode = string.Format("[{0}]", cultureInfo.IetfLanguageTag),
                        FormatValue = cultureInfo.DateTimeFormat.ShortDatePattern + " " + cultureInfo.DateTimeFormat.LongTimePattern
                    };

                    AvailableFormats.Add(format);
                    if (cultureInfo.Equals(CultureInfo.CurrentCulture))
                    {
                        SelectedFormat = format;
                    }
                }
            }
        }

        private void OnSetNullExecute()
        {
            DateTimeValue = null;
        }

        protected override void OnPropertyChanged(AdvancedPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (DateTimeValue != null && !string.IsNullOrEmpty(e.PropertyName) && e.HasPropertyChanged(e.PropertyName) && DateTimeValue.Value != null && SelectedFormat != null)
            {
                DateTimeValueString = DateTimeValue.Value.ToString(SelectedFormat.FormatValue);
            }
            else
            {
                DateTimeValueString = string.Empty;
            }
        }
        #endregion
    }
}