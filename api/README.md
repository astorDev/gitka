# Gitka API

API for working with files in git.

Connects to a git repository specified in `REPOSITORY__URL` environment variable. Serves content of the files in the specified endpoints:

- `GET /{branch}/{filepath}` - Serves file content from the specified branch.
- `PUT /{branch}/{filepath}` - Adds, commits and pushes file content passed in the body to the specified branch. Creates branch if it doesn't exists