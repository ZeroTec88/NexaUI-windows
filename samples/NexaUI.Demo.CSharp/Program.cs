namespace NexaUI.Demo.CSharp;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new GalleryShell());
    }
}