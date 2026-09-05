source /dev/stdin <<< "$(curl -sS https://raw.githubusercontent.com/astorDev/nice-shell/refs/heads/main/.sh)"

if [ -z $1 ]; then
    throw "Target branch (first argument) was not provided"
fi

current=$(git current)

log "Refreshing current branch (git pull --rebase origin ${current})"
git pull --rebase origin ${current}

log "Checking out the \`${1}\` target branch (git checkout ${1})"
git checkout ${1}

log "Pulling the latest changes from the remote repository (git pull --rebase origin ${1})"
git pull --rebase origin ${1}

log "Merging the \`${current}\` branch into the \`${1}\` branch (git merge --no-edit ${current})"
git merge --no-edit ${current}

log "Pushing the changes to the remote repository (git push --set-upstream origin ${1})"
git push --set-upstream origin ${1}

log "✅ Changes have been merged successfully, moving back to the \`${current}\` branch (git checkout ${current})"
git checkout ${current}

log "✅ Merged, and switched back to the \`${current}\` branch"