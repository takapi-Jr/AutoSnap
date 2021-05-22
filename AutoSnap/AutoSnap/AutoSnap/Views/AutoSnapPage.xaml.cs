using Xamarin.Forms;

namespace AutoSnap.Views
{
    public partial class AutoSnapPage : ContentPage
    {
        public AutoSnapPage()
        {
            InitializeComponent();

            this.Disappearing += (sender, e) =>
            {
                // 画面が非表示のときはプレビューを止める
                this.autoSnapView.IsPreviewing = false;
                base.OnDisappearing();
            };

            this.Appearing += (sender, e) =>
            {
                // 画面が表示されたらプレビューを開始する
                this.autoSnapView.IsPreviewing = true;
                base.OnAppearing();
            };
        }
    }
}
