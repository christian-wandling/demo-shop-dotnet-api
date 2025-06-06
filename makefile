.PHONY: up api-up down build logs clean db-update db-migrate test integration-test test-coverage integration-test-coverage

ENV ?= Development

COMPOSE_FILE = compose.yaml

GREEN := \033[0;32m
RED := \033[0;31m
NC := \033[0m

up:
	@echo "$(GREEN)Starting containers...$(NC)"
	docker compose up -d

api-up:
	@echo "$(GREEN)Starting api container...$(NC)"
	docker compose up -d api

down:
	@echo "$(GREEN)Stopping containers...$(NC)"
	docker compose down

build:
	@echo "$(GREEN)Building containers...$(NC)"
	docker compose build --no-cache

logs:
	@echo "$(GREEN)Showing logs...$(NC)"
	docker compose logs -f

clean:
	@echo "$(GREEN)Cleaning up...$(NC)"
	docker compose down -v --remove-orphans

db-update:
	@echo "$(GREEN)Updating db...$(NC)"
	dotnet ef database update --project src/DemoShop.Infrastructure

db-migrate:
	@if [ -z "${NAME}" ]; then \
		echo "$(RED)Error: MIGRATION_NAME is required$(NC)"; \
		echo "Usage: make db-migrate MIGRATION_NAME=YourMigrationName"; \
		exit 1; \
	fi
	@echo "$(GREEN)Creating migration...$(NC)"
	dotnet ef migrations add ${NAME} --project src/DemoShop.Infrastructure

test:
	@echo "$(GREEN)Running unit tests...$(NC)"
	dotnet test --filter "Category=Unit"

integration-test:
	@echo "$(GREEN)Running Integration tests...$(NC)"
	dotnet test Tests/DemoShop.IntegrationTests/DemoShop.IntegrationTests.csproj

test-coverage:
	@echo "$(GREEN)Generating coverage report...$(NC)"
	reportgenerator -reports:"**/TestResults/coverage.cobertura.xml" -targetdir:"Tests/coveragereport" -reporttypes:Html

integration-test-coverage:
	@echo "$(GREEN)Generating coverage report...$(NC)"
	reportgenerator -reports:"Tests/DemoShop.IntegrationTests/TestResults/coverage.cobertura.xml" -targetdir:"Tests/integrationtest-coveragereport" -reporttypes:Html
