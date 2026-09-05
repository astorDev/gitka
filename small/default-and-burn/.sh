source /dev/stdin <<< "$(curl -sS https://raw.githubusercontent.com/astorDev/nice-shell/refs/heads/main/.sh)"

log "Saving current branch name branch=\$(git branch --show-current)"
branch=$(git branch --show-current)

log "Branch var saved: branch=$branch. Switching to $(git default-branch) (git switch $(git default-branch))"
git switch "$(git default-branch)"

log "Switched to $(git default-branch). Deleting branch $branch (git branch -d $branch)..."
git branch -d $branch