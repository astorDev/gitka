source /dev/stdin <<< "$(curl -sS https://raw.githubusercontent.com/astorDev/nice-shell/refs/heads/main/.sh)"

log "Installing git default-branch alias..."

git config --global alias.default-branch '!f() { 
    git symbolic-ref --short refs/remotes/origin/HEAD | sed 's@^origin/@@'
}; f'

log "✅ Installed git default-branch alias."