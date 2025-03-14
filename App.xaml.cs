namespace DocumentValidator
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
        protected override Window CreateWindow(IActivationState activationState)
        {
            var window = base.CreateWindow(activationState);
            window.Width = 800; // Set desired width
            window.Height = 800; // Set desired height
            return window;
        }
    }
}
