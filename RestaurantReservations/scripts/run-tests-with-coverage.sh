#!/bin/bash
# Script para executar testes com cobertura

echo "Executando testes com cobertura..."

dotnet test tests/RestaurantReservations.UnitTests/RestaurantReservations.UnitTests.csproj \
    --configuration Release \
    --collect:"XPlat Code Coverage" \
    --results-directory ../TestResults \
    --logger "trx;LogFileName=test-results.trx" \
    -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover

echo "Relatório de cobertura gerado em TestResults/"