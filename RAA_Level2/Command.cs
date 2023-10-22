#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Linq;

#endregion

namespace RAA_Level2
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            // put any code needed for the form here
            ViewFamilyType floorPlanType = new FilteredElementCollector(doc)
                                 .OfClass(typeof(ViewFamilyType))
                                 .Cast<ViewFamilyType>()
                                 .Where(x => x.ViewFamily.Equals(ViewFamily.FloorPlan))
                                 .First();

            ViewFamilyType ceilingPlanType = new FilteredElementCollector(doc)
                                .OfClass(typeof(ViewFamilyType))
                                .Cast<ViewFamilyType>()
                                .Where(x => x.ViewFamily.Equals(ViewFamily.CeilingPlan))
                                .First();

            // open form
            MyForm currentForm = new MyForm()
            {
                Width = 500,
                Height = 400,
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen,
                Topmost = true,
            };

            currentForm.ShowDialog();

            // get form data

            if(currentForm.DialogResult == false)
            {
                return Result.Cancelled;
            }

            string csvFilePath = currentForm.GetTextBoxValue();
            string units = currentForm.GetRadioButtonValue();
            bool createFloorPlans = currentForm.GetCheckboxFloorPlans();
            bool createCeilingPlans = currentForm.GetCheckboxCeilingPlans();

            // open csv file and get csv data

            string[] csvData = File.ReadAllLines(csvFilePath);
            List<string[]> csvDataList = new List<string[]>();

            foreach (string line in csvData) 
            {
                string[] cellValue = line.Split(',');
                csvDataList.Add(cellValue); 
            }

            // remove header
            csvDataList.RemoveAt(0);

            // Create Plans
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Create Views from CSV file");

                int levelCount = 0;
                int viewCount = 0;

                foreach (string[] lineData in csvDataList)
                {
                    

                    // get elevation
                    double elevation;
                    if (units == "Metric")
                    {
                        double elevationInMeters = double.Parse(lineData[2]);
                        elevation = elevationInMeters * 3.28084;
                    }
                    else 
                        elevation = double.Parse(lineData[1]);

                    // create level
                    Level level = Level.Create(doc, elevation);
                    level.Name = lineData[0];
                    levelCount++;

                    // create floor plan
                    if (createFloorPlans == true)
                    {
                        ViewPlan.Create(doc, floorPlanType.Id, level.Id);
                        viewCount++;
                    }
                    if (createCeilingPlans == true)
                    {
                        ViewPlan.Create(doc, ceilingPlanType.Id, level.Id);
                        viewCount++;
                    }

                }
                TaskDialog.Show("Levels and Views Created", $"{levelCount}  levels were created!, {viewCount} plans were created!");
                t.Commit();
            }

            return Result.Succeeded;
        }

        public static String GetMethod()
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType?.FullName;
            return method;
        }
    }
}
