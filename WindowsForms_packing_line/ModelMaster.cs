using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsForms_packing_line
{
    class ModelMaster  //db model master
    {
        public int ID { get; set; }
        public string Kanban { get; set; }
        public string ModelNo { get; set; }
        public string InnerA { get; set; }
        public string InnerB { get; set; }
        public string Carton { get; set; }
        public string Export { get; set; }
        public int InnerMax { get; set; }
        public int CartonMax { get; set; }
        //public ModelMaster(int id, string kanban, string modelno, string inenr_a, string inner_b, string carton, string export, int inner_max, int carton_max)
        //{
        //    ID = id;
        //    Kanban = kanban;
        //    ModelNo = modelno;
        //    InnerA = inenr_a;
        //    InnerB = inner_b;
        //    Carton = carton;
        //    Export = export;
        //    InnerMax = inner_max;
        //    CartonMax = carton_max;
        //}
    }
}
