using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("ObticsWindowsBaseExtensions")]
[assembly: AssemblyDescription("WindowsBase extensions for Obtics")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("ObticsWindowsBaseExtensions")]
[assembly: AssemblyCopyright("Copyright © Obtics development team 2012")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("37945a4e-b552-460e-bd96-82d070c70522")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

#if !SILVERLIGHT
[assembly: AllowPartiallyTrustedCallers]
#endif

#if DEBUG
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("ObticsUnitTest, PublicKey=002400000480000094000000060200000024000052534131000400000100010067e15a9c4eb00abf3fc98e94181b9754298f936a386819437b083892e24f048a136aaf6fbe936c1af093c2049c9bd15ad60494501628afe9ddc665933d9146e1c4e139c895e7c560e68f1bdc8ff2d19c0e350174a0a0df0a4fe8e8315164fba495d87205f9e1a6ab0582843d8f67566ee5b650ff4a2948510c9c0ee691e189a9")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("ObticsUnitTest_PartialTrust, PublicKey=002400000480000094000000060200000024000052534131000400000100010067e15a9c4eb00abf3fc98e94181b9754298f936a386819437b083892e24f048a136aaf6fbe936c1af093c2049c9bd15ad60494501628afe9ddc665933d9146e1c4e139c895e7c560e68f1bdc8ff2d19c0e350174a0a0df0a4fe8e8315164fba495d87205f9e1a6ab0582843d8f67566ee5b650ff4a2948510c9c0ee691e189a9")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("ObticsUnitTest_Silverlight, PublicKey=002400000480000094000000060200000024000052534131000400000100010067e15a9c4eb00abf3fc98e94181b9754298f936a386819437b083892e24f048a136aaf6fbe936c1af093c2049c9bd15ad60494501628afe9ddc665933d9146e1c4e139c895e7c560e68f1bdc8ff2d19c0e350174a0a0df0a4fe8e8315164fba495d87205f9e1a6ab0582843d8f67566ee5b650ff4a2948510c9c0ee691e189a9")]
#endif

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("ObticsBindingHelper, PublicKey=002400000480000094000000060200000024000052534131000400000100010067e15a9c4eb00abf3fc98e94181b9754298f936a386819437b083892e24f048a136aaf6fbe936c1af093c2049c9bd15ad60494501628afe9ddc665933d9146e1c4e139c895e7c560e68f1bdc8ff2d19c0e350174a0a0df0a4fe8e8315164fba495d87205f9e1a6ab0582843d8f67566ee5b650ff4a2948510c9c0ee691e189a9")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("ObticsBindingHelper_Silverlight, PublicKey=002400000480000094000000060200000024000052534131000400000100010067e15a9c4eb00abf3fc98e94181b9754298f936a386819437b083892e24f048a136aaf6fbe936c1af093c2049c9bd15ad60494501628afe9ddc665933d9146e1c4e139c895e7c560e68f1bdc8ff2d19c0e350174a0a0df0a4fe8e8315164fba495d87205f9e1a6ab0582843d8f67566ee5b650ff4a2948510c9c0ee691e189a9")]


[assembly: System.Resources.NeutralResourcesLanguage("en-US")]