using ReactiveUI;

namespace QuickEndpoint.ViewModels;

public class ViewModelBase : ReactiveObject
{
    public virtual string Greeting => "Hello from Base ViewModel";
}
