.PHONY: up down build logs clean test

ENV ?= Development

COMPOSE_FILE = compose.yaml

GREEN := \033[0;32m
NC := \033[0m

up:
	@echo "$(GREEN)Starting containers...$(NC)"
	docker compose up -d

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
