namespace Dv.App.ViewModels;

using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Dv.App.Models;
using Dv.App.Services;
using Microsoft.Extensions.Logging;
using Dv.App.Interfaces;


public sealed partial class ProductionUnitsViewModel : ViewModelBase
{
   // Gets data from MaintenanceStore.cs to make it visible in UI
    public ObservableCollection<MaintenanceEvent> MaintenanceSchedules => MaintenanceStore.MaintenanceSchedules;
 }
