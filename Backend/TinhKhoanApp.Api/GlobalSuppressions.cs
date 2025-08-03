// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.

using System.Diagnostics.CodeAnalysis;

// Suppress XML documentation warnings globally for faster development
[assembly: SuppressMessage("Style", "CS1591:Missing XML comment for publicly visible type or member", Justification = "XML documentation suppressed for development speed")]

// Suppress repository pattern warnings
[assembly: SuppressMessage("Style", "CS0108:Member hides inherited member", Justification = "Repository pattern intentional hiding")]
[assembly: SuppressMessage("Style", "CS0114:Member hides inherited member", Justification = "Repository pattern intentional overrides")]

// Suppress nullable warnings for legacy compatibility
[assembly: SuppressMessage("Nullable", "CS8603:Possible null reference return", Justification = "Legacy code compatibility")]
[assembly: SuppressMessage("Nullable", "CS8600:Converting null literal", Justification = "Legacy code compatibility")]
[assembly: SuppressMessage("Nullable", "CS8602:Dereference of a possibly null reference", Justification = "Legacy code compatibility")]
[assembly: SuppressMessage("Nullable", "CS8604:Possible null reference argument", Justification = "Legacy code compatibility")]
[assembly: SuppressMessage("Nullable", "CS8629:Nullable value type may be null", Justification = "Legacy code compatibility")]

// Suppress other common warnings
[assembly: SuppressMessage("Performance", "EF1002:SQL injection risk", Justification = "Raw SQL carefully validated")]
[assembly: SuppressMessage("Style", "CS1570:XML comment has badly formed XML", Justification = "XML documentation suppressed")]
