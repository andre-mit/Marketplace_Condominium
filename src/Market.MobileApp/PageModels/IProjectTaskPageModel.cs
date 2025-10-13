using CommunityToolkit.Mvvm.Input;
using Market.MobileApp.Models;

namespace Market.MobileApp.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}