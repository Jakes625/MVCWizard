using System.Collections.Generic;

namespace Core.Utilities.Wizards
{
    public interface IWizard
    {
        Dictionary<int, IWizardStep> Steps { get; }
        Dictionary<string, ICollection<string>> Errors { get; set; }
        Dictionary<string, ICollection<string>> Messages { get; set; }
        bool Succeeded { get; }
        int StepIndex { get; set; }
        int MaxSteps { get; }
        bool Complete { get; }
    }
}
