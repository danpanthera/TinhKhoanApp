#!/bin/bash
# Frontend startup alias - can be run from anywhere in the project
exec "$(dirname "$(find /opt/Projects/Khoan -name "universal_frontend.sh" -type f | head -1)")/universal_frontend.sh" "$@"
