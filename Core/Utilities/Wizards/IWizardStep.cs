using System;

namespace Core.Utilities.Wizards
{
    public interface IWizardStep
    {
        Type ModelType { get; }
        
        object ObtainGetMethod { get; }

        object ObtainPostMethod { get; }
    }
}
