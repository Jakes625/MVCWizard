using System.Collections.Generic;
using System.Linq;

namespace Core.Utilities.Wizards
{
    public class WizardSession
    {
        public Dictionary<int, object> StepModels { get; private set; }
        public int CurrentStep { get; set; }

        public ICollection<string> ViewErrors { get; private set; }
        public bool ModelIsValid => !ViewErrors.Any();

        public WizardSession()
        {
            CurrentStep = 1;
            StepModels = new Dictionary<int, object>();
            ViewErrors = new List<string>();
        }
    }
}
