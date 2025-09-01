#!/bin/bash
# Fullstack startup alias - can be run from anywhere in the project
exec "$(dirname "$(find /opt/Projects/Khoan -name "universal_fullstack.sh" -type f | head -1)")/universal_fullstack.sh" "$@"
