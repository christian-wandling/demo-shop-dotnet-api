.PHONY: up down build logs clean test

ENV ?= Development

COMPOSE_FILE = compose.yaml

GREEN := \033[0;32m
NC := \033[0m

up:
	@echo "$(GREEN)Starting containers...$(NC)"
	docker compose up -d

api:
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

test:
	@echo "$(GREEN)Running tests...$(NC)"
	docker compose run --rm api dotnet test

migrate_db:
	@echo "$(GREEN)Creating migration...$(NC)"
	dotnet ef migrations add --project src/DemoShop.Infrastructure

update_db:
	@echo "$(GREEN)Updating db...$(NC)"
	dotnet ef database update --project src/DemoShop.Infrastructure
