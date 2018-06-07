namespace XamlPowerToys.Infrastructure {
    using System;
    using System.Globalization;

    //using System.Globalization;
    using System.IO;
    using System.Linq;
    //using System.Runtime.InteropServices;
    using EnvDTE;
    using Microsoft.Build.Construction;
    //using Microsoft.VisualStudio.Shell.Interop;
    using XamlPowerToys.Model;
    using XamlPowerToys.UI.Infrastructure;
    //using Constants = Microsoft.VisualStudio.OLE.Interop.Constants;
    //using IServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

    internal class AssemblyAssistant {

        public const String XamarinProjectGuidString = "786C830F-07A1-408B-BD7F-6EE04809D6DB";
        public const String WpfProjectGuidString = "60DC8134-EBA5-43B8-BCC9-BB4BC16C2548";
        public const String UwpProjectGuidString = "A5A43C5B-DE2A-4C0C-9213-0A381AF9435A";
        public const String SilverlightProjectGuidString = "A1591282-1198-4647-A2B1-27E5FF5F6F3B";

        internal static String GetAssemblyPath(Project vsProject) {
            var fullPath = Path.GetDirectoryName(vsProject.FullName);
            var outputPath = vsProject.ConfigurationManager.ActiveConfiguration.Properties.Item("OutputPath").Value.ToString();

            // ReSharper disable AssignNullToNotNullAttribute
            var outputDirectory = Path.Combine(fullPath, outputPath);

            // ReSharper restore AssignNullToNotNullAttribute
            var outputFileName = vsProject.Properties.Item("OutputFileName").Value.ToString();

            if (String.IsNullOrWhiteSpace(outputFileName)) {
                var assemblyName = vsProject.Properties.Item("AssemblyName").Value.ToString();
                if (String.IsNullOrWhiteSpace(assemblyName)) {
                    assemblyName = vsProject.Name;
                }
                if (File.Exists(Path.Combine(outputDirectory, $"{assemblyName}.dll"))) {
                    outputFileName = $"{assemblyName}.dll";
                } else if (File.Exists(Path.Combine(outputDirectory, $"{assemblyName}.exe"))) {
                    outputFileName = $"{assemblyName}.exe";
                } else {
                    return String.Empty;
                }
            }

            var assemblyPath = Path.Combine(outputDirectory, outputFileName);
            return assemblyPath;
        }

        internal static ProjectType GetProjectType(Project vsProject) {
            var result = GetProjectTypeGuids(vsProject).ToUpper();

            if (result.Contains(AssemblyAssistant.WpfProjectGuidString)) {
                return ProjectType.Wpf;
            }
            if (result.Contains(AssemblyAssistant.XamarinProjectGuidString)) {
                return ProjectType.Xamarin;
            }
            if (result.Contains(AssemblyAssistant.UwpProjectGuidString)) {
                return ProjectType.Uwp;
            }
            if (result.Contains(AssemblyAssistant.SilverlightProjectGuidString)) {
                return ProjectType.Silverlight;
            }

            return ProjectType.Unknown;
        }

        internal static String GetProjectFrameworkVersion(Project vsProject) {
            try {
                return vsProject.Properties.Item("TargetFrameworkMoniker").Value.ToString().Split(',')[1].Replace("Version=v", String.Empty);
            } catch (Exception ex) {
                DialogAssistant.ShowExceptionMessage(ex, "Checking Framework Version");
            }
            return String.Empty;
        }

        internal static String GetProjectTypeGuids(Project vsProject) {
            var projectRoot = ProjectRootElement.Open(vsProject.FileName);
            // ReSharper disable once PossibleNullReferenceException
            var netStandardTest = projectRoot.AllChildren.OfType<ProjectPropertyElement>().FirstOrDefault(x => x.Name == "TargetFramework" && x.Value.StartsWith("netstandard"));
            if (netStandardTest != null) {
                //is NETStandard project.
                var xamarinPackageReferenceTest = projectRoot.AllChildren.OfType<ProjectItemElement>().FirstOrDefault(x => x.ItemType == "PackageReference" && x.Include == "Xamarin.Forms");
                if (xamarinPackageReferenceTest != null) {
                    // has Xamarin NuGet package installed.
                    return AssemblyAssistant.XamarinProjectGuidString;
                }
                return String.Empty;
            }

            var projectTypeGuildsTest = projectRoot.AllChildren.OfType<ProjectPropertyElement>().FirstOrDefault(x => x.Name == "ProjectTypeGuids");
            if (projectTypeGuildsTest != null) {
                // non NET Standard project
                return projectTypeGuildsTest.Value;
            }

            return String.Empty;

            //var ivsSolution = (IVsSolution)GetService(proj.DTE, typeof(IVsSolution));
            //IVsHierarchy ivsHierarchy;
            //Int32 result = ivsSolution.GetProjectOfUniqueName(proj.UniqueName, out ivsHierarchy);

            //var projectTypeGuids = String.Empty;

            //if (result == 0) {
            //    var ivsAggregatableProject = (IVsAggregatableProject)ivsHierarchy;
            //    ivsAggregatableProject.GetAggregateProjectTypeGuids(out projectTypeGuids);
            //}

            //return projectTypeGuids;
        }

        internal static Boolean SkipLoadingAssembly(String assemblyName) {
            assemblyName = assemblyName.ToLower(CultureInfo.InvariantCulture);
            if (assemblyName.StartsWith("windows.")) {
                return true;
            }
            if (assemblyName.StartsWith("netstandard") || assemblyName.EndsWith("netstandard.library")) {
                return true;
            }
            if (assemblyName.StartsWith("xamarin")) {
                return true;
            }
            if (assemblyName.StartsWith("microsoft")) {
                return true;
            }
            if (assemblyName.StartsWith("system")) {
                return true;
            }
            if (assemblyName.StartsWith("mscorlib")) {
                return true;
            }
            if (assemblyName.StartsWith("presentationframework")) {
                return true;
            }
            if (assemblyName.StartsWith("presentationcore")) {
                return true;
            }
            if (assemblyName.StartsWith("windowsbase")) {
                return true;
            }
            if (assemblyName.StartsWith("wpftoolkit")) {
                return true;
            }
            if (assemblyName.StartsWith("uiautomationprovider")) {
                return true;
            }
            if (assemblyName.StartsWith("mono")) {
                return true;
            }
            if (assemblyName.StartsWith("infragistics")) {
                return true;
            }
            if (assemblyName.StartsWith("telerik")) {
                return true;
            }
            if (assemblyName.StartsWith("devexpress")) {
                return true;
            }
            if (assemblyName.StartsWith("xuni")) {
                return true;
            }
            if (assemblyName.StartsWith("skia")) {
                return true;
            }
            if (assemblyName.StartsWith("prism")) {
                return true;
            }
            if (assemblyName.StartsWith("dryloc")) {
                return true;
            }
            if (assemblyName.StartsWith("silverlight")) {
                return true;
            }
            return false;
        }

        //static Object GetService(Object serviceProvider, Guid guid) {
        //    Object service = null;
        //    IntPtr intPtr;
        //    var guidService = guid;
        //    var riidGuid = guidService;
        //    var iServiceProvider = (IServiceProvider)serviceProvider;
        //    var hr = iServiceProvider.QueryService(ref guidService, ref riidGuid, out intPtr);

        //    if (hr != 0) {
        //        Marshal.ThrowExceptionForHR(hr);
        //    } else if (!intPtr.Equals(IntPtr.Zero)) {
        //        service = Marshal.GetObjectForIUnknown(intPtr);
        //        Marshal.Release(intPtr);
        //    }

        //    return service;
        //}

        //static Object GetService(Object serviceProvider, Type type) {
        //    return GetService(serviceProvider, type.GUID);
        //}

    }
}
