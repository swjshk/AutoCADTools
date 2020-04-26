using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System.IO;
using System.Windows.Forms;

using AcAp = Autodesk.AutoCAD.ApplicationServices.Application;


namespace AutoCAD_Project2019
{
   public class Drawing
    {
        public string Name;
        public string Folder;
        public string Address;
        public string Type;
        public string Section;
        public string FO;


        public Document OpenDrawing()
        {
            string filePath = SelectDrawing();
            DocumentCollection acDocMgr = AcAp.DocumentManager;
            acDocMgr.Open(filePath, false);
            Document doc = acDocMgr.MdiActiveDocument;

            return doc;
        }
        private String SelectDrawing()
        {
            string filePath = string.Empty;
            using (OpenFileDialog openfiledialog1 = new OpenFileDialog())
            {
                openfiledialog1.InitialDirectory = "C:\\FO\\";
                openfiledialog1.Filter = "dwg files(*.dwg)|*.dwg";
                if (openfiledialog1.ShowDialog() == DialogResult.OK)
                {
                    filePath = openfiledialog1.FileName;
                }

            }
            
            return filePath;
        }
        public List<ObjectId> GetBlockRefList()
        {
            var doc = AcAp.DocumentManager.MdiActiveDocument;

            List<ObjectId> blockRefList = new List<ObjectId>();

            using (Transaction tr = doc.Database.TransactionManager.StartTransaction())
            {
                var db = doc.Database;
                var bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                var modelspace = bt[BlockTableRecord.ModelSpace].GetObject(OpenMode.ForRead) as BlockTableRecord;                             
               
                foreach (ObjectId blockRefId in modelspace)
                {
                    if (blockRefId.ObjectClass.Name == "AcDbBlockReference")
                    {
                        blockRefList.Add(blockRefId);
                    }
                }                  
                var count = blockRefList.Count;
            };
            return blockRefList;
        }

        public List<PAWSBlock> GetPawsBlock()
        {            
            List<PAWSBlock> pawsBlockList = new List<PAWSBlock>();
  
            var db = AcAp.DocumentManager.MdiActiveDocument.Database;
                   
            using (Transaction tr=db.TransactionManager.StartTransaction())
            {
                var modelSpace = SymbolUtilityServices.GetBlockModelSpaceId(db).GetObject(OpenMode.ForRead) as BlockTableRecord;
                var brClass = RXObject.GetClass(typeof(BlockReference));
                foreach (ObjectId id in modelSpace)
                {
                    if (id.ObjectClass==brClass)
                    {
                        var br = id.GetObject(OpenMode.ForRead) as BlockReference;
                        PAWSBlock pawsBlock = new PAWSBlock();
                        foreach (ObjectId attId in br.AttributeCollection)
                        {
                           
                            var attRef = attId.GetObject(OpenMode.ForRead) as AttributeReference;
                            if (attRef.Tag=="DEVICE_NAME")
                            {
                                pawsBlock.DeviceName = attRef.TextString;
                            }
                            if (attRef.Tag=="DEVICE_LOCATION")
                            {
                                pawsBlock.DeviceLoc = attRef.TextString;

                            }
                            

                        }
                        if ((pawsBlock.DeviceLoc != null) & (pawsBlock.DeviceLoc != null))
                        {
                            pawsBlock.BlockName = br.Name;
                            pawsBlockList.Add(pawsBlock);
                        }
                    }
                    
                    
                }
            }
            var qty = pawsBlockList.Count;

            return pawsBlockList;

        }
       public  void group()
        {
            var doc = AcAp.DocumentManager.MdiActiveDocument;
            using (doc.LockDocument())
            {
                var db = AcAp.DocumentManager.MdiActiveDocument.Database;
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    var gd = db.GroupDictionaryId.GetObject(OpenMode.ForRead) as DBDictionary;
                    string a = "GROUND";

                    if (gd.Contains(a))
                    {
                        var groupid = gd.GetAt(a);
                        var group = groupid.GetObject(OpenMode.ForRead) as Group;

                        ObjectId[] ids = group.GetAllEntityIds();
                        foreach (ObjectId obj in ids)
                        {
                            var entity = obj.GetObject(OpenMode.ForWrite) as Entity;
                            //entity.TransformBy(Matrix3d.Displacement(new Vector3d(3, 3, 3)));
                            var newEntity=entity.Clone () as Entity;
                            newEntity.TransformBy(Matrix3d.Displacement(new Vector3d(3, 3, 3)));
                            var modelSpace = SymbolUtilityServices.GetBlockModelSpaceId(db).GetObject(OpenMode.ForWrite) as BlockTableRecord;
                            modelSpace.AppendEntity(newEntity);
                            tr.AddNewlyCreatedDBObject(newEntity, true);
                        }

                    }

                    tr.Commit();
                }
            }
        }
    }
}
