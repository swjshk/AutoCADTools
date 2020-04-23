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
       public class Commands
    {
        [CommandMethod("TEST12")]
        public void test12()
        {
            var doc = AcAp.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            using (var tr = db.TransactionManager.StartTransaction())
            {
                tr.Commit();
            }
        }

        [CommandMethod("Opendrawing", CommandFlags.Session) ]
        public void OpenDrawing()
        {
            var filePath=string.Empty;
            using (OpenFileDialog openfiledialog1= new OpenFileDialog())
            {
                openfiledialog1.InitialDirectory = "C:\\FO\\";
                openfiledialog1.Filter = "dwg files(*.dwg)|*.dwg";
                if (openfiledialog1.ShowDialog()==DialogResult.OK)
                {
                    filePath = openfiledialog1.FileName;
                }
              
            }
            DocumentCollection acDocMgr = AcAp.DocumentManager;
            acDocMgr.Open(filePath,false);
            var docName=acDocMgr.MdiActiveDocument.Name;
            //acDocMgr.Open(filePath, false);
            acDocMgr.Add("drawing1.dwg");
           
        }
    }
}
