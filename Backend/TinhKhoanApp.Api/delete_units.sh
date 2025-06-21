#!/bin/bash

# Read unit IDs from file and delete them
while read id; do
  echo "Deleting unit with ID: $id"
  curl -X DELETE "http://localhost:5055/api/Units/$id"
  # Add a small delay to avoid overwhelming the server
  sleep 0.1
done < units_to_delete.txt

echo "All units with ID >= 1026 have been deleted."
