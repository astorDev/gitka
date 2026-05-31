ENV_FILE=../host/.env
PAT=$(shell grep REPOSITORY $(ENV_FILE) | sed 's/.*astorDev:\(.*\)@github.*/\1/' | cut -d';' -f1)
REPO=astorDev/gitka

test-git-push:
	@REPO_URL=$$(grep REPOSITORY $(ENV_FILE) | cut -d= -f2- | cut -d';' -f1) && \
	TMPDIR=$$(mktemp -d) && \
	GIT_TERMINAL_PROMPT=0 git -C "$$TMPDIR" clone "$$REPO_URL" . && \
	git -C "$$TMPDIR" checkout -b test-raw-push && \
	echo "test" > "$$TMPDIR/probe.txt" && \
	git -C "$$TMPDIR" -c user.email="gitka@bot.local" -c user.name="Gitka Bot" add . && \
	git -C "$$TMPDIR" -c user.email="gitka@bot.local" -c user.name="Gitka Bot" commit -m "probe" && \
	GIT_TERMINAL_PROMPT=0 git -C "$$TMPDIR" push origin test-raw-push && \
	echo "Git push OK"

test-pat:
	@echo "Testing PAT write access to $(REPO)..."
	@MAIN_SHA=$$(curl -sf -H "Authorization: token $(PAT)" \
		https://api.github.com/repos/$(REPO)/git/refs/heads/main \
		| python3 -c "import sys,json; print(json.load(sys.stdin)['object']['sha'])") && \
	RESULT=$$(curl -sf -X DELETE -H "Authorization: token $(PAT)" \
		https://api.github.com/repos/$(REPO)/git/refs/heads/pat-test 2>/dev/null; \
		curl -s -H "Authorization: token $(PAT)" \
		-d "{\"ref\": \"refs/heads/pat-test\", \"sha\": \"$$MAIN_SHA\"}" \
		https://api.github.com/repos/$(REPO)/git/refs \
		| python3 -c "import sys,json; r=json.load(sys.stdin); print(r.get('ref', 'FAIL: ' + r.get('message','unknown')))") && \
	echo "Result: $$RESULT"
