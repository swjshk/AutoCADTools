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

        public List<PAWSBlock> GetPawsBlock()//given a W drawing; reture list of device
        {            
            List<PAWSBlock> pawsBlockList = new List<PAWSBlock>();
  
            var db = AcAp.DocumentManager.MdiActiveDocument.Database;
                   
            using (Transaction tr=db.TransactionManager.StartTransaction())
            {
                var modelSpace = SymbolUtilityServices.GetBlockModelSpaceId(db).GetObject(OpenMode.ForRead) as BlockTableRecord;
                var brClass = RXObject.GetClass(typeof(BlockReference));
                foreach (ObjectId id in modelSpace)
                {
                    if (id.ObjectClass== brClass)
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
       public  void group() //given a group name and start point; pass next start point
        {   
            //
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
                        var insertPoint = new Point3d(0, 0,0);
                        var moveVector = new Vector3d(1, 1, 1);
                        var startPoint = new DBPoint();
                        var endPoint = new DBPoint();
                        ObjectId[] ids = group.GetAllEntityIds();

                        //loop find points and calculate current insert point and next insert point
                        //start point (0,0)
                        //get the insert point 4/30
                        foreach (ObjectId  obj in ids)
                        {
                            var entity = obj.GetObject(OpenMode.ForRead) as Entity;
                            
                            var point0 = new Point3d(0, 0, 0);
                            
                            if (entity is DBPoint)
                            {
                                if (entity.Color.ColorNameForDisplay == "red")
                                {
                                   
                                    startPoint = entity as DBPoint;

                                }
                                if (entity.Color.ColorNameForDisplay=="blue")
                                {
                               
                                    endPoint = entity as DBPoint;

                                }
                                
                            }

                            if (startPoint.Position!=point0 && endPoint.Position!=point0)
                            {
                                moveVector = startPoint.Position.GetVectorTo(insertPoint);
                            }
                            else
                            {
                                
                            }
                        }
                        //get next insert point 5/1


                        //loop to clone entity to new location and update Device Name
                        foreach (ObjectId obj in ids)
                        {
                            var entity = obj.GetObject(OpenMode.ForWrite) as Entity;

                            var newEntity =entity.Clone () as Entity;
                                                              
                            if (newEntity is BlockReference)
                            {   
                                var br = newEntity as BlockReference;
                                if (br.AttributeCollection.Count!=0)
                                {
                                    foreach ( AttributeReference attr in br.AttributeCollection)
                                    {   
                                        
                                        if (attr.Tag=="DEVICE_NAME")
                                        {
                                            attr.TextString = "New Name";
                                        }
                                    }
                                }
                            }
                           
                            newEntity.TransformBy(Matrix3d.Displacement(moveVector));
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
