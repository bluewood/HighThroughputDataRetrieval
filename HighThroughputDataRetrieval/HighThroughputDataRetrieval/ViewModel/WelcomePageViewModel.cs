using HighThroughputDataRetrievalBackend.Model;

namespace HighThroughputDataRetrieval.ViewModel
{
    /// <summary>
    /// The first wizard page in the workflow.  
    /// This page has no functionality.
    /// </summary>
    class WelcomePageViewModel : InputWizardViewModelBase
    {
        public WelcomePageViewModel(UserInput input)
            : base(input)
        {
        }

        public override string DisplayName
        {
            get { return "welcome"; }
        }

        internal override bool IsValid()
        {
            return true;
        }
    }
}
