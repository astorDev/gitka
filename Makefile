feature-branch:
	git switch --create $(BRANCH)

pr:
	test "$$(git default-branch)" != "$$(git branch --show-current)" || throw "Current branch is default ($$(git branch --show-current)). This is likely a mistake, PRs should be created from a feature branch."
	git save "$(TITLE)"
	gh pr create --title "$(TITLE)" --body "" || true
	gh pr view --web

full-pr:
	test "$$(git default-branch)" == "$$(git branch --show-current)" || throw "Current branch is not default ($$(git branch --show-current)). Full PR should start by forking default branch."
	git switch --create $(BRANCH)
	git save "$(TITLE)"
	gh pr create --title "$(TITLE)" --body "" || true
	gh pr view --web