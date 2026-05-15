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
                Expenses = dto.Expenses,
                Co2Emissions = dto.Co2Emissions,
                HeatProduction = dto.HeatProduction,
                ElectricityConsumption = dto.ElectricityConsumption,
                Capacity = dto.Capacity
            };
        }
        
        public static OptimisationRunPersistenceWrapper ToPersistenceWrapper(OptimisationRun domain)
        {
            if (domain == null)
            {
                throw new ArgumentNullException(nameof(domain));
            }

            return new OptimisationRunPersistenceWrapper
            {
                OptimisationRunPersistence = new OptimisationRunPersistence
                {
                    Id = domain.Id,
                    TimeFrom = domain.TimeFrom,
                    TimeTo = domain.TimeTo,
                    Scenario = domain.Scenario,
                    Type = domain.PeriodType
                },

                OptimisationResultsHourlyPersistence = domain.OptimisationResultsHourly?
                    .Select(hourly => new OptimisationResultsHourlyPersistenceWrapper
                    {
                        HourlyResult = ToPersistence(hourly),

                        ProductionUnitsPersistence = hourly.ProductionUnits?
                            .Select(ToPersistence)
                            .ToList() ?? new List<OptimisationProductionUnitPersistence>()
                    })
                    .ToList() ?? new List<OptimisationResultsHourlyPersistenceWrapper>()
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
            OptimisationRunId = 0,
            HeatProduction = domain.HeatProduction,
            ElectricityConsumption = domain.ElectricityConsumption,
            Co2Emissions = domain.Co2Emissions,
            Expenses = domain.Expenses,
            TimeFrom = domain.TimeFrom,
            TimeTo = domain.TimeTo
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
            OptimisationRunHourlyId = 0,
            ProductionUnitId = domain.ProductionUnitId,
            ProductionUnitType = domain.ProductionUnitType,
            Expenses = domain.Expenses,
            Co2Emissions = domain.Co2Emissions,
            HeatProduction = domain.HeatProduction,
            ElectricityConsumption = domain.ElectricityConsumption,
            Capacity = domain.Capacity
        };
    }
}