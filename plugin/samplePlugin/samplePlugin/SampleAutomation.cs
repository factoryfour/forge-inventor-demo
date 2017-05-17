using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using Inventor;
using System.Collections.Generic;
using Newtonsoft.Json;

using Autodesk.iLogic.Core;
using Autodesk.iLogic.Interfaces;

// Debug
using System.Security.AccessControl;
using System.Reflection;

namespace samplePlugin
{

    class PluginProperties
    {
        public static string ChangeParamsRule = "ChangeParameters";
        public static string DoOutputRule = "DoOutput";
        public static string DoOutputDefault = "DirectSave";
        public static string ChangeParametersDefault = "DirectSet";
    }

    [ComVisible(true)]
    public class SampleAutomation
    {
        Inventor.InventorServer m_inventorServer;
        IiLogicAutomation m_iLogicAuto = null;
        string m_otherFileFullPath;
        Document m_otherDoc;
        Document m_thisDoc;
        string m_docPath;
        string m_assyDataFolder;
        string m_assemblyFolder;
        string m_docDataFolder;

        public SampleAutomation(Inventor.InventorServer inventorServer)
        {
            Trace.TraceInformation("Starting sample plugin.");
            m_inventorServer = inventorServer;
            if (iLogicCentral.Instance != null)
            {
                m_iLogicAuto = iLogicCentral.Instance.ExternalApi;
            }
            else
            {
                Trace.TraceInformation("iLogic was not available!!!");
            }
        }

        public void Run(Document doc)
        {
            Trace.TraceInformation("Run called with {0}", doc.DisplayName);
            System.IO.File.AppendAllText("output.txt", "Document name: " + doc.DisplayName);
        }

        public void RunWithArguments(Document doc, NameValueMap map)
        {
            StringBuilder traceInfo = new StringBuilder("RunWithArguments called with ");
            traceInfo.Append(doc.DisplayName);
            Trace.TraceInformation(map.Count.ToString());

            // values in map are keyed on _1, _2, etc
            for (int i = 0; i < map.Count; i++)
            {
                traceInfo.Append(" and ");
                traceInfo.Append(map.Value["_" + (i + 1)]);
            }

            Trace.TraceInformation(traceInfo.ToString());

            m_thisDoc = doc;
            m_docPath = System.IO.Path.GetDirectoryName(m_thisDoc.FullFileName);


            // If my AppPackage has a 'data' folder then copy it to the document's folder.
            m_assemblyFolder = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            m_assyDataFolder = System.IO.Path.Combine(m_assemblyFolder, "data");
            string docFolder = System.IO.Path.GetDirectoryName(doc.FullFileName);
            m_docDataFolder = System.IO.Path.Combine(docFolder, "data");
            if (System.IO.Directory.Exists(m_assyDataFolder))
            {
                Trace.TraceInformation("Copying data folder: {0} \n to {1}", m_assyDataFolder, m_docDataFolder);
                DirectoryCopy(m_assyDataFolder, m_docDataFolder, true);
            }
            else
            {
                Trace.TraceInformation("No 'data' folder");
            }


            // Param "_1" should be the filename of the JSON file containing the plugin settings
            Trace.TraceInformation("Getting argument: _1");
            string settingsFile = map.Value["_1"];
            Trace.TraceInformation("settings file: " + settingsFile);
            string jsonSettings = System.IO.File.ReadAllText(settingsFile);
            Trace.TraceInformation("settingsFile file: \"" + jsonSettings + "\"");


            Trace.TraceInformation("Getting argument: _2");
            // Param "_2" should be the filename of the JSON file containing the parameters and values
            string paramFile = map.Value["_2"];
            Trace.TraceInformation("paramFile: " + paramFile);
            string json = System.IO.File.ReadAllText(paramFile);
            Trace.TraceInformation("changeParameters file: \"" + json + "\"");

            ChangeParameters(jsonSettings, json, doc);

            // Param "_3" should be the output filename
            Trace.TraceInformation("Getting argument: _3");
            string outputFile = map.Value["_3"];
            Trace.TraceInformation("Output file: {0}", outputFile);

            string dirPath = System.IO.Path.GetDirectoryName(doc.FullDocumentName);
            string fullPath = dirPath + "\\" + outputFile;
            Trace.TraceInformation("Full path of file to write: " + fullPath);


            DoOutput(jsonSettings, doc, fullPath);


        }

        void ChangeParameters(string jsonSettings, string json, Document doc)
        {

            Dictionary<string, string> settings = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonSettings);
            Dictionary<string, string> paras = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            if (paras.ContainsKey("otherFile")) 
            {
                // do the "simpleAPI" thing
                string otherFile;
                paras.TryGetValue("otherFile", out otherFile);
                m_otherFileFullPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(doc.FullFileName), "data", otherFile);
                Trace.TraceInformation("Other file is: {0}", m_otherFileFullPath);
                m_otherDoc = m_inventorServer.Documents.Open(m_otherFileFullPath);
                NameValueMap argMap = m_inventorServer.TransientObjects.CreateNameValueMap();
                foreach (KeyValuePair<string, string> entry in paras)
                {
                    if (string.Compare(entry.Key, "otherFile") != 0)
                    {
                        // entry.key = param name
                        // entry.value = param value string
                        Trace.TraceInformation("param to change: {0}:{1}", entry.Key, entry.Value);
                        argMap.Add(entry.Key, entry.Value);
                    }

                }
                iLogicRule rule = m_iLogicAuto.GetRule(m_otherDoc, PluginProperties.ChangeParamsRule);
                if (rule != null)
                {
                    Trace.TraceInformation("Running '{0}' rule.", PluginProperties.ChangeParamsRule);
                    m_iLogicAuto.RunRuleWithArguments(m_otherDoc, PluginProperties.ChangeParamsRule, argMap);
                }
                else
                {
                    string ruleFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(doc.FullFileName), "data/ChangeParameters.iLogicVb");
                    Trace.TraceInformation("Running external '{0}' rule.", ruleFile); // PluginProperties.ChangeParamsRule);
                    m_iLogicAuto.RunExternalRuleWithArguments(m_otherDoc, ruleFile, argMap); // PluginProperties.ChangeParamsRule
                }
                return;
            }

            // Otherwise do the generic thing.
            string changeType;
            if (!settings.TryGetValue("ChangeParameters", out changeType))
            {
                Trace.TraceInformation("Using default, '{0}', for 'ChangeParameters'", PluginProperties.ChangeParametersDefault);
                changeType = PluginProperties.ChangeParametersDefault;
            }
             
            if (changeType.CompareTo("DirectSet") == 0)
            {
                // JLB:  Note the following requires a PartDocument, not just a Document.
                PartDocument partDoc = (PartDocument)doc;
                PartComponentDefinition partDef = partDoc.ComponentDefinition;
                foreach (KeyValuePair<string, string> entry in paras)
                {
                    // entry.key = param name
                    // entry.value = param value string
                    Trace.TraceInformation("param to change: {0}:{1}", entry.Key, entry.Value);
                    Parameter param1 = partDef.Parameters[entry.Key];
                    param1.Expression = entry.Value;
                }
            }
            else if (changeType.CompareTo("ViaILogic") == 0)
            {

                iLogicRule rule = m_iLogicAuto.GetRule(doc, PluginProperties.ChangeParamsRule);
                if (rule == null)
                {
                    Trace.TraceInformation("{0} rule was not found in {1}", PluginProperties.ChangeParamsRule, doc.DisplayName);
                }
                else
                {
                    NameValueMap argMap = m_inventorServer.TransientObjects.CreateNameValueMap();
                    foreach (KeyValuePair<string, string> entry in paras)
                    {
                        argMap.Add(entry.Key, entry.Value);
                        // entry.key = param name
                        // entry.value = param value string
                        Trace.TraceInformation("param to change: {0}:{1}", entry.Key, entry.Value);
                    }
                    Trace.TraceInformation("Running '{0}' rule.", PluginProperties.ChangeParamsRule);
                    m_iLogicAuto.RunRuleWithArguments(doc, PluginProperties.ChangeParamsRule, argMap);
                }
            }
            else
            {
                Trace.TraceInformation("Incorrect value for ChangeParameter type: {0}", changeType);
            }

            doc.Update();
            Trace.TraceInformation("doc updated.");
        }

        void DoOutput(string jsonSettings, Document doc, string outputFullFileName)
        {
            // Ignore the "settings" ... although it works fine, we're doing something different now.

            //Dictionary<string, string> settings = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonSettings);

            //string outputType;
            //if (!settings.TryGetValue("DoOutput", out outputType))
            //{
            //    outputType = PluginProperties.DoOutputDefault;
            //}


            //if (outputType.CompareTo("DirectSave") == 0)
            //{
            //    DoOutput_DirectSave(doc, outputFileName);
            //}
            //else if (outputType.CompareTo("ViaILogic") == 0)
            //{
            //    DoOutput_ViaILogic(doc, outputFileName);
            //}
            //else if (outputType.CompareTo("DirectSTL") == 0)
            //{
            //    DoOutput_DirectSTLExport(doc, outputFileName);
            //}
            //else
            //{
            //    Trace.TraceInformation("Incorrect value for output type: {0}", outputType);
            //}

            iLogicRule rule = m_iLogicAuto.GetRule(m_otherDoc, PluginProperties.DoOutputRule);
            if (rule != null)
            {
                NameValueMap argMap = m_inventorServer.TransientObjects.CreateNameValueMap();
                argMap.Add("file1", outputFullFileName);
                Trace.TraceInformation("Running '{0}' rule.", PluginProperties.DoOutputRule);
                m_iLogicAuto.RunRuleWithArguments(m_otherDoc, PluginProperties.DoOutputRule, argMap);
            }
            else
            {
                // Just do the appropriate "direct" thing.
                string extension = System.IO.Path.GetExtension(outputFullFileName);
                if ((string.Compare(extension, ".STL", System.StringComparison.OrdinalIgnoreCase) == 0) ||
                    (string.Compare(extension, ".IGES", System.StringComparison.OrdinalIgnoreCase) == 0) ||
                    (string.Compare(extension, ".STEP", System.StringComparison.OrdinalIgnoreCase) == 0))
                {
                    Do3dOutput(m_otherDoc, extension, outputFullFileName);
                }
                else if (string.Compare(extension, ".PDF", System.StringComparison.OrdinalIgnoreCase) == 0) 
                {
                    DoDrawingOutput(outputFullFileName);
                }
                else
                {
                    Trace.TraceInformation("{0} didn't have the correct extension ({1}), cannot create output.", outputFullFileName, extension);
                }
            }
        }


        void DoOutput_DirectSave(Document doc, string outputFileName)
        {
            Trace.TraceInformation("Saving file as {0}", outputFileName);
            doc.SaveAs(outputFileName, false);
            Trace.TraceInformation("Done saving file.");
        }


        void DoOutput_ViaILogic(Document doc, string fullPath)
        {
            iLogicRule rule = m_iLogicAuto.GetRule(m_otherDoc, PluginProperties.DoOutputRule);
            if (rule == null)
            {
                Trace.TraceInformation("{0} rule was not found in {1}", PluginProperties.DoOutputRule, m_otherDoc.DisplayName);
            }
            else
            {
                NameValueMap argMap = m_inventorServer.TransientObjects.CreateNameValueMap();
                argMap.Add("file1", fullPath);
                Trace.TraceInformation("File to output: {0}", fullPath);
                Trace.TraceInformation("Running '{0}' rule in '{1}'", PluginProperties.DoOutputRule, m_otherDoc.FullFileName);
                m_iLogicAuto.RunRuleWithArguments(m_otherDoc, PluginProperties.DoOutputRule, argMap);
                Trace.TraceInformation("Done running rule.");
            }
        }

        private void Do3dOutput(Document doc, string ext, string outputFileName)
        {
            if (string.Compare(ext, ".STL", System.StringComparison.OrdinalIgnoreCase) == 0)
            {
                DoOutput_DirectSTLExport(doc, outputFileName);
            }
            else
            {
                // TODO:  IGES and STEP.
                Trace.TraceInformation("IGES and STEP not supported yet");
            }
        }

        private void DoDrawingOutput(string outputFullFileName)
        {
            DrawingDocument ddoc;

            Trace.TraceInformation("Doc data folder: {0}", m_docDataFolder);
            Trace.TraceInformation("outputFullFileName: {0}", outputFullFileName);

            string filename = System.IO.Path.GetFileNameWithoutExtension(outputFullFileName);

            string ddocFullPath = System.IO.Path.Combine(m_docDataFolder, filename + ".idw");
            if (!System.IO.File.Exists(ddocFullPath))
            {
                ddocFullPath = System.IO.Path.Combine(m_docDataFolder, filename + ".dwg");
                if (!System.IO.File.Exists(ddocFullPath))
                {
                    Trace.TraceInformation("{0} doesn't exist (nor IDW version)", ddocFullPath);
                    return;
                }
            }
            Trace.TraceInformation("ddocFullPath: {0}", ddocFullPath);


            Trace.TraceInformation("Opening ... " + ddocFullPath);
            ddoc = (DrawingDocument)m_inventorServer.Documents.Open(ddocFullPath, false);

            Trace.TraceInformation("Updating...");
            ddoc.Update2(true);

            Trace.TraceInformation("ExportToPDF...");
            ExportToPDF(ddoc, outputFullFileName);

            //string pdfPath = m_docPath + "\\output.pdf";  // <---- ExportToPDF rule will create this file.

            //Trace.TraceInformation("Copying '" + pdfPath + "' to '" + ddocFullPath + "'");
            //System.IO.File.Copy(pdfPath, outputFullFileName);
            //Trace.TraceInformation("Done copying");
        }

        void ExportToPDF(DrawingDocument ddoc, string outputFullFileName)
        {
            TranslatorAddIn transl = (TranslatorAddIn)m_inventorServer.ApplicationAddIns.get_ItemById("{0AC6FD96-2F4D-42CE-8BE0-8AEA580399E4}");
            if (transl == null)
            {
                Trace.TraceInformation("PDF translator was NOT available");
                return;
            }
            else
            {
                Trace.TraceInformation("PDF translator is available");
            }

            TranslationContext context = m_inventorServer.TransientObjects.CreateTranslationContext();

            NameValueMap options = m_inventorServer.TransientObjects.CreateNameValueMap();

            Trace.TraceInformation("Setting other translator things");

            context.Type = IOMechanismEnum.kFileBrowseIOMechanism;

            DataMedium data = m_inventorServer.TransientObjects.CreateDataMedium();

            string outputFolder = System.IO.Path.GetDirectoryName(outputFullFileName);
            string filename = System.IO.Path.GetFileNameWithoutExtension(outputFullFileName);
            string pdfFullFileName = System.IO.Path.Combine(outputFolder, filename + ".pdf");

            data.FileName = pdfFullFileName;


            Trace.TraceInformation("Generating PDF output to: {0}", pdfFullFileName);
            transl.SaveCopyAs(ddoc, context, options, data);
            Trace.TraceInformation("PDF output complete");
        }

        void DoOutput_DirectSTLExport(Document doc, string outputFileName)
        {
            TranslatorAddIn transl = null;
            transl = (TranslatorAddIn)m_inventorServer.ApplicationAddIns.ItemById["{533E9A98-FC3B-11D4-8E7E-0010B541CD80}"];

            // Translator: STL Export -- {533E9A98-FC3B-11D4-8E7E-0010B541CD80} 


            if (transl == null)
            {
                Trace.TraceInformation("STL translator was NOT available");
                return;
            }
            else
            {
                Trace.TraceInformation("STL translator is available");
            }

            TranslationContext context = m_inventorServer.TransientObjects.CreateTranslationContext();
            NameValueMap options = m_inventorServer.TransientObjects.CreateNameValueMap();
            if (transl.HasSaveCopyAsOptions[doc, context, options])
            {
                Trace.TraceInformation("Setting STL translator options");
                options.Value["ExportUnits"] = 4;   // Centimeters
                options.Value["Resolution"] = 0;    // High <-- changed this from 1
            }
            Trace.TraceInformation("Setting other STL translator things");
            context.Type = IOMechanismEnum.kFileBrowseIOMechanism; // ???
            DataMedium data = m_inventorServer.TransientObjects.CreateDataMedium();
            data.FileName = outputFileName;

            Trace.TraceInformation("Generating STL output");
            transl.SaveCopyAs(doc, context, options, data);
            Trace.TraceInformation("STL output complete");
        }


        //public void RunRuleWithArguments(PartDocument doc, string ruleName, Dictionary<string, string> arguments)
        //{
        //    // iLogic-- { 3BDD8D79 - 2179 - 4B11 - 8A5A - 257B1C0263AC}

        //}

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = System.IO.Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = System.IO.Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

    }
}
