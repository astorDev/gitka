source /dev/stdin <<< "$(curl -sS https://raw.githubusercontent.com/astorDev/nice-shell/refs/heads/main/.sh)"

if git config --global --get alias.default-and-burn >/dev/null 2>&1; then
  log "git default-and-burn alias is installed"
else
  throw "git default-and-burn alias is not installed. Install it first."
fi

log "Installing git post-pr alias..."

git config --global alias.post-pr '!f() { 
  source /dev/stdin <<< "$(curl -sS https://raw.githubusercontent.com/astorDev/nice-shell/refs/heads/main/.sh)"

  log "Switching to default branch and deleting current branch: git default-and-burn"
  git default-and-burn

  log "Switched to $(git default-branch). Pulling changes from the merged PR: git pull"
  git pull
}; f'

log "✅ Installed git post-pr alias."