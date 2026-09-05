git config --global alias.current 'rev-parse --abbrev-ref HEAD'

echo "✅ Installed the \`git current\` alias"

git config --global alias.save '!f() { 
    source /dev/stdin <<< "$(curl -sS https://raw.githubusercontent.com/astorDev/nice-shell/refs/heads/main/.sh)"

    if [ -z "$1" ]; then
        throw "Commit message was not provided"
    fi

    log "Adding all files to git (git add --all)"
    git add --all

    log "Committing changes (git commit --message \"${1}\")"
    git commit --message "$1"

    log "Pushing changes to the remote repository (git push --set-upstream origin $(git current))"
    git push  --set-upstream origin $(git current)

    log "Changes have been saved successfully ✅" 
}; f'

echo "✅ Installed the \`git save\` alias"