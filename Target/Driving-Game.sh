#!/bin/sh
echo -ne '\033c\033]0;Driving-Game\a'
base_path="$(dirname "$(realpath "$0")")"
"$base_path/Driving-Game.x86_64" "$@"
