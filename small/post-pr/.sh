source /dev/stdin <<< "$(curl -sS https://raw.githubusercontent.com/astorDev/nice-shell/refs/heads/main/.sh)"

log "Switching to default branch and deleting current branch: git default-and-burn"
git default-and-burn

log "Switched to $(git default-branch). Pulling changes from the merged PR: git pull"
git pull