source /dev/stdin <<< "$(curl -sS https://raw.githubusercontent.com/astorDev/nice-shell/refs/heads/main/.sh)"

if git config --global --get alias.default-branch >/dev/null 2>&1; then
  log "git default-branch alias is installed"
else
  throw "git default-branch alias is not installed. Install it first."
fi

log "Installing git default-and-burn alias..."

git config --global alias.default-and-burn '!f() { 
    source /dev/stdin <<< "$(curl -sS https://raw.githubusercontent.com/astorDev/nice-shell/refs/heads/main/.sh)"

    log "Saving current branch name branch=\$(git branch --show-current)"
    branch=$(git branch --show-current)

    log "Branch var saved: branch=$branch. Switching to $(git default-branch) (git switch $(git default-branch))"
    git switch "$(git default-branch)"

    log "Switched to $(git default-branch). Deleting branch $branch (git branch -d $branch)..."
    git branch -d $branch
}; f'

log "✅ Installed git default-and-burn alias."