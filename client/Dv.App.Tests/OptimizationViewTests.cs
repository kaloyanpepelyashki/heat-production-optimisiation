using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Dv.App.Services;
using Dv.App.ViewModels;
using Dv.App.Views;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Dv.App.Tests;

[TestFixture]
public class OptimizationViewTests
{
    private Mock<IApiService> CreateMockApiService() => new();
    private Mock<ILogger<OptimizationViewModel>> CreateMockLogger() => new();

    private (Window, OptimizationView, OptimizationViewModel) CreateTestWindow()
    {
        var window = new Window { Width = 1200, Height = 800 };
        var viewModel = new OptimizationViewModel(CreateMockApiService().Object, CreateMockLogger().Object);
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
    public void ViewModel_InitializesAllFourPeriodViewModels()
    {
        var (window, _, viewModel) = CreateTestWindow();
        window.Show();

        Assert.That(viewModel.SummerScenario1, Is.Not.Null);
        Assert.That(viewModel.WinterScenario1, Is.Not.Null);
        Assert.That(viewModel.SummerScenario2, Is.Not.Null);
        Assert.That(viewModel.WinterScenario2, Is.Not.Null);

        window.Close();
    }

    [AvaloniaTest]
    public void ViewModel_AssignsCorrectScenarioNumbersToPeriodsFromField()
    {
        var (window, _, viewModel) = CreateTestWindow();
        window.Show();

        Assert.That(viewModel.SummerScenario1.Scenario, Is.EqualTo("1"));
        Assert.That(viewModel.WinterScenario1.Scenario, Is.EqualTo("1"));
        Assert.That(viewModel.SummerScenario2.Scenario, Is.EqualTo("2"));
        Assert.That(viewModel.WinterScenario2.Scenario, Is.EqualTo("2"));

        window.Close();
    }

    // Boiler Configuration - Scenario 1
    [AvaloniaTest]
    public void Scenario1Summer_PopulatesWithFourBoilers()
    {
        var (window, _, viewModel) = CreateTestWindow();
        window.Show();

        var boilers = viewModel.SummerScenario1.Boilers;
        Assert.That(boilers, Is.Not.Null);
        Assert.That(boilers.Count, Is.EqualTo(4));

        Assert.That(boilers.Count(b => b.BoilerId == "GB1"), Is.EqualTo(1));
        Assert.That(boilers.Count(b => b.BoilerId == "GB2"), Is.EqualTo(1));
        Assert.That(boilers.Count(b => b.BoilerId == "GB3"), Is.EqualTo(1));
        Assert.That(boilers.Count(b => b.BoilerId == "OB1"), Is.EqualTo(1));

        window.Close();
    }

    [AvaloniaTest]
    public void Scenario1Winter_PopulatesWithFourBoilers()
    {
        var (window, _, viewModel) = CreateTestWindow();
        window.Show();

        var boilers = viewModel.WinterScenario1.Boilers;
        Assert.That(boilers, Is.Not.Null);
        Assert.That(boilers.Count, Is.EqualTo(4));

        Assert.That(boilers.Count(b => b.BoilerId == "GB1"), Is.EqualTo(1));
        Assert.That(boilers.Count(b => b.BoilerId == "GB2"), Is.EqualTo(1));
        Assert.That(boilers.Count(b => b.BoilerId == "GB3"), Is.EqualTo(1));
        Assert.That(boilers.Count(b => b.BoilerId == "OB1"), Is.EqualTo(1));

        window.Close();
    }

    // Boiler Configuration - Scenario 2
    [AvaloniaTest]
    public void Scenario2Summer_PopulatesWithDifferentBoilerMix()
    {
        var (window, _, viewModel) = CreateTestWindow();
        window.Show();

        var boilers = viewModel.SummerScenario2.Boilers;
        Assert.That(boilers, Is.Not.Null);
        Assert.That(boilers.Count, Is.EqualTo(4));

        Assert.That(boilers.Count(b => b.BoilerId == "GB1"), Is.EqualTo(1));
        Assert.That(boilers.Count(b => b.BoilerId == "GB2"), Is.EqualTo(1));
        Assert.That(boilers.Count(b => b.BoilerId == "GM1"), Is.EqualTo(1));
        Assert.That(boilers.Count(b => b.BoilerId == "EB1"), Is.EqualTo(1));

        window.Close();
    }

    [AvaloniaTest]
    public void Scenario2Winter_PopulatesWithDifferentBoilerMix()
    {
        var (window, _, viewModel) = CreateTestWindow();
        window.Show();

        var boilers = viewModel.WinterScenario2.Boilers;
        Assert.That(boilers, Is.Not.Null);
        Assert.That(boilers.Count, Is.EqualTo(4));

        Assert.That(boilers.Count(b => b.BoilerId == "GB1"), Is.EqualTo(1));
        Assert.That(boilers.Count(b => b.BoilerId == "GB2"), Is.EqualTo(1));
        Assert.That(boilers.Count(b => b.BoilerId == "GM1"), Is.EqualTo(1));
        Assert.That(boilers.Count(b => b.BoilerId == "EB1"), Is.EqualTo(1));

        window.Close();
    }

    [AvaloniaTest]
    public void BoilerSelection_StartsAsNull()
    {
        var (window, _, viewModel) = CreateTestWindow();
        window.Show();

        Assert.That(viewModel.SummerScenario1.SelectedBoiler, Is.Null);
        Assert.That(viewModel.SummerScenario2.SelectedBoiler, Is.Null);
        Assert.That(viewModel.WinterScenario1.SelectedBoiler, Is.Null);
        Assert.That(viewModel.WinterScenario2.SelectedBoiler, Is.Null);

        window.Close();
    }

    [AvaloniaTest]
    public void BoilerSelection_CanSelectBoiler()
    {
        var (window, _, viewModel) = CreateTestWindow();
        window.Show();

        var scenario = viewModel.SummerScenario1;
        var targetBoiler = scenario.Boilers[0];

        scenario.SelectedBoiler = targetBoiler;

        Assert.That(scenario.SelectedBoiler, Is.Not.Null);
        Assert.That(scenario.SelectedBoiler.BoilerId, Is.EqualTo("GB1"));
        Assert.That(scenario.SelectedBoiler, Is.SameAs(targetBoiler));

        window.Close();
    }

    [AvaloniaTest]
    public void BoilerSelection_CanChangeSelection()
    {
        var (window, _, viewModel) = CreateTestWindow();
        window.Show();

        var scenario = viewModel.SummerScenario1;
        scenario.SelectedBoiler = scenario.Boilers[0];
        Assert.That(scenario.SelectedBoiler.BoilerId, Is.EqualTo("GB1"));

        scenario.SelectedBoiler = scenario.Boilers[2];
        Assert.That(scenario.SelectedBoiler.BoilerId, Is.EqualTo("GB3"));

        window.Close();
    }

    [AvaloniaTest]
    public void BoilerSelection_CanClearSelection()
    {
        var (window, _, viewModel) = CreateTestWindow();
        window.Show();

        var scenario = viewModel.SummerScenario1;
        scenario.SelectedBoiler = scenario.Boilers[0];
        Assert.That(scenario.SelectedBoiler, Is.Not.Null);

        scenario.SelectedBoiler = null;
        Assert.That(scenario.SelectedBoiler, Is.Null);

        window.Close();
    }

    [AvaloniaTest]
    public void MaintenanceStartHour_DefaultsTo12()
    {
        var (window, _, viewModel) = CreateTestWindow();
        window.Show();

        Assert.That(viewModel.SummerScenario1.MaintenanceStartHour, Is.EqualTo(12));
        Assert.That(viewModel.SummerScenario2.MaintenanceStartHour, Is.EqualTo(12));
        Assert.That(viewModel.WinterScenario1.MaintenanceStartHour, Is.EqualTo(12));
        Assert.That(viewModel.WinterScenario2.MaintenanceStartHour, Is.EqualTo(12));

        window.Close();
    }

    [AvaloniaTest]
    public void MaintenanceStartHour_CanBeModified()
    {
        var (window, _, viewModel) = CreateTestWindow();
        window.Show();

        var scenario = viewModel.SummerScenario1;
        scenario.MaintenanceStartHour = 15;

        Assert.That(scenario.MaintenanceStartHour, Is.EqualTo(15));

        window.Close();
    }

    [AvaloniaTest]
    public void MaintenanceDuration_DefaultsTo30Minutes()
    {
        var (window, _, viewModel) = CreateTestWindow();
        window.Show();

        Assert.That(viewModel.SummerScenario1.MaintenanceDuration, Is.EqualTo(30));
        Assert.That(viewModel.SummerScenario2.MaintenanceDuration, Is.EqualTo(30));
        Assert.That(viewModel.WinterScenario1.MaintenanceDuration, Is.EqualTo(30));
        Assert.That(viewModel.WinterScenario2.MaintenanceDuration, Is.EqualTo(30));

        window.Close();
    }

    [AvaloniaTest]
    public void MaintenanceDuration_CanBeModified()
    {
        var (window, _, viewModel) = CreateTestWindow();
        window.Show();

        var scenario = viewModel.SummerScenario1;
        scenario.MaintenanceDuration = 45;

        Assert.That(scenario.MaintenanceDuration, Is.EqualTo(45));

        window.Close();
    }

    [AvaloniaTest]
    public void SummerPeriods_HaveCorrectDateRange()
    {
        var (window, _, viewModel) = CreateTestWindow();
        window.Show();

        var summerStart = new DateTime(2025, 9, 8, 0, 0, 0);
        var summerEnd = new DateTime(2025, 9, 21, 23, 59, 59);

        Assert.That(viewModel.SummerScenario1.PeriodStart, Is.EqualTo(summerStart));
        Assert.That(viewModel.SummerScenario1.PeriodEnd, Is.EqualTo(summerEnd));
        Assert.That(viewModel.SummerScenario2.PeriodStart, Is.EqualTo(summerStart));
        Assert.That(viewModel.SummerScenario2.PeriodEnd, Is.EqualTo(summerEnd));

        window.Close();
    }

    [AvaloniaTest]
    public void WinterPeriods_HaveCorrectDateRange()
    {
        var (window, _, viewModel) = CreateTestWindow();
        window.Show();

        var winterStart = new DateTime(2026, 1, 5, 0, 0, 0);
        var winterEnd = new DateTime(2026, 1, 18, 23, 59, 59);

        Assert.That(viewModel.WinterScenario1.PeriodStart, Is.EqualTo(winterStart));
        Assert.That(viewModel.WinterScenario1.PeriodEnd, Is.EqualTo(winterEnd));
        Assert.That(viewModel.WinterScenario2.PeriodStart, Is.EqualTo(winterStart));
        Assert.That(viewModel.WinterScenario2.PeriodEnd, Is.EqualTo(winterEnd));

        window.Close();
    }

    [AvaloniaTest]
    public void Periods_HaveCorrectSeasonNames()
    {
        var (window, _, viewModel) = CreateTestWindow();
        window.Show();

        Assert.That(viewModel.SummerScenario1.PeriodName, Is.EqualTo("Summer"));
        Assert.That(viewModel.WinterScenario1.PeriodName, Is.EqualTo("Winter"));
        Assert.That(viewModel.SummerScenario2.PeriodName, Is.EqualTo("Summer"));
        Assert.That(viewModel.WinterScenario2.PeriodName, Is.EqualTo("Winter"));

        window.Close();
    }

    [AvaloniaTest]
    public void ScenarioStates_RemainIndependent()
    {
        var (window, _, viewModel) = CreateTestWindow();
        window.Show();

        // Modify Scenario 1 Summer
        viewModel.SummerScenario1.MaintenanceStartHour = 14;
        viewModel.SummerScenario1.MaintenanceDuration = 60;
        viewModel.SummerScenario1.SelectedBoiler = viewModel.SummerScenario1.Boilers[0];

        // Verify Scenario 2 Summer is isolated
        Assert.That(viewModel.SummerScenario2.MaintenanceStartHour, Is.EqualTo(12));
        Assert.That(viewModel.SummerScenario2.MaintenanceDuration, Is.EqualTo(30));
        Assert.That(viewModel.SummerScenario2.SelectedBoiler, Is.Null);

        // Verify Winter scenarios are isolated
        Assert.That(viewModel.WinterScenario1.MaintenanceStartHour, Is.EqualTo(12));
        Assert.That(viewModel.WinterScenario2.MaintenanceStartHour, Is.EqualTo(12));

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

        viewModel1.SummerScenario1.MaintenanceStartHour = 15;
        viewModel1.SummerScenario1.MaintenanceDuration = 50;

        Assert.That(viewModel2.SummerScenario1.MaintenanceStartHour, Is.EqualTo(12));
        Assert.That(viewModel2.SummerScenario1.MaintenanceDuration, Is.EqualTo(30));

        window1.Close();
        window2.Close();
    }
}