// <copyright file="PexAssemblyInfo.cs" company="Microsoft">Copyright © Microsoft 2009</copyright>਍ഀ
using Microsoft.Pex.Framework.Coverage;਍ഀ
using Microsoft.Pex.Framework.Creatable;਍ഀ
using Microsoft.Pex.Framework.Instrumentation;਍ഀ
using Microsoft.Pex.Framework.Settings;਍ഀ
using Microsoft.Pex.Framework.Stubs;਍ഀ
using Microsoft.Pex.Framework.Validation;਍ഀ
਍ഀ
// Microsoft.Pex.Framework.Settings਍ഀ
[assembly: PexAssemblySettings(TestFramework = "VisualStudioUnitTest")]਍ഀ
਍ഀ
// Microsoft.Pex.Framework.Instrumentation਍ഀ
[assembly: PexAssemblyUnderTest("PortableDeviceLib")]਍ഀ
[assembly: PexInstrumentAssembly("Interop.PortableDeviceTypesLib")]਍ഀ
[assembly: PexInstrumentAssembly("Interop.PortableDeviceApiLib")]਍ഀ
਍ഀ
// Microsoft.Pex.Framework.Creatable਍ഀ
[assembly: PexCreatableFactoryForDelegates]਍ഀ
਍ഀ
// Microsoft.Pex.Framework.Coverage਍ഀ
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "Interop.PortableDeviceTypesLib")]਍ഀ
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "Interop.PortableDeviceApiLib")]਍ഀ
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "PortableDeviceLib")]਍ഀ
਍ഀ
// Microsoft.Pex.Framework.Stubs਍ഀ
[assembly: PexAssumeContractEnsuresFailureAtStubSurface]਍ഀ
[assembly: PexChooseAsStubFallbackBehavior]਍ഀ
[assembly: PexUseStubsFromTestAssembly]਍ഀ
਍ഀ
// Microsoft.Pex.Framework.Validation਍ഀ
[assembly: PexAllowedContractRequiresFailureAtTypeUnderTestSurface]਍ഀ
਍ഀ
