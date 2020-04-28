﻿namespace Orc.Wizard
{
    using System.Threading.Tasks;
    using Catel;

    public static class IWizardPageExtensions
    {
        public static Task MoveNextOrFinishAsync(this IWizardPage wizardPage)
        {
            Argument.IsNotNull(() => wizardPage);

            return wizardPage.Wizard.MoveNextOrResumeAsync();
        }
    }
}
