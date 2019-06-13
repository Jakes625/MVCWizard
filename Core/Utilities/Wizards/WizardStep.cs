using System;
using System.Web.Mvc;

namespace Core.Utilities.Wizards
{
    public class WizardStep<T> : IWizardStep
    {
        public Func<T, ActionResult> GetMethod { get; set; }

        public Action<T> PostMethod { get; set; }

        public Type ModelType { get; private set; }

        public object ObtainGetMethod => GetMethod;

        public object ObtainPostMethod => PostMethod;

        public WizardStep()
        {
            ModelType = typeof(T);
        }
    }
}
