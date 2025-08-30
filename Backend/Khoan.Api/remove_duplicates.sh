#!/bin/bash

echo "🧹 Removing duplicate using statements..."

# Remove duplicate using statements per file
find . -name "*.cs" -exec awk '
{
    if (/^using / && seen[$0]++) {
        # Skip duplicate using statements
    } else {
        print $0
    }
}
' {} \; -exec mv {} {}.tmp \; -exec mv {}.tmp {} \;

echo "✅ Duplicate using statements removed!"
