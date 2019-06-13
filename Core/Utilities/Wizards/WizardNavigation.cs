namespace Core.Utilities.Wizards
{
    public class WizardNavigation
    {
        public char? StepMove { get; set; }

        public bool GoNext => StepMove != null && StepMove == '+';

        public bool GoBack => StepMove != null && StepMove == '-';
    }
}
