namespace Dv.App.ViewModels;

using Avalonia.Media;

public sealed class BoilerStatusViewModel : ViewModelBase
{
    private static readonly SolidColorBrush AvailableBrush = new(Color.Parse("#10B981"));
    private static readonly SolidColorBrush UnavailableBrush = new(Color.Parse("#F97316"));
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

    public string StatusText => this.IsUnavailable ? "In Maintenance" : "Active";

    public IBrush StatusBrush => this.IsUnavailable ? UnavailableBrush : AvailableBrush;

    public void SetUnavailable(bool unavailable)
    {
        this.IsUnavailable = unavailable;
    }
}
