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
        [CommandMethod("TT")]
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

        [CommandMethod("test1", CommandFlags.Session) ]
        public void OpenDrawing()
        {  
            //acDocMgr.Add("drawing1.dwg");
            Drawing wDrawing = new Drawing();
            List<PAWSBlock> blocklist = wDrawing.GetPawsBlock();
            wDrawing.group();
        }

    }
}
