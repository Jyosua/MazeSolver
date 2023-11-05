// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "This project is a port of an old Framework program, so for now is Windows-only", Scope = "module")]
[assembly: SuppressMessage("Usage", "CA2254:Template should be a static expression", Justification = "Not doing full structured logging for a console application, at least at the moment, so this isn't relevant", Scope = "module")]
