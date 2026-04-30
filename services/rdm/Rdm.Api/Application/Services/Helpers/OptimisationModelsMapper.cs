using Rdm.Api.Application.Model;
using Rdm.Api.Inrastructure.DTOs;
using Rdm.Api.Inrastructure.Persistence.PersistenceModels;

namespace Rdm.Api.Application.Services.Helpers;

public class OptimisationModelsMapper
{
        public static OptimisationRun ToDomain(OptimisationWrapperDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            if (dto.OptimisationRun == null)
            {
                throw new ArgumentException("Optimisation run data is missing.", nameof(dto));
            }

            return new OptimisationRun
            {
                Id = dto.OptimisationRun.Id ?? 0,
                TimeFrom = dto.OptimisationRun.TimeFrom,
                TimeTo = dto.OptimisationRun.TimeTo,
                Scenario = dto.OptimisationRun.Scenario,
                PeriodType = dto.OptimisationRun.PeriodType,

                OptimisationResultsHourly = dto.OptimisationResultsHourly?
                    .Select(OptimisationResultHourlyToDomain)
                    .ToList() ?? new List<OptimisationResultsHourly>()
            };
        }

        private static OptimisationResultsHourly OptimisationResultHourlyToDomain(OptimisationResultHourlyDto dto)
        {
            return new OptimisationResultsHourly
            {
                Id = dto.Id ?? 0,
                HeatProduction = dto.HeatProduction,
                ElectricityConsumption = dto.ElectricityConsumption,
                Co2Emissions = dto.Co2Emissions,
                Expenses = dto.Expenses,
                TimeFrom = dto.TimeFrom,
                TimeTo = dto.TimeTo,

                ProductionUnits = dto.ProductionUnits?
                    .Select(ProductionUnitsToDomain)
                    .ToList() ?? new List<ProductionUnit>()
            };
        }

        private static ProductionUnit ProductionUnitsToDomain(ProductionUnitDto dto)
        {
            return new ProductionUnit
            {
                Id = dto.Id ?? 0,
                ProductionUnitId = dto.ProductionUnitId,
                ProductionUnitType = dto.ProductionUnitType,
                Capacity = dto.Capacity
            };
        }
        
    public static OptimisationRunPersistence ToPersistence(OptimisationRun domain)
    {
        if (domain == null)
        {
            throw new ArgumentNullException(nameof(domain));
        }

        return new OptimisationRunPersistence
        {
            Id = domain.Id,
            TimeFrom = domain.TimeFrom,
            TimeTo = domain.TimeTo,
            Scenario = domain.Scenario,
            Type = domain.PeriodType,

            OptimisationResultsHourly = domain.OptimisationResultsHourly?
                .Select(ToPersistence)
                .ToList() ?? new List<OptimisationResultsHourlyPersistence>()
        };
    }

    private static OptimisationResultsHourlyPersistence ToPersistence(OptimisationResultsHourly domain)
    {
        if (domain == null)
        {
            throw new ArgumentNullException(nameof(domain));
        }

        return new OptimisationResultsHourlyPersistence
        {
            Id = domain.Id,

            //This may be 0 before the ptimisationun is plugged.
            // Usually this FK is assigned after the parent row has been saved.
            OptimisationRunId = 0,

            HeatProduction = domain.HeatProduction,
            ElectricityConsumption = domain.ElectricityConsumption,
            Co2Emissions = domain.Co2Emissions,
            Expenses = domain.Expenses,
            TimeFrom = domain.TimeFrom,
            TimeTo = domain.TimeTo,

            ProductionUnits = domain.ProductionUnits?
                .Select(ToPersistence)
                .ToList() ?? new List<OptimisationProductionUnitPersistence>()
        };
    }

    private static OptimisationProductionUnitPersistence ToPersistence(ProductionUnit domain)
    {
        if (domain == null)
        {
            throw new ArgumentNullException(nameof(domain));
        }

        return new OptimisationProductionUnitPersistence
        {
            Id = domain.Id,

            //This may still be 0 before the hourly result row is inserted.
            // Usually this FK is assigned after the hourly row has been saved.
            OptimisationRunHourlyId = 0,

            ProductionUnitId = domain.ProductionUnitId,
            ProductionUnitType = domain.ProductionUnitType,
            Capacity = domain.Capacity
        };
        
    }
}