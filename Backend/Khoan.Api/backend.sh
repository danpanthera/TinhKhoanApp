#!/bin/bash
# Backend startup alias - can be run from anywhere in the project
exec "$(dirname "$(find /opt/Projects/Khoan -name "universal_backend.sh" -type f | head -1)")/universal_backend.sh" "$@"
