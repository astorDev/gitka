feature-branch:
	git switch --create $(BRANCH)

pr:
	git save "$(TITLE)"
	gh pr create --title "$(TITLE)" --body "" || true
	gh pr view --web

post-pr:
	sh ./small/switch-and-delete/.sh
	git pull