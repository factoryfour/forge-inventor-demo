# Sample plugin

This folder contains the samplePlugin that is rebuilt into samplePluginAppPackage.

Output from the build
```
1>------ Rebuild All started: Project: ZipAppPackage, Configuration: Release Any CPU ------
1>  ZipAppPackage -> [...]\fusiform\forge\change-parameters\samplePlugin\ZipAppPackage\bin\Release\ZipAppPackage.exe
2>------ Rebuild All started: Project: samplePlugin, Configuration: Release Any CPU ------
2>C:\Program Files (x86)\MSBuild\14.0\bin\Microsoft.Common.CurrentVersion.targets(1820,5): warning MSB3270: There was a mismatch between the processor architecture of the project being built "MSIL" and the processor architecture of the reference "Autodesk.iLogic.Core", "AMD64". This mismatch may cause runtime failures. Please consider changing the targeted processor architecture of your project through the Configuration Manager so as to align the processor architectures between your project and references, or take a dependency on references with a processor architecture that matches the targeted processor architecture of your project.
2>CSC : warning CS1762: A reference was created to embedded interop assembly 'Autodesk.Inventor.Interop, Version=21.0.0.0, Culture=neutral, PublicKeyToken=d84147f8b4276564' because of an indirect reference to that assembly created by assembly 'Autodesk.iLogic.Interfaces, Version=21.0.14200.0, Culture=neutral, PublicKeyToken=null'. Consider changing the 'Embed Interop Types' property on either assembly.
2>CSC : warning CS1762: A reference was created to embedded interop assembly 'Autodesk.Inventor.Interop, Version=21.0.0.0, Culture=neutral, PublicKeyToken=d84147f8b4276564' because of an indirect reference to that assembly created by assembly 'Autodesk.iLogic.Core, Version=21.0.14200.0, Culture=neutral, PublicKeyToken=null'. Consider changing the 'Embed Interop Types' property on either assembly.
2>  samplePlugin -> C:\Users\Pranav\Documents\GitHub\fusiform\forge\change-parameters\samplePluginAppPackage\samplePlugin.bundle\Contents\samplePlugin.dll
2>  'mt.exe' is not recognized as an internal or external command,
2>  operable program or batch file.
========== Rebuild All: 2 succeeded, 0 failed, 0 skipped ==========
```

**There's an error message in there, but I'm not sure how to fix it. How should I configure the build to prevent these errors?**

The contents of the bundle are then zipped and uploaded as an AppPackage.