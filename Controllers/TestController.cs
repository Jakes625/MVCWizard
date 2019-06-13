using Core.Utilities.Wizards;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Project.Controllers
{
    public class TestController : Controller
    {
        public Wizard ImportWizard;
        public WizardSession WizData
        {
            get
            {
                if (Session != null)
                    return Session["WizardData"] as WizardSession;

                return null;
            }
            set
            {
                if(Session != null)
                    Session["WizardData"] = value;
            }
        }

        public TestController()
        {
            ImportWizard = new Wizard(new Dictionary<int, IWizardStep>()
            {
                {   1,
                    new WizardStep<string>
                    {
                        GetMethod = Test1,
                        PostMethod = Test1Process
                    }
                },
                {   2,
                    new WizardStep<int>
                    {
                        GetMethod = Test2,
                    }
                }
            });
        }

        [HttpGet]
        public ActionResult Test(int? Step)
        {
            if (WizData == null) WizData = new WizardSession();
            WizData.CurrentStep = Step ?? 1;
            ImportWizard.SetStep(Step ?? 1);
            return ImportWizard.Process(WizData, null);
        }

        [HttpPost]
        public ActionResult Test(WizardNavigation nav)
        {
            var Result = ImportWizard.Process(WizData, nav);

            foreach (var error in WizData.ViewErrors)
                ModelState.AddModelError("", error);

            if (ImportWizard.Complete)
                return RedirectToAction("Complete", "Test");

            return Result;
        }

        public ActionResult Test1(string Name)
        {

            return View("Test1", (object)Name);
        }

        [HttpPost]
        public void Test1Process(string Name)
        {
            if(Name != "Jake")
                WizData.ViewErrors.Add("Name must equal Jake");
        }

        public ActionResult Test2(int Age)
        {

            return View("Test2", Age);
        }

        [HttpPost]
        public void Test2Process(int Age)
        {
            if (Age < 18)
                WizData.ViewErrors.Add("Age must be over 18.");
        }

        public ActionResult Complete()
        {
            WizData = null;
            ViewBag.Data = "You have completed the wizard!";

            return View();
        }
    }
}