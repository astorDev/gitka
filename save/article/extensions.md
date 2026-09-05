## Functions Wrapping

```sh
git config --global alias.check '!if [ -z "$1" ]; then echo "No arg"; else echo "Arg: $1"; fi'
```

`git check one` 

```text
if [ -z "$1" ]; then echo "No arg"; else echo "Arg: $1"; fi: -c: line 0: syntax error near unexpected token `"$@"'
if [ -z "$1" ]; then echo "No arg"; else echo "Arg: $1"; fi: -c: line 0: `if [ -z "$1" ]; then echo "No arg"; else echo "Arg: $1"; fi "$@"'
```

`syntax error near unexpected token "$@`

`f() { ... }; f`:

```sh
git config --global alias.check '!f() { if [ -z "$1" ]; then echo "No arg"; else echo "Arg: $1"; fi; }; f'
```

```text
Arg: one
```