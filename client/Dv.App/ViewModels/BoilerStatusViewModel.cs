namespace Dv.App.ViewModels;

using Avalonia.Media;

// sealed to not extend this class, it is done as it is
public sealed class BoilerStatusViewModel : ViewModelBase
{
    private static readonly SolidColorBrush AvailableBrush = new(Color.Parse("#10B981"));
    private static readonly SolidColorBrush UnavailableBrush = new(Color.Parse("#EF4444"));
    private bool isUnavailable;

    public BoilerStatusViewModel(string boilerId, string fuelType, string period)
    {
        this.BoilerId = boilerId;
        this.FuelType = fuelType;
        this.Period = period;
    }

    public string BoilerId { get; }

    public string FuelType { get; }

    public string Period { get; }

    // Active or Unavailable, changes with colour here
    public bool IsUnavailable
    {
        get => this.isUnavailable;
        private set
        {
            if (this.SetProperty(ref this.isUnavailable, value))
            {
                this.OnPropertyChanged(nameof(this.StatusText));
                this.OnPropertyChanged(nameof(this.StatusBrush));
            }
        }
    }

    public string StatusText => this.IsUnavailable ? "Unavailable" : "Active";

    public IBrush StatusBrush => this.IsUnavailable ? UnavailableBrush : AvailableBrush;

    public void SetUnavailable(bool unavailable)
    {
        this.IsUnavailable = unavailable;
    }
}
