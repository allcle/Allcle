using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// DeleteSubject.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DeleteSubject : Window
    {
        private string subjectName;
        public DeleteSubject()
        {
            InitializeComponent();
        }
        public DeleteSubject(string subjectName)
        {
            InitializeComponent();
            this.subjectName = subjectName;
            SubjectName_Tbk.Text ="'"+ subjectName + "' 과목을";
        }
       

        public  void Cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Hide();
        }

        public void Confirm_btn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Hide();
        }
    }
}
