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
        public void GetBlockRefList()
        {
            var doc = AcAp.DocumentManager.MdiActiveDocument;
            using (Transaction tr = doc.Database.TransactionManager.StartTransaction())
            {
                var db = doc.Database;
                var bt = 
                    tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                var modelspace = bt[BlockTableRecord.ModelSpace].GetObject(OpenMode.ForRead) as BlockTableRecord;
                               
                List<ObjectId> blockRefList = new List<ObjectId>();

                foreach (ObjectId blockRefId in modelspace)
                {
                    if (blockRefId.ObjectClass.Name == "AcDbBlockReference")
                    {
                        blockRefList.Add(blockRefId);
                    }
                }                  
                var count = blockRefList.Count;
            };

        }
    }
}
