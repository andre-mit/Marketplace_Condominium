using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.SharedApplication.ViewModels.AuthViewModels
{
    public record ChangePasswordRequestViewModel(string CurrentPassword, string NewPassword);
}
