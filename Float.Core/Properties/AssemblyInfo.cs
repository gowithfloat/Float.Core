// <copyright file="AssemblyInfo.cs" company="Float">
// Copyright (c) 2021 Float, All rights reserved.
// Shared under an MIT license. See license.md for details.
// </copyright>

using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;

// allow testing of internal classes, only on debug builds
#if DEBUG
[assembly: InternalsVisibleTo("Float.Core.Tests")]
#endif

[assembly: NeutralResourcesLanguage("en")]
[assembly: AssemblyVersion("0.0.1")]
