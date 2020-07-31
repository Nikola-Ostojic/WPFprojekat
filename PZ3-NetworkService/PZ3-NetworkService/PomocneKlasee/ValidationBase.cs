using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZ3_NetworkService.PomocneKlasee
{
    public abstract class ValidationBase : BindableBase
    {
        public ValidationErrors ValidationErrors { get; set; }

        public bool IsValid { get; private set; }

        protected ValidationBase()
        {
            ValidationErrors = new ValidationErrors();
        }

        protected abstract void ValidateSelf();

        public void Validate()
        {
            ValidationErrors.Clear();

            ValidateSelf();

            this.IsValid = this.ValidationErrors.IsValid;

            this.OnPropertyChanged("IsValid");

            this.OnPropertyChanged("ValidationErrors");
        }
    }
}
