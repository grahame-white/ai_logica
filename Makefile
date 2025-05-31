# Git Hooks Installation
install: .git/hooks/pre-commit .git/hooks/pre-push

.git/hooks/pre-commit: scripts/setup-git-hooks.sh
	./scripts/setup-git-hooks.sh

.git/hooks/pre-push: scripts/setup-git-hooks.sh
	./scripts/setup-git-hooks.sh

# Format and check code
format:
	dotnet format

check-format:
	./scripts/check-format.sh

# Test and build
test:
	dotnet test

build:
	dotnet build

# Clean up
clean:
	dotnet clean

.PHONY: install format check-format test build clean