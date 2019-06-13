using Core.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Core.Utilities.Wizards
{
    public class Wizard : IWizard
    {
        public Dictionary<int, IWizardStep> Steps { get; private set; }

        public Dictionary<string, ICollection<string>> Errors { get; set; }

        public Dictionary<string, ICollection<string>> Messages { get; set; }

        public bool Succeeded => !Errors.Any();

        public int MaxSteps { get; private set; }

        public int StepIndex { get; set; } = 1;

        public bool Complete { get; private set; }

        private readonly HttpRequest Request;

        public Wizard(Dictionary<int, IWizardStep> WizardSteps)
        {
            Steps = WizardSteps;
            MaxSteps = WizardSteps.Keys.Count();
            Complete = false;

            Request = HttpContext.Current.Request;
        }

        #region Navigation
        public void SetStep(int step)
        {
            StepIndex = step;

            if (StepIndex < 1)
                StepIndex = 1;

            if (StepIndex > MaxSteps)
                StepIndex = MaxSteps;
        }

        public void NextStep()
        {
            StepIndex++;

            if (StepIndex > MaxSteps)
            {
                StepIndex = MaxSteps;
                Complete = true;
            }
        }

        public void PrevStep()
        {
            StepIndex--;

            if (StepIndex < 1)
                StepIndex = 1;
        }
        #endregion

        #region AssistingMethods
        private IWizardStep GetStep()
        {
            return GetStep(StepIndex);
        }

        private IWizardStep GetStep(int index)
        {
            if (Steps.Keys.Contains(index))
                return Steps[index];

            return null;
        }

        public Type GetStepType()
        {
            int index = StepIndex;
            return GetStepType(index);
        }

        public Type GetStepType(int index)
        {
            var Step = GetStep(index);
            return Step?.ModelType;
        }

        public string[] GetMethodParameters()
        {
            var Step = GetStep();
            var method = Step.ObtainGetMethod as Delegate;
            MethodInfo methodInfo = method.Method;
            return methodInfo.GetParameters().Select(pi => pi.Name).ToArray();
        }
        #endregion

        #region ActionMethods
        public void RunCurrentService(object model)
        {
            var Step = GetStep();
            var method = Step.ObtainPostMethod as Delegate;

            if (method != null)
            {
                if (model?.GetType() == Step.ModelType || model == null)
                    method.DynamicInvoke(model); //Run only if method exists and model types match.
            }
        }

        public ActionResult CurrentView(object model)
        {
            var Step = GetStep();
            var method = Step.ObtainGetMethod as Delegate;

            if (method != null)
            {
                if (model?.GetType() == Step.ModelType || model == null)
                    return (ActionResult)method.DynamicInvoke(model);
            }

            return null;
        }

        public ActionResult Process(WizardSession wizardSession, WizardNavigation navigation = null)
        {
            if (Request == null)
                throw new NullReferenceException("The value \"null\" was found where an instance of an object is required.", new Exception("HttpRequest object required for Wizard functionality."));

            wizardSession?.ViewErrors.Clear();

            SetStep(wizardSession?.CurrentStep ?? 1);
            object model;
            string key = GetMethodParameters().FirstOrDefault();

            if (navigation?.GoNext ?? false)
            {
                model = Request.Form.CastForm(GetStepType(), key);
                wizardSession.StepModels[StepIndex] = model;

                RunCurrentService(model);
            }
            
            if(wizardSession?.ModelIsValid ?? false)
            {
                //navigation
                if (navigation?.GoNext ?? false)
                    NextStep();
                if (navigation?.GoBack ?? false)
                    PrevStep();

                key = GetMethodParameters().FirstOrDefault();
                wizardSession.CurrentStep = StepIndex;
            }

            if ((!navigation?.GoNext ?? false) && wizardSession.StepModels.ContainsKey(StepIndex))
            {
                object prevModel = null;
                wizardSession.StepModels.TryGetValue(StepIndex, out prevModel);
                return CurrentView(prevModel); //nav, may want to include this for info on view...
            }

            model = Request.Form.CastForm(GetStepType(), key);
            return CurrentView(model); //nav, may want to include this for info on view...
        }
        #endregion
    }
}
