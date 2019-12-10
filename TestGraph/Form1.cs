using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ZedGraph;

namespace TestGraph
{
    public partial class Form1 : Form
    {
        double realtime = 0;                                    //Khai báo biến thời gian để vẽ đồ thị
        double datas = 0;                                       //Khai báo biến dữ liệu thứ nhất để vẽ đồ thị
        double datas2 = 0;                                      //Khai báo biến dữ liệu thứ 2 để vẽ đồ thị
        double i = 0;                                           //Biến đếm
 


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Khởi tạo ZedGraph            
            GraphPane myPane = zedGraphControl1.GraphPane;      //Tác động các thành phần của Control, (GraphPane)
            myPane.Title.Text = "Đồ thị dữ liệu theo thời gian";
            myPane.XAxis.Title.Text = "Thời gian (s)";
            myPane.YAxis.Title.Text = "Dữ liệu";

            RollingPointPairList list = new RollingPointPairList(60000);        //Tạo mới danh sách dữ liệu 60000 phần tử, có khả năng cuốn chiếu
            LineItem curve = myPane.AddCurve("Dữ liệu", list, Color.Red, SymbolType.None);         //Tạo mới đường cong của đồ thị trên GraphPane dựa vào danh sách dữ liệu
            RollingPointPairList list2 = new RollingPointPairList(60000);
            LineItem curve2 = myPane.AddCurve("Dữ liệu 2", list2, Color.MediumSlateBlue, SymbolType.None);
            
            myPane.XAxis.Scale.Min = 0;                         //Đặt giới hạn đồ thị
            myPane.XAxis.Scale.Max = 30;
            myPane.XAxis.Scale.MinorStep = 1;                   //Đặt các bước độ chia
            myPane.XAxis.Scale.MajorStep = 5;
            myPane.YAxis.Scale.Min = -100;                      //Tương tự cho trục y
            myPane.YAxis.Scale.Max = 100;

            myPane.AxisChange();                                //Cập nhật các trục
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //Tính các giá trị để vẽ lên đồ thị
            realtime = i * 10.0; 
            datas = (50 * Math.Sin(i));
            datas2 = (60 * Math.Cos(i));
            i += 0.01;
            Draw();                                             //Vẽ các giá trị lên đồ thị
        }

        // Vẽ đồ thị
        private void Draw()
        {
            
            if (zedGraphControl1.GraphPane.CurveList.Count <= 0)    //Có đường cong nào trong danh sách chưa?
                return;

            LineItem curve = zedGraphControl1.GraphPane.CurveList[0] as LineItem;   //Khai báo đường cong từ danh sách đường cong đồ thị (kế thừa từ heap của dữ liệu ở Form_load)
            if (curve == null)
                return;

            LineItem curve2 = zedGraphControl1.GraphPane.CurveList[1] as LineItem;
            if (curve2 == null)
                return;
            
            IPointListEdit list = curve.Points as IPointListEdit;   //Khai báo danh sách dữ liệu cho đường cong đồ thị
            if (list == null)
                return;

            IPointListEdit list2 = curve2.Points as IPointListEdit;
            if (list2 == null)
                return;

            list.Add(realtime, datas);                          // Thêm điểm trên đồ thị
            list2.Add(realtime, datas2);                        // Thêm điểm trên đồ thị
            

            Scale xScale = zedGraphControl1.GraphPane.XAxis.Scale;  //Giới hạn của đồ thị
            Scale yScale = zedGraphControl1.GraphPane.YAxis.Scale;

            // Tự động Scale theo trục x
            if (realtime > xScale.Max - xScale.MajorStep)       //Nếu realtime lớn hơn Max x trừ đi 1 MajorStep (2 vạch lớn)
            {
                xScale.Max = realtime + xScale.MajorStep;       //Tự dời đồ thị qua 1 MajorStep 
                xScale.Min = xScale.Max - 30;
            }

            // Tự động Scale theo trục y
            if (datas > yScale.Max - yScale.MajorStep)          //Nếu datas vượt quá giới hạn trừ 1 MajorStep
            {
                yScale.Max = datas + yScale.MajorStep;          //Thì tăng giới hạn thêm 1 MajorStep
            }
            else if (datas < yScale.Min + yScale.MajorStep)
            {
                yScale.Min = datas - yScale.MajorStep;
            }

            zedGraphControl1.AxisChange();                      //Thay đổi trục theo giá trị Scale
            zedGraphControl1.Invalidate();                      //Mở khoá để và vẽ lại
            //zedGraphControl1.Refresh();                         //Vẽ lại (tăng cường cho Invalidate
        }

        // Xóa đồ thị, với ZedGraph thì phải khai báo lại như ở hàm Form1_Load, nếu không sẽ không hiển thị
        private
            void ClearZedGraph()
        {
            zedGraphControl1.GraphPane.CurveList.Clear(); // Xóa đường
            zedGraphControl1.GraphPane.GraphObjList.Clear(); // Xóa đối tượng

            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();

            GraphPane myPane = zedGraphControl1.GraphPane;
            myPane.Title.Text = "Đồ thị dữ liệu theo thời gian";
            myPane.XAxis.Title.Text = "Thời gian (s)";
            myPane.YAxis.Title.Text = "Dữ liệu";

            RollingPointPairList list = new RollingPointPairList(60000);
            LineItem curve = myPane.AddCurve("Dữ liệu", list, Color.Red, SymbolType.None);
            
            myPane.XAxis.Scale.Min = 0;
            myPane.XAxis.Scale.Max = 30;
            myPane.XAxis.Scale.MinorStep = 1;
            myPane.XAxis.Scale.MajorStep = 5;
            myPane.YAxis.Scale.Min = -100;
            myPane.YAxis.Scale.Max = 100;

            zedGraphControl1.AxisChange();
        }

        // Hàm xóa dữ liệu
        private
            void ResetValue()
        {
            realtime = 0;
            datas = 0;
            //status = 0; // Chuyển status về 0
        }


    }

}
