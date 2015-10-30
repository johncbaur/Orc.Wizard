﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WizardViewModel.cs" company="Wild Gums">
//   Copyright (c) 2013 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Wizard.ViewModels
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Catel;
    using Catel.MVVM;
    using Catel.Services;
    using Catel.Windows;

    public class WizardViewModel : ViewModelBase
    {
        private readonly IMessageService _messageService;
        private readonly ILanguageService _languageService;

        #region Constructors
        public WizardViewModel(IWizard wizard, IMessageService messageService, ILanguageService languageService)
        {
            Argument.IsNotNull(() => wizard);
            Argument.IsNotNull(() => messageService);
            Argument.IsNotNull(() => languageService);

            Wizard = wizard;
            _messageService = messageService;
            _languageService = languageService;

            Finish = new TaskCommand(OnFinishExecuteAsync, OnFinishCanExecute);
            Cancel = new TaskCommand(OnCancelExecuteAsync, OnCancelCanExecuteAsync);
            GoToNext = new TaskCommand(OnGoToNextExecuteAsync, OnGoToNextCanExecute);
            GoToPrevious = new TaskCommand(OnGoToPreviousExecuteAsync, OnGoToPreviousCanExecute);
        }
        #endregion

        #region Properties
        public override string Title
        {
            get { return Wizard.Title; }
        }

        [Model]
        public IWizard Wizard { get; set; }

        // TODO: take the value from Wizard.CurrentPage.Header
        public string PageHeader { get; private set; }
        #endregion

        #region Commands
        public TaskCommand GoToPrevious { get; set; }

        private Task OnGoToPreviousExecuteAsync()
        {
            return Wizard.MoveBackAsync();
        }

        private bool OnGoToPreviousCanExecute()
        {
            return Wizard.CanMoveBack;
        }

        public TaskCommand GoToNext { get; set; }

        private Task OnGoToNextExecuteAsync()
        {
            return Wizard.MoveForwardAsync();
        }

        private bool OnGoToNextCanExecute()
        {
            return Wizard.CanMoveForward;
        }

        public TaskCommand Finish { get; set; }

        private async Task OnFinishExecuteAsync()
        {
            if (await SaveAsync())
            {
                await Wizard.ResumeAsync();
                await CloseViewModelAsync(true);
            }
        }

        private bool OnFinishCanExecute()
        {
            var validationSummary = this.GetValidationSummary(true);
            return !validationSummary.HasErrors && !validationSummary.HasWarnings && Wizard.CanResume;
        }

        public new TaskCommand Cancel { get; set; }

        private async Task OnCancelExecuteAsync()
        {
            if (await _messageService.ShowAsync(_languageService.GetString("AreYouSureYouWantToCancelWizard"), button: MessageButton.YesNo) == MessageResult.No)
            {
                return;
            }

            if (await CancelAsync())
            {
                await Wizard.CancelAsync();
                await CloseViewModelAsync(true);
            }
        }

        private bool OnCancelCanExecuteAsync()
        {
            return Wizard.CanCancel;
        }
        #endregion
    }
}