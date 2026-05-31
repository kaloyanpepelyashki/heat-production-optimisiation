using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Dv.App.Interfaces;
using Dv.App.Services;
using Dv.App.ViewModels;
using Dv.App.Views;
using Moq;
using NUnit.Framework;

namespace Dv.App.Tests;

[TestFixture]
public class OptimizationViewTests
{
    private Mock<IApiService> CreateMockApiService() => new();
    private Mock<IDialogService> CreateMockDialogService() => new();

    private (Window, OptimizationView, OptimizationViewModel) CreateTestWindow()
    {
        var window = new Window { Width = 1200, Height = 800 };
        var mockApiService = CreateMockApiService();
        var mockDialogService = CreateMockDialogService();
        var maintenanceService = new MaintenanceService(mockApiService.Object);
        var viewModel = new OptimizationViewModel(
            mockApiService.Object,
            maintenanceService,
            mockDialogService.Object);
        var view = new OptimizationView { DataContext = viewModel };
        window.Content = view;
        return (window, view, viewModel);
    }

    [AvaloniaTest]
    public void View_Initializes_Successfully()
    {
        var (window, view, _) = CreateTestWindow();
        window.Show();

        Assert.That(view, Is.Not.Null);
        Assert.That(view.DataContext, Is.Not.Null);
        Assert.That(window.IsVisible, Is.True);

        window.Close();
    }

    [AvaloniaTest]
    public void View_BindsDataContext_ToOptimizationViewModel()
    {
        var (window, view, viewModel) = CreateTestWindow();
        window.Show();

        Assert.That(view.DataContext, Is.InstanceOf<OptimizationViewModel>());
        Assert.That(view.DataContext, Is.SameAs(viewModel));

        window.Close();
    }

    [AvaloniaTest]
    public void ViewModel_Initializes_WithCorrectComponents()
    {
        var (window, _, viewModel) = CreateTestWindow();
        window.Show();

        Assert.That(viewModel.Periods, Is.Not.Null);
        Assert.That(viewModel.Scenarios, Is.Not.Null);
        Assert.That(viewModel.CurrentContext, Is.Not.Null);
        Assert.That(viewModel.Maintenance, Is.Not.Null);
        Assert.That(viewModel.ChartsVM, Is.Not.Null);

        window.Close();
    }

    [AvaloniaTest]
    public void ViewModel_Periods_ContainsExpectedValues()
    {
        var (window, _, viewModel) = CreateTestWindow();
        window.Show();

        Assert.That(viewModel.Periods.Count, Is.EqualTo(2));
        Assert.That(viewModel.Periods, Does.Contain("Summer"));
        Assert.That(viewModel.Periods, Does.Contain("Winter"));

        window.Close();
    }

    [AvaloniaTest]
    public void ViewModel_Scenarios_ContainsExpectedValues()
    {
        var (window, _, viewModel) = CreateTestWindow();
        window.Show();

        Assert.That(viewModel.Scenarios.Count, Is.EqualTo(2));
        Assert.That(viewModel.Scenarios, Does.Contain("Scenario 1"));
        Assert.That(viewModel.Scenarios, Does.Contain("Scenario 2"));

        window.Close();
    }

    [AvaloniaTest]
    public void CurrentContext_Scenario1_HasFourBoilers()
    {
        var (window, _, viewModel) = CreateTestWindow();
        window.Show();

        viewModel.SelectedScenario = "Scenario 1";

        var boilers = viewModel.CurrentContext.Boilers;
        Assert.That(boilers.Count, Is.EqualTo(4));
        Assert.That(boilers[0].BoilerId, Is.EqualTo("GB1"));
        Assert.That(boilers[1].BoilerId, Is.EqualTo("GB2"));
        Assert.That(boilers[2].BoilerId, Is.EqualTo("GB3"));
        Assert.That(boilers[3].BoilerId, Is.EqualTo("OB1"));

        window.Close();
    }

    [AvaloniaTest]
    public void CurrentContext_Scenario1_BoilersHaveCorrectFuelTypes()
    {
        var (window, _, viewModel) = CreateTestWindow();
        window.Show();

        viewModel.SelectedScenario = "Scenario 1";

        var boilers = viewModel.CurrentContext.Boilers;
        Assert.That(boilers[0].FuelType, Is.EqualTo("Gas"));
        Assert.That(boilers[1].FuelType, Is.EqualTo("Gas"));
        Assert.That(boilers[2].FuelType, Is.EqualTo("Gas"));
        Assert.That(boilers[3].FuelType, Is.EqualTo("Oil"));

        window.Close();
    }

    [AvaloniaTest]
    public void CurrentContext_Scenario2_HasFourBoilersWithDifferentMix()
    {
        var (window, _, viewModel) = CreateTestWindow();
        window.Show();

        viewModel.SelectedScenario = "Scenario 2";

        var boilers = viewModel.CurrentContext.Boilers;
        Assert.That(boilers.Count, Is.EqualTo(4));
        Assert.That(boilers[0].BoilerId, Is.EqualTo("GB1"));
        Assert.That(boilers[1].BoilerId, Is.EqualTo("GB3"));
        Assert.That(boilers[2].BoilerId, Is.EqualTo("GM1"));
        Assert.That(boilers[3].BoilerId, Is.EqualTo("EB1"));

        window.Close();
    }

    [AvaloniaTest]
    public void CurrentContext_Scenario2_BoilersHaveCorrectFuelTypes()
    {
        var (window, _, viewModel) = CreateTestWindow();
        window.Show();

        viewModel.SelectedScenario = "Scenario 2";

        var boilers = viewModel.CurrentContext.Boilers;
        Assert.That(boilers[0].FuelType, Is.EqualTo("Gas"));
        Assert.That(boilers[1].FuelType, Is.EqualTo("Gas"));
        Assert.That(boilers[2].FuelType, Is.EqualTo("Gas"));
        Assert.That(boilers[3].FuelType, Is.EqualTo("Electric"));

        window.Close();
    }

    [AvaloniaTest]
    public void SelectedPeriod_DefaultsToWinter()
    {
        var (window, _, viewModel) = CreateTestWindow();
        window.Show();

        Assert.That(viewModel.SelectedPeriod, Is.EqualTo("Winter"));

        window.Close();
    }

    [AvaloniaTest]
    public void SelectedScenario_DefaultsToScenario2()
    {
        var (window, _, viewModel) = CreateTestWindow();
        window.Show();

        Assert.That(viewModel.SelectedScenario, Is.EqualTo("Scenario 2"));

        window.Close();
    }

    [AvaloniaTest]
    public void WinterPeriod_HasCorrectDateRange()
    {
        var (window, _, viewModel) = CreateTestWindow();
        window.Show();

        viewModel.SelectedPeriod = "Winter";

        Assert.That(viewModel.CurrentContext.StartDate, Is.EqualTo(new DateTime(2026, 1, 5, 0, 0, 0)));
        Assert.That(viewModel.CurrentContext.EndDate, Is.EqualTo(new DateTime(2026, 1, 19, 0, 0, 0)));

        window.Close();
    }

    [AvaloniaTest]
    public void SummerPeriod_HasCorrectDateRange()
    {
        var (window, _, viewModel) = CreateTestWindow();
        window.Show();

        viewModel.SelectedPeriod = "Summer";

        Assert.That(viewModel.CurrentContext.StartDate, Is.EqualTo(new DateTime(2025, 9, 8, 0, 0, 0)));
        Assert.That(viewModel.CurrentContext.EndDate, Is.EqualTo(new DateTime(2025, 9, 21, 23, 59, 59)));

        window.Close();
    }

    [AvaloniaTest]
    public void PeriodChange_UpdatesCurrentContext()
    {
        var (window, _, viewModel) = CreateTestWindow();
        window.Show();

        var initialPeriod = viewModel.CurrentContext.Period;
        viewModel.SelectedPeriod = "Summer";

        Assert.That(viewModel.CurrentContext.Period, Is.EqualTo("Summer"));
        Assert.That(viewModel.CurrentContext.Period, Is.Not.EqualTo(initialPeriod));

        window.Close();
    }

    [AvaloniaTest]
    public void ScenarioChange_UpdatesCurrentContext()
    {
        var (window, _, viewModel) = CreateTestWindow();
        window.Show();

        var initialScenario = viewModel.CurrentContext.Scenario;
        viewModel.SelectedScenario = "Scenario 1";

        Assert.That(viewModel.CurrentContext.Scenario, Is.EqualTo("Scenario 1"));
        Assert.That(viewModel.CurrentContext.Scenario, Is.Not.EqualTo(initialScenario));

        window.Close();
    }

    [AvaloniaTest]
    public void CurrentContextPeriodId_ReturnsCorrectIdForWinter()
    {
        var (window, _, viewModel) = CreateTestWindow();
        window.Show();

        viewModel.SelectedPeriod = "Winter";

        Assert.That(viewModel.CurrentContext.PeriodId, Is.EqualTo(2));

        window.Close();
    }

    [AvaloniaTest]
    public void CurrentContextPeriodId_ReturnsCorrectIdForSummer()
    {
        var (window, _, viewModel) = CreateTestWindow();
        window.Show();

        viewModel.SelectedPeriod = "Summer";

        Assert.That(viewModel.CurrentContext.PeriodId, Is.EqualTo(1));

        window.Close();
    }

    [AvaloniaTest]
    public void CurrentContextScenarioId_ReturnsCorrectIdForScenario1()
    {
        var (window, _, viewModel) = CreateTestWindow();
        window.Show();

        viewModel.SelectedScenario = "Scenario 1";

        Assert.That(viewModel.CurrentContext.ScenarioId, Is.EqualTo(1));

        window.Close();
    }

    [AvaloniaTest]
    public void CurrentContextScenarioId_ReturnsCorrectIdForScenario2()
    {
        var (window, _, viewModel) = CreateTestWindow();
        window.Show();

        viewModel.SelectedScenario = "Scenario 2";

        Assert.That(viewModel.CurrentContext.ScenarioId, Is.EqualTo(2));

        window.Close();
    }

    [AvaloniaTest]
    public void IsLoading_DefaultsToFalse()
    {
        var (window, _, viewModel) = CreateTestWindow();
        window.Show();

        Assert.That(viewModel.IsLoading, Is.False);

        window.Close();
    }

    [AvaloniaTest]
    public void HasOptimizationResults_DefaultsToFalse()
    {
        var (window, _, viewModel) = CreateTestWindow();
        window.Show();

        Assert.That(viewModel.HasOptimizationResults, Is.False);

        window.Close();
    }

    [AvaloniaTest]
    public void ErrorMessage_DefaultsToNull()
    {
        var (window, _, viewModel) = CreateTestWindow();
        window.Show();

        Assert.That(viewModel.ErrorMessage, Is.Null);

        window.Close();
    }

    [AvaloniaTest]
    public void WindowClose_IsSuccessful()
    {
        var (window, _, _) = CreateTestWindow();
        window.Show();

        Assert.That(window.IsVisible, Is.True);

        window.Close();

        Assert.That(window.IsVisible, Is.False);
    }

    [AvaloniaTest]
    public void MultipleViewInstances_DoNotShareState()
    {
        var (window1, _, viewModel1) = CreateTestWindow();
        var (window2, _, viewModel2) = CreateTestWindow();

        window1.Show();
        window2.Show();

        viewModel1.SelectedPeriod = "Summer";
        viewModel1.SelectedScenario = "Scenario 1";

        Assert.That(viewModel2.SelectedPeriod, Is.EqualTo("Winter"));
        Assert.That(viewModel2.SelectedScenario, Is.EqualTo("Scenario 2"));

        window1.Close();
        window2.Close();
    }
}
