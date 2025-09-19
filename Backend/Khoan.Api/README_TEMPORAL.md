# Temporal Mapping quick notes

- Guide: see `TEMPORAL_MAPPING_GUIDE.md` in this folder.
- Dev defaults: DP01 on, others off. Toggle per table in `appsettings.Development.json` under `TemporalMapping`.
- Important: Do NOT declare SysStartTime/SysEndTime in entity classes; EF creates them as shadow properties.
- If you see "Period property 'X.SysStartTime' must be a shadow property", remove period columns from the entity.
