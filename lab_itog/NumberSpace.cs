using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace lab_itog
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class NumberSpace : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
           
            FilteredElementCollector roomList = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms);
            IList<ElementId> roomids = roomList.ToElementIds() as IList<ElementId>;

            List<ViewPlan> listView = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewPlan))
                .OfType<ViewPlan>()
                .ToList();
            //View v = doc.ActiveView;
            Transaction trans = new Transaction(doc, "Создание нумерации помещения");
            trans.Start();
            foreach (View view in listView)
            {
                foreach (ElementId roomid in roomids)
                {
                    Element element = doc.GetElement(roomid);
                    Room room = element as Room;
                    XYZ roomCenter = GetElementCenter(room);
                    UV center = new UV(roomCenter.X, roomCenter.Y);
                    doc.Create.NewRoomTag(new LinkElementId(roomid), center, view.Id);
                }     
            }
            trans.Commit();
            return Result.Succeeded;
        }
           
        public XYZ GetElementCenter(Element element)
        {
            BoundingBoxXYZ bounding = element.get_BoundingBox(null);
            return (bounding.Max + bounding.Min) / 2;
        }
    }
}
