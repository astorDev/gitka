save-n-pr:
	git save "$(TITLE)"
	gh pr create --title "$(TITLE)" --body ""