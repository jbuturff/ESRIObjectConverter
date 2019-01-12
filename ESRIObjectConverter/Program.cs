using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

namespace ESRIObjectConverter
{
    class Program
    {
        private static LicenseInitializer m_AOLicenseInitializer = new ESRIObjectConverter.LicenseInitializer();
    
        [STAThread()]
        static void Main(string[] args)
        {

            if (args.Length == 0)
            {
                System.Console.WriteLine("Please enter a file geodatabase to convert.");
                System.Console.WriteLine("Usage: ESRIObjectConverter <file_gdb>");
                Environment.Exit(0);
            }

            string FILEGDB = args[0];
            

            //ESRI License Initializer generated code.
            System.Console.WriteLine ("Getting license...");
            m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeAdvanced },
            new esriLicenseExtensionCode[] { });


            // Create a file geodatabase workspace factory.
            IWorkspaceFactory2 workspaceFactory = System.Activator.CreateInstance(System.Type.GetTypeFromProgID("esriDataSourcesGDB.FileGDBWorkspaceFactory")) as IWorkspaceFactory2;
            IFeatureWorkspace featureWorkspace = workspaceFactory.OpenFromFile(FILEGDB, 0) as IFeatureWorkspace;
            
            // Open the geodatabase using the name object.
            IWorkspace workspace = (IWorkspace)featureWorkspace;

            IEnumDataset enumDS = workspace.get_Datasets(esriDatasetType.esriDTAny);
            enumDS.Reset();

            IDataset ds = enumDS.Next();

            while (ds != null)
            {
                System.Console.WriteLine("Found dataset: " + ds.Name);
                ProcessDataset(ds);
                ds = enumDS.Next();
            }
            
            //ESRI License Initializer generated code.
            //Do not make any call to ArcObjects after ShutDownApplication()

            System.Console.WriteLine("Returning license...");
            m_AOLicenseInitializer.ShutdownApplication();
        }

        static void CleanClass(IClass myClass)
        {
            try
            {

                IDataset ds = (IDataset)myClass;
                UID myUID = new UID();
                UID myExtUID = new UID();

                string curUID = myClass.CLSID.Value.ToString();

                switch (curUID)
                {
                    // Handle non-Network classes
                    case "{53D0BFE2-446E-11D3-88FC-00104B9F25F6}":
                    case "{53D0BFE5-446E-11D3-88FC-00104B9F25F6}":
                    case "{53D0BFE7-446E-11D3-88FC-00104B9F25F6}":
                    case "{53D0BFE9-446E-11D3-88FC-00104B9F25F6}":
                        myUID.Value = "{52353152-891A-11D0-BEC6-00805F7C4268}";
                        myExtUID = null;
                        break;

                    // Complex edges
                    case "{53D0BFFD-446E-11D3-88FC-00104B9F25F6}":
                        myUID.Value = "{A30E8A2A-C50B-11D1-AEA9-0000F80372B4}";
                        myExtUID = null;
                        break;

                    // Simple Junctions
                    case "{53D0BFF4-446E-11D3-88FC-00104B9F25F6}":
                    case "{EA831E03-7D3D-11D4-9A1B-0001031AE963}":
                        myUID.Value = "{CEE8D6B8-55FE-11D1-AE55-0000F80372B4}";
                        myExtUID = null;
                        break;

                    // Annotation
                    case "{1CBACE68-7E30-46EF-89F6-486082380E16}":
                        myUID.Value = "{E3676993-C682-11D2-8A2A-006097AFF44E}";
                        myExtUID.Value = "{24429589-D711-11D2-9F41-00C04F6BC6A5}";
                        break;

                    // Tables
                    case "{EA831E01-7D3D-11D4-9A1B-0001031AE963}":
                        myUID.Value = "{7A566981-C114-11D2-8A28-006097AFF44E}";
                        myExtUID = null;
                        break;
                }

                if (myUID.Value != null)
                {

                    System.Console.WriteLine("  " + ds.BrowseName + " :: " + myClass.CLSID.Value.ToString() + " :: " + myUID.Value.ToString());

                    IClassSchemaEdit4 se = (IClassSchemaEdit4)myClass;
                    ISchemaLock sl = (ISchemaLock)se;
                    sl.ChangeSchemaLock(esriSchemaLock.esriExclusiveSchemaLock);
                    se.AlterInstanceCLSID(myUID);
                    se.AlterClassExtensionCLSID(myExtUID, null);
                    sl.ChangeSchemaLock(esriSchemaLock.esriSharedSchemaLock);
                    System.Console.WriteLine("        Done.");
                }
                else
                {
                    System.Console.WriteLine("  " + ds.BrowseName + " :: " + myClass.CLSID.Value.ToString() + " :: Not changed.");
                }
            }
            catch (Exception exp)
            {
                System.Console.WriteLine("Exception in CleanClass :: " + exp);
            }

        }

        static void ProcessDataset(IDataset dataset)
        {
            try
            {

                IFeatureDataset fDataset;
                IFeatureClassContainer fcContainer;
                IEnumFeatureClass enumFeatCls;
                IFeatureClass featCls;
                if (dataset.Type == esriDatasetType.esriDTFeatureDataset)
                {
                    fDataset = (IFeatureDataset)dataset;
                    fcContainer = (IFeatureClassContainer)fDataset;
                    enumFeatCls = fcContainer.Classes;
                    //enumFeatCls.Reset();
                    featCls = enumFeatCls.Next();


                    while (featCls != null)
                    {

                        IDataset ds = (IDataset)featCls;
                        CleanClass((IClass) ds);
                        featCls = enumFeatCls.Next();

                    }
                }
                else if ((dataset.Type == esriDatasetType.esriDTTable) || (dataset.Type == esriDatasetType.esriDTFeatureClass))
                {
                    CleanClass((IClass)dataset);
                }
            }
            catch (Exception exp)
            {
                System.Console.WriteLine("Exception in ProcessDataset :: " + exp);
            }
        }




    }
}
