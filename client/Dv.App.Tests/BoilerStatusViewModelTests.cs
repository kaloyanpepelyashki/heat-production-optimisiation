using Dv.App.ViewModels;
using NUnit.Framework;

namespace Dv.App.Tests.ViewModels;

[TestFixture]
public class BoilerStatusViewModelTests
{
    [Test]
    public void Constructor_InitializesProperties()
    {
        var boiler = new BoilerStatusViewModel("GB1", "Gas", "Summer");

        Assert.That(boiler.BoilerId, Is.EqualTo("GB1"));
        Assert.That(boiler.FuelType, Is.EqualTo("Gas"));
    }

    [TestCase("GB1", "Gas")]
    [TestCase("OB1", "Oil")]
    [TestCase("GM1", "Gas")]
    [TestCase("EB1", "Electric")]
    public void Constructor_AcceptsVariousBoilerTypes(string id, string fuelType)
    {
        var boiler = new BoilerStatusViewModel(id, fuelType, "Winter");

        Assert.That(boiler.BoilerId, Is.EqualTo(id));
        Assert.That(boiler.FuelType, Is.EqualTo(fuelType));
    }

    [Test]
    public void StatusText_ReturnsCorrectString()
    {
        var boiler = new BoilerStatusViewModel("GB1", "Gas", "Summer");

        Assert.That(boiler.StatusText, Is.Not.Null);
        Assert.That(boiler.StatusText, Is.Not.Empty);
    }
}
